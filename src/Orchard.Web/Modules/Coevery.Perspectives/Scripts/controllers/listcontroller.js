'use strict';
define(['core/app/detourService', 'Modules/Coevery.Perspectives/Scripts/services/perspectivedataservice'], function (detour) {
    detour.registerController([
      'PerspectiveListCtrl',
      ['$rootScope', '$scope', 'logger', '$state', '$resource', '$stateParams', 'perspectiveDataService',
      function ($rootScope, $scope, logger, $state, $resource, $stateParams, perspectiveDataService) {

          var t = function (str) {
              var result = i18n.t(str);
              return result;
          };

          var perspectiveColumnDefs = [
              { name: 'Id', label: t('Id'), hidden: true },
              {
                  name: 'DisplayName', label: t('DisplayName'), 
                  formatter: $rootScope.cellLinkTemplate,
                  formatoptions: { hasView: true }
              }];

          $scope.gridOptions = {
              url: "api/perspectives/Perspective",
              colModel: perspectiveColumnDefs
          };

          angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);

          $scope.delete = function (id) {
              //$scope.perspectiveId = id;
              perspectiveDataService.delete({ id: id }, function () {
                  $scope.getAllPerspective();
                  logger.success('Delete the perspective successful.');
              }, function (result) {
                  logger.error('Failed to delete the perspective:' + result.data.Message);
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
//@ sourceURL=Coevery.Perspectives/listcontroller.js