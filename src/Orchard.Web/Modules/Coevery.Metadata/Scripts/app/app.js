

var metadataApp = angular.module('metadataApp', ['ngGrid', 'ngResource', 'ui.compat'])
    .value('$anchorScroll', angular.noop)
    .config(["$stateProvider", function ($stateProvider) {
        $stateProvider.
       state('FieldList', {
           url: '/{Moudle:[a-zA-Z]+}/FieldList/{name:[0-9a-zA-Z]+}',
           templateUrl: function (params) {
               return '/OrchardLocal/Metadata/Home/FieldList';
           }
       }).
       state('ManageField', {
           url: '/{Moudle:[a-zA-Z]+}/ManageField/{params:\\S*}',
           templateUrl: function (params) {
               return '/OrchardLocal/Metadata/Home/Field';
           }
       });
    }]);
