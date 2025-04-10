using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace InvAddIn
{
    public class CustomMessageBox : Form
    {
        private Label lblTitle;
        private ListBox lstErrors;
        private Button btnYes;
        private Button btnNo;
        private PictureBox pictureBoxIcon;
        private DialogResult dialogResult;

        public CustomMessageBox(List<string> errors, string title)
        {
            this.Text = title;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(420, 250);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // Ícone
            pictureBoxIcon = new PictureBox
            {
                Image = SystemIcons.Warning.ToBitmap(),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = new Point(10, 10),
                Size = new Size(30, 30)
            };

            // Título da mensagem
            lblTitle = new Label
            {
                Text = "Erros Encontrados:",
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.Red,
                Location = new Point(60, 15),
                Size = new Size(300, 20)
            };

            // Lista de erros
            lstErrors = new ListBox
            {
                Location = new Point(20, 50),
                Size = new Size(360, 100),
                Font = new Font("Arial", 9),
                ForeColor = Color.DarkRed
            };
            foreach (var error in errors)
            {
                lstErrors.Items.Add("• " + error);
            }

            // Botão "Sim"
            btnYes = new Button
            {
                Text = "Sim",
                DialogResult = DialogResult.Yes,
                Location = new Point(100, 170),
                Size = new Size(90, 30),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat
            };
            btnYes.Click += (sender, e) => { dialogResult = DialogResult.Yes; this.Close(); };

            // Botão "Não"
            btnNo = new Button
            {
                Text = "Não",
                DialogResult = DialogResult.No,
                Location = new Point(220, 170),
                Size = new Size(90, 30),
                BackColor = Color.LightCoral,
                FlatStyle = FlatStyle.Flat
            };
            btnNo.Click += (sender, e) => { dialogResult = DialogResult.No; this.Close(); };

            // Adicionar componentes ao formulário
            this.Controls.Add(pictureBoxIcon);
            this.Controls.Add(lblTitle);
            this.Controls.Add(lstErrors);
            this.Controls.Add(btnYes);
            this.Controls.Add(btnNo);
        }


        public static DialogResult ShowCustomMessage(List<string> errors, string title)
        {
            CustomMessageBox messageBox = new CustomMessageBox(errors, title);
            messageBox.ShowDialog();
            return messageBox.dialogResult;
        }
    }
    public class CustomMessageBox_old : Form
    {
        private Label lblMessage;
        private Button btnYes;
        private Button btnNo;
        private PictureBox pictureBoxIcon;
        private DialogResult dialogResult;

        public CustomMessageBox_old(string message, string title)
        {
            this.Text = title;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(400, 200);
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Ícone
            pictureBoxIcon = new PictureBox
            {
                Image = SystemIcons.Warning.ToBitmap(),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = new Point(10, 30),
                Size = new Size(40, 40)
            };

            // Label com a mensagem
            lblMessage = new Label
            {
                Text = message,
                Location = new Point(60, 20),
                Size = new Size(300, 80),
                AutoSize = false
            };

            // Botão "Sim"
            btnYes = new Button
            {
                Text = "Sim",
                DialogResult = DialogResult.Yes,
                Location = new Point(100, 120),
                Size = new Size(80, 30)
            };
            btnYes.Click += (sender, e) => { dialogResult = DialogResult.Yes; this.Close(); };

            // Botão "Não"
            btnNo = new Button
            {
                Text = "Não",
                DialogResult = DialogResult.No,
                Location = new Point(200, 120),
                Size = new Size(80, 30)
            };
            btnNo.Click += (sender, e) => { dialogResult = DialogResult.No; this.Close(); };

            // Adicionar componentes ao formulário
            this.Controls.Add(pictureBoxIcon);
            this.Controls.Add(lblMessage);
            this.Controls.Add(btnYes);
            this.Controls.Add(btnNo);
        }


        public static DialogResult ShowCustomMessage(string message, string title)
        {
            CustomMessageBox_old messageBox = new CustomMessageBox_old(message, title);
            messageBox.ShowDialog();
            return messageBox.dialogResult;
        }
    }
}
