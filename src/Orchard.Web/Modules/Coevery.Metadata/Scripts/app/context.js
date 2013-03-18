metadata.factory('Metadata', function ($resource) {
    return $resource(
        '/OrchardLocal/api/leads/lead/:leadId',
        { leadId: '@LeadId' },
        { update: { method: 'PUT' } });
});