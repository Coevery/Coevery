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
    //.directive('delConfirm', function () {
    //    return {
    //        template: function (element, attrs) {
    //            var templateHtml = '<section id="' + attrs.delId + '" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">';
    //            templateHtml += '<div class="modal-header">';
    //            templateHtml += '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">x</button>';
    //            templateHtml += '<h3 id="myModalLabel">Confirm</h3>';
    //            templateHtml += '</div>';
    //            templateHtml += '<div class="modal-body">';
    //            templateHtml += '<p>' + attrs.confirmMsg + '</p>';
    //            templateHtml += '</div>';
    //            templateHtml += '<div class="modal-footer">';
    //            templateHtml += '<button class="btn" data-dismiss="modal" aria-hidden="true">No</button>';
    //            templateHtml += '<button class="btn btn-primary" ng-click="' + attrs.delFuc + '">Yes</button>';
    //            templateHtml += '</div>';
    //            templateHtml += '</section>';
    //            return templateHtml;
    //        },
    //        replace: true,
    //        restrict: 'E'
    //    };
    //})
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
     .directive('indiClick',['$parse',function ($parse) {
         return {
             restrict: 'A',
             link: function (scope, element, attrs) {
                 var fn = $parse(attrs["indiClick"]);
                 element.on('click', function (event) {
                     scope.$apply(function () {
                         attrs.$set('disabled', true);
                         try
                         {
                             fn(scope, { $event: event })
                             .then(function (res) {
                                 attrs.$set('disabled', false);
                             }, function (res) {
                                 attrs.$set('disabled', false);
                             });
                         } catch (e) {
                             attrs.$set('disabled', false);
                         }
                     });
                 });
             }
         };
     }]);
