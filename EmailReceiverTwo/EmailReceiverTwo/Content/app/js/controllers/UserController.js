'use strict';

emailReceiverApp.controller('UserController',
    function UserController($scope, User, $location) {

        $scope.init = function() {
            $scope.users = User.query();
        };
        
        $scope.create = function () {
            var newUser = new User();
            newUser.password = $scope.Password;
            newUser.UserName = $scope.UserName;
            newUser.email = $scope.Email;
            newUser.FriendlyName = $scope.Name;
            newUser.$save(function (data) {
                $location.path('/users');
            });
        };

    });