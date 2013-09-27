(function () {
    var gridz;

    gridz = angular.module("coevery.grid", []);

    gridz.directive("agGrid", [
      "$rootScope","$compile", "logger", function ($rootScope,$compile, logger) {
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
                      return;
                  }
                  //logger.info("Initializing the grid");
                  $grid = $element.find("table.gridz");
                  gridOptions.pager = '#'+ ($element.find(".gridz-pager").attr("id") || "gridz-pager");
                  gridOptions.gridComplete = function () {
                      var width;
                      width = $element.parent().width() - 1;
                      var complieFunc = $compile($("div.gridCellText"));
                      complieFunc($scope);

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
                      $scope.$on("ViewAction", function (subevent, args) {
                          subevent.preventDefault();
                          $scope.view(args);
                      });
                      $rootScope.$broadcast("ViewAction", id);
                  });
                  $grid.on("click.edit-action", ".edit-action", function (event) {
                      event.preventDefault();
                      var id;
                      id = $(this).attr("data-id");
                      $scope.$on("EditAction", function (subevent, args) {
                          subevent.preventDefault();
                          $scope.edit(args);
                      });
                      $rootScope.$broadcast("EditAction", id);
                  });
                  $grid.on("click.default-action", ".default-action", function (event) {
                      event.preventDefault();
                      var id;
                      id = $(this).attr("data-id");
                      $scope.$on("DefaultAction", function (subevent, args) {
                          subevent.preventDefault();
                          $scope.setDefault(args);
                      });
                      $rootScope.$broadcast("DefaultAction", id);
                  });
                  
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
                  };

                  responsiveResize();
                  return $grid.on("click.delete-action", ".delete-action", function (event) {
                      event.preventDefault();
                      var id;
                      id = $(this).attr("data-id");
                      $scope.$on("DeleteAction", function (subevent, args) {
                          //subevent.preventDefault();
                          $scope.delete(args);
                      });
                      $rootScope.$broadcast("DeleteAction", id);
                  });
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