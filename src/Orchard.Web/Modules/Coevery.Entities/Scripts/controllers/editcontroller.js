'use strict';

define(['core/app/detourService'], function (detour) {
    detour.registerController([
      'EntityEditCtrl',
      ['$timeout', '$scope', 'logger', '$detour', '$stateParams', '$resource',
      function ($timeout, $scope, logger, $detour, $stateParams, $resource) {
          $scope.save = function () {
              var element = angular.element(myForm);
              $.ajax({
                  url: element.attr('action'),
                  type: element.attr('method'),
                  data: element.serialize() + '&submit.Save=Save',
                  success: function (result) {
                      $timeout($scope.exit, 0);
                  }
              });
          };

          $scope.exit = function () {
              if ($stateParams.Id) {
                  $detour.transitionTo('EntityDetail.Fields', { Id: $stateParams.Id });
              } else {
                  $detour.transitionTo('EntityList');
              }
          };
      }]
    ]);
});