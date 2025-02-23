using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DescriptionManager
{
    public interface IDescription
    {
        string Name { get; set; }
        string IpropField { get; set; }
        string Family { get; set; }
        string Script { get; set; }
        string BlankType { get; set; }
    }
}