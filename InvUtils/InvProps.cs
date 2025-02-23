using System;

using Inventor;

using WUtils;

namespace InvUtils
{
    public static class InvProps
    {
        public static void SetInvIProperties(string nomePropriedade, string valorPropriedade, string iPropetiesGroup)
        {
            var invDoc = InvDocs.InvDoc();

            var propertySet = invDoc.PropertySets[iPropetiesGroup];

            Property propriedade = null;

            var propExists = true;
            if (iPropetiesGroup == InvPropetiesGroup.CustomFields)
                try
                {
                    propriedade = propertySet[nomePropriedade];
                }
                catch
                {
                    propExists = false;
                }

            if (!propExists)
            {
                
                propriedade = propertySet.Add(valorPropriedade, nomePropriedade, null);
                
            }
            else
            {
                propriedade = propertySet[nomePropriedade];

                if (propriedade.Value.ToString() != valorPropriedade)
                {
                    propriedade.Value = valorPropriedade;
                }
            }
        }

        public static void SetInvIProperties(string nomePropriedade, string valorPropriedade, string iPropetiesGroup,
            Document document)
        {
            try
            {
                var invDoc = document;

                var propertySet = invDoc.PropertySets[iPropetiesGroup];

                Property propriedade = null;

                var propExists = true;
                if (iPropetiesGroup == InvPropetiesGroup.CustomFields)
                    try
                    {
                        propriedade = propertySet[nomePropriedade];
                    }
                    catch
                    {
                        propExists = false;
                    }

                if (!propExists)
                {
                    
                    propriedade = propertySet.Add(valorPropriedade, nomePropriedade, null);

                }
                else
                {
                    propriedade = propertySet[nomePropriedade];
                    
                    if (propriedade.Value.ToString() != valorPropriedade)
                    {
                        propriedade.Value = valorPropriedade;
                    }
                }
            }
            catch (Exception e)
            {
                CoreLog.WriteLog($"Erro: SetIProperties \n{e.Message}\n<----->");
                InvMsg.Msg($"A inserção das propriedades no arquivo \"{document.DisplayName}\" não foi inserida.");
            }
            

        }

        /// <summary>
        ///     Pega a propriedade do arquivo ativo
        /// </summary>
        /// <param name="nomePropriedade"></param>
        /// <returns></returns>
        public static string GetInventorCustomProperties(string nomePropriedade)
        {
            var invDoc = InvDocs.InvDoc();

            var propertySet = invDoc.PropertySets["Inventor User Defined Properties"];

            Property propriedadeCustom = null;

            var propExists = true;
            try
            {
                propriedadeCustom = propertySet[nomePropriedade];
            }
            catch
            {
                propExists = false;
            }

            if (propExists) return propriedadeCustom.Value.ToString();

            if (nomePropriedade == "MP_CUSTOM" || nomePropriedade == "PC_DESC_COMPLETA_CUSTOM")
                propriedadeCustom = propertySet.Add("0", nomePropriedade, null);
            else
                propriedadeCustom = propertySet.Add(string.Empty, nomePropriedade, null);

            return propriedadeCustom.Value.ToString();
        }
        
        /// <summary>
        ///     Pega a propriedade do arquivo ativo
        /// </summary>
        /// <param name="nomePropriedade"></param>
        /// <returns></returns>
        public static int GetInventorCustomPropertiesInt(string nomePropriedade)
        {
            var invDoc = InvDocs.InvDoc();

            var propertySet = invDoc.PropertySets["Inventor User Defined Properties"];

            Property propriedadeCustom = null;

            var propExists = true;
            try
            {
                propriedadeCustom = propertySet[nomePropriedade];
            }
            catch
            {
                propExists = false;
            }

            if (!propExists) propriedadeCustom = propertySet.Add(0, nomePropriedade, null);

            return (int) propriedadeCustom.Value;
        }

        /// <summary>
        ///     Pega a propriedade do arquivo ativo
        /// </summary>
        /// <param name="nomePropriedade"></param>
        /// <returns></returns>
        public static bool GetInventorCustomPropertiesBool(string nomePropriedade)
        {
            var invDoc = InvDocs.InvDoc();

            var propertySet = invDoc.PropertySets["Inventor User Defined Properties"];

            Property propriedadeCustom = null;

            var propExists = true;
            try
            {
                propriedadeCustom = propertySet[nomePropriedade];
            }
            catch
            {
                propExists = false;
            }

            if (!propExists) propriedadeCustom = propertySet.Add(0, nomePropriedade, null);

            return (bool) propriedadeCustom.Value;
        }
        
        /// <summary>
        ///     Pega a propriedade do arquivo indicado
        /// </summary>
        /// <param name="nomePropriedade"></param>
        /// <param name="partDoc"></param>
        /// <returns></returns>
        public static string GetInventorCustomProperties(string nomePropriedade, PartDocument partDoc)
        {
            var propertySet = partDoc.PropertySets["Inventor User Defined Properties"];

            Property propriedadeCustom = null;

            var propExists = true;
            try
            {
                propriedadeCustom = propertySet[nomePropriedade];
            }
            catch
            {
                propExists = false;
            }

            if (propExists) return propriedadeCustom.Value.ToString();

            if (nomePropriedade == "MP_CUSTOM" || nomePropriedade == "PC_DESC_COMPLETA_CUSTOM")
                propriedadeCustom = propertySet.Add("0", nomePropriedade, null);
            else
                propriedadeCustom = propertySet.Add(string.Empty, nomePropriedade, null);

            return propriedadeCustom.Value.ToString();
        }

        /// <summary>
        ///     Pega a propriedade do arquivo indicado
        /// </summary>
        /// <param name="nomePropriedade"></param>
        /// <param name="invDoc"></param>
        /// <returns></returns>
        public static string GetInventorCustomProperties(string nomePropriedade, Document invDoc)
        {
            var propertySet = invDoc.PropertySets["Inventor User Defined Properties"];

            Property propriedadeCustom = null;

            var propExists = true;
            try
            {
                propriedadeCustom = propertySet[nomePropriedade];
            }
            catch
            {
                propExists = false;
            }

            if (propExists) return propriedadeCustom.Value.ToString();

            try
            {
                propriedadeCustom = propertySet.Add(string.Empty, nomePropriedade, null);
            }
            catch (Exception ex)
            {
                //CoreLog.WriteLog($"Erro ao recuperar a propriedade: {nomePropriedade}.\n\t" +
                //                 $"Documento: {invDoc.DisplayName}\n\t" +
                //                 $"Tipo Documento: {invDoc.DocumentType}\n\t" +
                //                 $"{ex}");
                return string.Empty;
            }

            return propriedadeCustom.Value.ToString();
        }

        public static string GetInventorProperties(string nomePropriedade, string grupoPropriedades)
        {
            var invDoc = InvDocs.InvDoc();

            var propertySet = invDoc.PropertySets[grupoPropriedades];

            Property propriedadeCustom = null;

            var propExists = true;
            try
            {
                propriedadeCustom = propertySet[nomePropriedade];
            }
            catch
            {
                propExists = false;
            }


            if (!propExists) propriedadeCustom = propertySet.Add(string.Empty, nomePropriedade, null);


            return propriedadeCustom.Value.ToString();
        }

        public static string GetInventorProperties(string nomePropriedade, string grupoPropriedades, Document oDoc)
        {
            //Document invDoc = InvDocs.InvDoc();

            var propertySet = oDoc.PropertySets[grupoPropriedades];

            Property propriedadeCustom = null;

            var propExists = true;
            try
            {
                propriedadeCustom = propertySet[nomePropriedade];
            }
            catch
            {
                propExists = false;
            }
            try
            {
                if (!propExists) propriedadeCustom = propertySet.Add(string.Empty, nomePropriedade, null);
            }
            catch
            {
                //CoreLog.WriteLog($"Erro ao recuperar a Propriedade: {nomePropriedade}");
                return string.Empty;
            }

            return propriedadeCustom.Value.ToString();
        }
        
    }

}