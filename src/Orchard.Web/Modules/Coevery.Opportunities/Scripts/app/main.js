var opportunity = angular.module('opportunity', ['ngGrid', 'ngResource']).value('$anchorScroll', angular.noop);

opportunity.config(function ($routeProvider) {
    $routeProvider.
        when('/List', { controller: 'opportunityCtrl', templateUrl: '/OrchardLocal/opportunities/Home/List' }).
        when('/Detail', { controller: 'OpportunityDetailCtrl', templateUrl: '/OrchardLocal/Opportunities/Home/Detail' }).
        when('/Detail/:opportunityId', { controller: 'OpportunityDetailCtrl', templateUrl: '/OrchardLocal/Opportunities/Home/Detail' }).
        otherwise({ redirectTo: '/' });
});