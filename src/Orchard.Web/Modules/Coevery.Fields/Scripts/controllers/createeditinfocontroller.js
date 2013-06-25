'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'FieldCreateEditInfoCtrl',
        ['$scope', 'logger', '$detour', '$stateParams',
            function ($scope, logger, $detour, $stateParams) {
                var entityName = $stateParams.EntityName;
                
                $('.step3').hide();

                $scope.exit = function () {
                    $detour.transitionTo('EntityDetail.Fields', { Id: entityName });
                };
                $scope.prev = function () {
                    $detour.transitionTo('FieldCreateChooseType', { EntityName: entityName });
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