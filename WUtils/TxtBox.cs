using System.Windows.Forms;

namespace WUtils
{
    public static class TxtBox
    {
        /// <summary>
        ///     Preenche o TextBox de uma string
        /// </summary>
        /// <param name="textbox"></param>
        /// <param name="valor"></param>
        public static void SetToTextBox(TextBox textbox, string valor)
        {
            textbox.Text = valor.Replace('.', ',').Trim();
        }

        /// <summary>
        ///     Preeche variável informada vindo de um textBox
        /// </summary>
        /// <param name="valor"></param>
        /// <param name="textbox"></param>
        public static void GetFromTextBox(TextBox textbox, string valor)
        {
            valor = textbox.Text.Replace('.', ',').Trim();
        }
        /// <summary>
        /// Controla o status do TextBox
        /// </summary>
        /// <param name="txt">TextBox Object</param>
        /// <param name="state">Status: True = Write : False = ReadOnly</param>
        public static void TextBoxStatus(TextBox txt, bool state)
        {
            if (state)
            {
                txt.ReadOnly = false;
                txt.BackColor = System.Drawing.Color.FromArgb(192, 255, 192);
            }
            else
            {
                txt.ReadOnly = true;
                txt.BackColor = System.Drawing.SystemColors.Control;
            }

        }

        /// <summary>
        /// Retorna o endereço de uma pasta selecionada
        /// </summary>
        /// <returns></returns>
        public static string ChooseFolder()
        {
            var folderBrowser = new FolderBrowserDialog();
            return folderBrowser.ShowDialog() == DialogResult.OK ? (folderBrowser.SelectedPath + "\\") : string.Empty;
        }

    }
}