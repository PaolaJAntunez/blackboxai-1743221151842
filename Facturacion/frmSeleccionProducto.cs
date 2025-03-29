using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Facturacion
{
    public partial class frmSeleccionProducto : Form
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public decimal IVA { get; set; }
        public decimal Descuento { get; set; }
        public int Cantidad { get; set; }

        private DataTable productosTable = new DataTable();

        public frmSeleccionProducto()
        {
            InitializeComponent();
            SetupForm();
            LoadProducts();
        }

        private void SetupForm()
        {
            this.Text = "Seleccionar Producto";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Main Table Layout
            var tableLayout = new TableLayoutPanel();
            tableLayout.Dock = DockStyle.Fill;
            tableLayout.ColumnCount = 2;
            tableLayout.RowCount = 5;
            tableLayout.Padding = new Padding(10);

            // Products Grid
            dgvProductos = new DataGridView();
            dgvProductos.Dock = DockStyle.Fill;
            dgvProductos.AutoGenerateColumns = false;
            dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductos.MultiSelect = false;
            dgvProductos.ReadOnly = true;
            tableLayout.SetColumnSpan(dgvProductos, 2);
            tableLayout.Controls.Add(dgvProductos, 0, 0);

            // Quantity
            tableLayout.Controls.Add(new Label { Text = "Cantidad:", TextAlign = ContentAlignment.MiddleRight }, 0, 1);
            numCantidad = new NumericUpDown();
            numCantidad.Minimum = 1;
            numCantidad.Maximum = 999;
            numCantidad.Value = 1;
            numCantidad.Dock = DockStyle.Fill;
            tableLayout.Controls.Add(numCantidad, 1, 1);

            // Discount
            tableLayout.Controls.Add(new Label { Text = "Descuento %:", TextAlign = ContentAlignment.MiddleRight }, 0, 2);
            numDescuento = new NumericUpDown();
            numDescuento.Minimum = 0;
            numDescuento.Maximum = 100;
            numDescuento.DecimalPlaces = 2;
            numDescuento.Dock = DockStyle.Fill;
            tableLayout.Controls.Add(numDescuento, 1, 2);

            // Price Display
            tableLayout.Controls.Add(new Label { Text = "Precio Unitario:", TextAlign = ContentAlignment.MiddleRight }, 0, 3);
            lblPrecio = new Label { Text = "0.00", TextAlign = ContentAlignment.MiddleLeft };
            tableLayout.Controls.Add(lblPrecio, 1, 3);

            // Button Panel
            var buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonPanel.Height = 40;

            var btnAceptar = new Button { Text = "Aceptar", DialogResult = DialogResult.OK };
            var btnCancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel };

            buttonPanel.Controls.AddRange(new Control[] { btnAceptar, btnCancelar });

            // Event Handlers
            dgvProductos.SelectionChanged += (s, e) => UpdateProductInfo();
            btnAceptar.Click += (s, e) => SetProperties();

            // Add controls to form
            this.Controls.Add(tableLayout);
            this.Controls.Add(buttonPanel);
            this.AcceptButton = btnAceptar;
            this.CancelButton = btnCancelar;
        }

        private void LoadProducts()
        {
            // TODO: Replace with actual database call
            productosTable.Columns.Add("Codigo");
            productosTable.Columns.Add("Descripcion");
            productosTable.Columns.Add("Precio", typeof(decimal));
            productosTable.Columns.Add("IVA", typeof(decimal));

            // Sample data
            productosTable.Rows.Add("PROD001", "Laptop HP 15\"", 799.99m, 10m);
            productosTable.Rows.Add("PROD002", "Mouse Inalámbrico", 25.50m, 10m);

            dgvProductos.DataSource = productosTable;

            // Configure columns
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Código",
                DataPropertyName = "Codigo",
                Width = 100
            });

            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Descripción",
                DataPropertyName = "Descripcion",
                Width = 250
            });

            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Precio",
                DataPropertyName = "Precio",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle()
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "IVA %",
                DataPropertyName = "IVA",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle()
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvProductos.ClearSelection();
        }

        private void UpdateProductInfo()
        {
            if (dgvProductos.SelectedRows.Count > 0)
            {
                var row = dgvProductos.SelectedRows[0].DataBoundItem as DataRowView;
                lblPrecio.Text = Convert.ToDecimal(row["Precio"]).ToString("C2");
            }
        }

        private void SetProperties()
        {
            if (dgvProductos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Debe seleccionar un producto", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
                return;
            }

            var row = dgvProductos.SelectedRows[0].DataBoundItem as DataRowView;
            Codigo = row["Codigo"].ToString();
            Descripcion = row["Descripcion"].ToString();
            Precio = Convert.ToDecimal(row["Precio"]);
            IVA = Convert.ToDecimal(row["IVA"]);
            Descuento = Convert.ToDecimal(numDescuento.Value);
            Cantidad = Convert.ToInt32(numCantidad.Value);
        }
    }
}