namespace Praktika
{
    partial class SortFilterForm
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
            FilteredColumnComboBox = new ComboBox();
            FilterOperationComboBox = new ComboBox();
            FilterValueTextBox = new TextBox();
            ApplyFilterButton = new Button();
            CancelButton = new Button();
            FilterValueTextBox2 = new TextBox();
            FilterOperationComboBox2 = new ComboBox();
            FilteredColumnComboBox2 = new ComboBox();
            FilterValueTextBox3 = new TextBox();
            FilterOperationComboBox3 = new ComboBox();
            FilteredColumnComboBox3 = new ComboBox();
            SortedColumnComboBox = new ComboBox();
            SortingMethodComboBox = new ComboBox();
            SuspendLayout();
            // 
            // FilteredColumnComboBox
            // 
            FilteredColumnComboBox.FormattingEnabled = true;
            FilteredColumnComboBox.Location = new Point(12, 33);
            FilteredColumnComboBox.Name = "FilteredColumnComboBox";
            FilteredColumnComboBox.Size = new Size(237, 28);
            FilteredColumnComboBox.TabIndex = 0;
            // 
            // FilterOperationComboBox
            // 
            FilterOperationComboBox.FormattingEnabled = true;
            FilterOperationComboBox.Location = new Point(272, 34);
            FilterOperationComboBox.Name = "FilterOperationComboBox";
            FilterOperationComboBox.Size = new Size(221, 28);
            FilterOperationComboBox.TabIndex = 1;
            // 
            // FilterValueTextBox
            // 
            FilterValueTextBox.Location = new Point(523, 34);
            FilterValueTextBox.Name = "FilterValueTextBox";
            FilterValueTextBox.Size = new Size(250, 27);
            FilterValueTextBox.TabIndex = 2;
            // 
            // ApplyFilterButton
            // 
            ApplyFilterButton.Location = new Point(12, 378);
            ApplyFilterButton.Name = "ApplyFilterButton";
            ApplyFilterButton.Size = new Size(192, 60);
            ApplyFilterButton.TabIndex = 3;
            ApplyFilterButton.Text = "Применить";
            ApplyFilterButton.UseVisualStyleBackColor = true;
            // 
            // CancelButton
            // 
            CancelButton.Location = new Point(242, 378);
            CancelButton.Name = "CancelButton";
            CancelButton.Size = new Size(192, 60);
            CancelButton.TabIndex = 4;
            CancelButton.Text = "Отмена";
            CancelButton.UseVisualStyleBackColor = true;
            // 
            // FilterValueTextBox2
            // 
            FilterValueTextBox2.Location = new Point(523, 89);
            FilterValueTextBox2.Name = "FilterValueTextBox2";
            FilterValueTextBox2.Size = new Size(250, 27);
            FilterValueTextBox2.TabIndex = 7;
            // 
            // FilterOperationComboBox2
            // 
            FilterOperationComboBox2.FormattingEnabled = true;
            FilterOperationComboBox2.Location = new Point(272, 89);
            FilterOperationComboBox2.Name = "FilterOperationComboBox2";
            FilterOperationComboBox2.Size = new Size(221, 28);
            FilterOperationComboBox2.TabIndex = 6;
            // 
            // FilteredColumnComboBox2
            // 
            FilteredColumnComboBox2.FormattingEnabled = true;
            FilteredColumnComboBox2.Location = new Point(12, 88);
            FilteredColumnComboBox2.Name = "FilteredColumnComboBox2";
            FilteredColumnComboBox2.Size = new Size(237, 28);
            FilteredColumnComboBox2.TabIndex = 5;
            // 
            // FilterValueTextBox3
            // 
            FilterValueTextBox3.Location = new Point(523, 140);
            FilterValueTextBox3.Name = "FilterValueTextBox3";
            FilterValueTextBox3.Size = new Size(250, 27);
            FilterValueTextBox3.TabIndex = 10;
            // 
            // FilterOperationComboBox3
            // 
            FilterOperationComboBox3.FormattingEnabled = true;
            FilterOperationComboBox3.Location = new Point(272, 140);
            FilterOperationComboBox3.Name = "FilterOperationComboBox3";
            FilterOperationComboBox3.Size = new Size(221, 28);
            FilterOperationComboBox3.TabIndex = 9;
            // 
            // FilteredColumnComboBox3
            // 
            FilteredColumnComboBox3.FormattingEnabled = true;
            FilteredColumnComboBox3.Location = new Point(12, 139);
            FilteredColumnComboBox3.Name = "FilteredColumnComboBox3";
            FilteredColumnComboBox3.Size = new Size(237, 28);
            FilteredColumnComboBox3.TabIndex = 8;
            // 
            // SortedColumnComboBox
            // 
            SortedColumnComboBox.FormattingEnabled = true;
            SortedColumnComboBox.Location = new Point(12, 226);
            SortedColumnComboBox.Name = "SortedColumnComboBox";
            SortedColumnComboBox.Size = new Size(362, 28);
            SortedColumnComboBox.TabIndex = 11;
            // 
            // SortingMethodComboBox
            // 
            SortingMethodComboBox.FormattingEnabled = true;
            SortingMethodComboBox.Location = new Point(397, 226);
            SortingMethodComboBox.Name = "SortingMethodComboBox";
            SortingMethodComboBox.Size = new Size(376, 28);
            SortingMethodComboBox.TabIndex = 12;
            // 
            // SortFilterForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(SortingMethodComboBox);
            Controls.Add(SortedColumnComboBox);
            Controls.Add(FilterValueTextBox3);
            Controls.Add(FilterOperationComboBox3);
            Controls.Add(FilteredColumnComboBox3);
            Controls.Add(FilterValueTextBox2);
            Controls.Add(FilterOperationComboBox2);
            Controls.Add(FilteredColumnComboBox2);
            Controls.Add(CancelButton);
            Controls.Add(ApplyFilterButton);
            Controls.Add(FilterValueTextBox);
            Controls.Add(FilterOperationComboBox);
            Controls.Add(FilteredColumnComboBox);
            Name = "SortFilterForm";
            Text = "FilterForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox FilteredColumnComboBox;
        private ComboBox FilterOperationComboBox;
        private TextBox FilterValueTextBox;
        private Button ApplyFilterButton;
        private Button CancelButton;
        private TextBox FilterValueTextBox2;
        private ComboBox FilterOperationComboBox2;
        private ComboBox FilteredColumnComboBox2;
        private TextBox FilterValueTextBox3;
        private ComboBox FilterOperationComboBox3;
        private ComboBox FilteredColumnComboBox3;
        private ComboBox SortedColumnComboBox;
        private ComboBox SortingMethodComboBox;
    }
}