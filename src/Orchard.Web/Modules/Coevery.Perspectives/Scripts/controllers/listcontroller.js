'use strict';
define(['core/app/detourService', 'Modules/Coevery.Perspectives/Scripts/services/perspectivedataservice'], function (detour) {
    detour.registerController([
      'PerspectiveListCtrl',
      ['$rootScope', '$scope', 'logger', '$detour', '$resource', '$stateParams', 'perspectiveDataService',
      function ($rootScope, $scope, logger, $detour, $resource, $stateParams, perspectiveDataService) {
          var cellTemplateString = '<div class="ngCellText" ng-class="col.colIndex()" title="{{COL_FIELD}}">' +
          '<ul class="row-actions pull-right hide">' +
          '<li class="icon-edit" ng-click="edit(row.entity.Id)" title="Edit"></li>' +
          '<li class="icon-remove" ng-click="delete(row.entity.Id)" title="Delete"></li>' +
          '</ul>' +
          '<span class="btn-link" ng-click="view(row.entity.Id)">{{COL_FIELD}}</span>' +
          '</div>';
          $scope.mySelections = [];
          var moduleName = $stateParams.Module;
          var t = function (str) {
              var result = i18n.t(str);
              return result;
          };
          
          var perspectiveColumnDefs = [
              { field: 'DisplayName', displayName: t('DisplayName'), cellTemplate: cellTemplateString }];

          $scope.gridOptions = {
              data: 'myData',
              multiSelect: true,
              enableRowSelection: true,
              showSelectionCheckbox: true,
              selectedItems: $scope.mySelections,
              columnDefs: perspectiveColumnDefs,
          };
          
          angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);

          $scope.delete = function (id) {
              perspectiveDataService.delete({ Id: id }, function () {
                  $scope.getAllPerspective();
                  logger.success('Delete the ' + moduleName + ' successful.');
              }, function () {
                  logger.error('Failed to delete the ' + moduleName);
              });
          };

          $scope.addPerspective = function () {
              $detour.transitionTo('PerspectiveCreate', { Module: 'Perspectives' });
          };

          $scope.edit = function (id) {
              $detour.transitionTo('PerspectiveEdit', { Id: id });
          };
          
          $scope.view = function (id) {
              $detour.transitionTo('PerspectiveDetail', { Id: id });
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