'use strict';

define(['core/app/detourService', 'core/services/commondataservice', 'core/services/columndefinitionservice'], function (detour) {
    detour.registerController([
      'RelatedEntityListCtrl',
      ['$rootScope', '$scope', '$parse', 'logger', '$detour', '$resource', '$stateParams', '$location', 'commonDataService', 'columnDefinitionService',
      function ($rootScope, $scope, $parse, logger, $detour, $resource, $stateParams, $location, commonDataService, columnDefinitionService) {

          var primaryKeyGetter = $parse('ContentId');
          $scope.toolButtonDisplay = false;
          //$scope.moduleName = moduleName;
          
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
              var record = commonDataService.get({ contentType: $scope.entityTypeName, pageSize: pageSize, page: page, viewId: $scope.viewId }, function () {
                  $scope.myData = record.EntityRecords;
                  $scope.totalServerItems = record.TotalNumber;
                  if (!$scope.$$phase) {
                      $scope.$apply();
                  }
              }, function () {
                  logger.error("Failed to fetched records for " + $scope.entityTypeName);
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

          $scope.getRelatedData = function () {
              var gridColumns = columnDefinitionService.query({ contentType: $scope.entityTypeName, viewId: $scope.viewId }, function() {
                  $scope.columnDefs = gridColumns;
                  $scope.getPagedDataAsync($scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage);
              }, function() {
              });
          };

          $scope.Refresh = function () {
              $scope.getPagedDataAsync($scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage);
          };


          $scope.delete = function (id) {
              $scope.entityId = id;
              $('#myModalRelationship').modal({
                  backdrop: 'static',
                  keyboard: true
              });
          };

          $rootScope.deleteRelationship = function () {
              $('#myModalRelationship').modal('hide');
              var id = $scope.entityId;
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
                  logger.success('Delete the relationship successful.');
              }, function () {
                  logger.error('Failed to delete the relationship');
              });
          };

          $scope.add = function () {
              $detour.transitionTo('Create', { Module: $scope.entityTypeName });
          };

          $scope.edit = function (id) {
              if (!id && $scope.selectedItems.length > 0) {
                  id = primaryKeyGetter($scope.selectedItems[0]);
              }
              $detour.transitionTo('Detail', { Module: $scope.entityTypeName, Id: id });
          };
          
          $scope.view = function (id) {
              if (!id && $scope.selectedItems.length > 0) {
                  id = primaryKeyGetter($scope.selectedItems[0]);
              }
              $detour.transitionTo('View', { Module: $scope.entityTypeName, Id: id });
          };
      }]
    ]);
});