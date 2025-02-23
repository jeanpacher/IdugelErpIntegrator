namespace BomCore
{

    public class BomData
    {

        public string Fator { get; set; }

        public string Blank { get; set; }

        public double PesoBruto { get; set; }

        public double PesoAcabado { get; set; }
        
        public BlankType BlankType { get; set; }

        public double DiferencaPesoBrutoxAcabado()
        {
            return PesoBruto - PesoAcabado;
        }

    }

}