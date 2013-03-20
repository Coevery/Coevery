coevery.factory('opportunity', function ($resource)
{
    return $resource(
        '/OrchardLocal/api/opportunities/opportunity/:opportunityId',
        { opportunityId: '@OpportunityId' },
        { update: { method: 'PUT' } });
});