using System;
using System.Windows.Forms;

using Inventor;
using AppResources;

using InvUtils;

using WUtils;

namespace BomCore
{

    /// <summary>
    /// Cálculo do blank e peso bruto
    /// </summary>
    public class BlankCalc
    {
        private readonly double mil = 1000.0;
        
        public double SheetMetal(BomData bomData)
        {
            var xyBlankResult = SheetMetalResult(bomData.Blank);

            if (xyBlankResult.Length < 2) { MessageBox.Show("Verifique o Blank da Peça.\nO Blank possui apenas uma valor ou está vazio.\n\nO peso será definido com valor 0."); return 0;}      

            var xResult = xyBlankResult[0]; // SheetMetalResult(bomData.Blank)[0];
            var yResult = xyBlankResult[1]; // SheetMetalResult(bomData.Blank)[1];
            
            // Verifica se o reslutado está vazio.
            //Normalmente ocorre quando a família nào está cadastrada no Description Manager
            if (xResult == string.Empty || yResult == string.Empty)
            {
                MessageBox.Show("Um dos valores do Blank está com valor 0.\nVerifique o Blank.\n\nO peso será definido com valor 0.");
                return 0;
            }

            var xValue = xResult.ToDouble();
            var yValue = yResult.ToDouble();

            double fator = bomData.Fator.ToDouble();

            var calcResult = ((((xValue * yValue) / mil) / mil) * fator);

            //var logResult = $"Blank: {bomData.Blank} > ValorX: {xValue} > ValorY: {yValue} > Fator: {fator} = Resultado: {calcResult}";
            
            //CoreLog.WriteLog(logResult);

            // Cálculo do Blank
            return calcResult;
        }

        public double SheetMetal(BomData bomData,Document doc)
        {
            var xyBlankResult = SheetMetalResult(bomData.Blank);

            if (xyBlankResult.Length < 2) { MessageBox.Show("Verifique o Blank da Peça.\nO Blank possui apenas uma valor ou está vazio.\n\nO peso será definido com valor 0."); return 0; }

            var xResult = xyBlankResult[0]; // SheetMetalResult(bomData.Blank)[0];
            var yResult = xyBlankResult[1]; // SheetMetalResult(bomData.Blank)[1];

            // Verifica se o reslutado está vazio.
            //Normalmente ocorre quando a família nào está cadastrada no Description Manager
            if (xResult == string.Empty || yResult == string.Empty)
            {
                MessageBox.Show("Um dos valores do Blank está com valor 0.\nVerifique o Blank.\n\nO peso será definido com valor 0.");
                return 0;
            }

            var xValue = xResult.ToDouble();
            var yValue = yResult.ToDouble();

            double fator = bomData.Fator.ToDouble();

            var calcResult = ((((xValue * yValue) / mil) / mil) * fator);

            //var logResult = $"Blank: {bomData.Blank} > ValorX: {xValue} > ValorY: {yValue} > Fator: {fator} = Resultado: {calcResult}";

            //CoreLog.WriteLog(logResult);

            // Cálculo do Blank
            return calcResult;
        }

        public double Cylinder(BomData bomData)
        {
            var fator = bomData.Fator.ToDouble();
            var blank = bomData.Blank.ToDouble();
            
            return fator * blank;
        }

        /// <summary>
        /// Calcula blank via Metro Linear
        /// </summary>
        /// <param name="bomData"></param>
        /// <returns></returns>
        public double Linear(BomData bomData)
        {
            try
            {
                var fator = bomData.Fator.ToDouble();

                var resultCompParameter = InvParameter.GetParameter(ParameterNames.COMP);

                if (resultCompParameter == "0" || resultCompParameter == string.Empty) return 0;
           
                var result = resultCompParameter.ToDouble();

                var calcResult = (result/mil)*fator;
                
                return calcResult;
            }
            catch (Exception e)
            {
                CoreLog.WriteLog($"Erro ao calcular o blank. \n\tBomData: {bomData.Blank}.\n\t{e}");
                return 0;
            }
        }

        /// <summary>
        /// Cálculo Linear
        /// </summary>
        /// <param name="bomData"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public double Linear(BomData bomData, Document doc)
        {
            try
            {
                var fator = bomData.Fator.ToDouble();

                var resultCompParameter = InvParameter.GetParameter(ParameterNames.COMP,doc);

                if (resultCompParameter == "0" || resultCompParameter == string.Empty) return 0;

                var result = resultCompParameter.ToDouble();

                var calcResult = (result / mil) * fator;

                return Math.Round(calcResult,2);
            }
            catch (Exception e)
            {
                CoreLog.WriteLog($"Erro ao calcular o blank. \n\tBomData: {bomData.Blank}.\n\t{e}");
                return 0;
            }
        }


        private string[] SheetMetalResult(string value)
        {
            try
            {
                var valueSplitResult = value.ToUpper().Split('X');

                return valueSplitResult;
            }
            catch (Exception e)
            {
                CoreLog.WriteLog($"Erro ao calcular o blank - SheetMetalResult.\n\t{e}");
                return null;
            }
        }
        
    }

}