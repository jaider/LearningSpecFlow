Feature: Search Repositories
I want to get the list of the respositories that reference something
	Scenario: Searching Behat Respositories
		Given I am an anoymous user
		When I search for "behat"
		Then I expect a 200 response code
		And I expect at least 1 result