using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabLife.Project
{
    public class LLProject
    {
        public int Version;
        public List<AProject> ProjectList;


        public void XmlWrite(string name)
        {
            using (Stream stream = File.OpenWrite(name))
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(this.GetType());
                serializer.Serialize(stream,this);
            }
        }
        public void XmlRead(string name)
        {
            using (Stream stream = File.OpenRead(name))
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(this.GetType());
                var deserialize = (LLProject)serializer.Deserialize(stream);
            }
        }
    }
}
