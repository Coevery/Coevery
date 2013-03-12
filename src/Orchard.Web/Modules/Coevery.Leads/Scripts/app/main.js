var lead = angular.module('lead', ['ngGrid', 'ngResource']).value('$anchorScroll', angular.noop);

lead.config(function ($routeProvider) {
    $routeProvider.
        when('/List', { controller: 'LeadCtrl', templateUrl: '/OrchardLocal/Leads/List' }).
        when('/Detail', { controller: 'LeadDetailCtrl', templateUrl: '/OrchardLocal/Leads/Detail' }).
        when('/Detail/:leadId', { controller: 'LeadDetailCtrl', templateUrl: '/OrchardLocal/Leads/Detail' }).
        otherwise({ redirectTo: '/' });
});