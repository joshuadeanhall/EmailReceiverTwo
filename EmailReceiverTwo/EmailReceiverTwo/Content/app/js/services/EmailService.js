'use strict';

emailReceiverApp.factory('Email', function ($resource) {
    return $resource('/email/:dest/:id', {}, {
        process: {method: 'POST', params: {dest:"process", id:'@id'}}
    });
});