'use strict';
define(['core/app/couchPotatoService'],function (couchPotato) {
    couchPotato.registerController([
      'PerspectivesEditCtrl',
      ['$timeout', '$scope', 'logger', '$state',
      function ($timeout, $scope, logger, $state) {

          $scope.save = function () {
              $.ajax({
                  url: myForm.action,
                  type: myForm.method,
                  data: $(myForm).serialize() + '&submit.Save=Save',
                  success: function (result) {
                      logger.success("Perspective Saved.");
                  }
              });
          };
          
          $scope.exit = function () {
              $state.transitionTo('List', { Module: 'Perspectives'});
          };
      }]
    ]);
});