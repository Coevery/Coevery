Feature: The Admin side of the app
    In order to manage my site
    As a privileged user
    I want to not have my cheese moved in the admin

Scenario: The current version of Coevery is displayed in the admin
    Given I have installed Coevery
    When I go to "admin"
    Then I should see "<div id="coevery-version">Coevery v(?:\.\d+){2,4}</div>"
