define(['core/app/couchPotatoService', 'core/services/commondataservice'], function (couchPotato) {
    debugger;
    couchPotato.registerController([
      'GeneralListCtrl',
      ['$rootScope', '$scope', 'logger', '$state', 'localize', '$resource', '$stateParams', 'commonDataService',
      function ($rootScope, $scope, logger, $state, localize, $resource, $stateParams, commonDataService) {
          var moduleName = $rootScope.$stateParams.Module;
          var columnDefs = getColumnDefs(localize);
          
          $scope.mySelections = [];

          $scope.gridOptions = {
              data: 'myData',
              selectedItems: $scope.mySelections,
              multiSelect: false,
              showColumnMenu: true,
              enableColumnResize: true,
              enableColumnReordering: true,
              columnDefs: columnDefs
          };

          $scope.delete = function () {
              if ($scope.mySelections.length > 0) {
                  commonDataService.delete({ contentType: $scope.mySelections[0].ContentId }, function () {
                      $scope.mySelections.pop();
                      $scope.getAll();
                      logger.success('Delete the ' + moduleName + ' successful.');
                  }, function () {
                      logger.error('Failed to delete the lead.');
                  });
              }
          };

          $scope.add = function () {
              $state.transitionTo('Create', { Module: moduleName });
          };

          $scope.edit = function (id) {
              //if ($scope.mySelections.length > 0) {
              $state.transitionTo('Detail', { Module: moduleName, Id: id });
              //}
          };
          $scope.createnewview = function () {
              $state.transitionTo('SubCreate', { Module: 'Metadata', SubModule: 'Projection', Id: $stateParams.Module });
          };

          $scope.editview = function () {
              $state.transitionTo('SubDetail', { Module: 'Metadata', SubModule: 'Projection', View: 'Edit', Id: $stateParams.Module, SubId: viewId });
          };

          $scope.getAll = function () {
              var records = commonDataService.query(function () {
                  $scope.myData = records;
              }, function () {
                  logger.error("Failed to fetched records for " + moduleName);
              });
          };

          $scope.getAll();
      }]
    ]);
});


//@ sourceURL=Coevery.Core/listcontroller.js