

define(['core/app/couchPotatoService', 'core/services/commondataservice', 'core/services/nggriddataservice'], function (couchPotato) {
    couchPotato.registerController([
      'GeneralListCtrl',
      ['$rootScope', '$scope', 'logger', '$state', '$resource', '$stateParams', 'commonDataService', 'ngGridDataService',
      function ($rootScope, $scope, logger, $state, $resource, $stateParams, commonDataService, ngGridDataService) {
          var moduleName = $rootScope.$stateParams.Module;

          $scope.pagingOptions = {
              pageNumber: [],
              pageSize: 13,
              totalServerItems: 0,
              currentPage: 1
          };
          $scope.setPagingData = function (data, page, pageSize) {
              var pagedData = data.slice((page - 1) * pageSize, page * pageSize);
              var maxRow = data.length;
              var maxPages = Math.ceil(maxRow / $scope.pagingOptions.pageSize);
            
              $scope.pagingOptions.pageNumber = [];
              for (var index = 0; index < maxPages; index++) {
                  $scope.pagingOptions.pageNumber[index] = index + 1;
              }
              $scope.myData = pagedData;
              $scope.pagingOptions.totalServerItems = data.length;
              if (!$scope.$$phase) {
                  $scope.$apply();
              }
          };

          $scope.getPagedDataAsync = function (pageSize, page) {
              setTimeout(function () {
                  var records = commonDataService.query(function () {
                      $scope.setPagingData(records, page, pageSize);
                  }, function () {
                      logger.error("Failed to fetched records for " + moduleName);
                  });
              }, 100);
          };

          $scope.$watch('pagingOptions', function (newVal, oldVal) {
              if (newVal !== oldVal && newVal.currentPage !== oldVal.currentPage) {
                  $scope.getPagedDataAsync($scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage);
              }
          }, true);

          $scope.$watch('filterOptions', function (newVal, oldVal) {
              if (newVal !== oldVal) {
                  $scope.getPagedDataAsync($scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage);
              }
          }, true);

          $scope.gridOptions = {
              data: 'myData',
              enablePaging: true,
              showFooter: true,
              pagingOptions: $scope.pagingOptions,
              columnDefs: "myColumns"
          };

          var gridColumns = ngGridDataService.query(function () {
              $scope.myColumns = gridColumns;
          }, function () {
              $scope.myColumns = [];
          });

          $scope.getPagedDataAsync($scope.pagingOptions.pageSize, $scope.pagingOptions.currentPage);


          $scope.GetScrollHeight = function () {
              var height = document.height - 220;
              return {"height":height +"px"};
          };
          

          


          //var t = function (str) {
          //    var result = i18n.t(str);
          //    return result;
          //};

          //var columnDefs = [{ field: 'Id', displayName: t('Id') },
          //      { field: 'Topic', displayName: t('Topic') },
          //      { field: 'StatusCode', displayName: t('StatusCode') },
          //      { field: 'FirstName', displayName: t('FirstName') },
          //      { field: 'LastName', displayName: t('LastName') }];


          //$scope.mySelections = [];

          //$scope.gridOptions = {
          //    data: 'myData',
          //    selectedItems: $scope.mySelections,
          //    multiSelect: false,
          //    showColumnMenu: true,
          //    enableColumnResize: true,
          //    enableColumnReordering: true,
          //    columnDefs: columnDefs
          //};

          var idIndex = 1;
          $scope.filters = [{ id: '1' }];
          $scope.addFilter = function () {
              idIndex++;
              $scope.filters.splice($scope.filters.length, 0, { id: idIndex });
          };

          $scope.removeFilter = function (index) {
              $scope.filters.splice(index, 1);
          };

          $scope.expendCollapse = function () {
              if ($('#collapseBtn').hasClass('icon-collapse-up')) {
                  $('#collapseBtn').addClass('icon-collapse-down');
                  $('#collapseBtn').removeClass('icon-collapse-up');
                  $('#closeFilterLink').css('display', '');

              } else {
                  $('#collapseBtn').removeClass('icon-collapse-down');
                  $('#collapseBtn').addClass('icon-collapse-up');
                  $('#closeFilterLink').css('display', 'none');
              }
          };

          $scope.closeFilterCollapse = function () {
              $('#filterCollapse').css('display', 'none');
          };

          $scope.openFilterCollapse = function (fiterId) {
              $('#filterCollapse').css('display', '');
              if ($('#collapseBtn').hasClass('icon-collapse-up')) return;
              $scope.expendCollapse();
              $('#collapseBtn').click();
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
              $state.transitionTo('Detail', { Module: moduleName, Id: id });
          };

          //$scope.createnewview = function () {
          //    $state.transitionTo('SubCreate', { Module: 'Metadata', SubModule: 'Projection', Id: $stateParams.Module });
          //};

          //$scope.editview = function () {
          //    $state.transitionTo('SubDetail', { Module: 'Metadata', SubModule: 'Projection', View: 'Edit', Id: $stateParams.Module, SubId: viewId });
          //};

          //$scope.getAll = function () {
          //    var records = commonDataService.query(function() {
          //        //$scope.myData = records;
          //        $scope.myData = [{ 'Id': 1, 'Topic': 't1', 'StatusCode': '1', 'FirstName': 's1', 'LastName': 'c1' },
          //            { 'Id': 2, 'Topic': 't2', 'StatusCode': '2', 'FirstName': 's2', 'LastName': 'c2' }
          //        ];
          //    }, function () {
          //        logger.error("Failed to fetched records for " + moduleName);
          //    });
          //};

          //$scope.getAll();
      }]
    ]);
});