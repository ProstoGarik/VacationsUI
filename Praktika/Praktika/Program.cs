namespace Praktika
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            while (true)
            {
                using LoginForm loginForm = new LoginForm();
                if (loginForm.ShowDialog() != DialogResult.OK || loginForm.CurrentUser == null)
                    break;

                using MainForm mainForm = new MainForm(loginForm.CurrentUser);
                Application.Run(mainForm);

                if (!mainForm.LogoutRequested)
                    break;
            }
        }
    }
}
