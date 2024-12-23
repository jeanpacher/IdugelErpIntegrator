using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintManager
{
    public class PartPaint
    {

        [DisplayName("Código")]
        public string Code { get; set; }

        [DisplayName("Descrição")]
        public string Description { get; set; }

        [DisplayName("Quantidade")]
        public int Quantity { get; set; }

        [DisplayName("Local de Pintura")]
        public string PaintType { get; set; }

        [DisplayName("Área Un. M²")]
        public double Area { get; set; }

        [DisplayName("Acabamento")]
        public string AcabamentoPintura { get; set; }

        [DisplayName("Interno Diferente")]
        public string InternoDiferente { get; set; }

    }
}
