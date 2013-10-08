'use strict';

emailReceiverApp.controller('EmailController',
    function EmailController($scope, Email, EmailFinder) {
        $scope.emails = Email.query();
        var emailHub = $.connection.emailHub;
        emailHub.client.EmailRemoved = function (data) {
            $scope.$apply(function () {
                var index = EmailFinder.FindIndex($scope.emails, data);
                if(index >= 0)
                    $scope.emails.splice(index, 1);
            });
        };

        emailHub.client.AddEmail = function (data) {
            $scope.$apply(function() {
                $scope.emails.push(data); //({ Subject: data.Subject, To: data.To, From: data.From });
            });
        };
        $.connection.hub.start();

        $scope.processEmail = function (id) {

            Email.process({ id: id });
        };
    });