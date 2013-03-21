
var coevery = angular.module('coevery', ['ngGrid', 'ngResource', 'localization', 'ui.compat'])
    .value('$anchorScroll', angular.noop)
    .config(["$stateProvider", function($stateProvider) {

        $stateProvider
            .state('List', {
                url: '/{Moudle:[a-zA-Z]+}',
                templateUrl: function(params) {
                    return '/OrchardLocal/' + params.Moudle + '/Home/List';
                }
            })
            .state('Create', {
                url: '/{Moudle:[a-zA-Z]+}/Create',
                templateUrl: function(params) {
                    return '/OrchardLocal/' + params.Moudle + '/Home/Detail';
                }
            }).
            state('Detail', {
                url: '/{Moudle:[a-zA-Z]+}/{Id:[0-9]+}',
                templateUrl: function(params) {
                    return '/OrchardLocal/' + params.Moudle + '/Home/Detail';
                }
            });
    }]);
