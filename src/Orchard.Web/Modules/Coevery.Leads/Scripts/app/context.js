//coevery.factory('Lead', function ($resource) {
//    return $resource(
//        '/OrchardLocal/api/leads/lead/:leadId',
//        { leadId: '@LeadId' },
//        { update: { method: 'PUT' } });
//});

function LeadContext($resource) {
    return $resource(
        'api/leads/lead/:leadId',
        { leadId: '@LeadId' },
        { update: { method: 'PUT' } });
}