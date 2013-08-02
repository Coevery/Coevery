angular.module('coevery.layout', [])
    .directive('fdSection', function () {
        return {
            template: '<fieldset fd-section class="data-section"><legend>Section Title</legend><div ng-transclude></div></fieldset>',
            replace: true,
            restrict: 'E',
            transclude: true,
            link: function (scope, element, attrs) {
                var sectionHeader = element.find('legend');
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
    })
    .directive('btnActions',function() {
        return {
            template: '<div btn-actions ng-transclude ng-style="BtnActionLeft()" class="btn-toolbar pull-left"></div>',
            replace: true,
            scope: {},
            restrict: 'E',
            transclude: true,
            controller: ['$scope', '$element', '$attrs', '$transclude', '$window', function ($scope, $element, $attrs, $transclude, $window) {
                $scope.BtnActionLeft = function () {
                    if ($element.hasClass('btn-fixed')) {
                        var left = $('#page-title>h1').offset().left;
                        var width = $('#page-title>h1').width();
                        btnLeft = left + width + 20;
                        return { left: btnLeft };
                    } else {
                        return { left: 'auto' };
                    }
                };
                angular.element($window).bind("scroll", function () {
                    var scrollTop = $(window).scrollTop();
                    if (scrollTop > 35) {
                        $element.addClass('btn-fixed');
                    } else {
                        $element.removeClass('btn-fixed');
                    }
                    $element.css('left', $scope.BtnActionLeft().left);
                });
            }]
        };
    });
