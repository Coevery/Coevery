'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
       'NavigationItemEditCtrl',
       ['$timeout', '$scope', 'logger', '$detour', '$stateParams', '$resource',
       function ($timeout, $scope, logger, $detour, $stateParams, $resource) {
           
           $scope.exit = function () {
               $detour.transitionTo('PerspectiveDetail', { Id: $stateParams.Id });
           };

           $scope.save = function (isBack) {
               $.ajax({
                   url: myForm.action,
                   type: myForm.method,
                   data: $(myForm).serialize() + '&submit.Save=Save',
                   success: function (result) {
                       //logger.success("Perspective Saved.");
                       if (isBack)
                           $scope.exit();
                   }
               });
           };
       }]
    ]);
});

//@ sourceURL=Coevery.Perspectives/navigationitemdetailcontroller.js