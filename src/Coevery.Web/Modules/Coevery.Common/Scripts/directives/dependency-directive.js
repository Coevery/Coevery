angular.module('coevery.dependency', [])
    .directive('coDependency', function() {
        return {
            link: function(scope, element, attrs) {
                scope.$watch(attrs.coDependency, function (newValue) {
                    if (newValue) {
                        element.prop('checked', true);
                        element.prop('disabled', true);
                    } else {
                        element.prop('disabled', false);
                    }
                });
            }
        };
    });