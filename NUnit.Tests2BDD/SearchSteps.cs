using MyGiHub;
using NUnit.Framework;
using System;
using System.Net.Http;
using TechTalk.SpecFlow;

namespace NUnit.Tests2BDD
{
    [Binding]
    public class SearchSteps
    {
        private HttpResponseMessage response;
        private GitHubClient client;

        [Given(@"I am an anoymous user")]
        public void GivenIAmAnAnoymousUser()
        {
        }

        [When(@"I search for ""(.*)""")]
        public void WhenISearchFor(string arg)
        {
            client = new GitHubClient();
            response = client.Search(arg);
        }

        [Then(@"I expect a (.*) response code")]
        public void ThenIExpectAResponseCode(int responseCode)
        {
            if ((int)response.StatusCode != responseCode) {
                throw new Exception($"It didn't work. We exprected a {responseCode} response code but got a {response.StatusCode}");
            }
        }

        [Then(@"I expect at least (.*) result")]
        public void ThenIExpectAtLeastResult(int minTotal)
        {
            var result = client.MapResult<GitHubSummary>(response);
            Assert.GreaterOrEqual(result.total_count, minTotal, $"We expected at least {minTotal} results but found {result.total_count}");
        }
    }
}
