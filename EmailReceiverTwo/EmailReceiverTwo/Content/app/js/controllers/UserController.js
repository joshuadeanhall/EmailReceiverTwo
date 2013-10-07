'use strict';

emailReceiverApp.controller('UserController',
    function UserController($scope, User) {
        $scope.users = User.query();
    });