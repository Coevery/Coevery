'use strict';
define(['core/app/detourService',
        'Modules/Coevery.Perspectives/Scripts/services/perspectivedataservice',
        'Modules/Coevery.Perspectives/Scripts/services/navigationdataservice'], function (detour) {
            detour.registerController([
      'PerspectiveDetailCtrl',
      ['$rootScope', '$timeout', '$scope', 'logger', '$state', '$stateParams',
          '$resource',
          'perspectiveDataService',
          'navigationDataService',
      function ($rootScope, $timeout, $scope, logger, $state, $stateParams, $resource, perspectiveDataService, navigationDataService) {
          var perpectiveId = $stateParams.Id;

          $scope.exit = function () {
              $state.transitionTo('PerspectiveList');
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

          var navigationColumnDefs = [{
              name: 'Id', label: t('Id'), key: true, sortable: false
          }, {
              name: 'DisplayName',
              label: t('DisplayName'),
              formatter: $rootScope.cellLinkTemplate,
              formatoptions: { hasView: true }
          }, { name: 'Description', label: t('Description'), },
              { name: 'Parent', label: t('Parent'), },
              { name: 'Weight', label: t('Weight'), },
              { name: 'Level', label: t('Level'), },
              { name: 'LeafOnly', label: t('Leaf Only'), }
          ];

          var gridOptions = {
              url: "api/perspectives/Navigation?id=" + perpectiveId,
              colModel: navigationColumnDefs,
              rowIdName: 'Id',
              nestedDrag: true,
              initialLevel: 1
          };
          angular.extend(gridOptions, $rootScope.defaultGridOptions);
          gridOptions.multiselect = false;
          gridOptions.sortable = false;
          $scope.gridOptions = gridOptions;

          $scope.addNavigationItem = function () {
              $state.transitionTo('CreateNavigationItem', { Id: perpectiveId });
          };

          $scope.edit = function (navigationId) {
              $state.transitionTo('EditNavigationItem', { Id: perpectiveId, NId: navigationId });
          };

          $scope.view = $scope.edit;

          $scope.editPerspective = function () {
              $state.transitionTo('PerspectiveEdit', { Id: perpectiveId });
          };

          $scope.saveDeployment = function () {
              var postdata = [];
              var depth = 1;
              var getPosition = function (parent, order) {
                  if (!parent) {
                      return order;
                  }
                  var parentRecord = $scope.navigationList.getRow(parent);
                  return getPosition(parentRecord.Parent, parentRecord.Weight) + "." + order;
              };
              $scope.navigationList.getParam("data").forEach(function (element, index, array) {
                  var currentLevel = element.Level;
                  depth = currentLevel > depth ? currentLevel : depth;
                  var position = getPosition(element.Parent, element.Weight);
                  postdata.push({
                      NavigationId: element.Id,
                      Position: position.toString()
                  });
              });
              navigationDataService.save( { Id: perpectiveId, Positions: postdata, Depth: depth }, function (data) {
                  logger.success(t('Save layout successful.'));
                  $scope.getAllNavigationdata(true);
              }, function (response) {
                  logger.error(t('Failed to save layout:' + response.data.Message));
              });
          };

          $scope.delete = function (navigationId) {
              perspectiveDataService.delete({ Id: navigationId }, function () {
                  $scope.getAllNavigationdata();
                  logger.success(t('Delete the navigation successful.'));
              }, function (result) {
                  logger.error(t('Failed to delete the navigation:' + result));
              });
          };

          $scope.deletePerspective = function () {
              perspectiveDataService.delete({ id: perpectiveId }, function () {
                  $scope.exit();
                  logger.success('Delete the perspective successful.');
              }, function (result) {
                  logger.error('Failed to delete the perspective:' + result.data.Message);
              });
          };


          $scope.getAllNavigationdata = function (needReconstruct) {
              if (needReconstruct) {
                  var reloadOptions = {
                      needReloading: true
                  };
                  angular.extend(reloadOptions, gridOptions);
                  $scope.gridOptions = reloadOptions;
                  return;
              }
              $scope.navigationList.jqGrid('setGridParam', {
                  datatype: "json"
              }).trigger('reloadGrid');
          };
      }]
            ]);
        });