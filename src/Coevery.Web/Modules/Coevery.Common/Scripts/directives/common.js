angular.module('coevery.common', [])
    .directive("coDeleteButton", ['$compile', '$parse', function ($compile, $parse) {
        return {
            restrict: "A",
            scope: { confirmMessage: '@confirmMessage', deleteAction: '@deleteAction' },
            link: function(scope, element, attrs) {
                var template = '<div class="modal hide fade">\
                          <div class="modal-header">\
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>\
                            <h3>Delete Confirm</h3>\
                          </div>\
                          <div class="modal-body">\
                            <p>{{confirmMessage}}</p>\
                          </div>\
                          <div class="modal-footer">\
                            <button class="btn" data-dismiss="modal" aria-hidden="true">No</button>\
                            <button class="btn btn-primary" ng-click="delete()">Yes</button>\
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

                scope.delete = function(event) {
                    $(modal).modal('hide');
                    var fn = $parse('$parent.' + scope.deleteAction);
                    fn(scope, { $event: event });
                };
            }
        };
    }])
    .directive('helperText', function () {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                element.attr('rel', 'popover');
                element.attr('data-html', 'true');
                element.attr('data-placement', 'top');
                element.attr('data-content', '<p class="popoverTipContent">' + attrs.helperText + '</p>');
                element.attr('original-title', '');
                var icon = document.createElement("i");
                icon.className = "icon-question-sign popoverTipIcon";
                icon.id = "popoverIcon";
                element.parents("div").first().get(0).appendChild(icon);
                $(icon).mouseover(function () {
                    element.popover('show');
                    element.focus();
                });

                $(icon).mouseout(function () {
                    element.popover('destroy');
                    element.blur();
                });
            }
        };
    })
    .directive('changeRequired', ["logger", function (logger) {
         return {
             restrict: 'A',
             link: function (scope, element, attrs) {
                 element.css("display", "none");
                 var fieldtype = attrs["changeRequired"];
                 scope.$watch(function() {
                     return scope.fieldtype;
                 }, function (newval,oldval) {
                     if (newval == fieldtype) {
                         if (scope.entities.length == 0) {
                             logger.info("please create and a entity and then publish it first");
                             scope.fieldtype = oldval;
                             return;
                         }
                         element.css("display", "block");
                         element.find(":text").addClass("required");
                         var scrolloffset = element.find("#pos").offset();
                         $("body,html").animate({
                             scrollTop: scrolloffset.top
                         }, 800);
                     } else {
                         element.css("display", "none");
                         element.find(":text").removeClass("required");
                     }
                 });
             }
         };
    }])
;


