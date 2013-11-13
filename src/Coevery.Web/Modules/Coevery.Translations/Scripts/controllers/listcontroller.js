'use strict';
define(['core/app/detourService', 'Modules/Coevery.Translations/Scripts/services/columnDefinitionService'], function (detour) {
    detour.registerController([
      'TranslationListCtrl',
      ['$rootScope', '$scope', 'logger', '$state', 'cultureColumnDefinitionService',
      function ($rootScope, $scope, logger, $state, cultureColumnDefinitionService) {

          var t = function (str) {
              var result = i18n.t(str);
              return result;
          };

          $scope.FetchDefinitionViews = function () {
              var gridColumns = cultureColumnDefinitionService.query("", function () {
                  $.each(gridColumns, function (index, value) {
                      if (value.formatter) {
                          value.formatter = $rootScope[value.formatter];
                      }
                  });

                  //get data form API, then render by the metadataColumnDefs
                  $scope.gridOptions = {
                      url: "api/Translations/Translation",
                      colModel: gridColumns,
                  };

                  angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);
              });
          };

          $scope.FetchDefinitionViews();

          var metadataColumnDefs = [
              { name: 'Culture', label: t('Culture'), hidden: true },
              {
                  name: 'CultureDisplay', label: t('Hello'),
                  formatter: $rootScope.cellLinkTemplateWithoutDelete,
                  formatoptions: { hasView: true }
              },
              { name: 'Translatable', label: t('Translatable') },
              { name: 'Translated', label: t('Translated') },
              { name: 'Missing', label: t('Missing') }
          ];

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