angular.module('coevery.layout', ['ng'])
    .directive('fdSection', function () {
        return {
            template: function (element) {
                return $('.edit-mode:first').length
                    ? '<fieldset fd-section class="data-section"><legend class="title">Section Title</legend><div ng-transclude></div></fieldset>'
                    : '<section fd-section><header><span class="show-button"></span><h5 class="title">General Info</h5></header><div ng-transclude></div></section>';
            },
            replace: true,
            restrict: 'E',
            transclude: true,
            link: function (scope, element, attrs) {
                var sectionHeader = element.find('.title:first');
                sectionHeader.text(attrs.sectionTitle);
            }
        };
    })
    .directive('fdRow', function () {
        return {
            template: '<div fd-row class="data-row clearfix" ng-transclude></div>',
            //replace: true,
            restrict: 'E',
            transclude: true,
            link: function (scope, element, attrs) {
                element.find('[fd-field]:not(:empty)').length || element.remove();
            }
        };
    })
    .directive('fdColumn', function () {
        return {
            template: '<div fd-column ng-transclude></div>',
            replace: true,
            restrict: 'E',
            transclude: true,
            link: function (scope, element, attrs) {
                var row = element.parent(),
                    width;
                if (row.hasClass('merged-row')
                    || row.parents('[fd-section]').attr('section-columns') == 1) {
                    width = 12;
                } else {
                    var widths = $.map(row.parents('[fd-section]:first').attr('section-columns-width').split(':'), function (n) {
                        return parseInt(n);
                    });

                    width = widths[row.children('[fd-column]').index(element)];
                }
                element.addClass('span' + width);
            }
        };
    })
    .directive('fdField', function ($compile) {
        return {
            template: function (element) {
                return $('.edit-mode:first').length
                    ? '<div fd-field class="control-group"></div>'
                    : '<div fd-field></div>';
            },
            replace: true,
            restrict: 'E',
            link: function (scope, element, attrs) {
                var template = $('script[type="text/ng-template"][id="' + attrs.fieldName + '.html"]');
                element.html(template.text());
                $compile(element.children())(scope);
            }
        };
    })
    .directive('btnActions', function () {
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
