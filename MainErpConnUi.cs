using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TextBox = System.Windows.Forms.TextBox;
using System.Threading.Tasks;
using System.Linq;
using Inventor;
using System.Data;
//using Oracle.ManagedDataAccess.Client;
using PaintManager;
using System.Linq.Expressions;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ConnectorDataBase.Json;
using InvUtils;
using BomCore;
using WUtils;
using Prop = CadModelProperties.Resources.ResIproperties;
using DescriptionManager;
using WConnectorModels;
using OracleBancoDados;
using InvAddIn;




//http://192.168.50.213/webportal/ErpData

namespace IdugelErpIntegrator
{
    public partial class MainErpConnUi : Form
    {
        private bool NeedSaveToNewAge;
        private PartPaint partPaint;
        private PaintHelper paintHelper;
        private LoadPaintData paintData;

        private CadProperties.CadModelProperties _iProperties;
        private Document _selectedDocument;
        private bool _hasApplyMaterial;

        public MainErpConnUi()
        {
            InitializeComponent();
            MainWindowsInitialize();
        }
        private void MainWindowsInitialize()
        {
            //VerifyDocSelected();
            partPaint = new PartPaint();
            paintHelper = new PaintHelper();
            paintData = new LoadPaintData();
            //InitializeDataGrid();
            _iProperties = new CadProperties.CadModelProperties();
            //dataNewAge = new NEWAGE();
            //NamesAutoComplete();
            //dataNewAge = ErpData.FirstOrDefault(d => d.CODIGO == _iProperties.MpCod);
        }
        private void NamesAutoComplete()
        {
            // definição dos TextBox do Nomes
            AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
            collection.AddRange(AppConfig.GetNames().ToArray());

            txtProp_RP_NomeDesenhista.AutoCompleteCustomSource = collection;
            txtProp_RP_NomeProjetista.AutoCompleteCustomSource = collection;
            txtProp_RP_NomeAprovador.AutoCompleteCustomSource = collection;
        }
        private async void MainErpConnUi_Load(object sender, EventArgs e)
        {

            if (InvApp.StandardAddInServer.m_InvApp.Documents.Count == 0)
            {
                MessageBox.Show("Nenhum documento ativo");
                return;
            }

            // Load connection asynchronously to avoid blocking UI

            if (OracleBancoDados.OracleHelper.GetConnection())
                this.Text = this.Text + " -  CONECTADO";
            //await Task.Run(() => OracleHelper.GetConnection());

            _hasApplyMaterial = false;
            cbxProp_DT_TipoPintura.DataSource = paintHelper.PaintTypesList;

            foreach (var acabamento in paintData.PaintAcabamentoList)
            {
                cbxProp_DT_PinturaAcabamento.Items.Add(acabamento);
            }

            var docVerified = VerifyDocSelected();

            if (docVerified == null)
            {
                await LoadFormActionAsync();
            }
            else
            {
                await LoadFormActionAsync(docVerified);
            }

            partPaint = new PartPaint();


            // Set up event handlers
            txtEspessura.KeyPress += ValidateNumericTextBox_KeyPress;
            txtFatorUnidade.KeyPress += ValidateNumericTextBox_KeyPress;

            txtEspessura.TextChanged += ChangeValueMP_TextChanged;
            txtFatorUnidade.TextChanged += ChangeValueMP_TextChanged;
            txtDescInventor.TextChanged += ChangeValueMP_TextChanged;
            TxtUnidades.TextChanged += ChangeValueMP_TextChanged;

            // Set tooltips and populate UI
            DicaParaTextsBox.SetToolTip(this.txtFatorUnidade, "Usar virgula para separador decimais");
            NeedSaveToNewAge = false;
            PreencherComboBoxUnidades();

            // _selectedDocument = InvApp.StandardAddInServer.m_InvApp.ActiveEditDocument;
            //lblFileName.Text = _selectedDocument.FullFileName.Split('\\').Last();


        }

        /// <summary>
        ///     Método Load
        /// </summary>
        private async Task LoadFormActionAsync()
        {
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.MarqueeAnimationSpeed = 50;

            TransactionManager trxManager = InvApp.StandardAddInServer.m_InvApp.TransactionManager;
            Transaction trx =
                trxManager.StartTransaction(InvApp.StandardAddInServer.m_InvApp.ActiveEditDocument, "New Age Connector");

            try
            {
                LoadDocument();

                lblStatus.Text = "Carregando Dados da Peça...";

                await Task.Run(() => GetPropertiesToTextBox());

                //Task getProper = new Task(GetPropertiesToTextBox);
                //getProper.Start();
                //await getProper;

                lblStatus.Text = "Dados Carregados...";
                trx.End();
                lblStatus.Text = "";
            }
            catch (Exception ex)
            {
                trx.Abort();
                InvMsg.Msg("Houveram erros no carregamnto dos dados para o Form.\n\n " + ex);
                CoreLog.WriteLog($"Erros no carregamnto dos dados para o Form. \n\t{ex}");
            }

            progressBar.MarqueeAnimationSpeed = 0;
            progressBar.Style = ProgressBarStyle.Blocks;
            progressBar.Value = progressBar.Minimum;
        }

        /// <summary>
        ///     Método Load
        /// </summary>
        private async Task LoadFormActionAsync(Document oDoc)
        {
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.MarqueeAnimationSpeed = 50;

            TransactionManager trxManager = InvApp.StandardAddInServer.m_InvApp.TransactionManager;
            Transaction trx =
                trxManager.StartTransaction(InvApp.StandardAddInServer.m_InvApp.ActiveEditDocument, "New Age Connector");

            try
            {
                _selectedDocument = oDoc;
                lblFileName.Text = oDoc.DisplayName;

                lblStatus.Text = "Carregando Dados da Peça...";

                await Task.Run(() => GetPropertiesToTextBox(oDoc));
                lblStatus.Text = @"Dados Carregados...";
                trx.End();
                lblStatus.Text = "";
            }
            catch (Exception ex)
            {
                trx.Abort();
                InvMsg.Msg("Houveram erros no carregamnto dos dados para o Form.\n\n " + ex);
            }

            progressBar.MarqueeAnimationSpeed = 0;
            progressBar.Style = ProgressBarStyle.Blocks;
            progressBar.Value = progressBar.Minimum;
        }


        #region Eventos
        private void btnSaveMateriaPrimaToOracle_Click(object sender, EventArgs e)
        {
            if (OracleHelper.connection.State == ConnectionState.Closed) return;

            if (!ValidateAndParseTextBox(txtEspessura, out double espessura, "Valor de espessura inválido") ||
                !ValidateAndParseTextBox(txtFatorUnidade, out double fatorUnidade, "Valor de fator unidade inválido"))
            {
                return;
            }

            // Aplicar lógica de negócios
            OracleHelper.UpdateMateriaPrimaOnNewAge(txtCod.Text, txtDescInventor.Text, espessura, fatorUnidade);
            NeedSaveToNewAge = false;
        }
        private void btnApplyMp_Click_Click(object sender, EventArgs e)
        {

            if (CheckPreRequisitsToApplyMP(sender, e))
            {
                PreencherTextBoxPropriedadesDadosMP();
                ApplyMaterialExecution();
            }


        }

        private bool CheckPreRequisitsToApplyMP(object sender, EventArgs e)
        {
            if (checkMpCustomData.Checked)
            {
                DialogResult result = MessageBox.Show("A Matéria Prima está definida como CUSTOM.\n" +
                                             "Deseja utilizar a Matéria Prima selecionada?", "Atenção",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                    return false;

                checkMpCustomData.Checked = false;
            }

            if (NeedSaveToNewAge)
            {
                DialogResult Result = MessageBox.Show("Existem alterações não salvas no NewAge. Deseja Salvar as alterações?", "Salvar Alterações", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (Result == DialogResult.No) { return false; }

                if (OracleHelper.connection.State == ConnectionState.Closed)
                {
                    MessageBox.Show("Sem conexão com o NewAge");
                    return false;
                }
                btnSaveMateriaPrimaToOracle_Click(sender, e);
            }


            List<string> erros = new List<string>();

            // ANALISANDO O CAMPO ESPESSURA SE POSSUI VALOR E O VALOR É COERENTE
            if (string.IsNullOrEmpty(txtEspessura.Text))
                erros.Add("Valor da espessura está em branco");
            else
            {
                if (!Double.TryParse(txtEspessura.Text, out double espessura))
                    erros.Add("Espessura não é um número");
                else
                {
                    if (espessura <= 0) erros.Add("Espessura é menor ou igual a zero");
                    if (espessura >= 30) erros.Add("Espessura parece muito grande");
                }
            }

            // ANALISANDO O CAMPO FATOR UNIDADE SE POSSUI VALOR E O VALOR É COERENTE
            if (string.IsNullOrEmpty(txtFatorUnidade.Text))
                erros.Add("Fator da Unidade está em branco");
            else
            {
                if (!Double.TryParse(txtFatorUnidade.Text, out double fatorunidade))
                    erros.Add("Fator Unidade não é um número");
                else
                {
                    if (fatorunidade <= 0) erros.Add("Fator da Unidade é menor ou igual a zero");
                    if (fatorunidade >= 30) erros.Add("Fator Unidade parece muito grande");
                }
            }

            // ANALISANDO O CAMPO DESCRICAO DO INVENTOR SE POSSUI VALOR
            if (string.IsNullOrEmpty(txtDescInventor.Text)) erros.Add("O campo descrição do Inventor está em branco");

            if (erros.Count > 0)
            {
                DialogResult result = CustomMessageBox.ShowCustomMessage(erros, "Atenção");

                if (result == DialogResult.No) return false;
            }

            

            return true;


        }
        private void TxtFinder_TextChanged(object sender, EventArgs e)
        {
            FiltrarDados();
        }
        private void dgView_SelectionChanged(object sender, EventArgs e)
        {
            PreencherTextBoxComDadosDaMpFromNewAge();
            NeedSaveToNewAge = false;
        }
        private void ChangeValueMP_TextChanged(object sender, EventArgs e)
        {
            NeedSaveToNewAge = true;
        }
        private void checkMpCustomData_CheckedChanged(object sender, EventArgs e)
        {
            LimparTextBoxMpPropriedades();
            DefineReadWriteMpProperties(checkMpCustomData.Checked);

            if (checkMpCustomData.Checked)
            {
                btnApplyMp.Enabled = false;
                DicaParaTextsBox.Active = true;
            }

            else
            {
                btnApplyMp.Enabled = true;
                DicaParaTextsBox.Active = false;

            }
        }
        private void btnConectarNewAge_Click(object sender, EventArgs e)
        {
            if (OracleHelper.connection.State == ConnectionState.Closed)
                this.Cursor = Cursors.WaitCursor;
            if (OracleHelper.GetConnection())
                this.Text = this.Text + " -  CONECTADO";

            this.Cursor = Cursors.Default;
        }
        #endregion



        #region Metodos Para Aplicar a MP

        private void ApplyMaterialExecution()
        {
            if (ApplyMaterial()) return;

            //InitializeDataGrid();

            UpdateActiveFile();
        }
        private bool ApplyMaterial()
        {
            if (_selectedDocument == null)
            {
                InvMsg.Msg("Não há arquivo associado para aplicação do material. \n " +
                           "Entre em contato com o Desenvolvedor.");
                return true;
            }

            try
            {


                lblStatus.Text = "Aplicando dados no iProperties...";
                ApplyPartData(_selectedDocument);

                lblStatus.Text = "Verificando se foi aplicado Material...";
                HasApplyMaterialCommand();

                lblStatus.Text = string.Empty;
            }
            catch (Exception ex)
            {
                InvMsg.Msg("Algum erro ocorreu no processo de aplicação do material \n" + ex);

                return true;
            }

            return false;
        }

        /// <summary>
        ///     comando para aplicar dados dentro de uma Transaction
        /// </summary>
        private void ApplyPartData(Document oDoc)
        {
            TransactionManager trxManager = InvApp.StandardAddInServer.m_InvApp.TransactionManager;
            Transaction trx = trxManager.StartTransaction(InvApp.StandardAddInServer.m_InvApp.ActiveEditDocument, "ERP Connector Aplicar Dados");
            try
            {
                SetProperties(oDoc);
                if (oDoc.DocumentType == DocumentTypeEnum.kPartDocumentObject)
                {
                    if (InvSheetMetal.IsSheetMetalPart(oDoc as PartDocument))
                    {
                        SetSheetMetalThickness(oDoc as PartDocument);
                    }
                }
                trx.End();

            }
            catch (Exception)
            {
                trx.Abort();
            }
        }
        private static void UpdateActiveFile()
        {
            if (InvApp.StandardAddInServer.m_InvApp.ActiveEditDocument.RequiresUpdate)
            {
                InvApp.StandardAddInServer.m_InvApp.ActiveEditDocument.Update2(true);
            }
        }
        private void SetProperties()
        {
            try
            {
                // Matéria Prima
                InvProps.SetInvIProperties(Prop.MP_COD, txtCod.Text, InvPropetiesGroup.CustomFields);
                InvProps.SetInvIProperties(Prop.MP_FAMILIA, txtFamilia.Text, InvPropetiesGroup.CustomFields);
                InvProps.SetInvIProperties(Prop.MP_DESC_ERP, txtDescErp.Text, InvPropetiesGroup.CustomFields);
                InvProps.SetInvIProperties(Prop.MP_DESC_INVENTOR, txtDescInventor.Text, InvPropetiesGroup.CustomFields);
                InvProps.SetInvIProperties(Prop.MP_UNIDADE, TxtUnidades.Text, InvPropetiesGroup.CustomFields);
                InvProps.SetInvIProperties(Prop.MP_FATOR, txtFatorUnidade.Text.Replace('.', ',').Trim(),
                    InvPropetiesGroup.CustomFields);

                // Máquina
                InvProps.SetInvIProperties(Prop.MQ_COD, txtProp_MQ_Cod.Text, InvPropetiesGroup.CustomFields);
                InvProps.SetInvIProperties(Prop.MQ_NOME, txtProp_MQ_Nome.Text, InvPropetiesGroup.CustomFields);

                // Peça
                InvProps.SetInvIProperties(Prop.PC_COD_DESENHO, txtProp_PC_CodDesenho.Text, InvPropetiesGroup.CustomFields);
                InvProps.SetInvIProperties(Prop.PC_DESC, txtProp_PC_Desc.Text, InvPropetiesGroup.CustomFields);
                InvProps.SetInvIProperties(Prop.PC_DESC_COMPLETA, txtProp_PC_DescCompleta.Text,
                    InvPropetiesGroup.CustomFields);

                InvProps.SetInvIProperties(Prop.PC_DESC_COMPLETA_CUSTOM, CheckState(checkPcDescCustom),
                    InvPropetiesGroup.CustomFields);
                InvProps.SetInvIProperties(Prop.PC_NUM_REV, txtProp_PC_NumRevisao.Text,
                    InvPropetiesGroup.InventorSumaryInformation);

                // Desenhistas
                InvProps.SetInvIProperties(Prop.NOME_APROVADOR, txtProp_RP_NomeAprovador.Text,
                    InvPropetiesGroup.CustomFields);
                InvProps.SetInvIProperties(Prop.NOME_DESENHISTA, txtProp_RP_NomeDesenhista.Text,
                    InvPropetiesGroup.CustomFields);
                InvProps.SetInvIProperties(Prop.NOME_PROJETISTA, txtProp_RP_NomeProjetista.Text,
                    InvPropetiesGroup.CustomFields);

                // BlankType
                InvProps.SetInvIProperties(Prop.BLANK_TYPE, txtProp_DT_BlankType.Text, InvPropetiesGroup.CustomFields);
                InvProps.SetInvIProperties(Prop.MP_PESO_BRUTO, txtProp_DT_PesoBruto.Text, InvPropetiesGroup.CustomFields);

                // Pintura
                InvProps.SetInvIProperties(Prop.PC_TIPO_PINTURA, cbxProp_DT_TipoPintura.Text, InvPropetiesGroup.CustomFields);
                InvProps.SetInvIProperties(Prop.PC_AREA_PINTURA, txtProp_DT_AreaPintura.Text, InvPropetiesGroup.CustomFields);
                InvProps.SetInvIProperties(Prop.PC_ACABAMENTO_PINTURA, cbxProp_DT_PinturaAcabamento.Text, InvPropetiesGroup.CustomFields);

                InvProps.SetInvIProperties(Prop.PC_PINTURA_INTERNA_DIFERENTE, CheckState(checkPintura), InvPropetiesGroup.CustomFields);
            }
            catch (Exception ex)
            {
                CoreLog.WriteLog(
                    $"--------" +
                    $"Erro ao Aplicar Propriedades:\n -> " +
                    $" {ex} <-\n " +
                    $"\n-------"
                );
            }
        }

        /// <summary>
        ///     Carrega os dados no iProperties
        /// </summary>
        private void SetProperties(Document oDoc)
        {
            try
            {
                // Matéria Prima
                InvProps.SetInvIProperties(Prop.MP_CUSTOM, CheckState(checkMpCustomData), InvPropetiesGroup.CustomFields, oDoc);

                InvProps.SetInvIProperties(Prop.MP_COD, txtProp_MP_Cod.Text, InvPropetiesGroup.CustomFields, oDoc);
                InvProps.SetInvIProperties(Prop.MP_FAMILIA, txtProp_MP_Familia.Text, InvPropetiesGroup.CustomFields, oDoc);
                InvProps.SetInvIProperties(Prop.MP_DESC_ERP, txtProp_MP_DescERP.Text, InvPropetiesGroup.CustomFields, oDoc);
                InvProps.SetInvIProperties(Prop.MP_DESC_INVENTOR, txtProp_MP_DescInventor.Text, InvPropetiesGroup.CustomFields, oDoc);
                InvProps.SetInvIProperties(Prop.MP_UNIDADE, txtProp_MP_Unidade.Text, InvPropetiesGroup.CustomFields, oDoc);
                InvProps.SetInvIProperties(Prop.MP_FATOR, txtProp_MP_FatorUnidade.Text.Replace('.', ',').Trim(),
                    InvPropetiesGroup.CustomFields, oDoc);

                // Máquina
                InvProps.SetInvIProperties(Prop.MQ_COD, txtProp_MQ_Cod.Text, InvPropetiesGroup.CustomFields, oDoc);
                InvProps.SetInvIProperties(Prop.MQ_NOME, txtProp_MQ_Nome.Text, InvPropetiesGroup.CustomFields, oDoc);

                // Peça
                InvProps.SetInvIProperties(Prop.PC_COD_DESENHO, txtProp_PC_CodDesenho.Text, InvPropetiesGroup.CustomFields, oDoc);
                InvProps.SetInvIProperties(Prop.PC_DESC, txtProp_PC_Desc.Text, InvPropetiesGroup.CustomFields, oDoc);
                InvProps.SetInvIProperties(Prop.PC_DESC_COMPLETA, txtProp_PC_DescCompleta.Text,
                    InvPropetiesGroup.CustomFields, oDoc);

                InvProps.SetInvIProperties(Prop.PC_DESC_COMPLETA_CUSTOM, CheckState(checkPcDescCustom),
                    InvPropetiesGroup.CustomFields, oDoc);

                InvProps.SetInvIProperties(Prop.PC_NUM_REV, txtProp_PC_NumRevisao.Text,
                    InvPropetiesGroup.InventorSumaryInformation, oDoc);

                // Desenhistas
                InvProps.SetInvIProperties(Prop.NOME_APROVADOR, txtProp_RP_NomeAprovador.Text,
                    InvPropetiesGroup.CustomFields, oDoc);
                InvProps.SetInvIProperties(Prop.NOME_DESENHISTA, txtProp_RP_NomeDesenhista.Text,
                    InvPropetiesGroup.CustomFields, oDoc);
                InvProps.SetInvIProperties(Prop.NOME_PROJETISTA, txtProp_RP_NomeProjetista.Text,
                    InvPropetiesGroup.CustomFields, oDoc);

                // BlankType
                InvProps.SetInvIProperties(Prop.BLANK_TYPE, txtProp_DT_BlankType.Text, InvPropetiesGroup.CustomFields, oDoc);
                InvProps.SetInvIProperties(Prop.MP_PESO_BRUTO, txtProp_DT_PesoBruto.Text, InvPropetiesGroup.CustomFields, oDoc);

                // Pintura
                InvProps.SetInvIProperties(Prop.PC_TIPO_PINTURA, cbxProp_DT_TipoPintura.Text, InvPropetiesGroup.CustomFields, oDoc);
                InvProps.SetInvIProperties(Prop.PC_AREA_PINTURA, txtProp_DT_AreaPintura.Text, InvPropetiesGroup.CustomFields, oDoc);
                InvProps.SetInvIProperties(Prop.PC_ACABAMENTO_PINTURA, cbxProp_DT_PinturaAcabamento.Text, InvPropetiesGroup.CustomFields, oDoc);

                InvProps.SetInvIProperties(Prop.PC_PINTURA_INTERNA_DIFERENTE, CheckState(checkPintura), InvPropetiesGroup.CustomFields, oDoc);
            }
            catch (Exception ex)
            {
                CoreLog.WriteLog(
                    $"--------" +
                    $"Erro ao Aplicar Propriedades:\n -> " +
                    $" {ex.ToString()} <-\n " +
                    $"\n-------"
                );
            }

        }

        /// <summary>
        ///     Define a espessura da chapa conforme a matéria prima
        /// </summary>
        private void SetSheetMetalThickness()
        {
            if (txtEspessura.Text == string.Empty)
                return;

            var value = txtEspessura.Text.ToDouble();
            InvSheetMetal.SetThickness(value);
        }

        /// <summary>
        ///     Define a espessura da chapa INFORMADA conforme a matéria prima
        /// </summary>
        private void SetSheetMetalThickness(PartDocument oDoc)
        {
            if (txtEspessura.Text == string.Empty)
                return;

            var value = txtEspessura.Text.ToDouble();
            InvSheetMetal.SetThickness(value, oDoc);
        }

        /// <summary>
        ///     Método para verificar se o material novo selecionad já foi aplicado
        /// </summary>
        private void HasApplyMaterialCommand()
        {
            _hasApplyMaterial = true;
            lblAlert.Text = string.Empty;
        }
        #endregion

        #region MetodosValidação
        
        private void ValidateNumericTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only numeric input, control keys, and comma for decimals
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ',';
        }
        public bool ValidateAndParseTextBox(TextBox textBox, out double result, string errorMessage)
        {
            if (textBox.Text.StartsWith(","))
            {
                textBox.Text = "0" + textBox.Text;
            }

            if (!double.TryParse(textBox.Text, out result))
            {
                MessageBox.Show(errorMessage, "Erro de Validação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        private Document VerifyDocSelected()
        {

            if (InvApp.StandardAddInServer.m_InvApp.ActiveEditDocument.DocumentType == DocumentTypeEnum.kDrawingDocumentObject)
            {
                var drawDoc = InvApp.StandardAddInServer.m_InvApp.ActiveEditDocument;
                //var refFile = drawDoc.ReferencedFiles[0];

                foreach (Document referencedFile in drawDoc.ReferencedFiles)
                    try
                    {
                        var document = referencedFile;
                        return document;
                    }
                    catch
                    {
                        InvMsg.Msg("Não contém arquivo de peça.");
                    }
                //var activeSheet = SheetHelper.GetActiveSheet();
                //activeSheet.DrawingViews.
            }

            var selDoc = InvApp.StandardAddInServer.m_InvApp.ActiveEditDocument.SelectSet;

            if (selDoc.Count <= 0) return null;

            foreach (var doc in selDoc)
            {
                try
                {
                    if (doc.GetType() is ComponentOccurrence)
                    {

                        ComponentOccurrence occur = (ComponentOccurrence)doc;
                        return occur.Definition.Document as Document;
                    }
                }
                catch (Exception e)
                {
                    CoreLog.WriteLog($"\n-----> Open Selected\n {e}");
                }
            }
            return null;
        }

        #endregion


        #region MetodosAuxiliaresInterface
        private void FiltrarDados()
        {

            if (OracleBancoDados.OracleHelper.connection.State == ConnectionState.Closed) return;

            string txtFinder = TxtFinder.Text.Trim();
            if (txtFinder.Length <= 2)
                dgView.DataSource = null;

            else
            {
                DataTable resultado = null;
                if (char.IsDigit(txtFinder[0]))
                {
                    resultado = OracleHelper.PesquisarMateriaPrimaPorCodigo(txtFinder);
                }
                else if (char.IsLetter(txtFinder[0]))
                {
                    resultado = OracleHelper.PesquisarMateriaPrimaPorDescricao(txtFinder);
                }
                dgView.DataSource = resultado;
                dgView.Columns["DESCINV"].Visible = false;
                dgView.Columns["ESPMP"].Visible = false;
                dgView.Columns["FATORUN"].Visible = false;
                dgView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
        }
        private void PreencherComboBoxUnidades()
        {
            List<string> unidades = OracleHelper.GetUnidades();
            TxtUnidades.Items.Clear(); // Limpa os itens anteriores
            TxtUnidades.Items.AddRange(unidades.ToArray());
        }
        public void DefineReadWriteMpProperties(bool rw)
        {
            txtProp_MP_Familia.ReadOnly = !rw;
            txtProp_MP_Unidade.ReadOnly = !rw;
            txtProp_MP_Cod.ReadOnly = !rw;
            txtProp_MP_Espessura.ReadOnly = !rw;
            txtProp_MP_FatorUnidade.ReadOnly = !rw;
            txtProp_MP_DescERP.ReadOnly = !rw;
            txtProp_MP_DescInventor.ReadOnly = !rw;
        }
        private void LimparTextBoxMpDadosMP()
        {
            txtCod.Clear();
            txtDescErp.Clear();
            txtDescInventor.Clear();
            txtFamilia.Clear();
            txtEspessura.Clear();
            txtFatorUnidade.Clear();
            TxtUnidades.Text = string.Empty;
        }
        private void LimparTextBoxMpPropriedades()
        {
            txtProp_MP_Familia.Clear();
            txtProp_MP_Unidade.Clear();
            txtProp_MP_Cod.Clear();
            txtProp_MP_Espessura.Clear();
            txtProp_MP_FatorUnidade.Clear();
            txtProp_MP_DescERP.Clear();
            txtProp_MP_DescInventor.Clear();
        }
        private void PreencherTextBoxComDadosDaMpFromNewAge()
        {
            if (dgView.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgView.SelectedRows[0];

                // Preencher os TextBoxs com os valores das colunas da linha selecionada
                txtCod.Text = row.Cells["CODIGO"].Value.ToString();
                txtFamilia.Text = row.Cells["FAMILIA_COMPLETA"].Value.ToString();
                txtDescErp.Text = row.Cells["DESCR"].Value.ToString();
                TxtUnidades.Text = row.Cells["UNIDADE"].Value.ToString();
                txtDescInventor.Text = row.Cells["DESCINV"].Value.ToString();
                txtEspessura.Text = row.Cells["ESPMP"].Value.ToString();
                txtFatorUnidade.Text = row.Cells["FATORUN"].Value.ToString();

            }
            else
                LimparTextBoxMpDadosMP();
        }
        private void PreencherTextBoxPropriedadesDadosMP()
        {
            txtProp_MP_Cod.Text = txtCod.Text;
            txtProp_MP_DescERP.Text = txtDescErp.Text;
            txtProp_MP_DescInventor.Text = txtDescInventor.Text;
            txtProp_MP_Espessura.Text = txtEspessura.Text;
            txtProp_MP_Familia.Text = txtFamilia.Text;
            txtProp_MP_FatorUnidade.Text = txtFatorUnidade.Text;
            txtProp_MP_Unidade.Text = TxtUnidades.Text;

        }

        /// <summary>
        ///     Método para limpar todos os textbox
        /// </summary>
        private void ClearAllTextBox()
        {
            foreach (Control control in Controls)
                if (control is TextBox)
                    ((TextBox)control).Text = string.Empty;
                else if (control is ComboBox)
                    ((ComboBox)control).Text = string.Empty;
                else if (control is DataGridView)
                    ((DataGridView)control).Rows.Clear();
        }

        #endregion

        private void MainErpConnUi_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ClearAllTextBox();
        }


        #region Manipulação arquivos do Inventor

        private void LoadDocument()
        {
            _selectedDocument = InvApp.StandardAddInServer.m_InvApp.ActiveEditDocument;

            lblFileName.Text = _selectedDocument.DisplayName;

            if (_selectedDocument.DocumentType != DocumentTypeEnum.kPartDocumentObject) return;

            btnSelectDoc.Enabled = false;
            btnGetThisDoc.Enabled = false;
            BtnSaveAndReplace.Enabled = false;
            BtnReplaceAllInStructure.Enabled = false;

        }

        /// <summary>
        ///     Carrega os dados do iProperties para os TextBox
        /// </summary>
        private void GetPropertiesToTextBox()
        {
            _iProperties.GetAllCadModelProperties();

            if (_iProperties != null)
            {
                //
                // TextBox Dados MP Aba1
                // Verifica se a propriedade MP_CUSTOM é falsa
                if (_iProperties.MpCustom == "0")
                {
                    checkMpCustomData.Checked = false;
                    TxtBox.SetToTextBox(txtCod, _iProperties.MpCod);
                    TxtBox.SetToTextBox(txtFamilia, _iProperties.MpFamilia);
                    TxtBox.SetToTextBox(txtDescErp, _iProperties.MpDescErp);
                    TxtBox.SetToTextBox(txtDescInventor, _iProperties.MpDescInventor);
                    TxtBox.SetToComboBox(TxtUnidades, _iProperties.MpUnidade);
                    TxtBox.SetToTextBox(txtFatorUnidade, _iProperties.MpFator);
                }
                else
                {
                    checkMpCustomData.Checked = true;
                }

                //
                // Matéria Prima Aba Propriedades
                //
                TxtBox.SetToTextBox(txtProp_MP_Cod, _iProperties.MpCod);
                TxtBox.SetToTextBox(txtProp_MP_Familia, _iProperties.MpFamilia);
                TxtBox.SetToTextBox(txtProp_MP_DescERP, _iProperties.MpDescErp);
                TxtBox.SetToTextBox(txtProp_MP_DescInventor, _iProperties.MpDescInventor);
                TxtBox.SetToTextBox(txtProp_MP_Unidade, _iProperties.MpUnidade);
                TxtBox.SetToTextBox(txtProp_MP_FatorUnidade, _iProperties.MpFator);

                //
                // Dados Equipamento
                //
                TxtBox.SetToTextBox(txtProp_MQ_Cod, _iProperties.MqCodigo);
                TxtBox.SetToTextBox(txtProp_MQ_Nome, _iProperties.MqNome);

                //
                // Dados da Peça
                //
                //TxtBox.SetToTextBox(txtProp_PC_Nome, _iProperties.PcNome);
                //TxtBox.SetToTextBox(txtProp_PC_NumDesenho, _iProperties.PcNumDesenho);
                TxtBox.SetToTextBox(txtProp_PC_CodDesenho, _iProperties.PcCodDesenho);
                TxtBox.SetToTextBox(txtProp_PC_Desc, _iProperties.PcDesc);
                TxtBox.SetToTextBox(txtProp_PC_DescCompleta, _iProperties.PcDescCompleta);
                TxtBox.SetToTextBox(txtProp_PC_NumRevisao, _iProperties.PcNumRev);

                // Propriedade Bool
                checkPcDescCustom.Checked = CheckState(_iProperties.PcDescCompletaCustom);

                //
                // Dados Projetista
                //
                TxtBox.SetToTextBox(txtProp_RP_NomeAprovador, _iProperties.RpNomeAprovador);
                TxtBox.SetToTextBox(txtProp_RP_NomeDesenhista, _iProperties.RpNomeDesenhista);
                TxtBox.SetToTextBox(txtProp_RP_NomeProjetista, _iProperties.RpNomeProjetista);

                // 
                // Dados Técnicos
                //
                TxtBox.SetToTextBox(txtProp_DT_PesoAcabado, _iProperties.DtPesoAcabado);
                TxtBox.SetToTextBox(txtProp_DT_PesoBruto, _iProperties.PcPesoBruto);
                TxtBox.SetToTextBox(txtProp_DT_DimBlank, _iProperties.PcDimBlank);
                TxtBox.SetToTextBox(txtProp_DT_BlankType, _iProperties.BlankType);

                //
                // Pintura
                TxtBox.SetToTextBox(txtProp_DT_AreaPintura, _iProperties.PcPinturaArea);
                cbxProp_DT_TipoPintura.SelectedIndex = cbxProp_DT_TipoPintura.FindStringExact(_iProperties.PcPinturaTipo);
                checkPintura.Checked = CheckState(_iProperties.PcPinturaInternaDiferente);
                cbxProp_DT_PinturaAcabamento.SelectedIndex =
                    cbxProp_DT_PinturaAcabamento.FindStringExact(_iProperties.PcPinturaAcabamento);

                //
                // Coleta a espessura da peça se for um SheetMetal
                if (!InvSheetMetal.IsSheetMetalPart())
                {
                    //return true;
                }
                else
                {
                    VerificaDefineEspessura();
                }

            }
            else
            {
                //return false;
            }

        }
        private void GetPropertiesToTextBox(Document oDoc)
        {
            _iProperties = _iProperties.GetAllCadModelProperties(oDoc);

            if (_iProperties != null)
            {
                LoadDataToControls(_iProperties);
            }
        }


        /// <summary>
        /// Carregamento de dados para o Form
        /// </summary>
        /// <param name="_iProperties"></param>
        private void LoadDataToControls(CadProperties.CadModelProperties _iProperties)
        {
            try
            {
                TxtBox.SetToTextBox(txtCod, _iProperties.MpCod);
                TxtBox.SetToTextBox(txtFamilia, _iProperties.MpFamilia);
                TxtBox.SetToTextBox(txtDescErp, _iProperties.MpDescErp);
                TxtBox.SetToTextBox(txtDescInventor, _iProperties.MpDescInventor);
                TxtBox.SetToComboBox(TxtUnidades, _iProperties.MpUnidade);
                TxtBox.SetToTextBox(txtFatorUnidade, _iProperties.MpFator);

                //
                // Matéria Prima Aba Propriedades
                //
                TxtBox.SetToTextBox(txtProp_MP_Cod, _iProperties.MpCod);
                TxtBox.SetToTextBox(txtProp_MP_Familia, _iProperties.MpFamilia);
                TxtBox.SetToTextBox(txtProp_MP_DescERP, _iProperties.MpDescErp);
                TxtBox.SetToTextBox(txtProp_MP_DescInventor, _iProperties.MpDescInventor);
                TxtBox.SetToTextBox(txtProp_MP_Unidade, _iProperties.MpUnidade);
                TxtBox.SetToTextBox(txtProp_MP_FatorUnidade, _iProperties.MpFator);

                //
                // Dados Equipamento
                //
                TxtBox.SetToTextBox(txtProp_MQ_Cod, _iProperties.MqCodigo);
                TxtBox.SetToTextBox(txtProp_MQ_Nome, _iProperties.MqNome);

                //
                // Dados da Peça
                //
                TxtBox.SetToTextBox(txtProp_PC_CodDesenho, _iProperties.PcCodDesenho);
                TxtBox.SetToTextBox(txtProp_PC_Desc, _iProperties.PcDesc);
                TxtBox.SetToTextBox(txtProp_PC_DescCompleta, _iProperties.PcDescCompleta);
                TxtBox.SetToTextBox(txtProp_PC_NumRevisao, _iProperties.PcNumRev);

                // Propriedade Bool
                checkPcDescCustom.Checked = CheckState(_iProperties.PcDescCompletaCustom);

                //
                // Dados Projetista
                //
                TxtBox.SetToTextBox(txtProp_RP_NomeAprovador, _iProperties.RpNomeAprovador);
                TxtBox.SetToTextBox(txtProp_RP_NomeDesenhista, _iProperties.RpNomeDesenhista);
                TxtBox.SetToTextBox(txtProp_RP_NomeProjetista, _iProperties.RpNomeProjetista);

                // 
                // Dados Técnicos
                //
                TxtBox.SetToTextBox(txtProp_DT_PesoAcabado, _iProperties.DtPesoAcabado);
                TxtBox.SetToTextBox(txtProp_DT_PesoBruto, _iProperties.PcPesoBruto);
                TxtBox.SetToTextBox(txtProp_DT_DimBlank, _iProperties.PcDimBlank);
                TxtBox.SetToTextBox(txtProp_DT_BlankType, _iProperties.BlankType);

                //
                // Pintura
                TxtBox.SetToTextBox(txtProp_DT_AreaPintura, _iProperties.PcPinturaArea);
                cbxProp_DT_TipoPintura.SelectedIndex =
                    cbxProp_DT_TipoPintura.FindStringExact(_iProperties.PcPinturaTipo);
                checkPintura.Checked = CheckState(_iProperties.PcPinturaInternaDiferente);
                cbxProp_DT_PinturaAcabamento.SelectedIndex =
                    cbxProp_DT_PinturaAcabamento.FindStringExact(_iProperties.PcPinturaAcabamento);

                //
                // Coleta a espessura da peça se for um SheetMetal
                if (!InvSheetMetal.IsSheetMetalPart())
                    return;

                VerificaDefineEspessura();
            }
            catch (Exception ex)
            {
                CoreLog.WriteLog(
                    $"--------" +
                    $"Erro ao recuperar Propriedades:\n -> " +
                    $" {ex.ToString()} <-\n " +
                    $"\n-------"
                );
            }
        }

        /// <summary>
        /// Verifica as espessuras
        /// </summary>
        private void VerificaDefineEspessura()
        {
            try
            {
                var espessura = InvSheetMetal.GetThickness().ToString();

                if (_iProperties.Thickness != 0)
                {
                    if (_iProperties.Thickness != InvSheetMetal.GetThickness())
                    {
                        InvMsg.Msg($"A espessura configurada da peça parece estar diferente da do material aplicado.\n\n" +
                                   $"-> Espessura Configurada Inventor: ( {espessura} ).\n" +
                                   $"-> Espessura do Material ERP: ( {_iProperties.Thickness} ).\n\n" +
                                   $"Será aplicado a espessura configurada ( {espessura} )Verifique essas informaçãoes antes de finalizar a aplicação do material.");
                    }
                }

                txtEspessura.Text = espessura;
                txtProp_MP_Espessura.Text = espessura;
            }
            catch (Exception e)
            {
                CoreLog.WriteLog(e.ToString());
                InvMsg.Msg($"Erro ao verificar a espessura! \n\n {e}");
            }
        }

        /// <summary>
        /// Update do Blank do arquivo Ativo
        /// </summary>
        private void BlankUpdate()
        {
            CoreDescription cd = new CoreDescription();

            SetProperties();
            txtProp_PC_DescCompleta.Text = UpdateBlank.Execute(txtProp_MP_Familia.Text);
            txtProp_DT_BlankType.Text = cd.GetDescriptionType(txtProp_MP_Familia.Text);
            CalculaMateriaPrima();
        }

        #endregion



        /// <summary>
        /// Verifica o Status do Checkbox
        /// </summary>
        /// <param name="checkBox"></param>
        /// <returns></returns>
        public string CheckState(CheckBox checkBox)
        {
            return checkBox.Checked ? "1" : "0";
        }

        /// <summary>
        /// Verifica o Status do Checkbox
        /// </summary>
        /// <param name="checkBox"></param>
        /// <returns></returns>
        public bool CheckState(string checkBox)
        {
            if (checkBox.Equals("1"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        #region Dados de Pintura

        /// <summary>
        ///     Define valores de matéria prima para a peça
        /// </summary>
        private void CalculaMateriaPrima()
        {
            // Documento atual e Massa
            PartDocument partDoc = (PartDocument)InvApp.StandardAddInServer.m_InvApp.ActiveEditDocument;
            MassProperties mass = partDoc.ComponentDefinition.MassProperties;

            // Cria um BomData para coletar os dados necessários para Cálculo
            BomData bc = new BomData
            {
                Blank = InvProps.GetInventorCustomProperties(Prop.CH_DIM_BLANK),
                Fator = txtFatorUnidade.Text.Trim(),
                PesoAcabado = Math.Round(Convert.ToDouble(mass.Mass), 2)
            };


            // Peso Bruto
            if (txtProp_DT_BlankType.Text == BlankType.SheetMetal.ToString()) bc.BlankType = BlankType.SheetMetal;
            if (txtProp_DT_BlankType.Text == BlankType.Cylinder.ToString()) bc.BlankType = BlankType.Cylinder;
            if (txtProp_DT_BlankType.Text == BlankType.Linear.ToString()) bc.BlankType = BlankType.Linear;

            bc.PesoBruto = BomCalc.Calcule(bc);

            // Peso Bruto
            TxtBox.SetToTextBox(txtProp_DT_PesoBruto, bc.PesoBruto + " KG");

            // Blank
            TxtBox.SetToTextBox(txtProp_DT_DimBlank, bc.Blank);

            // Peso Acabado
            TxtBox.SetToTextBox(txtProp_DT_PesoAcabado, bc.PesoAcabado + " KG");

            // Salva nas propriedades
            InvProps.SetInvIProperties(Prop.PC_PESO_ACABADO, txtProp_DT_PesoAcabado.Text,
                InvPropetiesGroup.CustomFields);
            InvProps.SetInvIProperties(Prop.MP_PESO_BRUTO, txtProp_DT_PesoBruto.Text, InvPropetiesGroup.CustomFields);
        }

        /// <summary>
        ///     Define valores de matéria prima para a peça indicada
        /// </summary>
        private void CalculaMateriaPrima(Document oDoc)
        {
            // Documento atual e Massa
            PartDocument partDoc = (PartDocument)oDoc;
            MassProperties mass = partDoc.ComponentDefinition.MassProperties;

            // Cria um BomData para coletar os dados necessários para Cálculo
            BomData bc = new BomData
            {
                Blank = InvProps.GetInventorCustomProperties(Prop.CH_DIM_BLANK, oDoc),
                Fator = txtFatorUnidade.Text.Trim(),
                PesoAcabado = Math.Round(Convert.ToDouble(mass.Mass), 2)
            };

            // Peso Bruto
            if (txtProp_DT_BlankType.Text == BlankType.SheetMetal.ToString()) bc.BlankType = BlankType.SheetMetal;
            if (txtProp_DT_BlankType.Text == BlankType.Cylinder.ToString()) bc.BlankType = BlankType.Cylinder;
            if (txtProp_DT_BlankType.Text == BlankType.Linear.ToString()) bc.BlankType = BlankType.Linear;

            bc.PesoBruto = BomCalc.Calcule(bc, oDoc);

            // Peso Bruto
            TxtBox.SetToTextBox(txtProp_DT_PesoBruto, bc.PesoBruto + " KG");

            // Blank
            TxtBox.SetToTextBox(txtProp_DT_DimBlank, bc.Blank);

            // Peso Acabado
            TxtBox.SetToTextBox(txtProp_DT_PesoAcabado, bc.PesoAcabado + " KG");

            partPaint.Area = Math.Round(mass.Area, 2);
            partPaint.PaintType = cbxProp_DT_TipoPintura.Text;

            // Área de Pintura
            TxtBox.SetToTextBox(txtProp_DT_AreaPintura, paintHelper.SetArea(partPaint).ToString());

            // Salva nas propriedades
            InvProps.SetInvIProperties(Prop.PC_PESO_ACABADO, txtProp_DT_PesoAcabado.Text,
                InvPropetiesGroup.CustomFields, oDoc);
            InvProps.SetInvIProperties(Prop.MP_PESO_BRUTO, txtProp_DT_PesoBruto.Text, InvPropetiesGroup.CustomFields, oDoc);
            InvProps.SetInvIProperties(Prop.PC_AREA_PINTURA, txtProp_DT_AreaPintura.Text, InvPropetiesGroup.CustomFields, oDoc);
            InvProps.SetInvIProperties(Prop.PC_TIPO_PINTURA, cbxProp_DT_TipoPintura.Text, InvPropetiesGroup.CustomFields, oDoc);
            InvProps.SetInvIProperties(Prop.PC_ACABAMENTO_PINTURA, cbxProp_DT_PinturaAcabamento.Text, InvPropetiesGroup.CustomFields, oDoc);

            InvProps.SetInvIProperties(Prop.PC_ACABAMENTO_PINTURA, cbxProp_DT_PinturaAcabamento.Text, InvPropetiesGroup.CustomFields, oDoc);
            InvProps.SetInvIProperties(Prop.PC_PINTURA_INTERNA_DIFERENTE, CheckState(checkPintura), InvPropetiesGroup.CustomFields, oDoc);
        }

        #endregion

        private void tabDados_Click(object sender, EventArgs e)
        {

        }

        private void BtnRevClear_Click(object sender, EventArgs e)
        {
            txtProp_PC_NumRevisao.Clear();
        }

        private void cbxProp_DT_TipoPintura_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxProp_DT_TipoPintura.Text == "Inteiro")
            {
                checkPintura.Enabled = true;
            }
            else
            {
                checkPintura.Checked = false;
                checkPintura.Enabled = false;
            }
        }

        private void btnAtualizarBlank_Click(object sender, EventArgs e)
        {
            if (_selectedDocument.DocumentType == DocumentTypeEnum.kPartDocumentObject)
            {
                BlankUpdate(_selectedDocument);
            }
            else
            {
                InvMsg.Msg("Para atualização do Blank é necessário selecionar uma peça (ARQUIVO IPT)." +
                           "\nNão é possível calcular blank de uma montagem (ARQUIVO IAM).");
            }
        }

        /// <summary>
        /// Update do Blank do arquivo solicitado
        /// </summary>
        /// <param name="oDoc"></param>
        private void BlankUpdate(Document oDoc)
        {
            CoreDescription cd = new CoreDescription();

            SetProperties(oDoc);
            txtProp_PC_DescCompleta.Text = UpdateBlank.Execute(txtProp_MP_Familia.Text, oDoc);
            txtProp_DT_BlankType.Text = cd.GetDescriptionType(txtProp_MP_Familia.Text);

            CalculaMateriaPrima(oDoc);
        }
    }

}

//    private PartPaint partPaint;
//    private PaintHelper paintHelper;
//    private LoadPaintData paintData;


//    /// <summary>
//    ///     Defines the _iProperties
//    /// </summary>
//    private CadProperties.CadModelProperties _iProperties;

//    /// <summary>
//    ///     Defines the _dataSet
//    /// </summary>
//    private IQueryable _dataSet;

//    /// <summary>
//    /// Lista dos dados do ERP para pesquisa local
//    /// </summary>
//    private static List<NEWAGE> ErpData = new List<NEWAGE>();

//    private NEWAGE dataNewAge;

//    /// <summary>
//    /// Documento 
//    /// </summary>
//    private Document _selectedDocument;

//    // private  newAgeDb;

//    /// <summary>
//    ///     Defines the _hasApplyMaterial
//    /// </summary>
//    private bool _hasApplyMaterial;

//    /// <summary>
//    ///     Método Construtor
//    /// </summary>
//    public MainErpConnUi()
//    {
//        InitializeComponent();
//        MainWindowsInitialize();
//    }

//    private void MainWindowsInitialize()
//    {
//        //VerifyDocSelected();
//        partPaint = new PartPaint();
//        paintHelper = new PaintHelper();
//        paintData = new LoadPaintData();
//        InitializeDataGrid();
//        _iProperties = new CadProperties.CadModelProperties();
//        dataNewAge = new NEWAGE();
//        NamesAutoComplete();
//        dataNewAge = ErpData.FirstOrDefault(d => d.CODIGO == _iProperties.MpCod);
//    }

//    private void NamesAutoComplete()
//    {
//        // definição dos TextBox do Nomes
//        AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
//        collection.AddRange(AppConfig.GetNames().ToArray());

//        txtProp_RP_NomeDesenhista.AutoCompleteCustomSource = collection;
//        txtProp_RP_NomeProjetista.AutoCompleteCustomSource = collection;
//        txtProp_RP_NomeAprovador.AutoCompleteCustomSource = collection;
//    }

//    /// <summary>
//    ///     comando para aplicar dados dentro de uma Transaction
//    /// </summary>
//    private void ApplyPartData()
//    {
//        TransactionManager trxManager = AddinGlobal.InventorApp.TransactionManager;
//        Transaction trx =
//            trxManager.StartTransaction(AddinGlobal.InventorApp.ActiveEditDocument, "ERP Connector Aplicar Dados");
//        try
//        { 
//            SetProperties();

//            if (InvSheetMetal.IsSheetMetalPart())
//            {
//                SetSheetMetalThic kness();
//            }
//            trx.End();

//        }
//        catch (Exception)
//        {
//            trx.Abort();
//        }
//    }

//    /// <summary>
//    ///     comando para aplicar dados dentro de uma Transaction
//    /// </summary>
//    private void ApplyPartData(Document oDoc)
//    {
//        TransactionManager trxManager = AddinGlobal.InventorApp.TransactionManager;
//        Transaction trx = trxManager.StartTransaction(AddinGlobal.InventorApp.ActiveEditDocument, "ERP Connector Aplicar Dados");
//        try
//        {
//            SetProperties(oDoc);
//            if (oDoc.DocumentType == DocumentTypeEnum.kPartDocumentObject)
//            {
//                if (InvSheetMetal.IsSheetMetalPart(oDoc as PartDocument))
//                {
//                    SetSheetMetalThickness(oDoc as PartDocument);
//                }
//            }
//            trx.End();

//        }
//        catch (Exception)
//        {
//            trx.Abort();
//        }
//    }

//    /// <summary>
//    ///     Evento botão Aplicar
//    /// </summary>
//    /// <param name="sender"></param>
//    /// <param name="e"></param>
//    private async void btnApply_Click(object sender, EventArgs e)
//    {
//        try
//        {
//            Task taskDbUpdate = new Task(SetDbUpdate);
//            Task taskApplyData = new Task(ApplyPartData);
//            Task taskHasApplyMaterialCommand = new Task(HasApplyMaterialCommand);

//            taskDbUpdate.Start();
//            lblStatus.Text = "Aplicando Update no Banco...";
//            await taskDbUpdate;

//            taskApplyData.Start();
//            lblStatus.Text = "Aplicando dados no iProperties...";
//            await taskApplyData;

//            taskHasApplyMaterialCommand.Start();
//            lblStatus.Text = "Verificando se foi aplicado Material...";
//            await taskHasApplyMaterialCommand;

//            lblStatus.Text = "";
//        }
//        catch (Exception ex)
//        {
//            InvMsg.Msg("Algum erro ocorreu no processo de aplicação do material \n" + ex);

//            return;
//        }

//        InitializeDataGrid();
//    }

//    /// <summary>
//    ///     Evento botão Aplicar
//    /// </summary>
//    /// <param name="sender"></param>
//    /// <param name="e"></param>
//    private async void btnApplyMp_A_Click_Async(object sender, EventArgs e)
//    {
//        if (checkMpCustomData.Checked)
//        {
//            var result = MessageBox.Show("A Matéria Prima está definida como CUSTOM.\n" +
//                                         "Deseja utilizar a Matéria Prima selecionada?", "Atenção",
//                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

//            if (result == DialogResult.Yes)
//            {
//                checkMpCustomData.Checked = false;
//                await ApplyMaterialExecutionAsync();
//            }
//        }
//        else
//        {
//            await ApplyMaterialExecutionAsync();
//        }
//    }

//    /// <summary>
//    ///     Evento botão Aplicar
//    /// </summary>
//    /// <param name="sender"></param>
//    /// <param name="e"></param>
//    private void btnApplyMp_A_Click(object sender, EventArgs e)
//    {
//        if (checkMpCustomData.Checked)
//        {
//            var result = MessageBox.Show("A Matéria Prima está definida como CUSTOM.\n" +
//                                         "Deseja utilizar a Matéria Prima selecionada?", "Atenção",
//                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

//            if (result == DialogResult.Yes)
//            {
//                checkMpCustomData.Checked = false;
//                ApplyMaterialExecution();
//            }
//        }
//        else
//        {
//            ApplyMaterialExecution();
//        }
//    }

//    /// <summary>
//    /// Aplica o Material de forma Assícrona
//    /// </summary>
//    /// <returns></returns>
//    private async Task ApplyMaterialExecutionAsync()
//    {
//        if (await ApplyMaterialAsync()) return;

//        InitializeDataGrid();

//        UpdateActiveFile();
//    }

//    /// <summary>
//    /// Aplica o material
//    /// </summary>
//    /// <returns></returns>
//    private void ApplyMaterialExecution()
//    {
//        if (ApplyMaterial()) return;

//        InitializeDataGrid();

//        UpdateActiveFile();
//    }

//    private static void UpdateActiveFile()
//    {
//        if (AddinGlobal.InventorApp.ActiveEditDocument.RequiresUpdate)
//        {
//            AddinGlobal.InventorApp.ActiveEditDocument.Update2(true);
//        }
//    }

//    /// <summary>
//    /// Aplica o material
//    /// </summary>
//    /// <returns></returns>
//    private async Task<bool> ApplyMaterialAsync()
//    {
//        if (_selectedDocument == null)
//        {
//            InvMsg.Msg("Não há arquivo associado para aplicação do material. \n " +
//                       "Entre em contato com o Desenvolvedor.");
//            return true;
//        }

//        try
//        {
//            Task taskDbUpdate = new Task(SetDbUpdate);
//            Task taskHasApplyMaterialCommand = new Task(HasApplyMaterialCommand);

//            taskDbUpdate.Start();
//            lblStatus.Text = "Aplicando Update no Banco...";
//            await taskDbUpdate;

//            lblStatus.Text = "Aplicando dados no iProperties...";
//            await Task.Run(() => ApplyPartData(_selectedDocument));

//            taskHasApplyMaterialCommand.Start();
//            lblStatus.Text = "Verificando se foi aplicado Material...";
//            await taskHasApplyMaterialCommand;

//            lblStatus.Text = "";
//        }
//        catch (Exception ex)
//        {
//            InvMsg.Msg("Algum erro ocorreu no processo de aplicação do material \n" + ex);

//            return true;
//        }

//        return false;
//    }

//    /// <summary>
//    /// Aplica a matéria Prima
//    /// </summary>
//    /// <returns></returns>
//    private bool ApplyMaterial()
//    {
//        if (_selectedDocument == null)
//        {
//            InvMsg.Msg("Não há arquivo associado para aplicação do material. \n " +
//                       "Entre em contato com o Desenvolvedor.");
//            return true;
//        }

//        try
//        {
//            lblStatus.Text = "Aplicando Update no Banco...";
//            SetDbUpdate();

//            lblStatus.Text = "Aplicando dados no iProperties...";
//            ApplyPartData(_selectedDocument);

//            lblStatus.Text = "Verificando se foi aplicado Material...";
//            HasApplyMaterialCommand();

//            lblStatus.Text = string.Empty;
//        }
//        catch (Exception ex)
//        {
//            InvMsg.Msg("Algum erro ocorreu no processo de aplicação do material \n" + ex);

//            return true;
//        }

//        return false;
//    }

//    /// <summary>
//    ///     Evento para atualizar o blank da peça
//    /// </summary>
//    /// <param name="sender"></param>
//    /// <param name="e"></param>
//    private void BtnAtualizaBlank_Click(object sender, EventArgs e)
//    {
//        BlankUpdate();
//    }

//    /// <summary>
//    ///     Evento para atualizar o blank da peça solicitada
//    /// </summary>
//    /// <param name="sender"></param>
//    /// <param name="e"></param>
//    private void BtnAtualizaBlank_A_Click(object sender, EventArgs e)
//    {
//        if (_selectedDocument.DocumentType == DocumentTypeEnum.kPartDocumentObject)
//        {
//            BlankUpdate(_selectedDocument);
//        }
//        else
//        {
//            InvMsg.Msg("Para atualização do Blank é necessário selecionar uma peça (ARQUIVO IPT)." +
//                       "\nNão é possível calcular blank de uma montagem (ARQUIVO IAM).");
//        }

//    }

//    /// <summary>
//    /// Update do Blank do arquivo Ativo
//    /// </summary>
//    private void BlankUpdate()
//    {
//        CoreDescription cd = new CoreDescription();

//        SetProperties();
//        txtProp_PC_DescCompleta.Text = UpdateBlank.Execute(txtProp_MP_Familia.Text);
//        txtProp_DT_BlankType.Text = cd.GetDescriptionType(txtProp_MP_Familia.Text);
//        CalculaMateriaPrima();
//    }

//    /// <summary>
//    /// Update do Blank do arquivo solicitado
//    /// </summary>
//    /// <param name="oDoc"></param>
//    private void BlankUpdate(Document oDoc)
//    {
//        CoreDescription cd = new CoreDescription();

//        SetProperties(oDoc);
//        txtProp_PC_DescCompleta.Text = UpdateBlank.Execute(txtProp_MP_Familia.Text,oDoc);
//        txtProp_DT_BlankType.Text = cd.GetDescriptionType(txtProp_MP_Familia.Text);

//        CalculaMateriaPrima(oDoc);
//    }

//    private void BlankUpdate2(DocToProcessBlank document)
//    {
//        CoreDescription cd = new CoreDescription();

//        SetProperties(document.InvDocument);

//        var pcDescCompleta = UpdateBlank.Execute(document.Familia,document.InvDocument);
//        var dtBlankType = cd.GetDescriptionType(document.Familia);

//        CalculaMateriaPrima(document.InvDocument);
//    }

//    public class DocToProcessBlank
//    {
//        public string Familia { get; set; }
//        public Document InvDocument { get; set; }
//    }

//    /// <summary>
//    ///     Evento do botão cancelar
//    /// </summary>
//    /// <param name="sender"></param>
//    /// <param name="e"></param>
//    private void btnCancelar_Click(object sender, EventArgs e)
//    {
//        Close();
//    }

//    /// <summary>
//    ///     Evento do botão Ok
//    /// </summary>
//    /// <param name="sender"></param>
//    /// <param name="e"></param>
//    private void btnOk_A_Click(object sender, EventArgs e)
//    {
//        try
//        {
//            ApplyPartData(_selectedDocument);

//            Close();
//            Dispose(true);

//            UpdateActiveFile();

//        }
//        catch (Exception ex)
//        {
//            CoreLog.WriteLog($"\n - btnOK -> Erro no comando OK \n {ex}");
//        }
//    }

//    /// <summary>
//    ///     Método para limpara todos os textbox
//    /// </summary>
//    private void ClearAllTextBox()
//    {
//        foreach (Control control in Controls)
//            if (control is TextBox)
//                ((TextBox) control).Text = string.Empty;
//    }

//    /// <summary>
//    ///     Evento chamando o Builder de Código e Descrição
//    /// </summary>
//    /// <param name="sender"></param>
//    /// <param name="e"></param>
//    private async void CodDesc_A_Click(object sender, EventArgs e)
//    {
//        try
//        {
//            lblStatus.Text = @"Calculando o Código e a Descrição do Arquivo";
//            var codeDesc = new CodeDescription();

//            await Task.Run(() => codeDesc.GetCodeDescription(_selectedDocument));

//            txtProp_PC_CodDesenho.Text = codeDesc.Code.Trim();
//            txtProp_PC_Desc.Text = codeDesc.Description.Trim();

//            if (_selectedDocument.DocumentType  == DocumentTypeEnum.kAssemblyDocumentObject)
//                txtProp_PC_DescCompleta.Text = codeDesc.Description.Trim();

//            lblStatus.Text = @"Ok";
//        }
//        catch (Exception exception)
//        {
//            CoreLog.WriteLog(exception.ToString());
//        }
//    }

//    /// <summary>
//    ///     Evento do Datagrid para carregamento nos textbox da linnha selecionada
//    /// </summary>
//    /// <param name="e"></param>
//    private void PreencherTextBoxComDadosDaMpFromNewAge(DataGridViewCellEventArgs e)
//    {
//        LimparTextBoxMpDadosMP();

//        var dataNewAgeLocal = new NEWAGE();

//        // Aba Dados Material
//        dataNewAgeLocal.CODIGO = DataGridRowToTextBox_DoubleClick(txtCod, 1, e);
//        dataNewAgeLocal.DESCRICAO = DataGridRowToTextBox_DoubleClick(txtDescErp, 2, e);
//        dataNewAgeLocal.DESCRICAO_INVENTOR = DataGridRowToTextBox_DoubleClick(txtDescInventor, 3, e);
//        dataNewAgeLocal.FAMILIA = DataGridRowToTextBox_DoubleClick(txtFamilia, 4, e);
//        dataNewAgeLocal.UNIDADE = DataGridRowToTextBox_DoubleClick(txtUnidade, 5, e);
//        dataNewAgeLocal.FATOR = DataGridRowToTextBox_DoubleClick(txtFatorUnidade, 6, e);
//        dataNewAgeLocal.ESPESSURA = DataGridRowToTextBox_DoubleClick(txtEspessura, 7, e);

//        dataNewAge = dataNewAgeLocal;

//        // Aba Propriedades da Peça
//        // Verifica se as propriedades de material são CUSTOM
//        if (checkMpCustomData.Checked == false)
//        {
//            DataGridRowToTextBox_DoubleClick(txtProp_MP_Cod, 1, e);
//            DataGridRowToTextBox_DoubleClick(txtProp_MP_DescERP, 2, e);
//            DataGridRowToTextBox_DoubleClick(txtProp_MP_DescInventor, 3, e);
//            DataGridRowToTextBox_DoubleClick(txtProp_MP_Familia, 4, e);
//            DataGridRowToTextBox_DoubleClick(txtProp_MP_Unidade, 5, e);
//            DataGridRowToTextBox_DoubleClick(txtProp_MP_FatorUnidade, 6, e);
//            DataGridRowToTextBox_DoubleClick(txtProp_MP_Espessura, 7, e);
//        }

//        _hasApplyMaterial = false;
//    }

//    /// <summary>
//    ///     Metodo complementar do evento de duplo clique na linha do Datagrid
//    /// </summary>
//    /// <param name="textbox"></param>
//    /// <param name="index"></param>
//    /// <param name="e"></param>
//    private string DataGridRowToTextBox_DoubleClick(TextBox textbox, int index, DataGridViewCellEventArgs e)
//    {
//        try
//        {
//            if (dgView.Rows[e.RowIndex].Equals(-1))
//                return string.Empty;

//            var data = string.Empty;
//            if (dgView.Rows[e.RowIndex].Cells[index].Value != null)
//                data = dgView.Rows[e.RowIndex].Cells[index].Value.ToString().Trim();
//            textbox.Text = data;
//            return data;

//        }
//        catch
//        {
//            return string.Empty;
//        }
//    }

//    /// <summary>
//    ///     Updade do banco de matéria prima
//    /// </summary>
//    private void DbUpdateTable()
//    {
//        var db = new ConnectorDataClassesDataContext(AppConfig.GetSqlConn());

//        var materialDataActual = new NEWAGE();

//        materialDataActual = Enumerable.SingleOrDefault(from ma in db.NEWAGEs
//            where ma.CODIGO.Equals(txtCod.Text)
//            select ma);

//        var materialDataNew = new NEWAGE
//        {
//            ID = Enumerable.SingleOrDefault(
//                from ma in db.NEWAGEs
//                where ma.CODIGO.Equals(txtCod.Text)
//                select ma.ID),
//            CODIGO = txtCod.Text,
//            DESCRICAO = txtDescErp.Text,
//            FAMILIA = txtFamilia.Text
//        };

//        if (txtDescInventor.Text.Trim() != string.Empty)
//            materialDataNew.DESCRICAO_INVENTOR = txtDescInventor.Text.ToUpper().Trim();

//        if (txtUnidade.Text != string.Empty) materialDataNew.UNIDADE = txtUnidade.Text.Trim();

//        if (txtEspessura.Text != string.Empty) materialDataNew.ESPESSURA = txtEspessura.Text.Replace(",", ".");

//        // Verifica se a espessura é nula
//        if (materialDataNew.ESPESSURA == null) materialDataNew.ESPESSURA = "0";

//        if (materialDataActual != null && materialDataActual.ESPESSURA == null) materialDataActual.ESPESSURA = "0";

//        if (txtFatorUnidade.Text != string.Empty)
//            materialDataNew.FATOR = txtFatorUnidade.Text.ToUpper().Replace(",", ".");

//        if (materialDataActual != null &&
//            materialDataNew.ID == materialDataActual.ID &&
//            materialDataNew.CODIGO != null &&
//            materialDataActual.CODIGO != null &&
//            materialDataNew.CODIGO.Trim() == materialDataActual.CODIGO.Trim() &&
//            materialDataNew.DESCRICAO != null &&
//            materialDataActual.DESCRICAO != null &&
//            materialDataNew.DESCRICAO.Trim() == materialDataActual.DESCRICAO.Trim() &&
//            materialDataNew.DESCRICAO_INVENTOR != null &&
//            materialDataActual.DESCRICAO_INVENTOR != null &&
//            materialDataNew.DESCRICAO_INVENTOR.Trim() == materialDataActual.DESCRICAO_INVENTOR.Trim() &&
//            materialDataNew.ESPESSURA != null &&
//            materialDataActual.ESPESSURA != null &&
//            materialDataNew.ESPESSURA.Trim() == materialDataActual.ESPESSURA.Trim() &&
//            materialDataNew.FAMILIA != null &&
//            materialDataActual.FAMILIA != null &&
//            materialDataNew.FAMILIA.Trim() == materialDataActual.FAMILIA.Trim() &&
//            materialDataNew.FATOR != null &&
//            materialDataActual.FATOR != null &&
//            materialDataNew.FATOR.Trim() == materialDataActual.FATOR.Trim() &&
//            materialDataNew.UNIDADE != null &&
//            materialDataActual.UNIDADE != null &&
//            materialDataNew.UNIDADE.Trim() == materialDataActual.UNIDADE.Trim())
//            return;

//        ErpDataAccess.UpdateMaterial(materialDataNew);
//    }

//    /// <summary>
//    ///     Definição do tamanho das colunas
//    /// </summary>
//    private void DgvColumsConfig()
//    {
//        try
//        {
//            dgView.Columns[0].Width = 35;
//            dgView.Columns[0].HeaderText = FilterNames.Id;
//            dgView.Columns[0].SortMode = DataGridViewColumnSortMode.Automatic;
//            dgView.Columns[1].Width = 60;
//            dgView.Columns[1].HeaderText = FilterNames.Codigo;
//            dgView.Columns[1].SortMode = DataGridViewColumnSortMode.Automatic;
//            dgView.Columns[2].Width = 400;
//            dgView.Columns[2].HeaderText = FilterNames.Descricao;
//            dgView.Columns[2].SortMode = DataGridViewColumnSortMode.Automatic;
//            //dgView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
//            //dgView.AutoResizeColumns();
//            if (dgView.Columns[3].Width != 220)
//            {
//                dgView.Columns[3].Width = 220;
//                dgView.Columns[3].HeaderText = FilterNames.DescricaoInventor;
//                dgView.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
//                dgView.Columns[3].SortMode = DataGridViewColumnSortMode.Automatic;
//            }

//            dgView.Columns[4].Width = 150;
//            dgView.Columns[4].HeaderText = FilterNames.Familia;
//            dgView.Columns[4].SortMode = DataGridViewColumnSortMode.Automatic;
//            dgView.Columns[5].Width = 60;
//            dgView.Columns[5].HeaderText = FilterNames.Unidade;
//            dgView.Columns[5].SortMode = DataGridViewColumnSortMode.Automatic;
//            dgView.Columns[6].Width = 45;
//            dgView.Columns[6].HeaderText = FilterNames.FatorConversao;
//            dgView.Columns[6].SortMode = DataGridViewColumnSortMode.Automatic;
//            dgView.Columns[7].Width = 45;
//            dgView.Columns[7].HeaderText = FilterNames.Espessura;
//            dgView.Columns[7].SortMode = DataGridViewColumnSortMode.Automatic;
//        }
//        catch (Exception e)
//        {
//            CoreLog.WriteLog($"\nErro ao Organizar o Data GRID :\n   -> Erro: {e.Message}\n   -> InnerException: {e.InnerException}");
//        }
//    }


//    /// <summary>
//    ///     Evento duplo cique na grade
//    /// </summary>
//    /// <param name="sender"></param>
//    /// <param name="e"></param>
//    private void dgView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
//    {
//        PreencherTextBoxComDadosDaMpFromNewAge(e);
//    }

//    /// <summary>
//    ///     Filtro dos dados para a pesquisa
//    /// </summary>
//    private void FiltrarDadosLocal()
//    {
//        try
//        {
//            var finder = txtFind.Text;

//            var filterresult = new List<NEWAGE>();

//            switch (cbxFindIn.Text)
//            {
//                case "CODIGO":
//                    filterresult = ErpData.Where(i => i.CODIGO.Contains(finder)).OrderBy(i => i.CODIGO).ToList();
//                    break;
//                case "DESCRICAO":

//                    var filterTempResult = new List<NEWAGE>();

//                    foreach (NEWAGE m in ErpData)
//                    {
//                        if (!string.IsNullOrEmpty(m.DESCRICAO))
//                        {
//                            filterTempResult.Add(m);
//                        }
//                    }

//                    filterresult = filterTempResult.Where(i => i.DESCRICAO.Contains(finder)).OrderBy(i => i.DESCRICAO).ToList();
//                    break;
//                case "DESCRICAO_INVENTOR":
//                    filterresult = ErpData.Where(i => i.DESCRICAO_INVENTOR.Contains(finder)).OrderBy(i => i.DESCRICAO_INVENTOR).ToList();
//                    break;
//                case "FAMILIA":
//                    filterresult = ErpData.Where(i => i.FAMILIA.Contains(finder)).OrderBy(i => i.FAMILIA).ToList();
//                    break;
//                case "ESPESSURA":
//                    filterresult = ErpData.Where(i => i.ESPESSURA.Contains(finder)).OrderBy(i => i.ESPESSURA).ToList();
//                    break;
//            }

//            if (!filterresult.Any())
//            {
//                filterresult = ErpData;
//            }

//            dgView.DataSource = filterresult;
//            lblFindCount.Text = $"Itens na lista: {dgView.Rows.Count}";
//            DgvColumsConfig();

//        }
//        catch (Exception ex)
//        {
//            CoreLog.WriteLog(
//                $"--------" +
//                $"Erro no Filtro de Dados:\n -> " +
//                $" {ex.ToString()} <-\n " +
//                $"\n-------"
//                );

//            //InvMsg.Msg("Erro ao Filtrar os Dados.");
//        }
//    }

//    /// <summary>
//    /// Filtra os dados na tabela de Matéria Prima
//    /// </summary>
//    private void FiltrarDados()
//    {
//        try
//        {
//            var finder = TxtFinder.Text;
//            var filterresult = new List<NEWAGE>();

//            if (RadioMpDesc.Checked)
//            {
//                var filterTempResult = new List<NEWAGE>();

//                foreach (NEWAGE m in ErpData)
//                {
//                    if (!string.IsNullOrEmpty(m.DESCRICAO))
//                    {
//                        filterTempResult.Add(m);
//                    }
//                }
//                filterresult = filterTempResult.Where(i => i.DESCRICAO.Contains(finder)).OrderBy(i => i.DESCRICAO).ToList();
//            }

//            if (RadioMpCode.Checked)
//            {
//                filterresult = ErpData.Where(i => i.CODIGO.Contains(finder)).OrderBy(i => i.CODIGO).ToList();
//            }

//            if (!filterresult.Any())
//            {
//                filterresult = ErpData;
//            }

//            dgView.DataSource = filterresult;
//            lblFindCount.Text = $"Itens na lista: {dgView.Rows.Count}";
//            DgvColumsConfig();
//        }
//        catch (Exception ex)
//        {
//            CoreLog.WriteLog(
//                $"--------" +
//                $"Erro no Filtro de Dados:\n -> " +
//                $" {ex.ToString()} <-\n " +
//                $"\n-------"
//                );
//        }
//    }

//    /// <summary>
//    ///     Carrega os dados do iProperties para os TextBox
//    /// </summary>
//    private void GetPropertiesToTextBox()
//    {
//        _iProperties.GetAllCadModelProperties();

//        if (_iProperties != null)
//        {
//            //
//            // TextBox Dados MP Aba1
//            // Verifica se a propriedade MP_CUSTOM é falsa
//            if (_iProperties.MpCustom == "0")
//            {
//                checkMpCustomData.Checked = false;
//                TxtBox.SetToTextBox(txtCod, _iProperties.MpCod);
//                TxtBox.SetToTextBox(txtFamilia, _iProperties.MpFamilia);
//                TxtBox.SetToTextBox(txtDescErp, _iProperties.MpDescErp);
//                TxtBox.SetToTextBox(txtDescInventor, _iProperties.MpDescInventor);
//                TxtBox.SetToTextBox(txtUnidade, _iProperties.MpUnidade);
//                TxtBox.SetToTextBox(txtFatorUnidade, _iProperties.MpFator);
//            }
//            else
//            {
//                checkMpCustomData.Checked = true;
//            }

//            //
//            // Matéria Prima Aba Propriedades
//            //
//            TxtBox.SetToTextBox(txtProp_MP_Cod, _iProperties.MpCod);
//            TxtBox.SetToTextBox(txtProp_MP_Familia, _iProperties.MpFamilia);
//            TxtBox.SetToTextBox(txtProp_MP_DescERP, _iProperties.MpDescErp);
//            TxtBox.SetToTextBox(txtProp_MP_DescInventor, _iProperties.MpDescInventor);
//            TxtBox.SetToTextBox(txtProp_MP_Unidade, _iProperties.MpUnidade);
//            TxtBox.SetToTextBox(txtProp_MP_FatorUnidade, _iProperties.MpFator);

//            //
//            // Dados Equipamento
//            //
//            TxtBox.SetToTextBox(txtProp_MQ_Cod, _iProperties.MqCodigo);
//            TxtBox.SetToTextBox(txtProp_MQ_Nome, _iProperties.MqNome);

//            //
//            // Dados da Peça
//            //
//            //TxtBox.SetToTextBox(txtProp_PC_Nome, _iProperties.PcNome);
//            //TxtBox.SetToTextBox(txtProp_PC_NumDesenho, _iProperties.PcNumDesenho);
//            TxtBox.SetToTextBox(txtProp_PC_CodDesenho, _iProperties.PcCodDesenho);
//            TxtBox.SetToTextBox(txtProp_PC_Desc, _iProperties.PcDesc);
//            TxtBox.SetToTextBox(txtProp_PC_DescCompleta, _iProperties.PcDescCompleta);
//            TxtBox.SetToTextBox(txtProp_PC_NumRevisao, _iProperties.PcNumRev);

//            // Propriedade Bool
//            checkPcDescCustom.Checked = CheckState(_iProperties.PcDescCompletaCustom);

//            //
//            // Dados Projetista
//            //
//            TxtBox.SetToTextBox(txtProp_RP_NomeAprovador, _iProperties.RpNomeAprovador);
//            TxtBox.SetToTextBox(txtProp_RP_NomeDesenhista, _iProperties.RpNomeDesenhista);
//            TxtBox.SetToTextBox(txtProp_RP_NomeProjetista, _iProperties.RpNomeProjetista);

//            // 
//            // Dados Técnicos
//            //
//            TxtBox.SetToTextBox(txtProp_DT_PesoAcabado, _iProperties.DtPesoAcabado);
//            TxtBox.SetToTextBox(txtProp_DT_PesoBruto, _iProperties.PcPesoBruto);
//            TxtBox.SetToTextBox(txtProp_DT_DimBlank, _iProperties.PcDimBlank);
//            TxtBox.SetToTextBox(txtProp_DT_BlankType, _iProperties.BlankType);

//            //
//            // Pintura
//            TxtBox.SetToTextBox(txtProp_DT_AreaPintura, _iProperties.PcPinturaArea);
//            cbxProp_DT_TipoPintura.SelectedIndex = cbxProp_DT_TipoPintura.FindStringExact(_iProperties.PcPinturaTipo);
//            checkPintura.Checked = CheckState(_iProperties.PcPinturaInternaDiferente);
//            cbxProp_DT_PinturaAcabamento.SelectedIndex =
//                cbxProp_DT_PinturaAcabamento.FindStringExact(_iProperties.PcPinturaAcabamento);

//            //
//            // Coleta a espessura da peça se for um SheetMetal
//            if (!InvSheetMetal.IsSheetMetalPart())
//            {
//                //return true;
//            }
//            else
//            {
//                VerificaDefineEspessura();
//            }

//        }
//        else
//        {
//            //return false;
//        }

//    }

//    /// <summary>
//    /// Carrega para o Textbox os dados do iProperties
//    /// </summary>
//    /// <param name="oDoc"></param>
//    private void GetPropertiesToTextBox(Document oDoc)
//    {
//        _iProperties = _iProperties.GetAllCadModelProperties(oDoc);

//        if (_iProperties != null)
//        {
//            LoadDataToControls(_iProperties);
//        }
//    }

//    /// <summary>
//    /// Carregamento de dados para o Form
//    /// </summary>
//    /// <param name="_iProperties"></param>
//    private void LoadDataToControls(CadProperties.CadModelProperties _iProperties)
//    {
//        try
//        {
//            TxtBox.SetToTextBox(txtCod, _iProperties.MpCod);
//            TxtBox.SetToTextBox(txtFamilia, _iProperties.MpFamilia);
//            TxtBox.SetToTextBox(txtDescErp, _iProperties.MpDescErp);
//            TxtBox.SetToTextBox(txtDescInventor, _iProperties.MpDescInventor);
//            TxtBox.SetToTextBox(txtUnidade, _iProperties.MpUnidade);
//            TxtBox.SetToTextBox(txtFatorUnidade, _iProperties.MpFator);

//            //
//            // Matéria Prima Aba Propriedades
//            //
//            TxtBox.SetToTextBox(txtProp_MP_Cod, _iProperties.MpCod);
//            TxtBox.SetToTextBox(txtProp_MP_Familia, _iProperties.MpFamilia);
//            TxtBox.SetToTextBox(txtProp_MP_DescERP, _iProperties.MpDescErp);
//            TxtBox.SetToTextBox(txtProp_MP_DescInventor, _iProperties.MpDescInventor);
//            TxtBox.SetToTextBox(txtProp_MP_Unidade, _iProperties.MpUnidade);
//            TxtBox.SetToTextBox(txtProp_MP_FatorUnidade, _iProperties.MpFator);

//            //
//            // Dados Equipamento
//            //
//            TxtBox.SetToTextBox(txtProp_MQ_Cod, _iProperties.MqCodigo);
//            TxtBox.SetToTextBox(txtProp_MQ_Nome, _iProperties.MqNome);

//            //
//            // Dados da Peça
//            //
//            TxtBox.SetToTextBox(txtProp_PC_CodDesenho, _iProperties.PcCodDesenho);
//            TxtBox.SetToTextBox(txtProp_PC_Desc, _iProperties.PcDesc);
//            TxtBox.SetToTextBox(txtProp_PC_DescCompleta, _iProperties.PcDescCompleta);
//            TxtBox.SetToTextBox(txtProp_PC_NumRevisao, _iProperties.PcNumRev);

//            // Propriedade Bool
//            checkPcDescCustom.Checked = CheckState(_iProperties.PcDescCompletaCustom);

//            //
//            // Dados Projetista
//            //
//            TxtBox.SetToTextBox(txtProp_RP_NomeAprovador, _iProperties.RpNomeAprovador);
//            TxtBox.SetToTextBox(txtProp_RP_NomeDesenhista, _iProperties.RpNomeDesenhista);
//            TxtBox.SetToTextBox(txtProp_RP_NomeProjetista, _iProperties.RpNomeProjetista);

//            // 
//            // Dados Técnicos
//            //
//            TxtBox.SetToTextBox(txtProp_DT_PesoAcabado, _iProperties.DtPesoAcabado);
//            TxtBox.SetToTextBox(txtProp_DT_PesoBruto, _iProperties.PcPesoBruto);
//            TxtBox.SetToTextBox(txtProp_DT_DimBlank, _iProperties.PcDimBlank);
//            TxtBox.SetToTextBox(txtProp_DT_BlankType, _iProperties.BlankType);

//            //
//            // Pintura
//            TxtBox.SetToTextBox(txtProp_DT_AreaPintura, _iProperties.PcPinturaArea);
//            cbxProp_DT_TipoPintura.SelectedIndex =
//                cbxProp_DT_TipoPintura.FindStringExact(_iProperties.PcPinturaTipo);
//            checkPintura.Checked = CheckState(_iProperties.PcPinturaInternaDiferente);
//            cbxProp_DT_PinturaAcabamento.SelectedIndex =
//                cbxProp_DT_PinturaAcabamento.FindStringExact(_iProperties.PcPinturaAcabamento);

//            //
//            // Coleta a espessura da peça se for um SheetMetal
//            if (!InvSheetMetal.IsSheetMetalPart())
//                return;

//            VerificaDefineEspessura();
//        }
//        catch (Exception ex)
//        {
//            CoreLog.WriteLog(
//                $"--------" +
//                $"Erro ao recuperar Propriedades:\n -> " +
//                $" {ex.ToString()} <-\n " +
//                $"\n-------"
//            );
//        }
//    }

//    /// <summary>
//    /// Verifica as espessuras
//    /// </summary>
//    private void VerificaDefineEspessura()
//    {
//        try
//        {
//            var espessura = InvSheetMetal.GetThickness().ToString();

//            if (_iProperties.Thickness != 0)
//            {
//                if (_iProperties.Thickness != InvSheetMetal.GetThickness())
//                {
//                    InvMsg.Msg($"A espessura configurada da peça parece estar diferente da do material aplicado.\n\n" +
//                               $"-> Espessura Configurada Inventor: ( {espessura} ).\n" +
//                               $"-> Espessura do Material ERP: ( {_iProperties.Thickness} ).\n\n" +
//                               $"Será aplicado a espessura configurada ( {espessura} )Verifique essas informaçãoes antes de finalizar a aplicação do material.");
//                }
//            }

//            txtEspessura.Text = espessura;
//            txtProp_MP_Espessura.Text = espessura;
//        }
//        catch (Exception e)
//        {
//            CoreLog.WriteLog(e.ToString());
//            InvMsg.Msg($"Erro ao verificar a espessura! \n\n {e}");
//        }
//    }

//    /// <summary>
//    ///     Método que verifica se o material já foi aplicado
//    /// </summary>
//    private void HasApplyMaterial()
//    {
//        if (_hasApplyMaterial)
//            return;

//        //string userName = SystemInformation.UserName;
//        lblAlert.Text =
//            "Matéria Prima não Aplicada.";

//    }

//    /// <summary>
//    ///     Método para verificar se o material novo selecionad já foi aplicado
//    /// </summary>
//    private void HasApplyMaterialCommand()
//    {
//        _hasApplyMaterial = true;
//        lblAlert.Text = string.Empty;
//    }

//    /// <summary>
//    ///     Inicialização do DataGrid
//    /// </summary>
//    private void InitializeDataGrid()
//    {
//        try
//        {
//            _dataSet = ErpDataAccess.GetAllErpMaterialData();
//            ErpData = _dataSet.Cast<NEWAGE>().ToList();

//            dgView.AutoGenerateColumns = true;
//            dgView.DataSource = _dataSet;
//            dgView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
//            DgvColumsConfig();

//            // Combobox
//            cbxFindIn.Items.Clear();
//            cbxFindIn.Items.Add(FilterNames.FilterCodigo);
//            cbxFindIn.Items.Add(FilterNames.FilterDescricao);
//            cbxFindIn.Items.Add(FilterNames.FilterDescInventor);
//            cbxFindIn.Items.Add(FilterNames.FilterFamilia);
//            cbxFindIn.Items.Add(FilterNames.FilterEspessura);
//            cbxFindIn.SelectedIndex = 1;

//            // ReSharper disable once LocalizableElement
//            lblFindCount.Text = $"Itens na lista: {dgView.Rows.Count}";
//        }
//        catch (Exception e)
//        {
//            CoreLog.WriteLog($" ERRO-INITIALIZE DATAGRID \n {e} ");
//            dgView.DataSource = ErpData.Any() ? ErpData.AsQueryable() : null;
//        }
//    }

//    /// <summary>
//    ///     Método Load
//    /// </summary>
//    private async Task LoadFormActionAsync()
//    {
//        progressBar.Style = ProgressBarStyle.Marquee;
//        progressBar.MarqueeAnimationSpeed = 50;

//        TransactionManager trxManager = AddinGlobal.InventorApp.TransactionManager;
//        Transaction trx =
//            trxManager.StartTransaction(AddinGlobal.InventorApp.ActiveEditDocument, "New Age Connector");

//        try
//        {
//            LoadDocument();

//            lblStatus.Text = "Carregando Dados da Peça...";

//            GetPropertiesToTextBox();

//            //Task getProper = new Task(GetPropertiesToTextBox);
//            //getProper.Start();
//            //await getProper;

//            lblStatus.Text = "Dados Carregados...";
//            trx.End();
//            lblStatus.Text = "";
//        }
//        catch (Exception ex)
//        {
//            trx.Abort();
//            InvMsg.Msg("Houveram erros no carregamnto dos dados para o Form.\n\n " + ex);
//            CoreLog.WriteLog($"Erros no carregamnto dos dados para o Form. \n\t{ex}");
//        }

//        progressBar.MarqueeAnimationSpeed = 0;
//        progressBar.Style = ProgressBarStyle.Blocks;
//        progressBar.Value = progressBar.Minimum;
//    }

//    /// <summary>
//    ///     Método Load
//    /// </summary>
//    private async Task LoadFormActionAsync(Document oDoc)
//    {
//        progressBar.Style = ProgressBarStyle.Marquee;
//        progressBar.MarqueeAnimationSpeed = 50;

//        TransactionManager trxManager = AddinGlobal.InventorApp.TransactionManager;
//        Transaction trx =
//            trxManager.StartTransaction(AddinGlobal.InventorApp.ActiveEditDocument, "New Age Connector");

//        try
//        {
//            _selectedDocument = oDoc;
//            lblFileName.Text = oDoc.DisplayName;

//            lblStatus.Text = "Carregando Dados da Peça...";

//            await Task.Run(() => GetPropertiesToTextBox(oDoc));
//            lblStatus.Text = @"Dados Carregados...";
//            trx.End();
//            lblStatus.Text = "";
//        }
//        catch (Exception ex)
//        {
//            trx.Abort();
//            InvMsg.Msg("Houveram erros no carregamnto dos dados para o Form.\n\n " + ex);
//        }

//        progressBar.MarqueeAnimationSpeed = 0;
//        progressBar.Style = ProgressBarStyle.Blocks;
//        progressBar.Value = progressBar.Minimum;
//    }

//    /// <summary>
//    ///     Evento Load
//    /// </summary>
//    /// <param name="sender"></param>
//    /// <param name="e"></param>
//    private async void MainFormLoad(object sender, EventArgs e)
//    {
//        _hasApplyMaterial = false;

//        cbxProp_DT_TipoPintura.DataSource = paintHelper.PaintTypesList;

//        foreach (var acabamento in paintData.PaintAcabamentoList)
//        {
//            cbxProp_DT_PinturaAcabamento.Items.Add(acabamento);
//        }

//        var docVerified = VerifyDocSelected();
//        if (docVerified == null)
//        {
//            await LoadFormActionAsync();
//        }
//        else
//        {
//            await LoadFormActionAsync(docVerified);
//        }
//    }

//    /// <summary>
//    ///     Update do banco de matéria prima
//    /// </summary>
//    private void SetDbUpdate()
//    {
//        try
//        {
//            DbUpdateTable();

//            //InitializeDataGrid();
//        }
//        catch (Exception ex)
//        {
//            InvMsg.Msg("Erro ao Salvar no Database. \n\n" + ex);
//        }
//    }

//    /// <summary>
//    ///     Update do banco de matéria prima sem inicializar o Datagrid
//    /// </summary>
//    private void SetDbUpdate2()
//    {
//        try
//        {
//            DbUpdateTable();
//        }
//        catch (Exception ex)
//        {
//            InvMsg.Msg("Erro ao Salvar no Database. \n\n" + ex);
//        }
//    }

//    /// <summary>
//    ///     Carrega os dados no iProperties
//    /// </summary>
//    private void SetProperties()
//    {
//        try
//        {
//            // Matéria Prima
//            InvProps.SetInvIProperties(Prop.MP_COD, txtCod.Text, InvPropetiesGroup.CustomFields);
//            InvProps.SetInvIProperties(Prop.MP_FAMILIA, txtFamilia.Text, InvPropetiesGroup.CustomFields);
//            InvProps.SetInvIProperties(Prop.MP_DESC_ERP, txtDescErp.Text, InvPropetiesGroup.CustomFields);
//            InvProps.SetInvIProperties(Prop.MP_DESC_INVENTOR, txtDescInventor.Text, InvPropetiesGroup.CustomFields);
//            InvProps.SetInvIProperties(Prop.MP_UNIDADE, txtUnidade.Text, InvPropetiesGroup.CustomFields);
//            InvProps.SetInvIProperties(Prop.MP_FATOR, txtFatorUnidade.Text.Replace('.', ',').Trim(),
//                InvPropetiesGroup.CustomFields);

//            // Máquina
//            InvProps.SetInvIProperties(Prop.MQ_COD, txtProp_MQ_Cod.Text, InvPropetiesGroup.CustomFields);
//            InvProps.SetInvIProperties(Prop.MQ_NOME, txtProp_MQ_Nome.Text, InvPropetiesGroup.CustomFields);

//            // Peça
//            InvProps.SetInvIProperties(Prop.PC_COD_DESENHO, txtProp_PC_CodDesenho.Text, InvPropetiesGroup.CustomFields);
//            InvProps.SetInvIProperties(Prop.PC_DESC, txtProp_PC_Desc.Text, InvPropetiesGroup.CustomFields);
//            InvProps.SetInvIProperties(Prop.PC_DESC_COMPLETA, txtProp_PC_DescCompleta.Text,
//                InvPropetiesGroup.CustomFields);

//            InvProps.SetInvIProperties(Prop.PC_DESC_COMPLETA_CUSTOM, CheckState(checkPcDescCustom),
//                InvPropetiesGroup.CustomFields);
//            InvProps.SetInvIProperties(Prop.PC_NUM_REV, txtProp_PC_NumRevisao.Text,
//                InvPropetiesGroup.InventorSumaryInformation);

//            // Desenhistas
//            InvProps.SetInvIProperties(Prop.NOME_APROVADOR, txtProp_RP_NomeAprovador.Text,
//                InvPropetiesGroup.CustomFields);
//            InvProps.SetInvIProperties(Prop.NOME_DESENHISTA, txtProp_RP_NomeDesenhista.Text,
//                InvPropetiesGroup.CustomFields);
//            InvProps.SetInvIProperties(Prop.NOME_PROJETISTA, txtProp_RP_NomeProjetista.Text,
//                InvPropetiesGroup.CustomFields);

//            // BlankType
//            InvProps.SetInvIProperties(Prop.BLANK_TYPE, txtProp_DT_BlankType.Text, InvPropetiesGroup.CustomFields);
//            InvProps.SetInvIProperties(Prop.MP_PESO_BRUTO, txtProp_DT_PesoBruto.Text, InvPropetiesGroup.CustomFields);

//            // Pintura
//            InvProps.SetInvIProperties(Prop.PC_TIPO_PINTURA, cbxProp_DT_TipoPintura.Text, InvPropetiesGroup.CustomFields);
//            InvProps.SetInvIProperties(Prop.PC_AREA_PINTURA, txtProp_DT_AreaPintura.Text, InvPropetiesGroup.CustomFields);
//            InvProps.SetInvIProperties(Prop.PC_ACABAMENTO_PINTURA, cbxProp_DT_PinturaAcabamento.Text, InvPropetiesGroup.CustomFields);

//            InvProps.SetInvIProperties(Prop.PC_PINTURA_INTERNA_DIFERENTE, CheckState(checkPintura), InvPropetiesGroup.CustomFields);
//        }
//        catch (Exception ex)
//        {
//            CoreLog.WriteLog(
//                $"--------" +
//                $"Erro ao Aplicar Propriedades:\n -> " +
//                $" {ex} <-\n " +
//                $"\n-------"
//            );
//        }
//    }

//    /// <summary>
//    ///     Carrega os dados no iProperties
//    /// </summary>
//    private void SetProperties(Document oDoc)
//    {
//        try
//        {
//            // Matéria Prima
//            InvProps.SetInvIProperties(Prop.MP_CUSTOM, CheckState(checkMpCustomData), InvPropetiesGroup.CustomFields, oDoc);

//            InvProps.SetInvIProperties(Prop.MP_COD, txtProp_MP_Cod.Text, InvPropetiesGroup.CustomFields, oDoc);
//            InvProps.SetInvIProperties(Prop.MP_FAMILIA, txtProp_MP_Familia.Text, InvPropetiesGroup.CustomFields, oDoc);
//            InvProps.SetInvIProperties(Prop.MP_DESC_ERP, txtProp_MP_DescERP.Text, InvPropetiesGroup.CustomFields, oDoc);
//            InvProps.SetInvIProperties(Prop.MP_DESC_INVENTOR, txtProp_MP_DescInventor.Text, InvPropetiesGroup.CustomFields, oDoc);
//            InvProps.SetInvIProperties(Prop.MP_UNIDADE, txtProp_MP_Unidade.Text, InvPropetiesGroup.CustomFields, oDoc);
//            InvProps.SetInvIProperties(Prop.MP_FATOR, txtProp_MP_FatorUnidade.Text.Replace('.', ',').Trim(),
//                InvPropetiesGroup.CustomFields, oDoc);

//            // Máquina
//            InvProps.SetInvIProperties(Prop.MQ_COD, txtProp_MQ_Cod.Text, InvPropetiesGroup.CustomFields, oDoc);
//            InvProps.SetInvIProperties(Prop.MQ_NOME, txtProp_MQ_Nome.Text, InvPropetiesGroup.CustomFields, oDoc);

//            // Peça
//            InvProps.SetInvIProperties(Prop.PC_COD_DESENHO, txtProp_PC_CodDesenho.Text, InvPropetiesGroup.CustomFields, oDoc);
//            InvProps.SetInvIProperties(Prop.PC_DESC, txtProp_PC_Desc.Text, InvPropetiesGroup.CustomFields, oDoc);
//            InvProps.SetInvIProperties(Prop.PC_DESC_COMPLETA, txtProp_PC_DescCompleta.Text,
//                InvPropetiesGroup.CustomFields, oDoc);

//            InvProps.SetInvIProperties(Prop.PC_DESC_COMPLETA_CUSTOM, CheckState(checkPcDescCustom),
//                InvPropetiesGroup.CustomFields, oDoc);

//            InvProps.SetInvIProperties(Prop.PC_NUM_REV, txtProp_PC_NumRevisao.Text,
//                InvPropetiesGroup.InventorSumaryInformation, oDoc);

//            // Desenhistas
//            InvProps.SetInvIProperties(Prop.NOME_APROVADOR, txtProp_RP_NomeAprovador.Text,
//                InvPropetiesGroup.CustomFields, oDoc);
//            InvProps.SetInvIProperties(Prop.NOME_DESENHISTA, txtProp_RP_NomeDesenhista.Text,
//                InvPropetiesGroup.CustomFields, oDoc);
//            InvProps.SetInvIProperties(Prop.NOME_PROJETISTA, txtProp_RP_NomeProjetista.Text,
//                InvPropetiesGroup.CustomFields, oDoc);

//            // BlankType
//            InvProps.SetInvIProperties(Prop.BLANK_TYPE, txtProp_DT_BlankType.Text, InvPropetiesGroup.CustomFields, oDoc);
//            InvProps.SetInvIProperties(Prop.MP_PESO_BRUTO, txtProp_DT_PesoBruto.Text, InvPropetiesGroup.CustomFields, oDoc);

//            // Pintura
//            InvProps.SetInvIProperties(Prop.PC_TIPO_PINTURA, cbxProp_DT_TipoPintura.Text, InvPropetiesGroup.CustomFields, oDoc);
//            InvProps.SetInvIProperties(Prop.PC_AREA_PINTURA, txtProp_DT_AreaPintura.Text, InvPropetiesGroup.CustomFields, oDoc);
//            InvProps.SetInvIProperties(Prop.PC_ACABAMENTO_PINTURA, cbxProp_DT_PinturaAcabamento.Text, InvPropetiesGroup.CustomFields, oDoc);

//            InvProps.SetInvIProperties(Prop.PC_PINTURA_INTERNA_DIFERENTE, CheckState(checkPintura), InvPropetiesGroup.CustomFields, oDoc);
//        }
//        catch (Exception ex)
//        {
//            CoreLog.WriteLog(
//                $"--------" +
//                $"Erro ao Aplicar Propriedades:\n -> " +
//                $" {ex.ToString()} <-\n " +
//                $"\n-------"
//            );
//        }

//    }

//    /// <summary>
//    /// Verifica o Status do Checkbox
//    /// </summary>
//    /// <param name="checkBox"></param>
//    /// <returns></returns>
//    public string CheckState(CheckBox checkBox)
//    {
//        return checkBox.Checked ? "1" : "0";
//    }

//    /// <summary>
//    /// Verifica o Status do Checkbox
//    /// </summary>
//    /// <param name="checkBox"></param>
//    /// <returns></returns>
//    public bool CheckState(string checkBox)
//    {
//        if (checkBox.Equals("1"))
//        {
//            return true;
//        }
//        else
//        {
//            return false;
//        }
//    }

//    /// <summary>
//    ///     Define valores de matéria prima para a peça
//    /// </summary>
//    private void CalculaMateriaPrima()
//    {
//        // Documento atual e Massa
//        PartDocument partDoc = (PartDocument) AddinGlobal.InventorApp.ActiveEditDocument;
//        MassProperties mass = partDoc.ComponentDefinition.MassProperties;

//        // Cria um BomData para coletar os dados necessários para Cálculo
//        BomData bc = new BomData
//        {
//            Blank = InvProps.GetInventorCustomProperties(Prop.CH_DIM_BLANK),
//            Fator = txtFatorUnidade.Text.Trim(),
//            PesoAcabado = Math.Round(Convert.ToDouble(mass.Mass), 2)
//        };


//        // Peso Bruto
//        if (txtProp_DT_BlankType.Text == BlankType.SheetMetal.ToString()) bc.BlankType = BlankType.SheetMetal;
//        if (txtProp_DT_BlankType.Text == BlankType.Cylinder.ToString()) bc.BlankType = BlankType.Cylinder;
//        if (txtProp_DT_BlankType.Text == BlankType.Linear.ToString()) bc.BlankType = BlankType.Linear;

//        bc.PesoBruto = BomCalc.Calcule(bc);

//        // Peso Bruto
//        TxtBox.SetToTextBox(txtProp_DT_PesoBruto, bc.PesoBruto + " KG");

//        // Blank
//        TxtBox.SetToTextBox(txtProp_DT_DimBlank, bc.Blank);

//        // Peso Acabado
//        TxtBox.SetToTextBox(txtProp_DT_PesoAcabado, bc.PesoAcabado + " KG");

//        // Salva nas propriedades
//        InvProps.SetInvIProperties(Prop.PC_PESO_ACABADO, txtProp_DT_PesoAcabado.Text,
//            InvPropetiesGroup.CustomFields);
//        InvProps.SetInvIProperties(Prop.MP_PESO_BRUTO, txtProp_DT_PesoBruto.Text, InvPropetiesGroup.CustomFields);
//    }



//    class MpToCalc
//    {
//        public Document CadDocument { get; set; }

//        public BomData CadBomData { get; set; }

//        //public BlankType PartCalcBlankType { get; set; }

//        public CadBlankResult BlankResult { get; set; }

//        public MpToCalc(Document document)
//        {
//            CadDocument = document;
//            GetBlankData();
//        }

//        public void GetBlankData()
//        {
//            CadBomData = new BomData
//            {
//                Blank = InvProps.GetInventorCustomProperties(Prop.CH_DIM_BLANK, CadDocument),
//                Fator = InvProps.GetInventorCustomProperties(Prop.MP_FATOR, CadDocument),
//                PesoAcabado = Math.Round(Convert.ToDouble(((PartDocument)CadDocument).ComponentDefinition.MassProperties.Mass), 2)
//            };

//            var blankType = InvProps.GetInventorCustomProperties(Prop.BLANK_TYPE, CadDocument);

//            // Peso Bruto
//            if (blankType == BlankType.SheetMetal.ToString()) CadBomData.BlankType = BlankType.SheetMetal;
//            if (blankType == BlankType.Cylinder.ToString()) CadBomData.BlankType = BlankType.Cylinder;
//            if (blankType == BlankType.Linear.ToString()) CadBomData.BlankType = BlankType.Linear;

//            CadBomData.PesoBruto = BomCalc.Calcule(CadBomData, CadDocument);
//        }

//    }

//    class CadBlankResult
//    {
//        public string ValorBruto { get; set; }
//        public string Blank { get; set; }
//        public string PesoAcabado{ get; set; }
//    }

//    /// <summary>
//    ///     Define valores de matéria prima para a peça indicada
//    /// </summary>
//    private void CalculaMateriaPrima(Document oDoc)
//    {
//        // Documento atual e Massa
//        PartDocument partDoc = (PartDocument) oDoc;
//        MassProperties mass = partDoc.ComponentDefinition.MassProperties;

//        // Cria um BomData para coletar os dados necessários para Cálculo
//        BomData bc = new BomData
//        {
//            Blank = InvProps.GetInventorCustomProperties(Prop.CH_DIM_BLANK,oDoc),
//            Fator = txtFatorUnidade.Text.Trim(),
//            PesoAcabado = Math.Round(Convert.ToDouble(mass.Mass), 2)
//        };

//        // Peso Bruto
//        if (txtProp_DT_BlankType.Text == BlankType.SheetMetal.ToString()) bc.BlankType = BlankType.SheetMetal;
//        if (txtProp_DT_BlankType.Text == BlankType.Cylinder.ToString()) bc.BlankType = BlankType.Cylinder;
//        if (txtProp_DT_BlankType.Text == BlankType.Linear.ToString()) bc.BlankType = BlankType.Linear;

//        bc.PesoBruto = BomCalc.Calcule(bc,oDoc);

//        // Peso Bruto
//        TxtBox.SetToTextBox(txtProp_DT_PesoBruto, bc.PesoBruto + " KG");

//        // Blank
//        TxtBox.SetToTextBox(txtProp_DT_DimBlank, bc.Blank);

//        // Peso Acabado
//        TxtBox.SetToTextBox(txtProp_DT_PesoAcabado, bc.PesoAcabado + " KG");

//        partPaint.Area = Math.Round(mass.Area, 2);
//        partPaint.PaintType = cbxProp_DT_TipoPintura.Text;

//        // Área de Pintura
//        TxtBox.SetToTextBox(txtProp_DT_AreaPintura, paintHelper.SetArea(partPaint).ToString());

//        // Salva nas propriedades
//        InvProps.SetInvIProperties(Prop.PC_PESO_ACABADO, txtProp_DT_PesoAcabado.Text,
//            InvPropetiesGroup.CustomFields,oDoc);
//        InvProps.SetInvIProperties(Prop.MP_PESO_BRUTO, txtProp_DT_PesoBruto.Text, InvPropetiesGroup.CustomFields,oDoc);
//        InvProps.SetInvIProperties(Prop.PC_AREA_PINTURA, txtProp_DT_AreaPintura.Text, InvPropetiesGroup.CustomFields,oDoc);
//        InvProps.SetInvIProperties(Prop.PC_TIPO_PINTURA, cbxProp_DT_TipoPintura.Text, InvPropetiesGroup.CustomFields,oDoc);
//        InvProps.SetInvIProperties(Prop.PC_ACABAMENTO_PINTURA, cbxProp_DT_PinturaAcabamento.Text, InvPropetiesGroup.CustomFields,oDoc);

//        InvProps.SetInvIProperties(Prop.PC_ACABAMENTO_PINTURA, cbxProp_DT_PinturaAcabamento.Text, InvPropetiesGroup.CustomFields,oDoc);
//        InvProps.SetInvIProperties(Prop.PC_PINTURA_INTERNA_DIFERENTE, CheckState(checkPintura), InvPropetiesGroup.CustomFields, oDoc);
//    }

//    /// <summary>
//    ///     Define a espessura da chapa conforme a matéria prima
//    /// </summary>
//    private void SetSheetMetalThickness()
//    {
//        if (txtEspessura.Text == string.Empty)
//            return;

//        var value = txtEspessura.Text.ToDouble();
//        InvSheetMetal.SetThickness(value);
//    }

//    /// <summary>
//    ///     Define a espessura da chapa INFORMADA conforme a matéria prima
//    /// </summary>
//    private void SetSheetMetalThickness(PartDocument oDoc)
//    {
//        if (txtEspessura.Text == string.Empty)
//            return;

//        var value = txtEspessura.Text.ToDouble();
//        InvSheetMetal.SetThickness(value, oDoc);
//    }


//    /// <summary>
//    ///     Evento que monitora e troca de tab
//    /// </summary>
//    /// <param name="sender"></param>
//    /// <param name="e"></param>
//    private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
//    {
//        HasApplyMaterial();
//        if (tabControl1.SelectedTab.Name != tabControl1.TabPages[0].Name)
//            return;

//        if (checkMpCustomData.Checked) return;

//        txtProp_MP_Cod.Text = txtCod.Text;
//        txtProp_MP_DescERP.Text = txtDescErp.Text;
//        txtProp_MP_DescInventor.Text = txtDescInventor.Text;
//        txtProp_MP_Espessura.Text = txtEspessura.Text;
//        txtProp_MP_Familia.Text = txtFamilia.Text;
//        txtProp_MP_FatorUnidade.Text = txtFatorUnidade.Text;
//        txtProp_MP_Unidade.Text = txtUnidade.Text;
//    }

//    private void LimparTextBoxMpDadosMP()
//    {
//        txtCod.Clear();
//        txtDescErp.Clear();
//        txtDescInventor.Clear();
//        txtFamilia.Clear();
//        txtEspessura.Clear();
//        txtFatorUnidade.Clear();
//        txtUnidade.Clear();
//    }

//    /// <summary>
//    ///     Evento da pesquisa
//    /// </summary>
//    /// <param name="sender"></param>
//    /// <param name="e"></param>
//    private void txtFind_TextChanged(object sender, EventArgs e)
//    {
//        // FiltrarDados();
//        FiltrarDadosLocal();
//    }

//    private void checkPcDescCustom_CheckedChanged(object sender, EventArgs e)
//    {
//        if (checkPcDescCustom.Checked)
//        {
//            txtProp_PC_DescCompleta.ReadOnly = false;
//            txtProp_PC_DescCompleta.BackColor = System.Drawing.Color.FromArgb(192, 255, 192);
//        }
//        else if (!checkPcDescCustom.Checked)
//        {
//            txtProp_PC_DescCompleta.ReadOnly = true;
//            txtProp_PC_DescCompleta.BackColor = System.Drawing.SystemColors.Control;
//        }
//    }

//    public class DocEdit
//    {

//        public string Name { get; set; }

//        public string Path { get; set; }

//    }

//    #region Comando GetDocument

//    /// <summary>
//    ///     Pega o documento ativo - normalmente montagens
//    /// </summary>
//    /// <param name="sender"></param>
//    /// <param name="e"></param>
//    private async void BtnGetThisDoc_Click(object sender, EventArgs e)
//    {
//        ClearAllTextBox();

//        _selectedDocument = AddinGlobal.InventorApp.ActiveEditDocument;

//        await LoadFormActionAsync(_selectedDocument);
//    }

//    /// <summary>
//    ///     Comando para seleção de uma peça na montagem p/ leitura no Connector
//    /// </summary>
//    /// <param name="sender"></param>
//    /// <param name="e"></param>
//    private async void BtnSelectDoc_Click(object sender, EventArgs e)
//    {
//        var oldH = this.Height;

//        this.Height = 75;

//        _selectedDocument = GetDoc();

//        Height = oldH;

//        ClearAllTextBox();

//        await LoadFormActionAsync(_selectedDocument);
//    }

//    private Document VerifyDocSelected()
//    {

//        if (AddinGlobal.InventorApp.ActiveEditDocument.DocumentType == DocumentTypeEnum.kDrawingDocumentObject)
//        {
//            var drawDoc = AddinGlobal.InventorApp.ActiveEditDocument;
//            //var refFile = drawDoc.ReferencedFiles[0];

//            foreach (Document referencedFile in drawDoc.ReferencedFiles)
//                try
//                {
//                    var document = referencedFile;
//                    return document;
//                }
//                catch
//                {
//                    InvMsg.Msg("Não contém arquivo de peça.");
//                }
//            //var activeSheet = SheetHelper.GetActiveSheet();
//            //activeSheet.DrawingViews.
//        }

//        var selDoc = AddinGlobal.InventorApp.ActiveEditDocument.SelectSet;

//        if (selDoc.Count <= 0) return null;

//        foreach (var doc in selDoc)
//        {
//            try
//            {
//                if (doc.GetType() is ComponentOccurrence)
//                {

//                }
//                ComponentOccurrence occur = (ComponentOccurrence)doc;
//                return occur.Definition.Document as Document;
//            }
//            catch (Exception e)
//            {
//                CoreLog.WriteLog($"\n-----> Open Selected\n {e}");
//            }
//        }
//        return null;
//    }

//    private Document GetDoc()
//    {
//        ComponentOccurrence occur = (ComponentOccurrence) AddinGlobal.InventorApp.CommandManager.Pick(
//            SelectionFilterEnum.kAssemblyLeafOccurrenceFilter, "Selecione a Peça que Deseja Modificar");

//        return occur.Definition.Document as Document;
//    }

//    private void LoadDocument()
//    {
//        _selectedDocument = InvApp.AddinGlobal.InventorApp.ActiveEditDocument;

//        lblFileName.Text = _selectedDocument.DisplayName;

//        if (_selectedDocument.DocumentType != DocumentTypeEnum.kPartDocumentObject) return;

//        btnSelectDoc.Enabled = false;
//        btnGetThisDoc.Enabled = false;
//        BtnSaveAndReplace.Enabled = false;
//        BtnReplaceAllInStructure.Enabled = false;

//    }
//    #endregion

//    private void txtNames_TextChanged(object sender, EventArgs e)
//    {

//    }

//    void textBoxStateReadState(TextBox txt,bool state)
//    {
//        if (state)
//        {
//            txt.ReadOnly = false;
//            txt.BackColor = System.Drawing.Color.FromArgb(192, 255, 192);
//        }
//        else
//        {
//            txt.ReadOnly = true;
//            txt.BackColor = System.Drawing.SystemColors.Control;
//        }

//    }

//    private void checkMpCustomData_CheckedChanged(object sender, EventArgs e)
//    {

//        if (checkMpCustomData.Checked)
//        {
//            textBoxStateReadState(txtProp_MP_Cod, true);
//            txtProp_MP_Cod.Clear();
//            textBoxStateReadState(txtProp_MP_DescERP, true);
//            txtProp_MP_DescERP.Clear();
//            textBoxStateReadState(txtProp_MP_DescInventor, true);
//            txtProp_MP_DescInventor.Clear();
//            textBoxStateReadState(txtProp_MP_Espessura, true);
//            txtProp_MP_Espessura.Clear();
//            textBoxStateReadState(txtProp_MP_Familia, true);
//            txtProp_MP_Familia.Clear();
//            textBoxStateReadState(txtProp_MP_FatorUnidade, true);
//            txtProp_MP_FatorUnidade.Clear();
//            textBoxStateReadState(txtProp_MP_Unidade, true);
//            txtProp_MP_Unidade.Clear();
//        }
//        else if (!checkMpCustomData.Checked)
//        {

//            textBoxStateReadState(txtProp_MP_Cod, false);
//            txtProp_MP_Cod.Clear();
//            textBoxStateReadState(txtProp_MP_DescERP, false);
//            txtProp_MP_DescERP.Clear();
//            textBoxStateReadState(txtProp_MP_DescInventor, false);
//            txtProp_MP_DescInventor.Clear();
//            textBoxStateReadState(txtProp_MP_Espessura, false);
//            txtProp_MP_Espessura.Clear();
//            textBoxStateReadState(txtProp_MP_Familia, false);
//            txtProp_MP_Familia.Clear();
//            textBoxStateReadState(txtProp_MP_FatorUnidade, false);
//            txtProp_MP_FatorUnidade.Clear();
//            textBoxStateReadState(txtProp_MP_Unidade, false);
//            txtProp_MP_Unidade.Clear();
//        }
//    }

//    private void btnAppply_Click(object sender, EventArgs e)
//    {
//        try
//        {
//            ApplyPartData(_selectedDocument);

//            UpdateActiveFile();

//        }
//        catch (Exception ex)
//        {
//            CoreLog.WriteLog($"\n - btnOK -> Erro no comando OK \n {ex}");
//        }
//    }

//    private void BtnSaveFile_Click(object sender, EventArgs e)
//    {
//        SaveFile();
//    }
//    private void BtnSaveCopyAs_Click(object sender, EventArgs e)
//    {
//        SaveFile(true);
//    }

//    private void DocApplyUpdate()
//    {
//        ApplyPartData(_selectedDocument);
//        UpdateActiveFile();
//    }
//    private void SaveFile(bool saveAs = false)
//    {
//        var oldCode = txtProp_PC_CodDesenho.Text;
//        var fileCode = string.Empty;
//        DialogResult dialogResult;
//        string fileNameComplete;
//        if (txtProp_PC_Desc.Text == string.Empty)
//        {
//            var result = MessageBox.Show("Você não definiu uma descrição para o arquivo!\n\n" +
//                            "Se continuar o arquivo receberá apenas um código.\n\n" +
//                            "Deseja continuar?","Arquivo sem Descrição",
//                MessageBoxButtons.YesNo,MessageBoxIcon.Exclamation);
//            if (result == DialogResult.No)
//            {
//                return;
//            }
//        }
//        // Primeiro Salvamento
//        if (saveAs == false)
//        {
//            // verifica se é a primeira vez que o arquivo está sendo salvo
//            if (string.IsNullOrEmpty(_selectedDocument.File.FullFileName))
//            {
//                if (checkCodDesenho.Checked || txtProp_PC_CodDesenho.Text != string.Empty)
//                {
//                    dialogResult = DialogResultGerarCodigo();

//                    if (dialogResult == DialogResult.Cancel) return;

//                    if (dialogResult == DialogResult.Yes)
//                    {
//                        // Pega o código do Textbox
//                        fileCode = txtProp_PC_CodDesenho.Text.Trim();
//                    }

//                    if(dialogResult == DialogResult.No)
//                    {
//                        // Gera o código caso DialogResult == NO
//                        checkCodDesenho.Checked = false;
//                    }
//                }

//                if (checkCodDesenho.Checked == false)
//                {
//                    // Gera o código
//                    fileCode = WVaultConnector.Vault.GetFileNameCode();
//                }

//                // Insere o código na propriedade
//                txtProp_PC_CodDesenho.Text = fileCode;
//                // Atualiza o documento com as propriedades
//                DocApplyUpdate();

//                // Cria a Caixa de Diálogo
//                AddinGlobal.InventorApp.CreateFileDialog(out var oFileDlg);
//                // Gera o nome completo
//                fileNameComplete = $"{fileCode} {txtProp_PC_Desc.Text}";
//                // Prepara o filtro para Peça ou Montagem
//                switch (_selectedDocument)
//                {
//                    case PartDocument _:
//                        oFileDlg.Filter = FileTypeDescription.Part;
//                        break;
//                    case AssemblyDocument _:
//                        oFileDlg.Filter = FileTypeDescription.Assembly;
//                        txtProp_PC_DescCompleta.Text = txtProp_PC_Desc.Text;
//                        break;
//                }

//                // Define as propriedades da Caixa de Diálogo e verificar se o usuário Confirma ou Cancela a operação
//                try
//                {
//                    oFileDlg.FilterIndex = 1;
//                    oFileDlg.DialogTitle = $"Nome do Arquivo : {fileNameComplete}";
//                    oFileDlg.FileName = fileNameComplete;
//                    oFileDlg.CancelError = true;
//                    oFileDlg.ShowSave();
//                    try
//                    {   // Salva o arquivo
//                        _selectedDocument.SaveAs(oFileDlg.FileName, false);
//                        // Seta as propriedades
//                        lblFileName.Text = _selectedDocument.DisplayName;
//                    }
//                    catch (Exception e)
//                    {
//                        InvMsg.Msg("Erro ao Salvar.\n\n" + e.InnerException);
//                    }
//                }
//                catch (Exception e)
//                {
//                    //InvMsg.Msg("Cancelado.\n\n" + e.InnerException);
//                }
//            }
//            else // Se o arquivo já estiver salvo apenas Salva o Documento.
//            {   
//                DocApplyUpdate();
//                _selectedDocument.Save();
//            }
//        }

//        // Salvar Como
//        if (!saveAs) return;
//        {
//            if (checkCodDesenho.Checked)
//            {
//                dialogResult = DialogResultGerarCodigo();

//                if (dialogResult == DialogResult.Cancel) return;

//                if (dialogResult == DialogResult.Yes)
//                {
//                    // Pega o código do Textbox
//                    fileCode = txtProp_PC_CodDesenho.Text.Trim();
//                }

//                if(dialogResult == DialogResult.No)
//                {
//                    // Gera o código caso DialogResult == NO
//                    checkCodDesenho.Checked = false;
//                }
//            }

//            if (checkCodDesenho.Checked == false)
//            {
//                // Gera o código
//                fileCode = WVaultConnector.Vault.GetFileNameCode();
//            }

//            // Insere o código na propriedade
//            txtProp_PC_CodDesenho.Text = fileCode;
//            // Atualiza o documento com as propriedades
//            DocApplyUpdate();
//            // Gera o nome completo
//            fileNameComplete = $"{fileCode} {txtProp_PC_Desc.Text}";
//            // Cria a Caixa de Diálogo
//            AddinGlobal.InventorApp.CreateFileDialog(out var oFileDlg);
//            // Prepara o filtro para Peça ou Montagem    
//            switch (_selectedDocument)
//            {
//                case PartDocument _:
//                    oFileDlg.Filter = FileTypeDescription.Part;
//                    break;
//                case AssemblyDocument _:
//                    oFileDlg.Filter = FileTypeDescription.Assembly;
//                    txtProp_PC_DescCompleta.Text = txtProp_PC_Desc.Text;
//                    break;
//            }

//            // Define as propriedades da Caixa de Diálogo e verificar se o usuário Confirma ou Cancela a operação
//            try
//            {
//                oFileDlg.FilterIndex = 1;
//                oFileDlg.DialogTitle = $"Nome do Arquivo: {fileNameComplete}";
//                oFileDlg.FileName = fileNameComplete;
//                oFileDlg.CancelError = true;
//                oFileDlg.ShowSave();
//                try
//                {
//                    _selectedDocument.SaveAs(oFileDlg.FileName, false);
//                    lblFileName.Text = _selectedDocument.DisplayName;
//                }
//                catch (Exception e)
//                {
//                    txtProp_PC_CodDesenho.Text = oldCode;
//                    UpdateActiveFile();
//                    InvMsg.Msg("Erro ao Salvar.\n\n" + e.InnerException);
//                } 
//            }
//            catch (Exception e)
//            {
//                txtProp_PC_CodDesenho.Text = oldCode;
//                UpdateActiveFile();
//            }
//        }
//    }

//    private DialogResult DialogResultGerarCodigo()
//    {
//        DialogResult dialogResult;
//        dialogResult = MessageBox.Show("Atenção: O código da peça já está DIGITADO ou definido como CUSTOM." +
//                                       $"\n\nCódigo Digitado: {txtProp_PC_CodDesenho.Text}" +
//                                       "\n\nSe desejar manter esse código para formar o nome do arquivo clique em -> SIM <-." +
//                                       "\n\nSe desejar criar um NOVO Código clique em -> NÃO <-.", "Atenção para Gerar o Código",
//            MessageBoxButtons.YesNoCancel,MessageBoxIcon.Question);
//        return dialogResult;
//    }

//    private void checkCodDesenho_CheckedChanged(object sender, EventArgs e)
//    {
//        if (checkCodDesenho.Checked)
//        {
//            txtProp_PC_CodDesenho.ReadOnly = false;
//            txtProp_PC_CodDesenho.BackColor = System.Drawing.Color.FromArgb(192, 255, 192);
//        }
//        else
//        {
//            txtProp_PC_CodDesenho.ReadOnly = true;
//            txtProp_PC_CodDesenho.BackColor = System.Drawing.SystemColors.Control;
//        }

//    }

//    private void BtnSaveAndReplace_Click(object sender, EventArgs e)
//    {
//        Replace.SaveCopyAndReplace();
//    }

//    private void BtnReplaceAllInStructure_Click(object sender, EventArgs e)
//    {
//        Replace.SaveCopyAndReplaceAllLeafAssembly();
//    }

//    private void tabPropriedades_Click(object sender, EventArgs e)
//    {

//    }

//    private void cbxProp_DT_TipoPintura_SelectedIndexChanged(object sender, EventArgs e)
//    {
//        if (cbxProp_DT_TipoPintura.Text == "Inteiro")
//        {
//            checkPintura.Enabled = true;
//        }
//        else
//        {
//            checkPintura.Checked = false;
//            checkPintura.Enabled = false;
//        }
//    }

//    private void TxtFinder_TextChanged(object sender, EventArgs e)
//    {
//        FiltrarDados();
//    }

//    private void btnAddFavorite_Click(object sender, EventArgs e)
//    {

//    }

//    private void BtnStockNumber_Click(object sender, EventArgs e)
//    {
//        TxtBox.SetToTextBox(TxtFinder, InvProps.GetInventorProperties("Stock Number", InvPropetiesGroup.DesignTrackingProperties, _selectedDocument));
//    }

//    private void BtnRevClear_Click(object sender, EventArgs e)
//    {
//        txtProp_PC_NumRevisao.Clear();
//    }
//}

//private void CopiarPropriedades(TextBox textBox)
//{
//    // Criar um novo ComboBox
//    ComboBox comboBox = new ComboBox();

//    // Copiar a posição e o tamanho do TextBox
//    comboBox.Location = textBox.Location;
//    comboBox.Size = textBox.Size;

//    // Copiar propriedades visuais do TextBox para o ComboBox
//    comboBox.BackColor = textBox.BackColor;
//    comboBox.ForeColor = textBox.ForeColor;
//    comboBox.Font = textBox.Font;
//    comboBox.Text = textBox.Text;

//    // Definir estilo do ComboBox (opcional: DropDown para permitir digitação, DropDownList para seleção somente)
//    comboBox.DropDownStyle = ComboBoxStyle.DropDown; // Permite digitação

//    // Adicionar os itens desejados no ComboBox (exemplo)
//    comboBox.Items.Add("Aprovador 1");
//    comboBox.Items.Add("Aprovador 2");
//    comboBox.Items.Add("Aprovador 3");

//    // Adicionar o ComboBox ao formulário
//    //this.Controls.Add(comboBox);


//    // Remover o TextBox do formulário
//    this.Controls.Remove(textBox);


//}
