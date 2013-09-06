'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
       'NavigationItemEditCtrl',
       ['$timeout', '$scope', 'logger', '$detour', '$stateParams', '$resource','$http',
       function ($timeout, $scope, logger, $detour, $stateParams, $resource, $http) {
           
           var checkValid = function (form) {
               var validator = form.validate();
               if (!validator) {
                   return false;
               }
               if (!validator.form()) {
                   return false;
               }
               return true;
           };

           $scope.exit = function () {
               $detour.transitionTo('PerspectiveDetail', { Id: $stateParams.Id });
           };

           $scope.save = function () {       
               var form = $("form[name=myForm]");
               if (!checkValid(form)) {
                   return null;
               }
               var promise = $http({
                   url: form.attr('action'),
                   method: "POST",
                   data: form.serialize() + '&submit.Save=Save',
                   headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
               }).then(function () {
                   logger.success('Save succeeded.');
               }, function (reason) {
                   logger.error('Save Failed： ' + reason.data);
               });
               return promise;
           };
           
           $scope.saveAndBack = function () {
               var promise = $scope.save();
               promise.then(function () {
                   $scope.exit();
               });
           };

           $scope.opts = {
               backdropFade: true,
               dialogFade: true
           };
           
           $scope.openDialog = function () {
               $scope.dialogSelectIcons = true;
           };

           $scope.closeDialog = function () {
               $scope.dialogSelectIcons = false;
           };

           $scope.selected = function () {
               if ($("#icons div.iconspan.selected").length<=0) {
                   return;
               }
               var iconClass = $("#icons div.iconspan.selected i").attr("class");
               $("#hfIconClass").val(iconClass);
               $("#showIconClass").attr("class", iconClass);
               $scope.dialogSelectIcons = false;
           };
       }]
    ]);
});

//@ sourceURL=Coevery.Perspectives/navigationitemdetailcontroller.js