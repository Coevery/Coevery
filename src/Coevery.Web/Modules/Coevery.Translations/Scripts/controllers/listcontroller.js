'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
      'TranslationListCtrl',
      ['$rootScope', '$scope', 'logger', '$state', '$i18next',
      function ($rootScope, $scope, logger, $state, $i18next) {

          $scope.FetchDefinitionViews = function() {
              var columnDefs = [
                  { name: 'Culture', label: $i18next('Culture'), hidden: true, key: true },
                  {
                      name: 'CultureDisplay',
                      label: $i18next('Culture'),
                      formatter: $rootScope.cellLinkTemplateWithoutDelete,
                      formatoptions: { hasView: true }
                  },
                  { name: 'Translatable', label: $i18next('Translatable') },
                  { name: 'Translated', label: $i18next('Translated') },
                  { name: 'Missing', label: $i18next('Missing') }
              ];

              $scope.gridOptions = angular.extend({}, $rootScope.defaultGridOptions, {
                  url: "api/Translations/Translation",
                  colModel: columnDefs,
              });
          };

          $scope.FetchDefinitionViews();

          

          //For grid refresh btn.
          $scope.getAllMetadata = function () {
              $("#gridList").jqGrid('setGridParam', {
                  datatype: "json"
              }).trigger('reloadGrid');
          };

          //For grid eidt btn.
          $scope.edit = function (id) {
              $state.transitionTo('TranslationsCulture', { Id: id });
          };

          //For grid view btn
          $scope.view = function (id) {
              $state.transitionTo('TranslationsCulture', { Id: id });
          };

      }]
    ]);
});