opportunity.controller('opportunityCtrl', function ($scope, logger, $location, opportunity) {
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
            { field: 'OpportunityId', displayName: 'Id' },
            { field: 'Name', displayName: 'Name' },
            { field: 'Description', displayName: 'Description' },
            { field: 'SourceLeadId', displayName: 'SourceLeadId' }]
    };

    $scope.delete = function () {
        if ($scope.mySelections.length > 0) {
            opportunity.delete({ opportunityId: $scope.mySelections[0].OpportunityId }, function () {
                $scope.mySelections.pop();
                $scope.getOpportunities();
                logger.success("Delete the opportunity successful.");
            }, function () {
                logger.error("Failed to delete the opportunity.");
            });
        }
    };

    $scope.add = function () {
        $location.path('Detail');
    };

    $scope.edit = function () {
        if ($scope.mySelections.length > 0) {
            $location.path('Detail/' + $scope.mySelections[0].OpportunityId);
        }
    };

    $scope.getOpportunities = function () {
        var opportunitys = opportunity.query(function () {
            $scope.myData = opportunitys;
        }, function() {
            logger.error("Failed to fetched opportunitys.");
        });
    };

    $scope.getOpportunities();
});