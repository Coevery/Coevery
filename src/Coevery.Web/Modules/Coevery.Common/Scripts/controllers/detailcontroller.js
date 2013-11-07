define(['core/app/detourService'], function (detour) {
    detour.registerController([
      'GeneralDetailCtrl',
      ['$timeout', '$rootScope', '$scope', '$q', 'logger', '$state', '$http','$parse',
      function ($timeout, $rootScope, $scope, $q, logger, $state, $http, $parse) {
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
              promise && promise.then(function (response) {
                  var getter = $parse('Id');
                  var Id = getter(response.data);
                  if (Id)
                      $state.transitionTo('Root.Menu.Detail', { NavigationId: navigationId, Module: moduleName, Id: Id });
              });
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
              $state.transitionTo('Root.Menu.Detail', { NavigationId: navigationId, Module: moduleName, Id: id });
          };

          $scope.exit = function () {
              //if(window.history.length>1)
              //    window.history.back();
              //else
              $state.transitionTo('Root.Menu.List', { NavigationId: navigationId, Module: moduleName });
          };



          $scope.$on('$viewContentLoaded', function () {
              
          });
      }]
    ]);
});