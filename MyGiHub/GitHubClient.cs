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

        public GitHubClient(bool useAuthentication = false)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            _client = new HttpClient {
                BaseAddress = new Uri("https://api.github.com")
            };
            _client.DefaultRequestHeaders.Add("User-Agent", "C# App");

            if (useAuthentication) {
                var username = ConfigurationManager.AppSettings["GitHubUsername"];
                var password = ConfigurationManager.AppSettings["GitHubPassword"];
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password)) {
                    var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
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
