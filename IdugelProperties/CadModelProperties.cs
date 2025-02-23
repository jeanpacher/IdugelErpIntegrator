using System;
using CadModelProperties.Resources;
using Inventor;
using InvUtils;
using WUtils;

//using Prop = CadProperties.Resources.ResIproperties;

namespace CadProperties
{

    public class CadModelProperties : ICadModelProperties
    {

        #region Dados do Modelo 3d

        public double Thickness { get; set; }

        #endregion

        public CadModelProperties GetAllCadModelProperties()
        {
            try
            {
                // Matéria Prima
                //
                MpCod = InvProps.GetInventorCustomProperties(ResIproperties.MP_COD);
                MpFamilia = InvProps.GetInventorCustomProperties(ResIproperties.MP_FAMILIA);
                MpDescErp = InvProps.GetInventorCustomProperties(ResIproperties.MP_DESC_ERP);
                MpDescInventor = InvProps.GetInventorCustomProperties(ResIproperties.MP_DESC_INVENTOR);
                MpUnidade = InvProps.GetInventorCustomProperties(ResIproperties.MP_UNIDADE);
                MpFator = InvProps.GetInventorCustomProperties(ResIproperties.MP_FATOR);
                MpCustom = InvProps.GetInventorCustomProperties(ResIproperties.MP_CUSTOM);

                //
                // Dados Equipamento
                //
                MqCodigo = InvProps.GetInventorCustomProperties(ResIproperties.MQ_COD);
                MqNome = InvProps.GetInventorCustomProperties(ResIproperties.MQ_NOME);

                //
                // Dados Peça
                //
                PcCodDesenho = InvProps.GetInventorCustomProperties(ResIproperties.PC_COD_DESENHO);
                PcDesc = InvProps.GetInventorCustomProperties(ResIproperties.PC_DESC);
                PcDescCompleta = InvProps.GetInventorCustomProperties(ResIproperties.PC_DESC_COMPLETA);
                PcDescCompletaCustom = InvProps.GetInventorCustomProperties(ResIproperties.PC_DESC_COMPLETA_CUSTOM);

                PcPesoBruto = InvProps.GetInventorCustomProperties(ResIproperties.MP_PESO_BRUTO);
                PcNumRev = InvProps.GetInventorProperties("Revision Number",
                    InvPropetiesGroup.InventorSumaryInformation);

                //
                // Dados Técnicos
                //
                DtPesoAcabado = InvProps.GetInventorCustomProperties(ResIproperties.PC_PESO_ACABADO);
                PcDimBlank = InvProps.GetInventorCustomProperties(ResIproperties.CH_DIM_BLANK);
                PcPesoBruto = InvProps.GetInventorCustomProperties(ResIproperties.MP_PESO_BRUTO);
                BlankType = InvProps.GetInventorCustomProperties(ResIproperties.BLANK_TYPE);
                PcPinturaTipo = InvProps.GetInventorCustomProperties(ResIproperties.PC_TIPO_PINTURA);
                PcPinturaArea = InvProps.GetInventorCustomProperties(ResIproperties.PC_AREA_PINTURA);
                PcPinturaAcabamento = InvProps.GetInventorCustomProperties(ResIproperties.PC_ACABAMENTO_PINTURA);
                PcPinturaInternaDiferente = InvProps.GetInventorCustomProperties(ResIproperties.PC_PINTURA_INTERNA_DIFERENTE);

                //
                // Responsável do Projeto
                // 
                RpNomeProjetista = InvProps.GetInventorCustomProperties(ResIproperties.NOME_PROJETISTA);
                RpNomeDesenhista = InvProps.GetInventorCustomProperties(ResIproperties.NOME_DESENHISTA);
                RpNomeAprovador = InvProps.GetInventorCustomProperties(ResIproperties.NOME_APROVADOR);

                return this;
            }
            catch (Exception e)
            {
                CoreLog.WriteLog($"Erro para Pegar Propriedades\n\t{e}");
                return null;
            }
        }

        public CadModelProperties GetAllCadModelProperties(Document oDoc)
        {
            try
            {
                // Matéria Prima
                //
                MpCod = InvProps.GetInventorCustomProperties(ResIproperties.MP_COD, oDoc);
                MpFamilia = InvProps.GetInventorCustomProperties(ResIproperties.MP_FAMILIA, oDoc);
                MpDescErp = InvProps.GetInventorCustomProperties(ResIproperties.MP_DESC_ERP, oDoc);
                MpDescInventor = InvProps.GetInventorCustomProperties(ResIproperties.MP_DESC_INVENTOR, oDoc);
                MpUnidade = InvProps.GetInventorCustomProperties(ResIproperties.MP_UNIDADE, oDoc);
                MpFator = InvProps.GetInventorCustomProperties(ResIproperties.MP_FATOR, oDoc);
                MpCustom = InvProps.GetInventorCustomProperties(ResIproperties.MP_CUSTOM, oDoc);

                //
                // Dados Equipamento
                //
                MqCodigo = InvProps.GetInventorCustomProperties(ResIproperties.MQ_COD, oDoc);
                MqNome = InvProps.GetInventorCustomProperties(ResIproperties.MQ_NOME, oDoc);

                //
                // Dados Peça
                //
                PcCodDesenho = InvProps.GetInventorCustomProperties(ResIproperties.PC_COD_DESENHO, oDoc);
                PcDesc = InvProps.GetInventorCustomProperties(ResIproperties.PC_DESC, oDoc);
                PcDescCompleta = InvProps.GetInventorCustomProperties(ResIproperties.PC_DESC_COMPLETA, oDoc);
                PcDescCompletaCustom = InvProps.GetInventorCustomProperties(ResIproperties.PC_DESC_COMPLETA_CUSTOM, oDoc);

                PcDescCompletaCustom = InvProps.GetInventorCustomProperties(ResIproperties.PC_DESC_COMPLETA_CUSTOM);
                PcPesoBruto = InvProps.GetInventorCustomProperties(ResIproperties.MP_PESO_BRUTO, oDoc);
                PcNumRev =
                    InvProps.GetInventorProperties("Revision Number", InvPropetiesGroup.InventorSumaryInformation, oDoc);

                //
                // Dados Técnicos
                //
                DtPesoAcabado = InvProps.GetInventorCustomProperties(ResIproperties.PC_PESO_ACABADO, oDoc);
                PcDimBlank = InvProps.GetInventorCustomProperties(ResIproperties.CH_DIM_BLANK, oDoc);
                PcPesoBruto = InvProps.GetInventorCustomProperties(ResIproperties.MP_PESO_BRUTO, oDoc);
                BlankType = InvProps.GetInventorCustomProperties(ResIproperties.BLANK_TYPE, oDoc);
                PcPinturaTipo = InvProps.GetInventorCustomProperties(ResIproperties.PC_TIPO_PINTURA, oDoc);
                PcPinturaArea = InvProps.GetInventorCustomProperties(ResIproperties.PC_AREA_PINTURA, oDoc);
                PcPinturaAcabamento = InvProps.GetInventorCustomProperties(ResIproperties.PC_ACABAMENTO_PINTURA, oDoc);
                PcPinturaInternaDiferente = InvProps.GetInventorCustomProperties(ResIproperties.PC_PINTURA_INTERNA_DIFERENTE, oDoc);

                //
                // Responsável do Projeto
                // 
                RpNomeProjetista = InvProps.GetInventorCustomProperties(ResIproperties.NOME_PROJETISTA, oDoc);
                RpNomeDesenhista = InvProps.GetInventorCustomProperties(ResIproperties.NOME_DESENHISTA, oDoc);
                RpNomeAprovador = InvProps.GetInventorCustomProperties(ResIproperties.NOME_APROVADOR, oDoc);
                return this;
            }
            catch (Exception e)
            {
                CoreLog.WriteLog($"Erro para Pegar Propriedades\n\t{e}");
                return null;
            }
        }


        #region Dados das Propriedades

        //
        // Matéria Prima
        //
        public string MpCod { get; set; }

        public string MpDescErp { get; set; }

        public string MpDescInventor { get; set; }

        public string MpFamilia { get; set; }

        public string MpUnidade { get; set; }

        public string MpFator { get; set; }

        //
        // Código Máquina
        //
        public string MqCodigo { get; set; }

        public string MqNome { get; set; }

        #endregion


        #region Dados Peça

        //public string PcNome { get; set; }

        //public string PcNumDesenho { get; set; }

        public string PcCodDesenho { get; set; }

        public string PcDesc { get; set; }

        public string PcPesoBruto { get; set; }

        public string PcDescCompleta { get; set; }

        public string PcDescCompletaCustom { get; set; }

        #endregion


        #region Dados Técnicos

        public string DtPesoAcabado { get; set; }

        public string PcDimBlank { get; set; }

        #endregion

        #region Resp. Projeto

        public string RpNomeDesenhista { get; set; }

        public string RpNomeProjetista { get; set; }

        public string RpNomeAprovador { get; set; }

        public string PcNumRev { get; set; }

        public string BlankType { get; set; }

        public string MpCustom { get; set; }

        #endregion

        #region Pintura da Peça

        public string PcPinturaTipo { get; set; }

        public string PcPinturaArea { get; set; }

        public string PcPinturaAcabamento { get; set; }

        public string PcPinturaInternaDiferente { get; set; }

        #endregion

    }

}