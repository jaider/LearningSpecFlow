using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using TechTalk.SpecFlow;

namespace NUnit.Tests2BDD
{
    [Binding]
    public class SearchSteps
    {
        private HttpResponseMessage response;

        [Given(@"I am an anoymous user")]
        public void GivenIAmAnAnoymousUser()
        {
        }

        [When(@"I search for ""(.*)""")]
        public void WhenISearchFor(string arg)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            var client = new HttpClient {
                BaseAddress = new Uri("https://api.github.com")
            };

            client.DefaultRequestHeaders.Add("User-Agent", "C# App");

            var task = client.GetAsync($"/search/repositories?q={arg}");

            task.Wait();
            response = task.Result;
        }

        [Then(@"I expect a (.*) response code")]
        public void ThenIExpectAResponseCode(int responseCode)
        {
            //response.EnsureSuccessStatusCode();
            if ((int)response.StatusCode != responseCode) {
                throw new Exception($"It didn't work. We exprected a {responseCode} response code but got a {response.StatusCode}");
            }
        }

        [Then(@"I expect at least (.*) result")]
        public void ThenIExpectAtLeastResult(int minTotal)
        {
            var contentTask = response.Content.ReadAsStringAsync();
            contentTask.Wait();
            var repositories = Newtonsoft.Json.JsonConvert.DeserializeObject<GitHubRepositories>(contentTask.Result);

            Assert.GreaterOrEqual(repositories.total_count, minTotal, $"We expected at least {minTotal} results but found {repositories.total_count}");
        }

    }
}
