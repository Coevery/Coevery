'use strict';
define(['core/app/detourService', 'Modules/Coevery.Perspectives/Scripts/services/perspectivedataservice'], function (detour) {
    detour.registerController([
      'PerspectiveListCtrl',
      ['$rootScope', '$scope', 'logger', '$detour', '$resource', '$stateParams', 'perspectiveDataService',
      function ($rootScope, $scope, logger, $detour, $resource, $stateParams, perspectiveDataService) {
          $scope.mySelections = [];
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
              $scope.perspectiveId = id;
              $('#myModalPerspective').modal({
                  backdrop: 'static',
                  keyboard: true
              });
          };

          $scope.deletePerspective = function () {
              $('#myModalPerspective').modal('hide');
              perspectiveDataService.delete({ id: $scope.perspectiveId }, function () {
                  $scope.getAllPerspective();
                  logger.success('Delete the perspective successful.');
              }, function (result) {
                  logger.error('Failed to delete the perspective:' + result.data.Message);
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
              $("#perspectiveList").jqGrid('setGridParam', {
                  datatype: "json"
              }).trigger('reloadGrid');
          };
      }]
    ]);
});
//@ sourceURL=Coevery.Perspectives/listcontroller.js