function CommonContext($rootScope, $resource) {
    var moduleName = $rootScope.$stateParams.Module;
    return $resource(
        'api/CoeveryCore/Common/:contentType:contentId',
        { contentType: moduleName },
        { contentId: '@ContentId' },
        { update: { method: 'PUT' } });
}

