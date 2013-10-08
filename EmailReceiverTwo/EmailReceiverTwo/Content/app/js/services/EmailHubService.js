'use strict';


emailReceiverApp.factory('EmailHubService', function ($rootScope) {
    function EmailHubService() {
        var emailHub = $.connection.emailHub;
        emailHub.client.EmailRemoved = function (data) {
            $rootScope.$broadcast('emailRemoved', data);
        };

        emailHub.client.AddEmail = function (data) {
            $rootScope.$broadcast('addEmail', data);
        };
        $.connection.hub.start();
    }
    return EmailHubService;
});