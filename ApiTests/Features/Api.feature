Feature: Public API smoke checks
  As a test engineer
  I want to validate a public API
  So that I can assert contract and data quickly

  @api @Jira-200
  Scenario: GET a post from JSONPlaceholder
    Given the API base url is default
    When I GET "/posts/1"
    Then the response status should be 200
    And the response should contain JSON with "userId" and "id" and "title"
