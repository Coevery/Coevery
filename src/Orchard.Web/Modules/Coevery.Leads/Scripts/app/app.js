var lead = angular.module('lead', ['localization','ngGrid', 'ngResource']).value('$anchorScroll', angular.noop);

lead.config(function($routeProvider) {
    $routeProvider.
        when('', { controller: 'LeadCtrl', templateUrl: '/OrchardLocal/Leads/Home/List' }).
        when('/Create', { controller: 'LeadDetailCtrl', templateUrl: '/OrchardLocal/Leads/Home/Detail' }).
        when('/:leadId', { controller: 'LeadDetailCtrl', templateUrl: '/OrchardLocal/Leads/Home/Detail' }).
        otherwise({ redirectTo: '' });
});