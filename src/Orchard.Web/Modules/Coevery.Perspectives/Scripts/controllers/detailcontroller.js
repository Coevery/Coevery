'use strict';
define(['core/app/couchPotatoService',
        'Modules/Coevery.Perspectives/Scripts/services/perspectivedataservice',
        'Modules/Coevery.Perspectives/Scripts/services/navigationdataservice'],function (couchPotato) {
    couchPotato.registerController([
      'PerspectivesDetailCtrl',
      ['$timeout', '$scope', 'logger', '$state', '$stateParams',
          '$resource', 
          'perspectiveDataService',
          'navigationDataService',
      function ($timeout, $scope, logger, $state, $stateParams, $resource,perspectiveDataService, navigationDataService) {
          $scope.mySelections = [];
          var moduleName = $stateParams.Module;
          var perpectiveId = $stateParams.Id;

          $scope.exit = function () {
              $state.transitionTo('List', { Module: 'Perspectives'});
          };

          $scope.save = function () {
              $.ajax({
                  url: myForm.action,
                  type: myForm.method,
                  data: $(myForm).serialize() + '&submit.Save=Save',
                  success: function (result) {
                      logger.success("Perspective Saved.");
                  }
              });
          };

          var t = function (str) {
              var result = i18n.t(str);
              return result;
          };
          
          var navigationColumnDefs = [
              { field: 'Id', displayName: 'Actions', width: 100, cellTemplate: '<div class="ngCellText" ng-class="col.colIndex()"><a ng-click="edit(row.getProperty(col.field))">Edit</a>&nbsp;<a ng-click="delete(row.getProperty(col.field))">Remove</a></div>' },
              { field: 'Id', displayName: t('Id') },
              { field: 'DisplayName', displayName: t('DisplayName') }];

          $scope.gridOptions = {
              data: 'myData',
              selectedItems: $scope.mySelections,
              multiSelect: false,
              showColumnMenu: true,
              enableColumnResize: true,
              enableColumnReordering: true,
              columnDefs: navigationColumnDefs
          };

          

          $scope.addNavigationItem = function () {
              $state.transitionTo('SubDetail', { Module: 'Metadata', SubModule: 'Perspective', Id: perpectiveId, SubId: 0, View: 'EditNavigationItem' });
          };

          

          $scope.edit = function (navigationId) {
              $state.transitionTo('SubDetail', { Module: 'Metadata', SubModule: 'Perspective', Id: perpectiveId, SubId: navigationId, View: 'EditNavigationItem' });
          };

          $scope.delete = function (navigationId) {
              perspectiveDataService.delete({ Id: navigationId }, function () {
                  $scope.getAllNavigationdata();
                  logger.success('Delete the ' + moduleName + ' successful.');
              }, function () {
                  logger.error('Failed to delete the ' + moduleName);
              });
          };

          $scope.deletePerspective = function () {
              navigationDataService.delete({ Id: perpectiveId }, function () {
                  $scope.exit();
              }, function () {
                  logger.error('Failed to delete the ' + moduleName);
              });
          };


          $scope.getAllNavigationdata = function () {
              var navigations = navigationDataService.query({ Id: perpectiveId }, function () {
                  $scope.myData = navigations;
              }, function () {
                  logger.error("Failed to fetched Metadata.");
              });
          };
          $scope.getAllNavigationdata();
      }]
    ]);
});