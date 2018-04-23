using MyGiHub;
using NUnit.Framework;
using System;
using System.Net.Http;
using TechTalk.SpecFlow;
using System.Linq;

namespace NUnit.Tests2BDD
{
    [Binding]
    class FeaturesSteps
    {
        private HttpResponseMessage response;
        private GitHubClient client;

        [Given(@"I am an anoymous user")]
        public void GivenIAmAnAnoymousUser()
        {
            client = new GitHubClient(useAuthentication: false);
        }

        [When(@"I search for ""(.*)""")]
        public void WhenISearchFor(string arg)
        {
            response = client.Search(arg);
        }

        [Then(@"I expect a (.*) response code")]
        public void ThenIExpectAResponseCode(int responseCode)
        {
            Assert.AreEqual(responseCode, (int)response.StatusCode, "It didn't work");
        }

        [Then(@"I expect at least (.*) result")]
        public void ThenIExpectAtLeastResult(int minTotal)
        {
            var result = client.MapResult<GitHubSummary>(response);
            Assert.GreaterOrEqual(result.total_count, minTotal, $"We expected at least {minTotal} results but found {result.total_count}");
        }

        [Given(@"I am an authenticated user")]
        public void GivenIAmAnAuthenticatedUser()
        {
            client = new GitHubClient(useAuthentication: true);
            var response = client.TestAuth();
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Authentication didn't work!");
        }

        [When(@"I request a list of my repositories")]
        public void WhenIRequestAListOfMyRepositories()
        {
            response = client.UserRepos();
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Request Failed");
        }

        [Then(@"The results should include a repository name ""(.*)""")]
        public void ThenTheResultsShouldIncludeARepositoryName(string name)
        {
            var result = client.MapResult<GitHubRepositories[]>(response);
            var array = result.Select(repository => repository.name).ToArray();
            CollectionAssert.Contains(array, name, $"Expected to find a repository named '{name}' but didn't");
        }

        [When(@"I Create the ""(.*)"" repository")]
        public void WhenICreateTheRepository(string name)
        {
            var response = client.CreateRepo(name);
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode, "Request Failed");
        }
    }
}
