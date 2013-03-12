opportunity.factory('opportunity', function ($resource)
{
    return $resource(
        '/OrchardLocal/api/opportunities/opportunity/:opportunityId',
        { opportunityId: '@opportunityId' },
        { update: { method: 'PUT' } });
});