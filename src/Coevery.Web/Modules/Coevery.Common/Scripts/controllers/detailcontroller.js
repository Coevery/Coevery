define(['core/app/detourService'], function (detour) {
    detour.registerController([
      'GeneralDetailCtrl',
      ['$timeout', '$rootScope', '$scope', '$q', 'logger', '$state', '$http', '$parse', '$i18next','$window',
      function ($timeout, $rootScope, $scope, $q, logger, $state, $http, $parse, $i18next, $window) {
          var navigationId = $rootScope.$stateParams.NavigationId;
          var moduleName = $rootScope.$stateParams.Module;
          $scope.moduleName = moduleName;
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
                  tracker: 'saveentity'
              });
              return promise;
          };

          $scope.saveAndView = function() {
              var promise = $scope.save();
              promise && promise.then(function(response) {
                  logger.success($i18next('Save succeeded.'));
                  var getter = $parse('Id');
                  var Id = getter(response.data);
                  if (Id)
                      $state.transitionTo('Root.Menu.Detail', { NavigationId: navigationId, Module: moduleName, Id: Id });
              }, function(reason) {
                  logger.error($i18next('Save Failed： ') + reason.data);
              });
          };

          $scope.saveAndBack = function () {
              var promise = $scope.save();
              promise && promise.then(function () {
                  logger.success($i18next('Save succeeded.'));
                  $scope.exit();
              }, function (reason) {
                  logger.error($i18next('Save Failed： ') + reason.data);
              });
          };

          $scope.change = function () {

          };

          $scope.edit = function () {
              var id = $rootScope.$stateParams.Id;
              $state.transitionTo('Root.Menu.Detail', { NavigationId: navigationId, Module: moduleName, Id: id });
          };

          $scope.exit = function () {
              $rootScope.goBack();
          };

          $scope.$on('$viewContentLoaded', function () {

          });
      }]
    ]);
});