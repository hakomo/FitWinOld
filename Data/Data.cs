using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;

namespace FitWinN {

    public class Data {

        public bool HAHVisible = false, MultiVisible = false, EditorVisible = true, SelfVisible = false, Resident = true,
            HAHMultiEnable = true, HAHTaskEnable = true, HAHAutoNext = true, HAHIsSplit = false, HAHTemplateIsSplit = false, Filters = true;
        public int MultiHeight = 60, EditorHeight = 200, EditorBorderWidth = 8, TaskLines = 2, TemplateArea = 8192, HAHKind = 1;
        public Rectangle Bounds = new Rectangle(440, 0, 480, 700), MultiBounds = new Rectangle();
        public Split[] Splits;
        public HashSet<string> FilteredClassNames = new HashSet<string>();

        public static Data File {
            get {
                FileStream fs = null;
                try {
                    fs = new FileStream(Application.StartupPath + "\\fitwin.dat", FileMode.Open);
                    return (Data)(new DataContractJsonSerializer(typeof(Data))).ReadObject(fs);
                } catch {
                    Data data = new Data();
                    using(MemoryStream ms = new MemoryStream(Properties.Resources._default))
                        data.Splits = (Split[])(new DataContractJsonSerializer(typeof(Split[]))).ReadObject(ms);
                    return data;
                } finally {
                    if(fs != null)
                        fs.Dispose();
                }
            }
            set {
                FileStream fs = null;
                try {
                    fs = new FileStream(Application.StartupPath + "\\fitwin.dat", FileMode.Create);
                    (new DataContractJsonSerializer(typeof(Data))).WriteObject(fs, value);
                } catch {
                } finally {
                    if(fs != null)
                        fs.Dispose();
                }
            }
        }
    }
}
