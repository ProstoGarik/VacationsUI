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
            ((System.ComponentModel.ISupportInitialize)tableDataGridView).BeginInit();
            SuspendLayout();
            // 
            // tableDataGridView
            // 
            tableDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            tableDataGridView.Location = new Point(12, 12);
            tableDataGridView.Name = "tableDataGridView";
            tableDataGridView.RowHeadersWidth = 51;
            tableDataGridView.Size = new Size(813, 337);
            tableDataGridView.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(837, 479);
            Controls.Add(tableDataGridView);
            Name = "MainForm";
            Text = "Отпуска рабочих";
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)tableDataGridView).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView tableDataGridView;
    }
}
