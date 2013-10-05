'use strict';

emailReceiverApp.controller('EmailController',
    function EmailController($scope, Email, signalRHubProxy) {
        $scope.emails = Email.query();        

        var emailHubProxy = signalRHubProxy(
        signalRHubProxy.defaultServer, 'emailHub',
            { logging: true });

        emailHubProxy.on('EmailRemoved', function (data) {
            $scope.emails.splice($scope.emails.indexOf(data), 1);
        });

        emailHubProxy.on('AddEmail', function(data) {
            $scope.emails.push({ Subject: data.Subject, To: data.To, From: data.From });
        });

        $scope.processEmail = function (id) {
            Email.process({ id: id });
        };
    });