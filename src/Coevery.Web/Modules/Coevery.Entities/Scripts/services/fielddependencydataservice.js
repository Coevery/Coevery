'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerFactory([
        'fieldDependencyDataService',
        ['$resource', function ($resource) {
            return $resource('api/fields/FieldDependency',
                {},
                { update: { method: 'PUT' } });
        }]
    ]);
});