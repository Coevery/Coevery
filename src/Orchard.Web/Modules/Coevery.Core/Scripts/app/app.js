
var coevery = angular.module('coevery', ['ngGrid', 'ngResource', 'localization', 'ui.compat'])
    .value('$anchorScroll', angular.noop)
    .config(["$stateProvider", function($stateProvider) {

        $stateProvider
            .state('List', {
                url: '/{Module:[a-zA-Z]+}',
                templateUrl: function(params) {
                    debugger;
                    if (params.Module == 'Metadata'){
                        return params.Module + '/ViewTemplate/List';
                    } else{
                        return 'CoeveryCore/ContentViewTemplate/List/' + params.Module;
                    }
                }
            })
            .state('Create', {
                url: '/{Module:[a-zA-Z]+}/Create',
                templateUrl: function (params) {
                    debugger;
                    if (params.Module == 'Metadata'){
                        return params.Module + '/ViewTemplate/Create';
                    } else{
                        return 'CoeveryCore/ContentViewTemplate/Create/' + params.Module;
                    }
                }
            })
            .state('Detail', {
                url: '/{Module:[a-zA-Z]+}/{Id:[0-9a-zA-Z]+}',
                templateUrl: function (params) {
                    debugger;
                    if (params.Module =='Metadata'){
                        return params.Module + '/ViewTemplate/Edit/' + params.Id;
                    } else{
                        return 'CoeveryCore/ContentViewTemplate/Edit/' + params.Id;
                    }
                    
                }
            })
            .state('SubList', {
                url: '/{Module:[a-zA-Z]+}/{Id:[0-9a-zA-Z]+}/{SubModule:[a-zA-Z]+}/{View:[a-zA-Z]+}',
                templateUrl: function(params) {
                    return params.Module + '/' + params.SubModule + 'ViewTemplate/' + params.View;
                }
            })
            .state('SubCreate', {
                url: '/{Module:[a-zA-Z]+}/{Id:[0-9a-zA-Z]+}/{SubModule:[a-zA-Z]+}/{View:[a-zA-Z]+}/Create',
                templateUrl: function(params) {
                    return params.Module + '/' + params.SubModule + 'ViewTemplate/' + params.View;
                }
            })
            .state('SubDetail', {
                url: '/{Module:[a-zA-Z]+}/{Id:[0-9a-zA-Z]+}/{SubModule:[a-zA-Z]+}/{View:[a-zA-Z]+}/{SubId:[0-9a-zA-Z]+}',
                templateUrl: function(params) {
                    return params.Module + '/' + params.SubModule + 'ViewTemplate/' + params.View;
                }
            });
    }])
    .run(
        ['$rootScope', '$state', '$stateParams',
            function($rootScope, $state, $stateParams) {
                $rootScope.$state = $state;
                $rootScope.$stateParams = $stateParams;
            }]);

$(function() {
    $('body').on("submit", 'form', function(event) {
        event.preventDefault();
    });
});
