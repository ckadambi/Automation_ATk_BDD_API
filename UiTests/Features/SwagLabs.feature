Feature: Swag Labs shopping
  As a shopper
  I want to sign in and add items to cart
  So that I can purchase them later

  @realapp1 @Jira-100
  Scenario: Successful login and add an item to cart
    Given I am on the Swag Labs login page
    When I log in with username "standard_user" and password "secret_sauce"
    Then I should see the inventory page
    When I add "Sauce Labs Backpack" to the cart
    Then the cart count should be 1
    When I open the cart
    Then I should see "Sauce Labs Backpack" in the cart

  @realapp2 @Jira-101
  Scenario: Failed login shows an error
    Given I am on the Swag Labs login page
    When I log in with username "locked_out_user" and password "secret_sauce"
    Then I should see a login error
