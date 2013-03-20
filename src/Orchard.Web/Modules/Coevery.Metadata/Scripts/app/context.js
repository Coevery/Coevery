metadata.factory('metadata', function ($resource) {
    return $resource(
        '/OrchardLocal/api/metadata/metadata/:Name',
        { Name: '@Name' },
        { update: { method: 'PUT' } });
});

metadata.factory('field', function ($resource) {
    return $resource(
        '/OrchardLocal/api/metadata/field/:Name',
        { Name: '@Name' },
        { update: { method: 'PUT' } });
});