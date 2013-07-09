'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'FieldCreateChooseTypeCtrl',
        ['$scope', 'logger', '$detour', '$stateParams', '$location',
            function ($scope, logger, $detour, $stateParams, $location) {

                var entityName = $stateParams.Id;
                $scope.open = function () {
                    $scope.shouldBeOpen = true;
                };

                $scope.close = function () {
                    $scope.shouldBeOpen = false;
                };

                $scope.opts = {
                    backdrop: false,
                    backdropFade: false,
                    dialogFade: true,
                    backdropClick: false
                };

                $scope.fieldType = $('#field-type-form input:first').val();
                $scope.$on('toStep2Done', function() {
                    $scope.close();
                    $(".modal-backdrop").remove();
                });
                $scope.exit = function () {

                    $location.url("/Entities/" + entityName.toString());
                    $scope.close();
                    //$detour.transitionTo('EntityDetail.Fields', { Id: entityName });                  
                };

                $scope.next = function () {
                    if ($scope.fieldType) {
                        
                        $scope.$emit('toStep2', $scope.fieldType);
                        
                    }
                };
            }]
    ]);
});