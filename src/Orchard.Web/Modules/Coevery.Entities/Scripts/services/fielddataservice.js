'use strict';

define(['core/app/couchPotatoService'], function (couchPotato) {
    couchPotato.registerFactory([
        'fieldDataService',
        ['$rootScope', '$resource', function ($rootScope, $resource) {
            return $resource('api/entities/field/:Name',
                { Name: '@Name' },
                { update: { method: 'PUT' } });
        }]
    ]);
});