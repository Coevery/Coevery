'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
       'NavigationItemEditCtrl',
       ['$timeout', '$parse', '$scope', 'logger', '$state', '$stateParams', '$resource','$http',
       function ($timeout, $parse, $scope, logger, $state, $stateParams, $resource, $http) {
           

           $scope.exit = function () {
               $state.transitionTo('PerspectiveDetail', { Id: $stateParams.Id });
           };

           var validator = $("form[name=myForm]").validate({
               errorClass: "inputErrorLeft"
           });

           $scope.save = function () {

               if (!validator.form()) {
                   return null;
               }
               var form = $("form[name=myForm]");
               
               var promise = $http({
                   url: form.attr('action'),
                   method: "POST",
                   data: form.serialize() + '&submit.Save=Save',
                   headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                   tracker: 'savenavigationitem'
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
                   var getter = $parse('id');
                   var id = getter(response.data);
                   if(id)
                       $state.transitionTo('EditNavigationItem', { Id: $stateParams.Id, NId: id, Type: response.data.type });
               });
           };
           
           $scope.saveAndBack = function () {
               var promise = $scope.save();
               promise && promise.then(function () {
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
               validator.form();
           };
       }]
    ]);
});

//@ sourceURL=Coevery.Perspectives/navigationitemdetailcontroller.js