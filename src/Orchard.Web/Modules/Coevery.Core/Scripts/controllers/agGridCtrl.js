(function () {
    var AgGridCtrl, gridz;

    gridz = angular.module("coevery.grid");

    gridz.controller("AgGridCtrl", AgGridCtrl = (function () {

        AgGridCtrl.$inject = ["$q", /*"hasSearchFilters", */"flatten"];

        function AgGridCtrl($q, /*hasSearchFilters,*/ flatten) {
            this.$q = $q;
            //this.hasSearchFilters = hasSearchFilters;
            this.flatten = flatten;
        }

        AgGridCtrl.prototype.registerGridElement = function ($grid) {
            return this.$grid = $grid;
        };

        AgGridCtrl.prototype.getGridId = function () {
            return this.$grid.attr("id");
        };

        AgGridCtrl.prototype.getSelectedRowIds = function () {
            return this.getParam("selarrrow");
        };
        
        AgGridCtrl.prototype.getPageRows = function () {
            return this.getParam("rowNum");
        };
        AgGridCtrl.prototype.getCurrentPage = function () {
            return this.getParam("page");
        };

        AgGridCtrl.prototype.reload = function (callback) {
            if (callback == null) {
                callback = angular.noop;
            }
            this.$grid.trigger("reloadGrid");
            return this.$grid.one("jqGridAfterLoadComplete", callback);
        };

        AgGridCtrl.prototype.getParam = function (name) {
            return this.$grid.getGridParam(name);
        };

        AgGridCtrl.prototype.setParam = function (params) {
            return this.$grid.setGridParam(params);
        };

        AgGridCtrl.prototype.updateRow = function (id, data) {
            this.$grid.setRowData(id, this.flatten(data));
            return this._flashRow(id);
        };

        AgGridCtrl.prototype.addRow = function (id, data, position) {
            if (position == null) {
                position = "first";
            }
            this.$grid.addRowData(id, this.flatten(data), position);
            return this._flashRow(id);
        };

        AgGridCtrl.prototype.hasRow = function (id) {
            return !!this.$grid.getInd(id);
        };

        AgGridCtrl.prototype.saveRow = function (id, data) {
            if (this.hasRow(id)) {
                return this.updateRow(id, data);
            } else {
                return this.addRow(id, data);
            }
        };

        AgGridCtrl.prototype.removeRow = function (id) {
            var _this = this;
            return this._flashRow(id, function () {
                return _this.$grid.delRowData(id);
            });
        };

        //AgGridCtrl.prototype.search = function(filters) {
        //  var deferred, params;
        //  deferred = this.$q.defer();
        //  params = {
        //    search: this.hasSearchFilters(filters),
        //    postData: {
        //      filters: JSON.stringify(filters)
        //    }
        //  };
        //  this.setParam(params);
        //  this.reload(function() {
        //    return deferred.resolve(filters);
        //  });
        //  return deferred.promise;
        //};

        AgGridCtrl.prototype.isColumnHidden = function (columnId) {
            var column;
            column = _.findWhere(this._getColModel(), {
                name: columnId
            });
            return column != null ? column.hidden : void 0;
        };

        AgGridCtrl.prototype.toggleColumn = function (columnId) {
            var showOrHide;
            showOrHide = this.isColumnHidden(columnId) ? "showCol" : "hideCol";
            this.$grid.jqGrid(showOrHide, columnId);
            return this._triggerResize();
        };

        AgGridCtrl.prototype.columnChooser = function (options) {
            var _this = this;
            if (options == null) {
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

        AgGridCtrl.prototype._getColModel = function () {
            return this.$grid.jqGrid("getGridParam", "colModel");
        };

        AgGridCtrl.prototype._triggerResize = function () {
            return this.$grid.trigger("resize");
        };

        AgGridCtrl.prototype._flashRow = function (id, complete) {
            var $row;
            if (complete == null) {
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

        return AgGridCtrl;

    })());

}).call(this);
