'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerFactory([
        'optionItemsDataService',
        ['$resource', function ($resource) {
            return $resource('api/fields/OptionItems',
                {},
                { update: { method: 'PUT' } });
        }]
    ]);
});