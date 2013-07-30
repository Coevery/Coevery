define(['core/app/couchPotatoService', 'core/services/commondataservice'], function (couchPotato) {
    couchPotato.registerController([
      'GeneralDetailCtrl',
      ['$timeout', '$rootScope', '$scope', '$q', 'logger', '$state', '$http',
      function ($timeout, $rootScope, $scope, $q, logger, $state, $http) {
          var moduleName = $rootScope.$stateParams.Module;
          $scope.moduleName = moduleName;
          var validator = $(myForm).validate();

          $scope.save = function () {
              if (!validator.form()) {
                  return null;
              }

              var form = angular.element(myForm);
              var promise = $http({
                  url: form.attr('action'),
                  method: "POST",
                  data: form.serialize() + '&submit.Save=Save',
                  headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
              }).then(function () {
                  logger.success('Save succeeded.');
              }, function (reason) {
                  logger.error('Save Failed： ' + reason);
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
              $state.transitionTo('Detail', { Module: moduleName, Id: id });
          };

          $scope.exit = function () {
              $state.transitionTo('List', { Module: moduleName });
          };

          $scope.$on('$viewContentLoaded', function () {
          });
      }]
    ]);
});