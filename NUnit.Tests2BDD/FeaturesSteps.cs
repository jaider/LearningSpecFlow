using MyGiHub;
using NUnit.Framework;
using System;
using System.Net.Http;
using TechTalk.SpecFlow;
using System.Linq;
using System.Collections.Generic;

namespace NUnit.Tests2BDD
{
    [Binding]
    class FeaturesSteps
    {
        private HttpResponseMessage response;
        private GitHubClient client;
        private TableRows tableRows;

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
        public void ThenTheResultsShouldIncludeARepositoryName(string repoName)
        {
            var result = client.MapResult<GitHubRepository[]>(response);
            var array = result.Select(repository => repository.name).ToArray();
            CollectionAssert.Contains(array, repoName, $"Expected to find a repository named '{repoName}' but didn't");
        }

        [When(@"I Create the ""(.*)"" repository")]
        public void WhenICreateTheRepository(string repoName)
        {
            var response = client.CreateRepo(repoName);
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode, "Request Failed");
        }

        [Then(@"I delete the repository called ""(.*)""")]
        public void ThenIDeleteTheRepositoryCalled(string repoName)
        {
            var response = client.DeleteRepo(repoName);
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, response.StatusCode, "Request Failed");
        }

        [Given(@"I have a repository called ""(.*)""")]
        public void GivenIHaveARepositoryCalled(string repoName)
        {
            response = client.UserRepos();

            var result = client.MapResult<GitHubRepository[]>(response);
            var array = result.Select(repository => repository.name).ToArray();
            CollectionAssert.Contains(array, repoName, $"Expected to find a repository named '{repoName}' but didn't");
        }

        [When(@"I watch the ""(.*)"" repository")]
        public void WhenIWatchTheRepository(string repoName)
        {
            response = client.WatchRepo(client.Username, repoName);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Request Failed");
        }

        [Then(@"The ""(.*)"" repository will list me as a watcher")]
        public void ThenTheRepositoryWillListMeAsAWatcher(string repoName)
        {
            response = client.GetWatchersByRepo(client.Username, repoName);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Request Failed");
            var result = client.MapResult<GitHubWatcher[]>(response);
            var watchers = result.Select(w => w.login).ToArray();
            CollectionAssert.Contains(watchers, client.Username, $"Expected to find me as watcher but didn't");
        }

        [Given(@"I have the following repositories:")]
        public void GivenIHaveTheFollowingRepositories(Table table)
        {
            tableRows = table.Rows;
            foreach (var row in tableRows) {
                var response = client.GetRepo(row[0], row[1]);
                response.EnsureSuccessStatusCode();
            }
        }

        [When(@"I watch each repository")]
        public void WhenIWatchEachRepository()
        {
            foreach (var row in tableRows) {
                var response = client.WatchRepo(row[0], row[1]);
                response.EnsureSuccessStatusCode();
            }
        }

        [Then(@"My watch list will include those repositories")]
        public void ThenMyWatchListWillIncludeThoseRepositories()
        {
            var response = client.GetWatchedRepoByUser(client.Username);
            response.EnsureSuccessStatusCode();
            var result = client.MapResult<GitHubRepository[]>(response);
            var watchList = result.Select(w => w.full_name).ToArray();
            foreach (var row in tableRows) {
                var fullname = $"{row[0]}/{row[1]}";
                CollectionAssert.Contains(watchList, fullname);
            }
        }

    }
}
