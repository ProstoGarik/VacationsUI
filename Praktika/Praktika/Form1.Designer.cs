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
            tabControl1 = new TabControl();
            tablesTabPage = new TabPage();
            tableDataGridView = new DataGridView();
            TableSelectComboBox = new ComboBox();
            AddRowButton = new Button();
            FilterButton = new Button();
            FilterStatusLabel = new Label();
            ResetFilterButton = new Button();
            SortStatusLabel = new Label();
            quieryTabPage = new TabPage();
            quieryParamComboBox = new ComboBox();
            quieryTableDataGridView = new DataGridView();
            quierySelectComboBox = new ComboBox();
            quieryFilterButton = new Button();
            quieryFilterStatusLabel = new Label();
            quieryResetFilterButton = new Button();
            quierySortStatusLabel = new Label();
            ControlTabPage = new TabPage();
            AccountDataLabel = new Label();
            ConnectionStatusLabel = new Label();
            ConnectionDynamicButton = new Button();
            LogoutButton = new Button();
            CreateReportButton = new Button();
            tabControl1.SuspendLayout();
            tablesTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tableDataGridView).BeginInit();
            quieryTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)quieryTableDataGridView).BeginInit();
            ControlTabPage.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tablesTabPage);
            tabControl1.Controls.Add(quieryTabPage);
            tabControl1.Controls.Add(ControlTabPage);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(942, 511);
            tabControl1.TabIndex = 0;
            // 
            // tablesTabPage
            // 
            tablesTabPage.Controls.Add(tableDataGridView);
            tablesTabPage.Controls.Add(TableSelectComboBox);
            tablesTabPage.Controls.Add(AddRowButton);
            tablesTabPage.Controls.Add(FilterButton);
            tablesTabPage.Controls.Add(FilterStatusLabel);
            tablesTabPage.Controls.Add(ResetFilterButton);
            tablesTabPage.Controls.Add(SortStatusLabel);
            tablesTabPage.Location = new Point(4, 29);
            tablesTabPage.Name = "tablesTabPage";
            tablesTabPage.Size = new Size(934, 478);
            tablesTabPage.TabIndex = 0;
            tablesTabPage.Text = "Таблицы";
            tablesTabPage.UseVisualStyleBackColor = true;
            // 
            // tableDataGridView
            // 
            tableDataGridView.BackgroundColor = SystemColors.Control;
            tableDataGridView.BorderStyle = BorderStyle.None;
            tableDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            tableDataGridView.Location = new Point(10, 54);
            tableDataGridView.Name = "tableDataGridView";
            tableDataGridView.RowHeadersWidth = 51;
            tableDataGridView.ScrollBars = ScrollBars.Vertical;
            tableDataGridView.Size = new Size(914, 310);
            tableDataGridView.TabIndex = 7;
            tableDataGridView.CellClick += TableDataGridView_CellClick;
            // 
            // TableSelectComboBox
            // 
            TableSelectComboBox.FormattingEnabled = true;
            TableSelectComboBox.Location = new Point(10, 17);
            TableSelectComboBox.Name = "TableSelectComboBox";
            TableSelectComboBox.Size = new Size(266, 28);
            TableSelectComboBox.TabIndex = 8;
            TableSelectComboBox.SelectedIndexChanged += TableSelectComboBox_SelectedIndexChanged;
            // 
            // AddRowButton
            // 
            AddRowButton.Location = new Point(10, 370);
            AddRowButton.Name = "AddRowButton";
            AddRowButton.Size = new Size(204, 92);
            AddRowButton.TabIndex = 9;
            AddRowButton.Text = "Добавить данные";
            AddRowButton.UseVisualStyleBackColor = true;
            AddRowButton.Click += AddRowButton_Click;
            // 
            // FilterButton
            // 
            FilterButton.Location = new Point(243, 370);
            FilterButton.Name = "FilterButton";
            FilterButton.Size = new Size(162, 48);
            FilterButton.TabIndex = 10;
            FilterButton.Text = "Фильтр и сортировка";
            FilterButton.UseVisualStyleBackColor = true;
            FilterButton.Click += FilterButton_Click;
            // 
            // FilterStatusLabel
            // 
            FilterStatusLabel.AutoSize = true;
            FilterStatusLabel.Location = new Point(411, 370);
            FilterStatusLabel.Name = "FilterStatusLabel";
            FilterStatusLabel.Size = new Size(146, 20);
            FilterStatusLabel.TabIndex = 11;
            FilterStatusLabel.Text = "Фильтр: Отсутствует";
            // 
            // ResetFilterButton
            // 
            ResetFilterButton.Location = new Point(243, 424);
            ResetFilterButton.Name = "ResetFilterButton";
            ResetFilterButton.Size = new Size(162, 38);
            ResetFilterButton.TabIndex = 12;
            ResetFilterButton.Text = "Сбросить";
            ResetFilterButton.UseVisualStyleBackColor = true;
            ResetFilterButton.Click += ResetFilterButton_Click;
            // 
            // SortStatusLabel
            // 
            SortStatusLabel.AutoSize = true;
            SortStatusLabel.Location = new Point(411, 394);
            SortStatusLabel.Name = "SortStatusLabel";
            SortStatusLabel.Size = new Size(178, 20);
            SortStatusLabel.TabIndex = 13;
            SortStatusLabel.Text = "Сортировка: Отсутствует";
            // 
            // quieryTabPage
            // 
            quieryTabPage.Controls.Add(CreateReportButton);
            quieryTabPage.Controls.Add(quieryParamComboBox);
            quieryTabPage.Controls.Add(quieryTableDataGridView);
            quieryTabPage.Controls.Add(quierySelectComboBox);
            quieryTabPage.Controls.Add(quieryFilterButton);
            quieryTabPage.Controls.Add(quieryFilterStatusLabel);
            quieryTabPage.Controls.Add(quieryResetFilterButton);
            quieryTabPage.Controls.Add(quierySortStatusLabel);
            quieryTabPage.Location = new Point(4, 29);
            quieryTabPage.Name = "quieryTabPage";
            quieryTabPage.Padding = new Padding(3);
            quieryTabPage.Size = new Size(934, 478);
            quieryTabPage.TabIndex = 1;
            quieryTabPage.Text = "Запросы";
            quieryTabPage.UseVisualStyleBackColor = true;
            // 
            // quieryParamComboBox
            // 
            quieryParamComboBox.FormattingEnabled = true;
            quieryParamComboBox.Location = new Point(323, 17);
            quieryParamComboBox.Name = "quieryParamComboBox";
            quieryParamComboBox.Size = new Size(266, 28);
            quieryParamComboBox.TabIndex = 21;
            quieryParamComboBox.SelectedIndexChanged += quieryParamComboBox_SelectedIndexChanged;
            // 
            // quieryTableDataGridView
            // 
            quieryTableDataGridView.BackgroundColor = SystemColors.Control;
            quieryTableDataGridView.BorderStyle = BorderStyle.None;
            quieryTableDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            quieryTableDataGridView.Location = new Point(10, 54);
            quieryTableDataGridView.Name = "quieryTableDataGridView";
            quieryTableDataGridView.RowHeadersWidth = 51;
            quieryTableDataGridView.ScrollBars = ScrollBars.Vertical;
            quieryTableDataGridView.Size = new Size(914, 310);
            quieryTableDataGridView.TabIndex = 14;
            quieryTableDataGridView.CellClick += TableDataGridView_CellClick;
            // 
            // quierySelectComboBox
            // 
            quierySelectComboBox.FormattingEnabled = true;
            quierySelectComboBox.Location = new Point(10, 17);
            quierySelectComboBox.Name = "quierySelectComboBox";
            quierySelectComboBox.Size = new Size(266, 28);
            quierySelectComboBox.TabIndex = 15;
            quierySelectComboBox.SelectedIndexChanged += quierySelectComboBox_SelectedIndexChanged;
            // 
            // quieryFilterButton
            // 
            quieryFilterButton.Location = new Point(243, 370);
            quieryFilterButton.Name = "quieryFilterButton";
            quieryFilterButton.Size = new Size(162, 48);
            quieryFilterButton.TabIndex = 17;
            quieryFilterButton.Text = "Фильтр и сортировка";
            quieryFilterButton.UseVisualStyleBackColor = true;
            quieryFilterButton.Click += FilterButton_Click;
            // 
            // quieryFilterStatusLabel
            // 
            quieryFilterStatusLabel.AutoSize = true;
            quieryFilterStatusLabel.Location = new Point(411, 370);
            quieryFilterStatusLabel.Name = "quieryFilterStatusLabel";
            quieryFilterStatusLabel.Size = new Size(146, 20);
            quieryFilterStatusLabel.TabIndex = 18;
            quieryFilterStatusLabel.Text = "Фильтр: Отсутствует";
            // 
            // quieryResetFilterButton
            // 
            quieryResetFilterButton.Location = new Point(243, 424);
            quieryResetFilterButton.Name = "quieryResetFilterButton";
            quieryResetFilterButton.Size = new Size(162, 38);
            quieryResetFilterButton.TabIndex = 19;
            quieryResetFilterButton.Text = "Сбросить";
            quieryResetFilterButton.UseVisualStyleBackColor = true;
            quieryResetFilterButton.Click += ResetFilterButton_Click;
            // 
            // quierySortStatusLabel
            // 
            quierySortStatusLabel.AutoSize = true;
            quierySortStatusLabel.Location = new Point(411, 394);
            quierySortStatusLabel.Name = "quierySortStatusLabel";
            quierySortStatusLabel.Size = new Size(178, 20);
            quierySortStatusLabel.TabIndex = 20;
            quierySortStatusLabel.Text = "Сортировка: Отсутствует";
            // 
            // ControlTabPage
            // 
            ControlTabPage.Controls.Add(AccountDataLabel);
            ControlTabPage.Controls.Add(ConnectionStatusLabel);
            ControlTabPage.Controls.Add(ConnectionDynamicButton);
            ControlTabPage.Controls.Add(LogoutButton);
            ControlTabPage.Location = new Point(4, 29);
            ControlTabPage.Name = "ControlTabPage";
            ControlTabPage.Padding = new Padding(3);
            ControlTabPage.Size = new Size(934, 478);
            ControlTabPage.TabIndex = 2;
            ControlTabPage.Text = "Управление";
            ControlTabPage.UseVisualStyleBackColor = true;
            // 
            // AccountDataLabel
            // 
            AccountDataLabel.AutoSize = true;
            AccountDataLabel.Location = new Point(342, 314);
            AccountDataLabel.Name = "AccountDataLabel";
            AccountDataLabel.Size = new Size(150, 20);
            AccountDataLabel.TabIndex = 3;
            AccountDataLabel.Text = "Выполнен вход как: ";
            // 
            // ConnectionStatusLabel
            // 
            ConnectionStatusLabel.AutoSize = true;
            ConnectionStatusLabel.Location = new Point(342, 139);
            ConnectionStatusLabel.Name = "ConnectionStatusLabel";
            ConnectionStatusLabel.Size = new Size(248, 20);
            ConnectionStatusLabel.TabIndex = 2;
            ConnectionStatusLabel.Text = "Подключение к бд: Не выполнено";
            // 
            // ConnectionDynamicButton
            // 
            ConnectionDynamicButton.Location = new Point(342, 184);
            ConnectionDynamicButton.Name = "ConnectionDynamicButton";
            ConnectionDynamicButton.Size = new Size(226, 78);
            ConnectionDynamicButton.TabIndex = 1;
            ConnectionDynamicButton.Text = "Выбрать файл";
            ConnectionDynamicButton.UseVisualStyleBackColor = true;
            ConnectionDynamicButton.Click += ConnectionDynamicButton_Click;
            // 
            // LogoutButton
            // 
            LogoutButton.Location = new Point(342, 354);
            LogoutButton.Name = "LogoutButton";
            LogoutButton.Size = new Size(226, 74);
            LogoutButton.TabIndex = 0;
            LogoutButton.Text = "Выйти";
            LogoutButton.UseVisualStyleBackColor = true;
            LogoutButton.Click += LogoutButton_Click;
            // 
            // CreateReportButton
            // 
            CreateReportButton.Location = new Point(10, 370);
            CreateReportButton.Name = "CreateReportButton";
            CreateReportButton.Size = new Size(204, 92);
            CreateReportButton.TabIndex = 22;
            CreateReportButton.Text = "Сгенерировать отчёт";
            CreateReportButton.UseVisualStyleBackColor = true;
            CreateReportButton.Click += CreateReportButton_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(942, 511);
            Controls.Add(tabControl1);
            Name = "MainForm";
            Text = "Отпуска рабочих";
            Load += MainForm_Load;
            tabControl1.ResumeLayout(false);
            tablesTabPage.ResumeLayout(false);
            tablesTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tableDataGridView).EndInit();
            quieryTabPage.ResumeLayout(false);
            quieryTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)quieryTableDataGridView).EndInit();
            ControlTabPage.ResumeLayout(false);
            ControlTabPage.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private TabControl tabControl1;
        private TabPage tablesTabPage;
        private DataGridView tableDataGridView;
        private ComboBox TableSelectComboBox;
        private Button AddRowButton;
        private Button FilterButton;
        private Label FilterStatusLabel;
        private Button ResetFilterButton;
        private Label SortStatusLabel;
        private TabPage quieryTabPage;
        private DataGridView quieryTableDataGridView;
        private ComboBox quierySelectComboBox;
        private Button quieryFilterButton;
        private Label quieryFilterStatusLabel;
        private Button quieryResetFilterButton;
        private Label quierySortStatusLabel;
        private ComboBox quieryParamComboBox;
        private TabPage ControlTabPage;
        private Button ConnectionDynamicButton;
        private Button LogoutButton;
        private Label AccountDataLabel;
        private Label ConnectionStatusLabel;
        private Button CreateReportButton;
    }
}
