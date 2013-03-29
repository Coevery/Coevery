function CommonContext($rootScope, $resource) {
    var moduleName = $rootScope.$stateParams.Module;
    return $resource(
        'api/' + moduleName + '/' + moduleName + '/:contentId',
        { contentId: '@ContentId' },
        { update: { method: 'PUT' } });
}