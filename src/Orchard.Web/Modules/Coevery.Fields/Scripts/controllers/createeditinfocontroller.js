'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'FieldCreateEditInfoCtrl',
        ['$scope', 'logger', '$detour', '$stateParams','$location',
            function ($scope, logger, $detour, $stateParams, $location) {
                var entityName = $stateParams.Id;

                $scope.open = function() {
                    $(".step3").hide();
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

                $scope.$on('toStep1Done', function () {
                    $scope.close();
                    $(".modal-backdrop").remove();
                });

                $scope.exit = function () {
                    //$detour.transitionTo('EntityDetail.Fields', { Id: entityName });
                    
                    $location.url("/Entities/" + entityName.toString());
                    $scope.close();
                };
                $scope.prev = function () {
                    //$detour.transitionTo('EntityDetail.Fields.Create', { Id: entityName });
 
                    $scope.$emit('toStep1');
                    
                };
                $scope.back = function () {
                    $('.step2').show();
                    $('.step3').hide();
                };
                $scope.next = function () {
                    $('.step2').hide();
                    $('.step3').show();
                };

                $scope.save = function () {
                    var form = $('#field-info-form');
                    $.ajax({
                        url: form.attr('action'),
                        type: form.attr('method'),
                        data: form.serialize() + '&' + $('#AddInLayout').serialize() + '&submit.Save=Save',
                        success: function (result) {
                            logger.success('success');
                        },
                        error: function () {
                            logger.error('Failed');
                        }
                    });
                    $location.url("/Entities/" + entityName.toString());
                    $scope.close();
                };

                $('#DisplayName').keyup(copyName);
                $('#DisplayName').blur(copyName);
                function copyName() {
                    var names = $('#DisplayName').val().split(' ');
                    var fieldName = '';
                    $.each(names, function () {
                        fieldName += this;
                    });
                    $scope.fieldName = fieldName;
                    $scope.$apply();
                }
            }]
    ]);
});