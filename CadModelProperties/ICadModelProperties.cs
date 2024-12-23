namespace CadProperties
{
    /// <summary>
    /// Interface de Propriedades Idugel
    /// </summary>
    public interface ICadModelProperties
    {
        string MpCustom { get; set; }
        string MpCod { get; set; }
        string MpDescErp { get; set; }
        string MpDescInventor { get; set; }
        string MpFamilia { get; set; }
        string MpUnidade { get; set; }
        string MpFator { get; set; }
        string MqCodigo { get; set; }
        string MqNome { get; set; }

        string PcCodDesenho { get; set; }
        string PcDesc { get; set; }
        string PcPesoBruto { get; set; }
        string PcDescCompleta { get; set; }
        string DtPesoAcabado { get; set; }
        string PcDimBlank { get; set; }
        string RpNomeDesenhista { get; set; }
        string RpNomeProjetista { get; set; }
        string RpNomeAprovador { get; set; }
        string PcNumRev { get; set; }
        string BlankType { get; set; }
        string PcDescCompletaCustom { get; set; }
        string PcPinturaTipo { get; set; }
        string PcPinturaArea { get; set; }
        string PcPinturaAcabamento { get; set; }
        string PcPinturaInternaDiferente { get; set; }

    }
}