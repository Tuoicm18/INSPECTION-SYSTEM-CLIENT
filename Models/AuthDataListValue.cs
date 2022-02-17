using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ClientInspectionSystem.Models {
    public class AuthDataListValue {
        public static ObservableCollection<AuthDataListValueBinding> MultiValues { get; set; }
        public static ObservableCollection<string> SingleValues { get; set; } = new ObservableCollection<string>();
    }

    public class AuthDataListValueBinding {
        public string Value { get; set; }
        public string Group { get; set; }
        public bool MultiChecked { get; set; }
    }
}
