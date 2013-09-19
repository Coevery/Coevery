angular.module('coevery.filter', [])
    .directive('filterEditor', function($compile) {
        return {
            template: '<form class="filterCreatorContent span12"><div class="btn-group field-selector"><button class="btn btn-small dropdown-toggle" data-toggle="dropdown">{{fieldTitle}}&nbsp;&nbsp;<span class="caret"></span></button><ul class="dropdown-menu"><li ng-repeat="field in fieldFilters"><a href="" ng-click="showFilterEditor(field)">{{field.DisplayName}}</a></li></ul></div><div class="field-editor"></div><span class="close deleteBtn" ng-click="delete()">x</span></form>',
            replace: true,
            restrict: 'E',
            transclude: true,
            scope: { filterArgs: '=?filterArgs', fieldFilters: '=fieldFilters' },
            link: function(scope, element, attrs) {
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
                $compile(editor.children())(scope);

                scope.showFilterEditor = function(field) {
                    editor.empty();
                    scope.fieldTitle = field.DisplayName;
                    element.data('Type', field.Type);
                    editor.append($('script[type="text/ng-template"]#' + field.FormName).text());
                    $compile(editor.children())(scope);
                };

                scope.delete = function() {
                    element.remove();
                };
            }
        };
    })
    .directive('filterNumericOperator', function() {
        return {
            restrict: 'A',
            link: function(scope, element, attrs) {
                var siblings = element.siblings();
                var single = siblings.filter('[name="Value"]:first');
                var min = siblings.filter('[name="Min"]:first');
                var max = siblings.filter('[name="Max"]:first');
                displayNumericEditorOptions();
                element.change(displayNumericEditorOptions);
                element.selectpicker();

                function displayNumericEditorOptions() {
                    element.children("option:selected").each(function() {
                        var val = $(this).val();
                        if (val == 'Between' || val == 'NotBetween') {
                            single.hide();
                            min.show();
                            max.show();
                        } else {
                            single.show();
                            min.hide();
                            max.hide();
                        }
                    });
                }
            }
        };
    })
    .directive('filterDateOperator', function() {
        return {
            restrict: 'A',
            link: function(scope, element, attrs) {
                var options = attrs.filterDateOperator == 'date'
                    ? { pickTime: false }
                    : { pick12HourFormat: true, pickSeconds: false };
                var siblings = element.siblings();
                siblings.datetimepicker(options);
                var single = siblings.filter(':has([name="Value"]):first');
                var min = siblings.filter(':has([name="Min"]):first');
                var max = siblings.filter(':has([name="Max"]):first');
                displayNumericEditorOptions();
                element.change(displayNumericEditorOptions);
                element.selectpicker();

                function displayNumericEditorOptions() {
                    element.children("option:selected").each(function() {
                        var val = $(this).val();
                        if (val == 'Between' || val == 'NotBetween') {
                            single.hide();
                            min.show();
                            max.show();
                        } else {
                            single.show();
                            min.hide();
                            max.hide();
                        }
                    });
                }
            }
        };
    })
    .directive('filterReferenceValue', function($http) {
        return {
            restrict: 'A',
            link: function(scope, element, attrs) {
                var arr = element.parents('form:first').data('Type').split('.', 2);
                var entityName = arr[0];
                var fieldName = arr[1];
                var url = 'api/Projections/Reference/' + entityName + '?fieldName=' + fieldName;
                $http.get(url).then(function(response) {
                    $.each(response.data, function() {
                        element.append('<option value="' + this.Id + '">' + this.DisplayText + '</option>');
                    });
                    element.selectpicker();
                });
            }
        };
    })
    .directive('filterOptionsetValue', function ($http) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var arr = element.parents('form:first').data('Type').split('.', 2);
                var entityName = arr[0];
                var fieldName = arr[1];
                var url = 'api/Projections/OptionSet/' + entityName + '?fieldName=' + fieldName;
                $http.get(url).then(function (response) {
                    $.each(response.data, function () {
                        element.append('<option value="' + this.ID + '">' + this.DisplayText + '</option>');
                    });
                    element.selectpicker();
                });
            }
        };
    });
