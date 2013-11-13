'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
      'TranslationImportCtrl',
      ['$rootScope', '$scope', 'logger', '$state', '$http',
      function ($rootScope, $scope, logger, $state, $http) {

          $scope.save = function (formName) {
              if (!$(formName).validate({
                  errorClass: "inputError"
              }).form()) {
                  return null;
              }
              var form = $(formName);
              var promise = $http({
                  url: form.attr('action'),
                  method: "POST",
                  data: form.serialize(),
                  headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
              }).then(function (response) {
                  logger.success('Save succeeded.');
                  return response;
              }, function (reason) {
                  logger.error('Save Failed： ' + reason.data);
              });
              return promise;
          };

          $scope.generateFromSource = function (url) {
              var promise = $http({
                  url: url,
                  method: "GET",
                  data: "",
                  headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
              }).then(function (response) {
                  logger.success('Generate succeeded.');
                  return response;
              }, function (reason) {
                  logger.error('Save Failed： ' + reason.data);
              });
              return promise;
          };

          $scope.manualSubmit = function () {
              var promise = $scope.save("#manualForm");
              return promise;
          };

      }]
    ]);
});