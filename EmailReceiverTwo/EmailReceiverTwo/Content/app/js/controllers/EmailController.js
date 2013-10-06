'use strict';

emailReceiverApp.controller('EmailController',
    function EmailController($scope, Email) {
        $scope.emails = Email.query();


        var emailHub = $.connection.emailHub;

        emailHub.client.EmailRemoved = function () {
            var x = 'test';
        };

        emailHub.client.EmailRemovedTest = function() {
            var x = 'test2';
        };
        
        emailHub.client.EmailRemovedServerTest = function () {
            var x = 'test2';
        };


        //var emailHubProxy = signalRHubProxy(
        //signalRHubProxy.defaultServer, 'emailHub',
        //    { logging: true });

        //emailHubProxy.on('EmailRemoved', function (data) {
        //    $scope.emails.splice($scope.emails.indexOf(data), 1);
        //});

        //emailHubProxy.on('AddEmail', function(data) {
        //    $scope.emails.push({ Subject: data.Subject, To: data.To, From: data.From });
        //});

        $.connection.hub.start().done(function() {
            $scope.processEmail = function(id) {
                emailHub.server.addEmailTest();
                Email.process({ id: id });
            };
        });
        //$scope.processEmail = function (id) {

        //    Email.process({ id: id });
        //};
    });