function FieldEditInfoCtrl($http, $timeout, $scope, logger, $state, $stateParams, $element) {
    $scope.defaultValue = 'false';
    $scope.booleanDisplayOption = 'checkbox';
    $scope.textLength = 255;
    $scope.selectDisplayOption = 'picklist';
    $('.step3').hide();

    $scope.prev = function() {
        $state.transitionTo('SubCreate', { Module: 'Metadata', Id: $stateParams.Id, SubModule: 'Field', View: 'Create' });
    };
    
    $scope.next = function() {
        $('.step2').hide();
        $('.step3').show();
    };

    $scope.save = function() {

    };

    $scope.back = function() {
        $('.step2').show();
        $('.step3').hide();
    };

    $scope.exit = function() {
        $state.transitionTo('Detail', { Module: 'Metadata', Id: $stateParams.Id });
    };
}
//@ sourceURL=Coevery.Metadata/editfieldinfocontroller.js