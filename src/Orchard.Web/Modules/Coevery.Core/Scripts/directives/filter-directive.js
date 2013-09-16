angular.module('coevery.filter', [])
    .directive('filterEditor', function() {
        return {
            template: '<form class="filterCreatorContent span12"><div class="btn-group field-selector"><button class="btn btn-small dropdown-toggle" data-toggle="dropdown">{{fieldTitle}}&nbsp;&nbsp;<span class="caret"></span></button><ul class="dropdown-menu"><li ng-repeat="field in fieldFilters"><a href="" ng-click="showFilterEditor(field)">{{field.DisplayName}}</a></li></ul></div><div class="field-editor"></div><span class="close deleteBtn" ng-click="delete()">x</span></form>',
            replace: true,
            restrict: 'E',
            transclude: true,
            scope: { filterArgs: '=?filterArgs', fieldFilters: '=fieldFilters' },
            link: function (scope, element, attrs) {
                var args = scope.filterArgs
                    ? scope.filterArgs
                    : { Type: scope.fieldFilters[0].Type };
              
                var type = args.Type;
                element.data('Type', type);
                var fieldFilter;
                $.each(scope.fieldFilters, function() {
                    if (this.Type == type) {
                        fieldFilter = this;
                        return false;
                    }
                });
                var editor = element.find('.field-editor:first');
                editor.append($('script[type="text/ng-template"]#' + fieldFilter.FormName).text());
                scope.fieldTitle = fieldFilter.DisplayName;
                for (var property in args.State) {
                    editor.find('[name="' + property + '"]:first').val(args.State[property]);
                }
                element.find('select').selectpicker({ style: "btn-small" });

                scope.showFilterEditor = function(field) {
                    editor.empty();
                    scope.fieldTitle = field.DisplayName;
                    element.data('Type', field.Type);
                    editor.append($('script[type="text/ng-template"]#' + field.FormName).text());
                    element.find('select').selectpicker({ style: "btn-small" });
                };

                scope.delete = function() {
                    element.remove();
                };
            }
        };
    });