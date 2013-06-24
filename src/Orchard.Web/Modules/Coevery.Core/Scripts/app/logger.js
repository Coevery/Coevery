define(['app'], function (app) {
    app.factory('logger', function ($window) {
        var logger = {
            error: error,
            info: info,
            success: success,
            notice: notice,
            log: log
        };

        function error(message) {
            $.pnotify({
                title: 'Error',
                text: message,
                type: 'error'
            });
            log("Error: " + message);
        };

        function info(message) {
            $.pnotify({
                title: 'Information',
                text: message,
                type: 'info'
            });
            log("Info: " + message);
        };

        function success(message) {
            $.pnotify({
                title: 'Success',
                text: message,
                type: 'success'
            });
            log("Success: " + message);
        };

        function notice(message) {
            $.pnotify({
                title: 'Notice',
                text: message
            });
            log("Notice: " + message);
        };

        // IE and google chrome workaround
        // http://code.google.com/p/chromium/issues/detail?id=48662
        function log() {
            var console = $window.console;
            !!console && console.log && console.log.apply && console.log.apply(console, arguments);
        }

        return logger;
    });
});