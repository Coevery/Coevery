'use strict';
define(['core/app/detourService',
        'Modules/Coevery.Perspectives/Scripts/services/perspectivedataservice',
        'Modules/Coevery.Perspectives/Scripts/services/navigationdataservice'], function (detour) {
            detour.registerController([
      'PerspectiveDetailCtrl',
      ['$rootScope', '$timeout', '$scope', 'logger', '$detour', '$stateParams',
          '$resource', 
          'perspectiveDataService',
          'navigationDataService',
      function ($rootScope, $timeout, $scope, logger, $detour, $stateParams, $resource, perspectiveDataService, navigationDataService) {

          var cellTemplateString = '<div class="ngCellText" ng-class="col.colIndex()" title="{{COL_FIELD}}">' +
              '<ul class="row-actions pull-right hide">' +
              '<li class="icon-edit" ng-click="edit(row.entity.Id)" title="Edit"></li>' +
              '<li class="icon-remove" ng-click="delete(row.entity.Id)" title="Delete"></li>' +
              '</ul>' +
              '<span class="btn-link" ng-click="edit(row.entity.Id)">{{COL_FIELD}}</span>' +
              '</div>';
          $scope.mySelections = [];
          var perpectiveId = $stateParams.Id;

          $scope.exit = function () {
              $detour.transitionTo('PerspectiveList');
          };

          $scope.save = function () {
              var tempForm = $("form[name=myForm]");
              $.ajax({
                  url: tempForm.action,
                  type: tempForm.method,
                  data: tempForm.serialize() + '&submit.Save=Save',
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
              { field: 'DisplayName', displayName: t('DisplayName'), cellTemplate: cellTemplateString }];

          $scope.gridOptions = {
              data: 'myData',
              selectedItems: $scope.mySelections,
              multiSelect: false,
              enableRowSelection: false,
              showColumnMenu: true,
              enableColumnResize: true,
              enableColumnReordering: true,
              columnDefs: navigationColumnDefs
          };
          angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);

          $scope.addNavigationItem = function () {
              $detour.transitionTo('CreateNavigationItem', { Id: perpectiveId });
          };

          $scope.edit = function (navigationId) {
              $detour.transitionTo('EditNavigationItem', {Id:perpectiveId, NId: navigationId });
          };
          
          $scope.editPerspective = function () {
              $detour.transitionTo('PerspectiveEdit', { Id: perpectiveId });
          };
          

          $scope.delete = function (navigationId) {
              $scope.navigationId = navigationId;
              $('#myModalNavigation').modal({
                  backdrop: 'static',
                  keyboard: true
              });
              
          };

          $scope.deleteNavigation = function () {
              $('#myModalNavigation').modal('hide');
              perspectiveDataService.delete({ Id: $scope.navigationId }, function () {
                  $scope.getAllNavigationdata();
                  logger.success('Delete the navigation successful.');
              }, function (result) {
                  logger.error('Failed to delete the navigation:' + result.data.Message);
              });
          };

          $scope.deletePerspective = function () {
              $('#myModalPerspective').modal({
                  backdrop: 'static',
                  keyboard: true
              });
          };

          $scope.deletePerspectiveInDetails = function () {
              $('#myModalPerspective').modal('hide');
              perspectiveDataService.delete({ id: perpectiveId }, function () {
                  $scope.exit();
                  logger.success('Delete the perspective successful.');
              }, function (result) {
                  logger.error('Failed to delete the perspective:' + result.data.Message);
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