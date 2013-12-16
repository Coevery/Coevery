(function () {
    function getTargetList(sourceList, orderList) {
        if (!orderList) {
            return;
        }
        angular.forEach(sourceList, function (item, key) {
            if (item.Selected) {
                item.Order = $.inArray(item.Value, orderList);
            }
        });
    }

    var module = angular.module('coevery.picklist', []);
    module.directive("ngPicklist", ["$rootScope", "$compile", "logger", function ($rootScope, $compile, logger) {
        var link = function ($scope, $element, attrs, controller) {
            if (!attrs.ngPicklist) {
                logger.error("No options!");
                $element.html("");
                return null;
            }
            var sourceList = JSON.parse(attrs.ngPicklist);
            if (!sourceList || !angular.isArray(sourceList)) {
                logger.error("Wrong options");
                $element.html("");
                return null;
            }

            var sortE = $element.find('.sortable-list ul');
            $scope.selectId = attrs.id;
            $scope.selectName = attrs.name;
            $scope.sourceListLabel = attrs.sourceListLabel;
            $scope.targetListLabel = attrs.targetListLabel;

            if (attrs.useDefaultModel) {
                $scope.sourceList = sourceList;
            }
                //todo:add methods for data not use format { Text:"", Value: "", Selected: "",Order: "" }
            else {
            }

            $scope.addAllSource = function () {
                angular.forEach($scope.sourceList, function (item, key) {
                    item.Selected = true;
                });
            };

            $scope.$on("getSelectedList", function(event,target) {
                //event.stopPropagation();
                target.list = $scope.sourceList.filter(function (element) { return element.Selected; }).sort(function(a,b) {
                    return a.Order - b.Order;
                });
            });

            sortE.sortable({
                placeholder: 'placeholder',
                forcePlaceholderSize: true,
                update: function (e, ui) {
                    getTargetList($scope.sourceList, sortE.sortable("toArray", { attribute: "value" }));
                    $scope.$apply();
                },
            });

            return $element;
        };
        return {
            restrict: "A",
            template: '<div class="row-fluid"><div class="span12"> <div class="span6"> <div class="viewfields-widget">' +
                '<table class="table"> <thead> <tr> ' +
                '<th> {{sourceListLabel}} </th> ' +
                '<th class="rightCell"> <button class="btn-link" ng-click="addAllSource()" type="button">Add All</button></th>' +
                '</tr></thead>' +
                '<tbody><tr ng-repeat="sourceItem in sourceList">' +
                '<td class="unselectedField"> {{sourceItem.Text}}</td>' +
                '<td class="rightCell">' +
                '<span ng-show="sourceItem.Selected" class="label">Added</span>' +
                '<span ng-hide="sourceItem.Selected" style="line-height: 14px;">' +
                '<button class="btn-link" ng-click="sourceItem.Selected=true" type="button" style="line-height: 14px;">Add</button>' +
                '</span></td></tr></tbody>' +
                '</table></div></div>' +
                '<div class="span6"><div class="viewfields-widget">' +
                '<h5>{{targetListLabel}}</h5>' +
                '<div class="sortable-list"><ul id="sortable">' +
                '<li ng-repeat="targetItem in sourceList | filter: { Selected: true } | orderBy:\'+Order\'" ng-attr-value="{{targetItem.Value}}">' +
                '<span style="margin-left: 3px;">{{targetItem.Text}}</span>' +
                '<div class="pull-right"><button class="btn-link" type="button" ng-click="targetItem.Selected=false">Remove</button>' +
                '</div></li></ul></div></div></div> ' +
                //'<select multiple="multiple" style="display:none;" ng-attr-id="{{selectId}}">' +
                //'<option ng-repeat="option in sourceList | filter: { Selected: true } | orderBy:\'+Order\'" ng-attr-value="{{ option.Value }}" selected=true> ' +
                //'{{ option.Text }}</option>' +
                //'</select>' +
                '<div class="hide" ng-repeat="option in sourceList | filter: { Selected: true } | orderBy:\'+Order\'">' +
                '<input type="hidden" ng-attr-name="{{selectName + \'[\' + $index + \'].Category\'}}" ng-attr-value="{{ option.Category }}" />' +
                '<input type="hidden" ng-attr-name="{{selectName + \'[\' + $index + \'].Type\'}}" ng-attr-value="{{ option.Value }}" />' +
                '<input type="hidden" ng-attr-name="{{selectName + \'[\' + $index + \'].Text\'}}" ng-attr-value="{{ option.Text }}" />' +
                '</div></div></div>',
            compile: function (element, attrs) {
                return {
                    post: link
                };
            },
            replace: true,
            scope: true
        };
    }]);
})();