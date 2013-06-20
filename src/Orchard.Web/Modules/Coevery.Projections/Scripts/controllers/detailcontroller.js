'use strict';
define(['core/app/couchPotatoService'
        , 'Modules/Coevery.Projections/Scripts/services/projectiondataservice'
        , 'Modules/Coevery.Projections/Scripts/services/viewmodeldataservice'], function (couchPotato) {
    couchPotato.registerController([
      'ProjectionDetailCtrl',
      ['$timeout', '$scope', 'logger', '$state', '$stateParams', '$resource', '$element', '$compile', 'projectionDataService','viewmodelDataService',
      function ($timeout, $scope, logger, $state, $stateParams, $resource, $element, $compile, projectionDataService, viewmodelDataService) {
          var name = $stateParams.Id;
          $scope.mySelections = [];
          $scope.fieldCoumns = [];
          $scope.SelectedColumns = [];
          
          $scope.gridOptions = {
              data: 'myData',
              selectedItems: $scope.mySelections,
              multiSelect: false,
              showColumnMenu: true,
              enableColumnResize: true,
              enableColumnReordering: true,
              columnDefs: 'fieldCoumns'
          };

          $scope.preview = function () {
              $scope.fieldCoumns = [];
              for (var i = 0; i<$scope.SelectedColumns.length; i++) {
                  var fieldName = $scope.SelectedColumns[i].FieldName;
                  $scope.fieldCoumns[i] = { field: fieldName, displayName: fieldName };
              }
             
          };

          $scope.save = function () {
              var pickListValue = '';
              for (var i = 0; i<$scope.SelectedColumns.length; i++) {
                  var fieldName = $scope.SelectedColumns[i].FieldName;
                  pickListValue += fieldName + '$';
              }
              $('#picklist')[0].value = pickListValue;
              $.ajax({
                  url: myForm.action,
                  type: myForm.method,
                  data: $(myForm).serialize() + '&submit.Save=Save',
                  success: function (result) {
                      logger.success("Layout Saved.");
                  }
              });
          };

          $scope.change = function () {

          };

          $scope.exit = function () {
              $state.transitionTo('List', { Module: 'Projections'});
          };

          $scope.addfield = function (fieldName) {
              var selectedField = { FieldName: fieldName };
              $scope.SelectedColumns.splice($scope.SelectedColumns.length, 0, selectedField);
             
          };

          $scope.removefield = function (index) {
              $scope.SelectedColumns.splice(index, 1);
          };

          $scope.LabelClass = function (fieldName) {
              for (var i = 0; i < $scope.SelectedColumns.length; i++) {
                  if ($scope.SelectedColumns[i].FieldName == fieldName) return 'label';
              }
              return 'label hide';
          };
          
          $scope.ButtonStyle = function (fieldName) {
              for (var i = 0; i < $scope.SelectedColumns.length; i++) {
                  if ($scope.SelectedColumns[i].FieldName == fieldName)
                      return {'display':'none'};
              }
              return { 'display': 'block' };
          };
          
     

          $scope.InitSeletedFieldData = function() {
              var viewModel = viewmodelDataService.query({ id: $stateParams.Id }, function () {
                  for (var i = 0; i < viewModel.length; i++) {
                      $scope.addfield(viewModel[i].FieldName);
                  }
              }, function () {
                  
              });
          };
          $scope.InitSeletedFieldData();
      }]
    ]);
});