namespace DescriptionManager.Views
{
    partial class ViewDescriptionManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewDescriptionManager));
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.DescManager = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbxTipoBlank = new System.Windows.Forms.ComboBox();
            this.txtScriptDesc = new System.Windows.Forms.RichTextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbxFamily = new System.Windows.Forms.ComboBox();
            this.txtPropField = new System.Windows.Forms.TextBox();
            this.txtId = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvDescManager = new System.Windows.Forms.DataGridView();
            this.button3 = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.desc_script = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.familia = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.iprop = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControlMain.SuspendLayout();
            this.DescManager.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDescManager)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.DescManager);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.ItemSize = new System.Drawing.Size(58, 30);
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(1094, 626);
            this.tabControlMain.TabIndex = 0;
            // 
            // DescManager
            // 
            this.DescManager.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.DescManager.Controls.Add(this.groupBox2);
            this.DescManager.Controls.Add(this.groupBox1);
            this.DescManager.Controls.Add(this.btnOk);
            this.DescManager.Location = new System.Drawing.Point(4, 34);
            this.DescManager.Name = "DescManager";
            this.DescManager.Padding = new System.Windows.Forms.Padding(3);
            this.DescManager.Size = new System.Drawing.Size(1086, 588);
            this.DescManager.TabIndex = 0;
            this.DescManager.Text = "Gerenciador de Descrição";
            this.DescManager.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.cbxTipoBlank);
            this.groupBox2.Controls.Add(this.txtScriptDesc);
            this.groupBox2.Controls.Add(this.button4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.cbxFamily);
            this.groupBox2.Controls.Add(this.txtPropField);
            this.groupBox2.Controls.Add(this.txtId);
            this.groupBox2.Controls.Add(this.txtName);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Location = new System.Drawing.Point(8, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1070, 105);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Construtor de Descrição |  Design Rules";
            // 
            // cbxTipoBlank
            // 
            this.cbxTipoBlank.BackColor = System.Drawing.Color.PaleGreen;
            this.cbxTipoBlank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTipoBlank.FormattingEnabled = true;
            this.cbxTipoBlank.Location = new System.Drawing.Point(189, 75);
            this.cbxTipoBlank.Name = "cbxTipoBlank";
            this.cbxTipoBlank.Size = new System.Drawing.Size(180, 21);
            this.cbxTipoBlank.TabIndex = 8;
            // 
            // txtScriptDesc
            // 
            this.txtScriptDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtScriptDesc.BackColor = System.Drawing.Color.PaleGreen;
            this.txtScriptDesc.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtScriptDesc.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtScriptDesc.Location = new System.Drawing.Point(375, 36);
            this.txtScriptDesc.Name = "txtScriptDesc";
            this.txtScriptDesc.Size = new System.Drawing.Size(608, 62);
            this.txtScriptDesc.TabIndex = 7;
            this.txtScriptDesc.Text = "";
            this.txtScriptDesc.TextChanged += new System.EventHandler(this.txtScriptDesc_TextChanged);
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = new System.Drawing.Point(989, 13);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 1;
            this.button4.Text = "Limpar";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Família";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(186, 59);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Tipo do Blank";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(186, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Campo iProperties";
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(989, 73);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 26);
            this.button2.TabIndex = 1;
            this.button2.Text = "Modificar";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.update_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(0, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(16, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Id";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(73, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Nome";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(372, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Script Descrição";
            // 
            // cbxFamily
            // 
            this.cbxFamily.BackColor = System.Drawing.Color.PaleGreen;
            this.cbxFamily.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxFamily.FormattingEnabled = true;
            this.cbxFamily.Location = new System.Drawing.Point(3, 75);
            this.cbxFamily.Name = "cbxFamily";
            this.cbxFamily.Size = new System.Drawing.Size(180, 21);
            this.cbxFamily.TabIndex = 5;
            // 
            // txtPropField
            // 
            this.txtPropField.BackColor = System.Drawing.Color.PaleGreen;
            this.txtPropField.Location = new System.Drawing.Point(189, 36);
            this.txtPropField.Name = "txtPropField";
            this.txtPropField.Size = new System.Drawing.Size(180, 20);
            this.txtPropField.TabIndex = 4;
            // 
            // txtId
            // 
            this.txtId.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtId.Location = new System.Drawing.Point(3, 36);
            this.txtId.Name = "txtId";
            this.txtId.ReadOnly = true;
            this.txtId.Size = new System.Drawing.Size(67, 20);
            this.txtId.TabIndex = 4;
            // 
            // txtName
            // 
            this.txtName.BackColor = System.Drawing.Color.LimeGreen;
            this.txtName.Location = new System.Drawing.Point(76, 36);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(107, 20);
            this.txtName.TabIndex = 4;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button1.Location = new System.Drawing.Point(989, 41);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 26);
            this.button1.TabIndex = 1;
            this.button1.Text = "Adicionar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.dgvDescManager);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Location = new System.Drawing.Point(8, 117);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1070, 432);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Lista de Descrições";
            // 
            // dgvDescManager
            // 
            this.dgvDescManager.AllowUserToAddRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.dgvDescManager.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDescManager.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDescManager.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDescManager.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dgvDescManager.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvDescManager.Location = new System.Drawing.Point(6, 19);
            this.dgvDescManager.MultiSelect = false;
            this.dgvDescManager.Name = "dgvDescManager";
            this.dgvDescManager.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDescManager.Size = new System.Drawing.Size(1058, 378);
            this.dgvDescManager.TabIndex = 0;
            this.dgvDescManager.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDescManager_CellDoubleClick);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button3.Location = new System.Drawing.Point(6, 403);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(136, 23);
            this.button3.TabIndex = 1;
            this.button3.Text = "Excluir Configuração";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.delete_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(1003, 557);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "Fechar";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // desc_script
            // 
            this.desc_script.HeaderText = "script da Descrição";
            this.desc_script.Name = "desc_script";
            this.desc_script.Width = 600;
            // 
            // familia
            // 
            this.familia.HeaderText = "Familia";
            this.familia.Name = "familia";
            // 
            // iprop
            // 
            this.iprop.HeaderText = "Campo iProperties";
            this.iprop.Name = "iprop";
            this.iprop.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.iprop.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // name
            // 
            this.name.HeaderText = "Nome";
            this.name.Name = "name";
            // 
            // ViewDescriptionManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1094, 626);
            this.Controls.Add(this.tabControlMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ViewDescriptionManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gerenciador de Descrição";
            this.Load += new System.EventHandler(this.FormDescriptionManager_Load);
            this.tabControlMain.ResumeLayout(false);
            this.DescManager.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDescManager)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage DescManager;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.DataGridView dgvDescManager;
        private System.Windows.Forms.DataGridViewTextBoxColumn desc_script;
        private System.Windows.Forms.DataGridViewComboBoxColumn familia;
        private System.Windows.Forms.DataGridViewComboBoxColumn iprop;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbxFamily;
        private System.Windows.Forms.TextBox txtPropField;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox txtScriptDesc;
        private System.Windows.Forms.ComboBox cbxTipoBlank;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtId;
    }
}