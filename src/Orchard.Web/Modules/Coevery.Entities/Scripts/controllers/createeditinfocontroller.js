'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'FieldCreateEditInfoCtrl',
        ['$scope', 'logger', '$stateParams', '$state', '$http',
            function ($scope, logger, $stateParams, $state, $http) {
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
                    $state.transitionTo('EntityDetail.Fields', { Id: entityName });
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
                    //$.ajax({
                    //    url: form.attr('action'),
                    //    type: form.attr('method'),
                    //    data: form.serialize() + '&' + $('#AddInLayout').serialize() + '&submit.Save=Save',
                    //    success: function (result) {                           
                    //        logger.success('success');
                    //        $scope.$parent.getAllField();
                    //        $state.transitionTo('EntityDetail.Fields', { Id: entityName });                          
                    //        $scope.closeDialog();
                    //    },
                    //    error: function (result) {
                    //        logger.error('Failed:\n'+result.responseText);
                    //    }
                    //});

                    var promise = $http({
                        url: form.attr('action'),
                        method: form.attr('method'),
                        data: form.serialize() + '&' + $('#AddInLayout').serialize() + '&submit.Save=Save',
                        headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                    }).then(function () {
                        logger.success('success');
                        $scope.$parent.getAllField();
                        $state.transitionTo('EntityDetail.Fields', { Id: entityName });                          
                        $scope.closeDialog();
                    }, function (reason) {
                        logger.error('Failed:\n' + reason.responseText);
                    });
                    return promise;
                };

                $scope.$on('$destroy', function () {
                    if ($state.current.name != 'EntityDetail.Fields.Create' && $scope.$parent.dialog != null) {
                        $scope.closeDialog();
                    }
                });

                $scope.openDialog();
            }]
    ]);
});

