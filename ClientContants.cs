using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientInspectionSystem {
    public class ClientContants {
        public static readonly string LABEL_ADD_TEXT = "ADD TEXT";
        public static readonly string LABEL_ADD_VALUE = "ADD KEY";

        //VALIDATION
        public static readonly string LABEL_VALIDATION_ADD_GROUP = "DUPLICATE GROUP";
        public static readonly string LABEL_VALIDATION_ADD_CONTENT = "DUPLICATE CONTENT";
        public static readonly string LABEL_VALIDATION_ADD_KEY = "DUPLICATE KEY";

        //WINDOW CONTROL FORM AUTHORIZATION DATA NEW
        public static readonly double TEXT_BLOCK_DESCRIPTION_MAX_WIDTH = 740;
        public static readonly double DATA_GRID_MAX_WIDTH = 740;
        public static int MAX_WIDTH_TEXT_BLOCK = 850;
        public static int WIDTH_KEY_COLUMN = 250;
        public static int WIDTH_VALUE_COLUMN = 608;
        public static string HEADER_KEY_COLUMN = "KEY";
        public static string HEADER_VALUE_COLUMN = "     VALUE";
        public static string CODE_COLOR_ROW_DATA_GRID = "#111111";
        public static string CODE_COLOR_ALTERNATING_ROW_DATA_GRID = "#282828";
        public static string CONTENT_BTN_IMPORT_JSON = "IMPORT JSON";
        public static string CONTENT_BTN_IMPORT_JSON_CANCEL = "CANCEL";
        public static string CONTENT_BTN_VIEW_JWT = "VIEW";
        public static string CONTENT_BTN_VIEW_JWT_CANCEL = "CANCEL";

        //WINDOW CONTROL FORM AUTHORIZATION DATA RESLUT
        public static string LB_NOT_FOUND_FINGER = "NOT FOUND FINGER";

        //SOCKET RESPONSE CODE
        public static int SOCKET_RESP_CODE_BIO_AUTH_DENIED = 1302;
        public static int SOCKET_RESP_CODE_BIO_SUCCESS = 0;
    }
}
