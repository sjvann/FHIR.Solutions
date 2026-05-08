namespace Fhir.QueryBuilder.PanelControl
{
    partial class PanelToken
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
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
            panString.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            grbString.SuspendLayout();
            SuspendLayout();
            // 
            // panString
            // 
            panString.Controls.Add(tableLayoutPanel5);
            panString.Controls.Add(grbString);
            panString.Controls.Add(label16);
            panString.Location = new Point(0, 0);
            panString.Name = "panString";
            panString.Size = new Size(342, 334);
            panString.TabIndex = 24;
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
            // PanelToken
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panString);
            Name = "PanelToken";
            Size = new Size(342, 334);
            panString.ResumeLayout(false);
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel5.PerformLayout();
            grbString.ResumeLayout(false);
            grbString.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panString;
        private TableLayoutPanel tableLayoutPanel5;
        private Label label15;
        private TextBox txtString;
        private GroupBox grbString;
        private RadioButton rabString_text;
        private RadioButton rabString_missing;
        private RadioButton rabString_exact;
        private RadioButton rabString_contains;
        private Label label16;
    }
}
