metadata.factory('metadata', function ($resource) {
    return $resource(
        '/OrchardLocal/api/metadata/metadata/:Name',
        { Name: '@Name' },
        { update: { method: 'PUT' } });
});