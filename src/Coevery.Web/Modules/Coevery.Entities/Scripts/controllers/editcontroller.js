'use strict';

define(['core/app/detourService', 'Modules/Coevery.Entities/Scripts/services/entitydataservice'], function (detour) {
    detour.registerController([
      'EntityEditCtrl',
      ['$timeout', '$scope', 'logger', '$state', '$stateParams', '$resource', '$http', '$parse', 'entityDataService',
      function ($timeout, $scope, logger, $state, $stateParams, $resource, $http, $parse, entityDataService) {
          var validator = $("#myForm").validate({
              errorClass: "inputError"
          });

          $scope.save = function () {
              if (!validator.form()) {
                  return null;
              }
              var form = $("#myForm");
              var promise = $http({
                  url: form.attr('action'),
                  method: "POST",
                  data: form.serialize(),
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
                  var getter = $parse('entityName');
                  var entityName = getter(response.data);
                  if (entityName)
                      $state.transitionTo('EntityEdit', { Id: entityName });
              });
          };

          $scope.saveAndBack = function () {
              var promise = $scope.save();
              promise && promise.then(function () {
                  $scope.exit();
              });
          };
          
          $scope.exit = function () {
              if ($stateParams.Id) {
                  $state.transitionTo('EntityDetail.Fields', { Id: $stateParams.Id });
              } else {
                  $state.transitionTo('EntityList');
              }
          };
      }]
    ]);
});