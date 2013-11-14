'use strict';

define(['core/app/detourService'], function (detour) {
    detour.registerFactory([
        'userDataService',
        ['$rootScope', '$resource', function($rootScope, $resource) {
            return $resource('api/users/User/:Id',
                { Id: '@Id' },
                { update: { method: 'PUT' } });
        }]
    ]);
});