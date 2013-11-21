'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
      'TranslationExportCtrl',
      ['$rootScope', '$scope', 'logger', '$state', '$i18next',
      function ($rootScope, $scope, logger, $state, $i18next) {
          var metadataColumnDefs = [
              { name: 'Culture', label: $i18next('Culture'), hidden: true },
              { name: 'CultureDisplay', label: $i18next('CultureDisplay') },
              { name: 'Translatable', label: $i18next('Translatable') },
              { name: 'Translated', label: $i18next('Translated') },
              { name: 'Missing', label: $i18next('Missing') }
          ];

          //get data form API, then render by the metadataColumnDefs
          $scope.gridOptions = {
              url: "api/Translations/Translation",
              colModel: metadataColumnDefs,
          };

          angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);

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