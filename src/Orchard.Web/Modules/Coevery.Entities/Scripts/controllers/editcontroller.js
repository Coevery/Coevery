'use strict';

define(['core/app/detourService'], function (detour) {
    detour.registerController([
      'EntityEditCtrl',
      ['$timeout', '$scope', 'logger', '$detour', '$stateParams', '$resource','$http',
      function ($timeout, $scope, logger, $detour, $stateParams, $resource, $http) {
          
          var checkValid = function (form) {
              var validator = form.validate();
              if (!validator) {
                  return false;
              }
              if (!validator.form()) {
                  return false;
              }
              return true;
          };

          $scope.save = function () {
              if (!checkValid($("#myForm"))) {
                  return null;
              }
              var form = $("#myForm");
              var promise = $http({
                  url: form.attr('action'),
                  method: "POST",
                  data: form.serialize() + '&submit.Save=Save',
                  headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
              }).then(function () {
                  logger.success('Save succeeded.');
              }, function (reason) {
                  logger.error('Save Failed： ' + reason.data);
              });
              return promise;
          };

          $scope.saveAndBack = function () {
              var promise = $scope.save();
              promise && promise.then(function () {
                  $scope.exit();
              });
          };
          
          $scope.exit = function () {
              if ($stateParams.Id) {
                  $detour.transitionTo('EntityDetail.Fields', { Id: $stateParams.Id });
              } else {
                  $detour.transitionTo('EntityList');
              }
          };
      }]
    ]);
});