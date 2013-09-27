'use strict';

define(['core/app/detourService'], function (detour) {
    detour.registerController([
      'EntityEditCtrl',
      ['$timeout', '$scope', 'logger', '$detour', '$stateParams', '$resource','$http','$parse',
      function ($timeout, $scope, logger, $detour, $stateParams, $resource, $http, $parse) {

          var validator = $("#myForm").validate({
              errorClass: "inputError"
          });

          $scope.save = function () {
              if (!validator.form()) {
                  return null;
              }
              var form = $("#myForm");
              var promise = $http({
                  url: form.attr('action'),
                  method: "POST",
                  data: form.serialize() + '&submit.Save=Save',
                  headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
              }).then(function (response) {
                  logger.success('Save succeeded.');
                  window.location.reload();
                  return response;
              }, function (reason) {
                  logger.error('Save Failed： ' + reason.data);
              });
              return promise;
          };

          $scope.saveAndView = function () {
              var promise = $scope.save();
              promise.done(function (response) {
                  var getter = $parse('entityName');
                  var entityName = getter(response.data);
                  if (entityName)
                      $detour.transitionTo('EntityEdit', { Id: entityName });
              });
              return promise;
          };

          $scope.saveAndBack = function () {
              var promise = $scope.save();
              promise && promise.then(function () {
                  $scope.exit();
              });
              return promise;
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