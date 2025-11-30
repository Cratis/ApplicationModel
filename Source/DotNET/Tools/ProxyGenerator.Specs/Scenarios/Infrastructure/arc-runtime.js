// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// Arc Runtime Shim for Testing
// This provides minimal implementations of Arc's JavaScript APIs for testing in V8

var exports = exports || {};
var module = module || { exports: exports };

// Mock fetch API
var __pendingFetchRequests = [];
var __fetchResponses = {};

function fetch(url, options) {
    return new Promise(function(resolve, reject) {
        __pendingFetchRequests.push({ url: url, options: options, resolve: resolve, reject: reject });
    });
}

// Complete a pending fetch request with a response
function __completeFetch(url, response) {
    var request = __pendingFetchRequests.find(function(r) { return r.url.indexOf(url) !== -1; });
    if (request) {
        __pendingFetchRequests = __pendingFetchRequests.filter(function(r) { return r !== request; });
        request.resolve({
            ok: true,
            status: 200,
            json: function() { return Promise.resolve(response); }
        });
    }
}

// PropertyDescriptor
var PropertyDescriptor = function(name, constructor) {
    this.name = name;
    this.constructor = constructor;
};

// ParameterDescriptor
var ParameterDescriptor = function(name, constructor) {
    this.name = name;
    this.constructor = constructor;
};

// Validator
var Validator = function() {};

// CommandPropertyValidators type
var CommandPropertyValidators = {};

// CommandValidator
var CommandValidator = function() {
    this.properties = {};
};

// CommandResult
var CommandResult = function(result, responseType, isEnumerable) {
    this.correlationId = result.correlationId || '';
    this.isSuccess = result.isSuccess !== undefined ? result.isSuccess : true;
    this.isAuthorized = result.isAuthorized !== undefined ? result.isAuthorized : true;
    this.isValid = result.isValid !== undefined ? result.isValid : true;
    this.hasExceptions = result.hasExceptions || false;
    this.validationResults = result.validationResults || [];
    this.exceptionMessages = result.exceptionMessages || [];
    this.exceptionStackTrace = result.exceptionStackTrace || '';
    this.response = result.response;
};

CommandResult.failed = function(messages) {
    return new CommandResult({
        isSuccess: false,
        isValid: false,
        exceptionMessages: messages
    });
};

// QueryResult
var QueryResult = function(result, modelType, isEnumerable) {
    this.correlationId = result.correlationId || '';
    this.isSuccess = result.isSuccess !== undefined ? result.isSuccess : true;
    this.isAuthorized = result.isAuthorized !== undefined ? result.isAuthorized : true;
    this.isValid = result.isValid !== undefined ? result.isValid : true;
    this.hasExceptions = result.hasExceptions || false;
    this.validationResults = result.validationResults || [];
    this.exceptionMessages = result.exceptionMessages || [];
    this.exceptionStackTrace = result.exceptionStackTrace || '';
    this.data = result.data;
    this.paging = result.paging || { isPaged: false };
};

QueryResult.noSuccess = {
    isSuccess: false,
    isValid: false,
    data: null
};

// Paging
var Paging = function(page, pageSize) {
    this.page = page || 0;
    this.pageSize = pageSize || 25;
    this.hasPaging = pageSize > 0;
};

Paging.noPaging = new Paging(0, 0);

// Sorting
var Sorting = function(field, direction) {
    this.field = field || '';
    this.direction = direction || 'asc';
    this.hasSorting = !!field;
};

Sorting.none = new Sorting();

// SortDirection
var SortDirection = {
    ascending: 'asc',
    descending: 'desc'
};

// Globals
var Globals = {
    microservice: '',
    microserviceHttpHeader: 'X-Cratis-Microservice'
};

// UrlHelpers
var UrlHelpers = {
    replaceRouteParameters: function(route, args) {
        var result = route;
        var unusedParameters = {};
        if (args) {
            Object.keys(args).forEach(function(key) {
                var pattern = '{' + key + '}';
                if (result.indexOf(pattern) !== -1) {
                    result = result.replace(pattern, encodeURIComponent(args[key]));
                } else {
                    unusedParameters[key] = args[key];
                }
            });
        }
        return { route: result, unusedParameters: unusedParameters };
    },
    createUrlFrom: function(origin, basePath, route) {
        if (origin) {
            return origin + route;
        }
        return route;
    },
    buildQueryParams: function(params, additionalParams) {
        var merged = Object.assign({}, params, additionalParams);
        var searchParams = new URLSearchParams();
        Object.keys(merged).forEach(function(key) {
            if (merged[key] !== undefined && merged[key] !== null) {
                searchParams.append(key, merged[key].toString());
            }
        });
        return searchParams;
    }
};

// URLSearchParams polyfill if needed
if (typeof URLSearchParams === 'undefined') {
    var URLSearchParams = function() {
        this._params = {};
    };
    URLSearchParams.prototype.append = function(key, value) {
        this._params[key] = value;
    };
    URLSearchParams.prototype.toString = function() {
        var parts = [];
        for (var key in this._params) {
            parts.push(encodeURIComponent(key) + '=' + encodeURIComponent(this._params[key]));
        }
        return parts.join('&');
    };
}

// joinPaths
function joinPaths() {
    var parts = Array.prototype.slice.call(arguments);
    return parts.filter(function(p) { return p && p.length > 0; }).join('/').replace(/\/+/g, '/');
}

// JsonSerializer
var JsonSerializer = {
    serialize: function(obj) {
        return JSON.stringify(obj);
    },
    deserialize: function(json) {
        return JSON.parse(json);
    }
};

// ParametersHelper
var ParametersHelper = {
    collectParameterValues: function(query) {
        var values = {};
        if (query.parameterDescriptors) {
            query.parameterDescriptors.forEach(function(desc) {
                if (query[desc.name] !== undefined) {
                    values[desc.name] = query[desc.name];
                }
            });
        }
        return values;
    }
};

// ValidateRequestArguments
function ValidateRequestArguments(queryName, requiredParams, args) {
    if (!requiredParams || requiredParams.length === 0) {
        return true;
    }
    if (!args) {
        return false;
    }
    for (var i = 0; i < requiredParams.length; i++) {
        if (args[requiredParams[i]] === undefined) {
            return false;
        }
    }
    return true;
}

// Mock AbortController if not available
if (typeof AbortController === 'undefined') {
    var AbortController = function() {
        this.signal = {};
    };
    AbortController.prototype.abort = function() {};
}
