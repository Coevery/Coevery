(function () {
    function setIdValue(element, baseValue) {
        var alias, index = 0;
        alias = baseValue || "gridz";
        while ($('#' + alias).length !== 0) {
            alias = alias + index.toString(10);
            index++;
        }
        element.find("table.gridz").attr("id", alias);
        element.find("div.gridz-pager").attr("id", "" + alias + "-pager");
    }

    var gridz;

    gridz = angular.module("coevery.grid", []);

    gridz.directive("agGrid", [
        "$rootScope", "$compile", "logger", function ($rootScope, $compile, logger) {
            var link;
            link = function ($scope, $element, attrs, gridCtrl) {
                var alias, initializeGrid, loadGrid;
                gridCtrl.registerGridElement($element.find("table.gridz"));
                alias = attrs.agGridName;
                if (alias != null) {
                    $scope[alias] = gridCtrl;
                }
                initializeGrid = function (gridOptions) {
                    var $grid;
                    //logger.info("Initializing the grid");
                    $grid = $element.find("table.gridz");
                    gridOptions.pager = '#' + ($element.find(".gridz-pager").attr("id") || "gridz-pager");
                    gridOptions.gridComplete = function () {
                        $compile($grid)($scope);

                        var width, pager;
                        width = $element.parent().width() - 1;
                        pager = $(gridOptions.pager + '_center');
                        if (pager.find(".custom-pager").length === 0) {
                            var pagerOption = {
                                items: $grid.getGridParam("records"),
                                itemsOnPage: $grid.getGridParam("rowNum"),
                                currentPage: $grid.getGridParam("page"),
                                onPageClick: function(pageNumber, event) {
                                    if (!event) {
                                        return;
                                    }
                                    event.preventDefault();
                                    $grid.setGridParam({
                                        page: pageNumber
                                    });
                                    $grid.trigger("reloadGrid");
                                },
                                //cssStyle: 'compact-theme'
                            };
                            if (width < 560) {
                                pagerOption.displayedPages = 3;
                            }
                            pager.append("<section class='custom-pager pagination'></section>");
                            pager.find("section").pagination(pagerOption);
                        }

                        return $grid.setGridWidth(width);
                    };

                    gridOptions.onPaging = function (pageOption) {
                        if (pageOption === "records") {
                            $(gridOptions.pager).find(".custom-pager")
                                .pagination('updateItemsOnPage', $(".ui-pg-selbox :selected").val());
                        }
                    };

                    gridOptions.onSelectRow = function () {
                        $scope.$apply(function () {
                            $scope.selectedRow = null;
                            $scope.selectedItems = gridCtrl.getSelectedRowIds();
                            if ($scope.selectedItems.length === 1 && $scope.idAttr) {
                                $scope.selectedRow = gridCtrl.getParam("data").filter(function (element) {
                                    return element[$scope.idAttr].toString() === $scope.selectedItems[0];
                                });
                            }
                        });
                    };

                    gridOptions.onSelectAll = function () {
                        $scope.$apply(function () {
                            $scope.selectedItems = gridCtrl.getSelectedRowIds();
                        });
                    };

                    $grid.jqGrid(gridOptions);

                    /*
                    adds listener to resize grid to parent container when window is resized.
                    This will work for reponsive and fluid layouts
                    */

                    function responsiveResize() {
                        var gboxId = "#gbox_" + ($grid.attr("id"));
                        return $(window).on("resize", function (event, ui) {
                            var curWidth, parWidth, w;
                            parWidth = $(gboxId).parent().width();
                            curWidth = $(gboxId).width();
                            w = parWidth - 1;
                            if (Math.abs(w - curWidth) > 2) {
                                $grid.setGridWidth(w);
                            }
                        });
                    }

                    responsiveResize();
                };

                loadGrid = function (gridOptions) {
                    if (gridOptions == null) {
                        return;
                    }
                    var $grid;
                    $grid = $element.find("table.gridz");
                    if (gridOptions.needReloading === true) {
                        gridOptions.needReloading = false;

                        $grid.GridDestroy($grid.attr("id"));
                        $element.html("<table class=\"gridz\"></table>\n<div class=\"gridz-pager\"></div>");
                        setIdValue($element, attrs.agGridName);
                    }
                    initializeGrid(gridOptions);
                };
                return $scope.$watch(attrs.agGrid, loadGrid);
            };
            return {
                restrict: "A",
                template: "<table class=\"gridz\"></table>\n<div class=\"gridz-pager\"></div>",
                compile: function (element, attrs) {
                    setIdValue(element, attrs.agGridName);
                    return {
                        post: link
                    };
                },
                require: "agGrid",
                controller: "AgGridCtrl"
            };
        }
    ]);

    gridz.value("flatten", function (target, opts) {
        var delimiter, getKey, output, step;
        if (opts == null) {
            opts = {
                delimiter: "."
            };
        }
        delimiter = opts.delimiter;
        getKey = function (key, prev) {
            if (prev) {
                return prev + delimiter + key;
            } else {
                return key;
            }
        };
        step = function (object, prev) {
            return angular.forEach(Object.keys(object), function (key) {
                var isArray, isObject, type;
                isArray = opts.safe && object[key] instanceof Array;
                type = Object.prototype.toString.call(object[key]);
                isObject = type === "[object Object]" || type === "[object Array]";
                if (!isArray && isObject) {
                    return step(object[key], getKey(key, prev));
                }
                return output[getKey(key, prev)] = object[key];
            });
        };
        output = {};
        step(target);
        return output;
    });

})();

/*Abandoned Code
            $grid.on("click.view-action", ".view-action", function(event) {
                event.preventDefault();
                var id;
                id = $(this).attr("data-id");
                $scope.$on("ViewAction", function(subevent, args) {
                    subevent.preventDefault();
                    $scope.view(args);
                });
                $rootScope.$broadcast("ViewAction", id);
            });
*/