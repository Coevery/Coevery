/* logger: display color-coded messages in "toasts" and to console */

// create and add logger service to the Ng injector
metadata.factory('logger', function ($window) {

    // This logger wraps the toastr logger and also logs to console
    // toastr.js is library by John Papa that shows messages in pop up toast.
    // https://github.com/CodeSeven/toastr

    var logger = {
        error: error,
        info: info,
        success: success,
        notice: notice,
        log: log // straight to console; bypass toast
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