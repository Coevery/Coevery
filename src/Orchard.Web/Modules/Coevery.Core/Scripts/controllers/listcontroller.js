define(['core/app/couchPotatoService', 'core/services/commondataservice'], function (couchPotato) {
    couchPotato.registerController([
      'GeneralListCtrl',
      ['$rootScope', '$scope', 'logger', '$state', '$resource', '$stateParams', 'commonDataService',
      function ($rootScope, $scope, logger, $state, $resource, $stateParams, commonDataService) {
          var moduleName = $rootScope.$stateParams.Module;

          var t = function (str) {
              var result = i18n.t(str);
              return result;
          };

          var columnDefs = [{ field: 'Id', displayName: t('Id') },
                { field: 'Topic', displayName: t('Topic') },
                { field: 'StatusCode', displayName: t('StatusCode') },
                { field: 'FirstName', displayName: t('FirstName') },
                { field: 'LastName', displayName: t('LastName') }];
          

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
              var records = commonDataService.query(function() {
                  //$scope.myData = records;
                  $scope.myData = [{ 'Id': 1, 'Topic': 't1', 'StatusCode': '1', 'FirstName': 's1', 'LastName': 'c1' },
                      { 'Id': 2, 'Topic': 't2', 'StatusCode': '2', 'FirstName': 's2', 'LastName': 'c2' }
                  ];
              }, function () {
                  logger.error("Failed to fetched records for " + moduleName);
              });
          };

          $scope.getAll();
      }]
    ]);
});