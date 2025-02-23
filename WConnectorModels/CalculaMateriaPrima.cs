using System;
using System.Collections.Generic;

using BomCore;

using Inventor;

using InvUtils;

using BlankType = DescriptionManager.Blanks.BlankType;

using Prop = CadModelProperties.Resources.ResIproperties;

using System.Linq;

namespace WConnectorModels
{

    public class CalculaMateriaPrima
    {
        //public IEnumerable<PartDocument> partDocumentsList;

        /// <summary>
        /// Define valores de matéria prima para a peça
        /// </summary>
        public void Calcular(Document partDocument)
        {
            // Documento atual e Massa
            PartDocument partDoc = (PartDocument) partDocument;
            MassProperties mass = partDoc.ComponentDefinition.MassProperties;

            // Cria um BomData para coletar os dados necessários para Cálculo
            BomData bc = new BomData
            {
                Blank = InvProps.GetInventorCustomProperties(Prop.CH_DIM_BLANK,partDoc),
                Fator = InvProps.GetInventorCustomProperties(Prop.MP_FATOR, partDoc),
                PesoAcabado = Math.Round(Convert.ToDouble(mass.Mass), 2)
            };

            // Get dos valores atuais
            // Peso Bruto
            if (InvProps.GetInventorCustomProperties(Prop.BLANK_TYPE,partDocument) == BlankType.SheetMetal)
            {
                bc.BlankType = BomCore.BlankType.SheetMetal;
            }
            if (InvProps.GetInventorCustomProperties(Prop.BLANK_TYPE, partDocument) == BlankType.Cylinder)
            {
                bc.BlankType = BomCore.BlankType.Cylinder;
            }
            if (InvProps.GetInventorCustomProperties(Prop.BLANK_TYPE, partDocument) == BlankType.Linear)
            {
                bc.BlankType = BomCore.BlankType.Linear;
            }

            bc.PesoBruto = BomCalc.Calcule(bc,partDocument);

            InvProps.SetInvIProperties(Prop.PC_PESO_ACABADO, $"{bc.PesoAcabado} KG", InvPropetiesGroup.CustomFields,partDocument);
            InvProps.SetInvIProperties(Prop.MP_PESO_BRUTO, $"{bc.PesoBruto} KG", InvPropetiesGroup.CustomFields, partDocument);
            InvProps.SetInvIProperties(Prop.CH_DIM_BLANK, bc.Blank, InvPropetiesGroup.CustomFields, partDocument);

            // Peso Bruto
            //TxtBox.SetToTextBox(txtProp_DT_PesoBruto, bc.PesoBruto + " KG");

            // Blank
            //TxtBox.SetToTextBox(txtProp_DT_DimBlank, bc.Blank);
             
            // Peso Acabado
            //TxtBox.SetToTextBox(txtProp_DT_PesoAcabado, bc.PesoAcabado + " KG");

            // Salva nas propriedades
            //InvProps.SetInvIProperties(Prop.PC_PESO_ACABADO, txtProp_DT_PesoAcabado.Text, InvPropetiesGroup.CustomFields);
            //InvProps.SetInvIProperties(Prop.MP_PESO_BRUTO, txtProp_DT_PesoBruto.Text, InvPropetiesGroup.CustomFields);

        }
        
        public BomData CalcularAudit(Document partDocument)
        {
            if (partDocument.DocumentType == DocumentTypeEnum.kAssemblyDocumentObject)
            {
                return null;
            }
            // Documento atual e Massa
            PartDocument partDoc = (PartDocument)partDocument;
            MassProperties mass = partDoc.ComponentDefinition.MassProperties;

            // Cria um BomData para coletar os dados necessários para Cálculo
            BomData bc = new BomData
            {
                Blank = InvProps.GetInventorCustomProperties(Prop.CH_DIM_BLANK, partDoc),
                Fator = InvProps.GetInventorCustomProperties(Prop.MP_FATOR, partDoc),
                PesoAcabado = Math.Round(Convert.ToDouble(mass.Mass), 2)
            };

            // Get dos valores atuais
            // Peso Bruto
            if (InvProps.GetInventorCustomProperties(Prop.BLANK_TYPE, partDocument) == BlankType.SheetMetal)
            {
                bc.BlankType = BomCore.BlankType.SheetMetal;
            }
            if (InvProps.GetInventorCustomProperties(Prop.BLANK_TYPE, partDocument) == BlankType.Cylinder)
            {
                bc.BlankType = BomCore.BlankType.Cylinder;
            }
            if (InvProps.GetInventorCustomProperties(Prop.BLANK_TYPE, partDocument) == BlankType.Linear)
            {
                bc.BlankType = BomCore.BlankType.Linear;
            }

            bc.PesoBruto = BomCalc.Calcule(bc, partDocument);

            return bc;

            //InvProps.SetInvIProperties(Prop.PC_PESO_ACABADO, $"{bc.PesoAcabado} KG", InvPropetiesGroup.CustomFields, partDocument);
            //InvProps.SetInvIProperties(Prop.MP_PESO_BRUTO, $"{bc.PesoBruto} KG", InvPropetiesGroup.CustomFields, partDocument);
            //InvProps.SetInvIProperties(Prop.CH_DIM_BLANK, bc.Blank, InvPropetiesGroup.CustomFields, partDocument);

            // Peso Bruto
            //TxtBox.SetToTextBox(txtProp_DT_PesoBruto, bc.PesoBruto + " KG");

            // Blank
            //TxtBox.SetToTextBox(txtProp_DT_DimBlank, bc.Blank);

            // Peso Acabado
            //TxtBox.SetToTextBox(txtProp_DT_PesoAcabado, bc.PesoAcabado + " KG");

            // Salva nas propriedades
            //InvProps.SetInvIProperties(Prop.PC_PESO_ACABADO, txtProp_DT_PesoAcabado.Text, InvPropetiesGroup.CustomFields);
            //InvProps.SetInvIProperties(Prop.MP_PESO_BRUTO, txtProp_DT_PesoBruto.Text, InvPropetiesGroup.CustomFields);

        }


        /// <summary>
        /// Processa Lista e retorna o nome dos itens processados
        /// </summary>
        /// <param name="partDocuments"></param>
        /// <returns></returns>
        //public IEnumerable<string> ProcessaLista(IEnumerable<PartDocument> partDocuments)
        //{
        //    foreach (PartDocument partDocument in partDocuments)
        //    {
        //        Calcular(partDocument);
        //        yield return partDocument.FullDocumentName;
        //    }

        //}

    }

}