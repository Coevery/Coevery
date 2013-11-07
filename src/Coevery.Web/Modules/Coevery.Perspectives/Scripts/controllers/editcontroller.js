'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
      'PerspectiveEditCtrl',
      ['$timeout', '$parse', '$scope', 'logger', '$state','$http',
      function ($timeout, $parse,$scope, logger, $state, $http) {

          var validator = $("form[name=myForm]").validate({
              errorClass: "inputError"
          });

          $scope.save = function () {
              if (!validator.form()) {
                  return null;
              }

              var form = $("form[name=myForm]");
              var promise = $http({
                  url: form.attr('action'),
                  method: "POST",
                  data: form.serialize() + '&submit.Save=Save',
                  headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                  tracker: 'saveperspective'
              }).then(function (response) {
                  logger.success('Save succeeded');
                  return response;
              }, function (reason) {
                  logger.error('Save Failed： ' + reason.data.Message);
              });
              return promise;
          };

          $scope.saveAndView = function () {
              var promise = $scope.save();
              promise && promise.then(function (response) {
                  var getter = $parse('id');
                  var id = getter(response.data);
                  if(id)
                      $state.transitionTo('PerspectiveEdit', { Id: id });
              });
          };

          $scope.saveAndBack = function () {
              var promise = $scope.save();
              promise && promise.then(function () {
                  $scope.exit();
              });
          };
          
          $scope.exit = function () {
              $state.transitionTo('PerspectiveList');
          };
      }]
    ]);
});