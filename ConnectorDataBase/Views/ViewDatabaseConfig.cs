using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;
//using ConnectorDataBase.Json;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace ConnectorDataBase.Views
{
    public partial class ViewDatabaseConfig : Form
    {
        public ViewDatabaseConfig()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            //List<string> listOfNames = (from object t in listNames.Items select t.ToString()).ToList();

            //AppConfig appConfig = new AppConfig
            //{
            //    AppPath = AppConfig.GetAppPath,
            //    AppSqlString = txtStrConection.Text.Trim(),
            //    AppWebPortal = txtWebPortalPath.Text.Trim(),
            //    Names = listOfNames
            //};

            //File.WriteAllText($"{AppConfig.GetAppPath}"+ @"\AppConfig.json", JsonConvert.SerializeObject(appConfig));

            //Close();
        }

        /// <summary>
        /// Form Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DatabaseConfig_Load(object sender, EventArgs e)
        {
            //var appConfig = AppConfig.GetAppConfig();

            //txtAppPath.Text = appConfig.AppPath;
            //txtStrConection.Text = appConfig.AppSqlString;
            //txtWebPortalPath.Text = appConfig.AppWebPortal;
            //ListNameBoxLoad(appConfig.Names);
        }

        private void ListNameBoxLoad(List<string> list)
        {
            foreach (var s in list)
            {
                listNames.Items.Add(s);
            }

            listNames.Sorted = true;
        }


        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void btnAddName_Click(object sender, EventArgs e)
        {
            ListNameAdd();
        }

        private void ListNameAdd()
        {
            if (txtNames.Text == string.Empty) return;

            if (!listNames.Items.Contains(txtNames.Text))
            {
                listNames.Items.Add(txtNames.Text);
            }
            else
            {
                MessageBox.Show($"O nome -> {txtNames.Text} já existe!");
            }

            txtNames.Clear();
        }

        private void btnRemoveName_Click(object sender, EventArgs e)
        {
            ListNameRemove();
        }

        private void ListNameRemove()
        {
            var selectedItem = listNames.SelectedItem;
            if (selectedItem != null)
            {
                listNames.Items.Remove(selectedItem);
            }
        }

    }
}