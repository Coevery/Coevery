define(['core/app/detourService', 'core/services/entitydataservice', 'core/services/gridcolumndataservice', 'core/services/viewdefinitionservice', 'core/services/filterdefinitionservice'], function (detour) {
    detour.registerController([
        'GeneralListCtrl',
        ['$rootScope', '$scope', '$parse', '$http', 'logger', '$compile', '$state', '$stateParams', '$location', 'commonDataService', 'gridColumnDataService', 'viewDefinitionService', 'filterDefinitionService',
            function ($rootScope, $scope, $parse, $http, logger, $compile, $state, $stateParams, $location, commonDataService, gridColumnDataService, viewDefinitionService, filterDefinitionService) {
                var navigationId = $stateParams.NavigationId;
                var moduleName = $stateParams.Module;
                $scope.isInit = true;
                $scope.toolButtonDisplay = false;
                $scope.currentViewId = 0;
                $scope.moduleName = moduleName;
                $scope.definitionViews = [];
                $rootScope.moduleName = moduleName;
                $scope.dataCanModify = true;

                //init pagingoption
                var pageSizes = [50, 100, 200];
                var currentPage = parseInt($location.$$search.Page, 10);
                if (!currentPage) currentPage = 1;
                var pageSize = parseInt($location.$$search.Rows, 10);
                if (!pageSize || pageSizes.indexOf(pageSize) < 0) pageSize = 50;

                var getPostData = function () {
                    return {
                        ViewId: $scope.currentViewId,
                        FilterGroupId: currentFilterGroupId,
                        Filters: JSON.stringify(getFilters())
                    };
                };

                $scope.getPagedDataAsync = function () {
                    $scope.contentList.setParam({ postData: getPostData() });
                    $scope.contentList.reload();
                };

                // fetch view columns
                $scope.FetchViewColumns = function (viewId) {
                    if (viewId <= 0) return;
                    if (viewId === $scope.currentViewId) return;
                    var needGridReloading = true;
                    $scope.currentViewId = viewId;
                    $location.search("ViewId", viewId);
                    var gridColumnQuery = gridColumnDataService.get({ contentType: moduleName, viewId: viewId }, function () {
                        $.each(gridColumnQuery.colModel, function (index, value) {
                            if (value.formatter) {
                                value.formatter = $rootScope[value.formatter];
                            }
                        });
                        
                        if (!$scope.isInit && !needGridReloading) {
                            $scope.getPagedDataAsync();
                        } else {
                            var gridOptions = {
                                url: 'api/projections/entity/' + moduleName,
                                mtype: "post",
                                postData: getPostData(),
                                rowNum: pageSize,
                                rowList: pageSizes,
                                needReloading: needGridReloading && !$scope.isInit,
                                page: currentPage,
                                loadComplete: function (data) {
                                    currentPage = data.page;
                                    pageSize = data.records;
                                    if (currentFilterGroupId === 0) {
                                        $scope.filterDescription = data.filterDescription;
                                    }
                                    $scope.$apply();
                                },
                                loadError: function (xhr, status, error) {
                                    logger.error("Failed to fetched records for " + moduleName + ":\n" + error);
                                }
                            };
                            
                            $scope.gridOptions = angular.extend({}, $rootScope.defaultGridOptions, gridOptions, gridColumnQuery);
                            $scope.isInit = false;
                        }
                    }, function (response) {
                        logger.error("Failed to fetched columns");
                    });
                };

                $scope.refresh = function () {
                    $scope.selectedItems = [];
                    $scope.getPagedDataAsync();
                };

                $scope.$on("grid.refresh", function (event) {
                    event.stopPropagation();
                    $scope.refresh();
                });
                
                $scope.$on("version.change", function (event,arg) {
                    event.stopPropagation();
                    $scope.dataCanModify = arg;
                });

                // init views
                $scope.FetchDefinitionViews = function () {
                    var views = viewDefinitionService.query({ contentType: moduleName }, function () {
                        if (views.length == 0) {
                            logger.info("No view for " + moduleName+", please create one.");
                            return;
                        }
                        $scope.definitionViews = views;
                        var defaultViewId = $location.$$search.ViewId;
                        if (!defaultViewId) {
                            views.forEach(function (value, index) {
                                if (value.Default) {
                                    defaultViewId = value.ContentId;
                                }
                            });
                            if (defaultViewId === 0 && views.length > 0)
                                defaultViewId = views[0].ContentId;
                        }
                        $scope.FetchViewColumns(defaultViewId);
                    }, function () {
                        logger.error("Failed to fetched views for " + moduleName);
                    });
                };

                $scope.CreateView = function () {
                    var createViewPath = 'SystemAdmin#/Projections/' + moduleName + '/Create';
                    window.location = createViewPath;
                };

                $scope.FetchDefinitionViews();



                /*Grid methods*/
                $scope['delete'] = function (id) {
                    var deleteRelationship = id || ($scope.selectedItems.length > 0 ? $scope.selectedItems : null);

                    if (!deleteRelationship) {
                        logger.error('No data selected.');
                        return;
                    }
                    var ids;
                    if ($.isArray(deleteRelationship)) {
                        ids = deleteRelationship.join(",");
                    } else {
                        ids = deleteRelationship.toString();
                    }
                    commonDataService.delete({ contentId: ids }, function () {
                        $scope.Refresh();
                        logger.success('Delete the ' + moduleName + ' successful.');
                        $scope.entityId = [];
                    }, function (reason) {
                        logger.error(reason.data.Message);
                    });
                };

                $scope.add = function (params) {
                    var temp = !params ? '' : params;
                    $state.transitionTo('Root.Menu.Create', { NavigationId: navigationId, Module: moduleName, Params: temp });
                };

                $scope.edit = function (id) {
                    if (!id && $scope.selectedItems.length > 0) {
                        id = $scope.selectedItems[0];
                    }
                    $state.transitionTo('Root.Menu.Detail', { NavigationId: navigationId, Module: moduleName, Id: id });
                };

                $scope.view = function (id) {
                    if (!id && $scope.selectedItems.length > 0) {
                        id = $scope.selectedItems[0];
                    }
                    $state.transitionTo('Root.Menu.View', { NavigationId: navigationId, Module: moduleName, Id: id });
                };

                // filters
                $scope.currentFilter = {};
                var needNewFilterEditor = false,
                    currentFilterGroupId = 0;

                $scope.applyFilter = function () {
                    var forms = $('.filterCreatorWrap').find('form');
                    var passValidate = true;
                    forms.each(function () {
                        var validator = $(this).validate({ errorClass: "inputError" });
                        passValidate = validator.form() && passValidate;
                    });
                    if (!passValidate) {
                        return;
                    }
                    
                    currentFilterGroupId = 0;
                    $scope.getPagedDataAsync();
                    if ($scope.needSaveFilter) {
                        var title = $scope.currentFilter.Title || 'Filter';
                        title = title.trim() || 'Filter';
                        var filter = {
                            Title: title,
                            Filters: getFilters()
                        };

                        var url = 'api/Projections/Filter/' + moduleName;
                        $http.post(url, filter).then(function (response) {
                            $scope.definitionFilters = response.data;
                        }, function () {
                            logger.error("Save filter failed.");
                        });
                        $scope.currentFilter.Title = null;
                        $scope.needSaveFilter = false;
                    }
                };

                $scope.expendCollapse = function () {
                    if (!$scope.showFilterEditorZone) {
                        showFilterEditorZone();
                    } else {
                        hideFilterEditorZone();
                    }
                };
                hideFilterEditorZone();

                $scope.clearFilter = function () {
                    $scope.showFilter = false;
                    currentFilterGroupId = 0;
                    $scope.needSaveFilter = false;
                    $scope.currentFilter = {};
                    $('.filterCreatorWrap').empty();
                    $scope.getPagedDataAsync();
                };

                $scope.loadFilter = function (filter) {
                    $scope.filterDescription = filter.Title;
                    $scope.currentFilter = filter;
                    currentFilterGroupId = filter.FilterGroupId;
                    needNewFilterEditor = true;
                    $scope.showFilter = true;
                    hideFilterEditorZone();
                    $scope.getPagedDataAsync();
                };

                $scope.createFilter = function () {
                    $scope.currentFilter = {};
                    $scope.filterDescription = '';
                    needNewFilterEditor = true;
                    $scope.showFilter = true;
                    showFilterEditorZone();
                };

                $scope.deleteFilter = function (filter) {
                    if (filter.FilterGroupId === currentFilterGroupId) {
                        currentFilterGroupId = 0;
                    }
                    var index = $scope.definitionFilters.indexOf(filter);
                    $scope.definitionFilters.splice(index, 1);
                    filterDefinitionService.delete({ filterId: filter.Id, contentType: moduleName });
                };

                $scope.addNewFilterCondition = function () {
                    addNewEditor();
                };

                $scope.fetchDefinitionFilters = function () {
                    var filters = filterDefinitionService.query({ contentType: moduleName }, function () {
                        $scope.definitionFilters = filters;
                    }, function () {
                        logger.error("Failed to fetched filters for " + moduleName);
                    });
                };
                $scope.fetchDefinitionFilters();

                function displayFilterEditors() {
                    if ($scope.fieldFilters) {
                        loadEditors();
                        return;
                    }
                    var url = 'Projections/FilterFields/GetFieldFilters/' + moduleName;
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
                        $.each(filters, function () {
                            addNewEditor({
                                Type: this.Type,
                                Category: this.Category,
                                State: this.State
                            });
                        });
                    } else {
                        addNewEditor();
                    }
                    needNewFilterEditor = false;
                }

                function getFilters() {
                    var forms = $('form.filter-form');
                    var filters = [];
                    $.each(forms, function () {
                        var form = $(this);
                        filters.push({
                            Type: form.data('type'),
                            Category: form.data('category'),
                            FormData: form.serializeArray()
                        });
                    });
                    return filters;
                }

                function showFilterEditorZone() {
                    displayFilterEditors();
                    $scope.showFilterEditorZone = true;
                }

                function hideFilterEditorZone() {
                    $scope.showFilterEditorZone = false;
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
