function CommonContext($rootScope, $resource) {
    var moduleName = $rootScope.$stateParams.Module;
    return $resource(
        'api/CoeveryCore/' + moduleName + '/:contentId',
        { contentId: '@ContentId' },
        { update: { method: 'PUT' } });
}