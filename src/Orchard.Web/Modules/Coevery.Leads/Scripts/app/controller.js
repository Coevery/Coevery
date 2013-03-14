lead.controller('LeadCtrl', function ($scope, logger, $location, Lead, localize) {
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
        columnDefs: [
            { field: 'LeadId', displayName: 'LeadId' },
            { field: 'Topic', displayName: 'Topic' },
            { field: 'StatusCode', displayName: 'Status' },
            { field: 'FirstName', displayName: 'FirstName' },
            { field: 'LastName', displayName: 'LastName' }]
    };

    $scope.$on('localizeResourcesUpdates', function () {
        for (var i = 0; i < $scope.gridOptions.$gridScope.columns.length; i++) {
            $scope.gridOptions.$gridScope.columns[i].displayName
                = localize.getLocalizedString('_' + $scope.gridOptions.$gridScope.columns[i].field + '_');
        }
    });
    
    $scope.delete = function () {
        if ($scope.mySelections.length > 0) {
            Lead.delete({ leadId: $scope.mySelections[0].LeadId }, function () {
                $scope.mySelections.pop();
                $scope.getAllLeads();
                logger.success("Delete the lead successful.");
            }, function () {
                logger.error("Failed to delete the lead.");
            });
        }
    };

    $scope.add = function () {
        $location.path('Detail');
    };

    $scope.edit = function () {
        if ($scope.mySelections.length > 0) {
            $location.path('Detail/' + $scope.mySelections[0].LeadId);
        }
    };

    $scope.getAllLeads = function () {
        var leads = Lead.query(function () {
            $scope.myData = leads;
        }, function() {
            logger.error("Failed to fetched leads.");
        });
    };

    $scope.getAllLeads();
});