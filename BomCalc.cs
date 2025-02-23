using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Inventor;

using MessageBox = System.Windows.MessageBox;

namespace BomCore
{

    // Dados para Cálculo de MP

    /// <summary>
    /// Verificação e Cálculo do peso bruto
    /// </summary>
    public static class BomCalc
    {
        
        public static double Calcule(BomData bomData)
        {
            if (bomData.Fator.Trim() != String.Empty)
            {
                BlankCalc blankCalc = new BlankCalc();

                switch (bomData.BlankType)
                {
                    case BlankType.SheetMetal:

                        bomData.PesoBruto = blankCalc.SheetMetal(bomData);

                        break;
                    case BlankType.Cylinder:

                        bomData.PesoBruto = blankCalc.Cylinder(bomData);

                        break;
                    case BlankType.Linear:

                        bomData.PesoBruto = blankCalc.Linear(bomData);

                        break;

                    default:

                        MessageBox.Show($"Nenhum dos Padrões Foi Encontrado!\n\n{bomData.BlankType}");

                        bomData.PesoBruto = 0;

                        break;
                }

                //return Math.Round(bomData.PesoBruto, 2);

                return Math.Round(bomData.PesoBruto,2);

            }
            else
            {
                System.Windows.Forms.MessageBox.Show("O fator da Matéria Prima está vazio ou é inválido\nAdicione o Fator para realizar o cálculo.","Erro no Fator",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                return 0;
            }
           
            
        }

        public static double Calcule(BomData bomData, Document doc)
        {
            if (bomData.Fator.Trim() != string.Empty)
            {
                BlankCalc blankCalc = new BlankCalc();

                switch (bomData.BlankType)
                {
                    case BlankType.SheetMetal:

                        bomData.PesoBruto = blankCalc.SheetMetal(bomData, doc);

                        break;
                    case BlankType.Cylinder:

                        bomData.PesoBruto = blankCalc.Cylinder(bomData);

                        break;
                    case BlankType.Linear:

                        bomData.PesoBruto = blankCalc.Linear(bomData, doc);

                        break;

                    default:

                        MessageBox.Show($"Nenhum dos Padrões Foi Encontrado!\n\n{bomData.BlankType}");

                        bomData.PesoBruto = 0;

                        break;
                }

                //return Math.Round(bomData.PesoBruto, 2);

                return Math.Round(bomData.PesoBruto, 2);

            }
            else
            {
                System.Windows.Forms.MessageBox.Show("O fator da Matéria Prima está vazio ou é inválido\nAdicione o Fator para realizar o cálculo.", "Erro no Fator", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return 0;
            }


        }
    }


}

