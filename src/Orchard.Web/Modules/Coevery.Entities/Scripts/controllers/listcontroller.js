'use strict';

define(['core/app/detourService', 'Modules/Coevery.Entities/Scripts/services/entitydataservice', 'Modules/Coevery.Entities/Scripts/services/generationservice'], function (detour) {
    detour.registerController([
      'EntityListCtrl',
      ['$rootScope', '$scope', 'logger', '$detour', '$resource', '$stateParams', 'entityDataService', 'generationService',
      function ($rootScope, $scope, logger, $detour, $resource, $stateParams, entityDataService, generationService) {
          var cellTemplateString = '<div class="ngCellText" ng-class="col.colIndex()"><a href ="#/Entities/{{row.entity.Name}}" class="ngCellText">{{row.entity.DisplayName}}</a></div>';
          $scope.mySelections = [];
          
          var t = function (str) {
              var result = i18n.t(str);
              return result;
          };
          
          var metadataColumnDefs = [
              { field: 'Name', displayName: 'Actions', width: 100, cellTemplate: '<div class="ngCellText" ng-class="col.colIndex()"><a ng-click="edit(row.getProperty(col.field))">Edit</a>&nbsp;<a ng-click="delete(row.getProperty(col.field))">Remove</a></div>' },
              { field: 'DisplayName', displayName: t('DisplayName'), cellTemplate: cellTemplateString },
              { field: 'IsDeployed', displayName: t('IsDeployed') }];

          $scope.gridOptions = {
              data: 'myData',
              selectedItems: $scope.mySelections,
              multiSelect: false,
              showColumnMenu: true,
              enableColumnResize: true,
              enableColumnReordering: true,
              columnDefs: metadataColumnDefs
          };

          $scope.delete = function (entityName) {
              entityDataService.delete({ name: entityName }, function () {
                  $scope.mySelections.pop();
                  $scope.getAllMetadata();
                  logger.success("Delete the metadata successful.");
              }, function () {
                  logger.error("Failed to delete the metadata.");
              });
          };

          $scope.add = function () {
              $detour.transitionTo('Create', { Module: 'Entities' });
          };

          $scope.edit = function (entityName) {
              $detour.transitionTo('EntityDetail', { Id: entityName });
          };

          $scope.getAllMetadata = function () {
              var metadatas = entityDataService.query(function () {
                  $scope.myData = metadatas;
              }, function () {
                  logger.error("Failed to fetched Metadata.");
              });
          };

          $scope.generate = function () {
              generationService.save({ name: '' }, function () {
                  $scope.getAllMetadata();
                  logger.success("Generate metadata successful.");
              }, function () {
                  logger.error("Failed to Generate the metadata.");
              });
          };


          $scope.getAllMetadata();
      }]
    ]);
});