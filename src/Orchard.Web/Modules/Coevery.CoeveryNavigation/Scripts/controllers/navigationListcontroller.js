function NavigationListCtrl($scope, logger, $state, $stateParams, $resource) {
    $scope.saveAll = function () {
        $.ajax({
            url: myForm.action,
            type: myForm.method,
            data: $(myForm).serialize() + '&submit.Save=Save',
            success: function (result) {
                //$state.transitionTo('Detail', { Module: 'Metadata', Id: $stateParams.Id });
            }
        });
    };
    
    $scope.edit = function (id) {
            $state.transitionTo('Detail', { Module: 'CoeveryNavigation', Id: id });
    };
    
    $scope.add = function (id,menuId) {
        $state.transitionTo('SubDetail', { Module: 'CoeveryNavigation', SubModule: '', View: 'Create', Id: id, SubId: menuId });
    };

    $scope.delete = function(id,url) {
        $.ajax({
            url: url,
            type: 'post',
            data: {id: id, __RequestVerificationToken: $("input[name=__RequestVerificationToken]").val()},
            success: function(result) {
                //$state.transitionTo('Detail', { Module: 'Metadata', Id: $stateParams.Id });
            }
        });
    };
}

//@ sourceURL=Coevery.CoeveryNavigation/navigationListcontroller.js