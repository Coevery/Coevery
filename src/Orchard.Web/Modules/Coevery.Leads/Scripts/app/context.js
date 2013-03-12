lead.factory('Lead', function ($resource) {
    return $resource(
        '/OrchardLocal/api/leads/lead/:leadId',
        { leadId: '@LeadId' },
        { update: { method: 'PUT' } });
});