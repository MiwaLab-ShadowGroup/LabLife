using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabLife.Data
{
    public abstract class AData
    {
        public virtual void Save(string filename)
        {
            System.Xml.Serialization.XmlSerializer serialize = new System.Xml.Serialization.XmlSerializer(this.GetType());
            System.IO.StreamWriter sw = new System.IO.StreamWriter(filename,false,new System.Text.UTF8Encoding(false));
            serialize.Serialize(sw, this);
            sw.Close();
        }
        public virtual void Load(string filename)
        {
            System.Xml.Serialization.XmlSerializer serialize = new System.Xml.Serialization.XmlSerializer(this.GetType());
            System.IO.StreamReader sr = new System.IO.StreamReader(filename, new System.Text.UTF8Encoding(false));
            this.setData((AData)serialize.Deserialize(sr));
            sr.Close();
        }
        public abstract void setData(AData adata);
    }
}
