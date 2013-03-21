
coevery.config(function ($stateProvider) {
    
    function enterMenu($stateParams) {
        var currentMenu = 'nav li>a[href$="' + $stateParams.Moudle + '"]';
        $(currentMenu).parent().addClass('active');
        var childs = $(currentMenu).parent().parent().parent().parent().children();
        for (var childIndex = 0; childIndex < childs.length; childIndex++) {
            childs[childIndex].className = '';
        }
        $(currentMenu).parent().parent().parent().addClass('active');
    }

    function exitMenu($stateParams) {
        var currentMenu = 'nav li>a[href$="' + $stateParams.Moudle + '"]';
        $(currentMenu).parent().removeClass('active');
    }
    
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
            url: '/{Moudle:[a-zA-Z]+}/FieldList/{name:[0-9a-zA-Z]+}',
            templateUrl: fieldListUrl,
            onEnter: enterMenu,
            onExit: exitMenu
        }).
        state('ManageField', {
            url: '/{Moudle:[a-zA-Z]+}/ManageField/{params:\\S*}',
            templateUrl: fieldUrl,
            onEnter: enterMenu,
            onExit: exitMenu
        });
});