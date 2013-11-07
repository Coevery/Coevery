
var coeverybusy=angular.module('coevery.Busy', ['ajoslin.promise-tracker']);

coeverybusy.directive('cobusy', ['promiseTracker', function (promiseTracker) {
	    return {
	        restrict: 'A',
	        link: function (scope, element, attrs) {

	            var options = scope.$eval(attrs.cobusy);

	            if (typeof options === 'string') {
	                options = { tracker: options };
	            }

	            if (typeof options === 'undefined' || typeof options.tracker === 'undefined') {
	                throw new Error('Options for cobusy directive must be provided (tracker option is required).');
	            }

	            if (!scope.$coBusyTracker) {
	                scope.$coBusyTracker = {};
	            }
	            
	            scope.$coBusyTracker[options.tracker] = promiseTracker(options.tracker);

	            function toBoolean(value) {
	                if (value && value.length !== 0) {
	                    var v = angular.lowercase("" + value);
	                    value = !(v == 'f' || v == '0' || v == 'false' || v == 'no' || v == 'n' || v == '[]');
	                } else {
	                    value = false;
	                }
	                return value;
	            }
	            
	            //var spinner,height = element.height(),
                //    oldWidth = element.width(),
                //    opts = {
                //        length: Math.round(height / 3),
                //        radius: Math.round(height / 5),
                //        width: Math.round(height / 10),
                //        color: element.css("color"),
                //        left: -5
                //    };
	            
	            

	            var first = true;
	            scope.$watch('$coBusyTracker.' + options.tracker + '.active()', function (value) {
	                //ignore first
	                if (first) {
	                    first = false;
	                    return; 
	                }
	                if (toBoolean(value)) {
	                    //spinner = new Spinner(opts).spin(target);
	                    //element.width(oldWidth + oldWidth / 2);
	                    attrs.$set('disabled', true);
	                } else {
	                    //if (spinner) {
	                    //    spinner.stop();
	                    //    element.width(oldWidth);
	                    //}
	                    attrs.$set('disabled', false);
	                }
	            });

	        }
	    };
	}
    ]);

