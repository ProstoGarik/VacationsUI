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
            AddRowButton = new Button();
            FilterButton = new Button();
            FilterStatusLabel = new Label();
            ResetFilterButton = new Button();
            SortStatusLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)tableDataGridView).BeginInit();
            SuspendLayout();
            // 
            // tableDataGridView
            // 
            tableDataGridView.BackgroundColor = SystemColors.Control;
            tableDataGridView.BorderStyle = BorderStyle.None;
            tableDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            tableDataGridView.Location = new Point(-3, 46);
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
            // AddRowButton
            // 
            AddRowButton.Location = new Point(12, 389);
            AddRowButton.Name = "AddRowButton";
            AddRowButton.Size = new Size(204, 92);
            AddRowButton.TabIndex = 2;
            AddRowButton.Text = "Добавить данные";
            AddRowButton.UseVisualStyleBackColor = true;
            AddRowButton.Click += AddRowButton_Click;
            // 
            // FilterButton
            // 
            FilterButton.Location = new Point(245, 389);
            FilterButton.Name = "FilterButton";
            FilterButton.Size = new Size(162, 48);
            FilterButton.TabIndex = 3;
            FilterButton.Text = "Фильтр";
            FilterButton.UseVisualStyleBackColor = true;
            FilterButton.Click += FilterButton_Click;
            // 
            // FilterStatusLabel
            // 
            FilterStatusLabel.AutoSize = true;
            FilterStatusLabel.Location = new Point(413, 389);
            FilterStatusLabel.Name = "FilterStatusLabel";
            FilterStatusLabel.Size = new Size(146, 20);
            FilterStatusLabel.TabIndex = 4;
            FilterStatusLabel.Text = "Фильтр: Отсутствует";
            // 
            // ResetFilterButton
            // 
            ResetFilterButton.Location = new Point(245, 443);
            ResetFilterButton.Name = "ResetFilterButton";
            ResetFilterButton.Size = new Size(162, 38);
            ResetFilterButton.TabIndex = 5;
            ResetFilterButton.Text = "Сбросить";
            ResetFilterButton.UseVisualStyleBackColor = true;
            ResetFilterButton.Click += ResetFilterButton_Click;
            // 
            // SortStatusLabel
            // 
            SortStatusLabel.AutoSize = true;
            SortStatusLabel.Location = new Point(413, 413);
            SortStatusLabel.Name = "SortStatusLabel";
            SortStatusLabel.Size = new Size(178, 20);
            SortStatusLabel.TabIndex = 6;
            SortStatusLabel.Text = "Сортировка: Отсутствует";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(942, 493);
            Controls.Add(SortStatusLabel);
            Controls.Add(ResetFilterButton);
            Controls.Add(FilterStatusLabel);
            Controls.Add(FilterButton);
            Controls.Add(AddRowButton);
            Controls.Add(TableSelectComboBox);
            Controls.Add(tableDataGridView);
            Name = "MainForm";
            Text = "Отпуска рабочих";
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)tableDataGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView tableDataGridView;
        private ComboBox TableSelectComboBox;
        private Button AddRowButton;
        private Button FilterButton;
        private Label FilterStatusLabel;
        private Button ResetFilterButton;
        private Label SortStatusLabel;
    }
}
