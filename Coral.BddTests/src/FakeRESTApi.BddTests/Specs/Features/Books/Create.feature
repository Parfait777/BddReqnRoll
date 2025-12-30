Feature: Create a Book
	As a user of the Fake REST API
	I want to create a new book entry
	So that I can store information about books in the system

Background:
    Given the API base url is configured
    And I authenticate

@positive
Scenario: Create a new book entry
    When I POST "/api/v1/Books" with json
      """
      {
        "id": 0,
        "title": "BDD Test Book",
        "description": "Created by Reqnroll tests",
        "pageCount": 123,
        "excerpt": "Short excerpt",
        "publishDate": "2025-12-29T00:00:00Z"
      }
      """
    Then the response status should be 200
    And the response json should contain "id"
