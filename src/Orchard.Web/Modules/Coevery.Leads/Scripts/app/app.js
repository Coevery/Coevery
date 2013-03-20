var coevery = angular.module('coevery', ['localization', 'ngGrid', 'ngResource', 'ui.compat']).value('$anchorScroll', angular.noop);

coevery.config(function ($routeProvider, $stateProvider) {
    //$routeProvider.
    //    when('', { controller: 'LeadCtrl', templateUrl: '/OrchardLocal/Leads/Home/List' }).
    //    when('/Create', { controller: 'LeadDetailCtrl', templateUrl: '/OrchardLocal/Leads/Home/Detail' }).
    //    when('/:leadId', { controller: 'LeadDetailCtrl', templateUrl: '/OrchardLocal/Leads/Home/Detail' });

    function enterMenu() {
        var currentMenu = 'nav li>a[href*="' + this.url.match(/\/\w+/) + '"]';
        $(currentMenu).parent().addClass('active');
    }

    function exitMenu() {
        var currentMenu = 'nav li>a[href*="' + this.url.match(/\/\w+/) + '"]';
        $(currentMenu).parent().removeClass('active');
    }
    
    $stateProvider.
        state('leadList', {
            url: '/Lead',
            templateUrl: '/OrchardLocal/Leads/Home/List',
            //resolve: { Lead: 'Lead' },
            controller: 'LeadCtrl',
            onEnter: enterMenu,
            onExit: exitMenu
        }).
        state('leadCreate', {
            url: '/Lead/Create',
            templateUrl: '/OrchardLocal/Leads/Home/Detail',
            controller: 'LeadDetailCtrl',
            onEnter: enterMenu,
            onExit: exitMenu
        }).
        state('leadDetail', {
            url: '/Lead/{leadId:[0-9]+}',
            templateUrl: '/OrchardLocal/Leads/Home/Detail',
            controller: 'LeadDetailCtrl',
            onEnter: enterMenu,
            onExit: exitMenu
        }).
        state('opportunityList', {
            url: '/Opportunity',
            templateUrl: '/OrchardLocal/Opportunities/Home/List',
            controller: 'OpportunityCtrl',
            onEnter: enterMenu,
            onExit: exitMenu
        }).
        state('opportunityCreate', {
            url: '/Opportunity/Create',
            templateUrl: '/OrchardLocal/Opportunities/Home/Detail',
            controller: 'OpportunityDetailCtrl',
            onEnter: enterMenu,
            onExit: exitMenu
        }).
        state('opportunityDetail', {
            url: '/Opportunity/{opportunityId:[0-9]+}',
            templateUrl: '/OrchardLocal/Opportunities/Home/Detail',
            controller: 'OpportunityDetailCtrl',
            onEnter: enterMenu,
            onExit: exitMenu
        });
});