define(['core/app/couchPotatoService', 'core/services/commondataservice', 'core/services/columndefinitionservice'], function (couchPotato) {
    couchPotato.registerController([
      'GeneralListCtrl',
      ['$rootScope', '$scope', '$parse', 'logger', '$state', '$resource', '$stateParams', 'commonDataService', 'columnDefinitionService',
      function ($rootScope, $scope, $parse, logger, $state, $resource, $stateParams, commonDataService, columnDefinitionService) {
          var moduleName = $rootScope.$stateParams.Module;
          var primaryKeyGetter = $parse('ContentId');

          $scope.moduleName = moduleName;

          $(window).scroll(function () {
              var scrollTop = $(window).scrollTop();
              scrollTop > 43 ? $('#actions').slideDown(100) : $('#actions').slideUp(100);
          });
          
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

          $scope.columnDefs = [];
          var gridColumns = columnDefinitionService.query({ contentType: moduleName }, function () {
              $scope.columnDefs = gridColumns;
          }, function () { });

          $scope.selectedItems = [];

          $scope.gridOptions = {
              data: 'myData',
              enablePaging: true,
              showFooter: true,
              multiSelect: true,
              enableRowSelection: true,
              showSelectionCheckbox: true,
              selectedItems: $scope.selectedItems,
              pagingOptions: $scope.pagingOptions,
              columnDefs: "columnDefs"
          };

          angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);

          $scope.toolButtonDisplay = false;

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
              var ids = [];
              if (id) {
                  ids.push(id);
              } else {
                  angular.forEach($scope.selectedItems, function (entity) {
                      ids.push(primaryKeyGetter(entity));
                  }, ids);
              }
              commonDataService.delete({ contentId: ids }, function () {
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
              if (!id && $scope.selectedItems.length > 0) {
                  id = primaryKeyGetter($scope.selectedItems[0]);
              }
              $state.transitionTo('Detail', { Module: moduleName, Id: id });
          };
          $scope.view = function (id) {
              if (!id && $scope.selectedItems.length > 0) {
                  id = primaryKeyGetter($scope.selectedItems[0]);
              }
              $state.transitionTo('View', { Module: moduleName, Id: id });
          };
      }]
    ]);
});