using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dBC = ConnectorDataBase;


namespace DescriptionManager
{
    public class FieldsDescription : IDescription
    {
        //public FieldsDescription(string name, string iprop, string family, string script, string blankType)
        //{
        //    Name = name;
        //    IpropField = iprop;
        //    Family = family;
        //    Script = script;
        //    BlankType = blankType;

        //}

        public FieldsDescription(dBC.DESCRIPTION_MANAGER dm)
        {
            Name = dm.NAME_DESCRIPTION;
            IpropField = dm.NAME_IPROP_FIELD;
            Family = dm.FAMILY_TYPE;
            Script = dm.SCRIPT_DESC;
            BlankType = dm.COMMENT;
        }

        public string Name { get; set; }
        public string IpropField { get; set; }
        public string Family { get; set; }
        public string Script { get; set; }
        public string BlankType { get; set; }
    }
}