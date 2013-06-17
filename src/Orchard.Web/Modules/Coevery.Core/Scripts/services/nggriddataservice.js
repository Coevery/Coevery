'use strict';

define(['core/app/couchPotatoService'], function(couchPotato) {

    //This handles retrieving data and is used by controllers. 3 options (server, factory, provider) with 
    //each doing the same thing just structuring the functions/data differently.

    //Although this is an AngularJS factory I prefer the term "service" for data operations
    couchPotato.registerFactory([
       'ngGridDataService',
       ['$rootScope', '$resource', function ($rootScope, $resource) {
           var moduleName = $rootScope.$stateParams.Module;
           return $resource('api/CoeveryCore/NGgrid/:contentType:contentId',
               { contentType: moduleName },
               { contentId: '@ContentId' },
               { update: { method: 'PUT' } });
       }]
    ]);
});