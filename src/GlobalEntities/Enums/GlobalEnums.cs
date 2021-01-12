using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalEntities.Enums
{
  public  class GlobalEnums
    {
        public enum DBProvider
        {
            SqlClient = 1,
            Oracle = 2
        }
        
        public enum DBName
        {
            Sparrows = 1
        }
       
        public enum MTI
        {
            GetToken = 1
            
        }

        public enum ResponseCode
        {
            Success = 100,
            InvalidApiKey = 101,
            OperationFailed = 102,
            InvalidMobile = 103,
            InvalidEmail = 104,
            InvalidUserId = 105,
            InvalidInputParameter = 106,
            NoDataFound = 107,
            AlreadyExist = 108,
            InvalidAccount = 109,
            SameAccount = 110,
            SystemError = 111,
            NoAction = 112,
            DuplicateOperation = 113
        }


        public enum StatusCode
        {
            Inactieve = 1,
            Actieve = 2
        }


        #region Response Message
        public static class ResponseMessage
        {
            public const string Success = "Operation Success";
            public const string MailSendSuccess = "Email Send Successfully.";
            public const string MailSendFailed = "Email Send Failed.";
            public const string GetMenuCategorySuccess = "Get Menu category item successfully.";
            public const string GetMenuCategoryFailed = "Get Menu category item failed.";
            public const string MenuCategoryAddSuccess = "Menu category item added successfully.";
            public const string MenuAddSuccess = "Menu item added successfully.";
            public const string MenuAddFailed = "Failed to add menu item.";
        }
        #endregion


    }
}
