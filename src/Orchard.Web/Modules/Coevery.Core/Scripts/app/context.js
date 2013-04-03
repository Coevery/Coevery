function CommonContext($rootScope, $resource) {
    var moduleName = $rootScope.$stateParams.Module;
    return $resource(
        'api/CoeveryCore/Common/:contentType',
        { contentType: moduleName },
        { update: { method: 'PUT' } });
}