'use strict';
define(['core/app/couchPotatoService', 'Modules/Coevery.Perspectives/Scripts/services/perspectivedataservice'], function (couchPotato) {
    couchPotato.registerController([
      'PerspectivesListCtrl',
      ['$rootScope', '$scope', 'logger', '$state', '$resource', '$stateParams', 'perspectiveDataService',
      function ($rootScope, $scope, logger, $state, $resource, $stateParams, perspectiveDataService) {
          var cellTemplateString = '<div class="ngCellText" ng-class="col.colIndex()"><a href ="#/Metadata/{{row.entity.Name}}" class="ngCellText">{{row.entity.DisplayName}}</a></div>';
          $scope.mySelections = [];
          var moduleName = $stateParams.Module;
          var t = function (str) {
              var result = i18n.t(str);
              return result;
          };
          
          var perspectiveColumnDefs = [
              { field: 'Id', displayName: 'Actions', width: 100, cellTemplate: '<div class="ngCellText" ng-class="col.colIndex()"><a ng-click="edit(row.getProperty(col.field))">Edit</a>&nbsp;<a ng-click="delete(row.getProperty(col.field))">Remove</a></div>' },
              { field: 'Id', displayName: 'Id' },
              { field: 'DisplayName', displayName: t('DisplayName'), cellTemplate: cellTemplateString }];

          $scope.gridOptions = {
              data: 'myData',
              selectedItems: $scope.mySelections,
              multiSelect: false,
              showColumnMenu: true,
              enableColumnResize: true,
              enableColumnReordering: true,
              columnDefs: perspectiveColumnDefs
          };

          $scope.delete = function (id) {
              perspectiveDataService.delete({ Id: id }, function () {
                  $scope.getAllPerspective();
                  logger.success('Delete the ' + moduleName + ' successful.');
              }, function () {
                  logger.error('Failed to delete the ' + moduleName);
              });
          };

          $scope.addPerspective = function () {
              $state.transitionTo('Create', { Module: 'Perspectives'});
          };

          $scope.edit = function (id) {
              $state.transitionTo('Detail', { Module: 'Perspectives', Id: id});
          };


          $scope.getAllPerspective = function () {
              var perspectives = perspectiveDataService.query(function () {
                  $scope.myData = perspectives;
              }, function () {
                  logger.error("Failed to fetched Metadata.");
              });
          };
          $scope.getAllPerspective();
      }]
    ]);
});
//@ sourceURL=Coevery.Perspectives/listcontroller.js