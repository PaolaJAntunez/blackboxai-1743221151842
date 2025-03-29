using System;
using System.Windows.Forms;

namespace Facturacion
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Show login form
            using (var login = new frmLogin())
            {
                if (login.ShowDialog() == DialogResult.OK && login.IsAuthenticated)
                {
                    try
                    {
                        // Initialize main form with authenticated user
                        var mainForm = new frmMain_NEW();
                        mainForm.Username = login.Username;
                        Application.Run(mainForm);
                    }
                    catch (Exception ex)
                    {
                        Utils.ShowError($"Error inicializando la aplicación: {ex.Message}");
                    }
                }
            }
        }
    }
}