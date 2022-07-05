using System;
using System.Collections.Generic;

namespace ClientInspectionSystem {
    public class InspectionSystemContanst {
        //Optional details
        public static string PERSONAL_NUMBER = "PERSONAL NUMBER";
        public static string FULL_NAME = "FULL NAME";
        public static string BIRTH_DATE = "BIRTH DATE";
        public static string GENDER = "GENDER";
        public static string NATIONALITY = "NATIONALITY";
        public static string ETHNIC = "ETHNIC";
        public static string RELIGION = "RELIGION";
        public static string PLACE_OF_ORIGIN = "PLACE OF ORIGIN";
        public static string PLACE_OF_RESIDENCE = "PLACE OF RESIDENCE";
        public static string PERSONAL_IDENTIFICATION = "PERSONAL IDENTIFICATION";
        public static string ISSUANCE_DATE = "ISSUANCE DATE";
        public static string EXPIRY_DATE = "EXPIRY DATE";
        public static string EX_IDENTIFICATION_DOCUMENT = "ID DOCUMENT";
        public static string FULL_NAME_OF_FATHER = "FULL NAME OF FATHER";
        public static string FULL_NAME_OF_MOTHER = "FULL NAME OF MOTHER";
        public static string FULL_NAME_OF_SPOUSE = "FULL NAME OF SPOUSE";

        //Device details
        public static string DEVICE_NAME = "DEVICE NAME";
        public static string DEVIC_SERIAL_NUMBER = "DEVICE SERIAL NUMBER";

        //Title & message content Metro Dialog
        public static string TITLE_MESSAGE_BOX = "CHECKID CLIENT DEMO" + "\n";
        public static string CONTENT_CONNECTING_MESSAGE_BOX = "CONNECTING SOCKET SERVER";
        public static string CONTENT_RE_CONNECT_MESSAGE_BOX = "RE-CONNECT SOCKET SERVER";
        public static string CONTENT_CONNECTED_MESSAGE_BOX = "SOCKET SERVER CONNECTION SUCCESSFUL";
        public static string CONTENT_CONNECTED_DENIED_MESSAGE_BOX = "CONNECTION DENIED";
        public static string CONTENT_CONNECTED_FAIL_MESSAGE_BOX = "CONNECTION FAIL";
        public static string CONTENT_FALIL = "ISSUE FOUND. PLEASE CHECK AGAIN";
        public static string CONTENT_SUCCESS = "SUCCESSFULY";
        public static string CONTENT_READING_CHIP_MESSAGE_BOX = "READING CHIP...";
        public static string CONTENT_WATTING_BIOMETRIC_RESULT_MESSAGE_BOX = "WATING FOR RESULT...";
        public static string CONTENT_CARD_DETECTION_EVENT = "DETECT CARD";
        public static string CONTENT_CARD_NOT_DETECTED_EVENT = "CARD NOT DETECTED";
        public static string CONTENT_REFRESH_SUCCESS_MESSAGE_BOX = "SUCCESSFULY REFRESH";
        public static string CONTENT_SCAN_SUCCESS_MESSAGE_BOX = "SUCCESSFULY SCAN";

        //Time out dialog
        public static int DIALOG_TIME_OUT_1k = 1000;
        public static int DIALOG_TIME_OUT_2k = 2000;
        public static int DIALOG_TIME_OUT_3k = 3000;
        public static int DIALOG_TIME_OUT_4k = 4000;
        public static int DIALOG_TIME_OUT_5k = 5000;

        //Time out response socket
        public static int TIME_OUT_RESP_SOCKET_1S = 1;
        public static int TIME_OUT_RESP_SOCKET_2S = 2;
        public static int TIME_OUT_RESP_SOCKET_3S = 3;
        public static int TIME_OUT_RESP_SOCKET_4S = 4;
        public static int TIME_OUT_RESP_SOCKET_5S = 5;
        public static int TIME_OUT_RESP_SOCKET_10S = 10;
        public static int TIME_OUT_RESP_SOCKET_15S = 15;
        public static int TIME_OUT_RESP_SOCKET_20S = 20;
        public static int TIME_OUT_RESP_SOCKET_25S = 25;
        public static int TIME_OUT_RESP_SOCKET_30S = 30;
        public static int TIME_OUT_RESP_SOCKET_35S = 35;
        public static int TIME_OUT_RESP_SOCKET_40S = 40;
        public static int TIME_OUT_RESP_SOCKET_45S = 45;
        public static int TIME_OUT_RESP_SOCKET_50S = 50;
        public static int TIME_OUT_RESP_SOCKET_55S = 55;
        public static int TIME_OUT_RESP_SOCKET_60S = 60;
        public static int TIME_OUT_RESP_SOCKET_2M = 120;
        public static int TIME_OUT_RESP_SOCKET_3M = 180;
        public static int TIME_OUT_RESP_SOCKET_4M = 240;
        public static int TIME_OUT_RESP_SOCKET_5M = 300;
        public static int TIME_OUT_RESP_SOCKET_10M = 600;
        public static int TIME_OUT_RESP_SOCKET_15M = 900;
        public static int TIME_OUT_RESP_SOCKET_20M = 1200;
        public static int TIME_OUT_RESP_SOCKET_25M = 1500;
        public static int TIME_OUT_RESP_SOCKET_30M = 1800;
        public static int TIME_OUT_RESP_SOCKET_60M = 3600;
        //Connect Socket
        public static string SUB_URL = "/ISPlugin";

        //FORM AUTHENTICATION
        public static string TITLE_FORM_BIOMETRIC_AUTH_FACE = "FACE VERIFICATION";
        public static string TITLE_FORM_BIOMETRIC_AUTH_FINGER= "FINGER VERIFICATION";
        public static string RESUT_FORM_BIOMETRIC_AUTH_SUCCESS = "SUCCESS";
        public static string RESUT_FORM_BIOMETRIC_AUTH_FAILURE = "FAILURE";
        public static string RESUT_FORM_BIOMETRIC_AUTH_DENIED = "CANCEl";
        public static string RESUT_FORM_BIOMETRIC_AUTH_NOT_FOUND_FINGER = "NO FINGER FOUND";
        public static string PATH_IMG_FACE_SUCCESS = "/Resource/47_FA_passed.png";
        public static string PATH_IMG_FACE_FAILURE = "/Resource/47_FA_failed.png";
        public static string PATH_IMG_FINGER_LEFT_FAILURE = "/Resource/01_left_hand_Failed.png";
        public static string PATH_IMG_FINGER_LEFT_SUCCESS = "/Resource/01_left_hand_Passed.png";
        public static string PATH_IMG_FINGER_RIGHT_FAILURE = "/Resource/01_right_hand_Failed.png";
        public static string PATH_IMG_FINGER_RIGHT_SUCCESS = "/Resource/01_right_hand_Passed.png";
        public static string PATH_IMG_FINGER_LEFT_NOT_FOUND = "/Resource/02_left_hand_1.png";
        public static string PATH_IMG_FINGER_RIGHT_NOT_FOUND = "/Resource/01_right_hand_1.png";

        //Background button 
        public static string GET_BACKGROUND_BTN_PASSALL = "#FF0767B3";
        public static string SET_BACKGROUND_BTN_PASSALL = "#0767b3";
    }
}
