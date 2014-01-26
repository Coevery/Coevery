'use strict';

define(['core/app/detourService'], function (detour) {
    detour.registerFactory([
        'optionItemDataService',
        ['$rootScope', '$resource', function ($rootScope, $resource) {
            return $resource('api/OptionSet/OptionItem/:OptionSetId',
                { OptionSetId: '@OptionSetId' },
                { update: { method: 'PUT' } });
        }]
    ]);
});