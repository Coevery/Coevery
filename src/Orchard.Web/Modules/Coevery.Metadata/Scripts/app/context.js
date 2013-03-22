function MetadataContext($resource) {
    return $resource(
        '/OrchardLocal/api/metadata/metadata/:Name',
        { Name: '@Name' },
        { update: { method: 'PUT' } });
}

function FieldContext($resource) {
    return $resource(
        '/OrchardLocal/api/metadata/field/:Name',
        { Name: '@Name' },
        { update: { method: 'PUT' } });
}