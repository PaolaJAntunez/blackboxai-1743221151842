using System;
using System.Globalization;
using System.Windows.Forms;

namespace Facturacion
{
    public partial class frmProductoDetalle : Form
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public decimal IVA { get; set; }
        public int Stock { get; set; }
        public string Categoria { get; set; }

        public frmProductoDetalle()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            this.Text = "Detalle de Producto";
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
            tableLayout.Controls.Add(new Label { Text = "Descripción:", TextAlign = ContentAlignment.MiddleRight }, 0, 1);
            tableLayout.Controls.Add(new Label { Text = "Precio:", TextAlign = ContentAlignment.MiddleRight }, 0, 2);
            tableLayout.Controls.Add(new Label { Text = "IVA %:", TextAlign = ContentAlignment.MiddleRight }, 0, 3);
            tableLayout.Controls.Add(new Label { Text = "Stock:", TextAlign = ContentAlignment.MiddleRight }, 0, 4);
            tableLayout.Controls.Add(new Label { Text = "Categoría:", TextAlign = ContentAlignment.MiddleRight }, 0, 5);

            // Input Controls
            var txtCodigo = new TextBox { Dock = DockStyle.Fill };
            var txtDescripcion = new TextBox { Dock = DockStyle.Fill };
            var txtPrecio = new TextBox { Dock = DockStyle.Fill };
            var txtIVA = new TextBox { Dock = DockStyle.Fill };
            var txtStock = new TextBox { Dock = DockStyle.Fill };
            
            var cmbCategoria = new ComboBox { Dock = DockStyle.Fill };
            cmbCategoria.Items.AddRange(new object[] { "Electrónica", "Oficina", "Hogar", "Otros" });

            // Format numeric inputs
            txtPrecio.TextChanged += (s, e) => FormatCurrency(txtPrecio);
            txtIVA.TextChanged += (s, e) => FormatPercentage(txtIVA);
            txtStock.KeyPress += (s, e) => e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);

            tableLayout.Controls.Add(txtCodigo, 1, 0);
            tableLayout.Controls.Add(txtDescripcion, 1, 1);
            tableLayout.Controls.Add(txtPrecio, 1, 2);
            tableLayout.Controls.Add(txtIVA, 1, 3);
            tableLayout.Controls.Add(txtStock, 1, 4);
            tableLayout.Controls.Add(cmbCategoria, 1, 5);

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
                Descripcion = txtDescripcion.Text;
                Precio = decimal.Parse(txtPrecio.Text.Replace("$", "").Trim());
                IVA = decimal.Parse(txtIVA.Text.Replace("%", "").Trim());
                Stock = int.Parse(txtStock.Text);
                Categoria = cmbCategoria.Text;
            };

            // Load existing data if editing
            if (!string.IsNullOrEmpty(Codigo))
            {
                txtCodigo.Text = Codigo;
                txtDescripcion.Text = Descripcion;
                txtPrecio.Text = Precio.ToString("C2", CultureInfo.CurrentCulture);
                txtIVA.Text = IVA.ToString("0.##") + "%";
                txtStock.Text = Stock.ToString();
                cmbCategoria.Text = Categoria;
                txtCodigo.ReadOnly = true; // Don't allow editing code
            }

            // Add controls to form
            this.Controls.Add(tableLayout);
            this.Controls.Add(buttonPanel);
            this.AcceptButton = btnAceptar;
            this.CancelButton = btnCancelar;
        }

        private void FormatCurrency(TextBox textBox)
        {
            if (decimal.TryParse(textBox.Text.Replace("$", ""), out decimal value))
            {
                textBox.Text = value.ToString("C2");
                textBox.SelectionStart = textBox.Text.Length;
            }
        }

        private void FormatPercentage(TextBox textBox)
        {
            if (decimal.TryParse(textBox.Text.Replace("%", ""), out decimal value))
            {
                textBox.Text = value.ToString("0.##") + "%";
                textBox.SelectionStart = textBox.Text.Length - 1;
            }
        }
    }
}