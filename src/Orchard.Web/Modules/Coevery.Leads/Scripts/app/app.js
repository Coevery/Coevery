
var coevery = angular.module('coevery', ['ngGrid', 'ngResource', 'localization', 'ui.compat'])
    .value('$anchorScroll', angular.noop)
    .config(["$stateProvider", function($stateProvider) {

        $stateProvider
            .state('List', {
                url: '/{Moudle:[a-zA-Z]+}',
                templateUrl: function(params) {
                    return params.Moudle + '/ViewTemplate/List';
                }
            })
            .state('Create', {
                url: '/{Moudle:[a-zA-Z]+}/Create',
                templateUrl: function(params) {
                    return params.Moudle + '/ViewTemplate/Detail';
                }
            })
            .state('Detail', {
                url: '/{Moudle:[a-zA-Z]+}/{Id:[0-9a-zA-Z]+}',
                templateUrl: function(params) {
                    return params.Moudle + '/ViewTemplate/Detail';
                }
            })
            .state('SubView', {
                url: '/{Moudle:[a-zA-Z]+}/{Id:[0-9a-zA-Z]+}/{SubModule:[a-zA-Z]+}/{View:[a-zA-Z]+}',
                templateUrl: function(params) {
                    return params.Moudle + '/' + params.SubModule + 'ViewTemplate/' + params.View;
                }
            });
    }])
    .run(
        ['$rootScope', '$state', '$stateParams',
            function($rootScope, $state, $stateParams) {
                $rootScope.$state = $state;
            }]);
