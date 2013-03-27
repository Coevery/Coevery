
Array.prototype.Sum = function (property) {
    var items = this;
    var total = 0;
    var isFunction = typeof (property) == "function";
    for (var i = 0; i < items.length; i++) {
        if (isFunction) {
            total += property(items[i]);
        }
        else {
            var value = items[i][property];
            if (value != undefined && value != null) {
                total += value * 1;
            }
        }
    }
    return total;
};

Array.prototype.Where = function (predicateFunction) {
    var results = new Array();
    var items = this;
    for (var i = 0; i < items.length; i++) {
        var item = items[i];
        if (predicateFunction(item)) {
            results.push(item);
        }
    }
    return results;
};

Array.prototype.OrderBy = function (property, compare) {
    var items = this;
    for (var i = 0; i < items.length - 1; i++) {
        for (var j = 0; j < items.length - 1 - i; j++) {
            if (isFirstGreaterThanSecond(items[j], items[j + 1])) {
                var temp = items[j + 1];
                items[j + 1] = items[j];
                items[j] = temp;
            }
        }
    }
    function isFirstGreaterThanSecond(first, second) {
        if (compare != undefined) {
            return compare(first, second);
        }
        else if (property == undefined || property == null) {
            return first > second;
        }
        else {
            return first[property] > second[property];
        }
    }

    return items;
};

Array.prototype.OrderByDescending = function (property, compare) {
    var items = this;
    for (var i = 0; i < items.length - 1; i++) {
        for (var j = 0; j < items.length - 1 - i; j++) {
            if (!isFirstGreaterThanSecond(items[j], items[j + 1])) {
                var temp = items[j + 1];
                items[j + 1] = items[j];
                items[j] = temp;
            }
        }
    }
    function isFirstGreaterThanSecond(first, second) {
        if (compare != undefined) {
            return compare(first, second);
        }
        else if (property == undefined || property == null) {
            return first > second;
        }
        else {
            return first[property] > second[property];
        }
    }
    return items;
};

Array.prototype.GroupBy = function (predicate) {
    var results = [];
    var items = this;

    var keys = {}, index = 0;
    for (var i = 0; i < items.length; i++) {
        var selector;
        if (typeof predicate === "string") {
            selector = items[i][predicate];
        } else {
            selector = predicate(items[i]);
        }
        if (keys[selector] === undefined) {
            keys[selector] = index++;
            results.push({ key: selector, value: [items[i]] });
        } else {
            results[keys[selector]].value.push(items[i]);
        }
    }
    return results;
};

Array.prototype.Each = function (func) {
    var items = this;
    for (var i = 0; i < items.length; i++) {
        var ret = func(i, items[i]);
        if (ret === false) {
            break;
        }
    }
};

Array.prototype.Skip = function (count) {
    var results = new Array();
    var items = this;
    for (var i = count; i < items.length; i++) {
        results.push(items[i]);
    }
    return results;
};

Array.prototype.Take = function (count) {
    var results = new Array();
    var items = this;
    for (var i = 0; i < items.length && i < count; i++) {
        results.push(items[i]);
    }
    return results;
};

Array.prototype.FirstOrDefault = function (predicateFunction) {
    var items = this;
    if (predicateFunction == undefined && items.length > 0) {
        return items[0];
    }
    for (var i = 0; i < items.length; i++) {
        var item = items[i];
        if (predicateFunction(item)) {
            return item;
        }
    }
    return null;
};

Array.prototype.Any = function (predicateFunction) {
    var items = this;
    if (predicateFunction == undefined) {
        return items.length > 0;
    }
    for (var i = 0; i < items.length; i++) {
        if (predicateFunction(items[i])) {
            return true;
        }
    }
    return false;
};



/*
 * 以下方法将在现有集合上做更改，不会返回新的集合。
 */


/*
 * predecateFunction's arg: the item in array
 * predecateFunction's return value: boolean
 */
Array.prototype.Remove = function (predicate) {
    var index = this.length;
    if (typeof predicate == "function") {
        for (var i = 0; i < this.length; i++) {
            var result = predicate(this[i]);
            if (result) {
                index = i;
                break;
            }
        }
    } else {
        index = this.indexOf(predicate);
    }
    return this.splice(index, 1);
};

//Array.prototype.Interchange = function (predicate1, predicate2) {
//    var index1 = this.length, index2 = this.length;
//    if (typeof predicate == "function") {
//        for (var i = 0; i < this.length; i++) {
//            var result1 = predicate1(this[i]);
//            var result2 = predicate2(this[i]);
//            if (result1) {
//                index1 = i;
//            }
//            if (result2) {
//                index2 = i;
//            }
//        }
//    } else {
//        index1 = this.indexOf(predicate1);
//        index2 = this.indexOf(predicate2);
//    }
//    var temp = this[index1];
//    this[index1] = this[index2];
//    this[index2] = temp;
//}

Array.prototype.Insert = function (obj, index) {
    this.splice(index, 0, obj);
};

Array.prototype.Clear = function () {
    this.splice(0, this.length);
};