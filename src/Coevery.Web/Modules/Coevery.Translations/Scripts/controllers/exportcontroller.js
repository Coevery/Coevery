'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
      'TranslationExportCtrl',
      ['$rootScope', '$scope', 'logger', '$state',
      function ($rootScope, $scope, logger, $state) {

          var t = function (str) {
              var result = i18n.t(str);
              return result;
          };

          var metadataColumnDefs = [
              { name: 'Culture', label: t('Culture'), hidden: true },
              {
                  name: 'CultureDisplay', label: 'CultureDisplay',
                  formatter: $rootScope.cellLinkTemplateWithoutDelete,
                  formatoptions: { hasView: true }
              },
              { name: 'Translatable', label: t('Translatable') },
              { name: 'Translated', label: t('Translated') },
              { name: 'Missing', label: t('Missing') }
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