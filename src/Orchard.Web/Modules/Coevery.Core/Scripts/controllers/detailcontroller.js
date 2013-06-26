define(['core/app/couchPotatoService', 'core/services/commondataservice'], function (couchPotato) {
    couchPotato.registerController([
      'GeneralDetailCtrl',
      ['$timeout', '$rootScope', '$scope', 'logger', '$state', '$stateParams', '$element', 'commonDataService',
      function ($timeout, $rootScope, $scope, logger, $state, $stateParams, $element, commonDataService) {
          var moduleName = $rootScope.$stateParams.Module;
          

          $scope.save = function () {
              $.ajax({
                  url: myForm.action,
                  type: myForm.method,
                  data: $(myForm).serialize() + '&submit.Save=Save',
                  success: function (result) {
                      $timeout($scope.exit, 0);
                  }
              });
          };
         
          $scope.change = function () {

          };

          $scope.exit = function () {
              $state.transitionTo('List', { Module: moduleName });
          };

          $scope.$on('$viewContentLoaded', function () {
          });
      }]
    ]);
});