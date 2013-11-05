Feature: Setup
    In order to install coevery
    As a new user
    I want to setup a new site from the default screen

Scenario: Root request shows setup form
    Given I have a clean site with
            | extension | names |
            | Module | Coevery.Setup, Coevery.Pages, Coevery.ContentPicker, Coevery.Blogs, Coevery.Messaging, Coevery.MediaLibrary, Coevery.Modules, Coevery.Packaging, Coevery.PublishLater, Coevery.Themes, Coevery.Scripting, Coevery.Widgets, Coevery.Users, Coevery.ContentTypes, Coevery.Roles, Coevery.Comments, Coevery.jQuery, Coevery.Tags, TinyMce, Coevery.Recipes, Coevery.Warmup, Coevery.Alias, Coevery.Forms, Coevery.Tokens, Coevery.Autoroute, Coevery.Projections, Coevery.Fields, Coevery.MediaProcessing, Coevery.OutputCache, Coevery.Taxonomies, Coevery.Workflows, Coevery.Scripting.CSharp |
            | Core | Common, Containers, Dashboard, Feeds, Navigation, Contents, Scheduling, Settings, Shapes, XmlRpc, Title, Reports |
            | Theme | SafeMode |
    When I go to "/"
    Then I should see "Welcome to Coevery"
        And I should see "Finish Setup"
        And the status should be 200 "OK"

Scenario: Setup folder also shows setup form
    Given I have a clean site with
            | extension | names |
            | Module | Coevery.Setup, Coevery.Pages, Coevery.ContentPicker, Coevery.Blogs, Coevery.Messaging, Coevery.MediaLibrary, Coevery.Modules, Coevery.Packaging, Coevery.PublishLater, Coevery.Themes, Coevery.Scripting, Coevery.Widgets, Coevery.Users, Coevery.ContentTypes, Coevery.Roles, Coevery.Comments, Coevery.jQuery, Coevery.Tags, TinyMce, Coevery.Recipes, Coevery.Warmup, Coevery.Alias, Coevery.Forms, Coevery.Tokens, Coevery.Autoroute, Coevery.Projections, Coevery.Fields, Coevery.MediaProcessing, Coevery.OutputCache, Coevery.Taxonomies, Coevery.Workflows, Coevery.Scripting.CSharp |
            | Core | Common, Containers, Dashboard, Feeds, Navigation, Contents, Scheduling, Settings, Shapes, XmlRpc, Title, Reports |
            | Theme | SafeMode |
    When I go to "/Setup"
    Then I should see "Welcome to Coevery"
        And I should see "Finish Setup"
        And the status should be 200 "OK"

Scenario: Some of the initial form values are required
    Given I have a clean site with
            | extension | names |
            | Module | Coevery.Setup, Coevery.Pages, Coevery.ContentPicker, Coevery.Blogs, Coevery.Messaging, Coevery.MediaLibrary, Coevery.Modules, Coevery.Packaging, Coevery.PublishLater, Coevery.Themes, Coevery.Scripting, Coevery.Widgets, Coevery.Users, Coevery.ContentTypes, Coevery.Roles, Coevery.Comments, Coevery.jQuery, Coevery.Tags, TinyMce, Coevery.Recipes, Coevery.Warmup, Coevery.Alias, Coevery.Forms, Coevery.Tokens, Coevery.Autoroute, Coevery.Projections, Coevery.Fields, Coevery.MediaProcessing, Coevery.OutputCache, Coevery.Taxonomies, Coevery.Workflows, Coevery.Scripting.CSharp |
            | Core | Common, Containers, Dashboard, Feeds, Navigation, Contents, Scheduling, Settings, Shapes, XmlRpc, Title, Reports |
            | Theme | SafeMode |
    When I go to "/Setup"
        And I hit "Finish Setup"
    Then I should see "Site name is required."
        And I should see "Password is required."
        And I should see "Password confirmation is required."

Scenario: Calling setup on a brand new install
    Given I have a clean site with
            | extension | names |
            | Module | Coevery.Setup, Coevery.Pages, Coevery.ContentPicker, Coevery.Blogs, Coevery.Messaging, Coevery.MediaLibrary, Coevery.Modules, Coevery.Packaging, Coevery.PublishLater, Coevery.Themes, Coevery.Scripting, Coevery.Widgets, Coevery.Users, Coevery.ContentTypes, Coevery.Roles, Coevery.Comments, Coevery.jQuery, Coevery.Tags, TinyMce, Coevery.Recipes, Coevery.Warmup, Coevery.Alias, Coevery.Forms, Coevery.Tokens, Coevery.Autoroute, Coevery.Projections, Coevery.Fields, Coevery.MediaProcessing, Coevery.OutputCache, Coevery.Taxonomies, Coevery.Workflows, Coevery.Scripting.CSharp |
            | Core | Common, Containers, Dashboard, Feeds, Navigation, Contents, Scheduling, Settings, Shapes, XmlRpc, Title, Reports |
            | Theme | SafeMode, TheAdmin, TheThemeMachine |
        And I am on "/Setup"
    When I fill in 
            | name | value |
            | SiteName | My Site |
            | AdminPassword | 6655321 |
            | ConfirmPassword | 6655321 |
        And I hit "Finish Setup"
        And I go to "/"
    Then I should see "My Site"
        And I should see "Welcome, <strong><a href="/Users/Account/ChangePassword">admin</a></strong>!"
