/*! angular-detour - v0.3.1 - 2013-06-05
 * https://github.com/afterglowtech/angular-detour
 * Copyright (c) 2013 Stu Salsbury;
 *    Based on and uses software code found at https://github.com/angular-ui/ui-router which is 
 *    Copyright (c) 2013, Karsten Sperling
 * Licensed MIT
 */
(function () {
    define('common', [], function () {
        function Common() {
            this.isDefined = function (value) { return typeof value !== 'undefined'; };
            function isFunction(value) { return typeof value === 'function'; }
            this.isFunction = isFunction;

            this.isString = function (value) { return typeof value === 'string'; };
            this.isObject = function (value) { return value != null && typeof value === 'object'; };
            this.isArray = function (value) { return toString.apply(value) === '[object Array]'; };
            function isArrayLike(obj) {
                if (!obj || (typeof obj.length !== 'number')) {
                    return false;
                }

                // We have on object which has length property. Should we treat it as array?
                if (typeof obj.hasOwnProperty !== 'function' &&
                    typeof obj.constructor !== 'function') {
                    // This is here for IE8: it is a bogus object treat it as array;
                    return true;
                } else {
                    return typeof obj === 'JQLite' ||                      // JQLite
                           (typeof jQuery !== 'undefined' && typeof obj === 'jQuery') ||          // jQuery
                           toString.call(obj) !== '[object Object]' ||   // some browser native object
                           typeof obj.callee === 'function';              // arguments (on IE8 looks like regular obj)
                }
            }
            this.isArrayLike = isArrayLike;

            function forEach(obj, iterator, context) {
                var key;
                if (obj) {
                    if (isFunction(obj)) {
                        for (key in obj) {
                            if (key !== 'prototype' && key !== 'length' && key !== 'name' && obj.hasOwnProperty(key)) {
                                iterator.call(context, obj[key], key);
                            }
                        }
                    } else if (obj.forEach && obj.forEach !== forEach) {
                        obj.forEach(iterator, context);
                    } else if (isArrayLike(obj)) {
                        for (key = 0; key < obj.length; key++) {
                            iterator.call(context, obj[key], key);
                        }
                    } else {
                        for (key in obj) {
                            if (obj.hasOwnProperty(key)) {
                                iterator.call(context, obj[key], key);
                            }
                        }
                    }
                }
                return obj;
            }
            this.forEach = forEach;

            function setHashKey(obj, h) {
                if (h) {
                    obj.$$hashKey = h;
                }
                else {
                    delete obj.$$hashKey;
                }
            }
            this.setHashKey = setHashKey;

            function extend(dst) {
                var h = dst.$$hashKey;
                forEach(arguments, function (obj) {
                    if (obj !== dst) {
                        forEach(obj, function (value, key) {
                            dst[key] = value;
                        });
                    }
                });

                setHashKey(dst, h);
                return dst;
            }
            this.extend = extend;

            this.inherit = function (parent, extra) {
                return extend(new (extend(function () { }, { prototype: parent }))(), extra);
            };

            function deepMerge(target, src) {
                var array = Array.isArray(src);
                var dst = array && [] || {};

                if (array) {
                    target = target || [];
                    dst = dst.concat(target);
                    forEach(src, function (e, i) {
                        if (typeof e === 'object') {
                            dst[i] = deepMerge(target[i], e);
                        } else {
                            if (target.indexOf(e) === -1) {
                                dst.push(e);
                            }
                        }
                    });
                } else {
                    if (target && typeof target === 'object') {
                        forEach(Object.keys(target), function (key) {
                            dst[key] = target[key];
                        });
                    }
                    forEach(Object.keys(src), function (key) {
                        if (typeof src[key] !== 'object' || !src[key]) {
                            dst[key] = src[key];
                        }
                        else {
                            if (!target[key]) {
                                dst[key] = src[key];
                            } else {
                                dst[key] = deepMerge(target[key], src[key]);
                            }
                        }
                    });
                }

                return dst;
            }
            this.deepMerge = deepMerge;

            this.merge = function (dst) {
                forEach(arguments, function (obj) {
                    if (obj !== dst) {
                        forEach(obj, function (value, key) {
                            if (!dst.hasOwnProperty(key)) {
                                dst[key] = value;
                            }
                        });
                    }
                });
                return dst;
            };

        }

        return new Common();
    });

    define('UrlMatcher', ['common'], function (common) {
        /**
         * Matches URLs against patterns and extracts named parameters from the path or the search
         * part of the URL. A URL pattern consists of a path pattern, optionally followed by '?' and a list
         * of search parameters. Multiple search parameter names are separated by '&'. Search parameters
         * do not influence whether or not a URL is matched, but their values are passed through into
         * the matched parameters returned by {@link UrlMatcher#exec exec}.
         *
         * Path parameter placeholders can be specified using simple colon/catch-all syntax or curly brace
         * syntax, which optionally allows a regular expression for the parameter to be specified:
         *
         * * ':' name - colon placeholder
         * * '*' name - catch-all placeholder
         * * '{' name '}' - curly placeholder
         * * '{' name ':' regexp '}' - curly placeholder with regexp. Should the regexp itself contain
         *   curly braces, they must be in matched pairs or escaped with a backslash.
         *
         * Parameter names may contain only word characters (latin letters, digits, and underscore) and
         * must be unique within the pattern (across both path and search parameters). For colon
         * placeholders or curly placeholders without an explicit regexp, a path parameter matches any
         * number of characters other than '/'. For catch-all placeholders the path parameter matches
         * any number of characters.
         *
         * ### Examples
         *
         * * '/hello/' - Matches only if the path is exactly '/hello/'. There is no special treatment for
         *   trailing slashes, and patterns have to match the entire path, not just a prefix.
         * * '/user/:id' - Matches '/user/bob' or '/user/1234!!!' or even '/user/' but not '/user' or
         *   '/user/bob/details'. The second path segment will be captured as the parameter 'id'.
         * * '/user/{id}' - Same as the previous example, but using curly brace syntax.
         * * '/user/{id:[^/]*}' - Same as the previous example.
         * * '/user/{id:[0-9a-fA-F]{1,8}}' - Similar to the previous example, but only matches if the id
         *   parameter consists of 1 to 8 hex digits.
         * * '/files/{path:.*}' - Matches any URL starting with '/files/' and captures the rest of the
         *   path into the parameter 'path'.
         * * '/files/*path' - ditto.
         *
         * @constructor
         * @param {string} pattern  the pattern to compile into a matcher.
         *
         * @property {string} prefix  A static prefix of this pattern. The matcher guarantees that any
         *   URL matching this matcher (i.e. any string for which {@link UrlMatcher#exec exec()} returns
         *   non-null) will start with this prefix.
         */
        function UrlMatcher(pattern) {

            // Find all placeholders and create a compiled pattern, using either classic or curly syntax:
            //   '*' name
            //   ':' name
            //   '{' name '}'
            //   '{' name ':' regexp '}'
            // The regular expression is somewhat complicated due to the need to allow curly braces
            // inside the regular expression. The placeholder regexp breaks down as follows:
            //    ([:*])(\w+)               classic placeholder ($1 / $2)
            //    \{(\w+)(?:\:( ... ))?\}   curly brace placeholder ($3) with optional regexp ... ($4)
            //    (?: ... | ... | ... )+    the regexp consists of any number of atoms, an atom being either
            //    [^{}\\]+                  - anything other than curly braces or backslash
            //    \\.                       - a backslash escape
            //    \{(?:[^{}\\]+|\\.)*\}     - a matched set of curly braces containing other atoms
            var placeholder = /([:*])(\w+)|\{(\w+)(?:\:((?:[^{}\\]+|\\.|\{(?:[^{}\\]+|\\.)*\})+))?\}/g,
                names = {}, compiled = '^', last = 0, m,
                segments = this.segments = [],
                params = this.params = [];

            function addParameter(id) {
                if (!/^\w+$/.test(id)) {
                    throw new Error('Invalid parameter name \'' + id + '\' in pattern \'' + pattern + '\'');
                }
                if (names[id]) {
                    throw new Error('Duplicate parameter name \'' + id + '\' in pattern \'' + pattern + '\'');
                }
                names[id] = true;
                params.push(id);
            }

            function quoteRegExp(string) {
                return string.replace(/[\\\[\]\^$*+?.()|{}]/g, '\\$&');
            }

            this.source = pattern;

            // Split into static segments separated by path parameter placeholders.
            // The number of segments is always 1 more than the number of parameters.
            var id, regexp, segment;
            while ((m = placeholder.exec(pattern))) {
                id = m[2] || m[3]; // IE[78] returns '' for unmatched groups instead of null
                regexp = m[4] || (m[1] === '*' ? '.*' : '[^/]*');
                segment = pattern.substring(last, m.index);
                if (segment.indexOf('?') >= 0) {
                    break; // we're into the search part
                }
                compiled += quoteRegExp(segment) + '(' + regexp + ')';
                addParameter(id);
                segments.push(segment);
                last = placeholder.lastIndex;
            }
            segment = pattern.substring(last);

            // Find any search parameter names and remove them from the last segment
            var i = segment.indexOf('?');
            if (i >= 0) {
                var search = this.sourceSearch = segment.substring(i);
                segment = segment.substring(0, i);
                this.sourcePath = pattern.substring(0, last + i);

                // Allow parameters to be separated by '?' as well as '&' to make concat() easier
                common.forEach(search.substring(1).split(/[&?]/), addParameter);
            } else {
                this.sourcePath = pattern;
                this.sourceSearch = '';
            }

            compiled += quoteRegExp(segment) + '$';
            segments.push(segment);
            this.regexp = new RegExp(compiled);
            this.prefix = segments[0];
        }

        /**
         * Returns a new matcher for a pattern constructed by appending the path part and adding the
         * search parameters of the specified pattern to this pattern. The current pattern is not
         * modified. This can be understood as creating a pattern for URLs that are relative to (or
         * suffixes of) the current pattern.
         *
         * ### Example
         * The following two matchers are equivalent:
         * ```
         * new UrlMatcher('/user/{id}?q').concat('/details?date');
         * new UrlMatcher('/user/{id}/details?q&date');
         * ```
         *
         * @param {string} pattern  The pattern to append.
         * @return {UrlMatcher}  A matcher for the concatenated pattern.
         */
        UrlMatcher.prototype.concat = function (pattern) {
            // Because order of search parameters is irrelevant, we can add our own search
            // parameters to the end of the new pattern. Parse the new pattern by itself
            // and then join the bits together, but it's much easier to do this on a string level.
            return new UrlMatcher(this.sourcePath + pattern + this.sourceSearch);
        };

        UrlMatcher.prototype.toString = function () {
            return this.source;
        };

        /**
         * Tests the specified path against this matcher, and returns an object containing the captured
         * parameter values, or null if the path does not match. The returned object contains the values
         * of any search parameters that are mentioned in the pattern, but their value may be null if
         * they are not present in `searchParams`. This means that search parameters are always treated
         * as optional.
         *
         * ### Example
         * ```
         * new UrlMatcher('/user/{id}?q&r').exec('/user/bob', { x:'1', q:'hello' });
         * // returns { id:'bob', q:'hello', r:null }
         * ```
         *
         * @param {string} path  The URL path to match, e.g. `$location.path()`.
         * @param {Object} searchParams  URL search parameters, e.g. `$location.search()`.
         * @return {Object}  The captured parameter values.
         */
        UrlMatcher.prototype.exec = function (path, searchParams) {
            var m = this.regexp.exec(path);
            if (!m) {
                return null;
            }

            var params = this.params, nTotal = params.length,
              nPath = this.segments.length - 1,
              values = {}, i;

            for (i = 0; i < nPath; i++) {
                values[params[i]] = decodeURIComponent(m[i + 1]);
            }
            for (/**/; i < nTotal; i++) {
                values[params[i]] = searchParams[params[i]];
            }

            return values;
        };

        /**
         * Returns the names of all path and search parameters of this pattern in an unspecified order.
         * @return {Array.<string>}  An array of parameter names. Must be treated as read-only. If the
         *    pattern has no parameters, an empty array is returned.
         */
        UrlMatcher.prototype.parameters = function () {
            return this.params;
        };

        /**
         * Creates a URL that matches this pattern by substituting the specified values
         * for the path and search parameters. Null values for path parameters are
         * treated as empty strings.
         *
         * ### Example
         * ```
         * new UrlMatcher('/user/{id}?q').format({ id:'bob', q:'yes' });
         * // returns '/user/bob?q=yes'
         * ```
         *
         * @param {Object} values  the values to substitute for the parameters in this pattern.
         * @return {string}  the formatted URL (path and optionally search part).
         */
        UrlMatcher.prototype.format = function (values) {
            var segments = this.segments, params = this.params;
            if (!values) {
                return segments.join('');
            }

            var nPath = segments.length - 1, nTotal = params.length,
              result = segments[0], i, search, value;

            for (i = 0; i < nPath; i++) {
                value = values[params[i]];
                // TODO: Maybe we should throw on null here? It's not really good style to use '' and null interchangeabley
                if (value != null) {
                    result += value;
                }
                result += segments[i + 1];
            }
            for (/**/; i < nTotal; i++) {
                value = values[params[i]];
                if (value != null) {
                    result += (search ? '&' : '?') + params[i] + '=' + encodeURIComponent(value);
                    search = true;
                }
            }

            return result;
        };

        UrlMatcher.prototype.isMatcher = function (o) {
            return o instanceof UrlMatcher;
        };

        return UrlMatcher;
    });

    define('StateBase', ['./common', 'UrlMatcher'], function (common, UrlMatcher) {
        var abstractVar = 'abstract'
        ;

        function StateBase() {
            this.children = {};
        }


        //*********************************************
        // initialize
        //*********************************************
        Object.defineProperty(StateBase.prototype, 'self', {
            get: function () { return this; }
        });

        StateBase.prototype.resetAll = function () {
            this.resetFullName();
            this.resetUrl();
            this.resetParams();
            this.resetNavigable();
            this.resetPath();
            this.resetViews();
            this.resetIncludes();
            this.resetHandlers();
        };

        StateBase.prototype.initialize = function (forceInit) {
            if (this.needsInit || forceInit) {
                this.resetAll();

                for (var child in this.children) {
                    this.children[child].initialize(true);
                }
                this.needsInit = false;
            }
        };

        //*********************************************
        // name/fullName/localName
        //*********************************************
        Object.defineProperty(StateBase.prototype, 'fullName', {
            get: function () { return this._fullName; }
        });
        Object.defineProperty(StateBase.prototype, 'name', {
            get: function () { return this._fullName; }
        });
        Object.defineProperty(StateBase.prototype, 'localName', {
            get: function () { return this._localName; },
            set: function (val) {
                this.validateName(val);
                this._localName = val;
                this.needsInit = true;
            }
        });

        StateBase.prototype.resetFullName = function () {
            this._fullName = (this.parent.fullName)
              ? this.parent.fullName + '.' + this.localName
              : this.localName;
        };
        StateBase.prototype.toString = function () { return this.fullName; };

        StateBase.prototype.validateName = function (localName) {
            if (!common.isString(localName) || localName.indexOf('@') >= 0) {
                throw new Error('Invalid local state name (' + localName + ')');
            }

            // can't redefine if we throw this error here
            //not really useful, anyway
            // if (this.parent && this.parent.getChild(localName)) {
            //     throw new Error('State ' + parent.fullName  + ' already has child ' + localName);
            // }
        };

        //*********************************************
        // root
        //*********************************************
        Object.defineProperty(StateBase.prototype, 'root', {
            get: function () { return this.parent.root; }
        });

        //*********************************************
        // children
        //*********************************************
        Object.defineProperty(StateBase.prototype, 'children', {
            get: function () { return this._children; },
            set: function (val) {
                this._children = val;
                if (this._children) {
                    for (var child in this._children) {
                        //assigning their parent takes care of resetting the children
                        this._children[child].parent = this;
                    }
                }
            }
        });

        //*********************************************
        // path
        //*********************************************
        StateBase.prototype.resetPath = function () {
            // Keep a full path from the root down to this state as this is needed for state activation.
            this.path = this.parent.path.concat(this); // exclude root from path
        };

        //*********************************************
        // url
        //*********************************************
        Object.defineProperty(StateBase.prototype, 'url', {
            get: function () { return this._url; },
            set: function (val) {
                this._url = val;
                this.needsInit = true;
            }
        });
        Object.defineProperty(StateBase.prototype, 'aliases', {
            get: function () { return this._aliases; },
            set: function (val) {
                this._aliases = val;
                this.needsInit = true;
            }
        });


        StateBase.prototype.resetUrl = function () {
            /*jshint eqeqeq:false */
            this.preparedUrl = null;
            if (common.isString(this.url)) {
                if (this.url.charAt(0) === '^') {
                    this.preparedUrl = new UrlMatcher(this.url.substring(1));
                } else {
                    this.preparedUrl = (this.parent.navigable || this.root).preparedUrl.concat(this.url);
                }
            } else if (common.isObject(this.url) &&
                common.isFunction(this.url.exec) && common.isFunction(this.url.format) && common.isFunction(this.url.concat)) {
                this.preparedUrl = this.url;
                /* use UrlMatcher (or compatible object) as is */
            } else if (this.url != null) {
                throw new Error('Invalid url ' + this.url + ' in state ' + this);
            }

        };

        //*********************************************
        // params
        //*********************************************
        Object.defineProperty(StateBase.prototype, 'params', {
            get: function () { return this._params; },
            set: function (val) {
                this._params = val;
                this.needsInit = true;
            }
        });
        StateBase.prototype.resetParams = function () {
            // Derive parameters for this state and ensure they're a super-set of parent's parameters
            this.preparedParams = null;

            // Derive parameters for this state and ensure they're a super-set of parent's parameters
            var params = this.params;
            if (params) {
                if (!common.isArray(params)) {
                    throw new Error('Invalid params in state \'' + this + '\'');
                }
                else {
                    if (this.preparedUrl) {
                        throw new Error('Both params and url specicified in state \'' + this + '\'');
                    }
                    else {
                        this.perparedParams = params;
                    }
                }
            }
            else {
                this.preparedParams = this.preparedUrl ? this.preparedUrl.parameters() : this.parent.preparedParams;
            }

            var paramNames = {};
            common.forEach(this.preparedParams, function (p) {
                paramNames[p] = true;
            });
            if (this.parent) {
                var that = this;
                common.forEach(this.parent.preparedParams, function (p) {
                    if (!paramNames[p]) {
                        throw new Error('Missing required parameter \'' + p + '\' in state \'' + that.name + '\'');
                    }
                    paramNames[p] = false;
                });

                var ownParams = this.ownParams = [];
                common.forEach(paramNames, function (own, p) {
                    if (own) {
                        ownParams.push(p);
                    }
                });
            } else {
                this.ownParams = this.preparedParams;
            }
        };

        //*********************************************
        // navigable
        //*********************************************
        StateBase.prototype.resetNavigable = function () {
            this.navigable = (this.url)
              ? this
              : (this.parent)
                ? this.parent.navigable
                : null;
        };

        //*********************************************
        // abstract
        //*********************************************
        Object.defineProperty(StateBase.prototype, abstractVar, {
            get: function () { return this._abstract; },
            set: function (val) {
                this._abstract = val;
                this.needsInit = true;
            }
        });

        //*********************************************
        // includes
        //*********************************************
        StateBase.prototype.resetIncludes = function () {
            // Speed up $detour.contains() as it's used a lot
            this.includes = (this.parent)
              ? common.extend({}, this.parent.includes)
              : {};
            this.includes[this.name] = true;
        };

        //*********************************************
        // views
        //*********************************************
        Object.defineProperty(StateBase.prototype, 'views', {
            get: function () { return this._views; },
            set: function (val) {
                this._views = val;
                this.needsInit = true;
            }
        });
        StateBase.prototype.resetViews = function () {
            var state = this;
            // If there is no explicit multi-view configuration, make one up so we don't have
            // to handle both cases in the view directive later. Note that having an explicit
            // 'views' property will mean the default unnamed view properties are ignored. This
            // is also a good time to resolve view names to absolute names, so everything is a
            // straight lookup at link time.
            var views = {};
            var myViews = this.views;
            common.forEach(common.isDefined(myViews) ? myViews : { '': state }, function (view, name) {
                if (name.indexOf('@') < 0) {
                    name = name + '@' + state.parent.name;
                }
                views[name] = view;
            });
            this.preparedViews = views;
        };

        //this is somewhat particular to client-side
        //for now, separate -- but revisit this when
        //implementing incremental matching
        // //*********************************************
        // // handleUrl
        // //*********************************************
        // StateBase.prototype._buildRule = function(what, handler) {
        //   var rule, redirect;
        //   if (isString(what)) {
        //     what = new UrlMatcher(what);
        //   }

        //   if (what instanceof UrlMatcher) {
        //     if (isString(handler)) {
        //       redirect = new UrlMatcher(handler);
        //       handler = [matchSvc, function ($match) { return redirect.format($match); }];
        //     }
        //     else if (!isFunction(handler) && !isArray(handler)) {
        //       throw new Error('invalid \'handler\' in when()');
        //     }

        //     rule = function ($injector, $location) {
        //       return handleIfMatch($injector, handler, what.exec($location.path(), $location.search()));
        //     };
        //     rule.prefix = isString(what.prefix) ? what.prefix : '';
        //   }
        //   else if (what instanceof RegExp) {
        //     if (isString(handler)) {
        //       redirect = handler;
        //       handler = [matchSvc, function ($match) { return interpolate(redirect, $match); }];
        //     }
        //     else if (!isFunction(handler) && !isArray(handler)) {
        //       throw new Error('invalid \'handler\' in when()');
        //     }

        //     if (what.global || what.sticky) {
        //       throw new Error('when() RegExp must not be global or sticky');
        //     }

        //     rule = function ($injector, $location) {
        //       return handleIfMatch($injector, handler, what.exec($location.path()));
        //     };
        //     rule.prefix = regExpPrefix(what);
        //   }
        //   else {
        //     throw new Error('invalid \'what\' in when()');
        //   }
        //   return rule;
        // };

        StateBase.prototype.resetHandlers = function () {
            //this implementation is client-specific
            throw new Error('not implemented: "resetHandlers"');
            // if (this[abstractVar] || !this.preparedUrl) {
            //   this.handleUrl = null;
            //   return;
            // }

            // var what = this.preparedUrl;
            // var that = this;
            // var handler = [matchSvc, detourSvc, function ($match, $detour) {
            //   $detour.transitionTo(that, $match, false);
            // }];

            // this.handleUrl = this._buildRule(what, handler);

            // this.preparedAliases = [];
            // if (this.aliases) {
            //   common.forEach(this.aliases, function(value, key) {
            //     value = value.charAt(0) === '^'
            //       ? value.substring(1)
            //       : (that.parent.navigable || that.root).preparedUrl.concat(value).source;
            //     that.preparedAliases.push(that._buildRule(key, value));
            //   });
            // }

        };

        // StateBase.prototype.tryHandle = function($injector, $location) {
        //   //this implementation is client-specific
        //   throw new Error('not implemented: "resetHandlers"');
        //   var handled = false;

        //   if (this.handleUrl) {
        //     handled = this.handleUrl($injector, $location);
        //     for (var i = 0; i < this.preparedAliases.length && !handled; i++) {
        //       handled = this.preparedAliases[i]($injector, $location);
        //     }
        //   }

        //   if (!handled) {
        //     for (var child in this.children) {
        //       handled = this.children[child].tryHandle($injector, $location);
        //       if (handled) {
        //         break;
        //       }
        //     }
        //   }

        //   return handled;
        // };


        StateBase.prototype.newInstance = function () {
            return new StateBase();
        };

        //*********************************************
        // getChild
        //*********************************************
        StateBase.prototype.getChild = function (localName) {
            return (this.children)
              ? this.children[localName]
              : null;
        };


        //*********************************************
        // setChild
        //*********************************************
        //this redefines the child in place (i.e. doesn't wipe out its children)
        StateBase.prototype.setChild = function (stateDef, deep) {
            var state = this.newInstance();
            common.extend(state, stateDef);

            return this.setChildState(state, deep);
        };

        //*********************************************
        // removeChild
        //*********************************************
        //undefines the child (and any descendants of the child)
        StateBase.prototype.removeChild = function (localName) {
            if (this.children[localName]) {
                delete this.children[localName];
            }
            //this._needsInit = true;
            return this;
        };


        //*********************************************
        // setChildState
        //*********************************************
        //this redefines the child in place (i.e. doesn't wipe out its children)
        StateBase.prototype.setChildState = function (state, deep) {
            if (!deep) {
                var existingChild = this.getChild(state.localName);
                var existingChildren = (existingChild)
                  ? existingChild.children
                  : null;

                if (existingChildren) {
                    state._children = existingChildren;
                }

            }

            this.children[state.localName] = state;
            state.parent = this;
            this.needsInit = true;
            return state;
        };


        //*********************************************
        // updateChild
        //*********************************************
        //this updates properties of the child in place (i.e. doesn't wipe out its children)
        //nor does it start with a fresh state, so properties not overwritten are maintained
        //however, if no existing state, a new one is created
        StateBase.prototype.updateChild = function (stateDef) {
            var state = this.getChild(stateDef.localName);
            if (!state) {
                // deep doesn't really matter since this will be a new state, but
                // for form it's set to true
                return this.setChild(stateDef, true);
            }
            else {
                common.extend(state, stateDef);

                return this.setChildState(state, false);
            }
        };

        //*********************************************
        // prepareFlatDefinition
        //*********************************************
        StateBase.prototype.prepFlatGetParent = function (stateDef) {
            var parent, localName;
            if (stateDef.parent) {
                parent = this.getState(stateDef.parent);
                localName = stateDef.fullName;
            }
            else {
                var fullName = stateDef.fullName
                  ? stateDef.fullName
                  : stateDef.name;

                var parts = /^(.*?)\.?([^\.]*)$/.exec(fullName);

                var parentName = parts[1];
                localName = parts[2];

                parent = parentName
                  ? this.getState(parentName)
                  : this.root;
            }

            stateDef.localName = localName;

            delete stateDef.name;
            delete stateDef.fullName;
            delete stateDef.parent;

            return parent;
        };


        //*********************************************
        // setState
        //*********************************************
        //specify name/fullName in the definition to indicate
        //parent (which must already exist) -- for compatibility
        //with ui-router or other non-oo definition style
        StateBase.prototype.setState = function (stateDef, deep) {
            var parent = this.prepFlatGetParent(stateDef);

            return parent.setChild(stateDef, deep);
        };

        //*********************************************
        // updateState
        //*********************************************
        //specify name/fullName in the definition to indicate
        //parent (which must already exist) -- for compatibility
        //with ui-router or other non-oo definition style
        StateBase.prototype.updateState = function (stateDef) {
            var parent = this.prepFlatGetParent(stateDef);

            return parent.updateChild(stateDef);
        };

        //*********************************************
        // findState
        //*********************************************
        StateBase.prototype.findState = function (partialName) {
            var parts = /^([^\.]+)(\.(.*))?$/.exec(partialName);
            var firstPart = parts[1];
            if (this.localName === firstPart) {
                //first part matches this node
                //grab the rest of the name
                var rest = parts[3];
                if (rest) {
                    return this.findStateChildren(rest);
                }
                else {
                    //there is no 'rest' -- we've found the state
                    return this;
                }
            }
            else {
                //this is not a path for this partialName
                return null;
            }
        };

        //*********************************************
        // findStateChildren
        //*********************************************
        StateBase.prototype.findStateChildren = function (partialName) {
            if (this.children) {
                for (var child in this.children) {
                    var found = this.children[child].findState(partialName);
                    if (found) {
                        return found;
                    }
                }
            }
            //nothing was found
            return null;
        };

        //*********************************************
        // getState
        //*********************************************
        StateBase.prototype.getState = function (state) {
            if (!common.isString(state)) {
                return this.root.findStateChildren(state.fullName);
            }
            else {
                return this.root.findStateChildren(state);
            }
        };

        //*********************************************
        // JSON support
        //*********************************************
        StateBase.prototype.getIntJson = function (object, longPropertyName, shortPropertyName) {
            return object[shortPropertyName]
              ? parseInt(object[shortPropertyName], 10)
              : object[longPropertyName]
                ? parseInt(object[longPropertyName], 10)
                : null;
        };

        StateBase.prototype.getObjJson = function (object, longPropertyName, shortPropertyName) {
            return object[shortPropertyName]
              ? object[shortPropertyName]
              : object[longPropertyName]
                ? object[longPropertyName]
                : null;
        };

        StateBase.prototype.expandDefinition = function (definition) {
            this.expandJson(definition, 'url', 'u');
            this.expandJson(definition, 'dependencies', 'd');
            this.expandJson(definition, 'resolveByService', 'r');
            this.expandJson(definition, 'templateService', 'i');
            this.expandJson(definition, 'aliases', 's');
            this.expandJson(definition, 'controller', 'c');
            this.expandJson(definition, 'templateUrl', 't');
            this.expandJson(definition, 'template', 'l');
            this.expandJson(definition, 'data', 'a');
            this.expandJson(definition, 'abstract', 'b');
            this.expandJson(definition, 'views', 'v');
        };

        StateBase.prototype.expandView = function (view) {
            this.expandJson(view, 'url', 'u');
            this.expandJson(view, 'resolveByService', 'r');
            this.expandJson(view, 'templateService', 'i');
            this.expandJson(view, 'controller', 'c');
            this.expandJson(view, 'templateUrl', 't');
            this.expandJson(view, 'template', 'l');
            this.expandJson(view, 'data', 'a');
        };

        StateBase.prototype.expandJson = function (object, longPropertyName, shortPropertyName) {
            if (object[shortPropertyName]) {
                object[longPropertyName] = object[shortPropertyName];
                delete object[shortPropertyName];
            }
        };

        StateBase.prototype.mergeChild = function (name, childJson) {
            //the name of the child we're working with
            var del = this.getObjJson(childJson, 'delete', 'x');
            if (del) {
                this.removeChild(name);
            }
            else {
                var definition = this.getObjJson(childJson, 'definition', 'd');
                if (definition) {
                    //a definition is specified -- update child
                    definition.localName = name;
                    this.expandDefinition(definition);
                    if (definition.views) {
                        for (var viewName in definition.views) {
                            var view = definition.views[viewName];
                            this.expandView(view);
                        }
                    }

                    this.updateChild(definition);
                }

                var children = this.getObjJson(childJson, 'children', 'c');
                if (children) {
                    var thisChild = this.getChild(name);
                    for (var grandchildName in children) {
                        var grandChild = children[grandchildName];
                        thisChild.mergeChild(grandchildName, grandChild);
                    }
                }
            }

            return true;
        };

        Object.defineProperty(StateBase.prototype, 'knownStates', {
            get: function () {
                var summary = {};

                if (Object.keys(this.children).length > 0) {
                    var children = {};
                    for (var childName in this.children) {
                        var child = this.children[childName];
                        children[child.localName] = child.knownStates;
                    }
                    summary = children;
                }

                return summary;
            }
        });

        return StateBase;

    });

    /*! angular-couchPotato - v0.0.4 - 2013-05-29
     * https://github.com/afterglowtech/angular-couchPotato
     * Copyright (c) 2013 Stu Salsbury;
     *    Uses software code found at https://github.com/szhanginrhythm/angular-require-lazyload
     * Licensed MIT
     */
    (function () { function e(e, r, n, t) { function i(e) { n.value.apply(null, e) } function o(e) { n.factory.apply(null, e) } function l(e) { t.register.apply(null, e) } function c(e) { r.directive.apply(null, e) } function u(r) { e.register.apply(null, r) } function s(e) { function r(r, n) { var t = r.defer(); return require(e, function () { t.resolve(), n.$apply() }), t.promise } return r.$inject = ["$q", "$rootScope"], r } function p(e) { if (e.dependencies) { var r = e, n = e.dependencies; return delete r.dependencies, r.resolve = {}, r.resolve.delay = s(n), r } return e } this.resolveDependencies = s, this.resolveDependenciesProperty = p, this.$get = function () { var e = {}; return e.registerValue = i, e.registerFactory = o, e.registerFilter = l, e.registerDirective = c, e.registerController = u, e.resolveDependenciesProperty = p, e.resolveDependencies = s, e } } var r = angular.module("agt.couchPotato", ["ng"]); e.$inject = ["$controllerProvider", "$compileProvider", "$provide", "$filterProvider"], r.provider("$couchPotato", e) })();
    define("couchPotato", function () { });

    define('detourModule', ['couchPotato'], function () {

        return angular.module('agt.detour', ['agt.couchPotato']);
    });

    define('templateFactory', ['./detourModule'], function () {
        /**
         * Service. Manages loading of templates.
         * @constructor
         * @name $templateFactory
         * @requires $http
         * @requires $templateCache
         * @requires $injector
         */
        function $TemplateFactory($http, $templateCache, $injector) {

            /**
             * Creates a template from a configuration object.
             * @function
             * @name $templateFactory#fromConfig
             * @methodOf $templateFactory
             * @param {Object} config  Configuration object for which to load a template. The following
             *    properties are search in the specified order, and the first one that is defined is
             *    used to create the template:
             * @param {string|Function} config.template  html string template or function to load via
             *    {@link $templateFactory#fromString fromString}.
             * @param {string|Function} config.templateUrl  url to load or a function returning the url
             *    to load via {@link $templateFactory#fromUrl fromUrl}.
             * @param {Function} config.templateProvider  function to invoke via
             *    {@link $templateFactory#fromProvider fromProvider}.
             * @param {Object} params  Parameters to pass to the template function.
             * @param {Object} [locals] Locals to pass to `invoke` if the template is loaded via a
             *      `templateProvider`. Defaults to `{ params: params }`.
             * @return {string|Promise.<string>}  The template html as a string, or a promise for that string,
             *      or `null` if no template is configured.
             */
            this.fromConfig = function (config, params, locals) {
                return (
                  angular.isDefined(config.template) ? this.fromString(config.template, params) :
                  angular.isDefined(config.templateUrl) ? this.fromUrl(config.templateUrl, params) :
                  angular.isDefined(config.templateProvider) ? this.fromProvider(config.templateProvider, params, locals) :
                  angular.isDefined(config.templateService) ? this.fromService(config.templateService, params, locals) :
                  null
                );
            };

            /**
             * Creates a template from a string or a function returning a string.
             * @function
             * @name $templateFactory#fromString
             * @methodOf $templateFactory
             * @param {string|Function} template  html template as a string or function that returns an html
             *      template as a string.
             * @param {Object} params  Parameters to pass to the template function.
             * @return {string|Promise.<string>}  The template html as a string, or a promise for that string.
             */
            this.fromString = function (template, params) {
                return angular.isFunction(template) ? template(params) : template;
            };

            /**
             * Loads a template from the a URL via `$http` and `$templateCache`.
             * @function
             * @name $templateFactory#fromUrl
             * @methodOf $templateFactory
             * @param {string|Function} url  url of the template to load, or a function that returns a url.
             * @param {Object} params  Parameters to pass to the url function.
             * @return {string|Promise.<string>}  The template html as a string, or a promise for that string.
             */
            this.fromUrl = function (url, params) {
                if (angular.isFunction(url)) {
                    url = url(params);
                }
                if (url == null) {
                    return null;
                }
                else {
                    return $http
                      .get(url, { cache: $templateCache })
                      .then(function (response) { return response.data; });
                }
            };

            /**
             * Creates a template by invoking an injectable provider function.
             * @function
             * @name $templateFactory#fromProvider
             * @methodOf $templateFactory
             * @param {Function} provider Function to invoke via `$injector.invoke`
             * @param {Object} params Parameters for the template.
             * @param {Object} [locals] Locals to pass to `invoke`. Defaults to `{ params: params }`.
             * @return {string|Promise.<string>} The template html as a string, or a promise for that string.
             */
            this.fromProvider = function (provider, params, locals) {
                return $injector.invoke(provider, null, locals || { params: params });
            };

            /**
             * Creates a template by invoking a service.
             * @function
             * @name $templateFactory#fromService
             * @methodOf $templateFactory
             * @param {Function} serviceName Service to invoke via `$injector.invoke`
             * @param {Object} params Parameters for the template.
             * @param {Object} [locals] Locals to pass to `invoke`. Defaults to `{ params: params }`.
             * @return {string|Promise.<string>} The template html as a string, or a promise for that string.
             */
            this.fromService = function (serviceName, params, locals) {
                return $injector.invoke([serviceName, function (service) { return service.getTemplate(params, locals); }]);
            };

        }
        $TemplateFactory.$inject = ['$http', '$templateCache', '$injector'];

        angular.module('agt.detour').service('$templateFactory', $TemplateFactory);

    });

    define('detourProvider', ['./common', 'UrlMatcher', 'StateBase', 'couchPotato', 'templateFactory', './detourModule'], function (common, UrlMatcher, StateBase) {
        var matchSvc = '$match'
            , abstractVar = 'abstract'
            , detourSvc = '$detour'
        ;

        function $DetourProvider(
        ) {
            var that = this;

            function State() {
                this.children = {};
            }

            State.prototype = new StateBase();
            State.prototype.constructor = State;

            State.prototype.newInstance = function () {
                return new State();
            };

            //*********************************************
            // handleUrl
            //*********************************************
            State.prototype._buildRule = function (what, handler) {
                var rule, redirect;
                if (common.isString(what)) {
                    what = new UrlMatcher(what);
                }

                if (what instanceof UrlMatcher) {
                    if (common.isString(handler)) {
                        redirect = new UrlMatcher(handler);
                        handler = [matchSvc, function ($match) { return redirect.format($match); }];
                    }
                    else if (!common.isFunction(handler) && !common.isArray(handler)) {
                        throw new Error('invalid \'handler\' in when()');
                    }

                    rule = function ($injector, $location) {
                        return handleIfMatch($injector, handler, what.exec($location.path(), $location.search()));
                    };
                    rule.prefix = common.isString(what.prefix) ? what.prefix : '';
                }
                else if (what instanceof RegExp) {
                    if (common.isString(handler)) {
                        redirect = handler;
                        handler = [matchSvc, function ($match) { return interpolate(redirect, $match); }];
                    }
                    else if (!common.isFunction(handler) && !common.isArray(handler)) {
                        throw new Error('invalid \'handler\' in when()');
                    }

                    if (what.global || what.sticky) {
                        throw new Error('when() RegExp must not be global or sticky');
                    }

                    rule = function ($injector, $location) {
                        return handleIfMatch($injector, handler, what.exec($location.path()));
                    };
                    rule.prefix = regExpPrefix(what);
                }
                else {
                    throw new Error('invalid \'what\' in when()');
                }
                return rule;
            };

            State.prototype.resetHandlers = function () {
                if (this[abstractVar] || !this.preparedUrl) {
                    this.handleUrl = null;
                    return;
                }

                var what = this.preparedUrl;
                var that = this;
                var handler = [matchSvc, detourSvc, function ($match, $detour) {
                    $detour.transitionTo(that, $match, false);
                }];

                this.handleUrl = this._buildRule(what, handler);

                this.preparedAliases = [];
                if (this.aliases) {
                    common.forEach(this.aliases, function (value, key) {
                        value = value.charAt(0) === '^'
                          ? value.substring(1)
                          : (that.parent.navigable || that.root).preparedUrl.concat(value).source;
                        that.preparedAliases.push(that._buildRule(key, value));
                    });
                }

            };

            State.prototype.tryHandle = function ($injector, $location) {
                var handled = false;

                if (this.handleUrl) {
                    handled = this.handleUrl($injector, $location);
                    for (var i = 0; i < this.preparedAliases.length && !handled; i++) {
                        handled = this.preparedAliases[i]($injector, $location);
                    }
                }

                if (!handled) {
                    for (var child in this.children) {
                        handled = this.children[child].tryHandle($injector, $location);
                        if (handled) {
                            break;
                        }
                    }
                }

                return handled;
            };


            //*********************************************
            //*********************************************
            // StatesTree
            //*********************************************
            //*********************************************
            function StatesTree() {
                this.locals = { globals: { $stateParams: {} } };
                this.serial = 0;
                this.resetAll();
            }

            StatesTree.prototype = new State();
            StatesTree.prototype.constructor = StatesTree;

            StatesTree.prototype.initialize = function () {
                //no need to initialize the root/tree itself
                for (var child in this.children) {
                    //don't force initialization -- children
                    //that need to initialize will do so
                    //and will force deep initialization of their
                    //own children
                    this.children[child].initialize(false);
                }
                this.needsInit = false;
            };

            StatesTree.prototype.tryHandle = function ($injector, $location, doFallback) {
                var that = this;
                var handled = State.prototype.tryHandle.call(that, $injector, $location);
                if (!handled && doFallback) {
                    if (this.fallback) {
                        handled = this.fallback($injector, $location);
                    }
                }

                return handled;
            };

            Object.defineProperty(StatesTree.prototype, 'fallback', {
                get: function () { return this._fallback; },
                set: function (val) {
                    if (common.isString(val)) {
                        var redirect = val;
                        if (redirect) {
                            this._fallback = function () { return redirect; };
                        }
                        else {
                            this._fallback = null;
                        }
                    }
                    else if (!isFunction(rule)) {
                        throw new Error('\'rule\' must be a function');
                    }
                }
            });

            StatesTree.prototype.resetFullName = function () { };
            StatesTree.prototype.resetPath = function () { };

            Object.defineProperty(StatesTree.prototype, 'name', {
                get: function () { return ''; }
            });
            Object.defineProperty(StatesTree.prototype, 'localName', {
                get: function () { return ''; }
            });
            Object.defineProperty(StatesTree.prototype, 'url', {
                get: function () { return '^'; }
            });
            Object.defineProperty(StatesTree.prototype, 'views', {
                get: function () { return {}; }
            });
            Object.defineProperty(StatesTree.prototype, 'abstract', {
                get: function () { return true; }
            });
            Object.defineProperty(StatesTree.prototype, 'path', {
                get: function () { return []; }
            });
            Object.defineProperty(StatesTree.prototype, 'root', {
                get: function () { return this; }
            });
            Object.defineProperty(StatesTree.prototype, 'isRoot', {
                get: function () { return true; }
            });
            Object.defineProperty(StatesTree.prototype, 'fullName', {
                get: function () { return null; }
            });
            Object.defineProperty(StatesTree.prototype, 'navigable', {
                get: function () { return null; },
                set: function (val) { }
            });

            StatesTree.prototype.mergeJson = function (json) {
                var serial = this.getIntJson(json, 'serial', 's');

                if (serial && serial <= this.serial) {
                    //this update is specifically old
                    //if serial had been omitted we'd assume that it's not being used
                    return false;
                }
                else {
                    var tree = this.getObjJson(json, 'tree', 't');

                    if (tree) {
                        for (var childName in tree) {
                            var child = tree[childName];
                            this.mergeChild(childName, child);
                        }
                    }

                    var fallback = this.getObjJson(json, 'fallback', 'f');
                    if (fallback) {
                        this.fallback = fallback;
                    }

                    this.serial = serial;

                    this.initialize();

                    return true;
                }
            };

            Object.defineProperty(StatesTree.prototype, 'knownStates', {
                get: function () {
                    var summary = {
                        s: this.serial,
                        f: this.fallback
                    };

                    var tree = {};
                    for (var childName in this.children) {
                        var child = this.children[childName];
                        tree[childName] = child.knownStates;
                    }

                    summary.t = tree;

                    return summary;
                }
            });



            //***************************************
            //***************************************
            //URLPROVIDER
            //***************************************
            //***************************************
            // Returns a string that is a prefix of all strings matching the RegExp
            function regExpPrefix(re) {
                var prefix = /^\^((?:\\[^a-zA-Z0-9]|[^\\\[\]\^$*+?.()|{}]+)*)/.exec(re.source);
                return (prefix !== null) ? prefix[1].replace(/\\(.)/g, '$1') : '';
            }

            // Interpolates matched values into a String.replace()-style pattern
            function interpolate(pattern, match) {
                return pattern.replace(/\$(\$|\d{1,2})/, function (m, what) {
                    return match[what === '$' ? 0 : Number(what)];
                });
            }

            function handleIfMatch($injector, handler, match) {
                if (!match) {
                    return false;
                }
                var result = $injector.invoke(handler, handler, { $match: match });
                return common.isDefined(result) ? result : true;
            }
            //***************************************
            //(end URLPROVIDER)
            //***************************************

            //***************************************
            //set up the state tree
            //***************************************
            var statesTree = new StatesTree();

            //***************************************
            //provider api for ui-router stateProvider compatibility
            //***************************************
            function state(name, definition) {
                definition.fullName = name;
                statesTree.setState(definition);
                return this;
            }
            this.state = state;

            //***************************************
            //otherwise from urlRouterProvider
            //***************************************
            function otherwise(rule) {
                statesTree.fallback = rule;
                return this;
            }
            this.otherwise = otherwise;

            function initialize() {
                statesTree.initialize();
            }
            this.initialize = initialize;

            function removeState(fullName) {
                var state = this.getState(fullName);
                if (state) {
                    state.parent.removeChild(state.localName);
                }
                return this;
            }
            this.removeState = removeState;

            function setState(stateDef) {
                return statesTree.setState(stateDef);
            }
            this.setState = setState;

            function updateState(stateDef) {
                return statesTree.updateState(stateDef);
            }
            this.updateState = updateState;

            function getState(state) {
                return statesTree.getState(state);
            }
            this.getState = getState;

            function mergeJson(json) {
                return statesTree.mergeJson(json);
            }
            this.mergeJson = mergeJson;

            //default: lazy not enabled, autoUpdate not enabled (period 0)
            var _loader = {
                lazy: {
                    enabled: false,
                    routeUrl: null,
                    stateUrl: null,
                    routeParameter: 'r',
                    stateParameter: 's'
                },
                httpMethod: 'GET',
                knownStatesParameter: 'k',
                updatesUrl: null,
                crossDomain: false,
                autoUpdateMinutes: 0
            };

            Object.defineProperty(this, 'loader', {
                get: function () { return _loader; },
                set: function (val) {
                    _loader = common.deepMerge(_loader, val);
                }
            });


            //***************************************
            //service definition
            //***************************************
            function $get($rootScope, $q, $templateFactory, $injector, $stateParams, $location, $couchPotato, $http) {

                var TransitionSuperseded = $q.reject(new Error('transition superseded'));
                var TransitionPrevented = $q.reject(new Error('transition prevented'));

                var loader = that.loader;

                var $detour = {
                    params: {},
                    current: statesTree.self,
                    $current: statesTree,
                    stateLoader: loader,
                    transition: null
                };

                $detour.registerValue = $couchPotato.registerValue;
                $detour.registerFactory = $couchPotato.registerFactory;
                $detour.registerFilter = $couchPotato.registerFilter;
                $detour.registerDirective = $couchPotato.registerDirective;
                $detour.registerController = $couchPotato.registerController;


                $detour.state = state;
                $detour.otherwise = otherwise;
                $detour.initialize = initialize;
                $detour.setState = setState;
                $detour.updateState = updateState;
                $detour.getState = getState;
                $detour.removeState = removeState;

                $detour.mergeJson = mergeJson;

                var httpConfig = $http.defaults;

                function lazyDoGet(requestUrl) {
                    var deferred = $q.defer();

                    var config = null;
                    if (loader.crossDomain) {
                        config = angular.copy(httpConfig, config);
                        delete config.headers.common['X-Requested-With'];
                        config.useXDomain = true;
                    }
                    else {
                        config = httpConfig;
                    }

                    $http({ method: loader.httpMethod, url: requestUrl, config: config }).
                      success(function (data, status, headers, config) {
                          deferred.resolve(angular.fromJson(data));
                      }).
                      error(function (data, status, headers, config) {
                          deferred.resolve(null);
                      });

                    return deferred.promise;
                }

                function getLazyRoute(route) {
                    var requestUrl = loader.lazy.routeUrl
                      + '?' + loader.lazy.routeParameter + '=' + encodeURIComponent(route)
                      + '&' + loader.knownStatesParameter + '=' + encodeURIComponent(angular.toJson(statesTree.knownStates));

                    var deferred = $q.defer();
                    lazyDoGet(requestUrl).then(
                      function (json) {
                          if (json) {
                              statesTree.mergeJson(json);
                          }
                          deferred.resolve();
                      }
                    );

                    return deferred.promise;
                }

                function getLazyState(stateName) {
                    var requestUrl = loader.lazy.stateUrl
                      + '?' + loader.lazy.stateParameter + '=' + encodeURIComponent(stateName)
                      + '&' + loader.knownStatesParameter + '=' + encodeURIComponent(angular.toJson(statesTree.knownStates));

                    var deferred = $q.defer();
                    lazyDoGet(requestUrl).then(
                      function (json) {
                          if (json) {
                              statesTree.mergeJson(json);
                          }
                          deferred.resolve();
                      }
                    );

                    return deferred.promise;
                }


                //***************************************
                //transitionTo
                //***************************************
                function transitionTo(to, toParams, updateLocation, secondTry) {
                    var toState = statesTree.getState(to);
                    if (!toState && loader.lazy.enabled && !secondTry) {
                        return getLazyState(to).then(function () {
                            return transitionTo(to, toParams, updateLocation, true);
                        });
                    }
                    else {
                        to = toState;

                        if (!common.isDefined(updateLocation)) {
                            updateLocation = true;
                        }

                        if (to['abstract']) {
                            throw new Error('Cannot transition to abstract state \'' + to + '\'');
                        }
                        var toPath = to.path,
                            from = $detour.$current, fromParams = $detour.params, fromPath = from.path;

                        // Starting from the root of the path, keep all levels that haven't changed
                        var keep, state, locals = statesTree.locals, toLocals = [];
                        for (keep = 0, state = toPath[keep];
                             state && state === fromPath[keep] && equalForKeys(toParams, fromParams, state.ownParams) ;
                             keep++, state = toPath[keep]) {
                            locals = toLocals[keep] = state.locals;
                        }

                        // If we're going to the same state and all locals are kept, we've got nothing to do.
                        // But clear 'transition', as we still want to cancel any other pending transitions.
                        // TODO: We may not want to bump 'transition' if we're called from a location change that we've initiated ourselves,
                        // because we might accidentally abort a legitimate transition initiated from code?
                        if (to === from && locals === from.locals) {
                            $detour.transition = null;
                            return $q.when($detour.current);
                        }

                        // Normalize/filter parameters before we pass them to event handlers etc.
                        toParams = normalize(to.preparedParams, toParams || {});

                        // Broadcast start event and cancel the transition if requested
                        if ($rootScope.$broadcast('$stateChangeStart', to.self, toParams, from.self, fromParams).defaultPrevented) {
                            return TransitionPrevented;
                        }

                        // Resolve locals for the remaining states, but don't update any global state just
                        // yet -- if anything fails to resolve the current state needs to remain untouched.
                        // We also set up an inheritance chain for the locals here. This allows the view directive
                        // to quickly look up the correct definition for each view in the current state. Even
                        // though we create the locals object itself outside resolveState(), it is initially
                        // empty and gets filled asynchronously. We need to keep track of the promise for the
                        // (fully resolved) current locals, and pass this down the chain.
                        var resolved = $q.when(locals);
                        for (var l = keep; l < toPath.length; l++, state = toPath[l]) {
                            locals = toLocals[l] = common.inherit(locals);
                            resolved = resolveState(state, toParams, state === to, resolved, locals);
                        }


                        // Once everything is resolved, we are ready to perform the actual transition
                        // and return a promise for the new state. We also keep track of what the
                        // current promise is, so that we can detect overlapping transitions and
                        // keep only the outcome of the last transition.
                        var transition = $detour.transition = resolved.then(function () {
                            var l, entering, exiting;

                            if ($detour.transition !== transition) {
                                return TransitionSuperseded;
                            }

                            // Exit 'from' states not kept
                            for (l = fromPath.length - 1; l >= keep; l--) {
                                exiting = fromPath[l];
                                if (exiting.self.onExit) {
                                    $injector.invoke(exiting.self.onExit, exiting.self, exiting.locals.globals);
                                }
                                exiting.locals = null;
                            }

                            // Enter 'to' states not kept
                            for (l = keep; l < toPath.length; l++) {
                                entering = toPath[l];
                                entering.locals = toLocals[l];
                                if (entering.self.onEnter) {
                                    $injector.invoke(entering.self.onEnter, entering.self, entering.locals.globals);
                                }
                            }

                            // Update globals in $detour
                            $detour.$current = to;
                            $detour.current = to.self;
                            $detour.params = toParams;
                            angular.copy($detour.params, $stateParams);
                            $detour.transition = null;

                            // Update $location
                            var toNav = to.navigable;
                            if (updateLocation && toNav) {
                                $location.url(toNav.preparedUrl.format(toNav.locals.globals.$stateParams));
                            }

                            $rootScope.$broadcast('$stateChangeSuccess', to.self, toParams, from.self, fromParams);

                            return $detour.current;
                        }, function (error) {
                            if ($detour.transition !== transition) {
                                return TransitionSuperseded;
                            }

                            $detour.transition = null;
                            $rootScope.$broadcast('$stateChangeError', to.self, toParams, from.self, fromParams, error);

                            return $q.reject(error);
                        });

                        return transition;
                    }

                }
                $detour.transitionTo = transitionTo;

                $detour.is = function (stateOrName) {
                    return $detour.$current === statesTree.getState(stateOrName);
                };

                $detour.includes = function (stateOrName) {
                    var state = statesTree.getState(stateOrName);
                    if (state) {
                        return $detour.$current.includes[state.name];
                    }
                    else {
                        return false;
                    }
                };

                $detour.href = function (stateOrName, params) {
                    var state = this.getState(stateOrName), nav = state.navigable;
                    if (!nav) {
                        throw new Error('State \'' + state + '\' is not navigable');
                    }
                    return nav.url.format(normalize(state.preparedParams, params || {}));
                };

                function resolveState(state, params, paramsAreFiltered, inherited, dst) {
                    var dependencies = [];
                    if (state.dependencies) {
                        dependencies.push(
                          $q.when(
                            $injector.invoke(
                              $couchPotato.resolveDependencies(state.dependencies)
                            )
                          )
                        );
                    }

                    return $q.all(dependencies).then(function () {

                        // We need to track all the promises generated during the resolution process.
                        // The first of these is for the fully resolved parent locals.
                        var promises = [inherited];

                        // Make a restricted $stateParams with only the parameters that apply to this state if
                        // necessary. In addition to being available to the controller and onEnter/onExit callbacks,
                        // we also need $stateParams to be available for any $injector calls we make during the
                        // dependency resolution process.
                        var $stateParams;
                        if (paramsAreFiltered) {
                            $stateParams = params;
                        }
                        else {
                            $stateParams = {};
                            common.forEach(state.preparedParams, function (name) {
                                $stateParams[name] = params[name];
                            });
                        }
                        var locals = { $stateParams: $stateParams };

                        // Resolves the values from an individual 'resolve' dependency spec
                        function resolve(deps, dst) {
                            common.forEach(deps, function (value, key) {
                                promises.push($q
                                  .when(common.isString(value) ?
                                      $injector.get(value) :
                                      $injector.invoke(value, state.self, locals))
                                  .then(function (result) {
                                      dst[key] = result;
                                  }));
                            });
                        }

                        // Resolves the values from an individual 'resolveServices' dependency spec
                        function resolveServices(deps, dst) {
                            common.forEach(deps, function (value, key) {
                                promises.push($q
                                  .when(
                                    $injector.invoke([value, function (service) { return service.resolve($stateParams, locals); }])
                                  )
                                  .then(function (result) {
                                      dst[key] = result;
                                  }));
                            });
                        }

                        // Resolve 'global' dependencies for the state, i.e. those not specific to a view.
                        // We're also including $stateParams in this; that we're the parameters are restricted
                        // to the set that should be visible to the state, and are independent of when we update
                        // the global $detour and $stateParams values.
                        var globals = dst.globals = { $stateParams: $stateParams };
                        resolve(state.resolve, globals);
                        resolveServices(state.resolveByService, globals);
                        globals.$$state = state; // Provide access to the state itself for internal use


                        // Resolve template and dependencies for all views.
                        common.forEach(state.preparedViews, function (view, name) {
                            // References to the controller (only instantiated at link time)
                            var $view = dst[name] = {
                                $$controller: view.controller
                            };

                            // Template
                            promises.push($q
                              .when($templateFactory.fromConfig(view, $stateParams, locals) || '')
                              .then(function (result) {
                                  $view.$template = result;
                              }));

                            // View-local dependencies. If we've reused the state definition as the default
                            // view definition in .state(), we can end up with state.resolve === view.resolve.
                            // Avoid resolving everything twice in that case.
                            if (view.resolve !== state.resolve) {
                                resolve(view.resolve, $view);
                            }
                        });


                        // Once we've resolved all the dependencies for this state, merge
                        // in any inherited dependencies, and merge common state dependencies
                        // into the dependency set for each view. Finally return a promise
                        // for the fully popuplated state dependencies.
                        return $q.all(promises).then(function (values) {
                            common.merge(dst.globals, values[0].globals); // promises[0] === inherited
                            common.forEach(state.preparedViews, function (view, name) {
                                common.merge(dst[name], dst.globals);
                            });
                            return dst;
                        });
                    });
                }

                function normalize(keys, values) {
                    /*jshint eqeqeq:false */
                    var normalized = {};

                    common.forEach(keys, function (name) {
                        var value = values[name];
                        normalized[name] = (value != null) ? String(value) : null;
                    });
                    return normalized;
                }

                function equalForKeys(a, b, keys) {
                    /*jshint eqeqeq:false */
                    for (var i = 0; i < keys.length; i++) {
                        var k = keys[i];
                        if (a[k] != b[k]) {
                            return false; // Not '===', values aren't necessarily normalized
                        }
                    }
                    return true;
                }

                //***************************************
                //URLPROVIDER
                //***************************************

                // TODO: Optimize groups of rules with non-empty prefix into some sort of decision tree
                function update(event, next, current, secondTry) {
                    var doFallback = !loader.lazy.enabled || secondTry;

                    var handled = statesTree.tryHandle($injector, $location, doFallback);
                    if (handled) {
                        if (common.isString(handled)) {
                            $location.replace().url(handled);
                        }
                    }

                    if (!handled && !secondTry && loader.lazy.enabled) {
                        getLazyRoute($location.path()).then(function () {
                            update(event, next, current, true);
                        });
                    }
                }

                $rootScope.$on('$locationChangeSuccess', update);


                //***************************************
                //(end URLPROVIDER)
                //***************************************

                return $detour;
            }
            $get.$inject = ['$rootScope', '$q', '$templateFactory', '$injector', '$stateParams', '$location', '$couchPotato', '$http'];
            this.$get = $get;

        }
        //  $DetourProvider.$inject = ['$urlMatcherFactoryProvider'];

        angular.module('agt.detour')
          .value('$stateParams', {})
          .provider(detourSvc, $DetourProvider);
    });

    define('viewDirective', ['./common', './detourModule', './detourProvider'], function (common) {
        function $ViewDirective($detour, $compile, $controller, $injector, $anchorScroll) {
            // Unfortunately there is no neat way to ask $injector if a service exists
            var $animator; try { $animator = $injector.get('$animator'); } catch (e) { /* do nothing */ }

            var directive = {
                restrict: 'ECA',
                terminal: true,
                link: function (scope, element, attr) {
                    var viewScope, viewLocals,
                        initialContent = element.contents(),
                        name = attr[directive.name] || attr.name || '',
                        onloadExp = attr.onload || '',
                        animate = common.isDefined($animator) && $animator(scope, attr);

                    function updateView(doAnimate) {
                        var locals = $detour.$current && $detour.$current.locals[name];
                        if (locals === viewLocals) {
                            return; // nothing to do
                        }

                        // Remove existing content
                        if (animate && doAnimate) {
                            animate.leave(element.contents(), element);
                        } else {
                            element.html('');
                        }

                        // Destroy previous view scope
                        if (viewScope) {
                            viewScope.$destroy();
                            viewScope = null;
                        }

                        if (locals) {
                            viewLocals = locals;
                            view.state = locals.$$state;

                            var contents;
                            if (animate && doAnimate) {
                                contents = angular.element('<div></div>').html(locals.$template).contents();
                                animate.enter(contents, element);
                            } else {
                                element.html(locals.$template);
                                contents = element.contents();
                            }

                            var link = $compile(contents);
                            viewScope = scope.$new();
                            if (locals.$$controller) {
                                locals.$scope = viewScope;
                                var controller = $controller(locals.$$controller, locals);
                                element.children().data('$ngControllerController', controller);
                            }
                            link(viewScope);
                            viewScope.$emit('$viewContentLoaded');
                            viewScope.$eval(onloadExp);

                            // TODO: This seems strange, shouldn't $anchorScroll listen for $viewContentLoaded if necessary?
                            // $anchorScroll might listen on event...
                            $anchorScroll();
                        } else {
                            viewLocals = null;
                            view.state = null;

                            // Restore initial view
                            if (doAnimate) {
                                animate.enter(initialContent, element);
                            } else {
                                element.html(initialContent);
                            }
                        }
                    }

                    // Find the details of the parent view directive (if any) and use it
                    // to derive our own qualified view name, then hang our own details
                    // off the DOM so child directives can find it.
                    var parent = element.parent().inheritedData('$uiView');
                    if (name.indexOf('@') < 0) {
                        name = name + '@' + (parent ? parent.state.name : '');
                    }
                    var view = { name: name, state: null };
                    element.data('$uiView', view);

                    scope.$on('$stateChangeSuccess', function () { updateView(true); });
                    updateView(false);
                }
            };
            return directive;
        }
        $ViewDirective.$inject = ['$detour', '$compile', '$controller', '$injector', '$anchorScroll'];

        angular.module('agt.detour').directive('uiView', $ViewDirective);

    });

    define('angular-detour', [
      './detourProvider',
      './viewDirective'
    ], function () { });
}());