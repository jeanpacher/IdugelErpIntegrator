using System;
using System.Collections.Generic;
using System.Linq;

using AppResources;

using InvApp;

using Inventor;

using InvUtils;

using WUtils;

namespace PipeManager
{

    /// <summary>
    ///     Representa um item de tubo e seu dados
    /// </summary>
    public class Tubulacao
    {

        /// <summary>
        ///     Lista de Tubos
        /// </summary>
        public List<ITubo> TubosList;

        /// <summary>
        ///     Grupo de Tubos por Tipo
        /// </summary>
        /// public IEnumerable
        /// <IGrouping
        /// <string, ITubo>> TubosGroup;
        public Tubulacao()
        {
            TubosList = new List<ITubo>();
        }

        /// <summary>
        ///     Adiciona um tubo a lista
        /// </summary>
        /// <param name="tubo"></param>
        public void AddTubo(ITubo tubo)
        {
            TubosList.Add(tubo);
        }

        /// <summary>
        ///     Calcula todos os tubos da montagem
        /// </summary>
        public void Calculate()
        {
            var pipeList = GetPipeList();

            var group = pipeList.GroupBy(c => c.GetType());

            foreach (var tubo in group)
            {
                //if (tubo.Key)
                //{

                //}

                //foreach (var tubo1 in tubo)
                //{
                //    CoreLog.WriteLog($"Tubo: {tubo1.}.");


                //}
            }
        }

        /// <summary>
        ///     Recupera a Lista de Tubos da Montagem Ativa
        /// </summary>
        /// <returns></returns>
        private List<ITubo> GetPipeList()
        {
            try
            {
                var oAssyDoc = (AssemblyDocument)InvApp.StandardAddInServer.m_InvApp.ActiveEditDocument; 

                var result = oAssyDoc.ComponentDefinition.RepresentationsManager.ActiveLevelOfDetailRepresentation.Name;

                if (result != "Master")
                {
                    //MessageBox.Show(@"É necessário ativar o Level Master em Level of Details. " + "\nLevel atual: " + result);
                    return null;
                }

                var bom = oAssyDoc.ComponentDefinition.BOM;

                bom.PartsOnlyViewEnabled = true;
                bom.PartsOnlyViewMinimumDigits = 1;

                var bomView = bom.BOMViews["Parts Only"];

                var bomRowsEnum = bomView.BOMRows;

                var partList = GetPartListPartsOnlyFactory(bomRowsEnum, TubosList);

                return partList;
            }
            catch (Exception e)
            {
                CoreLog.WriteLog($"Erro ao calcular o PartList - PartListOnly.\n\t{e}");
                return null;
            }
        }

        private List<ITubo> GetPartListPartsOnlyFactory(BOMRowsEnumerator bomRowsEnum, List<ITubo> listPartList)
        {
            foreach (BOMRow bomRow in bomRowsEnum)
            {
                var oCompDef = bomRow.ComponentDefinitions;

                var oCompDefEnum = oCompDef[1];

                var doc = (Document) oCompDefEnum.Document;

                InvApp.StandardAddInServer.m_InvApp.StatusBarText = $"Analisando: {doc.DisplayName}";

                var partListFields = PipeTypeRouter.PartListRouter(bomRow, doc);

                listPartList.Add(partListFields);

                if (bomRow.ChildRows != null)
                    GetPartListPartsOnlyFactory(bomRow.ChildRows, listPartList);
            }

            return listPartList;
        }

    }

    /// <summary>
    ///     Responsável pelo roteamento dos tipos de tubo
    /// </summary>
    public static class PipeTypeRouter
    {
        /// <summary>
        ///     Faz o roteamento do tipo de tubo e preenche os dados
        /// </summary>
        /// <param name="bomRow"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public static ITubo PartListRouter(BOMRow bomRow, Document document)
        {
            if (document.DocumentType == DocumentTypeEnum.kPartDocumentObject)
            {
                var param = InvParameter.GetParameterText(ParameterNames.NOME_TIPO, document);

                switch (param)
                {
                    case PipeNameTypes.TuboConduto:
                        CoreLog.WriteLog($"{document.DisplayName}: {param}");
                        return GetTuboConduto(bomRow, document);
                    case PipeNameTypes.TuboGrampeado:
                        CoreLog.WriteLog($"{document.DisplayName}: {param}");
                        break;
                    case PipeNameTypes.TuboMecanico:
                        CoreLog.WriteLog($"{document.DisplayName}: {param}");
                        break;
                    case PipeNameTypes.JoelhoArticulado:
                        CoreLog.WriteLog($"{document.DisplayName}: {param}");
                        break;
                    case PipeNameTypes.JoelhoTubo:
                        CoreLog.WriteLog($"{document.DisplayName}: {param}");
                        break;
                    case PipeNameTypes.JoelhoCustom:
                        CoreLog.WriteLog($"{document.DisplayName}: {param}");
                        break;
                }
            }

            return null;
        }

        private static ITubo GetTuboConduto(BOMRow bomRow, Document document)
        {
            var tuboConduto = new TuboConduto();

            tuboConduto = GetBaseData<TuboConduto>(tuboConduto, bomRow, document);

            tuboConduto.Comprimento = InvParameter.GetParameter(ParameterNames.COMP, document).ToDouble();

            //tuboConduto.DiaNominal = 

            return tuboConduto;
        }

        /// <summary>
        /// Preenchimento base para todos os tipos de Tubo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tubo"></param>
        /// <param name="bomRow"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        private static T GetBaseData<T>(ITubo tubo, BOMRow bomRow, Document document)
        {
            tubo.NomeTipo = InvParameter.GetParameterText(ParameterNames.NOME_TIPO, document);
            tubo.DiaNominal = InvParameter.GetParameter(ParameterNames.NOME_TIPO, document).ToDouble();
            tubo.Imagem = GetImagem(tubo.NomeTipo);
            tubo.Item = bomRow.ItemNumber;
            
            return (T) tubo;
        }

        /// <summary>
        /// Retorna o Índice para a Imagem
        /// </summary>
        /// <param name="nomeTipo"></param>
        /// <returns></returns>
        private static int GetImagem(string nomeTipo)
        {
            switch (nomeTipo)
            {
                case PipeNameTypes.TuboConduto:
                    return 0;
                    
                case PipeNameTypes.TuboGrampeado:
                    return 1;
                    
                case PipeNameTypes.TuboMecanico:
                    return 2;
                    
                case PipeNameTypes.JoelhoArticulado:
                    return 3;
                   
                case PipeNameTypes.JoelhoTubo:
                    return 4;
                    
                case PipeNameTypes.JoelhoCustom:
                    return 5;
                    
            }

            return -1;
        }
    }

    /// <summary>
    ///     Tipos de Tubos
    /// </summary>
    public abstract class PipeNameTypes
    {
        public const string TuboConduto = "TUBO CONDUTO";
        public const string TuboGrampeado = "TUBO GRAMPEADO";
        public const string TuboMecanico = "TUBO MECÂNICO";
        public const string JoelhoArticulado = "JOELHO ARTICULADO";
        public const string JoelhoTubo = "JOELHO TUBO";
        public const string JoelhoCustom = "JOELHO CUSTOM";
    }

    /// <summary>
    ///     Dados refetente ao Orçamento
    /// </summary>
    public interface IErpData
    {
        string Cliente { get; set; }

        string Orcamento { get; set; }

        string Of { get; set; }

        string Item { get; set; }
    }

    /// <summary>
    ///     Interface para Modelos de Tubos
    /// </summary>
    public interface ITubo : IErpData, IPipeTecData
    {
        int Imagem { get; set; }

        string NomeTipo { get; set; }

        double DiaNominal { get; set; }

        string Observacoes { get; set; }
    }

    /// <summary>
    ///     Dados Técnicos do tubo, Material, Peso, etc.
    /// </summary>
    public interface IPipeTecData
    {
        double Peso { get; set; }

        double Area { get; set; }

        double Espessura { get; set; }

        double Material { get; set; }
    }

    /// <summary>
    ///     Tubo Base, padrão
    /// </summary>
    public abstract class Tubo : ITubo
    {
        public int Imagem { get; set; }

        public string NomeTipo { get; set; }

        public double DiaNominal { get; set; }

        public string Observacoes { get; set; }

        public string Cliente { get; set; }

        public string Orcamento { get; set; }

        public string Of { get; set; }

        public string Item { get; set; }

        public double Peso { get; set; }

        public double Area { get; set; }

        public double Espessura { get; set; }

        public double Material { get; set; }

    }

    /// <summary>
    ///     Ennum das pontas do Tubo
    /// </summary>
    public enum PontasTubo
    {
        VirolaAnel = 1,
        PontaLisa = 2,
        Grampeado = 3
    }

    /// <summary>
    ///     Dados referente a Tubos
    /// </summary>
    public interface ITuboData
    {

        double DiaNominal { get; set; }

        double Comprimento { get; set; }

    }

    /// <summary>
    ///     Tipo de tubo
    /// </summary>
    public abstract class TuboCalandrado : Tubo, ITuboData
    {

        public virtual PontasTubo TuboPontaA { get; set; }

        public virtual PontasTubo TuboPontaB { get; set; }

        public double Perimetro { get; set; }

        //public double DiaNominal { get; set; }

        public double Comprimento { get; set; }

    }

    /// <summary>
    ///     Tipo de Tubo
    /// </summary>
    public class TuboConduto : TuboCalandrado
    {

    }

    /// <summary>
    ///     Tipo de Tubo
    /// </summary>
    public class TuboGrampeado : TuboCalandrado
    {

    }

    /// <summary>
    ///     Transição Simples
    /// </summary>
    public class TransicaoSimples : Tubo
    {

    }

    /// <summary>
    ///     Tipo de Tubo Comercial
    /// </summary>
    public class TuboMecanico : Tubo
    {

    }

}