'use strict';

define(['core/app/detourService'], function (detour) {
    detour.registerController([
      'EntityEditCtrl',
      ['$timeout', '$scope', 'logger', '$state', '$stateParams', '$resource', '$element',
      function ($timeout, $scope, logger, $state, $stateParams, $resource, $element) {
          $scope.save = function () {
              $.ajax({
                  url: $element.attr('action'),
                  type: $element.attr('method'),
                  data: $element.serialize() + '&submit.Save=Save',
                  success: function (result) {
                      $timeout($scope.exit, 0);
                  }
              });
          };

          $scope.exit = function () {
              $state.transitionTo('EntityList', { Module: 'Entities' });
          };
      }]
    ]);
});