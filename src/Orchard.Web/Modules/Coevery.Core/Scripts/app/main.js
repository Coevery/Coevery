require.config({
    paths: {
        core: 'Modules/Coevery.Core/Scripts',
        app: 'Modules/Coevery.Core/Scripts/app/' + appPrefix() + 'app'
    }
});


require(['app', 'core/app/logger'], function (app) {
    'use strict';
    
    var config = requirejs.s.contexts._.config;
    
    angular.element(document).ready(function() {
        angular.bootstrap(document, ['coevery', function($locationProvider) {
            //$locationProvider.html5Mode(true).hashPrefix('!');
        }]);
    });
});
