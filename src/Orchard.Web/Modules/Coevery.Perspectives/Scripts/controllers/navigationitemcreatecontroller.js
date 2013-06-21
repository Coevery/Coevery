'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
       'NavigationItemCreateCtrl',
       ['$timeout', '$scope', 'logger', '$detour', '$stateParams', '$resource',
       function ($timeout, $scope, logger, $detour, $stateParams, $resource) {
           
           $scope.exit = function () {
               $detour.transitionTo('PerspectiveDetail', { Id: $stateParams.Id});
           };

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
       }]
    ]);
});

//@ sourceURL=Coevery.Perspectives/navigationitemdetailcontroller.js