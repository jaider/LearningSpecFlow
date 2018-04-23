using System;
using TechTalk.SpecFlow;

namespace NUnit.Tests2BDD
{
    [Binding]
    public class GetMyRepositoriesSteps
    {
        [Given(@"I am an authenticated user")]
        public void GivenIAmAnAuthenticatedUser()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"I request a list of my repositories")]
        public void WhenIRequestAListOfMyRepositories()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"The results should include a repository name ""(.*)""")]
        public void ThenTheResultsShouldIncludeARepositoryName(string p0)
        {
            ScenarioContext.Current.Pending();
        }
    }
}
