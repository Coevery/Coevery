'use strict';

define(['core/app/detourService', 'Modules/Coevery.Entities/Scripts/services/entitydataservice'], function (detour) {
    detour.registerController([
      'EntityListCtrl',
      ['$rootScope', '$scope', 'logger', '$state', '$resource', '$stateParams', 'entityDataService', '$http',
      function ($rootScope, $scope, logger, $state, $resource, $stateParams, entityDataService, $http) {
          var t = function (str) {
              var result = i18n.t(str);
              return result;
          };

          $scope.idAttr = "Name"; //The attribute represent the id of a row
          var metadataColumnDefs = [              
              {
                  name: 'Name', label: 'Name',
                  formatter: $rootScope.cellLinkTemplate,
                  formatoptions: { hasView: true }
              },
              { name: 'Id', label: 'Id', hidden: true, sorttype: 'int' },
              { name: 'DisplayName', label: t('Display Name') },
              { name: 'Modified', label: t('Has Unpublished Modification') },
              { name: 'HasPublished', label: t('Has Ever Published') }
          ];

          $scope.gridOptions = {
              url: "api/entities/entity",
              colModel: metadataColumnDefs,
          };

          angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);

          $scope.delete = function () {
              var deleteEntity = $scope.selectedItems.length > 0 ? $scope.selectedRow[0].Id : null;
              if (!deleteEntity) return;
              entityDataService.delete({ name: deleteEntity }, function () {
                  if ($scope.selectedItems.length != 0) {
                      $scope.selectedItems = [];
                  }
                  $scope.getAllMetadata();
                  logger.success("Delete the entity successful.");
                  //window.location.reload();
              }, function (reason) {
                  logger.error("Failed to delete the entity:" + reason);
              });
          };
          $scope.add = function () {
              $state.transitionTo('EntityCreate', { Module: 'Entities' });
          };

          $scope.view = function (entityName) {
              $state.transitionTo('EntityDetail.Fields', { Id: entityName });
          };

          $scope.edit = function (entityName) {
              $state.transitionTo('EntityEdit', { Id: entityName });
          };

          $scope.getAllMetadata = function () {
              $("#gridList").jqGrid('setGridParam', {
                  datatype: "json"
              }).trigger('reloadGrid');
          };

          $scope.publish = function() {
              $http.get('Entities/SystemAdmin/Publish/' + $scope.selectedItems[0]).then(function () {
                  $scope.getAllMetadata();
                  $scope.selectedItems = [];
              });
          };
      }]
    ]);
});
