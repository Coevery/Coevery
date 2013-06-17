PerspectiveDetailCtrl.$inject = ['$rootScope', '$scope', 'logger', '$state', 'localize', '$resource', '$stateParams'];

function PerspectiveDetailCtrl($rootScope, $scope, logger, $state, localize, $resource, $stateParams) {
    
    $scope.mySelections = [];
    var perpectiveId = $stateParams.Id;
    var navigation = NavigationContext($resource);
    var perspective = PerspectiveContext($resource);

    var navigationColumnDefs = [
        { field: 'Id', displayName: 'Actions', width: 100, cellTemplate: '<div class="ngCellText" ng-class="col.colIndex()"><a ng-click="edit(row.getProperty(col.field))">Edit</a>&nbsp;<a ng-click="delete(row.getProperty(col.field))">Remove</a></div>' },
        { field: 'Id', displayName: localize.getLocalizedString('Id') },
        { field: 'DisplayName', displayName: localize.getLocalizedString('DisplayName') }];

    $scope.gridOptions = {
        data: 'myData',
        selectedItems: $scope.mySelections,
        multiSelect: false,
        showColumnMenu: true,
        enableColumnResize: true,
        enableColumnReordering: true,
        columnDefs: navigationColumnDefs
    };

    $scope.exit = function() {
        $state.transitionTo('SubList', { Module: 'Metadata', SubModule: 'Perspective', View: 'List' });
    };
    
    $scope.addNavigationItem = function() {
        $state.transitionTo('SubDetail', { Module: 'Metadata', SubModule: 'Perspective', Id: perpectiveId, SubId: 0, View: 'EditNavigationItem' });
    };
    
    $scope.save = function () {
        $.ajax({
            url: myForm.action,
            type: myForm.method,
            data: $(myForm).serialize() + '&submit.Save=Save',
            success: function (result) {
                logger.success("Perspective Saved.");
            }
        });
    };
    
    $scope.edit = function (navigationId) {
        $state.transitionTo('SubDetail', { Module: 'Metadata', SubModule: 'Perspective', Id: perpectiveId, SubId: navigationId, View: 'EditNavigationItem' });
    };
    
    $scope.delete = function (navigationId) {
        perspective.delete({ Id: navigationId }, function () {
            $scope.getAllNavigationdata();
            logger.success('Delete the ' + moduleName + ' successful.');
        }, function () {
            logger.error('Failed to delete the ' + moduleName);
        });
    };
    
    $scope.deletePerspective = function () {
        navigation.delete({ Id: perpectiveId }, function () {
            $scope.exit();
        }, function () {
            logger.error('Failed to delete the ' + moduleName);
        });
    };
  
    
    $scope.getAllNavigationdata = function () {
        var navigations = navigation.query({ Id: perpectiveId }, function () {
            $scope.myData = navigations;
        }, function () {
            logger.error("Failed to fetched Metadata.");
        });
    };

    $scope.getAllNavigationdata();
}

//@ sourceURL=Coevery.Metadata/perspectivedetailcontroller.js