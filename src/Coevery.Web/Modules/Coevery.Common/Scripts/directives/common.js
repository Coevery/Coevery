angular.module('coevery.common', [])
    .directive("coDeleteButton", ['$compile', '$parse', function($compile, $parse) {
        return {
            restrict: "A",
            scope: { confirmMessage: '@confirmMessage', deleteAction: '@deleteAction' },
            link: function(scope, element, attrs) {
                var template = '<div class="modal hide fade">\
                          <div class="modal-header">\
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>\
                            <h3 ng-i18next>Delete Confirm</h3>\
                          </div>\
                          <div class="modal-body">\
                            <p>{{confirmMessage}}</p>\
                          </div>\
                          <div class="modal-footer">\
                            <button class="btn" data-dismiss="modal" aria-hidden="true" ng-i18next>No</button>\
                            <button class="btn btn-primary" ng-click="delete()" ng-i18next>Yes</button>\
                          </div>\
                    </div>';
                var modal = $(template);
                var link = $compile(modal);
                link(scope);

                element.on('click', showModal);

                function showModal() {
                    $(modal).modal({
                        backdrop: 'static',
                        keyboard: true
                    }).css({
                        "position": "fixed",
                        "top": "50% !important"
                    });
                }

                scope['delete'] = function (event) {
                    $(modal).modal('hide');
                    var fn = $parse('$parent.' + scope.deleteAction);
                    fn(scope, { $event: event });
                };
            }
        };
    }])
    .directive('helperText', function() {
        return {
            restrict: 'A',
            link: function(scope, element, attrs) {
                element.attr('rel', 'popover');
                element.attr('data-html', 'true');
                element.attr('data-placement', 'top');
                element.attr('data-content', '<p class="popoverTipContent">' + attrs.helperText + '</p>');
                element.attr('original-title', '');
                var icon = document.createElement("i");
                icon.className = "icon-question-sign popoverTipIcon";
                icon.id = "popoverIcon";
                element.parents("div").first().get(0).appendChild(icon);
                $(icon).mouseover(function() {
                    element.popover('show');
                    element.focus();
                });

                $(icon).mouseout(function() {
                    element.popover('destroy');
                    element.blur();
                });
            }
        };
    })
    .directive('featureFilter', function() {
        return {
            restrict: "A",
            link: function(scope) {
                scope.$watch(function() {
                    return scope.featurename;
                }, function(newval) {
                    if (newval == undefined) return;
                    $("div.category:hidden").show();
                    $(".row-fluid > div").each(function(i, item) {
                        if ($(item).find(".title").text().toLowerCase().indexOf(newval.toLowerCase()) >= 0) {
                            $(item).show();
                        } else {
                            $(item).hide();
                        }
                    });
                    $("div.category:not(:has(.row-fluid > div:visible))").hide();
                });
            }
        };
    })
    .directive('featureSelector', function() {
        return {
            restrict: "A",
            link: function(scope, element) {
                var checkbox = element.find(":checkbox:first");
                checkbox.on('click', function(e) {
                    setcss();
                    e.stopPropagation();
                });
                element.on('click', function() {
                    checkbox.get(0).checked = !checkbox.get(0).checked;
                    setcss();
                });

                var setcss = function() {
                    if (checkbox.get(0).checked)
                        element.find("span:first").css("color", "rgba(255, 255, 0, 1);");
                    else {
                        element.find("span:first").css("color", "");
                    }
                };
            }
        };
    })
    .directive('loadingIndicator', function() {
        return {
            restrict: "A",
            link: function(scope, element) {
                // hide the element initially
                element.hide();

                scope.$on('_START_REQUEST_', function() {
                    // got the request start notification, show the element
                    element.show();
                });

                scope.$on('_END_REQUEST_', function() {
                    // got the request end notification, hide the element
                    element.hide();
                });
            }
        };
    })
    .directive('coDatetimePicker', function () {
        return {
            restrict: "A",
            link: function (scope, element, attrs) {
                if (attrs.coDatetimePicker == 'date') {
                    $(element).datetimepicker({ pickTime: false });
                } else {
                    $(element).datetimepicker({ pickSeconds: false });
                }
            }
        };
    })
    .directive('coUpdateScrollbar', function () {
        return {
            restrict: "A",
            link: function (scope, element, attrs) {
                var navigation = $('#navigation');
                var scrollbar = $('#navigation>.ps-scrollbar-y');
                element.click(function() {
                    setTimeout(function () {
                        scrollbar.hide();
                        navigation.perfectScrollbar('update');
                        scrollbar.show();
                    }, 300);
                });
            }
        };
    });
    


