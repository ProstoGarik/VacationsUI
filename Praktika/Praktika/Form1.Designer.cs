namespace Praktika
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tableDataGridView = new DataGridView();
            TableSelectComboBox = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)tableDataGridView).BeginInit();
            SuspendLayout();
            // 
            // tableDataGridView
            // 
            tableDataGridView.BackgroundColor = SystemColors.Control;
            tableDataGridView.BorderStyle = BorderStyle.None;
            tableDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            tableDataGridView.Location = new Point(12, 48);
            tableDataGridView.Name = "tableDataGridView";
            tableDataGridView.RowHeadersWidth = 51;
            tableDataGridView.ScrollBars = ScrollBars.Vertical;
            tableDataGridView.Size = new Size(918, 337);
            tableDataGridView.TabIndex = 0;
            tableDataGridView.CellClick += TableDataGridView_CellClick;
            // 
            // TableSelectComboBox
            // 
            TableSelectComboBox.FormattingEnabled = true;
            TableSelectComboBox.Location = new Point(12, 12);
            TableSelectComboBox.Name = "TableSelectComboBox";
            TableSelectComboBox.Size = new Size(266, 28);
            TableSelectComboBox.TabIndex = 1;
            TableSelectComboBox.SelectedIndexChanged += TableSelectComboBox_SelectedIndexChanged;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(942, 493);
            Controls.Add(TableSelectComboBox);
            Controls.Add(tableDataGridView);
            Name = "MainForm";
            Text = "Отпуска рабочих";
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)tableDataGridView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView tableDataGridView;
        private ComboBox TableSelectComboBox;
    }
}
