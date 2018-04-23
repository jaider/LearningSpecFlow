Feature: This is an important repository
	Scenario: I want to know when something happens with this repository
	Given I am an authenticated user
	And I have a repository called "azure-quickstart-templates"
	When I watch the "azure-quickstart-templates" repository
	Then The "azure-quickstart-templates" repository will list me as a watcher