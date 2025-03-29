using System;
using System.Windows.Forms;

namespace Facturacion
{
    public partial class frmClienteDetalle : Form
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string RUC { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }

        public frmClienteDetalle()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            this.Text = "Detalle de Cliente";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Main Table Layout
            var tableLayout = new TableLayoutPanel();
            tableLayout.Dock = DockStyle.Fill;
            tableLayout.ColumnCount = 2;
            tableLayout.RowCount = 7;
            tableLayout.Padding = new Padding(10);

            // Add Rows
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // Labels
            tableLayout.Controls.Add(new Label { Text = "Código:", TextAlign = ContentAlignment.MiddleRight }, 0, 0);
            tableLayout.Controls.Add(new Label { Text = "Nombre:", TextAlign = ContentAlignment.MiddleRight }, 0, 1);
            tableLayout.Controls.Add(new Label { Text = "RUC:", TextAlign = ContentAlignment.MiddleRight }, 0, 2);
            tableLayout.Controls.Add(new Label { Text = "Dirección:", TextAlign = ContentAlignment.MiddleRight }, 0, 3);
            tableLayout.Controls.Add(new Label { Text = "Teléfono:", TextAlign = ContentAlignment.MiddleRight }, 0, 4);
            tableLayout.Controls.Add(new Label { Text = "Email:", TextAlign = ContentAlignment.MiddleRight }, 0, 5);

            // TextBoxes
            var txtCodigo = new TextBox { Dock = DockStyle.Fill };
            var txtNombre = new TextBox { Dock = DockStyle.Fill };
            var txtRUC = new TextBox { Dock = DockStyle.Fill };
            var txtDireccion = new TextBox { Dock = DockStyle.Fill };
            var txtTelefono = new TextBox { Dock = DockStyle.Fill };
            var txtEmail = new TextBox { Dock = DockStyle.Fill };

            tableLayout.Controls.Add(txtCodigo, 1, 0);
            tableLayout.Controls.Add(txtNombre, 1, 1);
            tableLayout.Controls.Add(txtRUC, 1, 2);
            tableLayout.Controls.Add(txtDireccion, 1, 3);
            tableLayout.Controls.Add(txtTelefono, 1, 4);
            tableLayout.Controls.Add(txtEmail, 1, 5);

            // Button Panel
            var buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonPanel.Height = 40;

            var btnAceptar = new Button { Text = "Aceptar", DialogResult = DialogResult.OK };
            var btnCancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel };

            buttonPanel.Controls.AddRange(new Control[] { btnAceptar, btnCancelar });

            // Event Handlers
            btnAceptar.Click += (s, e) => 
            {
                Codigo = txtCodigo.Text;
                Nombre = txtNombre.Text;
                RUC = txtRUC.Text;
                Direccion = txtDireccion.Text;
                Telefono = txtTelefono.Text;
                Email = txtEmail.Text;
            };

            // Load existing data if editing
            if (!string.IsNullOrEmpty(Codigo))
            {
                txtCodigo.Text = Codigo;
                txtNombre.Text = Nombre;
                txtRUC.Text = RUC;
                txtDireccion.Text = Direccion;
                txtTelefono.Text = Telefono;
                txtEmail.Text = Email;
                txtCodigo.ReadOnly = true; // Don't allow editing code
            }

            // Add controls to form
            this.Controls.Add(tableLayout);
            this.Controls.Add(buttonPanel);
            this.AcceptButton = btnAceptar;
            this.CancelButton = btnCancelar;
        }
    }
}