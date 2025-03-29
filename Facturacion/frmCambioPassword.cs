using System;
using System.Drawing;
using System.Windows.Forms;

namespace Facturacion
{
    public partial class frmCambioPassword : Form
    {
        public string Username { get; set; }
        public string NewPassword { get; private set; }

        public frmCambioPassword()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            this.Text = "Cambiar Contraseña";
            this.Size = new Size(350, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Main Table Layout
            var tableLayout = new TableLayoutPanel();
            tableLayout.Dock = DockStyle.Fill;
            tableLayout.ColumnCount = 2;
            tableLayout.RowCount = 4;
            tableLayout.Padding = new Padding(15);

            // Current Password
            tableLayout.Controls.Add(new Label { Text = "Contraseña Actual:", TextAlign = ContentAlignment.MiddleRight }, 0, 0);
            txtCurrentPassword = new TextBox { Dock = DockStyle.Fill, UseSystemPasswordChar = true };
            tableLayout.Controls.Add(txtCurrentPassword, 1, 0);

            // New Password
            tableLayout.Controls.Add(new Label { Text = "Nueva Contraseña:", TextAlign = ContentAlignment.MiddleRight }, 0, 1);
            txtNewPassword = new TextBox { Dock = DockStyle.Fill, UseSystemPasswordChar = true };
            tableLayout.Controls.Add(txtNewPassword, 1, 1);

            // Confirm Password
            tableLayout.Controls.Add(new Label { Text = "Confirmar Nueva:", TextAlign = ContentAlignment.MiddleRight }, 0, 2);
            txtConfirmPassword = new TextBox { Dock = DockStyle.Fill, UseSystemPasswordChar = true };
            tableLayout.Controls.Add(txtConfirmPassword, 1, 2);

            // Button Panel
            var buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonPanel.Height = 40;

            var btnAceptar = new Button { Text = "Aceptar", DialogResult = DialogResult.OK };
            var btnCancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel };

            btnAceptar.Click += (s, e) => ValidateAndSave();
            buttonPanel.Controls.AddRange(new Control[] { btnAceptar, btnCancelar });

            // Add controls to form
            this.Controls.Add(tableLayout);
            this.Controls.Add(buttonPanel);
            this.AcceptButton = btnAceptar;
            this.CancelButton = btnCancelar;
        }

        private void ValidateAndSave()
        {
            if (string.IsNullOrWhiteSpace(txtCurrentPassword.Text) ||
                string.IsNullOrWhiteSpace(txtNewPassword.Text) ||
                string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
            {
                MessageBox.Show("Debe completar todos los campos", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Las nuevas contraseñas no coinciden", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (txtNewPassword.Text.Length < 6)
            {
                MessageBox.Show("La nueva contraseña debe tener al menos 6 caracteres", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (!SecurityHelper.ChangePassword(Username, txtCurrentPassword.Text, txtNewPassword.Text))
            {
                MessageBox.Show("No se pudo cambiar la contraseña. Verifique la contraseña actual.", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }

            NewPassword = txtNewPassword.Text;
            MessageBox.Show("Contraseña cambiada exitosamente", "Éxito", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}