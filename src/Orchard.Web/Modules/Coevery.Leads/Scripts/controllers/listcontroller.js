lead.controller('LeadCtrl', function ($scope, logger, $location, Lead) {
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
        columnDefs:leadColumnDefs
    };

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