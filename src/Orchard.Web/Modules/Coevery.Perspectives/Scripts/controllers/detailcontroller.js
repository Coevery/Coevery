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
          '<li class="icon-edit" ng-click="edit(row.entity.ID)" title="Edit"></li>' +
          '<li class="icon-remove" ng-click="delete(row.entity.ID)" title="Delete"></li>' +
          '</ul>' +
          '<span class="btn-link" ng-click="edit(row.entity.ID)">{{COL_FIELD}}</span>' +
          '</div>';
          $scope.mySelections = [];
          var moduleName = $stateParams.Module;
          var perpectiveId = $stateParams.Id;

          $scope.exit = function () {
              $detour.transitionTo('PerspectiveList');
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