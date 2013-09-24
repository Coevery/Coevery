define(['core/app/detourService', 'core/services/commondataservice', 'core/services/columndefinitionservice', 'core/services/viewdefinitionservice', 'core/services/filterdefinitionservice'], function(detour) {
    detour.registerController([
        'GeneralListCtrl',
        ['$rootScope', '$scope', '$parse', '$http', 'logger', '$compile', '$detour', '$stateParams', '$location', 'commonDataService', 'columnDefinitionService', 'viewDefinitionService', 'filterDefinitionService',
            function($rootScope, $scope, $parse, $http, logger, $compile, $detour, $stateParams, $location, commonDataService, columnDefinitionService, viewDefinitionService, filterDefinitionService) {
                var moduleName = $stateParams.Module;
                var primaryKeyGetter = $parse('ContentId');
                $scope.toolButtonDisplay = false;
                $scope.currentViewId = 0;
                $scope.moduleName = moduleName;
                $scope.definitionViews = [];
                $scope.columnDefs = [];
                $rootScope.moduleName = moduleName;

                //init pagingoption
                var pageSizes = [50, 100, 200];
                var currentPage = parseInt($location.$$search['Page']);
                if (!currentPage) currentPage = 1;
                var pageSize = parseInt($location.$$search['PageSize']);
                if (!pageSize | pageSizes.indexOf(pageSize) < 0) pageSize = 50;
                $scope.pagingOptions = {
                    pageSizes: pageSizes,
                    pageSize: pageSize,
                    currentPage: currentPage
                };

                $scope.getPagedDataAsync = function () {
                    var pageSize = $scope.pagingOptions.pageSize;
                    var page = $scope.pagingOptions.currentPage;
                    $location.search("PageSize", pageSize);
                    $location.search("Page", page);
                    $stateParams["PageSize"] = pageSize;
                    var url = 'api/CoeveryCore/Common/' + moduleName;
                    var data = {
                        PageSize: pageSize,
                        Page: page,
                        ViewId: $scope.currentViewId,
                        FilterGroupId: currentFilterGroupId,
                        Filters: getFilters()
                    };
                    $http.post(url, data).then(function(response) {
                        $scope.myData = response.data.EntityRecords;
                        $scope.totalServerItems = response.data.TotalNumber;
                        $scope.filterDescription = response.data.FilterDescription;
                    }, function() {
                        logger.error("Failed to fetched records for " + moduleName);
                    });
                };

                $scope.$watch('pagingOptions', function(newVal, oldVal) {
                    if (newVal !== oldVal) {
                        if (newVal.pageSize != oldVal.pageSize) {
                            var maxPage = Math.ceil($scope.totalServerItems / newVal.pageSize);
                            //var currentPage = Math.ceil(oldVal.pageSize * $scope.pagingOptions.currentPage / newVal.pageSize);
                            //if (currentPage > maxPage) currentPage = maxPage;
                            if ($scope.pagingOptions.currentPage > maxPage) {
                                $scope.pagingOptions.currentPage = maxPage;
                                return;
                            }
                        }
                        $scope.getPagedDataAsync();
                    }
                }, true);

                $scope.selectedItems = [];
                $scope.gridOptions = {
                    data: 'myData',
                    enablePaging: true,
                    showFooter: true,
                    multiSelect: true,
                    enableRowSelection: true,
                    showSelectionCheckbox: true,
                    selectedItems: $scope.selectedItems,
                    pagingOptions: $scope.pagingOptions,
                    columnDefs: "columnDefs"
                };
                angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);

                // fetch view columns
                $scope.FetchViewColumns = function(viewId) {
                    if (viewId <= 0) return;
                    if (viewId == $scope.currentViewId) return;
                    $scope.currentViewId = viewId;
                    $location.search("ViewId", viewId);
                    var gridColumns = columnDefinitionService.query({ contentType: moduleName, viewId: viewId }, function() {
                        $scope.columnDefs = gridColumns;
                        $scope.getPagedDataAsync();
                    }, function() {
                    });
                };

                $scope.Refresh = function() {
                    $scope.getPagedDataAsync();
                };

                // init views
                $scope.FetchDefinitionViews = function() {
                    var views = viewDefinitionService.query({ contentType: moduleName }, function() {
                        $scope.definitionViews = views;
                        var defaultViewId = $location.$$search['ViewId'];
                        if (!defaultViewId) {
                            views.forEach(function(value, index) {
                                if (value.Default) {
                                    defaultViewId = value.ContentId;
                                }
                            });
                            if (defaultViewId == 0 && views.length > 0)
                                defaultViewId = views[0].ContentId;
                        }
                        $scope.FetchViewColumns(defaultViewId);
                    }, function() {
                        logger.error("Failed to fetched views for " + moduleName);
                    });
                };

                $scope.CreateView = function() {
                    var createViewPath = 'SystemAdmin#/Projections/' + moduleName + '/Create';
                    window.location = createViewPath;
                };

                $scope.FetchDefinitionViews();

                $scope.delete = function(id) {
                    $scope.entityId = id;
                    $('#myModalEntity').modal({
                        backdrop: 'static',
                        keyboard: true
                    });
                };

                $scope.deleteEntity = function() {
                    $('#myModalEntity').modal('hide');
                    var id = $scope.$$childTail.entityId;
                    var ids = [];
                    if (id) {
                        ids.push(id);
                    } else {
                        angular.forEach($scope.selectedItems, function(entity) {
                            ids.push(primaryKeyGetter(entity));
                        }, ids);
                    }
                    commonDataService.delete({ contentId: ids }, function() {
                        $scope.Refresh();
                        logger.success('Delete the ' + moduleName + ' successful.');
                    }, function() {
                        logger.error('Failed to delete the lead.');
                    });
                };

                $scope.add = function() {
                    $detour.transitionTo('Create', { Module: moduleName });
                };

                $scope.edit = function(id) {
                    if (!id && $scope.selectedItems.length > 0) {
                        id = primaryKeyGetter($scope.selectedItems[0]);
                    }
                    $detour.transitionTo('Detail', { Module: moduleName, Id: id });
                };

                $scope.view = function(id) {
                    if (!id && $scope.selectedItems.length > 0) {
                        id = primaryKeyGetter($scope.selectedItems[0]);
                    }
                    $detour.transitionTo('View', { Module: moduleName, Id: id });
                };

                // filters
                $scope.currentFilter = {};
                var needNewFilterEditor = false,
                    currentFilterGroupId = 0;

                $scope.applyFilter = function () {
                    var forms = $('.filterCreatorWrap').find('form');
                    var passValidate = true;
                    forms.each(function() {
                        var validator = $(this).validate({ errorClass: "inputError" });
                        passValidate = validator.form() && passValidate;
                    });
                    if (!passValidate) {
                        return;
                    }
                    var needSave = !!$('#filter-save-box:checked').length;
                    currentFilterGroupId = 0;
                    $scope.getPagedDataAsync();
                    if (needSave) {
                        var filter = {
                            Id: $scope.currentFilter.Id,
                            FilterGroupId: $scope.currentFilter.FilterGroupId,
                            Title: $scope.currentFilter.Title,
                            Filters: getFilters()
                        };

                        var url = 'api/Projections/Filter/' + moduleName;
                        $http.post(url, filter).then(function (response) {
                            $scope.definitionFilters = response.data;
                        }, function () {
                            logger.error("Save filter failed.");
                        });
                    }
                };

                $scope.expendCollapse = function() {
                    if ($('#collapseBtn').hasClass('icon-collapse-hide')) {
                        showFilterEditorZone();
                    } else {
                        hideFilterEditorZone();
                    }
                };
                hideFilterEditorZone();

                $scope.closeFilterCollapse = function() {
                    $('#filterCollapse').hide();
                    currentFilterGroupId = 0;
                    $scope.currentFilter = {};
                    $('.filterCreatorWrap').empty();
                    $scope.getPagedDataAsync();
                };

                $scope.loadFilter = function (filter) {
                    $scope.filterTitle = filter.Title;
                    $scope.filterDescription = '';
                    $scope.currentFilter = filter;
                    currentFilterGroupId = filter.FilterGroupId;
                    needNewFilterEditor = true;
                    $('#filterCollapse').show();
                    hideFilterEditorZone();
                    $scope.getPagedDataAsync();
                };

                $scope.createFilter = function() {
                    $scope.currentFilter = {};
                    $scope.filterTitle = '';
                    $scope.filterDescription = '';
                    needNewFilterEditor = true;
                    $('#filterCollapse').show();
                    showFilterEditorZone();
                };

                $scope.deleteFilter = function (filter) {
                    if (filter.FilterGroupId == currentFilterGroupId) {
                        currentFilterGroupId = 0;
                    }
                    var index = $scope.definitionFilters.indexOf(filter);
                    $scope.definitionFilters.splice(index, 1);
                    filterDefinitionService.delete({ filterId: filter.Id, contentType: moduleName });
                };

                $scope.addNewFilterCondition = function() {
                    addNewEditor();
                };

                $scope.fetchDefinitionFilters = function() {
                    var filters = filterDefinitionService.query({ contentType: moduleName }, function() {
                        $scope.definitionFilters = filters;
                    }, function() {
                        logger.error("Failed to fetched filters for " + moduleName);
                    });
                };
                $scope.fetchDefinitionFilters();

                function displayFilterEditors() {
                    if ($scope.fieldFilters) {
                        loadEditors();
                        return;
                    }
                    var url = 'Coevery/CoeveryCore/Filter/GetFieldFilters/' + moduleName;
                    $http.get(url).then(function (response) {
                        $scope.fieldFilters = response.data;
                        loadEditors();
                    });
                }
                
                function loadEditors() {
                    if (!needNewFilterEditor) {
                        return;
                    }
                    $('.filterCreatorWrap').empty();
                    if ($scope.currentFilter.Id) {
                        var filters = $scope.currentFilter.Filters;
                        $.each(filters, function() {
                            addNewEditor({
                                Type: this.Type,
                                State: this.State
                            });
                        });
                    } else {
                        addNewEditor();
                    }
                    needNewFilterEditor = false;
                }

                function getFilters() {
                    var forms = $('.filterCreatorWrap').find('form');
                    var filters = [];
                    $.each(forms, function() {
                        var form = $(this);
                        filters.push({
                            Type: form.data('Type'),
                            FormData: form.serializeArray()
                        });
                    });
                    return filters;
                }

                function showFilterEditorZone() {
                    displayFilterEditors();
                    $('#collapseBtn').addClass('icon-collapse-show');
                    $('#collapseBtn').removeClass('icon-collapse-hide');
                    $('#closeFilterLink').hide();
                    $('#collapseZone').show();
                }

                function hideFilterEditorZone() {
                    $('#collapseBtn').removeClass('icon-collapse-show');
                    $('#collapseBtn').addClass('icon-collapse-hide');
                    $('#closeFilterLink').show();
                    $('#collapseZone').hide();
                }
                
                function addNewEditor(args) {
                    $scope.filterArgs = args;
                    var editor = $('<filter-editor filter-args="filterArgs" field-filters="fieldFilters"></filter-editor>');
                    $('.filterCreatorWrap').append(editor);
                    $compile(editor)($scope);
                }
            }]
    ]);
});