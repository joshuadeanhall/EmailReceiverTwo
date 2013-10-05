'use strict';

emailReceiverApp.factory('User', function ($resource) {
    return $resource('/user', {}, {

    });
});