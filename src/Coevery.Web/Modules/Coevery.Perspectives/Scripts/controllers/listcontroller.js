'use strict';
define(['core/app/detourService', 'Modules/Coevery.Perspectives/Scripts/services/perspectivedataservice'], function (detour) {
    detour.registerController([
      'PerspectiveListCtrl',
      ['$rootScope', '$scope', 'logger', '$state', '$resource', '$stateParams', 'perspectiveDataService', '$i18next',
      function ($rootScope, $scope, logger, $state, $resource, $stateParams, perspectiveDataService, $i18next) {

          var perspectiveColumnDefs = [
              { name: 'Id', label: $i18next('Id'), hidden: true, key: true, sortable: false },
              {
                  name: 'DisplayName', label: $i18next('DisplayName'),
                  formatter: $rootScope.cellLinkTemplate,
                  formatoptions: { hasView: true }
              },
              {
                  name: 'Description', label: $i18next('Description')
              }];

          var gridOptions = {
              url: "api/perspectives/Perspective",
              colModel: perspectiveColumnDefs
          };

          angular.extend(gridOptions, $rootScope.defaultGridOptions);
          gridOptions.multiselect = false;
          $scope.gridOptions = gridOptions;

          $scope.delete = function (id) {
              //$scope.perspectiveId = id;
              perspectiveDataService.delete({ id: id }, function () {
                  $scope.getAllPerspective();
                  logger.success($i18next('Delete the perspective successful.'));
              }, function (result) {
                  logger.error($i18next('Failed to delete the perspective:') + result.data.Message);
              });
          };

          $scope.addPerspective = function () {
              $state.transitionTo('PerspectiveCreate', { Module: 'Perspectives' });
          };

          $scope.edit = function (id) {
              $state.transitionTo('PerspectiveEdit', { Id: id });
          };

          $scope.view = function (id) {
              $state.transitionTo('PerspectiveDetail', { Id: id });
          };

          $scope.getAllPerspective = function () {
              $("#perspectiveList").jqGrid('setGridParam', {
                  datatype: "json"
              }).trigger('reloadGrid');
          };
      }]
    ]);
});