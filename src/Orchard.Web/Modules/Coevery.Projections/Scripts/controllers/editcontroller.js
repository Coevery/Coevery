'use strict';
define(['core/app/detourService', 'Modules/Coevery.Projections/Scripts/services/projectiondataservice'], function (detour) {
    detour.registerController([
      'ProjectionEditCtrl',
      ['$timeout', '$scope', 'logger', '$detour', '$stateParams', '$resource',  'projectionDataService',
      function ($timeout, $scope, logger, $detour, $stateParams, $resource, projectionDataService) {
          var name = $stateParams.Id;
          var isNew = (name || name == '') ? false : true;
          $scope.mySelections = [];
          $scope.filedCoumns = [];

          $scope.gridOptions = {
              data: 'myData',
              selectedItems: $scope.mySelections,
              multiSelect: false,
              showColumnMenu: true,
              enableColumnResize: true,
              enableColumnReordering: true,
              columnDefs: 'filedCoumns'
          };

          $scope.preview = function () {
              var selectedFieldItems = $('#sortable >li');
              var index = 0;
              $scope.filedCoumns = [];
              selectedFieldItems.each(function (k, v) {
                  var fieldName = v.id.replace('SelectedField', '');
                  $scope.filedCoumns[index] = { field: fieldName, displayName: fieldName };
                  index++;
              });
          };

          $scope.save = function () {
              var selectedFieldItems = $('#sortable >li');
              var pickListValue = '';
              selectedFieldItems.each(function (k, v) {
                  var fieldName = v.id.replace('SelectedField', '');
                  pickListValue += fieldName + '$';
              });
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
              $detour.transitionTo('ProjectionList');
          };

          $scope.addfield = function (fieldName) {
              var elementTemp = '<li id="SelectedField' + fieldName + '" class="ui-state-default">' + fieldName + '';
              elementTemp += '<div class="pull-right">';
              elementTemp += '<button class="btn-link" type="button"  ng-click="removefield(\'' + fieldName + '\')">Remove</button>';
              elementTemp += '</div>';
              elementTemp += '</li>';
              $compile(elementTemp)($scope).appendTo($('#sortable'));
              $('#UnSelectedLabelField' + fieldName).removeClass('label hide');
              $('#UnSelectedLabelField' + fieldName).addClass('label');
              $('#UnSelectedButtonField' + fieldName).css('display', 'none');
          };

          $scope.removefield = function (fieldName) {
              $('#SelectedField' + fieldName).remove();
              $('#UnSelectedLabelField' + fieldName).removeClass('label');
              $('#UnSelectedLabelField' + fieldName).addClass('label hide');
              $('#UnSelectedButtonField' + fieldName).css('display', 'block');
          };
      }]
    ]);
});