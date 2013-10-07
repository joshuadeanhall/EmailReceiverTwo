'use strict';

var emailReceiverApp = angular.module('emailReceiverApp', ['ngResource']);

emailReceiverApp.config(function ($routeProvider) {

    $routeProvider.
        when('/emails', {
            controller: 'EmailController',
            templateUrl: 'Content/app/views/emails.html'
        }).
        when('#/users', {
            controller: 'UserController',
            templateUrl: 'Content/app/views/user.html'
        }).
        when('/createuser', {
            controller: 'CreateUserController',
            templateUrl: 'Content/app/views/create-user.html'
        }).
        otherwise({ redirectTo: '/' });
});