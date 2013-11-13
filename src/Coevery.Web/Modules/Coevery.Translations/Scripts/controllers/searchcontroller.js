'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
      'TranslationSearchCtrl',
      ['$rootScope', '$scope', 'logger', '$state', '$http',
      function ($rootScope, $scope, logger, $state, $http) {

          //For grid search btn.
          $scope.search = function () {
              if ($("#queryString").val() == "") {
                  var validator = $("#queryForm").validate({
                      errorClass: "inputError"
                  });
                  validator.form();
                  return false;
              }
              var queryString = $("#queryString").val();
              var culture = $("#culture").val();
              var url = "/OrchardLocal/SystemAdmin/Translations/Search/" + queryString + "/" + culture;

              $http({
                  url: url,
                  method: "POST",
                  data: "",
                  headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
              }).success(function (data) {
                  if (data == "") {
                      $("#searchResult").empty();
                      $("#btnSave").attr("disabled", "disabled");
                  } else {
                      $("#searchResult").empty();
                      $("#searchResult").append(data);
                      $("section.ng-scope").last().css("margin-top", "0");
                      $("#btnSave").removeAttr("disabled");
                  }
              });
          };

          $scope.save = function () {
              var culture = $("#culture").val();
              var url = "/OrchardLocal/SystemAdmin/Translations/Detail/" + culture + "/";
              var form = $("#myForm");
              var promise = $http({
                  url: url,
                  method: "POST",
                  data: form.serialize(),
                  headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
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
              return promise;
          };

      }]
    ]);
});