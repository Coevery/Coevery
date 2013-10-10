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
          
          var navigationColumnDefs = [
              { name: 'Id', label: t('Id'), hidden: true },
              {
                  name: 'DisplayName', label: t('DisplayName'), 
                  formatter: $rootScope.cellLinkTemplate,
                  formatoptions: { hasView: true }
              }];

          $scope.gridOptions = {
              url: "api/perspectives/Navigation?id=" + perpectiveId,
              colModel: navigationColumnDefs
          };
          angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);

          $scope.addNavigationItem = function () {
              $state.transitionTo('CreateNavigationItem', { Id: perpectiveId });
          };

          $scope.edit = function (navigationId) {
              $state.transitionTo('EditNavigationItem', {Id:perpectiveId, NId: navigationId });
          };

          $scope.view = $scope.edit;

          $scope.editPerspective = function () {
              $state.transitionTo('PerspectiveEdit', { Id: perpectiveId });
          };
          

          $scope.delete = function (navigationId) {
              perspectiveDataService.delete({ Id: navigationId }, function () {
                  $scope.getAllNavigationdata();
                  logger.success('Delete the navigation successful.');
              }, function (result) {
                  logger.error('Failed to delete the navigation:' + result);
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


          $scope.getAllNavigationdata = function () {
              $("#navigationList").jqGrid('setGridParam', {
                  datatype: "json"
              }).trigger('reloadGrid');
          };
      }]
    ]);
});