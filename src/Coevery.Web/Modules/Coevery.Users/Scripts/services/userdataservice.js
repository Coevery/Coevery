'use strict';

define(['core/app/detourService'], function (detour) {
    detour.registerFactory([
        'userdataserviceDataService',
        ['$rootScope', '$resource', function($rootScope, $resource) {
            return $resource('api/users/User/:Id',
                { Id: '@Id' },
                { update: { method: 'PUT' } });
        }]
    ]);
});