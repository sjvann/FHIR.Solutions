namespace Fhir.QueryBuilder.Controls
{
    partial class AdvancedSearchControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabAdvanced = new TabControl();
            tabChaining = new TabPage();
            grpChaining = new GroupBox();
            btnAddChain = new Button();
            txtChainValue = new TextBox();
            txtChainPath = new TextBox();
            lblChainValue = new Label();
            lblChainPath = new Label();
            lstChains = new ListBox();
            tabComposite = new TabPage();
            grpComposite = new GroupBox();
            btnAddComposite = new Button();
            txtCompositeComponents = new TextBox();
            txtCompositeParam = new TextBox();
            lblCompositeComponents = new Label();
            lblCompositeParam = new Label();
            lstComposite = new ListBox();
            tabFilter = new TabPage();
            grpFilter = new GroupBox();
            btnAddFilter = new Button();
            txtFilterExpression = new TextBox();
            lblFilterExpression = new Label();
            lstFilters = new ListBox();
            tabResultControl = new TabPage();
            grpResultControl = new GroupBox();
            txtElements = new TextBox();
            lblElements = new Label();
            cboSummary = new ComboBox();
            lblSummary = new Label();
            cboTotal = new ComboBox();
            lblTotal = new Label();
            txtOffset = new TextBox();
            lblOffset = new Label();
            txtCount = new TextBox();
            lblCount = new Label();
            btnClearAll = new Button();
            tabAdvanced.SuspendLayout();
            tabChaining.SuspendLayout();
            grpChaining.SuspendLayout();
            tabComposite.SuspendLayout();
            grpComposite.SuspendLayout();
            tabFilter.SuspendLayout();
            grpFilter.SuspendLayout();
            tabResultControl.SuspendLayout();
            grpResultControl.SuspendLayout();
            SuspendLayout();
            // 
            // tabAdvanced
            // 
            tabAdvanced.Controls.Add(tabChaining);
            tabAdvanced.Controls.Add(tabComposite);
            tabAdvanced.Controls.Add(tabFilter);
            tabAdvanced.Controls.Add(tabResultControl);
            tabAdvanced.Dock = DockStyle.Fill;
            tabAdvanced.Location = new Point(0, 0);
            tabAdvanced.Name = "tabAdvanced";
            tabAdvanced.SelectedIndex = 0;
            tabAdvanced.Size = new Size(400, 500);
            tabAdvanced.TabIndex = 0;
            // 
            // tabChaining
            // 
            tabChaining.Controls.Add(grpChaining);
            tabChaining.Controls.Add(lstChains);
            tabChaining.Location = new Point(4, 24);
            tabChaining.Name = "tabChaining";
            tabChaining.Padding = new Padding(3);
            tabChaining.Size = new Size(392, 472);
            tabChaining.TabIndex = 0;
            tabChaining.Text = "Chaining";
            tabChaining.UseVisualStyleBackColor = true;
            // 
            // grpChaining
            // 
            grpChaining.Controls.Add(btnAddChain);
            grpChaining.Controls.Add(txtChainValue);
            grpChaining.Controls.Add(txtChainPath);
            grpChaining.Controls.Add(lblChainValue);
            grpChaining.Controls.Add(lblChainPath);
            grpChaining.Dock = DockStyle.Top;
            grpChaining.Location = new Point(3, 3);
            grpChaining.Name = "grpChaining";
            grpChaining.Size = new Size(386, 120);
            grpChaining.TabIndex = 0;
            grpChaining.TabStop = false;
            grpChaining.Text = "Add Chaining Parameter";
            // 
            // btnAddChain
            // 
            btnAddChain.Location = new Point(300, 80);
            btnAddChain.Name = "btnAddChain";
            btnAddChain.Size = new Size(75, 23);
            btnAddChain.TabIndex = 4;
            btnAddChain.Text = "Add";
            btnAddChain.UseVisualStyleBackColor = true;
            btnAddChain.Click += btnAddChain_Click;
            // 
            // txtChainValue
            // 
            txtChainValue.Location = new Point(80, 50);
            txtChainValue.Name = "txtChainValue";
            txtChainValue.Size = new Size(295, 23);
            txtChainValue.TabIndex = 3;
            // 
            // txtChainPath
            // 
            txtChainPath.Location = new Point(80, 20);
            txtChainPath.Name = "txtChainPath";
            txtChainPath.Size = new Size(295, 23);
            txtChainPath.TabIndex = 2;
            // 
            // lblChainValue
            // 
            lblChainValue.AutoSize = true;
            lblChainValue.Location = new Point(10, 53);
            lblChainValue.Name = "lblChainValue";
            lblChainValue.Size = new Size(38, 15);
            lblChainValue.TabIndex = 1;
            lblChainValue.Text = "Value:";
            // 
            // lblChainPath
            // 
            lblChainPath.AutoSize = true;
            lblChainPath.Location = new Point(10, 23);
            lblChainPath.Name = "lblChainPath";
            lblChainPath.Size = new Size(34, 15);
            lblChainPath.TabIndex = 0;
            lblChainPath.Text = "Path:";
            // 
            // lstChains
            // 
            lstChains.Dock = DockStyle.Fill;
            lstChains.FormattingEnabled = true;
            lstChains.ItemHeight = 15;
            lstChains.Location = new Point(3, 123);
            lstChains.Name = "lstChains";
            lstChains.Size = new Size(386, 346);
            lstChains.TabIndex = 1;
            lstChains.KeyDown += lstChains_KeyDown;
            // 
            // tabComposite
            // 
            tabComposite.Controls.Add(grpComposite);
            tabComposite.Controls.Add(lstComposite);
            tabComposite.Location = new Point(4, 24);
            tabComposite.Name = "tabComposite";
            tabComposite.Padding = new Padding(3);
            tabComposite.Size = new Size(392, 472);
            tabComposite.TabIndex = 1;
            tabComposite.Text = "Composite";
            tabComposite.UseVisualStyleBackColor = true;
            // 
            // grpComposite
            // 
            grpComposite.Controls.Add(btnAddComposite);
            grpComposite.Controls.Add(txtCompositeComponents);
            grpComposite.Controls.Add(txtCompositeParam);
            grpComposite.Controls.Add(lblCompositeComponents);
            grpComposite.Controls.Add(lblCompositeParam);
            grpComposite.Dock = DockStyle.Top;
            grpComposite.Location = new Point(3, 3);
            grpComposite.Name = "grpComposite";
            grpComposite.Size = new Size(386, 120);
            grpComposite.TabIndex = 0;
            grpComposite.TabStop = false;
            grpComposite.Text = "Add Composite Parameter";
            // 
            // btnAddComposite
            // 
            btnAddComposite.Location = new Point(300, 80);
            btnAddComposite.Name = "btnAddComposite";
            btnAddComposite.Size = new Size(75, 23);
            btnAddComposite.TabIndex = 4;
            btnAddComposite.Text = "Add";
            btnAddComposite.UseVisualStyleBackColor = true;
            btnAddComposite.Click += btnAddComposite_Click;
            // 
            // txtCompositeComponents
            // 
            txtCompositeComponents.Location = new Point(80, 50);
            txtCompositeComponents.Name = "txtCompositeComponents";
            txtCompositeComponents.Size = new Size(295, 23);
            txtCompositeComponents.TabIndex = 3;
            // 
            // txtCompositeParam
            // 
            txtCompositeParam.Location = new Point(80, 20);
            txtCompositeParam.Name = "txtCompositeParam";
            txtCompositeParam.Size = new Size(295, 23);
            txtCompositeParam.TabIndex = 2;
            // 
            // lblCompositeComponents
            // 
            lblCompositeComponents.AutoSize = true;
            lblCompositeComponents.Location = new Point(10, 53);
            lblCompositeComponents.Name = "lblCompositeComponents";
            lblCompositeComponents.Size = new Size(77, 15);
            lblCompositeComponents.TabIndex = 1;
            lblCompositeComponents.Text = "Components:";
            // 
            // lblCompositeParam
            // 
            lblCompositeParam.AutoSize = true;
            lblCompositeParam.Location = new Point(10, 23);
            lblCompositeParam.Name = "lblCompositeParam";
            lblCompositeParam.Size = new Size(64, 15);
            lblCompositeParam.TabIndex = 0;
            lblCompositeParam.Text = "Parameter:";
            // 
            // lstComposite
            // 
            lstComposite.Dock = DockStyle.Fill;
            lstComposite.FormattingEnabled = true;
            lstComposite.ItemHeight = 15;
            lstComposite.Location = new Point(3, 123);
            lstComposite.Name = "lstComposite";
            lstComposite.Size = new Size(386, 346);
            lstComposite.TabIndex = 1;
            lstComposite.KeyDown += lstComposite_KeyDown;
            // 
            // tabFilter
            // 
            tabFilter.Controls.Add(grpFilter);
            tabFilter.Controls.Add(lstFilters);
            tabFilter.Location = new Point(4, 24);
            tabFilter.Name = "tabFilter";
            tabFilter.Size = new Size(392, 472);
            tabFilter.TabIndex = 2;
            tabFilter.Text = "Filter";
            tabFilter.UseVisualStyleBackColor = true;
            // 
            // grpFilter
            // 
            grpFilter.Controls.Add(btnAddFilter);
            grpFilter.Controls.Add(txtFilterExpression);
            grpFilter.Controls.Add(lblFilterExpression);
            grpFilter.Dock = DockStyle.Top;
            grpFilter.Location = new Point(0, 0);
            grpFilter.Name = "grpFilter";
            grpFilter.Size = new Size(392, 80);
            grpFilter.TabIndex = 0;
            grpFilter.TabStop = false;
            grpFilter.Text = "Add Filter Expression";
            // 
            // btnAddFilter
            // 
            btnAddFilter.Location = new Point(300, 45);
            btnAddFilter.Name = "btnAddFilter";
            btnAddFilter.Size = new Size(75, 23);
            btnAddFilter.TabIndex = 2;
            btnAddFilter.Text = "Add";
            btnAddFilter.UseVisualStyleBackColor = true;
            btnAddFilter.Click += btnAddFilter_Click;
            // 
            // txtFilterExpression
            // 
            txtFilterExpression.Location = new Point(80, 20);
            txtFilterExpression.Name = "txtFilterExpression";
            txtFilterExpression.Size = new Size(295, 23);
            txtFilterExpression.TabIndex = 1;
            // 
            // lblFilterExpression
            // 
            lblFilterExpression.AutoSize = true;
            lblFilterExpression.Location = new Point(10, 23);
            lblFilterExpression.Name = "lblFilterExpression";
            lblFilterExpression.Size = new Size(67, 15);
            lblFilterExpression.TabIndex = 0;
            lblFilterExpression.Text = "Expression:";
            // 
            // lstFilters
            // 
            lstFilters.Dock = DockStyle.Fill;
            lstFilters.FormattingEnabled = true;
            lstFilters.ItemHeight = 15;
            lstFilters.Location = new Point(0, 80);
            lstFilters.Name = "lstFilters";
            lstFilters.Size = new Size(392, 392);
            lstFilters.TabIndex = 1;
            lstFilters.KeyDown += lstFilters_KeyDown;
            // 
            // tabResultControl
            // 
            tabResultControl.Controls.Add(grpResultControl);
            tabResultControl.Location = new Point(4, 24);
            tabResultControl.Name = "tabResultControl";
            tabResultControl.Size = new Size(392, 472);
            tabResultControl.TabIndex = 3;
            tabResultControl.Text = "Result Control";
            tabResultControl.UseVisualStyleBackColor = true;
            // 
            // grpResultControl
            // 
            grpResultControl.Controls.Add(txtElements);
            grpResultControl.Controls.Add(lblElements);
            grpResultControl.Controls.Add(cboSummary);
            grpResultControl.Controls.Add(lblSummary);
            grpResultControl.Controls.Add(cboTotal);
            grpResultControl.Controls.Add(lblTotal);
            grpResultControl.Controls.Add(txtOffset);
            grpResultControl.Controls.Add(lblOffset);
            grpResultControl.Controls.Add(txtCount);
            grpResultControl.Controls.Add(lblCount);
            grpResultControl.Dock = DockStyle.Fill;
            grpResultControl.Location = new Point(0, 0);
            grpResultControl.Name = "grpResultControl";
            grpResultControl.Size = new Size(392, 472);
            grpResultControl.TabIndex = 0;
            grpResultControl.TabStop = false;
            grpResultControl.Text = "Result Control Parameters";
            // 
            // txtElements
            // 
            txtElements.Location = new Point(80, 140);
            txtElements.Name = "txtElements";
            txtElements.Size = new Size(295, 23);
            txtElements.TabIndex = 9;
            // 
            // lblElements
            // 
            lblElements.AutoSize = true;
            lblElements.Location = new Point(10, 143);
            lblElements.Name = "lblElements";
            lblElements.Size = new Size(58, 15);
            lblElements.TabIndex = 8;
            lblElements.Text = "Elements:";
            // 
            // cboSummary
            // 
            cboSummary.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSummary.FormattingEnabled = true;
            cboSummary.Items.AddRange(new object[] { "true", "false", "text", "data", "count" });
            cboSummary.Location = new Point(80, 110);
            cboSummary.Name = "cboSummary";
            cboSummary.Size = new Size(295, 23);
            cboSummary.TabIndex = 7;
            // 
            // lblSummary
            // 
            lblSummary.AutoSize = true;
            lblSummary.Location = new Point(10, 113);
            lblSummary.Name = "lblSummary";
            lblSummary.Size = new Size(62, 15);
            lblSummary.TabIndex = 6;
            lblSummary.Text = "Summary:";
            // 
            // cboTotal
            // 
            cboTotal.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTotal.FormattingEnabled = true;
            cboTotal.Items.AddRange(new object[] { "none", "estimate", "accurate" });
            cboTotal.Location = new Point(80, 80);
            cboTotal.Name = "cboTotal";
            cboTotal.Size = new Size(295, 23);
            cboTotal.TabIndex = 5;
            // 
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.Location = new Point(10, 83);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(35, 15);
            lblTotal.TabIndex = 4;
            lblTotal.Text = "Total:";
            // 
            // txtOffset
            // 
            txtOffset.Location = new Point(80, 50);
            txtOffset.Name = "txtOffset";
            txtOffset.Size = new Size(295, 23);
            txtOffset.TabIndex = 3;
            // 
            // lblOffset
            // 
            lblOffset.AutoSize = true;
            lblOffset.Location = new Point(10, 53);
            lblOffset.Name = "lblOffset";
            lblOffset.Size = new Size(42, 15);
            lblOffset.TabIndex = 2;
            lblOffset.Text = "Offset:";
            // 
            // txtCount
            // 
            txtCount.Location = new Point(80, 20);
            txtCount.Name = "txtCount";
            txtCount.Size = new Size(295, 23);
            txtCount.TabIndex = 1;
            // 
            // lblCount
            // 
            lblCount.AutoSize = true;
            lblCount.Location = new Point(10, 23);
            lblCount.Name = "lblCount";
            lblCount.Size = new Size(43, 15);
            lblCount.TabIndex = 0;
            lblCount.Text = "Count:";
            // 
            // btnClearAll
            // 
            btnClearAll.Dock = DockStyle.Bottom;
            btnClearAll.Location = new Point(0, 477);
            btnClearAll.Name = "btnClearAll";
            btnClearAll.Size = new Size(400, 23);
            btnClearAll.TabIndex = 1;
            btnClearAll.Text = "Clear All Advanced Parameters";
            btnClearAll.UseVisualStyleBackColor = true;
            btnClearAll.Click += btnClearAll_Click;
            // 
            // AdvancedSearchControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tabAdvanced);
            Controls.Add(btnClearAll);
            Name = "AdvancedSearchControl";
            Size = new Size(400, 500);
            tabAdvanced.ResumeLayout(false);
            tabChaining.ResumeLayout(false);
            grpChaining.ResumeLayout(false);
            grpChaining.PerformLayout();
            tabComposite.ResumeLayout(false);
            grpComposite.ResumeLayout(false);
            grpComposite.PerformLayout();
            tabFilter.ResumeLayout(false);
            grpFilter.ResumeLayout(false);
            grpFilter.PerformLayout();
            tabResultControl.ResumeLayout(false);
            grpResultControl.ResumeLayout(false);
            grpResultControl.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabAdvanced;
        private TabPage tabChaining;
        private TabPage tabComposite;
        private TabPage tabFilter;
        private TabPage tabResultControl;
        private GroupBox grpChaining;
        private Button btnAddChain;
        private TextBox txtChainValue;
        private TextBox txtChainPath;
        private Label lblChainValue;
        private Label lblChainPath;
        private ListBox lstChains;
        private GroupBox grpComposite;
        private Button btnAddComposite;
        private TextBox txtCompositeComponents;
        private TextBox txtCompositeParam;
        private Label lblCompositeComponents;
        private Label lblCompositeParam;
        private ListBox lstComposite;
        private GroupBox grpFilter;
        private Button btnAddFilter;
        private TextBox txtFilterExpression;
        private Label lblFilterExpression;
        private ListBox lstFilters;
        private GroupBox grpResultControl;
        private TextBox txtElements;
        private Label lblElements;
        private ComboBox cboSummary;
        private Label lblSummary;
        private ComboBox cboTotal;
        private Label lblTotal;
        private TextBox txtOffset;
        private Label lblOffset;
        private TextBox txtCount;
        private Label lblCount;
        private Button btnClearAll;
    }
}
