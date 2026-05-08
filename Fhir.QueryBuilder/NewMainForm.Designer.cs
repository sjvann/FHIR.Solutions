namespace Fhir.QueryBuilder
{
    partial class NewMainForm
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
            label1 = new Label();
            txt_FhirUrl = new TextBox();
            Btn_Connect = new Button();
            label2 = new Label();
            Btn_GetToken = new Button();
            Cob_Resource = new ComboBox();
            tabControl1 = new TabControl();
            tap_Common = new TabPage();
            Lib_CommonParameter = new ListBox();
            tap_Resource = new TabPage();
            Lib_ResourceParameter = new ListBox();
            tap_Advanced = new TabPage();
            advancedSearchControl = new Controls.AdvancedSearchControl();
            lab_SearchField = new Label();
            lab_SerchType = new Label();
            Pan_InputParameters = new Panel();
            panReference = new Panel();
            tableLayoutPanel1 = new TableLayoutPanel();
            label6 = new Label();
            label7 = new Label();
            txtReference_id = new TextBox();
            txtReference_version = new TextBox();
            label8 = new Label();
            txtReference_TypeId_Type = new TextBox();
            label13 = new Label();
            txtReference_TypeId_Id = new TextBox();
            label10 = new Label();
            txtReference_url = new TextBox();
            label14 = new Label();
            grbReference = new GroupBox();
            rabReference_text = new RadioButton();
            rabReference_notin = new RadioButton();
            rabReference_missing = new RadioButton();
            rabReference_identifier = new RadioButton();
            rabReference_below = new RadioButton();
            rabReference_above = new RadioButton();
            panQuantity = new Panel();
            tableLayoutPanel3 = new TableLayoutPanel();
            label4 = new Label();
            label5 = new Label();
            txtQuantity_nsc_code = new TextBox();
            label24 = new Label();
            txtQuantity_nsc_system = new TextBox();
            label25 = new Label();
            label26 = new Label();
            txtQuantity_nsc_number = new TextBox();
            cobQuantity_prefix = new ComboBox();
            txtQuantity_number = new TextBox();
            grbQuantity = new GroupBox();
            rabQuantity_missing = new RadioButton();
            label27 = new Label();
            panToken = new Panel();
            tableLayoutPanel6 = new TableLayoutPanel();
            label9 = new Label();
            label19 = new Label();
            txtToken_SystemCode_Code = new TextBox();
            label20 = new Label();
            txtToken_Code = new TextBox();
            txtToken_SystemCode_System = new TextBox();
            grbToken = new GroupBox();
            rabToken_text = new RadioButton();
            rabToken_oftype = new RadioButton();
            rabToken_notin = new RadioButton();
            rabToken_not = new RadioButton();
            rabToken_missing = new RadioButton();
            rabToken_in = new RadioButton();
            rabToken_below = new RadioButton();
            rabToken_above = new RadioButton();
            label23 = new Label();
            panUri = new Panel();
            tableLayoutPanel7 = new TableLayoutPanel();
            label21 = new Label();
            txtUri_url = new TextBox();
            grbUri = new GroupBox();
            rabUri_missing = new RadioButton();
            rabUri_contains = new RadioButton();
            rabUri_below = new RadioButton();
            rabUri_above = new RadioButton();
            label22 = new Label();
            panString = new Panel();
            tableLayoutPanel5 = new TableLayoutPanel();
            label15 = new Label();
            txtString = new TextBox();
            grbString = new GroupBox();
            rabString_text = new RadioButton();
            rabString_missing = new RadioButton();
            rabString_exact = new RadioButton();
            rabString_contains = new RadioButton();
            label16 = new Label();
            panNumber = new Panel();
            tableLayoutPanel4 = new TableLayoutPanel();
            label28 = new Label();
            label29 = new Label();
            cobNumber_prefix = new ComboBox();
            txtNumber_value = new TextBox();
            grbNumber = new GroupBox();
            rabNumber_missing = new RadioButton();
            label30 = new Label();
            panDate = new Panel();
            tableLayoutPanel2 = new TableLayoutPanel();
            label31 = new Label();
            cobDate_prefix = new ComboBox();
            txtDate_value = new TextBox();
            label32 = new Label();
            grbDate = new GroupBox();
            rabDate_missing = new RadioButton();
            label33 = new Label();
            Btn_AddParameter = new Button();
            Lib_ParameterList = new ListBox();
            Btn_RemoveParameter = new Button();
            Btn_ModifyingResult = new Button();
            Btn_ResourceOperation = new Button();
            txt_Message = new TextBox();
            label11 = new Label();
            Btn_TreeView = new Button();
            Btn_BundleHelper = new Button();
            Clb_Include = new CheckedListBox();
            label12 = new Label();
            Clb_Revinclude = new CheckedListBox();
            label17 = new Label();
            Ckb_includeAll = new CheckBox();
            Ckb_RevincludeAll = new CheckBox();
            Txt_SearchUrl = new TextBox();
            label18 = new Label();
            Btn_Search = new Button();
            Btn_Copy = new Button();
            Btn_Save = new Button();
            Btn_CreateUrl = new Button();
            Btn_Exit = new Button();
            trv_Message = new TreeView();
            lab_Loading = new Label();
            Btn_RemoveAllParameter = new Button();
            tab_LastControl = new TabControl();
            tap_Modifying = new TabPage();
            Lib_ModifyingList = new ListBox();
            tabControl1.SuspendLayout();
            tap_Common.SuspendLayout();
            tap_Resource.SuspendLayout();
            Pan_InputParameters.SuspendLayout();
            panReference.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            grbReference.SuspendLayout();
            panQuantity.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            grbQuantity.SuspendLayout();
            panToken.SuspendLayout();
            tableLayoutPanel6.SuspendLayout();
            grbToken.SuspendLayout();
            panUri.SuspendLayout();
            tableLayoutPanel7.SuspendLayout();
            grbUri.SuspendLayout();
            panString.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            grbString.SuspendLayout();
            panNumber.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            grbNumber.SuspendLayout();
            panDate.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            grbDate.SuspendLayout();
            tab_LastControl.SuspendLayout();
            tap_Modifying.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 13);
            label1.Name = "label1";
            label1.Size = new Size(98, 15);
            label1.TabIndex = 0;
            label1.Text = "FHIR Server's Url";
            // 
            // txt_FhirUrl
            // 
            txt_FhirUrl.Location = new Point(12, 30);
            txt_FhirUrl.Name = "txt_FhirUrl";
            txt_FhirUrl.Size = new Size(375, 23);
            txt_FhirUrl.TabIndex = 1;
            // 
            // Btn_Connect
            // 
            Btn_Connect.Location = new Point(393, 31);
            Btn_Connect.Name = "Btn_Connect";
            Btn_Connect.Size = new Size(75, 23);
            Btn_Connect.TabIndex = 2;
            Btn_Connect.Text = "Connect";
            Btn_Connect.UseVisualStyleBackColor = true;
            Btn_Connect.Click += Btn_Connect_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(577, 12);
            label2.Name = "label2";
            label2.Size = new Size(59, 15);
            label2.TabIndex = 3;
            label2.Text = "Resource";
            // 
            // Btn_GetToken
            // 
            Btn_GetToken.Location = new Point(474, 31);
            Btn_GetToken.Name = "Btn_GetToken";
            Btn_GetToken.Size = new Size(97, 23);
            Btn_GetToken.TabIndex = 4;
            Btn_GetToken.Text = "Get Token";
            Btn_GetToken.UseVisualStyleBackColor = true;
            Btn_GetToken.Visible = false;
            Btn_GetToken.Click += Btn_GetToken_Click;
            // 
            // Cob_Resource
            // 
            Cob_Resource.FormattingEnabled = true;
            Cob_Resource.Location = new Point(576, 30);
            Cob_Resource.Name = "Cob_Resource";
            Cob_Resource.Size = new Size(228, 23);
            Cob_Resource.TabIndex = 5;
            Cob_Resource.SelectedIndexChanged += Cob_Resource_SelectedIndexChanged;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tap_Common);
            tabControl1.Controls.Add(tap_Resource);
            tabControl1.Controls.Add(tap_Advanced);
            tabControl1.Location = new Point(12, 66);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(210, 374);
            tabControl1.TabIndex = 6;
            // 
            // tap_Common
            // 
            tap_Common.Controls.Add(Lib_CommonParameter);
            tap_Common.Location = new Point(4, 24);
            tap_Common.Name = "tap_Common";
            tap_Common.Padding = new Padding(3);
            tap_Common.Size = new Size(202, 346);
            tap_Common.TabIndex = 0;
            tap_Common.Text = "Common";
            tap_Common.UseVisualStyleBackColor = true;
            // 
            // Lib_CommonParameter
            // 
            Lib_CommonParameter.FormattingEnabled = true;
            Lib_CommonParameter.ItemHeight = 15;
            Lib_CommonParameter.Location = new Point(6, 6);
            Lib_CommonParameter.Name = "Lib_CommonParameter";
            Lib_CommonParameter.Size = new Size(190, 334);
            Lib_CommonParameter.TabIndex = 0;
            Lib_CommonParameter.SelectedIndexChanged += Lib_CommonParameter_SelectedIndexChanged;
            // 
            // tap_Resource
            // 
            tap_Resource.Controls.Add(Lib_ResourceParameter);
            tap_Resource.Location = new Point(4, 24);
            tap_Resource.Name = "tap_Resource";
            tap_Resource.Padding = new Padding(3);
            tap_Resource.Size = new Size(202, 346);
            tap_Resource.TabIndex = 1;
            tap_Resource.Text = "Resource";
            tap_Resource.UseVisualStyleBackColor = true;
            //
            // tap_Advanced
            //
            tap_Advanced.Controls.Add(advancedSearchControl);
            tap_Advanced.Location = new Point(4, 24);
            tap_Advanced.Name = "tap_Advanced";
            tap_Advanced.Padding = new Padding(3);
            tap_Advanced.Size = new Size(202, 346);
            tap_Advanced.TabIndex = 2;
            tap_Advanced.Text = "Advanced";
            tap_Advanced.UseVisualStyleBackColor = true;
            //
            // advancedSearchControl
            //
            advancedSearchControl.Dock = DockStyle.Fill;
            advancedSearchControl.Location = new Point(3, 3);
            advancedSearchControl.Name = "advancedSearchControl";
            advancedSearchControl.Size = new Size(196, 340);
            advancedSearchControl.TabIndex = 0;
            //
            // Lib_ResourceParameter
            // 
            Lib_ResourceParameter.FormattingEnabled = true;
            Lib_ResourceParameter.ItemHeight = 15;
            Lib_ResourceParameter.Location = new Point(6, 6);
            Lib_ResourceParameter.Name = "Lib_ResourceParameter";
            Lib_ResourceParameter.Size = new Size(190, 334);
            Lib_ResourceParameter.TabIndex = 0;
            Lib_ResourceParameter.SelectedIndexChanged += Lib_ResourceParameter_SelectedIndexChanged;
            // 
            // lab_SearchField
            // 
            lab_SearchField.BorderStyle = BorderStyle.FixedSingle;
            lab_SearchField.Location = new Point(228, 66);
            lab_SearchField.Name = "lab_SearchField";
            lab_SearchField.Size = new Size(181, 23);
            lab_SearchField.TabIndex = 8;
            lab_SearchField.Visible = false;
            // 
            // lab_SerchType
            // 
            lab_SerchType.BorderStyle = BorderStyle.FixedSingle;
            lab_SerchType.Location = new Point(415, 66);
            lab_SerchType.Name = "lab_SerchType";
            lab_SerchType.Size = new Size(156, 23);
            lab_SerchType.TabIndex = 9;
            lab_SerchType.Visible = false;
            lab_SerchType.TextChanged += lab_SerchType_TextChanged;
            // 
            // Pan_InputParameters
            // 
            Pan_InputParameters.Controls.Add(panReference);
            Pan_InputParameters.Controls.Add(panQuantity);
            Pan_InputParameters.Controls.Add(panToken);
            Pan_InputParameters.Controls.Add(panUri);
            Pan_InputParameters.Controls.Add(panString);
            Pan_InputParameters.Controls.Add(panNumber);
            Pan_InputParameters.Controls.Add(panDate);
            Pan_InputParameters.Location = new Point(228, 92);
            Pan_InputParameters.Name = "Pan_InputParameters";
            Pan_InputParameters.Size = new Size(343, 348);
            Pan_InputParameters.TabIndex = 10;
            // 
            // panReference
            // 
            panReference.Controls.Add(tableLayoutPanel1);
            panReference.Controls.Add(label14);
            panReference.Controls.Add(grbReference);
            panReference.Location = new Point(0, 0);
            panReference.Name = "panReference";
            panReference.Size = new Size(342, 334);
            panReference.TabIndex = 14;
            panReference.Visible = false;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 72F));
            tableLayoutPanel1.Controls.Add(label6, 0, 2);
            tableLayoutPanel1.Controls.Add(label7, 0, 0);
            tableLayoutPanel1.Controls.Add(txtReference_id, 1, 0);
            tableLayoutPanel1.Controls.Add(txtReference_version, 1, 4);
            tableLayoutPanel1.Controls.Add(label8, 0, 1);
            tableLayoutPanel1.Controls.Add(txtReference_TypeId_Type, 1, 1);
            tableLayoutPanel1.Controls.Add(label13, 0, 4);
            tableLayoutPanel1.Controls.Add(txtReference_TypeId_Id, 1, 2);
            tableLayoutPanel1.Controls.Add(label10, 0, 3);
            tableLayoutPanel1.Controls.Add(txtReference_url, 1, 3);
            tableLayoutPanel1.Location = new Point(3, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 5;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.Size = new Size(336, 150);
            tableLayoutPanel1.TabIndex = 19;
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label6.AutoSize = true;
            label6.Location = new Point(1, 61);
            label6.Margin = new Padding(1);
            label6.Name = "label6";
            label6.Size = new Size(92, 28);
            label6.TabIndex = 11;
            label6.Text = "/id";
            label6.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label7.AutoSize = true;
            label7.Location = new Point(1, 1);
            label7.Margin = new Padding(1);
            label7.Name = "label7";
            label7.Size = new Size(92, 28);
            label7.TabIndex = 0;
            label7.Text = "id";
            label7.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtReference_id
            // 
            txtReference_id.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtReference_id.Location = new Point(97, 3);
            txtReference_id.Name = "txtReference_id";
            txtReference_id.Size = new Size(236, 23);
            txtReference_id.TabIndex = 1;
            // 
            // txtReference_version
            // 
            txtReference_version.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtReference_version.Location = new Point(97, 123);
            txtReference_version.Name = "txtReference_version";
            txtReference_version.Size = new Size(236, 23);
            txtReference_version.TabIndex = 9;
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label8.AutoSize = true;
            label8.Location = new Point(1, 31);
            label8.Margin = new Padding(1);
            label8.Name = "label8";
            label8.Size = new Size(92, 28);
            label8.TabIndex = 2;
            label8.Text = "type";
            label8.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtReference_TypeId_Type
            // 
            txtReference_TypeId_Type.Location = new Point(97, 33);
            txtReference_TypeId_Type.Name = "txtReference_TypeId_Type";
            txtReference_TypeId_Type.Size = new Size(236, 23);
            txtReference_TypeId_Type.TabIndex = 5;
            // 
            // label13
            // 
            label13.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label13.AutoSize = true;
            label13.Location = new Point(1, 121);
            label13.Margin = new Padding(1);
            label13.Name = "label13";
            label13.Size = new Size(92, 28);
            label13.TabIndex = 8;
            label13.Text = "version";
            label13.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtReference_TypeId_Id
            // 
            txtReference_TypeId_Id.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtReference_TypeId_Id.Location = new Point(97, 63);
            txtReference_TypeId_Id.Name = "txtReference_TypeId_Id";
            txtReference_TypeId_Id.Size = new Size(236, 23);
            txtReference_TypeId_Id.TabIndex = 3;
            // 
            // label10
            // 
            label10.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label10.AutoSize = true;
            label10.Location = new Point(1, 91);
            label10.Margin = new Padding(1);
            label10.Name = "label10";
            label10.Size = new Size(92, 28);
            label10.TabIndex = 6;
            label10.Text = "url";
            label10.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtReference_url
            // 
            txtReference_url.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtReference_url.Location = new Point(97, 93);
            txtReference_url.Name = "txtReference_url";
            txtReference_url.Size = new Size(236, 23);
            txtReference_url.TabIndex = 7;
            // 
            // label14
            // 
            label14.BorderStyle = BorderStyle.Fixed3D;
            label14.Location = new Point(3, 168);
            label14.Name = "label14";
            label14.Size = new Size(339, 2);
            label14.TabIndex = 18;
            // 
            // grbReference
            // 
            grbReference.Controls.Add(rabReference_text);
            grbReference.Controls.Add(rabReference_notin);
            grbReference.Controls.Add(rabReference_missing);
            grbReference.Controls.Add(rabReference_identifier);
            grbReference.Controls.Add(rabReference_below);
            grbReference.Controls.Add(rabReference_above);
            grbReference.Location = new Point(2, 173);
            grbReference.Name = "grbReference";
            grbReference.Size = new Size(337, 130);
            grbReference.TabIndex = 13;
            grbReference.TabStop = false;
            grbReference.Text = "Modifier";
            // 
            // rabReference_text
            // 
            rabReference_text.AutoSize = true;
            rabReference_text.Location = new Point(10, 107);
            rabReference_text.Name = "rabReference_text";
            rabReference_text.Size = new Size(46, 19);
            rabReference_text.TabIndex = 17;
            rabReference_text.TabStop = true;
            rabReference_text.Text = "text";
            rabReference_text.UseVisualStyleBackColor = true;
            // 
            // rabReference_notin
            // 
            rabReference_notin.AutoSize = true;
            rabReference_notin.Location = new Point(97, 82);
            rabReference_notin.Name = "rabReference_notin";
            rabReference_notin.Size = new Size(59, 19);
            rabReference_notin.TabIndex = 16;
            rabReference_notin.TabStop = true;
            rabReference_notin.Text = "not-in";
            rabReference_notin.UseVisualStyleBackColor = true;
            // 
            // rabReference_missing
            // 
            rabReference_missing.AutoSize = true;
            rabReference_missing.Location = new Point(10, 82);
            rabReference_missing.Name = "rabReference_missing";
            rabReference_missing.Size = new Size(67, 19);
            rabReference_missing.TabIndex = 15;
            rabReference_missing.TabStop = true;
            rabReference_missing.Text = "missing";
            rabReference_missing.UseVisualStyleBackColor = true;
            // 
            // rabReference_identifier
            // 
            rabReference_identifier.AutoSize = true;
            rabReference_identifier.Location = new Point(10, 57);
            rabReference_identifier.Name = "rabReference_identifier";
            rabReference_identifier.Size = new Size(75, 19);
            rabReference_identifier.TabIndex = 14;
            rabReference_identifier.TabStop = true;
            rabReference_identifier.Text = "identifier";
            rabReference_identifier.UseVisualStyleBackColor = true;
            // 
            // rabReference_below
            // 
            rabReference_below.AutoSize = true;
            rabReference_below.Location = new Point(97, 32);
            rabReference_below.Name = "rabReference_below";
            rabReference_below.Size = new Size(60, 19);
            rabReference_below.TabIndex = 13;
            rabReference_below.TabStop = true;
            rabReference_below.Text = "below";
            rabReference_below.UseVisualStyleBackColor = true;
            // 
            // rabReference_above
            // 
            rabReference_above.AutoSize = true;
            rabReference_above.Location = new Point(10, 32);
            rabReference_above.Name = "rabReference_above";
            rabReference_above.Size = new Size(61, 19);
            rabReference_above.TabIndex = 12;
            rabReference_above.TabStop = true;
            rabReference_above.Text = "above";
            rabReference_above.UseVisualStyleBackColor = true;
            // 
            // panQuantity
            // 
            panQuantity.Controls.Add(tableLayoutPanel3);
            panQuantity.Controls.Add(grbQuantity);
            panQuantity.Controls.Add(label27);
            panQuantity.Location = new Point(0, 0);
            panQuantity.Name = "panQuantity";
            panQuantity.Size = new Size(342, 334);
            panQuantity.TabIndex = 31;
            panQuantity.Visible = false;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 2;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            tableLayoutPanel3.Controls.Add(label4, 0, 4);
            tableLayoutPanel3.Controls.Add(label5, 0, 3);
            tableLayoutPanel3.Controls.Add(txtQuantity_nsc_code, 1, 4);
            tableLayoutPanel3.Controls.Add(label24, 0, 0);
            tableLayoutPanel3.Controls.Add(txtQuantity_nsc_system, 1, 3);
            tableLayoutPanel3.Controls.Add(label25, 0, 1);
            tableLayoutPanel3.Controls.Add(label26, 0, 2);
            tableLayoutPanel3.Controls.Add(txtQuantity_nsc_number, 1, 2);
            tableLayoutPanel3.Controls.Add(cobQuantity_prefix, 1, 0);
            tableLayoutPanel3.Controls.Add(txtQuantity_number, 1, 1);
            tableLayoutPanel3.Location = new Point(3, 4);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 5;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel3.Size = new Size(336, 150);
            tableLayoutPanel3.TabIndex = 32;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Location = new Point(1, 121);
            label4.Margin = new Padding(1);
            label4.Name = "label4";
            label4.Size = new Size(98, 28);
            label4.TabIndex = 32;
            label4.Text = "code";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label5.AutoSize = true;
            label5.Location = new Point(1, 91);
            label5.Margin = new Padding(1);
            label5.Name = "label5";
            label5.Size = new Size(98, 28);
            label5.TabIndex = 33;
            label5.Text = "system | ";
            label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtQuantity_nsc_code
            // 
            txtQuantity_nsc_code.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtQuantity_nsc_code.Location = new Point(103, 123);
            txtQuantity_nsc_code.Name = "txtQuantity_nsc_code";
            txtQuantity_nsc_code.Size = new Size(230, 23);
            txtQuantity_nsc_code.TabIndex = 7;
            // 
            // label24
            // 
            label24.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label24.AutoSize = true;
            label24.Location = new Point(1, 1);
            label24.Margin = new Padding(1);
            label24.Name = "label24";
            label24.Size = new Size(98, 28);
            label24.TabIndex = 0;
            label24.Text = "prefix";
            label24.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtQuantity_nsc_system
            // 
            txtQuantity_nsc_system.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtQuantity_nsc_system.Location = new Point(103, 93);
            txtQuantity_nsc_system.Name = "txtQuantity_nsc_system";
            txtQuantity_nsc_system.Size = new Size(230, 23);
            txtQuantity_nsc_system.TabIndex = 6;
            // 
            // label25
            // 
            label25.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label25.AutoSize = true;
            label25.Location = new Point(1, 31);
            label25.Margin = new Padding(1);
            label25.Name = "label25";
            label25.Size = new Size(98, 28);
            label25.TabIndex = 2;
            label25.Text = "number";
            label25.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label26
            // 
            label26.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label26.AutoSize = true;
            label26.Location = new Point(1, 61);
            label26.Margin = new Padding(1);
            label26.Name = "label26";
            label26.Size = new Size(98, 28);
            label26.TabIndex = 4;
            label26.Text = "number | ";
            label26.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtQuantity_nsc_number
            // 
            txtQuantity_nsc_number.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtQuantity_nsc_number.Location = new Point(103, 63);
            txtQuantity_nsc_number.Name = "txtQuantity_nsc_number";
            txtQuantity_nsc_number.Size = new Size(230, 23);
            txtQuantity_nsc_number.TabIndex = 5;
            // 
            // cobQuantity_prefix
            // 
            cobQuantity_prefix.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            cobQuantity_prefix.FormattingEnabled = true;
            cobQuantity_prefix.Items.AddRange(new object[] { "eq", "ne", "gt", "lt", "ge", "le", "sa", "eb", "ap" });
            cobQuantity_prefix.Location = new Point(103, 3);
            cobQuantity_prefix.Name = "cobQuantity_prefix";
            cobQuantity_prefix.Size = new Size(230, 23);
            cobQuantity_prefix.TabIndex = 1;
            // 
            // txtQuantity_number
            // 
            txtQuantity_number.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtQuantity_number.Location = new Point(103, 33);
            txtQuantity_number.Name = "txtQuantity_number";
            txtQuantity_number.Size = new Size(230, 23);
            txtQuantity_number.TabIndex = 3;
            // 
            // grbQuantity
            // 
            grbQuantity.Controls.Add(rabQuantity_missing);
            grbQuantity.Location = new Point(10, 173);
            grbQuantity.Name = "grbQuantity";
            grbQuantity.Size = new Size(329, 130);
            grbQuantity.TabIndex = 31;
            grbQuantity.TabStop = false;
            grbQuantity.Text = "Modifier";
            // 
            // rabQuantity_missing
            // 
            rabQuantity_missing.AutoSize = true;
            rabQuantity_missing.Location = new Point(12, 28);
            rabQuantity_missing.Name = "rabQuantity_missing";
            rabQuantity_missing.Size = new Size(67, 19);
            rabQuantity_missing.TabIndex = 0;
            rabQuantity_missing.TabStop = true;
            rabQuantity_missing.Text = "missing";
            rabQuantity_missing.UseVisualStyleBackColor = true;
            // 
            // label27
            // 
            label27.BorderStyle = BorderStyle.Fixed3D;
            label27.Location = new Point(0, 168);
            label27.Name = "label27";
            label27.Size = new Size(336, 2);
            label27.TabIndex = 30;
            // 
            // panToken
            // 
            panToken.Controls.Add(tableLayoutPanel6);
            panToken.Controls.Add(grbToken);
            panToken.Controls.Add(label23);
            panToken.Location = new Point(0, 0);
            panToken.Name = "panToken";
            panToken.Size = new Size(342, 334);
            panToken.TabIndex = 29;
            panToken.Visible = false;
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.ColumnCount = 2;
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            tableLayoutPanel6.Controls.Add(label9, 0, 1);
            tableLayoutPanel6.Controls.Add(label19, 0, 0);
            tableLayoutPanel6.Controls.Add(txtToken_SystemCode_Code, 1, 2);
            tableLayoutPanel6.Controls.Add(label20, 0, 2);
            tableLayoutPanel6.Controls.Add(txtToken_Code, 1, 0);
            tableLayoutPanel6.Controls.Add(txtToken_SystemCode_System, 1, 1);
            tableLayoutPanel6.Location = new Point(3, 3);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 5;
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel6.Size = new Size(336, 150);
            tableLayoutPanel6.TabIndex = 32;
            // 
            // label9
            // 
            label9.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label9.AutoSize = true;
            label9.Location = new Point(1, 31);
            label9.Margin = new Padding(1);
            label9.Name = "label9";
            label9.Size = new Size(98, 28);
            label9.TabIndex = 30;
            label9.Text = "system | ";
            label9.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label19
            // 
            label19.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label19.AutoSize = true;
            label19.Location = new Point(1, 1);
            label19.Margin = new Padding(1);
            label19.Name = "label19";
            label19.Size = new Size(98, 28);
            label19.TabIndex = 0;
            label19.Text = "code";
            label19.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtToken_SystemCode_Code
            // 
            txtToken_SystemCode_Code.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtToken_SystemCode_Code.Location = new Point(101, 61);
            txtToken_SystemCode_Code.Margin = new Padding(1);
            txtToken_SystemCode_Code.Name = "txtToken_SystemCode_Code";
            txtToken_SystemCode_Code.Size = new Size(234, 23);
            txtToken_SystemCode_Code.TabIndex = 4;
            // 
            // label20
            // 
            label20.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label20.AutoSize = true;
            label20.Location = new Point(1, 61);
            label20.Margin = new Padding(1);
            label20.Name = "label20";
            label20.Size = new Size(98, 28);
            label20.TabIndex = 2;
            label20.Text = "code";
            label20.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtToken_Code
            // 
            txtToken_Code.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtToken_Code.Location = new Point(101, 1);
            txtToken_Code.Margin = new Padding(1);
            txtToken_Code.Name = "txtToken_Code";
            txtToken_Code.Size = new Size(234, 23);
            txtToken_Code.TabIndex = 1;
            // 
            // txtToken_SystemCode_System
            // 
            txtToken_SystemCode_System.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtToken_SystemCode_System.Location = new Point(101, 31);
            txtToken_SystemCode_System.Margin = new Padding(1);
            txtToken_SystemCode_System.Name = "txtToken_SystemCode_System";
            txtToken_SystemCode_System.Size = new Size(234, 23);
            txtToken_SystemCode_System.TabIndex = 3;
            // 
            // grbToken
            // 
            grbToken.Controls.Add(rabToken_text);
            grbToken.Controls.Add(rabToken_oftype);
            grbToken.Controls.Add(rabToken_notin);
            grbToken.Controls.Add(rabToken_not);
            grbToken.Controls.Add(rabToken_missing);
            grbToken.Controls.Add(rabToken_in);
            grbToken.Controls.Add(rabToken_below);
            grbToken.Controls.Add(rabToken_above);
            grbToken.Location = new Point(3, 173);
            grbToken.Name = "grbToken";
            grbToken.Size = new Size(336, 158);
            grbToken.TabIndex = 31;
            grbToken.TabStop = false;
            grbToken.Text = "Modifier";
            // 
            // rabToken_text
            // 
            rabToken_text.AutoSize = true;
            rabToken_text.Location = new Point(9, 122);
            rabToken_text.Name = "rabToken_text";
            rabToken_text.Size = new Size(46, 19);
            rabToken_text.TabIndex = 7;
            rabToken_text.TabStop = true;
            rabToken_text.Text = "text";
            rabToken_text.UseVisualStyleBackColor = true;
            // 
            // rabToken_oftype
            // 
            rabToken_oftype.AutoSize = true;
            rabToken_oftype.Location = new Point(9, 97);
            rabToken_oftype.Name = "rabToken_oftype";
            rabToken_oftype.Size = new Size(67, 19);
            rabToken_oftype.TabIndex = 6;
            rabToken_oftype.TabStop = true;
            rabToken_oftype.Text = "of-type";
            rabToken_oftype.UseVisualStyleBackColor = true;
            // 
            // rabToken_notin
            // 
            rabToken_notin.AutoSize = true;
            rabToken_notin.Location = new Point(87, 72);
            rabToken_notin.Name = "rabToken_notin";
            rabToken_notin.Size = new Size(59, 19);
            rabToken_notin.TabIndex = 5;
            rabToken_notin.TabStop = true;
            rabToken_notin.Text = "not-in";
            rabToken_notin.UseVisualStyleBackColor = true;
            // 
            // rabToken_not
            // 
            rabToken_not.AutoSize = true;
            rabToken_not.Location = new Point(9, 72);
            rabToken_not.Name = "rabToken_not";
            rabToken_not.Size = new Size(44, 19);
            rabToken_not.TabIndex = 4;
            rabToken_not.TabStop = true;
            rabToken_not.Text = "not";
            rabToken_not.UseVisualStyleBackColor = true;
            // 
            // rabToken_missing
            // 
            rabToken_missing.AutoSize = true;
            rabToken_missing.Location = new Point(9, 47);
            rabToken_missing.Name = "rabToken_missing";
            rabToken_missing.Size = new Size(67, 19);
            rabToken_missing.TabIndex = 3;
            rabToken_missing.TabStop = true;
            rabToken_missing.Text = "missing";
            rabToken_missing.UseVisualStyleBackColor = true;
            // 
            // rabToken_in
            // 
            rabToken_in.AutoSize = true;
            rabToken_in.Location = new Point(166, 24);
            rabToken_in.Name = "rabToken_in";
            rabToken_in.Size = new Size(35, 19);
            rabToken_in.TabIndex = 2;
            rabToken_in.TabStop = true;
            rabToken_in.Text = "in";
            rabToken_in.UseVisualStyleBackColor = true;
            // 
            // rabToken_below
            // 
            rabToken_below.AutoSize = true;
            rabToken_below.Location = new Point(87, 24);
            rabToken_below.Name = "rabToken_below";
            rabToken_below.Size = new Size(60, 19);
            rabToken_below.TabIndex = 1;
            rabToken_below.TabStop = true;
            rabToken_below.Text = "below";
            rabToken_below.UseVisualStyleBackColor = true;
            // 
            // rabToken_above
            // 
            rabToken_above.AutoSize = true;
            rabToken_above.Location = new Point(9, 22);
            rabToken_above.Name = "rabToken_above";
            rabToken_above.Size = new Size(61, 19);
            rabToken_above.TabIndex = 0;
            rabToken_above.TabStop = true;
            rabToken_above.Text = "above";
            rabToken_above.UseVisualStyleBackColor = true;
            // 
            // label23
            // 
            label23.BorderStyle = BorderStyle.Fixed3D;
            label23.Location = new Point(3, 168);
            label23.Name = "label23";
            label23.Size = new Size(335, 2);
            label23.TabIndex = 30;
            // 
            // panUri
            // 
            panUri.Controls.Add(tableLayoutPanel7);
            panUri.Controls.Add(grbUri);
            panUri.Controls.Add(label22);
            panUri.Location = new Point(0, 0);
            panUri.Name = "panUri";
            panUri.Size = new Size(342, 334);
            panUri.TabIndex = 30;
            panUri.Visible = false;
            // 
            // tableLayoutPanel7
            // 
            tableLayoutPanel7.ColumnCount = 2;
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            tableLayoutPanel7.Controls.Add(label21, 0, 0);
            tableLayoutPanel7.Controls.Add(txtUri_url, 1, 0);
            tableLayoutPanel7.Location = new Point(3, 3);
            tableLayoutPanel7.Name = "tableLayoutPanel7";
            tableLayoutPanel7.RowCount = 5;
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel7.Size = new Size(336, 150);
            tableLayoutPanel7.TabIndex = 32;
            // 
            // label21
            // 
            label21.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label21.AutoSize = true;
            label21.Location = new Point(1, 1);
            label21.Margin = new Padding(1);
            label21.Name = "label21";
            label21.Size = new Size(98, 28);
            label21.TabIndex = 0;
            label21.Text = "url";
            label21.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtUri_url
            // 
            txtUri_url.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtUri_url.Location = new Point(103, 3);
            txtUri_url.Name = "txtUri_url";
            txtUri_url.Size = new Size(230, 23);
            txtUri_url.TabIndex = 1;
            // 
            // grbUri
            // 
            grbUri.Controls.Add(rabUri_missing);
            grbUri.Controls.Add(rabUri_contains);
            grbUri.Controls.Add(rabUri_below);
            grbUri.Controls.Add(rabUri_above);
            grbUri.Location = new Point(3, 173);
            grbUri.Name = "grbUri";
            grbUri.Size = new Size(336, 140);
            grbUri.TabIndex = 31;
            grbUri.TabStop = false;
            grbUri.Text = "Modifier";
            // 
            // rabUri_missing
            // 
            rabUri_missing.AutoSize = true;
            rabUri_missing.Location = new Point(9, 78);
            rabUri_missing.Name = "rabUri_missing";
            rabUri_missing.Size = new Size(67, 19);
            rabUri_missing.TabIndex = 3;
            rabUri_missing.TabStop = true;
            rabUri_missing.Text = "missing";
            rabUri_missing.UseVisualStyleBackColor = true;
            // 
            // rabUri_contains
            // 
            rabUri_contains.AutoSize = true;
            rabUri_contains.Location = new Point(9, 53);
            rabUri_contains.Name = "rabUri_contains";
            rabUri_contains.Size = new Size(72, 19);
            rabUri_contains.TabIndex = 2;
            rabUri_contains.TabStop = true;
            rabUri_contains.Text = "contains";
            rabUri_contains.UseVisualStyleBackColor = true;
            // 
            // rabUri_below
            // 
            rabUri_below.AutoSize = true;
            rabUri_below.Location = new Point(82, 28);
            rabUri_below.Name = "rabUri_below";
            rabUri_below.Size = new Size(60, 19);
            rabUri_below.TabIndex = 1;
            rabUri_below.TabStop = true;
            rabUri_below.Text = "below";
            rabUri_below.UseVisualStyleBackColor = true;
            // 
            // rabUri_above
            // 
            rabUri_above.AutoSize = true;
            rabUri_above.Location = new Point(10, 28);
            rabUri_above.Name = "rabUri_above";
            rabUri_above.Size = new Size(61, 19);
            rabUri_above.TabIndex = 0;
            rabUri_above.TabStop = true;
            rabUri_above.Text = "above";
            rabUri_above.UseVisualStyleBackColor = true;
            // 
            // label22
            // 
            label22.BorderStyle = BorderStyle.Fixed3D;
            label22.Location = new Point(6, 168);
            label22.Name = "label22";
            label22.Size = new Size(336, 2);
            label22.TabIndex = 30;
            // 
            // panString
            // 
            panString.Controls.Add(tableLayoutPanel5);
            panString.Controls.Add(grbString);
            panString.Controls.Add(label16);
            panString.Location = new Point(0, 0);
            panString.Name = "panString";
            panString.Size = new Size(342, 334);
            panString.TabIndex = 23;
            panString.Visible = false;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 2;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            tableLayoutPanel5.Controls.Add(label15, 0, 0);
            tableLayoutPanel5.Controls.Add(txtString, 1, 0);
            tableLayoutPanel5.Location = new Point(3, 3);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 5;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel5.Size = new Size(336, 150);
            tableLayoutPanel5.TabIndex = 4;
            // 
            // label15
            // 
            label15.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label15.AutoSize = true;
            label15.Location = new Point(1, 1);
            label15.Margin = new Padding(1);
            label15.Name = "label15";
            label15.Size = new Size(98, 28);
            label15.TabIndex = 0;
            label15.Text = "value";
            label15.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtString
            // 
            txtString.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtString.Location = new Point(103, 3);
            txtString.Name = "txtString";
            txtString.Size = new Size(230, 23);
            txtString.TabIndex = 1;
            // 
            // grbString
            // 
            grbString.Controls.Add(rabString_text);
            grbString.Controls.Add(rabString_missing);
            grbString.Controls.Add(rabString_exact);
            grbString.Controls.Add(rabString_contains);
            grbString.Location = new Point(3, 173);
            grbString.Name = "grbString";
            grbString.Size = new Size(336, 130);
            grbString.TabIndex = 3;
            grbString.TabStop = false;
            grbString.Text = "Modifier";
            // 
            // rabString_text
            // 
            rabString_text.AutoSize = true;
            rabString_text.Location = new Point(10, 101);
            rabString_text.Name = "rabString_text";
            rabString_text.Size = new Size(46, 19);
            rabString_text.TabIndex = 3;
            rabString_text.TabStop = true;
            rabString_text.Text = "text";
            rabString_text.UseVisualStyleBackColor = true;
            // 
            // rabString_missing
            // 
            rabString_missing.AutoSize = true;
            rabString_missing.Location = new Point(10, 77);
            rabString_missing.Name = "rabString_missing";
            rabString_missing.Size = new Size(67, 19);
            rabString_missing.TabIndex = 2;
            rabString_missing.TabStop = true;
            rabString_missing.Text = "missing";
            rabString_missing.UseVisualStyleBackColor = true;
            // 
            // rabString_exact
            // 
            rabString_exact.AutoSize = true;
            rabString_exact.Location = new Point(10, 52);
            rabString_exact.Name = "rabString_exact";
            rabString_exact.Size = new Size(55, 19);
            rabString_exact.TabIndex = 1;
            rabString_exact.TabStop = true;
            rabString_exact.Text = "exact";
            rabString_exact.UseVisualStyleBackColor = true;
            // 
            // rabString_contains
            // 
            rabString_contains.AutoSize = true;
            rabString_contains.Location = new Point(10, 27);
            rabString_contains.Name = "rabString_contains";
            rabString_contains.Size = new Size(72, 19);
            rabString_contains.TabIndex = 0;
            rabString_contains.TabStop = true;
            rabString_contains.Text = "contains";
            rabString_contains.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            label16.BorderStyle = BorderStyle.Fixed3D;
            label16.Location = new Point(3, 168);
            label16.Name = "label16";
            label16.Size = new Size(336, 2);
            label16.TabIndex = 2;
            // 
            // panNumber
            // 
            panNumber.Controls.Add(tableLayoutPanel4);
            panNumber.Controls.Add(grbNumber);
            panNumber.Controls.Add(label30);
            panNumber.Location = new Point(0, 0);
            panNumber.Name = "panNumber";
            panNumber.Size = new Size(342, 334);
            panNumber.TabIndex = 30;
            panNumber.Visible = false;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            tableLayoutPanel4.Controls.Add(label28, 0, 0);
            tableLayoutPanel4.Controls.Add(label29, 0, 1);
            tableLayoutPanel4.Controls.Add(cobNumber_prefix, 1, 0);
            tableLayoutPanel4.Controls.Add(txtNumber_value, 1, 1);
            tableLayoutPanel4.Location = new Point(3, 3);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 5;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel4.Size = new Size(336, 150);
            tableLayoutPanel4.TabIndex = 32;
            // 
            // label28
            // 
            label28.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label28.AutoSize = true;
            label28.Location = new Point(1, 1);
            label28.Margin = new Padding(1);
            label28.Name = "label28";
            label28.Padding = new Padding(1);
            label28.Size = new Size(98, 28);
            label28.TabIndex = 0;
            label28.Text = "prefix";
            label28.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label29
            // 
            label29.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label29.AutoSize = true;
            label29.Location = new Point(1, 31);
            label29.Margin = new Padding(1);
            label29.Name = "label29";
            label29.Padding = new Padding(1);
            label29.Size = new Size(98, 28);
            label29.TabIndex = 3;
            label29.Text = "value";
            label29.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cobNumber_prefix
            // 
            cobNumber_prefix.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            cobNumber_prefix.FormattingEnabled = true;
            cobNumber_prefix.Items.AddRange(new object[] { "gt", "lt", "ge", "le", "sa", "eb" });
            cobNumber_prefix.Location = new Point(103, 3);
            cobNumber_prefix.Name = "cobNumber_prefix";
            cobNumber_prefix.Size = new Size(230, 23);
            cobNumber_prefix.TabIndex = 1;
            // 
            // txtNumber_value
            // 
            txtNumber_value.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtNumber_value.Location = new Point(103, 33);
            txtNumber_value.Name = "txtNumber_value";
            txtNumber_value.Size = new Size(230, 23);
            txtNumber_value.TabIndex = 2;
            // 
            // grbNumber
            // 
            grbNumber.Controls.Add(rabNumber_missing);
            grbNumber.Location = new Point(3, 173);
            grbNumber.Name = "grbNumber";
            grbNumber.Size = new Size(336, 130);
            grbNumber.TabIndex = 31;
            grbNumber.TabStop = false;
            grbNumber.Text = "Modifier";
            // 
            // rabNumber_missing
            // 
            rabNumber_missing.AutoSize = true;
            rabNumber_missing.Location = new Point(10, 29);
            rabNumber_missing.Name = "rabNumber_missing";
            rabNumber_missing.Size = new Size(67, 19);
            rabNumber_missing.TabIndex = 0;
            rabNumber_missing.TabStop = true;
            rabNumber_missing.Text = "missing";
            rabNumber_missing.UseVisualStyleBackColor = true;
            // 
            // label30
            // 
            label30.BorderStyle = BorderStyle.Fixed3D;
            label30.Location = new Point(3, 168);
            label30.Name = "label30";
            label30.Size = new Size(336, 2);
            label30.TabIndex = 30;
            // 
            // panDate
            // 
            panDate.Controls.Add(tableLayoutPanel2);
            panDate.Controls.Add(grbDate);
            panDate.Controls.Add(label33);
            panDate.Location = new Point(0, 0);
            panDate.Name = "panDate";
            panDate.Size = new Size(342, 334);
            panDate.TabIndex = 30;
            panDate.Visible = false;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            tableLayoutPanel2.Controls.Add(label31, 0, 0);
            tableLayoutPanel2.Controls.Add(cobDate_prefix, 1, 0);
            tableLayoutPanel2.Controls.Add(txtDate_value, 1, 1);
            tableLayoutPanel2.Controls.Add(label32, 0, 1);
            tableLayoutPanel2.Location = new Point(3, 6);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 5;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel2.Size = new Size(336, 150);
            tableLayoutPanel2.TabIndex = 33;
            // 
            // label31
            // 
            label31.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label31.AutoSize = true;
            label31.Location = new Point(1, 1);
            label31.Margin = new Padding(1);
            label31.Name = "label31";
            label31.Size = new Size(98, 28);
            label31.TabIndex = 0;
            label31.Text = "prefix";
            label31.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cobDate_prefix
            // 
            cobDate_prefix.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            cobDate_prefix.FormattingEnabled = true;
            cobDate_prefix.Items.AddRange(new object[] { "eq", "ne", "gt", "lt", "ge", "le", "sa", "eb", "ap" });
            cobDate_prefix.Location = new Point(103, 3);
            cobDate_prefix.Name = "cobDate_prefix";
            cobDate_prefix.Size = new Size(230, 23);
            cobDate_prefix.TabIndex = 1;
            // 
            // txtDate_value
            // 
            txtDate_value.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtDate_value.Location = new Point(103, 33);
            txtDate_value.Name = "txtDate_value";
            txtDate_value.Size = new Size(230, 23);
            txtDate_value.TabIndex = 3;
            // 
            // label32
            // 
            label32.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label32.AutoSize = true;
            label32.Location = new Point(1, 31);
            label32.Margin = new Padding(1);
            label32.Name = "label32";
            label32.Size = new Size(98, 28);
            label32.TabIndex = 2;
            label32.Text = "value";
            label32.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // grbDate
            // 
            grbDate.Controls.Add(rabDate_missing);
            grbDate.Location = new Point(9, 173);
            grbDate.Name = "grbDate";
            grbDate.Size = new Size(330, 130);
            grbDate.TabIndex = 32;
            grbDate.TabStop = false;
            grbDate.Text = "Modifier";
            // 
            // rabDate_missing
            // 
            rabDate_missing.AutoSize = true;
            rabDate_missing.Location = new Point(10, 22);
            rabDate_missing.Name = "rabDate_missing";
            rabDate_missing.Size = new Size(67, 19);
            rabDate_missing.TabIndex = 0;
            rabDate_missing.TabStop = true;
            rabDate_missing.Text = "missing";
            rabDate_missing.UseVisualStyleBackColor = true;
            // 
            // label33
            // 
            label33.BorderStyle = BorderStyle.Fixed3D;
            label33.Location = new Point(0, 168);
            label33.Name = "label33";
            label33.Size = new Size(336, 2);
            label33.TabIndex = 31;
            // 
            // Btn_AddParameter
            // 
            Btn_AddParameter.Location = new Point(496, 446);
            Btn_AddParameter.Name = "Btn_AddParameter";
            Btn_AddParameter.Size = new Size(76, 23);
            Btn_AddParameter.TabIndex = 11;
            Btn_AddParameter.Text = "Add";
            Btn_AddParameter.UseVisualStyleBackColor = true;
            Btn_AddParameter.Click += Btn_AddParameter_Click;
            // 
            // Lib_ParameterList
            // 
            Lib_ParameterList.FormattingEnabled = true;
            Lib_ParameterList.ItemHeight = 15;
            Lib_ParameterList.Location = new Point(576, 90);
            Lib_ParameterList.Name = "Lib_ParameterList";
            Lib_ParameterList.Size = new Size(181, 349);
            Lib_ParameterList.TabIndex = 12;
            Lib_ParameterList.SelectedIndexChanged += Lib_ParameterList_SelectedIndexChanged;
            // 
            // Btn_RemoveParameter
            // 
            Btn_RemoveParameter.Location = new Point(576, 446);
            Btn_RemoveParameter.Name = "Btn_RemoveParameter";
            Btn_RemoveParameter.Size = new Size(76, 23);
            Btn_RemoveParameter.TabIndex = 13;
            Btn_RemoveParameter.Text = "Remove";
            Btn_RemoveParameter.UseVisualStyleBackColor = true;
            Btn_RemoveParameter.Click += Btn_RemoveParameter_Click;
            // 
            // Btn_ModifyingResult
            // 
            Btn_ModifyingResult.Enabled = false;
            Btn_ModifyingResult.Location = new Point(577, 673);
            Btn_ModifyingResult.Name = "Btn_ModifyingResult";
            Btn_ModifyingResult.Size = new Size(180, 26);
            Btn_ModifyingResult.TabIndex = 14;
            Btn_ModifyingResult.Text = "Modifying Results";
            Btn_ModifyingResult.UseVisualStyleBackColor = true;
            Btn_ModifyingResult.Click += Btn_ModifyingResult_Click;
            // 
            // Btn_ResourceOperation
            // 
            Btn_ResourceOperation.Enabled = false;
            Btn_ResourceOperation.Location = new Point(577, 673);
            Btn_ResourceOperation.Name = "Btn_ResourceOperation";
            Btn_ResourceOperation.Size = new Size(180, 26);
            Btn_ResourceOperation.TabIndex = 16;
            Btn_ResourceOperation.Text = "Resource Operation";
            Btn_ResourceOperation.UseVisualStyleBackColor = true;
            Btn_ResourceOperation.Visible = false;
            // 
            // txt_Message
            // 
            txt_Message.Location = new Point(762, 90);
            txt_Message.Multiline = true;
            txt_Message.Name = "txt_Message";
            txt_Message.ScrollBars = ScrollBars.Vertical;
            txt_Message.Size = new Size(417, 609);
            txt_Message.TabIndex = 17;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(763, 66);
            label11.Name = "label11";
            label11.Size = new Size(41, 15);
            label11.TabIndex = 18;
            label11.Text = "Result";
            // 
            // Btn_TreeView
            // 
            Btn_TreeView.Location = new Point(810, 61);
            Btn_TreeView.Name = "Btn_TreeView";
            Btn_TreeView.Size = new Size(99, 23);
            Btn_TreeView.TabIndex = 19;
            Btn_TreeView.Text = "Tree View";
            Btn_TreeView.UseVisualStyleBackColor = true;
            Btn_TreeView.Click += Btn_TreeView_Click;
            // 
            // Btn_BundleHelper
            // 
            Btn_BundleHelper.Enabled = false;
            Btn_BundleHelper.Location = new Point(1081, 61);
            Btn_BundleHelper.Name = "Btn_BundleHelper";
            Btn_BundleHelper.Size = new Size(98, 23);
            Btn_BundleHelper.TabIndex = 20;
            Btn_BundleHelper.Text = "Bundle Helper";
            Btn_BundleHelper.UseVisualStyleBackColor = true;
            // 
            // Clb_Include
            // 
            Clb_Include.FormattingEnabled = true;
            Clb_Include.Location = new Point(16, 479);
            Clb_Include.Name = "Clb_Include";
            Clb_Include.Size = new Size(202, 220);
            Clb_Include.TabIndex = 21;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(16, 461);
            label12.Name = "label12";
            label12.Size = new Size(53, 15);
            label12.TabIndex = 22;
            label12.Text = "_include";
            // 
            // Clb_Revinclude
            // 
            Clb_Revinclude.FormattingEnabled = true;
            Clb_Revinclude.Location = new Point(228, 479);
            Clb_Revinclude.Name = "Clb_Revinclude";
            Clb_Revinclude.Size = new Size(343, 220);
            Clb_Revinclude.TabIndex = 23;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(228, 461);
            label17.Name = "label17";
            label17.Size = new Size(70, 15);
            label17.TabIndex = 24;
            label17.Text = "_revinclude";
            // 
            // Ckb_includeAll
            // 
            Ckb_includeAll.AutoSize = true;
            Ckb_includeAll.Location = new Point(75, 460);
            Ckb_includeAll.Name = "Ckb_includeAll";
            Ckb_includeAll.Size = new Size(39, 19);
            Ckb_includeAll.TabIndex = 25;
            Ckb_includeAll.Text = "all";
            Ckb_includeAll.UseVisualStyleBackColor = true;
            // 
            // Ckb_RevincludeAll
            // 
            Ckb_RevincludeAll.AutoSize = true;
            Ckb_RevincludeAll.Location = new Point(304, 460);
            Ckb_RevincludeAll.Name = "Ckb_RevincludeAll";
            Ckb_RevincludeAll.Size = new Size(39, 19);
            Ckb_RevincludeAll.TabIndex = 26;
            Ckb_RevincludeAll.Text = "all";
            Ckb_RevincludeAll.UseVisualStyleBackColor = true;
            // 
            // Txt_SearchUrl
            // 
            Txt_SearchUrl.Location = new Point(52, 716);
            Txt_SearchUrl.Name = "Txt_SearchUrl";
            Txt_SearchUrl.Size = new Size(519, 23);
            Txt_SearchUrl.TabIndex = 27;
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new Point(16, 719);
            label18.Name = "label18";
            label18.Size = new Size(30, 15);
            label18.TabIndex = 28;
            label18.Text = "URL";
            // 
            // Btn_Search
            // 
            Btn_Search.Location = new Point(658, 715);
            Btn_Search.Name = "Btn_Search";
            Btn_Search.Size = new Size(75, 23);
            Btn_Search.TabIndex = 29;
            Btn_Search.Text = "Search";
            Btn_Search.UseVisualStyleBackColor = true;
            Btn_Search.Click += Btn_Search_Click;
            // 
            // Btn_Copy
            // 
            Btn_Copy.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Btn_Copy.Location = new Point(939, 705);
            Btn_Copy.Name = "Btn_Copy";
            Btn_Copy.Size = new Size(75, 23);
            Btn_Copy.TabIndex = 30;
            Btn_Copy.Text = "Copy";
            Btn_Copy.UseVisualStyleBackColor = true;
            Btn_Copy.Click += Btn_Copy_Click;
            // 
            // Btn_Save
            // 
            Btn_Save.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Btn_Save.Location = new Point(1022, 705);
            Btn_Save.Name = "Btn_Save";
            Btn_Save.Size = new Size(75, 23);
            Btn_Save.TabIndex = 31;
            Btn_Save.Text = "Save";
            Btn_Save.UseVisualStyleBackColor = true;
            Btn_Save.Click += Btn_Save_Click;
            // 
            // Btn_CreateUrl
            // 
            Btn_CreateUrl.Location = new Point(577, 716);
            Btn_CreateUrl.Name = "Btn_CreateUrl";
            Btn_CreateUrl.Size = new Size(75, 23);
            Btn_CreateUrl.TabIndex = 32;
            Btn_CreateUrl.Text = "Create";
            Btn_CreateUrl.UseVisualStyleBackColor = true;
            Btn_CreateUrl.Click += Btn_CreateUrl_Click;
            // 
            // Btn_Exit
            // 
            Btn_Exit.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Btn_Exit.Location = new Point(1105, 705);
            Btn_Exit.Name = "Btn_Exit";
            Btn_Exit.Size = new Size(75, 23);
            Btn_Exit.TabIndex = 37;
            Btn_Exit.Text = "Exit";
            Btn_Exit.UseVisualStyleBackColor = true;
            Btn_Exit.Click += Btn_Exit_Click;
            // 
            // trv_Message
            // 
            trv_Message.Location = new Point(762, 90);
            trv_Message.Name = "trv_Message";
            trv_Message.Size = new Size(417, 609);
            trv_Message.TabIndex = 33;
            trv_Message.Visible = false;
            // 
            // lab_Loading
            // 
            lab_Loading.AutoSize = true;
            lab_Loading.Location = new Point(393, 13);
            lab_Loading.Name = "lab_Loading";
            lab_Loading.Size = new Size(66, 15);
            lab_Loading.TabIndex = 34;
            lab_Loading.Text = "Loading ...";
            lab_Loading.Visible = false;
            // 
            // Btn_RemoveAllParameter
            // 
            Btn_RemoveAllParameter.Enabled = false;
            Btn_RemoveAllParameter.Location = new Point(658, 446);
            Btn_RemoveAllParameter.Name = "Btn_RemoveAllParameter";
            Btn_RemoveAllParameter.Size = new Size(98, 23);
            Btn_RemoveAllParameter.TabIndex = 35;
            Btn_RemoveAllParameter.Text = "Remove All";
            Btn_RemoveAllParameter.UseVisualStyleBackColor = true;
            Btn_RemoveAllParameter.Click += Btn_RemoveAllParameter_Click;
            // 
            // tab_LastControl
            // 
            tab_LastControl.Controls.Add(tap_Modifying);
            tab_LastControl.Location = new Point(576, 479);
            tab_LastControl.Name = "tab_LastControl";
            tab_LastControl.SelectedIndex = 0;
            tab_LastControl.Size = new Size(180, 188);
            tab_LastControl.TabIndex = 36;
            // 
            // tap_Modifying
            // 
            tap_Modifying.Controls.Add(Lib_ModifyingList);
            tap_Modifying.Location = new Point(4, 24);
            tap_Modifying.Name = "tap_Modifying";
            tap_Modifying.Padding = new Padding(3);
            tap_Modifying.Size = new Size(172, 160);
            tap_Modifying.TabIndex = 0;
            tap_Modifying.Text = "Modifying";
            tap_Modifying.UseVisualStyleBackColor = true;
            // 
            // Lib_ModifyingList
            // 
            Lib_ModifyingList.FormattingEnabled = true;
            Lib_ModifyingList.ItemHeight = 15;
            Lib_ModifyingList.Location = new Point(3, 1);
            Lib_ModifyingList.Name = "Lib_ModifyingList";
            Lib_ModifyingList.Size = new Size(169, 154);
            Lib_ModifyingList.TabIndex = 0;
            // 
            // NewMainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1196, 762);
            Controls.Add(tab_LastControl);
            Controls.Add(Btn_RemoveAllParameter);
            Controls.Add(lab_Loading);
            Controls.Add(trv_Message);
            Controls.Add(Btn_CreateUrl);
            Controls.Add(Btn_Exit);
            Controls.Add(Btn_Save);
            Controls.Add(Btn_Copy);
            Controls.Add(Btn_Search);
            Controls.Add(label18);
            Controls.Add(Txt_SearchUrl);
            Controls.Add(Ckb_RevincludeAll);
            Controls.Add(Ckb_includeAll);
            Controls.Add(label17);
            Controls.Add(Clb_Revinclude);
            Controls.Add(label12);
            Controls.Add(Clb_Include);
            Controls.Add(Btn_BundleHelper);
            Controls.Add(Btn_TreeView);
            Controls.Add(label11);
            Controls.Add(txt_Message);
            Controls.Add(Btn_ResourceOperation);
            Controls.Add(Btn_ModifyingResult);
            Controls.Add(Btn_RemoveParameter);
            Controls.Add(Lib_ParameterList);
            Controls.Add(Btn_AddParameter);
            Controls.Add(Pan_InputParameters);
            Controls.Add(lab_SerchType);
            Controls.Add(lab_SearchField);
            Controls.Add(tabControl1);
            Controls.Add(Cob_Resource);
            Controls.Add(Btn_GetToken);
            Controls.Add(label2);
            Controls.Add(Btn_Connect);
            Controls.Add(txt_FhirUrl);
            Controls.Add(label1);
            Name = "NewMainForm";
            Padding = new Padding(1);
            Text = "FHIR Query Builder";
            tabControl1.ResumeLayout(false);
            tap_Common.ResumeLayout(false);
            tap_Resource.ResumeLayout(false);
            Pan_InputParameters.ResumeLayout(false);
            panReference.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            grbReference.ResumeLayout(false);
            grbReference.PerformLayout();
            panQuantity.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            grbQuantity.ResumeLayout(false);
            grbQuantity.PerformLayout();
            panToken.ResumeLayout(false);
            tableLayoutPanel6.ResumeLayout(false);
            tableLayoutPanel6.PerformLayout();
            grbToken.ResumeLayout(false);
            grbToken.PerformLayout();
            panUri.ResumeLayout(false);
            tableLayoutPanel7.ResumeLayout(false);
            tableLayoutPanel7.PerformLayout();
            grbUri.ResumeLayout(false);
            grbUri.PerformLayout();
            panString.ResumeLayout(false);
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel5.PerformLayout();
            grbString.ResumeLayout(false);
            grbString.PerformLayout();
            panNumber.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel4.PerformLayout();
            grbNumber.ResumeLayout(false);
            grbNumber.PerformLayout();
            panDate.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            grbDate.ResumeLayout(false);
            grbDate.PerformLayout();
            tab_LastControl.ResumeLayout(false);
            tap_Modifying.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txt_FhirUrl;
        private Button Btn_Connect;
        private Label label2;
        private Button Btn_GetToken;
        private ComboBox Cob_Resource;
        private TabControl tabControl1;
        private TabPage tap_Common;
        private TabPage tap_Resource;
        private TabPage tap_Advanced;
        private Controls.AdvancedSearchControl advancedSearchControl;
        private Label lab_SearchField;
        private Label lab_SerchType;
        private Panel Pan_InputParameters;
        private ListBox Lib_CommonParameter;
        private ListBox Lib_ResourceParameter;
        private Panel panReference;
        private Label label14;
        private GroupBox grbReference;
        private RadioButton rabReference_text;
        private RadioButton rabReference_notin;
        private RadioButton rabReference_missing;
        private RadioButton rabReference_identifier;
        private RadioButton rabReference_below;
        private RadioButton rabReference_above;
        private TextBox txtReference_version;
        private Label label13;
        private TextBox txtReference_url;
        private Label label10;
        private TextBox txtReference_TypeId_Type;
        private TextBox txtReference_TypeId_Id;
        private Label label8;
        private TextBox txtReference_id;
        private Label label7;
        private Panel panDate;
        private GroupBox grbDate;
        private RadioButton rabDate_missing;
        private Label label33;
        private TextBox txtDate_value;
        private Label label32;
        private ComboBox cobDate_prefix;
        private Label label31;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private Panel panQuantity;
        private GroupBox grbQuantity;
        private RadioButton rabQuantity_missing;
        private Label label27;
        private TextBox txtQuantity_nsc_code;
        private TextBox txtQuantity_nsc_system;
        private TextBox txtQuantity_nsc_number;
        private Label label26;
        private TextBox txtQuantity_number;
        private Label label25;
        private ComboBox cobQuantity_prefix;
        private Label label24;
        private TableLayoutPanel tableLayoutPanel3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Panel panNumber;
        private TableLayoutPanel tableLayoutPanel4;
        private GroupBox grbNumber;
        private RadioButton rabNumber_missing;
        private Label label30;
        private Label label29;
        private TextBox txtNumber_value;
        private ComboBox cobNumber_prefix;
        private Label label28;
        private Panel panString;
        private GroupBox grbString;
        private RadioButton rabString_text;
        private RadioButton rabString_missing;
        private RadioButton rabString_exact;
        private RadioButton rabString_contains;
        private Label label16;
        private TextBox txtString;
        private Label label15;
        private TableLayoutPanel tableLayoutPanel5;
        private Panel panToken;
        private GroupBox grbToken;
        private RadioButton rabToken_text;
        private RadioButton rabToken_oftype;
        private RadioButton rabToken_notin;
        private RadioButton rabToken_not;
        private RadioButton rabToken_missing;
        private RadioButton rabToken_in;
        private RadioButton rabToken_below;
        private RadioButton rabToken_above;
        private Label label23;
        private TextBox txtToken_SystemCode_Code;
        private TextBox txtToken_SystemCode_System;
        private Label label20;
        private TextBox txtToken_Code;
        private Label label19;
        private TableLayoutPanel tableLayoutPanel6;
        private Label label9;
        private Panel panUri;
        private TableLayoutPanel tableLayoutPanel7;
        private GroupBox grbUri;
        private RadioButton rabUri_missing;
        private RadioButton rabUri_contains;
        private RadioButton rabUri_below;
        private RadioButton rabUri_above;
        private Label label22;
        private TextBox txtUri_url;
        private Label label21;
        private Button Btn_AddParameter;
        private ListBox Lib_ParameterList;
        private Button Btn_RemoveParameter;
        private Button Btn_ModifyingResult;
        private Button Btn_ResourceOperation;
        private TextBox txt_Message;
        private Label label11;
        private Button Btn_TreeView;
        private Button Btn_BundleHelper;
        private CheckedListBox Clb_Include;
        private Label label12;
        private CheckedListBox Clb_Revinclude;
        private Label label17;
        private CheckBox Ckb_includeAll;
        private CheckBox Ckb_RevincludeAll;
        private TextBox Txt_SearchUrl;
        private Label label18;
        private Button Btn_Search;
        private Button Btn_Copy;
        private Button Btn_Save;
        private Button Btn_CreateUrl;
        private Button Btn_Exit;
        private TreeView trv_Message;
        private Label lab_Loading;
        private Button Btn_RemoveAllParameter;
        private TabControl tab_LastControl;
        private TabPage tap_Modifying;
        private ListBox Lib_ModifyingList;
    }
}