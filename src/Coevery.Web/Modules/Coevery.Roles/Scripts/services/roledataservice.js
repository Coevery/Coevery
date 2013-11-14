'use strict';

define(['core/app/detourService'], function (detour) {
    detour.registerFactory([
        'roleDataService',
        ['$rootScope', '$resource', function($rootScope, $resource) {
            return $resource('api/roles/Role/:Id',
                { Id: '@Id' },
                { update: { method: 'PUT' } });
        }]
    ]);
});