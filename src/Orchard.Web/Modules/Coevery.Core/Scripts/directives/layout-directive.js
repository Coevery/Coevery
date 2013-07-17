angular.module('coevery.layout', [])
    .directive('fdSection', function () {
        return {
            template: '<fieldset fd-section class="data-section"><header><legend><h5>Section Title</h5></legend></header><div ng-transclude></div></fieldset>',
            replace: true,
            restrict: 'E',
            transclude: true,
            link: function (scope, element, attrs) {
                var sectionHeader = element.find('header h5');
                sectionHeader.text(attrs.sectionTitle);
            }
        };
    })
    .directive('fdRow', function () {
        return {
            template: '<div fd-row class="data-row clearfix" ng-transclude></div>',
            replace: true,
            restrict: 'E',
            transclude: true
        };
    })
    .directive('fdColumn', function () {
        return {
            template: '<div fd-column ng-transclude></div>',
            replace: true,
            restrict: 'E',
            transclude: true,
            link: function (scope, element, attrs) {
                var columnCount = parseInt(element.parents('[fd-section]:first').attr('section-columns'));
                var width = 12 / columnCount;
                element.addClass('span' + width);
            }
        };
    })
    .directive('fdField', function () {
        return {
            template: '<div fd-field class="control-group"></div>',
            replace: true,
            restrict: 'E',
            link: function (scope, element, attrs) {
                var template = $('script[type="text/ng-template"][id="' + attrs.fieldName + '.html"]');
                element.html(template.text());
            }
        };
    });
