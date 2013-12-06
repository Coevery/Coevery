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
    
    gridz.controller("AgGridCtrl", ["$q", "flatten", function ($q, flatten) {

        this.$q = $q;
        this.flatten = flatten;

        this.registerGridElement = function ($grid) {
            return this.$grid = $grid;
        };

        this.getGridId = function () {
            return this.$grid.attr("id");
        };

        this.getSelectedRowIds = function () {
            return this.getParam("selarrrow");
        };

        this.getPageRows = function () {
            return this.getParam("rowNum");
        };
        this.getCurrentPage = function () {
            return this.getParam("page");
        };

        this.reload = function (callback) {
            if (!callback) {
                callback = angular.noop;
            }
            this.$grid.trigger("reloadGrid");
            return this.$grid.one("jqGridAfterLoadComplete", callback);
        };

        this.getParam = function (name) {
            return this.$grid.getGridParam(name);
        };

        this.setParam = function (params) {
            return this.$grid.setGridParam(params);
        };

        this.updateRow = function (id, data) {
            this.$grid.setRowData(id, this.flatten(data));
            return this._flashRow(id);
        };

        this.getRow = function (rowId, flag) {
            return this.$grid.getLocalRow(rowId, flag);
        };

        this.addRow = function (id, data, position) {
            if (!position) {
                position = "first";
            }
            this.$grid.addRowData(id, this.flatten(data), position);
            return this._flashRow(id);
        };

        this.hasRow = function (id) {
            return !!this.$grid.getInd(id);
        };

        this.saveRow = function (id, data) {
            if (this.hasRow(id)) {
                return this.updateRow(id, data);
            } else {
                return this.addRow(id, data);
            }
        };

        this.removeRow = function (id) {
            var _this = this;
            return this._flashRow(id, function () {
                return _this.$grid.delRowData(id);
            });
        };

        this.isColumnHidden = function (columnId) {
            var column;
            column = _.findWhere(this._getColModel(), {
                name: columnId
            });
            return !column ? column.hidden : void 0;
        };

        this.toggleColumn = function (columnId) {
            var showOrHide;
            showOrHide = this.isColumnHidden(columnId) ? "showCol" : "hideCol";
            this.$grid.jqGrid(showOrHide, columnId);
            return this._triggerResize();
        };

        this.columnChooser = function (options) {
            var _this = this;
            if (!options) {
                options = {};
            }
            options.done = function (perm) {
                var choosedColumns;
                if (perm) {
                    _this.$grid.jqGrid("remapColumns", perm, true);
                }
                choosedColumns = _.map(_this._getColModel(), function (column) {
                    return _.pick(column, "name", "hidden");
                });
                return window.localStorage.setItem("gridz." + (_this.getGridId()) + ".choosedColumns", angular.toJson(choosedColumns));
            };
            return this.$grid.jqGrid("columnChooser", options);
        };

        this._getColModel = function () {
            return this.$grid.jqGrid("getGridParam", "colModel");
        };

        this._triggerResize = function () {
            return this.$grid.trigger("resize");
        };

        this._flashRow = function (id, complete) {
            var $row;
            if (!complete) {
                complete = angular.noop;
            }
            $row = $(this.$grid[0].rows.namedItem(id));
            $row.css("background-color", "#DFF0D8");
            $row.delay(100).fadeOut("medium", function () {
                return $row.css("background-color", "");
            });
            return $row.fadeIn("fast", function () {
                return complete();
            });
        };

    }]);

    gridz.directive("agGrid", [
        "$rootScope", "$compile", "logger", "$http", '$i18next', function ($rootScope, $compile, logger, $http, $i18next) {
            var link;
            link = function ($scope, $element, attrs, gridCtrl) {
                var alias, initializeGrid, loadGrid;
                alias = attrs.agGridName;
                if (alias) {
                    $scope[alias] = gridCtrl;
                }
                initializeGrid = function (gridOptions) {
                    var $grid, isInitial = true;
                    $grid = $element.find("table.gridz");
                    if (!gridOptions.treeGrid && !gridOptions.nestedDrag) {
                        gridOptions.pager = '#' + ($element.find(".gridz-pager").attr("id") || "gridz-pager");
                    } else {
                        gridOptions.pager = "";
                    }

                    gridOptions.loadBeforeSend = function (xhr, settings) {
                        if (isInitial) {
                            $element.hide();
                        }
                        $http.pendingRequests.unshift("jqGrid");
                        $rootScope.$broadcast('_START_REQUEST_');
                        return true;
                    },
                    gridOptions.gridComplete = function () {
                        $compile($grid)($scope);

                        var width, pager;
                        width = $element.parent().width() - 1;
                        if (!gridOptions.treeGrid && !gridOptions.nestedDrag) {
                            pager = $(gridOptions.pager + '_center');
                            if (pager.find(".custom-pager").length === 0) {
                                var pagerOption = {
                                    prevText: $i18next('Prev'),
                                    nextText: $i18next('Next'),
                                    items: $grid.getGridParam("records"),
                                    itemsOnPage: $grid.getGridParam("rowNum"),
                                    currentPage: $grid.getGridParam("page"),
                                    onPageClick: function (pageNumber, event) {
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
                        } else if (gridOptions.treeGrid) {
                            $grid.SortTree(1);
                            $grid.find("tr:last-child").css("border-bottom", "solid 1px #D2D2D2");
                        }
                        

                        //Set row sortable
                        if (typeof attrs.agGridDrag !== 'undefined') {
                            var settings = !!attrs.agGridDrag ? JSON.parse(attrs.agGridDrag) : null;
                            
                            if (!gridOptions.nestedDrag) {
                                $grid.jqGrid('sortableRows', {
                                    update: function (event, ui) {
                                        var postData = [];
                                        if (!settings.Handler) {
                                            postData = $grid.find("tbody:first").sortable("toArray");
                                        }
                                        $http({
                                            url: settings.Url,
                                            method: settings.Method,
                                            data: postData
                                        }).then(function () {
                                            logger.success('Reordering succeeded.');
                                        }, function (response) {
                                            logger.error('Reordering Failed');
                                        });
                                    }
                                });
                            } else {
                                $grid.tableDrag({
                                    tableId: alias,
                                    initialLevel: 1,
                                    group: {
                                        columnName: 'Level',
                                        depthLimit: 3, /* child element depth, start from 1, 0 means no limit, actrual depth will be +1 deeper than that*/
                                    },
                                });
                                $grid.find("tr:last-child").css("border-bottom", "solid 1px #D2D2D2");
                                $element.find("tbody:first").disableSelection();
                            }
                        }

                        $grid.setGridWidth(width);
                        if (isInitial) {
                            $element.show();
                            isInitial = false;
                        }
                        $http.pendingRequests.shift();
                        $rootScope.$broadcast('_END_REQUEST_');
                        return $grid;
                    };

                    gridOptions.onPaging = function (pageOption) {
                        if (pageOption === "records") {
                            $(gridOptions.pager).find(".custom-pager")
                                .pagination('updateItemsOnPage', $(".ui-pg-selbox :selected").val());
                        }
                    };

                    gridOptions.beforeSelectRow = function (rowId, e) {
                        if (!gridOptions.multiselect || gridOptions.treeGrid) {
                            $scope.$apply(function () {
                                $scope.selectedRow = $grid.getLocalRow(rowId);
                                $scope.selectedItems = [rowId];
                            });
                        }
                        return true;
                    };

                    gridOptions.onSelectRow = function (rowid, status) {
                        if (gridOptions.multiselect && !gridOptions.treeGrid) {
                            $scope.$apply(function() {
                                $scope.selectedRow = null;
                                $scope.selectedItems = gridCtrl.getSelectedRowIds();
                                if ($scope.selectedItems.length === 1 && !!gridOptions.rowIdName) {
                                    $scope.selectedRow = $grid.getLocalRow(rowid);
                                }
                            });
                        }
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
                    if (!gridOptions) {
                        return;
                    }
                    var $grid;
                    $grid = $element.find("table.gridz");
                    if (gridOptions.needReloading === true) {
                        gridOptions.needReloading = false;
                        $grid.setGridParam({
                            data:[]
                        });
                        $grid.GridDestroy($grid.attr("id"));
                        $element.html("<table class=\"gridz\"></table>\n<div class=\"gridz-pager\"></div>");
                        setIdValue($element, attrs.agGridName);
                    }
                    $grid = $element.find("table.gridz");
                    gridCtrl.registerGridElement($grid);
                    if (!!attrs.importedOptions) {
                        var importedOptions = JSON.parse(attrs.importedOptions);
                        angular.extend(gridOptions, importedOptions);
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
        if (!opts) {
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