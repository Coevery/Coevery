var metadata = angular.module('Metadata', ['localization','ngGrid', 'ngResource']).value('$anchorScroll', angular.noop);

metadata.config(function ($routeProvider) {
    $routeProvider.
        when('/List', { controller: 'MetadataCtrl', templateUrl: '/OrchardLocal/Metadata/Home/List' }).
        when('/Detail', { controller: 'MetadataDetailCtrl', templateUrl: '/OrchardLocal/Metadata/Home/Detail' }).
        //when('/Detail/:leadId', { controller: 'LeadDetailCtrl', templateUrl: '/OrchardLocal/Leads/Home/Detail' }).
        otherwise({ redirectTo: '/' });
});