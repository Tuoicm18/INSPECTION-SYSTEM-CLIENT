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

        //WINDOW CONTROL
        public static readonly double TEXT_BLOCK_DESCRIPTION_MAX_WIDTH = 740;
        public static readonly double DATA_GRID_MAX_WIDTH = 740;
        public static int MAX_WIDTH_TEXT_BLOCK = 850;
        public static int WIDTH_KEY_COLUMN = 250;
        public static int WIDTH_VALUE_COLUMN = 608;
        public static string HEADER_KEY_COLUMN = "KEY";
        public static string HEADER_VALUE_COLUMN = "     VALUE";
        public static string CODE_COLOR_ROW_DATA_GRID = "#111111";
        public static string CODE_COLOR_ALTERNATING_ROW_DATA_GRID = "#282828";
    }
}
