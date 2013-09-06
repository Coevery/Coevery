'use strict';

define(['core/app/detourService'], function (detour) {
    detour.registerFactory([
        'optionItemDataService',
        ['$rootScope', '$resource', function ($rootScope, $resource) {
            return $resource('api/OptionSet/OptionItem/:OptionSetId',
                { OptionSetId1: '@optionSetId1' },
                { update: { method: 'PUT' } });
        }]
    ]);
});