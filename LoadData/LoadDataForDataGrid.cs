using ClientInspectionSystem.Models;
using PluginICAOClientSDK.Models;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ClientInspectionSystem.LoadData {
    public class LoadDataForDataGrid {
        public static void loadDataGridMain(DataGrid dataGridInputDevice,
                                            OptionalDetails optionalDetails) {
            List<DataInputFromDevice> listTest = new List<DataInputFromDevice>();
            if (optionalDetails != null) {
                listTest.Add(new DataInputFromDevice { NO = "01", ITEM = InspectionSystemContanst.PERSONAL_NUMBER, DESCRIPTION = optionalDetails.personalNumber.ToUpper() });
                listTest.Add(new DataInputFromDevice { NO = "02", ITEM = InspectionSystemContanst.FULL_NAME, DESCRIPTION = optionalDetails.fullName.ToUpper() });
                listTest.Add(new DataInputFromDevice { NO = "03", ITEM = InspectionSystemContanst.BIRTH_DATE, DESCRIPTION = optionalDetails.birthDate.ToUpper() });
                listTest.Add(new DataInputFromDevice { NO = "04", ITEM = InspectionSystemContanst.GENDER, DESCRIPTION = optionalDetails.gender.ToUpper() });
                listTest.Add(new DataInputFromDevice { NO = "05", ITEM = InspectionSystemContanst.NATIONALITY, DESCRIPTION = optionalDetails.nationality.ToUpper() });
                listTest.Add(new DataInputFromDevice { NO = "06", ITEM = InspectionSystemContanst.ETHNIC, DESCRIPTION = optionalDetails.ethnic.ToUpper() });
                listTest.Add(new DataInputFromDevice { NO = "07", ITEM = InspectionSystemContanst.RELIGION, DESCRIPTION = optionalDetails.religion.ToUpper() });
                listTest.Add(new DataInputFromDevice { NO = "08", ITEM = InspectionSystemContanst.PLACE_OF_ORIGIN, DESCRIPTION = optionalDetails.placeOfOrigin.ToUpper() });
                listTest.Add(new DataInputFromDevice { NO = "09", ITEM = InspectionSystemContanst.PLACE_OF_RESIDENCE, DESCRIPTION = optionalDetails.placeOfResidence.ToUpper() });
                listTest.Add(new DataInputFromDevice { NO = "10", ITEM = InspectionSystemContanst.PERSONAL_IDENTIFICATION, DESCRIPTION = optionalDetails.personalIdentification.ToUpper() });
                listTest.Add(new DataInputFromDevice { NO = "11", ITEM = InspectionSystemContanst.ISSUANCE_DATE, DESCRIPTION = optionalDetails.issuanceDate.ToUpper() });
                listTest.Add(new DataInputFromDevice { NO = "12", ITEM = InspectionSystemContanst.EXPIRY_DATE, DESCRIPTION = optionalDetails.expiryDate.ToUpper() });
                listTest.Add(new DataInputFromDevice { NO = "13", ITEM = InspectionSystemContanst.EX_IDENTIFICATION_DOCUMENT, DESCRIPTION = optionalDetails.idDocument.ToUpper() }); // DG13 [15] Chillkat
                listTest.Add(new DataInputFromDevice { NO = "14", ITEM = InspectionSystemContanst.FULL_NAME_OF_FATHER, DESCRIPTION = optionalDetails.fullNameOfFather.ToUpper() }); // DG13 [13]
                listTest.Add(new DataInputFromDevice { NO = "15", ITEM = InspectionSystemContanst.FULL_NAME_OF_MOTHER, DESCRIPTION = optionalDetails.fullNameOfMother.ToUpper() }); // DG13 [12]
                listTest.Add(new DataInputFromDevice { NO = "16", ITEM = InspectionSystemContanst.FULL_NAME_OF_SPOUSE, DESCRIPTION = optionalDetails.fullNameOfSpouse.ToUpper() }); // DG13 [14]
                dataGridInputDevice.ItemsSource = listTest;
            }
            else {
                listTest.Add(new DataInputFromDevice { NO = "01", ITEM = InspectionSystemContanst.PERSONAL_NUMBER, DESCRIPTION = string.Empty });
                listTest.Add(new DataInputFromDevice { NO = "02", ITEM = InspectionSystemContanst.FULL_NAME, DESCRIPTION = string.Empty });
                listTest.Add(new DataInputFromDevice { NO = "03", ITEM = InspectionSystemContanst.BIRTH_DATE, DESCRIPTION = string.Empty });
                listTest.Add(new DataInputFromDevice { NO = "04", ITEM = InspectionSystemContanst.GENDER, DESCRIPTION = string.Empty });
                listTest.Add(new DataInputFromDevice { NO = "05", ITEM = InspectionSystemContanst.NATIONALITY, DESCRIPTION = string.Empty });
                listTest.Add(new DataInputFromDevice { NO = "06", ITEM = InspectionSystemContanst.ETHNIC, DESCRIPTION = string.Empty });
                listTest.Add(new DataInputFromDevice { NO = "07", ITEM = InspectionSystemContanst.RELIGION, DESCRIPTION = string.Empty });
                listTest.Add(new DataInputFromDevice { NO = "08", ITEM = InspectionSystemContanst.PLACE_OF_ORIGIN, DESCRIPTION = string.Empty });
                listTest.Add(new DataInputFromDevice { NO = "09", ITEM = InspectionSystemContanst.PLACE_OF_RESIDENCE, DESCRIPTION = string.Empty });
                listTest.Add(new DataInputFromDevice { NO = "10", ITEM = InspectionSystemContanst.PERSONAL_IDENTIFICATION, DESCRIPTION = string.Empty });
                listTest.Add(new DataInputFromDevice { NO = "11", ITEM = InspectionSystemContanst.ISSUANCE_DATE, DESCRIPTION = string.Empty });
                listTest.Add(new DataInputFromDevice { NO = "12", ITEM = InspectionSystemContanst.EXPIRY_DATE, DESCRIPTION = string.Empty });
                listTest.Add(new DataInputFromDevice { NO = "13", ITEM = InspectionSystemContanst.EX_IDENTIFICATION_DOCUMENT, DESCRIPTION = string.Empty });
                listTest.Add(new DataInputFromDevice { NO = "14", ITEM = InspectionSystemContanst.FULL_NAME_OF_FATHER, DESCRIPTION = string.Empty });
                listTest.Add(new DataInputFromDevice { NO = "15", ITEM = InspectionSystemContanst.FULL_NAME_OF_MOTHER, DESCRIPTION = string.Empty });
                listTest.Add(new DataInputFromDevice { NO = "16", ITEM = InspectionSystemContanst.FULL_NAME_OF_SPOUSE, DESCRIPTION = string.Empty });
                dataGridInputDevice.ItemsSource = listTest;
            }
        }

        #region LOAD DATA DEVICE DETAILS DATA GRID
        //Load Data Data Grid Details Device
        public static void loadDataDetailsDeviceNotConnect(DataGrid dataGridDetails,
                                                           string serialNum,
                                                           string deviceName,
                                                           string lastScanTiem,
                                                           string totalPreceeded) {
            List<DeviceDetails> deviceDetails = new List<DeviceDetails>();
            deviceDetails.Add(new DeviceDetails { TITLE = "DEVICE SERIAL NUMBER ", CONTENT = string.IsNullOrEmpty(serialNum) ? "DEVICE NOT CONNECTED" : serialNum, SHOWBUTTON = serialNum.Equals(string.Empty) ? "False" : "True" });
            deviceDetails.Add(new DeviceDetails { TITLE = "DEVICE NAME ", CONTENT = string.IsNullOrEmpty(deviceName) ? "DEVICE NOT CONNECTED" : deviceName, SHOWBUTTON = "False" });
            deviceDetails.Add(new DeviceDetails { TITLE = "LAST SCAN TIME ", CONTENT = string.IsNullOrEmpty(lastScanTiem) ? "N/A" : lastScanTiem, SHOWBUTTON = "False" });
            deviceDetails.Add(new DeviceDetails { TITLE = "TOTAL PRECEEDED ", CONTENT = string.IsNullOrEmpty(totalPreceeded) ? "N/A" : totalPreceeded, SHOWBUTTON = "False" });
            dataGridDetails.ItemsSource = deviceDetails;
        }
        #endregion
    }
}
