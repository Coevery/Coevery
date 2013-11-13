'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
      'TranslationDetailCtrl',
      ['$rootScope', '$scope', 'logger', '$state', '$stateParams', '$http',
      function ($rootScope, $scope, logger, $state, $stateParams, $http) {
          var culture = $stateParams.Culture;

          var validator = $("#myForm").validate({
              errorClass: "inputError"
          });

          $scope.save = function () {
              if (!validator.form()) {
                  return null;
              }
              var form = $("#myForm");
              var url = "SystemAdmin/Translations/Detail";
              var promise = $http({
                  url: url,
                  method: "POST",
                  data: form.serialize() + "&culture=" + culture,
                  headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
              }).then(function (response) {
                  logger.success('Save succeeded.');
                  return response;
              }, function (reason) {
                  logger.error('Save Failed： ' + reason.data);
              });
              return promise;
          };

          $scope.saveAndView = function () {
              var promise = $scope.save();
              return promise;
          };

          $scope.saveAndBack = function () {
              var promise = $scope.save();
              promise && promise.then(function () {
                  $scope.exit();
              });
              return promise;
          };

          //For back btn
          $scope.exit = function () {
              $state.transitionTo('TranslationsCulture', { Id: culture });
          };

      }]
    ]);
});