namespace Fhir.QueryBuilder
{
    partial class ModifyResult
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
            Pan_Contained = new Panel();
            grb_ContainedType = new GroupBox();
            rab_ContainedType_contained = new RadioButton();
            rab_ContainedType_container = new RadioButton();
            grb_Contained = new GroupBox();
            rab_Contained_Both = new RadioButton();
            rab_Contained_True = new RadioButton();
            rab_Contained_False = new RadioButton();
            pan_Number = new Panel();
            groupBox2 = new GroupBox();
            grb_Maxresults = new GroupBox();
            txt_Maxresults_value = new TextBox();
            groupBox1 = new GroupBox();
            txt_Count_value = new TextBox();
            Pan_Summary = new Panel();
            grb_Summary = new GroupBox();
            rab_Summary_count = new RadioButton();
            rab_Summary_data = new RadioButton();
            rab_summary_text = new RadioButton();
            rab_summary_false = new RadioButton();
            rab_Summary_true = new RadioButton();
            cob_ResourceElements = new ComboBox();
            lib_Elements = new ListBox();
            label1 = new Label();
            lib_Sort = new ListBox();
            Btn_Cancel = new Button();
            Btn_OK = new Button();
            Btn_AddToElements = new Button();
            rab_PrefixForSort = new RadioButton();
            Btn_AddToSort = new Button();
            label2 = new Label();
            Btn_ClearSort = new Button();
            Btn_ClearElement = new Button();
            Pan_Contained.SuspendLayout();
            grb_ContainedType.SuspendLayout();
            grb_Contained.SuspendLayout();
            pan_Number.SuspendLayout();
            grb_Maxresults.SuspendLayout();
            groupBox1.SuspendLayout();
            Pan_Summary.SuspendLayout();
            grb_Summary.SuspendLayout();
            SuspendLayout();
            // 
            // Pan_Contained
            // 
            Pan_Contained.Controls.Add(grb_ContainedType);
            Pan_Contained.Controls.Add(grb_Contained);
            Pan_Contained.Location = new Point(12, 12);
            Pan_Contained.Name = "Pan_Contained";
            Pan_Contained.Size = new Size(160, 170);
            Pan_Contained.TabIndex = 6;
            // 
            // grb_ContainedType
            // 
            grb_ContainedType.Controls.Add(rab_ContainedType_contained);
            grb_ContainedType.Controls.Add(rab_ContainedType_container);
            grb_ContainedType.Location = new Point(3, 86);
            grb_ContainedType.Name = "grb_ContainedType";
            grb_ContainedType.Size = new Size(153, 80);
            grb_ContainedType.TabIndex = 7;
            grb_ContainedType.TabStop = false;
            grb_ContainedType.Text = "Contained Tyep";
            // 
            // rab_ContainedType_contained
            // 
            rab_ContainedType_contained.AutoSize = true;
            rab_ContainedType_contained.Location = new Point(15, 47);
            rab_ContainedType_contained.Name = "rab_ContainedType_contained";
            rab_ContainedType_contained.Size = new Size(82, 19);
            rab_ContainedType_contained.TabIndex = 7;
            rab_ContainedType_contained.Text = "contained";
            rab_ContainedType_contained.UseVisualStyleBackColor = true;
            // 
            // rab_ContainedType_container
            // 
            rab_ContainedType_container.AutoSize = true;
            rab_ContainedType_container.Location = new Point(15, 22);
            rab_ContainedType_container.Name = "rab_ContainedType_container";
            rab_ContainedType_container.Size = new Size(78, 19);
            rab_ContainedType_container.TabIndex = 7;
            rab_ContainedType_container.Text = "container";
            rab_ContainedType_container.UseVisualStyleBackColor = true;
            // 
            // grb_Contained
            // 
            grb_Contained.Controls.Add(rab_Contained_Both);
            grb_Contained.Controls.Add(rab_Contained_True);
            grb_Contained.Controls.Add(rab_Contained_False);
            grb_Contained.Location = new Point(3, 3);
            grb_Contained.Name = "grb_Contained";
            grb_Contained.Size = new Size(153, 77);
            grb_Contained.TabIndex = 0;
            grb_Contained.TabStop = false;
            grb_Contained.Text = "Contained";
            // 
            // rab_Contained_Both
            // 
            rab_Contained_Both.AutoSize = true;
            rab_Contained_Both.Location = new Point(15, 47);
            rab_Contained_Both.Name = "rab_Contained_Both";
            rab_Contained_Both.Size = new Size(52, 19);
            rab_Contained_Both.TabIndex = 7;
            rab_Contained_Both.Text = "both";
            rab_Contained_Both.UseVisualStyleBackColor = true;
            // 
            // rab_Contained_True
            // 
            rab_Contained_True.AutoSize = true;
            rab_Contained_True.Location = new Point(72, 22);
            rab_Contained_True.Name = "rab_Contained_True";
            rab_Contained_True.Size = new Size(47, 19);
            rab_Contained_True.TabIndex = 7;
            rab_Contained_True.Text = "true";
            rab_Contained_True.UseVisualStyleBackColor = true;
            // 
            // rab_Contained_False
            // 
            rab_Contained_False.AutoSize = true;
            rab_Contained_False.Location = new Point(15, 22);
            rab_Contained_False.Name = "rab_Contained_False";
            rab_Contained_False.Size = new Size(51, 19);
            rab_Contained_False.TabIndex = 0;
            rab_Contained_False.Text = "false";
            rab_Contained_False.UseVisualStyleBackColor = true;
            // 
            // pan_Number
            // 
            pan_Number.Controls.Add(groupBox2);
            pan_Number.Controls.Add(grb_Maxresults);
            pan_Number.Controls.Add(groupBox1);
            pan_Number.Location = new Point(12, 188);
            pan_Number.Name = "pan_Number";
            pan_Number.Size = new Size(160, 176);
            pan_Number.TabIndex = 7;
            // 
            // groupBox2
            // 
            groupBox2.Enabled = false;
            groupBox2.Location = new Point(3, 124);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(153, 46);
            groupBox2.TabIndex = 9;
            groupBox2.TabStop = false;
            groupBox2.Text = "Score";
            // 
            // grb_Maxresults
            // 
            grb_Maxresults.Controls.Add(txt_Maxresults_value);
            grb_Maxresults.Location = new Point(3, 64);
            grb_Maxresults.Name = "grb_Maxresults";
            grb_Maxresults.Size = new Size(153, 54);
            grb_Maxresults.TabIndex = 9;
            grb_Maxresults.TabStop = false;
            grb_Maxresults.Text = "Maxresults";
            // 
            // txt_Maxresults_value
            // 
            txt_Maxresults_value.Location = new Point(6, 22);
            txt_Maxresults_value.Name = "txt_Maxresults_value";
            txt_Maxresults_value.Size = new Size(141, 23);
            txt_Maxresults_value.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(txt_Count_value);
            groupBox1.Location = new Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(153, 55);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "Count";
            // 
            // txt_Count_value
            // 
            txt_Count_value.Location = new Point(6, 22);
            txt_Count_value.Name = "txt_Count_value";
            txt_Count_value.Size = new Size(141, 23);
            txt_Count_value.TabIndex = 0;
            // 
            // Pan_Summary
            // 
            Pan_Summary.Controls.Add(grb_Summary);
            Pan_Summary.Location = new Point(12, 370);
            Pan_Summary.Name = "Pan_Summary";
            Pan_Summary.Size = new Size(160, 100);
            Pan_Summary.TabIndex = 8;
            // 
            // grb_Summary
            // 
            grb_Summary.Controls.Add(rab_Summary_count);
            grb_Summary.Controls.Add(rab_Summary_data);
            grb_Summary.Controls.Add(rab_summary_text);
            grb_Summary.Controls.Add(rab_summary_false);
            grb_Summary.Controls.Add(rab_Summary_true);
            grb_Summary.Location = new Point(3, 3);
            grb_Summary.Name = "grb_Summary";
            grb_Summary.Size = new Size(153, 94);
            grb_Summary.TabIndex = 0;
            grb_Summary.TabStop = false;
            grb_Summary.Text = "Summary";
            // 
            // rab_Summary_count
            // 
            rab_Summary_count.AutoSize = true;
            rab_Summary_count.Location = new Point(6, 72);
            rab_Summary_count.Name = "rab_Summary_count";
            rab_Summary_count.Size = new Size(57, 19);
            rab_Summary_count.TabIndex = 9;
            rab_Summary_count.TabStop = true;
            rab_Summary_count.Text = "count";
            rab_Summary_count.UseVisualStyleBackColor = true;
            // 
            // rab_Summary_data
            // 
            rab_Summary_data.AutoSize = true;
            rab_Summary_data.Location = new Point(72, 47);
            rab_Summary_data.Name = "rab_Summary_data";
            rab_Summary_data.Size = new Size(51, 19);
            rab_Summary_data.TabIndex = 9;
            rab_Summary_data.TabStop = true;
            rab_Summary_data.Text = "data";
            rab_Summary_data.UseVisualStyleBackColor = true;
            // 
            // rab_summary_text
            // 
            rab_summary_text.AutoSize = true;
            rab_summary_text.Location = new Point(6, 47);
            rab_summary_text.Name = "rab_summary_text";
            rab_summary_text.Size = new Size(46, 19);
            rab_summary_text.TabIndex = 9;
            rab_summary_text.TabStop = true;
            rab_summary_text.Text = "text";
            rab_summary_text.UseVisualStyleBackColor = true;
            // 
            // rab_summary_false
            // 
            rab_summary_false.AutoSize = true;
            rab_summary_false.Location = new Point(72, 22);
            rab_summary_false.Name = "rab_summary_false";
            rab_summary_false.Size = new Size(51, 19);
            rab_summary_false.TabIndex = 9;
            rab_summary_false.TabStop = true;
            rab_summary_false.Text = "false";
            rab_summary_false.UseVisualStyleBackColor = true;
            // 
            // rab_Summary_true
            // 
            rab_Summary_true.AutoSize = true;
            rab_Summary_true.Location = new Point(6, 22);
            rab_Summary_true.Name = "rab_Summary_true";
            rab_Summary_true.Size = new Size(47, 19);
            rab_Summary_true.TabIndex = 9;
            rab_Summary_true.TabStop = true;
            rab_Summary_true.Text = "true";
            rab_Summary_true.UseVisualStyleBackColor = true;
            // 
            // cob_ResourceElements
            // 
            cob_ResourceElements.FormattingEnabled = true;
            cob_ResourceElements.Location = new Point(178, 12);
            cob_ResourceElements.Name = "cob_ResourceElements";
            cob_ResourceElements.Size = new Size(221, 23);
            cob_ResourceElements.TabIndex = 9;
            // 
            // lib_Elements
            // 
            lib_Elements.FormattingEnabled = true;
            lib_Elements.ItemHeight = 15;
            lib_Elements.Location = new Point(178, 301);
            lib_Elements.Name = "lib_Elements";
            lib_Elements.Size = new Size(221, 139);
            lib_Elements.TabIndex = 10;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(178, 283);
            label1.Name = "label1";
            label1.Size = new Size(58, 15);
            label1.TabIndex = 11;
            label1.Text = "Elements";
            // 
            // lib_Sort
            // 
            lib_Sort.FormattingEnabled = true;
            lib_Sort.ItemHeight = 15;
            lib_Sort.Location = new Point(178, 111);
            lib_Sort.Name = "lib_Sort";
            lib_Sort.Size = new Size(221, 139);
            lib_Sort.TabIndex = 12;
            // 
            // Btn_Cancel
            // 
            Btn_Cancel.Location = new Point(97, 476);
            Btn_Cancel.Name = "Btn_Cancel";
            Btn_Cancel.Size = new Size(75, 23);
            Btn_Cancel.TabIndex = 13;
            Btn_Cancel.Text = "Reset";
            Btn_Cancel.UseVisualStyleBackColor = true;
            Btn_Cancel.Click += Btn_Cancel_Click;
            // 
            // Btn_OK
            // 
            Btn_OK.Location = new Point(324, 476);
            Btn_OK.Name = "Btn_OK";
            Btn_OK.Size = new Size(75, 23);
            Btn_OK.TabIndex = 14;
            Btn_OK.Text = "Done";
            Btn_OK.UseVisualStyleBackColor = true;
            Btn_OK.Click += Btn_OK_Click;
            // 
            // Btn_AddToElements
            // 
            Btn_AddToElements.Location = new Point(259, 69);
            Btn_AddToElements.Name = "Btn_AddToElements";
            Btn_AddToElements.Size = new Size(140, 23);
            Btn_AddToElements.TabIndex = 15;
            Btn_AddToElements.Text = "Add to Elements";
            Btn_AddToElements.UseVisualStyleBackColor = true;
            Btn_AddToElements.Click += Btn_AddToElements_Click;
            // 
            // rab_PrefixForSort
            // 
            rab_PrefixForSort.AutoSize = true;
            rab_PrefixForSort.Location = new Point(200, 41);
            rab_PrefixForSort.Name = "rab_PrefixForSort";
            rab_PrefixForSort.Size = new Size(36, 19);
            rab_PrefixForSort.TabIndex = 16;
            rab_PrefixForSort.TabStop = true;
            rab_PrefixForSort.Text = " - ";
            rab_PrefixForSort.UseVisualStyleBackColor = true;
            // 
            // Btn_AddToSort
            // 
            Btn_AddToSort.Location = new Point(259, 41);
            Btn_AddToSort.Name = "Btn_AddToSort";
            Btn_AddToSort.Size = new Size(140, 23);
            Btn_AddToSort.TabIndex = 17;
            Btn_AddToSort.Text = "Add to Sort";
            Btn_AddToSort.UseVisualStyleBackColor = true;
            Btn_AddToSort.Click += Btn_AddToSort_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(178, 93);
            label2.Name = "label2";
            label2.Size = new Size(30, 15);
            label2.TabIndex = 18;
            label2.Text = "Sort";
            // 
            // Btn_ClearSort
            // 
            Btn_ClearSort.Location = new Point(178, 256);
            Btn_ClearSort.Name = "Btn_ClearSort";
            Btn_ClearSort.Size = new Size(75, 23);
            Btn_ClearSort.TabIndex = 19;
            Btn_ClearSort.Text = "Clear";
            Btn_ClearSort.UseVisualStyleBackColor = true;
            Btn_ClearSort.Click += Btn_ClearSort_Click;
            // 
            // Btn_ClearElement
            // 
            Btn_ClearElement.Location = new Point(178, 447);
            Btn_ClearElement.Name = "Btn_ClearElement";
            Btn_ClearElement.Size = new Size(75, 23);
            Btn_ClearElement.TabIndex = 20;
            Btn_ClearElement.Text = "Clear";
            Btn_ClearElement.UseVisualStyleBackColor = true;
            Btn_ClearElement.Click += Btn_ClearElement_Click;
            // 
            // ModifyResutl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(415, 509);
            Controls.Add(Btn_ClearElement);
            Controls.Add(Btn_ClearSort);
            Controls.Add(label2);
            Controls.Add(Btn_AddToSort);
            Controls.Add(rab_PrefixForSort);
            Controls.Add(Btn_AddToElements);
            Controls.Add(Btn_OK);
            Controls.Add(Btn_Cancel);
            Controls.Add(lib_Sort);
            Controls.Add(label1);
            Controls.Add(lib_Elements);
            Controls.Add(cob_ResourceElements);
            Controls.Add(Pan_Summary);
            Controls.Add(pan_Number);
            Controls.Add(Pan_Contained);
            Name = "ModifyResutl";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Modifying Search Results";
            Pan_Contained.ResumeLayout(false);
            grb_ContainedType.ResumeLayout(false);
            grb_ContainedType.PerformLayout();
            grb_Contained.ResumeLayout(false);
            grb_Contained.PerformLayout();
            pan_Number.ResumeLayout(false);
            grb_Maxresults.ResumeLayout(false);
            grb_Maxresults.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            Pan_Summary.ResumeLayout(false);
            grb_Summary.ResumeLayout(false);
            grb_Summary.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Panel Pan_Contained;
        private RadioButton rab_Contained_False;
        private GroupBox grb_ContainedType;
        private GroupBox grb_Contained;
        private RadioButton rab_Contained_Both;
        private RadioButton rab_Contained_True;
        private RadioButton rab_ContainedType_container;
        private RadioButton rab_ContainedType_contained;
        private Panel pan_Number;
        private GroupBox groupBox1;
        private TextBox txt_Count_value;
        private GroupBox grb_Maxresults;
        private TextBox txt_Maxresults_value;
        private GroupBox groupBox2;
        private Panel Pan_Summary;
        private GroupBox grb_Summary;
        private RadioButton rab_summary_false;
        private RadioButton rab_Summary_true;
        private RadioButton rab_summary_text;
        private RadioButton rab_Summary_count;
        private RadioButton rab_Summary_data;
        private ComboBox cob_ResourceElements;
        private ListBox lib_Elements;
        private Label label1;
        private ListBox lib_Sort;
        private Button Btn_Cancel;
        private Button Btn_OK;
        private Button Btn_AddToElements;
        private RadioButton rab_PrefixForSort;
        private Button Btn_AddToSort;
        private Label label2;
        private Button Btn_ClearSort;
        private Button Btn_ClearElement;
    }
}