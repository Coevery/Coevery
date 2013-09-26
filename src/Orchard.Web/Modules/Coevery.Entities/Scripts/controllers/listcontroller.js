'use strict';

define(['core/app/detourService', 'Modules/Coevery.Entities/Scripts/services/entitydataservice'], function (detour) {
    detour.registerController([
      'EntityListCtrl',
      ['$rootScope', '$scope', 'logger', '$detour', '$resource', '$stateParams', 'entityDataService',
      function ($rootScope, $scope, logger, $detour, $resource, $stateParams, entityDataService) {
          var t = function (str) {
              var result = i18n.t(str);
              return result;
          };

          var metadataColumnDefs = [
              { name: 'Id', label: 'Id', hidden: true, sorttype: 'int' },
              {
                  name: 'Name', label: 'Name',
                  formatter: $rootScope.cellLinkTemplate,
                  formatoptions: { hasView: true }
              },
              {
                  name: 'DisplayName', label: t('Display Name')
              }];

          $scope.gridOptions = {
              url: "api/entities/entity",
              colModel: metadataColumnDefs,
          };

          angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);

          $scope.delete = function (entityName) {
              var deleteEntity = entityName || $scope.selectedItems.length > 0 ? $scope.selectedItems[0] : null;
              if (!deleteEntity) return;
              entityDataService.delete({ name: deleteEntity }, function () {
                  if ($scope.selectedItems.length != 0) {
                      $scope.selectedItems.pop();
                  }
                  $scope.getAllMetadata();
                  logger.success("Delete the entity successful.");
                  window.location.reload();
              }, function (reason) {
                  logger.error("Failed to delete the entity:" + reason);
              });
          };

          $scope.add = function () {
              $detour.transitionTo('EntityCreate', { Module: 'Entities' });
          };

          $scope.view = function (entityName) {
              $detour.transitionTo('EntityDetail.Fields', { Id: entityName });
          };

          $scope.edit = function (entityName) {
              $detour.transitionTo('EntityEdit', { Id: entityName });
          };

          $scope.getAllMetadata = function () {
              $("#gridList").jqGrid('setGridParam', {
                  datatype: "json"
              }).trigger('reloadGrid');
          };
      }]
    ]);
});
