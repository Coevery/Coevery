'use strict';

define(['core/app/app'], function(app) {

    //This handles retrieving data and is used by controllers. 3 options (server, factory, provider) with 
    //each doing the same thing just structuring the functions/data differently.

    //Although this is an AngularJS factory I prefer the term "service" for data operations
    app.factory('commonDataService', ['$rootScope', '$resource', function($rootScope, $resource) {
        var moduleName = $rootScope.$stateParams.Module;
        return $resource('api/CoeveryCore/Common/:contentType:contentId',
            { contentType: moduleName },
            { contentId: '@ContentId' },
            { update: { method: 'PUT' } });
    }]);
});