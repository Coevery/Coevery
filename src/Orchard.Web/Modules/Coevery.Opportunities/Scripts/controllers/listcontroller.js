function OpportunityCtrl($scope, logger, $state, $resource) {
    var Opportunity = OpportunityContext($resource);
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
        columnDefs: opportunityColumDefs
    };

    $scope.delete = function() {
        if ($scope.mySelections.length > 0) {
            Opportunity.delete({ opportunityId: $scope.mySelections[0].OpportunityId }, function () {
                $scope.mySelections.pop();
                $scope.getOpportunities();
                logger.success("Delete the opportunity successful.");
            }, function() {
                logger.error("Failed to delete the opportunity.");
            });
        }
    };

    $scope.add = function() {
        $state.transitionTo('Create', { Moudle: 'Opportunities' });
    };

    $scope.edit = function() {
        if ($scope.mySelections.length > 0) {
            $state.transitionTo('Detail', { Moudle: 'Opportunities', Id: $scope.mySelections[0].OpportunityId });
        }
    };

    $scope.getOpportunities = function() {
        var opportunitys = Opportunity.query(function () {
            $scope.myData = opportunitys;
        }, function() {
            logger.error("Failed to fetched opportunitys.");
        });
    };

    $scope.getOpportunities();
}