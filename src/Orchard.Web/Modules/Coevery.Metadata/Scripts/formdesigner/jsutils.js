function newGuid() {
    var guid = "";
    for (var i = 1; i <= 32; i++) {
        var n = Math.floor(Math.random() * 16.0).toString(16);
        guid += n;
        if ((i == 8) || (i == 12) || (i == 16) || (i == 20))
            guid += "-";
    }
    return guid;
}

String.prototype.format = function() {
    var args = arguments;
    return this.replace(/\{(\d+)\}/g,
        function(match, paren) {
            return args[paren];
        });
};

if (typeof (Array.prototype.indexOf) != "function") {
    Array.prototype.indexOf = function (item, startIndex) {
        var index = startIndex == undefined ? 0 : startIndex;
        if (window.isNaN(index)) {
            throw "the startIndex is not a number";
        }

        var arrLength = this.length;
        if (index < 0) {
            index += arrLength;
        }
        if (index < 0) {
            index = 0;
        }

        for (var i = index; i < this.length; i++) {
            if (this[i] === item) {
                return i;
            }
        }
        return -1;
    };
}

if (typeof (Array.prototype.lastIndexOf) != "function") {
    Array.prototype.lastIndexOf = function (item, startIndex) {
        var index = parseInt(startIndex);
        if (window.isNaN(index)) {
            throw "the startIndex is not a number";
        }

        var arrLength = this.length;
        if (index < 0) {
            index += arrLength;
        }
        if (index >= this.length) {
            index = this.length - 1;
        }

        for (var i = index; i >= 0; i--) {
            if (this[i] === item) {
                return i;
            }
        }
        return -1;
    };
}