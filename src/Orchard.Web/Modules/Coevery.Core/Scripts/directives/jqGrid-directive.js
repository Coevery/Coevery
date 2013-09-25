(function () {
    var gridz;

    gridz = angular.module("coevery.grid", []);

    gridz.directive("agGrid", [
      "logger", function (logger) {
          var link;
          link = function ($scope, $element, attrs, gridCtrl) {
              var alias, initializeGrid;
              gridCtrl.registerGridElement($element.find("table.gridz"));
              alias = attrs.agGridName;
              if (alias != null) {
                  $scope[alias] = gridCtrl;
              }
              initializeGrid = function (gridOptions) {
                  var $grid;
                  if (gridOptions == null) {
                      return null;
                  }
                  //logger.info("Initializing the grid");
                  $grid = $element.find("table.gridz");
                  gridOptions.pager = '#'+ ($element.find(".gridz-pager").attr("id") || "gridz-pager");
                  gridOptions.gridComplete = function () {
                      var width;
                      width = $element.parent().width() - 1;
                      return $grid.setGridWidth(width);
                  };
                  gridOptions.onSelectRow = function () {
                      $scope.$apply(function() {
                          $scope.selectedItems = gridCtrl.getSelectedRowIds();
                      });
                  };
                  gridOptions.onSelectAll = function () {
                      $scope.$apply(function() {
                          $scope.selectedItems = gridCtrl.getSelectedRowIds();
                      });
                  };
                  $grid.jqGrid(gridOptions);
                  
                  $grid.on("click.view-action", ".view-action", function (event) {        
                      event.preventDefault();
                      var id;
                      id = $(this).attr("data-id");
                      return $scope.view(id);
                  });
                  $grid.on("click.edit-action", ".edit-action", function (event) {
                      event.preventDefault();
                      var id;
                      id = $(this).attr("data-id");
                      return $scope.edit(id);
                  });
                  $grid.on("click.default-action", ".default-action", function (event) {
                      event.preventDefault();
                      var id;
                      id = $(this).attr("data-id");
                      return $scope.setDefault(id);
                  });
                  
                  $(window).bind('resize', function () {
                      var target = $('#page-actions');
                      if (target.length === 0) {
                          return;
                      }
                      $grid.setGridWidth(target.width(), false).trigger('reloadGrid'); //Resized to new width as buttons
                  }).trigger('resize');

                  //todo: need to refactor
                  //return $grid.on("click.delete-action", ".delete-action", function (event) {
                  //    event.preventDefault();
                  //    var id;
                  //    id = $(this).attr("data-id");
                  //    return $scope.delete(id);
                  //});
              };

              return $scope.$watch(attrs.agGrid, initializeGrid);
          };
          return {
              restrict: "A",
              template: "<table class=\"gridz\"></table>\n<div class=\"gridz-pager\"></div>",   
              compile: function (element, attrs) {
                  var alias;
                  alias = attrs.agGridName || "gridz";
                  element.find("table.gridz").attr("id", alias);
                  element.find("div.gridz-pager").attr("id", "" + alias + "-pager");
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

}).call(this);

/*Current Useless Code
showItem = function (id) {
                      return $scope.$apply(function () {
                          if ($scope.showItem != null) {
                              return $scope.showItem(id);
                          } else {
                              return logger.error("`$scope.showItem` is not defined");
                          }
                      });
                  };
                  editItem = function (id) {
                      return $scope.$apply(function () {
                          if ($scope.editItem != null) {
                              return $scope.editItem(id);
                          } else {
                              return logger.error("`$scope.editItem` is not defined");
                          }
                      });
                  };
                  $grid.on("showAction", function (event, id) {
                      event.preventDefault();
                      return showItem(id);
                  });
                  $grid.on("editAction", function (event, id) {
                      event.preventDefault();
                      return editItem(id);
                  });
                  $grid.on("click", "a.editActionLink", function (event) {
                      var id;
                      event.preventDefault();
                      id = $(this).parents("tr:first").attr("id");
                      return editItem(id);
                  });

                  return $grid.on("deleteAction", function (event, id) {
                      event.preventDefault();
                      return $scope.$apply(function () {
                          return $scope.deleteItem(id);
                      });
                  });
*/