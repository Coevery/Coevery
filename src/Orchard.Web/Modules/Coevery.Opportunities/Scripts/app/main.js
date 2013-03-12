var opportunity = angular.module('opportunity', ['ngGrid', 'ngResource']).value('$anchorScroll', angular.noop);

opportunity.config(function ($routeProvider) {
    $routeProvider.
        when('/List', { controller: 'opportunityCtrl', templateUrl: '/OrchardLocal/opportunities/List' }).
        when('/Detail', { controller: 'opportunityDetailCtrl', templateUrl: '/OrchardLocal/Opportunities/Detail' }).
        when('/Detail/:opportunityId', { controller: 'opportunityDetailCtrl', templateUrl: '/OrchardLocal/Opportunities/Detail' }).
        otherwise({ redirectTo: '/' });
});