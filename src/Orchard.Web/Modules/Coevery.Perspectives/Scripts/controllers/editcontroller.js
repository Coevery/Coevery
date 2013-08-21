'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
      'PerspectiveEditCtrl',
      ['$timeout', '$scope', 'logger', '$detour','$http',
      function ($timeout, $scope, logger, $detour,$http) {

          $scope.save = function () {
              var form = $("form[name=myForm]");
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
              });
          };
          
          $scope.exit = function () {
              $detour.transitionTo('PerspectiveList');
          };
      }]
    ]);
});