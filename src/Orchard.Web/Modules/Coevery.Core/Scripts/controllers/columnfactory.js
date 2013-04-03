
function getColumnDefs(localize) {
    var columnDefs = [{ field: 'Id', displayName: localize.getLocalizedString('Id') },
                { field: 'Topic', displayName: localize.getLocalizedString('Topic') },
                { field: 'StatusCode', displayName: localize.getLocalizedString('StatusCode') },
                { field: 'FirstName', displayName: localize.getLocalizedString('FirstName') },
                { field: 'LastName', displayName: localize.getLocalizedString('LastName') }];
    debugger;
    return columnDefs;
}