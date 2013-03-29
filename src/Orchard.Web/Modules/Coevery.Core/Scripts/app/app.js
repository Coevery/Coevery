
var coevery = angular.module('coevery', ['ngGrid', 'ngResource', 'localization', 'ui.compat'])
    .value('$anchorScroll', angular.noop)
    .config(["$stateProvider", function($stateProvider) {

        $stateProvider
            .state('List', {
                url: '/{Moudle:[a-zA-Z]+}',
                templateUrl: function(params) {
                    //return params.Moudle + '/ViewTemplate/List';
                    return 'CoeveryCore/ContentViewTemplate/List/' + params.Moudle;
                }
            })
            .state('Create', {
                url: '/{Moudle:[a-zA-Z]+}/Create',
                templateUrl: function(params) {
                    // return params.Moudle + '/ViewTemplate/Detail';
                    return 'CoeveryCore/ContentViewTemplate/Detail/' + params.Moudle;
                }
            })
            .state('Detail', {
                url: '/{Moudle:[a-zA-Z]+}/{Id:[0-9a-zA-Z]+}',
                templateUrl: function(params) {
                    // return params.Moudle + '/ViewTemplate/Detail';
                    return 'CoeveryCore/ContentViewTemplate/Detail/'+ params.Moudle;
                }
            })
            .state('SubList', {
                url: '/{Moudle:[a-zA-Z]+}/{Id:[0-9a-zA-Z]+}/{SubModule:[a-zA-Z]+}/{View:[a-zA-Z]+}',
                templateUrl: function(params) {
                    return params.Moudle + '/' + params.SubModule + 'ViewTemplate/' + params.View;
                }
            })
            .state('SubCreate', {
                url: '/{Moudle:[a-zA-Z]+}/{Id:[0-9a-zA-Z]+}/{SubModule:[a-zA-Z]+}/{View:[a-zA-Z]+}/Create',
                templateUrl: function(params) {
                    return params.Moudle + '/' + params.SubModule + 'ViewTemplate/' + params.View;
                }
            })
            .state('SubDetail', {
                url: '/{Moudle:[a-zA-Z]+}/{Id:[0-9a-zA-Z]+}/{SubModule:[a-zA-Z]+}/{View:[a-zA-Z]+}/{SubId:[0-9a-zA-Z]+}',
                templateUrl: function(params) {
                    return params.Moudle + '/' + params.SubModule + 'ViewTemplate/' + params.View;
                }
            });
    }])
    .run(
        ['$rootScope', '$state', '$stateParams',
            function($rootScope, $state, $stateParams) {
                $rootScope.$state = $state;
                $rootScope.$stateParams = $stateParams;
            }]);
