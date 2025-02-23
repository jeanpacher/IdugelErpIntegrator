using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ConnectorDataBase;
using Newtonsoft.Json;
using ConnectorDataBase.Json;



namespace DescriptionManager.Views
{
    public partial class ViewDescriptionManager : Form
    {
        public ViewDescriptionManager()
        {
            InitializeComponent();
            InitializeDataGrid();
        }

        private void InitializeDataGrid()
        {
            dgvDescManager.DataSource = DescriptionDataAccess.GetAllDescManagerData();

            dgvDescManager.Columns[0].HeaderText = "Id";
            dgvDescManager.Columns[1].HeaderText = "Nome";
            dgvDescManager.Columns[2].HeaderText = "Nome Campo iProperties";
            dgvDescManager.Columns[3].HeaderText = "Família";
            dgvDescManager.Columns[4].HeaderText = "Script de Descrição";
            dgvDescManager.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvDescManager.Columns[5].HeaderText = "Tipo do Blank";

            dgvDescManager.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDescManager.AutoResizeColumns();
        }

        private void FormDescriptionManager_Load(object sender, EventArgs e)
        {
            FormFieldsFill();

            //var num = Assembly.GetExecutingAssembly().ImageRuntimeVersion;

            var res = ProductVersion;

            Text = "Gerenciador e Construtor de Descrições | Versão: " + res.Trim();
        }

        private void FormFieldsFill()
        {
            ComboBoxFamilyFill();
            ComboBoxBlankTypeFill();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InsertDescription();
        }

        private void ComboBoxBlankTypeFill()
        {
            var bType = new ConnectorDataClassesDataContext(AppConfig.GetSqlConn());

            var blankType = from bt in bType.BLANK_TYPE_NAMEs orderby bt.BLANK_TYPE_NAME1 select bt.BLANK_TYPE_NAME1;

            cbxTipoBlank.DataSource = blankType.ToList();
        }

        private void ComboBoxFamilyFill()
        {
            var vf = new ConnectorDataClassesDataContext(AppConfig.GetSqlConn());

            var famiy = from f in vf.View_Families orderby f.FAMILIA select f.FAMILIA;

            cbxFamily.DataSource = famiy.ToList();
        }

        private void InsertDescription()
        {
            if (txtName.Text.Trim() != string.Empty)
            {
                var dm = DmFill();

                DescriptionDataAccess.Insert(dm);

                InitializeDataGrid();

                ClearFields();
            }
        }

        private DESCRIPTION_MANAGER DmFill()
        {
            var dm = new DESCRIPTION_MANAGER
            {
                NAME_DESCRIPTION = txtName.Text.Trim(),
                NAME_IPROP_FIELD = txtPropField.Text.Trim(),
                FAMILY_TYPE = cbxFamily.Text.Trim(),
                SCRIPT_DESC = txtScriptDesc.Text.Trim(),
                COMMENT = cbxTipoBlank.Text.Trim()
            };
            if (txtId.Text != string.Empty) dm.ID = Convert.ToInt16(txtId.Text);

            return dm;
        }

        private void dgvDescManager_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //Textbox
                txtId.Text = dgvDescManager.Rows[e.RowIndex].Cells[0].Value.ToString().Trim();
                txtName.Text = dgvDescManager.Rows[e.RowIndex].Cells[1].Value.ToString().Trim();
                txtPropField.Text = dgvDescManager.Rows[e.RowIndex].Cells[2].Value.ToString().Trim();
                txtScriptDesc.Text = dgvDescManager.Rows[e.RowIndex].Cells[4].Value.ToString().Trim();
                // Combobox
                cbxTipoBlank.Text = dgvDescManager.Rows[e.RowIndex].Cells[5].Value.ToString().Trim();
                cbxFamily.SelectedIndex =
                    cbxFamily.FindStringExact(dgvDescManager.Rows[e.RowIndex].Cells[3].Value.ToString());
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }

        private void update_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() != string.Empty)
            {
                DescriptionDataAccess.Update(DmFill());

                InitializeDataGrid();

                ClearFields();
            }
        }

        private void ClearFields()
        {
            txtId.Clear();
            txtName.Clear();
            txtPropField.Clear();
            txtScriptDesc.Clear();
            // Combobox
            cbxTipoBlank.Refresh();
            cbxFamily.Refresh();
            txtScriptDesc.SelectionFont = new Font("Segoe UI", 10, FontStyle.Regular);
            txtScriptDesc.SelectionColor = Color.Black;
        }

        private void delete_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult =
                MessageBox.Show(@"Atenção, você irá excluir a configuração da linha selecionada.",
                    @"Excluir Configuração", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                if (dgvDescManager.CurrentRow != null)
                {
                    int result = Convert.ToInt32(dgvDescManager.Rows[dgvDescManager.CurrentRow.Index].Cells[0].Value);

                    DescriptionDataAccess.Delete(result);
                }

                InitializeDataGrid();
            }
        }

        private void txtScriptDesc_TextChanged(object sender, EventArgs e)
        {
            ScriptMarkup();
        }

        private void ScriptMarkup()
        {
            CheckKeyword("IPROP", Color.OrangeRed, 0);
            CheckKeyword("PARAM", Color.Crimson, 0);
            CheckKeyword("TEXT", Color.DarkViolet, 0);
            CheckKeyword("=", Color.DarkBlue, 0);
            CheckKeyword("<", Color.Magenta, 0);
            CheckKeyword(">", Color.Magenta, 0);
            CheckKeyword(",", Color.Maroon, 0);
        }

        private void CheckKeyword(string word, Color color, int startIndex)
        {
            if (txtScriptDesc.Text.Contains(word))
            {
                int index = -1;
                int selectStart = txtScriptDesc.SelectionStart;

                while ((index = txtScriptDesc.Text.IndexOf(word, (index + 1))) != -1)
                {
                    txtScriptDesc.Select((index + startIndex), word.Length);
                    txtScriptDesc.SelectionColor = color;
                    txtScriptDesc.SelectionFont = new Font("Segoe UI", 12, FontStyle.Bold);
                    txtScriptDesc.Select(selectStart, 0);
                    txtScriptDesc.SelectionFont = new Font("Segoe UI", 10, FontStyle.Regular);
                    txtScriptDesc.SelectionColor = Color.Black;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ClearFields();
        }
    }
}