namespace Praktika
{
    partial class LoginForm
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
            LoginTextBox = new TextBox();
            PasswordTextBox = new TextBox();
            LoginLabel = new Label();
            PasswordLabel = new Label();
            LoginButton = new Button();
            SuspendLayout();
            // 
            // LoginTextBox
            // 
            LoginTextBox.Location = new Point(212, 129);
            LoginTextBox.Name = "LoginTextBox";
            LoginTextBox.Size = new Size(414, 27);
            LoginTextBox.TabIndex = 0;
            // 
            // PasswordTextBox
            // 
            PasswordTextBox.Location = new Point(212, 226);
            PasswordTextBox.Name = "PasswordTextBox";
            PasswordTextBox.Size = new Size(414, 27);
            PasswordTextBox.TabIndex = 1;
            // 
            // LoginLabel
            // 
            LoginLabel.AutoSize = true;
            LoginLabel.Location = new Point(56, 129);
            LoginLabel.Name = "LoginLabel";
            LoginLabel.Size = new Size(139, 20);
            LoginLabel.TabIndex = 2;
            LoginLabel.Text = "Имя пользователя";
            // 
            // PasswordLabel
            // 
            PasswordLabel.AutoSize = true;
            PasswordLabel.Location = new Point(56, 226);
            PasswordLabel.Name = "PasswordLabel";
            PasswordLabel.Size = new Size(62, 20);
            PasswordLabel.TabIndex = 3;
            PasswordLabel.Text = "Пароль";
            // 
            // LoginButton
            // 
            LoginButton.Location = new Point(333, 311);
            LoginButton.Name = "LoginButton";
            LoginButton.Size = new Size(166, 52);
            LoginButton.TabIndex = 4;
            LoginButton.Text = "Войти";
            LoginButton.UseVisualStyleBackColor = true;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(LoginButton);
            Controls.Add(PasswordLabel);
            Controls.Add(LoginLabel);
            Controls.Add(PasswordTextBox);
            Controls.Add(LoginTextBox);
            Name = "LoginForm";
            Text = "LoginForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox LoginTextBox;
        private TextBox PasswordTextBox;
        private Label LoginLabel;
        private Label PasswordLabel;
        private Button LoginButton;
    }
}