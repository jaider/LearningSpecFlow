using System;
using System.Net;
using System.Net.Http;

namespace MyGiHub
{
    public class GitHubClient
    {
        public HttpResponseMessage Search(string arg)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var client = new HttpClient {
                BaseAddress = new Uri("https://api.github.com")
            };

            client.DefaultRequestHeaders.Add("User-Agent", "C# App");

            var task = client.GetAsync($"/search/repositories?q={arg}");

            task.Wait();
            return task.Result;
        }

        public GitHubRepositories Convert(HttpResponseMessage response)
        {
            var contentTask = response.Content.ReadAsStringAsync();
            contentTask.Wait();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<GitHubRepositories>(contentTask.Result);
        }
    }
}
