'use strict';

emailReceiverApp.controller('EmailController',
    function EmailController($scope, Email, EmailHubService, EmailFinder) {
        $scope.emails = Email.query();
        var emailHub = EmailHubService();

        $scope.$on('emailRemoved', function (event, data) {
            var index = EmailFinder.FindIndex($scope.emails, data);
            if (index >= 0) {
                $scope.emails.splice(index, 1);
                $scope.$apply();
            }
        });

        $scope.$on('addEmail', function (event, data) {
            $scope.$apply(function() {
                $scope.emails.push({ Subject: data.Subject, To: data.To, From: data.From });
            });
        });


        $scope.processEmail = function (id) {

            Email.process({ id: id });
        };
    });