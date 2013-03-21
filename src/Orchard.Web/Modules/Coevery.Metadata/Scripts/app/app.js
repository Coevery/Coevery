
coevery.config(function ($stateProvider) {
    
    function fieldListUrl($stateParams) {
        var url = '/OrchardLocal/Metadata/Home/FieldList';
        return url;
    }
    
    function fieldUrl($stateParams) {
        var url = '/OrchardLocal/Metadata/Home/Field';
        return url;
    }
    
    $stateProvider.
        state('FieldList', {
            url: '/{Moudle:[a-zA-Z]+}/FieldList/{name:[0-9]+}',
            templateUrl: fieldListUrl
        }).
        state('ManageField', {
            url: '/{Moudle:[a-zA-Z]+}/ManageField/{params:[*]+}',
            templateUrl: fieldUrl
        });
});