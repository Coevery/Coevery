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

          var cellLinkTemplate = function (cellvalue, options, rowObject) {
              return '<ul class="row-actions pull-right hide">' +
                  '<li class="icon-edit edit-action" data-id="' + options.rowId + '" title="Edit"></li>' +
                  '<li class="icon-remove delete-action" data-id="' + options.rowId + '" title="Delete"></li>' +
                  '</ul>' +
                  '<span class="btn-link view-action" data-id="' + options.rowId + '">' + cellvalue +
                  '</span>';
          };

          //var columns = [t('DisplayName'), t('IsDeployed')];
          var columns = ['Id', 'DisplayName', 'IsDeployed'];

          var metadataColumnDefs = [
              { "name": columns[0], "index": columns[0], hidden: true },
              {
                  "name": columns[1], "index": columns[1], width: 450, formatter: cellLinkTemplate, align: 'center'
              },
              { "name": columns[2], "index": columns[2], width: 450, align: 'center' }];

          $scope.selectedItems = [];
          $scope.gridOptions = {
              datatype: "json",
              url: "api/entities/entity/request",
              colNames: columns,
              colModel: metadataColumnDefs,
              rowNum: 10,
              rowList: [10, 20, 50],
          };

          angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);

          $scope.delete = function (entityName) {
              $scope.entityName = entityName;
              $('#myModalEntity').modal({
                  backdrop: 'static',
                  keyboard: true
              });
          };

          $scope.deleteEntity = function () {
              $('#myModalEntity').modal('hide');
              entityDataService.delete({ name: $scope.entityName }, function () {
                  if ($scope.selectedItems.length != 0) {
                      $scope.selectedItems.pop();
                  }
                  $scope.getAllMetadata();
                  logger.success("Delete the entity successful.");
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
              $("#gridList").trigger("reloadGrid");
          };

      }]
    ]);
});

//Abondoned codes
/*
$scope.pagingOptions = {
              pageSizes: [50, 100, 200],
              pageSize: 50,
              currentPage: 1
          };


var cellTemplateString = '<div class="ngCellText" ng-class="col.colIndex()" title="{{COL_FIELD}}">' +
              '<ul class="row-actions pull-right hide">' +
              '<li class="icon-edit" ng-click="edit(row.entity.Name)" title="Edit"></li>' +
              '<li class="icon-remove" ng-click="delete(row.entity.Name)" title="Delete"></li>' +
              '</ul>' +
              '<span class="btn-link" ng-click="view(row.entity.Name)">{{COL_FIELD}}</span>' +
              '</div>';
          $scope.selectedItems = [];

          var metadataColumnDefs = [
              { field: 'DisplayName', displayName: t('DisplayName'), cellTemplate: cellTemplateString },
              { field: 'IsDeployed', displayName: t('IsDeployed') }];

              var metadatas = entityDataService.query(function () {
                  $scope.myData = metadatas;
                  $scope.totalServerItems = metadatas.length;
              }, function () {
                  logger.error("Failed to fetched Metadata.");
              });
*/
