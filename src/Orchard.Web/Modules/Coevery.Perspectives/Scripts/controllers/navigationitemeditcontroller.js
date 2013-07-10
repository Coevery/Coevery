'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
       'NavigationItemEditCtrl',
       ['$timeout', '$scope', 'logger', '$detour', '$stateParams', '$resource','$http',
       function ($timeout, $scope, logger, $detour, $stateParams, $resource,$http) {
           
           $scope.exit = function () {
               $detour.transitionTo('PerspectiveDetail', { Id: $stateParams.Id });
           };

           $scope.save = function () {
               var form = angular.element(myForm);
               var promise = $http({
                   url: form.attr('action'),
                   method: "POST",
                   data: form.serialize() + '&submit.Save=Save',
                   headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
               }).then(function () {
                   logger.success('Save succeeded.');
               }, function (reason) {
                   logger.success('Save Failed： ' + reason);
               });
               return promise;
           };
           
           $scope.saveAndBack = function () {
               var promise = $scope.save();
               promise.then(function () {
                   $scope.exit();
               }, function () {
               });
           };
       }]
    ]);
});

//@ sourceURL=Coevery.Perspectives/navigationitemdetailcontroller.js