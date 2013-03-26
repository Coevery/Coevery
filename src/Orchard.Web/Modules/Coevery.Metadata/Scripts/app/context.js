function MetadataContext($resource) {
    return $resource(
        'api/metadata/metadata/:Name',
        { Name: '@Name' },
        { update: { method: 'PUT' } });
}

function FieldContext($resource) {
    return $resource(
        'api/metadata/field/:Name',
        { Name: '@Name' },
        { update: { method: 'PUT' } });
}

function GenerateContext($resource) {
    return $resource(
        'api/metadata/metadata/:Name',
        { Name: '@Name' },
        { update: { method: 'PUT' } });
}