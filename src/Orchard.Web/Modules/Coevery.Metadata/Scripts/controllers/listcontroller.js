lead.controller('MetadataCtrl', function ($scope, logger, $location, Metadata) {
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
        columnDefs:metadataColumnDefs
    };

    $scope.delete = function () {
        if ($scope.mySelections.length > 0) {
            Metadata.delete({ leadId: $scope.mySelections[0].Name }, function () {
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

    //$scope.getAllLeads = function () {
    //    var leads = Metadata.query(function () {
    //        $scope.myData = leads;
    //    }, function() {
    //        logger.error("Failed to fetched leads.");
    //    });
    //};

    //$scope.getAllLeads();
});