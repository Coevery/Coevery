'use strict';
define(['core/app/detourService', 'Modules/Coevery.Translations/Scripts/services/columnDefinitionService'], function (detour) {
    detour.registerController([
      'TranslationCultureCtrl',
      ['$rootScope', '$scope', 'logger', '$state', '$stateParams', 'moduleColumnDefinitionService',
          function ($rootScope, $scope, logger, $state, $stateParams, moduleColumnDefinitionService) {
              var culture = $stateParams.Id;

              var t = function (str) {
                  var result = i18n.t(str);
                  return result;
              };

              $scope.FetchDefinitionViews = function () {
                  var gridColumns = moduleColumnDefinitionService.query("", function () {
                      $.each(gridColumns, function (index, value) {
                          if (value.formatter) {
                              value.formatter = $rootScope[value.formatter];
                          }
                      });

                      //get data form API, then render by the gridColumns
                      //culture(string culture,int page,int rows)
                      $scope.gridOptions = {
                          url: "api/Translations/Culture?culture=" + culture,
                          colModel: gridColumns,
                      };

                      angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);
                  });
              };

              $scope.FetchDefinitionViews();

              var metadataColumnDefs = [
                  { name: 'Path', label: 'Path', hidden: true },
                  {
                      name: 'Module',
                      label: t('Module'),
                      formatter: $rootScope.cellLinkTemplateWithoutDelete,
                      formatoptions: { hasView: true }
                  },
                  { name: 'Total', label: t('Total') },
                  { name: 'Translated', label: t('Translated') },
                  { name: 'Missing', label: t('Missing') }
              ];

              //For grid refresh btn.
              $scope.getAllMetadata = function () {
                  $("#gridList").jqGrid('setGridParam', {
                      datatype: "json"
                  }).trigger('reloadGrid');
              };

              $scope.view = function (path) {
                  $state.transitionTo('TranslationsDetail', {
                      Culture: culture, Path: path
                  });
              };

              $scope.edit = function (path) {
                  $state.transitionTo('TranslationsDetail', {
                      Culture: culture, Path: path
                  });
              };

              //For back btn
              $scope.exit = function () {
                  $state.transitionTo('TranslationsList');
              };

          }]
    ]);
});