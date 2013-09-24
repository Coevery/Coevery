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
                    : { Type: scope.fieldFilters[0].Type, State: {} };

                scope.args = args;
                element.data('Type', args.Type);
                var fieldFilter;
                $.each(scope.fieldFilters, function() {
                    if (this.Type == args.Type) {
                        fieldFilter = this;
                        return false;
                    }
                });
                var editor = element.find('.field-editor:first');
                editor.append($('script[type="text/ng-template"]#' + fieldFilter.FormName).text());
                scope.fieldTitle = fieldFilter.DisplayName;
                for (var property in args.State) {
                    var formElement = editor.find('[name="' + property + '"]:first');
                    formElement.val(args.State[property]);
                }
                $compile(editor.children())(scope);

                scope.showFilterEditor = function(field) {
                    editor.empty();
                    scope.fieldTitle = field.DisplayName;
                    scope.args.Type = field.Type;
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
    .directive('selectpicker', function() {
        return {
            restrict: 'C',
            link: function(scope, element, attrs) {
                element.selectpicker();
            }
        };
    })
    .directive('filterNumericOperator', function() {
        return {
            restrict: 'A',
            link: function(scope, element, attrs) {
                var siblings = element.siblings().children();
                var single = siblings.filter('[name="Value"]:first');
                var min = siblings.filter('[name="Min"]:first');
                var max = siblings.filter('[name="Max"]:first');
                displayNumericEditorOptions();
                element.change(displayNumericEditorOptions);

                function displayNumericEditorOptions() {
                    element.children("option:selected").each(function() {
                        var val = $(this).val();
                        if (val == 'Between' || val == 'NotBetween') {
                            single.hide();
                            single.attr('required', null);
                            min.show();
                            min.attr('required', '');
                            max.show();
                            max.attr('required', '');
                        } else {
                            single.show();
                            single.attr('required', '');
                            min.hide();
                            min.attr('required', null);
                            max.hide();
                            max.attr('required', null);
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
                var singleInput = single.children(':input');
                var min = siblings.filter(':has([name="Min"]):first');
                var minInput = min.children(':input');
                var max = siblings.filter(':has([name="Max"]):first');
                var maxInput = max.children(':input');
                displayNumericEditorOptions();
                element.change(displayNumericEditorOptions);

                function displayNumericEditorOptions() {
                    element.children("option:selected").each(function() {
                        var val = $(this).val();
                        if (val == 'Between' || val == 'NotBetween') {
                            single.hide();
                            singleInput.attr('required', null);
                            min.show();
                            minInput.attr('required', '');
                            max.show();
                            maxInput.attr('required', '');
                        } else {
                            single.show();
                            singleInput.attr('required', '');
                            min.hide();
                            minInput.attr('required', null);
                            max.hide();
                            maxInput.attr('required', null);
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
                var container = element.parents('.filterCreatorWrap:first'),
                    options = container.data('reference-options'),
                    typeName = scope.args.Type;

                options = options ? options : {};
                if (options[typeName]) {
                    displayOptions(options[typeName]);
                } else {
                    var arr = typeName.split('.', 2),
                        entityName = arr[0],
                        fieldName = arr[1],
                        url = 'api/Projections/Reference/' + entityName + '?fieldName=' + fieldName;
                    $http.get(url).then(function(response) {
                        displayOptions(response.data);
                        options[typeName] = response.data;
                        container.data('reference-options', options);
                    });
                }

                function displayOptions(data) {
                    $.each(data, function() {
                        element.append('<option value="' + this.Id + '">' + this.DisplayText + '</option>');
                    });
                    var value = scope.args.State[element.attr('name')]
                        ? scope.args.State[element.attr('name')].split('&')
                        : null;
                    element.val(value);
                    element.selectpicker();
                }
            }
        };
    })
    .directive('filterOptionsetValue', function ($http) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var container = element.parents('.filterCreatorWrap:first'),
                    options = container.data('filter-options'),
                    typeName = scope.args.Type;

                options = options ? options : {};
                if (options[typeName]) {
                    displayOptions(options[typeName]);
                } else {
                    var arr = typeName.split('.', 2),
                        entityName = arr[0],
                        fieldName = arr[1],
                        url = 'api/Projections/OptionSet/' + entityName + '?fieldName=' + fieldName;
                    $http.get(url).then(function (response) {
                        displayOptions(response.data);
                        options[typeName] = response.data;
                        container.data('filter-options', options);
                    });
                }

                function displayOptions(data) {
                    $.each(data, function () {
                        element.append('<option value="' + this.ID + '">' + this.DisplayText + '</option>');
                    });
                    var value = scope.args.State[element.attr('name')]
                     ? scope.args.State[element.attr('name')].split('&')
                     : null;
                    element.val(value);
                    element.selectpicker();
                }
            }
        };
    });