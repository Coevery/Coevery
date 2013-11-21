'use strict';
define(['core/app/detourService', 'Modules/Coevery.Translations/Scripts/services/columnDefinitionService'], function (detour) {
    detour.registerController([
      'TranslationCultureCtrl',
      ['$rootScope', '$scope', 'logger', '$state', '$stateParams', '$i18next',
          function ($rootScope, $scope, logger, $state, $stateParams, $i18next) {
              var culture = $stateParams.Id;

              $scope.FetchDefinitionViews = function () {
                  var metadataColumnDefs = [
                      { name: 'Path', label: $i18next('Path'), hidden: true },
                      { name: 'Module', label: $i18next('Module'), formatoptions: { hasView: true } },
                      { name: 'Total', label: $i18next('Total') },
                      { name: 'Translated', label: $i18next('Translated') },
                      { name: 'Missing', label: $i18next('Missing') }
                  ];

                  $scope.gridOptions = angular.extend({}, $rootScope.defaultGridOptions, {
                      url: "api/Translations/Culture?culture=" + culture,
                      colModel: metadataColumnDefs
                  });
              };

              $scope.FetchDefinitionViews();

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