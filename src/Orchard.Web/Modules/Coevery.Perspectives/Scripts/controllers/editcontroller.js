'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
      'PerspectiveEditCtrl',
      ['$timeout', '$scope', 'logger', '$detour',
      function ($timeout, $scope, logger, $detour) {

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
              $detour.transitionTo('PerspectiveList');
          };
      }]
    ]);
});