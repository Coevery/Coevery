var metadata = angular.module('metadata', ['localization','ngGrid', 'ngResource']).value('$anchorScroll', angular.noop);

metadata.config(function ($routeProvider) {
    $routeProvider.
        when('', { controller: 'MetadataCtrl', templateUrl: '/OrchardLocal/Metadata/Home/List' }).
        when('/Create', { controller: 'MetadataDetailCtrl', templateUrl: '/OrchardLocal/Metadata/Home/Detail' }).
        when('/:name', { controller: 'MetadataDetailCtrl', templateUrl: '/OrchardLocal/Metadata/Home/Detail' }).
        otherwise({ redirectTo: '/' });
});