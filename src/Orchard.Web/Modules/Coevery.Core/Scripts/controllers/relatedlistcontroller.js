'use strict';

define(['core/app/couchPotatoService', 'core/services/commondataservice', 'core/services/columndefinitionservice'], function (detour) {
    detour.registerController([
      'RelatedListCtrl',
      ['$rootScope', '$scope', '$parse', 'logger', '$state', '$resource', '$stateParams', '$location', 'commonDataService', 'columnDefinitionService', 'viewDefinitionService',
      function ($rootScope, $scope, $parse, logger, $state, $resource, $stateParams, $location, commonDataService, columnDefinitionService, viewDefinitionService) {
          var moduleName = $rootScope.$stateParams.Module;
          
          var primaryKeyGetter = $parse('ContentId');
          $scope.toolButtonDisplay = false;
          $scope.currentViewId = 0;
          $scope.moduleName = moduleName;
          
          $scope.definitionViews = [];
          $scope.columnDefs = [];

          //init pagingoption
          var pageSizes = [50, 100, 200];//[250, 500, 1000];
          var currentPage = parseInt($location.$$search['Page']);
          if (!currentPage) currentPage = 1;
          var pageSize = parseInt($location.$$search['PageSize']);
          if (!pageSize | pageSizes.indexOf(pageSize) < 0) pageSize = 50;//250
          $scope.pagingOptions = {
              pageSizes: pageSizes,
              pageSize: pageSize,
              currentPage: currentPage
          };

          $scope.getPagedDataAsync = function (pageSize, page) {
              $location.search("PageSize", pageSize);
              $location.search("Page", page);
              $stateParams["PageSize"] = pageSize;
              var record = commonDataService.get({ contentType: moduleName, pageSize: pageSize, page: page, viewId: $scope.currentViewId }, function () {
                  $scope.myData = record.EntityRecords;
                  $scope.totalServerItems = record.TotalNumber;
                  if (!$scope.$$phase) {
                      $scope.$apply();
                  }
              }, function () {
                  logger.error("Failed to fetched records for " + moduleName);
              });
          };

          $scope.$watch('pagingOptions', function (newVal, oldVal) {
              if (newVal !== oldVal) {
                  if (newVal.pageSize != oldVal.pageSize) {
                      var maxPage = Math.ceil($scope.totalServerItems / newVal.pageSize);
                      //var currentPage = Math.ceil(oldVal.pageSize * $scope.pagingOptions.currentPage / newVal.pageSize);
                      //if (currentPage > maxPage) currentPage = maxPage;
                      if ($scope.pagingOptions.currentPage > maxPage) {
                          $scope.pagingOptions.currentPage = maxPage;
                          return;
                      }
                  }
                  $scope.getPagedDataAsync($scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage);
              }
          }, true);

          $scope.$watch('filterOptions', function (newVal, oldVal) {
              if (newVal !== oldVal) {
                  $scope.getPagedDataAsync($scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage);
              }
          }, true);


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

          // fetch view columns
          $scope.FetchViewColumns = function (viewId) {
              if (viewId <= 0) return;
              if (viewId == $scope.currentViewId) return;
              $scope.currentViewId = viewId;
              //$location.search("ViewId", viewId);
              var gridColumns = columnDefinitionService.query({ contentType: moduleName, viewId: viewId }, function () {
                  $scope.columnDefs = gridColumns;
                  $scope.getPagedDataAsync($scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage);
              }, function () {
              });

          };

          $scope.Refresh = function () {
              $scope.getPagedDataAsync($scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage);
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
              $rootScope.Search = $location.$$search;
              $state.transitionTo('Create', { Module: moduleName });
          };

          $scope.edit = function (id) {
              if (!id && $scope.selectedItems.length > 0) {
                  id = primaryKeyGetter($scope.selectedItems[0]);
              }
              $rootScope.Search = $location.$$search;
              $state.transitionTo('Detail', { Module: moduleName, Id: id });
          };
          $scope.view = function (id) {
              if (!id && $scope.selectedItems.length > 0) {
                  id = primaryKeyGetter($scope.selectedItems[0]);
              }
              $rootScope.Search = $location.$$search;
              $state.transitionTo('View', { Module: moduleName, Id: id });
          };
      }]
    ]);
});