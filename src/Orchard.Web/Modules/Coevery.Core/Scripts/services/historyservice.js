'use strict';

define(['core/app/couchPotatoService'], function (couchPotato) {

    //This handles retrieving data and is used by controllers. 3 options (server, factory, provider) with 
    //each doing the same thing just structuring the functions/data differently.

    //Although this is an AngularJS factory I prefer the term "service" for data operations
    couchPotato.registerFactory([
       'historyService',
       ['$rootScope', '$resource', function ($rootScope, $resource) {
           return $resource('api/CoeveryCore/History/:contentId',
               { contentId: '@ContentId' },
               { update: { method: 'PUT' } });
       }]
    ]);
});