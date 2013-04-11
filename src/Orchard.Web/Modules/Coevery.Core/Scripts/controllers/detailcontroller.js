//angular.module('coevery.layout', [])
//    .directive('fdSection', function () {
//        return {
//            template: '<div fd-section class="row-fluid"><section class="span12 widget"><header class="widget-header light"><span class="title">&nbsp;</span></header><section class="widget-content form-container"><form class="form-horizontal" ng-transclude></form></section></section></div>',
//            replace: true,
//            restrict: 'E',
//            transclude: true
//        };
//    })
//    .directive('fdRow', function () {
//        return {
//            template: '<div fd-row class="row-fluid" ng-transclude></div>',
//            replace: true,
//            restrict: 'E',
//            transclude: true
//        };
//    })
//    .directive('fdColumn', function () {
//        return {
//            template: '<div fd-column ng-transclude></div>',
//            replace: true,
//            restrict: 'E',
//            transclude: true,
//            link: function (scope, element, attrs) {
//                var columnCount = parseInt(element.parents('[fd-section]:first').attr('section-columns'));
//                var width = 12 / columnCount;
//                element.addClass('span' + width);
//            }
//        };
//    })
//    .directive('fdField', function () {
//        return {
//            template: '<div fd-field></div>',
//            replace: true,
//            restrict: 'E',
//            link: function (scope, element, attrs) {
//                var template = $('script[type="text/ng-template"][id="' + attrs.fieldName + '.html"]');
//                element.html(template.text());
//            }
//        };
//    });

//angular.bootstrap($('[ng-app]'), ['coevery', 'coevery.layout']);

CommonDetailCtrl.$inject = ['$rootScope', '$scope', 'logger', '$state', '$stateParams', '$resource'];

function CommonDetailCtrl($rootScope, $scope, logger, $state, $stateParams, $resource) {
    var moduleName = $rootScope.$stateParams.Module;
    var module = CommonContext($rootScope, $resource);
    var id = $stateParams.Id;
    var isNew = id ? false : true;

    $scope.save = function () {
        $.ajax({
            url: myForm.action,
            type: myForm.method,
            data: $(myForm).serialize() + '&submit.Save=Save',
            success: function (result) {
                $state.transitionTo('List', { Module: moduleName });
            }
        });
    };

    $scope.change = function () {

    };

    $scope.exit = function () {
        $state.transitionTo('List', { Module: moduleName });
    };

    //if (!isNew) {
    //    var lead = module.get({ leadId: id }, function() {
    //        $scope.item = lead;
    //    }, function() {
    //        logger.error("The lead does not exist.");
    //    });
    //} else {
    //    $scope.item = new module();
    //}
}

//@ sourceURL=Coevery.Core/detailcontroller.js