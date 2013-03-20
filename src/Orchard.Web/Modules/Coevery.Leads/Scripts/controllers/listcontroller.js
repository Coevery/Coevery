function LeadCtrl($scope, logger, $state, localize, $resource) {
    var Lead = LeadContext($resource);
    $scope.mySelections = [];
    
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
        columnDefs: leadColumnDefs
    };


    $scope.$on("localizeResourcesUpdates", function () {
        for (var colIndex = 0; colIndex < $scope.gridOptions.$gridScope.columns.length; colIndex++) {
            $scope.gridOptions.$gridScope.columns[colIndex].displayName
                = localize.getLocalizedString($scope.gridOptions.$gridScope.columns[colIndex].field);
        }
    });
    
    $scope.delete = function() {
        if ($scope.mySelections.length > 0) {
            Lead.delete({ leadId: $scope.mySelections[0].LeadId }, function() {
                $scope.mySelections.pop();
                $scope.getAllLeads();
                logger.success("Delete the lead successful.");
            }, function() {
                logger.error("Failed to delete the lead.");
            });
        }
    };

    $scope.add = function () {
        $state.transitionTo('Create', { Moudle: 'Leads' });
    };

    $scope.edit = function() {
        if ($scope.mySelections.length > 0) {
            $state.transitionTo('Detail', { Moudle: 'Leads', Id: $scope.mySelections[0].LeadId });
        }
    };

    $scope.getAllLeads = function() {
        var leads = Lead.query(function() {
            $scope.myData = leads;
        }, function() {
            logger.error("Failed to fetched leads.");
        });
    };

    $scope.getAllLeads();
}