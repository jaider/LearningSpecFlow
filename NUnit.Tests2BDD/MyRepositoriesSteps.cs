using MyGiHub;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net.Http;
using TechTalk.SpecFlow;

namespace NUnit.Tests2BDD
{
    [Binding]
    public class GetMyRepositoriesSteps
    {
        private GitHubClient client;
        private HttpResponseMessage response;

        [Given(@"I am an authenticated user")]
        public void GivenIAmAnAuthenticatedUser()
        {
            client = new GitHubClient(true);
            var response = client.TestAuth();
            if(response.StatusCode != System.Net.HttpStatusCode.OK) {
                throw new Exception("Authentication didn't work!");
            }
        }
        
        [When(@"I request a list of my repositories")]
        public void WhenIRequestAListOfMyRepositories()
        {
            response = client.UserRepos();
            if (response.StatusCode != System.Net.HttpStatusCode.OK) {
                throw new Exception("Request Failed");
            }
        }
        
        [Then(@"The results should include a repository name ""(.*)""")]
        public void ThenTheResultsShouldIncludeARepositoryName(string name)
        {
            var result = client.MapResult<GitHubRepositories[]>(response);
            var array = result.Select(repository => repository.name).ToArray();
            CollectionAssert.Contains(array, name, $"Expected to find a repository named '{name}' but didn't");
        }
    }
}
