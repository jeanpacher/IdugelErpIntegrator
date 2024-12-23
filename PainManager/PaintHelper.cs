using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintManager
{
    /// <summary>
    ///     Paint Helper
    /// </summary>
    public class PaintHelper
    {

        public PaintHelper()
        {
            PaintTypesList = new List<string>
            {
                PaintType.Inteiro,
                PaintType.Metade,
                PaintType.Nada
            };
        }

        public List<string> PaintTypesList { get; set; }

        public List<PaintAcabamento> PaintAcabamento { get; set; }

        public double SetArea(PartPaint partPaint)
        {
            if (partPaint.PaintType == PaintType.Inteiro)
            {
                return partPaint.Area / 10000;
            }
            else if (partPaint.PaintType == PaintType.Metade)
            {
                return (partPaint.Area / 2) / 10000;
            }
            else
            {
                return 0;
            }
        }

    }

}
