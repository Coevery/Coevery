require.config({
    baseUrl: '/OrchardLocal',
    paths: {
        core: 'Modules/Coevery.Core/Scripts'
    }
});


require(['core/app/app', 'core/app/logger'], function (app) {
    'use strict';
    debugger;
    
    var config = requirejs.s.contexts._.config;
    
    angular.element(document).ready(function() {
        angular.bootstrap(document, ['coevery', function($locationProvider) {
            //$locationProvider.html5Mode(true).hashPrefix('!');
        }]);
    });
});
