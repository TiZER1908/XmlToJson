using System.Xml.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Globalization;
using System.ComponentModel.DataAnnotations;


namespace Lab1{
    public class XmlToJson{
        public static void Main(){
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        string pathfile = "V006.xml";

        string format = "dd.mm.yyyy";
        string zglvtype = "";
        double zglvvers = 0;
        DateTime zglvdate = DateTime.Now;
        CultureInfo culture;
        culture = new CultureInfo("");
        culture.NumberFormat.NumberDecimalSeparator = ".";
        var packet = new List<DictionaryBaseType>();

        XDocument xml = XDocument.Load(pathfile);

        XElement? packetxml = xml.Element("packet");

        if (packetxml is not null){
            XElement? zglvxml = packetxml.Element("zglv");
            if (zglvxml is not null){
                XElement? type = zglvxml.Element("type");
                XElement? version = zglvxml.Element("version");
                XElement? date = zglvxml.Element("date");
                if (type is not null){
                    zglvtype = type.Value;
                }
                if (version is not null){
                    zglvvers = double.Parse(version.Value, culture);
                }
                if (date is not null){
                    zglvdate = DateTime.ParseExact(date.Value, format, null);
                } 
            }

            foreach (XElement zapxml in packetxml.Elements("zap")){
                DictionaryBaseType zap = new DictionaryBaseType();
                XElement? idump = zapxml.Element("IDUMP");
                XElement? datebeg = zapxml.Element("DATEBEG");
                XElement? dateend = zapxml.Element("DATEEND");
                XElement? umpname = zapxml.Element("UMPNAME");
                if (idump is not null){
                    zap.Code = int.Parse(idump.Value);
                }
                if (datebeg is not null){
                    if (datebeg.Value != ""){
                        zap.BeginDate = DateTime.ParseExact(datebeg.Value, format, null);
                    }
                }
                if (dateend is not null ){
                    if (dateend.Value != ""){
                        zap.EndDate = DateTime.ParseExact(dateend.Value, format, null);
                    }
                }
                if (umpname is not null){
                    zap.Name = umpname.Value;
                }
                packet.Add(zap);
            }
        }

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        string path = "V006.json";

        Zglv zglvresult = new Zglv(zglvtype, zglvvers, zglvdate);
        Packet packetresult = new Packet(zglvresult, packet);
        Json result = new Json(packetresult);

        File.WriteAllTextAsync(path, JsonSerializer.Serialize<Json>(result, options));
        }
    }


    public class Json{
        public Packet packet {get; set;}

        public Json(Packet pack){
            packet = pack;
        }
    }

    public class Packet{
        public Zglv zglv {get; set;}

        public List<DictionaryBaseType> zap {get; set;}

        public Packet(Zglv zg, List<DictionaryBaseType> za){
            zglv = zg;
            zap = za;
        }
    }

    public class Zglv{
        public string Type {get; set;}

        public double Version {get; set;}

        public DateTime Date {get; set;}

        public Zglv(string type, double version, DateTime date){
            Type = type;
            Version = version;
            Date = date;
        }
    }

    public class DictionaryBaseType{
        [Display(Name = "Код")]
        public int Code { get; set; }
        
        [Display(Name = "Начало")]
        public DateTime BeginDate { get; set; }

        [Display(Name = "Окончание")]
        public DateTime EndDate { get; set; }
        
        [Display(Name = "Наименование")]
        public string Name { get; set; } = string.Empty;

        public DictionaryBaseType(){
        }
    }
}