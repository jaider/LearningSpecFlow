using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace MyGiHub
{
    public class GitHubClient
    {
        HttpClient _client;

        string Username { get; } = ConfigurationManager.AppSettings["GitHubUsername"];
        string Password { get; } = ConfigurationManager.AppSettings["GitHubPassword"];

        public GitHubClient(bool useAuthentication = false)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            _client = new HttpClient {
                BaseAddress = new Uri("https://api.github.com")
            };
            _client.DefaultRequestHeaders.Add("User-Agent", "C# App");

            if (useAuthentication) {
                if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password)) {
                    var byteArray = Encoding.ASCII.GetBytes($"{Username}:{Password}");
                    var parameter = System.Convert.ToBase64String(byteArray);
                    _client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Basic", parameter);
                }
                else {
                    throw new Exception("Please setup GitHub credentials");
                }
            }
        }

        public HttpResponseMessage Search(string arg) => Get($"/search/repositories?q={arg}");

        public HttpResponseMessage UserRepos() => Get("/user/repos");

        public HttpResponseMessage TestAuth() => Get("/");

        //https://developer.github.com/v3/activity/watching/#list-watchers
        //https://developer.github.com/v3/repos/#list-your-repositories
        public HttpResponseMessage GetWatchers(string repo) => Get($"/repos/{Username}/{repo}/subscribers");

        public HttpResponseMessage CreateRepo(string name)
        {
            var body = new StringContent("{\"name\": \"" + name + "\"}");
            var task = _client.PostAsync("/user/repos", body);
            task.Wait();
            return task.Result;
        }

        public HttpResponseMessage DeleteRepo(string name)
        {
            var task = _client.DeleteAsync($"/repos/{Username}/{name}");
            task.Wait();
            return task.Result;
        }

        public HttpResponseMessage WatchRepo(string repo)
        {
            var body = new StringContent("{\"subscribed\": true}");
            var owner = this.Username;
            var task = _client.PutAsync($"/repos/{owner}/{repo}/subscription", body);
            task.Wait();
            return task.Result;
        }

        public T MapResult<T>(HttpResponseMessage response)
        {
            var contentTask = response.Content.ReadAsStringAsync();
            contentTask.Wait();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(contentTask.Result);
        }

        //use path like: "/", or "/user/repos"
        private HttpResponseMessage Get(string path)
        {
            var task = _client.GetAsync(path);
            task.Wait();
            return task.Result;
        }
    }
}
