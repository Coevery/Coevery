'use strict';

define(['core/app/detourService', 'Modules/Coevery.Modules/Scripts/services/featuredataservice'], function (detour) {
    detour.registerController([
      'FeatureListCtrl',
      ['$scope', '$http', 'featureDataService', 'logger', '$compile', function ($scope, $http, featureDataService, logger, $compile) {
          $scope.categories= featureDataService.query();
          
          $scope.jump = function (featureid) {
              //$("html,body").animate({ scrollTop: $("#" + featureid).offset().top - 140 }, 1000);
          };

          $scope.action = function(featureId) {
              $("[name='submit.BulkExecute']").val("yes");
              $scope.getAction(featureId);
              $("[name='bulkAction']").val($scope.featureAction.FeatureAction.Action);
              //var tempFeatureIds = $("[name='featureIds']").val();
              $("[name='featureIds']").val(featureId);
              $("[name='force']").val($scope.featureAction.FeatureAction.Force);
              postform(featureId);
              //$("[name='featureIds']").val(tempFeatureIds);
          };

          function postform(featureId) {
              var form = $("form[name=myForm]:first");
              $http({
                  url: form.attr('action'),
                  method: "POST",
                  data: form.serialize(),
                  headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                  tracker: 'featureaction'
              }).then(function (response) {
                  $scope.update(response.data);
                  featureId = featureId || "";
                  logger.success(featureId + ' ' + $scope.featureAction.FeatureAction.Action + ' succeeded');
              }, function (reason) {
                  logger.error(featureId + ' ' + $scope.featureAction.FeatureAction.Action + ' Failed');
              });
          }

          $scope.submit = function (action) {
              $("[name='submit.BulkExecute']").val("yes");
              $("[name='bulkAction']").val(action);
              $("[name='force']").val();
              postform();
          };


          $scope.getAction = function (featureId) {
              $scope.categories.forEach(function(category) {
                  category.Features.forEach(function (feature) {
                      if (feature.FeatureId == featureId)
                          $scope.featureAction=feature;
                  });
              });
          };

          $scope.update = function (categories) {
              $scope.categories.forEach(function(category) {
                  category.Features.forEach(function(feature) {
                      categories.forEach(function (c) {
                          c.Features.forEach(function (f) {
                              if (feature.FeatureId == f.FeatureId) {
                                  feature.FeatureState = f.FeatureState;
                                  feature.NeedsUpdate = f.NeedsUpdate;
                                  feature.TitleStyle = f.TitleStyle;
                                  feature.FeatureAction= f.FeatureAction;
                              }
                          });
                      });
                  });
              });
          };
      }]
    ]);
});
