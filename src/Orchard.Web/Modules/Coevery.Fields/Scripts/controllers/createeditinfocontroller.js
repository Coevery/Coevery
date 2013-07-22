'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'FieldCreateEditInfoCtrl',
        ['$scope', 'logger', '$stateParams', '$detour',
            function ($scope, logger, $stateParams, $detour) {
                var entityName = $stateParams.Id;
                var checkValid = function (form) {
                    var validator = form.validate();
                    if (!validator) {
                        return false;
                    }
                    if (!validator.form()) {
                        return false;
                    }
                    if (!validator.element("#inputFieldName")) {
                        return false;
                    }
                    return true;
                };

                $(".step3").hide();
                //Scope method
                $scope.exit = function () {
                    $detour.transitionTo('EntityDetail.Fields', { Id: entityName });
                };
                $scope.prev = function () {
                    $scope.$emit('toStep1');
                };
                $scope.back = function () {
                    $('.step2').show();
                    $('.step3').hide();
                };

                $scope.next = function () {
                    if (!checkValid($("#field-info-form"))) {
                        return;
                    }
                    $scope.fieldName = $("#inputFieldName").val();
                    $('.step2').hide();
                    $('.step3').show();
                };

                $scope.save = function () {
                    if (!checkValid($("#field-info-form"))) {
                        return;
                    }
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
                    $detour.transitionTo('EntityDetail.Fields', { Id: entityName });
                };

                $scope.$on('$destroy', function () {
                    if ($detour.current.name != 'EntityDetail.Fields.Create') {
                        $scope.closeDialog();
                    }
                });

                $scope.openDialog();
            }]
    ]);
});

