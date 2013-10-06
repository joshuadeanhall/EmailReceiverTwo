'use strict';

emailReceiverApp.controller('CreateUserController',
    function CreateUserController($scope, User, $location) {
        $scope.create = function() {
            var newUser = new User();
            newUser.password = $scope.Password;
            newUser.UserName = $scope.UserName;
            newUser.email = $scope.Email;
            newUser.FriendlyName = $scope.Name;
            newUser.$save(function(data) {
                $location.path('/users');
            });
        };

    });