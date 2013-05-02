function UserViewDetailCtrl($scope, logger, $state, $stateParams, $resource) {
    var name = $stateParams.Id;
    var metadata = UserViewContext($resource);
    var isNew = (name || name == '') ? false : true;
    $scope.mySelections = [];
    $scope.filedCoumns = [];
    
    $scope.gridOptions = {
        data: 'myData',
        //enableCellSelection: true,
        //enableRowSelection: false,
        //showSelectionCheckbox: true,
        selectedItems: $scope.mySelections,
        multiSelect: false,
        showColumnMenu: true,
        enableColumnResize: true,
        enableColumnReordering: true,
        //enableCellEdit: true,
        columnDefs: 'filedCoumns'
    };

    $scope.preview = function () {
        var obj = picklistobj;
       
        var columns = obj.children();
        $scope.filedCoumns = new Array();
        var index = 0;
        columns.each(function (k, v) {
            var fieldName = $(v).text();
            
            var selected = $(v).attr("selected");
            if (selected) {
                
                $scope.filedCoumns[index] = { field: fieldName, displayName: fieldName };
            }
            index++;
        });
       // $scope.myData = new Array();
    };

    $scope.save = function () {
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

    function getFieldName(fieldId) {

        return "LeadName";
    }
 
   
    $scope.addfield = function (fieldId) {
        var fieldName = getFieldName(fieldId);
        var addFieldId = fieldId + 'Add';
        var elementTemp = '<li id ="{1}" class="ui-state-default "><span class="icon-th-list"></span>{0}<span class="fieldbtn"><a  href="javascript:void(0);"  ng-click="removefield(\'{1}\')" >Remove</a></span></li>';
        var element = $.format(elementTemp, fieldName, addFieldId);
        $('#sortable').append(element);
      
    };
    
    $scope.removefield = function (fieldId) {
        $('#' + fieldId).remove();
    };
    
}


$.format = function (source, params) { 
    if (arguments.length == 1) 
        return function () { 
            var args = $.makeArray(arguments); 
            args.unshift(source); 
            return $.format.apply(this, args); 
        }; 
    if (arguments.length > 2 && params.constructor != Array) { 
        params = $.makeArray(arguments).slice(1); 
    } 
    if (params.constructor != Array) { 
        params = [params]; 
    } 
    $.each(params, function (i, n) { 
        source = source.replace(new RegExp("\\{" + i + "\\}", "g"), n); 
    }); 
    return source; 
}; 

//@ sourceURL=Coevery.Metadata/userviewDetailcontroller.js