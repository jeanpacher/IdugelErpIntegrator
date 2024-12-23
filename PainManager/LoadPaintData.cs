using System.Collections.Generic;

namespace PaintManager
{

    /// <summary>
    ///     Carrega os dados de acabamento de pintura
    /// </summary>
    public class LoadPaintData
    {
        public LoadPaintData()
        {
            PaintAcabamentoList = new List<string>();
            GetPaintData();
        }

        public List<string> PaintAcabamentoList { get; set; }

        public void GetPaintData()
        {
            //var import = File.ReadAllText($"{FileHelper.GetAppPath<LoadPaintData>()}\\AcabamentosPintura.json");

            //var data = JsonConvert.DeserializeObject<IList<PaintAcabamento>>(import);

            PaintAcabamentoList.Add(PaintAcabamentoType.Padrao);
            PaintAcabamentoList.Add(PaintAcabamentoType.Interno);
            PaintAcabamentoList.Add(PaintAcabamentoType.LaranjaSeguranca);
            PaintAcabamentoList.Add(PaintAcabamentoType.AmareloSeguranca);
            PaintAcabamentoList.Add(PaintAcabamentoType.Outro);
        }



    }

}