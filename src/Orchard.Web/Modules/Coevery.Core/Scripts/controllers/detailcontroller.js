define(['core/app/detourService', 'core/services/commondataservice'], function (detour) {
    detour.registerController([
      'GeneralDetailCtrl',
      ['$timeout', '$rootScope', '$scope', '$q', 'logger', '$detour', '$http',
      function ($timeout, $rootScope, $scope, $q, logger, $detour, $http) {
          var moduleName = $rootScope.$stateParams.Module;
          $scope.moduleName = moduleName;
          var validator = $("form[name=myForm]").validate();

          $scope.save = function () {
              if (!validator.form()) {
                  return null;
              }

              var form = $("form[name=myForm]");
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

          $scope.change = function () {

          };

          $scope.edit = function () {
              var id = $rootScope.$stateParams.Id;
              $detour.transitionTo('Detail', { Module: moduleName, Id: id });
          };

          $scope.exit = function () {
              if(window.history.length>1)
                  window.history.back();
              else
                $detour.transitionTo('List', { Module: moduleName });
          };

          $scope.$on('$viewContentLoaded', function () {
          });
      }]
    ]);
});