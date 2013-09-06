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
              }).then(function (result) {
                  if (result.data && result.data.Success) {
                      promise.Id = result.data.Value;
                      logger.success('Save succeeded.');
                  } else {
                      logger.error('Save Failed： ' + result.data.Message);
                  }
              }, function (reason) {
                  logger.error('Save Failed： ' + reason.data.Message);
              });
              return promise;
          };

          $scope.saveAndView = function () {
              var promise = $scope.save();
              promise.then(function () {
                  var id = promise.Id;
                  $detour.transitionTo('PerspectiveDetail', { Id: id });
              });
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