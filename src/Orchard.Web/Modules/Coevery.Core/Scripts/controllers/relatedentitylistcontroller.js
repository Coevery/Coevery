'use strict';

define(['core/app/detourService', 'core/services/commondataservice', 'core/services/columndefinitionservice'], function (detour) {
    detour.registerController([
      'RelatedEntityListCtrl',
      ['$rootScope', '$scope', '$parse', 'logger', '$detour', '$resource', '$stateParams', '$location', 'commonDataService', 'columnDefinitionService',
      function ($rootScope, $scope, $parse, logger, $detour, $resource, $stateParams, $location, commonDataService, columnDefinitionService) {

          $scope.toolButtonDisplay = false;
          $scope.isInit = true;
          $scope.definitionViews = [];
          $scope.columnDefs = [];

          //init pagingoption
          var pageSizes = [50, 100, 200];
          var currentPage = parseInt($location.$$search['Page']);
          if (!currentPage) currentPage = 1;
          var pageSize = parseInt($location.$$search['Rows']);
          if (!pageSize | pageSizes.indexOf(pageSize) < 0) pageSize = 50;
          
          var getPostData = function () {
              return {
                  ViewId: $scope.viewId
              };
          };
          $scope.getPagedDataAsync = function () {
              $("#referenceList").jqGrid('setGridParam', {
                  postData: getPostData()
              }).trigger('reloadGrid');
          };

          $scope.$watch('filterOptions', function (newVal, oldVal) {
              if (newVal !== oldVal) {
                  $scope.getPagedDataAsync();
              }
          }, true);

          $scope.getRelatedData = function () {
              var gridColumns = columnDefinitionService.query({ contentType: $scope.entityTypeName, viewId: $scope.viewId }, function() {
                  $.each(gridColumns, function (index, value) {
                      if (value.formatter) {
                          value.formatter = $rootScope[value.formatter];
                      }
                  });
                  $scope.columnDefs = gridColumns;
                  if (!$scope.isInit) {
                      $scope.getPagedDataAsync();
                  } else {
                      $scope.gridOptions = {
                          url: 'api/CoeveryCore/Common/' + $scope.entityTypeName,
                          mtype: "post",
                          postData: getPostData(),
                          rowNum: pageSize,
                          rowList: pageSizes,
                          page: currentPage,
                          colModel: $scope.columnDefs,
                          loadComplete: function (data) {
                              currentPage = data.page;
                              pageSize = data.records;
                          },
                          loadError: function (xhr, status, error) {
                              logger.error("Failed to fetched records for " + moduleName + ":\n" + error);
                          }
                      };
                      angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);
                  }
              }, function() {
              });
          };

          /*Grid Methods*/
          $scope.Refresh = function () {
              $scope.getPagedDataAsync();
          };

          $scope.delete = function (id) {
              $scope.entityId = id;
          };

          $rootScope.deleteRelationship = function () {
              if (!$scope.entityId) {
                  logger.error('No data selected.');
                  return;
              }
              var ids;
              if ($.isArray($scope.entityId)) {
                  ids = $scope.entityId.join(",");
              } else {
                  ids = $scope.entityId.toString();
              }
              commonDataService.delete({ contentId: ids }, function () {
                  $scope.Refresh();
                  logger.success('Delete the relationship successful.');
                  $scope.entityId = [];
                  $scope.selectedItems = [];
              }, function () {
                  logger.error('Failed to delete the relationship');
              });
          };

          $scope.add = function () {
              $detour.transitionTo('Create', { NavigationId: null , Module: $scope.entityTypeName });
          };

          $scope.edit = function (id) {
              if (!id && $scope.selectedItems.length > 0) {
                  id = $scope.selectedItems[0];
              }
              $detour.transitionTo('Detail', { NavigationId: null, Module: $scope.entityTypeName, Id: id });
          };
          
          $scope.view = function (id) {
              if (!id && $scope.selectedItems.length > 0) {
                  id = $scope.selectedItems[0];
              }
              $detour.transitionTo('View', { NavigationId: $stateParams.NavigationId, Module: $scope.entityTypeName, Id: id });
          };
      }]
    ]);
});