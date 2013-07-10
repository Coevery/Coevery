'use strict';

define(['core/app/detourService'], function (detour) {
    detour.registerController([
      'EntityEditCtrl',
      ['$timeout', '$scope', 'logger', '$detour', '$stateParams', '$resource','$http',
      function ($timeout, $scope, logger, $detour, $stateParams, $resource, $http) {
          $scope.save = function () {
              var form = angular.element(myForm);
              var promise = $http({
                  url: form.attr('action'),
                  method: "POST",
                  data: form.serialize() + '&submit.Save=Save',
                  headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
              }).then(function () {
                  logger.success('Save succeeded.');
              }, function (reason) {
                  logger.success('Save Failed： ' + reason);
              });
              return promise;
          };

          $scope.saveAndBack = function () {
              var promise = $scope.save();
              promise.then(function () {
                  $scope.exit();
              }, function () {
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