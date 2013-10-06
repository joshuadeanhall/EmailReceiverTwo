'use strict';

emailReceiverApp.controller('EmailController',
    function EmailController($scope, Email, signalRHubProxy) {
        $scope.emails = Email.query();


        var emailHub = $.connection.emailHub;

        emailHub.client.EmailRemoved = function (data) {
            $scope.emails.splice($scope.emails.indexOf(data), 1);
        };

        emailHub.client.AddEmail = function(data) {
            $scope.emails.push({ Subject: data.Subject, To: data.To, From: data.From });
        };
        $.connection.hub.start();
        
        $scope.processEmail = function (id) {

            Email.process({ id: id });
        };
    });