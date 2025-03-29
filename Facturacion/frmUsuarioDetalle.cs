using System;
using System.Drawing;
using System.Windows.Forms;

namespace Facturacion
{
    public partial class frmUsuarioDetalle : Form
    {
        public string Usuario { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public string Password { get; set; }

        public frmUsuarioDetalle()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            this.Text = "Detalle de Usuario";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Main Table Layout
            var tableLayout = new TableLayoutPanel();
            tableLayout.Dock = DockStyle.Fill;
            tableLayout.ColumnCount = 2;
            tableLayout.RowCount = 5;
            tableLayout.Padding = new Padding(10);

            // Add Rows
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // Controls
            tableLayout.Controls.Add(new Label { Text = "Usuario:", TextAlign = ContentAlignment.MiddleRight }, 0, 0);
            txtUsuario = new TextBox { Dock = DockStyle.Fill };
            tableLayout.Controls.Add(txtUsuario, 1, 0);

            tableLayout.Controls.Add(new Label { Text = "Nombre:", TextAlign = ContentAlignment.MiddleRight }, 0, 1);
            txtNombre = new TextBox { Dock = DockStyle.Fill };
            tableLayout.Controls.Add(txtNombre, 1, 1);

            tableLayout.Controls.Add(new Label { Text = "Contraseña:", TextAlign = ContentAlignment.MiddleRight }, 0, 2);
            txtPassword = new TextBox { Dock = DockStyle.Fill, UseSystemPasswordChar = true };
            tableLayout.Controls.Add(txtPassword, 1, 2);

            tableLayout.Controls.Add(new Label { Text = "Confirmar:", TextAlign = ContentAlignment.MiddleRight }, 0, 3);
            txtConfirmar = new TextBox { Dock = DockStyle.Fill, UseSystemPasswordChar = true };
            tableLayout.Controls.Add(txtConfirmar, 1, 3);

            chkActivo = new CheckBox { Text = "Usuario Activo", Checked = true };
            tableLayout.SetColumnSpan(chkActivo, 2);
            tableLayout.Controls.Add(chkActivo, 0, 4);

            // Button Panel
            var buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonPanel.Height = 40;

            var btnAceptar = new Button { Text = "Aceptar", DialogResult = DialogResult.OK };
            var btnCancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel };

            btnAceptar.Click += (s, e) => ValidateAndSave();
            buttonPanel.Controls.AddRange(new Control[] { btnAceptar, btnCancelar });

            // Load existing data if editing
            if (!string.IsNullOrEmpty(Usuario))
            {
                txtUsuario.Text = Usuario;
                txtNombre.Text = Nombre;
                chkActivo.Checked = Activo;
                txtUsuario.ReadOnly = true;
                this.Text = "Editar Usuario";
            }

            // Add controls to form
            this.Controls.Add(tableLayout);
            this.Controls.Add(buttonPanel);
            this.AcceptButton = btnAceptar;
            this.CancelButton = btnCancelar;
        }

        private void ValidateAndSave()
        {
            if (string.IsNullOrWhiteSpace(txtUsuario.Text) || 
                string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Debe completar todos los campos", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (!string.IsNullOrEmpty(Usuario) && string.IsNullOrEmpty(txtPassword.Text))
            {
                // Editing existing user without changing password
                Password = null;
            }
            else
            {
                if (txtPassword.Text != txtConfirmar.Text)
                {
                    MessageBox.Show("Las contraseñas no coinciden", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.None;
                    return;
                }

                if (txtPassword.Text.Length < 6)
                {
                    MessageBox.Show("La contraseña debe tener al menos 6 caracteres", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.None;
                    return;
                }

                Password = txtPassword.Text;
            }

            Usuario = txtUsuario.Text;
            Nombre = txtNombre.Text;
            Activo = chkActivo.Checked;
        }
    }
}