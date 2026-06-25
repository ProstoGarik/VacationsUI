namespace Praktika
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            using (LoginForm loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() != DialogResult.OK || loginForm.CurrentUser == null)
                    return;

                Application.Run(new MainForm(loginForm.CurrentUser));
            }
        }
    }
}
