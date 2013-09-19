define(['core/app/detourService', 'core/services/navigationdataservice'], function (detour) {
    detour.registerController([
        'NavigationCtrl',
        ['$scope', '$detour', '$stateParams', 'navigationDataService', 'logger',
            function ($scope, $detour, $stateParams, navigationDataService, logger) {
                var navigationId = $stateParams.NavigationId;
                if (navigationId == undefined) navigationId = 0;
                $scope.Init = function () {
                    var navdata = navigationDataService.query({ navigationId: navigationId }, function () {
                        
                        navdata.curr = navdata.curr ? navdata.curr : navdata.alls[0];
                        for (var i = 0; i < navdata.curr.Items.length; i++) {
                            if (navdata.curr.Items[i].Classes.length == 0) {
                                navdata.curr.Items[i].classesstr = "icon-home";
                                continue;
                            }
                            navdata.curr.Items[i].classesstr = "";
                            for (var j = 0; j < navdata.curr.Items[i].Classes.length; j++) {
                                navdata.curr.Items[i].classesstr += navdata.curr.Items[i].Classes[j]+" ";
                            }
                        }
                        //navdata.curr.linkurl = navdata.curr.Items.length == 0 ? navdata.curr.Href : navdata.curr.Items[0].Href;
                        $scope.navdata = navdata;
                        //var warpcontainer = $("#nav-selector>.dropdown");
                        //if (navdata.alls.length == 0) return;
                        //var innerhtml = '<a id="selector" class="dropdown-toggle" role="button" data-toggle="dropdown" href="#">';
                        //navdata.curr = navdata.curr ? navdata.curr : navdata.alls[0];
                        //innerhtml += '<span id="first-menu-title" data-label-placement title="' + navdata.curr.Text.Text + '">' + navdata.curr.Text.Text + '</span><b class="caret"></b>';
                        //innerhtml += '</a>';
                        //innerhtml += '<ul class="dropdown-menu bullet" role="menu" aria-labelledby="dLabel">';
                        //$(navdata.alls).each(function (i, item) {
                        //    innerhtml += '<li data-ng-class="{active: $uiRoute}" data-ui-route="' + item.Href + '">';
                        //    innerhtml += '<a href="' + item.Href + '" title="' + item.Text.Text + '">';
                        //    innerhtml += '<label><strong>' + item.Text.Text + '</strong></label>';
                        //    innerhtml += '</li>';
                        //});
                        //innerhtml += '</ul>';
                        //warpcontainer.append(innerhtml);

                        //innerhtml = '<ul class="collapse in">';
                        //$(navdata.curr.Items).each(function (i, item) {
                        //    if (!item.LocalNav) {
                        //        innerhtml += '<li data-ng-class="{active: $uiRoute}" data-ui-route="' + item.Href + '/?.*">';
                        //        innerhtml += '<a href="' + item.Href + '" title="' + item.Text.Text + '">';  
                        //        innerhtml += '<i class="" vertical-middle"></i>';
                        //        innerhtml += '<span class="nav-title vertical-middle">' + item.Text.Text + '</span>';
                        //        innerhtml += '</a>';
                        //        innerhtml += '</li>';
                        //    }
                        //});
                        //innerhtml += '</ul>';
                        //$("#navigation").append(innerhtml);

                    }, function () {
                        logger.error("Failed to fetched menus in front");
                    });
                }
                $scope.Init();
            }]
    ]);
});