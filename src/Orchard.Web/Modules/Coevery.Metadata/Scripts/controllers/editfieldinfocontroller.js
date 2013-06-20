function FieldEditInfoCtrl($http, $timeout, $scope, logger, $state, $stateParams, $element) {
    //$scope.defaultValue = 'false';
    //$scope.booleanDisplayOption = 'checkbox';
    $scope.textLength = 255;
    $scope.selectDisplayOption = 'picklist';
    $('.step3').hide();

    $scope.prev = function () {
        $state.transitionTo('SubCreate', { Module: 'Metadata', Id: $stateParams.Id, SubModule: 'Field', View: 'Create' });
    };

    $scope.next = function () {
        $('.step2').hide();
        $('.step3').show();
    };

    $scope.save = function () {
        var form = $element.find('#field-info-form');
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

    $scope.back = function () {
        $('.step2').show();
        $('.step3').hide();
    };

    $scope.exit = function () {
        $state.transitionTo('Detail', { Module: 'Metadata', Id: $stateParams.Id });
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
}
//@ sourceURL=Coevery.Metadata/editfieldinfocontroller.js