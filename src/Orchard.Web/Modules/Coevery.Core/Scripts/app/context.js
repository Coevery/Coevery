function CommonContext($rootScope, $resource) {
    var moduleName = $rootScope.$stateParams.Module;
    return $resource(
        'api/CoeveryCore/Common/:contentId',
        { contentId: '@ContentId' },
        { update: { method: 'PUT' } });
}