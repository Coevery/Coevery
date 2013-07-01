define(['core/app/couchPotatoService', 'core/services/commondataservice', 'core/services/nggriddataservice'], function (couchPotato) {
    couchPotato.registerController([
      'GeneralListCtrl',
      ['$rootScope', '$scope', 'logger', '$state', '$resource', '$stateParams', 'commonDataService', 'ngGridDataService',
      function ($rootScope, $scope, logger, $state, $resource, $stateParams, commonDataService, ngGridDataService) {
          var moduleName = $rootScope.$stateParams.Module;

          $scope.pagingOptions = {
              pageSizes: [250, 500, 1000],
              pageSize: 250,
              currentPage: 1
          };

          $scope.setPagingData = function (data, page, pageSize) {
              var pagedData = data.slice((page - 1) * pageSize, page * pageSize);
              var maxRow = data.length;
              var maxPages = Math.ceil(maxRow / $scope.pagingOptions.pageSize);

              $scope.pagingOptions.pageNumber = [];
              for (var index = 0; index < maxPages; index++) {
                  $scope.pagingOptions.pageNumber[index] = index + 1;
              }
              $scope.myData = pagedData;
              $scope.pagingOptions.totalServerItems = data.length;
              if (!$scope.$$phase) {
                  $scope.$apply();
              }
          };

          $scope.getPagedDataAsync = function (pageSize, page) {
              var records = commonDataService.query({ contentType: moduleName }, function () {
                  $scope.setPagingData(records, page, pageSize);
              }, function () {
                  logger.error("Failed to fetched records for " + moduleName);
              });
          };

          $scope.$watch('pagingOptions', function (newVal, oldVal) {
              if (newVal !== oldVal && newVal.currentPage !== oldVal.currentPage) {
                  $scope.getPagedDataAsync($scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage);
              }
          }, true);

          $scope.$watch('filterOptions', function (newVal, oldVal) {
              if (newVal !== oldVal) {
                  $scope.getPagedDataAsync($scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage);
              }
          }, true);

          var rowTemplate = '<div ng-mouseover="mouseOverState(row,true)" style="overflow:visible;" ng-click="rowClick(row,renderedRows,col)" ng-mouseout="mouseOverState(row,false)" ng-style="{\'cursor\': row.cursor, \'z-index\': col.zIndex() }" ng-repeat="col in renderedColumns" ng-class="col.colIndex()" class="ngCell {{col.cellClass}}" ng-cell></div>';
          $scope.gridOptions = {
              data: 'myData',
              enablePaging: true,
              showFooter: true,
              multiSelect: true,
              rowTemplate: rowTemplate,
              pagingOptions: $scope.pagingOptions,
              columnDefs: "myColumns"
          };

         

          $scope.toolButtonDisplay = false;
          $scope.checkedIds = [];
          $scope.checkChange = function (checkAll,rows) {
              if (checkAll)
              {
                  //check all
                  $.each(rows, function (i, v) {
                      v.checked = rows.allSelected;
                  });
              }
              var checkItems = $(rows).filter(function () {
                  return this.checked;
              });
              rows.allSelected = checkItems.length == rows.length;
              $scope.toolButtonDisplay = checkItems.length > 0;
              $scope.checkedIds = [];
              $.each(checkItems, function (i, v) {
                  $scope.checkedIds[i] = v.entity.ContentId;
              });
              
              if (!checkAll && $scope.checkedIds.length > 1)
              {
                  $scope.mouseOverState(this.row, false);
              }
          };
          
          $scope.mouseOverState = function (row, isOver) {
              if ($scope.checkedIds.length > 1) {
                  row.MouseOve = false;
                  return;
              }
              row.MouseOve = isOver;
          };

          $scope.rowClick = function (row, rows, col) {
              if (col.index <= 0) return;
              $.each(rows, function (i, v) {
                  if(v != row)
                  v.checked = false;
              });
              if ($scope.checkedIds.length <= 1) {
                  if (!row.checked) row.checked = true;
                  else row.checked = false;
              }
              $scope.checkChange(false, rows);
              $scope.mouseOverState(row, true);
          };
          
          angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);

          var gridColumns = ngGridDataService.query({ contentType: moduleName }, function () {
              $scope.myColumns = gridColumns;
          }, function () {
              $scope.myColumns = [];
          });

          $scope.getPagedDataAsync($scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage);
          $scope.Refresh = function () {
              
              $scope.getPagedDataAsync($scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage);
          };

          //var t = function (str) {
          //    var result = i18n.t(str);
          //    return result;
          //};

          var idIndex = 1;
          $scope.filters = [{ id: '1' }];
          $scope.addFilter = function () {
              idIndex++;
              $scope.filters.splice($scope.filters.length, 0, { id: idIndex });
          };

          $scope.removeFilter = function (index) {
              $scope.filters.splice(index, 1);
          };

          $scope.expendCollapse = function () {
              if ($('#collapseBtn').hasClass('icon-collapse-up')) {
                  $('#collapseBtn').addClass('icon-collapse-down');
                  $('#collapseBtn').removeClass('icon-collapse-up');
                  $('#closeFilterLink').css('display', '');

              } else {
                  $('#collapseBtn').removeClass('icon-collapse-down');
                  $('#collapseBtn').addClass('icon-collapse-up');
                  $('#closeFilterLink').css('display', 'none');
              }
          };

          $scope.closeFilterCollapse = function () {
              $('#filterCollapse').css('display', 'none');
          };

          $scope.openFilterCollapse = function (fiterId) {
              $('#filterCollapse').css('display', '');
              if ($('#collapseBtn').hasClass('icon-collapse-up')) return;
              $scope.expendCollapse();
              $('#collapseBtn').click();
          };


         
          
          $scope.delete = function (id) {
              if (!id) {
                  id = $scope.checkedIds.toString();
              }
              commonDataService.delete({ contentId: id }, function () {
                  $scope.Refresh();
                  logger.success('Delete the ' + moduleName + ' successful.');
              }, function () {
                  logger.error('Failed to delete the lead.');
              });
          };

          $scope.add = function () {
              $state.transitionTo('Create', { Module: moduleName });
          };

          $scope.edit = function (id) {
              $state.transitionTo('Detail', { Module: moduleName, Id: id });
          };
      }]
    ]);
});