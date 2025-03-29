using System;
using System.Drawing;
using System.Windows.Forms;

namespace Facturacion
{
    public partial class frmLogin : Form
    {
        public bool IsAuthenticated { get; private set; }
        public string Username { get; private set; }

        public frmLogin()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            this.Text = "Inicio de Sesión";
            this.Size = new Size(350, 250);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Main Table Layout
            var tableLayout = new TableLayoutPanel();
            tableLayout.Dock = DockStyle.Fill;
            tableLayout.ColumnCount = 1;
            tableLayout.RowCount = 4;
            tableLayout.Padding = new Padding(20);

            // Logo Panel
            var logoPanel = new Panel();
            logoPanel.Dock = DockStyle.Fill;
            logoPanel.Height = 60;

            try
            {
                string logoPath = ConfigurationManager.AppSettings["InvoiceLogoPath"];
                if (File.Exists(logoPath))
                {
                    var logo = new PictureBox();
                    logo.Image = Image.FromFile(logoPath);
                    logo.SizeMode = PictureBoxSizeMode.Zoom;
                    logo.Dock = DockStyle.Fill;
                    logoPanel.Controls.Add(logo);
                }
            }
            catch { /* Ignore logo errors */ }

            // Login Form
            var loginPanel = new TableLayoutPanel();
            loginPanel.Dock = DockStyle.Fill;
            loginPanel.ColumnCount = 2;
            loginPanel.RowCount = 2;
            loginPanel.Padding = new Padding(10);

            // Username
            loginPanel.Controls.Add(new Label { Text = "Usuario:", TextAlign = ContentAlignment.MiddleRight }, 0, 0);
            txtUsuario = new TextBox { Dock = DockStyle.Fill };
            loginPanel.Controls.Add(txtUsuario, 1, 0);

            // Password
            loginPanel.Controls.Add(new Label { Text = "Contraseña:", TextAlign = ContentAlignment.MiddleRight }, 0, 1);
            txtPassword = new TextBox { Dock = DockStyle.Fill, UseSystemPasswordChar = true };
            loginPanel.Controls.Add(txtPassword, 1, 1);

            // Button Panel
            var buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonPanel.Height = 40;

            var btnIngresar = new Button { Text = "Ingresar", Width = 100 };
            var btnCancelar = new Button { Text = "Cancelar", Width = 100 };

            btnIngresar.Click += (s, e) => Authenticate();
            btnCancelar.Click += (s, e) => this.Close();

            buttonPanel.Controls.Add(btnIngresar);
            buttonPanel.Controls.Add(btnCancelar);

            // Add controls to main layout
            tableLayout.Controls.Add(logoPanel, 0, 0);
            tableLayout.Controls.Add(loginPanel, 0, 1);
            tableLayout.Controls.Add(buttonPanel, 0, 2);

            this.Controls.Add(tableLayout);
            this.AcceptButton = btnIngresar;
            this.CancelButton = btnCancelar;
        }

        private void Authenticate()
        {
                if (!Utils.ValidateRequiredField(txtUsuario, "usuario") || 
                    !Utils.ValidateRequiredField(txtPassword, "contraseña"))
                {
                    return;
                }

                try
                {
                    if (SecurityHelper.AuthenticateUser(txtUsuario.Text, txtPassword.Text))
                    {
                        Username = txtUsuario.Text;
                        IsAuthenticated = true;
                        Utils.LogActivity(Username, "Login exitoso");
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        Utils.LogActivity(txtUsuario.Text, "Intento de login fallido");
                        Utils.ShowError("Usuario o contraseña incorrectos");
                    }
                }
                catch (Exception ex)
                {
                    Utils.ShowError($"Error al autenticar: {ex.Message}");
                }
        }
    }
}