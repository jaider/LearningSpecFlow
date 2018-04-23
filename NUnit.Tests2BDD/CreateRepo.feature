Feature: I want to create a new repository
	Scenario: I need a new repository
		Given I am an authenticated user
		When I Create the "a1monkey" repository
		And I request a list of my repositories
		Then The results should include a repository name "a1monkey"
		And I delete the repository called "a1monkey"