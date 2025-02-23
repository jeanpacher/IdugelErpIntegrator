namespace DescriptionManager
{
    public interface ICoreDescription
    {
        /// <summary>
        ///     Retorna doi campos do banco
        /// </summary>
        /// <param name="familyName"></param>
        /// <returns>Coluna: ScriptDesc</returns>
        string GetDescriptionScript(string familyName);
    }
}