var coevery = angular.module('coevery', ['localization', 'ngGrid', 'ngResource', 'ui.compat']).value('$anchorScroll', angular.noop);

coevery.config(function ($stateProvider) {
    function enterMenu($stateParams) {
        var currentMenu = 'nav li>a[href$="' + $stateParams.Moudle + '"]';
        $(currentMenu).parent().addClass('active');
    }

    function exitMenu($stateParams) {
        var currentMenu = 'nav li>a[href$="' + $stateParams.Moudle + '"]';
        $(currentMenu).parent().removeClass('active');
    }

    function listUrl($stateParams) {
        var url = '/OrchardLocal/' + $stateParams.Moudle + '/Home/List';
        return url;
    }
    
    function detailUrl($stateParams) {
        var url = '/OrchardLocal/' + $stateParams.Moudle + '/Home/Detail';
        return url;
    }

    $stateProvider.
        state('List', {
            url: '/{Moudle:[a-zA-Z]+}',
            templateUrl: listUrl,
            onEnter: enterMenu,
            onExit: exitMenu
        }).
        state('Create', {
            url: '/{Moudle:[a-zA-Z]+}/Create',
            templateUrl: detailUrl,
            onEnter: enterMenu,
            onExit: exitMenu
        }).
        state('Detail', {
            url: '/{Moudle:[a-zA-Z]+}/{Id:[0-9]+}',
            templateUrl: detailUrl,
            onEnter: enterMenu,
            onExit: exitMenu
        });
});