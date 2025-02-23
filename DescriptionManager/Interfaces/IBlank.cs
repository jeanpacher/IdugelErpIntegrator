using Inventor;

namespace DescriptionManager
{
    public interface IBlank
    {
        void Calculation();
        void Calculation(Document oDoc, bool message = true);
    }
}