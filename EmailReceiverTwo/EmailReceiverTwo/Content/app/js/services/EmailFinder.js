'use strict';

emailReceiverApp.factory('EmailFinder', function() {
    return {
       FindIndex : function(emails, email) {
           for (var i = 0, len = emails.length; i < len; i++) {
               if (emails[i].Id == email.Id)
                   return i;
           }
           return -1;
           }
       };
});