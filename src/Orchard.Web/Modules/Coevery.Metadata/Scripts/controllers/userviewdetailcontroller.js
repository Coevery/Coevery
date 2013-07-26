function UserViewDetailCtrl($scope, logger, $state, $stateParams, $resource, $compile) {
    var name = $stateParams.Id;
    var metadata = UserViewContext($resource);
    var isNew = (name || name == '') ? false : true;
    $scope.mySelections = [];
    $scope.filedCoumns = [];
    
    $scope.gridOptions = {
        data: 'myData',
        selectedItems: $scope.mySelections,
        multiSelect: false,
        enableRowSelection: false,
        showColumnMenu: true,
        enableColumnResize: true,
        enableColumnReordering: true,
        columnDefs: 'filedCoumns'
    };

    $scope.preview = function () {
        var selectedFieldItems = $('#sortable >li');
        var index = 0;
        $scope.filedCoumns = [];
        selectedFieldItems.each(function (k, v) {
            var fieldName = v.id.replace('SelectedField', '');
            $scope.filedCoumns[index] = { field: fieldName, displayName: fieldName };
            index++;
        });
    };

    $scope.save = function () {
        var selectedFieldItems = $('#sortable >li');
        var pickListValue = '';
        selectedFieldItems.each(function (k, v) {
            var fieldName = v.id.replace('SelectedField', '');
            pickListValue += fieldName + '$';
        });
        $('#picklist')[0].value = pickListValue;
        $.ajax({
            url: myForm.action,
            type: myForm.method,
            data: $(myForm).serialize() + '&submit.Save=Save',
            success: function (result) {
                logger.success("Layout Saved.");
            }
        });
    };

    $scope.change = function () {

    };

    $scope.exit = function () {
        $state.transitionTo('SubList', { Module: 'Metadata', SubModule: 'Projection', View: 'List', Id: $stateParams.Id });
    };

    $scope.addfield = function (fieldName) {
        var elementTemp = '<li id="SelectedField'+fieldName+'" class="ui-state-default">'+fieldName+'';
        elementTemp += '<div class="pull-right">';
        elementTemp += '<button class="btn-link" type="button"  ng-click="removefield(\''+fieldName+'\')">Remove</button>';
        elementTemp += '</div>';
        elementTemp += '</li>';
        $compile(elementTemp)($scope).appendTo($('#sortable'));
        $('#UnSelectedLabelField' + fieldName).removeClass('label hide');
        $('#UnSelectedLabelField' + fieldName).addClass('label');
        $('#UnSelectedButtonField' + fieldName).css('display', 'none');
    };
    
    $scope.removefield = function (fieldName) {
        $('#SelectedField' + fieldName).remove();
        $('#UnSelectedLabelField' + fieldName).removeClass('label');
        $('#UnSelectedLabelField' + fieldName).addClass('label hide');
        $('#UnSelectedButtonField' + fieldName).css('display', 'block');
    };
    
}

//@ sourceURL=Coevery.Metadata/userviewDetailcontroller.js