var metadata = angular.module('metadata', ['localization','ngGrid', 'ngResource']).value('$anchorScroll', angular.noop);

metadata.config(function ($routeProvider) {
    $routeProvider.
        when('', { controller: 'MetadataCtrl', templateUrl: '/OrchardLocal/Metadata/Home/List' }).
        when('/Create', { controller: 'MetadataDetailCtrl', templateUrl: '/OrchardLocal/Metadata/Home/Detail' }).
        when('/FieldList/CreateField/:name', { controller: 'FieldDetailCtrl', templateUrl: '/OrchardLocal/Metadata/Home/Field' }).
        when('/FieldList/EditField/:name', { controller: 'FieldDetailCtrl', templateUrl: '/OrchardLocal/Metadata/Home/Field' }).
        when('/FieldList/:name', { controller: 'FieldCtrl', templateUrl: '/OrchardLocal/Metadata/Home/FieldList' }).
        when('/Edit/:name', { controller: 'MetadataDetailCtrl', templateUrl: '/OrchardLocal/Metadata/Home/Detail' }).
        otherwise({ redirectTo: '/' });
});