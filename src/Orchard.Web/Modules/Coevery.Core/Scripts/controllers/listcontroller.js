define(['core/app/detourService', 'core/services/commondataservice', 'core/services/columndefinitionservice', 'core/services/viewdefinitionservice', 'core/services/filterdefinitionservice'], function (detour) {
    detour.registerController([
        'GeneralListCtrl',
        ['$rootScope', '$scope', '$parse', '$http', 'logger', '$compile', '$detour', '$stateParams', '$location', 'commonDataService', 'columnDefinitionService', 'viewDefinitionService', 'filterDefinitionService',
            function ($rootScope, $scope, $parse, $http, logger, $compile, $detour, $stateParams, $location, commonDataService, columnDefinitionService, viewDefinitionService, filterDefinitionService) {
                var navigationId = $stateParams.NavigationId;
                var moduleName = $stateParams.Module;
                $scope.isInit = true;
                $scope.toolButtonDisplay = false;
                $scope.currentViewId = 0;
                $scope.moduleName = moduleName;
                $scope.definitionViews = [];
                $scope.columnDefs = [];
                $rootScope.moduleName = moduleName;

                //init pagingoption
                var pageSizes = [50, 100, 200];
                var currentPage = parseInt($location.$$search['Page'], 10);
                if (!currentPage) currentPage = 1;
                var pageSize = parseInt($location.$$search['Rows'], 10);
                if (!pageSize || pageSizes.indexOf(pageSize) < 0) pageSize = 50;

                var getPostData = function () {
                    return {
                        ViewId: $scope.currentViewId,
                        FilterGroupId: currentFilterGroupId,
                        Filters: getFilters()
                    };
                };
                //var getPageSize = function() {
                //    return pageSize;
                //};
                //var getPage = function() {
                //    return currentPage;
                //};
                $scope.getPagedDataAsync = function () {
                    $("#contentList").jqGrid('setGridParam', {
                        postData: getPostData()
                    }).trigger('reloadGrid');
                };

                // fetch view columns
                $scope.FetchViewColumns = function (viewId) {
                    if (viewId <= 0) return;
                    if (viewId == $scope.currentViewId) return;
                    $scope.currentViewId = viewId;
                    $location.search("ViewId", viewId);
                    var gridColumns = columnDefinitionService.query({ contentType: moduleName, viewId: viewId }, function () {
                        $.each(gridColumns, function (index, value) {
                            if (value.formatter) {
                                value.formatter = $rootScope[value.formatter];
                            }
                        });
                        $scope.columnDefs = gridColumns;
                        if (!$scope.isInit) {
                            $scope.getPagedDataAsync();
                        } else {
                            $scope.gridOptions = {
                                url: 'api/CoeveryCore/Common/' + moduleName,
                                mtype: "post",
                                postData: getPostData(),
                                rowNum: pageSize,
                                rowList: pageSizes,
                                page: currentPage,
                                colModel: $scope.columnDefs,
                                loadComplete: function (data) {
                                    currentPage = data.page;
                                    pageSize = data.records;
                                    $scope.filterDescription = data.filterDescription;
                                    $scope.$apply();
                                },
                                loadError: function (xhr, status, error) {
                                    logger.error("Failed to fetched records for " + moduleName + ":\n" + error);
                                }
                            };
                            angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);
                        }
                    }, function (response) {
                        logger.error("Failed to fetched columns");
                    });
                };

                $scope.Refresh = function () {
                    $scope.getPagedDataAsync();
                };

                // init views
                $scope.FetchDefinitionViews = function () {
                    var views = viewDefinitionService.query({ contentType: moduleName }, function () {
                        $scope.definitionViews = views;
                        var defaultViewId = $location.$$search['ViewId'];
                        if (!defaultViewId) {
                            views.forEach(function (value, index) {
                                if (value.Default) {
                                    defaultViewId = value.ContentId;
                                }
                            });
                            if (defaultViewId == 0 && views.length > 0)
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
                $scope.delete = function (id) {
                    var deleteRelationship = id || $scope.selectedItems.length > 0 ? $scope.selectedItems : null;

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
                        $scope.selectedItems = [];
                    }, function () {
                        logger.error('Failed to delete the lead.');
                    });
                };

                $scope.add = function () {
                    $detour.transitionTo('Create', { NavigationId: navigationId, Module: moduleName });
                };

                $scope.edit = function (id) {
                    if (!id && $scope.selectedItems.length > 0) {
                        id = $scope.selectedItems[0];
                    }
                    $detour.transitionTo('Detail', { NavigationId: navigationId, Module: moduleName, Id: id });
                };

                $scope.view = function (id) {
                    if (!id && $scope.selectedItems.length > 0) {
                        id = $scope.selectedItems[0];
                    }
                    $detour.transitionTo('View', { NavigationId: navigationId, Module: moduleName, Id: id });
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

                $scope.expendCollapse = function () {
                    if ($('#collapseBtn').hasClass('icon-collapse-hide')) {
                        showFilterEditorZone();
                    } else {
                        hideFilterEditorZone();
                    }
                };
                hideFilterEditorZone();

                $scope.closeFilterCollapse = function () {
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

                $scope.createFilter = function () {
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
                        $.each(filters, function () {
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
                    $.each(forms, function () {
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

/*Abandoned code
var primaryKeyGetter = $parse('ContentId');

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
*/
