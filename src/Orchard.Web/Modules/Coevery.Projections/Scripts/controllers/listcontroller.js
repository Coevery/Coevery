'use strict';

define(['core/app/couchPotatoService', 'Modules/Coevery.Projections/Scripts/services/projectiondataservice'], function (couchPotato) {
    couchPotato.registerController([
      'EntityListCtrl',
      ['$rootScope', '$scope', 'logger', '$state', '$resource', '$stateParams', 'projectionDataService',
      function ($rootScope, $scope, logger, $state, $resource, $stateParams, projectionDataService) {
          var moduleName = $stateParams.Module;
          $stateParams.Id = 'Leads';
          var t = function (str) {
              var result = i18n.t(str);
              return result;
          };
          
          var columnDefs = [
            { field: 'ContentId', displayName: 'Actions', width: 100, cellTemplate: '<div class="ngCellText" ng-class="col.colIndex()"><a ng-click="edit(row.getProperty(col.field))">Edit</a>&nbsp;<a ng-click="delete(row.getProperty(col.field))">Delete</a></div>' },
            { field: 'ContentId', displayName: t('Id') },
            { field: 'EntityType', displayName: t('EntityType') },
            { field: 'DisplayName', displayName: t('DisplayName') }];
          $scope.mySelections = [];

          $scope.gridOptions = {
              data: 'myData',
              //showSelectionCheckbox: true,
              selectedItems: $scope.mySelections,
              multiSelect: true,
              enableColumnResize: true,
              enableColumnReordering: true,
              columnDefs: columnDefs
          };

          $scope.delete = function (id) {
              projectionDataService.delete({ Id: id }, function () {
                  $scope.getAll();
                  logger.success('Delete the ' + moduleName + ' successful.');
              }, function () {
                  logger.error('Failed to delete the ' + moduleName);
              });
          };

          $scope.add = function () {
              $state.transitionTo('Create', { Module: 'Projections', Id: $stateParams.Id });
          };

          $scope.edit = function (id) {
              $state.transitionTo('Detail', { Module: 'Projections', Id: id});
          };

          $scope.getAll = function () {
              var records = projectionDataService.query({ Name: $stateParams.Id }, function () {
                  $scope.myData = records;
              }, function () {
                  logger.error("Failed to fetched projections for " + $stateParams.Id);
              });
          };

          $scope.getAll();
      }]
    ]);
});
//@ sourceURL=Coevery.Projections/listcontroller.js