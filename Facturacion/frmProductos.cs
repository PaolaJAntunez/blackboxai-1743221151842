using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Facturacion
{
    public partial class frmProductos : Form
    {
        private DataTable productosTable = new DataTable();

        public frmProductos()
        {
            InitializeComponent();
            SetupForm();
            LoadProducts();
        }

        private void SetupForm()
        {
            this.Text = "Gestión de Productos";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Main Table Layout
            var tableLayout = new TableLayoutPanel();
            tableLayout.Dock = DockStyle.Fill;
            tableLayout.ColumnCount = 1;
            tableLayout.RowCount = 2;
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 80));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20));

            // DataGridView Configuration
            dgvProductos = new DataGridView();
            dgvProductos.Dock = DockStyle.Fill;
            dgvProductos.AutoGenerateColumns = false;
            dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductos.MultiSelect = false;
            dgvProductos.ReadOnly = true;

            // Add Columns
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

            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Stock",
                DataPropertyName = "Stock",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle() 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            // Button Panel
            var buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;

            var btnNuevo = new Button() { Text = "Nuevo", Width = 100 };
            var btnEditar = new Button() { Text = "Editar", Width = 100 };
            var btnEliminar = new Button() { Text = "Eliminar", Width = 100 };
            var btnCerrar = new Button() { Text = "Cerrar", Width = 100 };

            // Event Handlers
            btnNuevo.Click += (s, e) => AddProduct();
            btnEditar.Click += (s, e) => EditProduct();
            btnEliminar.Click += (s, e) => DeleteProduct();
            btnCerrar.Click += (s, e) => this.Close();

            buttonPanel.Controls.AddRange(new Control[] { btnCerrar, btnEliminar, btnEditar, btnNuevo });

            // Add controls to form
            tableLayout.Controls.Add(dgvProductos, 0, 0);
            tableLayout.Controls.Add(buttonPanel, 0, 1);
            this.Controls.Add(tableLayout);
        }

        private void LoadProducts()
        {
            // TODO: Replace with actual database call
            productosTable.Columns.Add("Codigo");
            productosTable.Columns.Add("Descripcion");
            productosTable.Columns.Add("Precio", typeof(decimal));
            productosTable.Columns.Add("IVA", typeof(decimal));
            productosTable.Columns.Add("Stock", typeof(int));

            // Sample data
            productosTable.Rows.Add("PROD001", "Laptop HP 15\"", 799.99m, 10m, 15);
            productosTable.Rows.Add("PROD002", "Mouse Inalámbrico", 25.50m, 10m, 50);

            dgvProductos.DataSource = productosTable;
        }

        private void AddProduct()
        {
            var frm = new frmProductoDetalle();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                // TODO: Save to database
                productosTable.Rows.Add(
                    frm.Codigo,
                    frm.Descripcion,
                    frm.Precio,
                    frm.IVA,
                    frm.Stock);
            }
        }

        private void EditProduct()
        {
            if (dgvProductos.SelectedRows.Count == 0) return;

            var row = dgvProductos.SelectedRows[0].DataBoundItem as DataRowView;
            var frm = new frmProductoDetalle()
            {
                Codigo = row["Codigo"].ToString(),
                Descripcion = row["Descripcion"].ToString(),
                Precio = Convert.ToDecimal(row["Precio"]),
                IVA = Convert.ToDecimal(row["IVA"]),
                Stock = Convert.ToInt32(row["Stock"])
            };

            if (frm.ShowDialog() == DialogResult.OK)
            {
                // TODO: Update database
                row["Descripcion"] = frm.Descripcion;
                row["Precio"] = frm.Precio;
                row["IVA"] = frm.IVA;
                row["Stock"] = frm.Stock;
                productosTable.AcceptChanges();
            }
        }

        private void DeleteProduct()
        {
            if (dgvProductos.SelectedRows.Count == 0) return;

            if (MessageBox.Show("¿Eliminar este producto?", "Confirmar", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // TODO: Delete from database
                var row = dgvProductos.SelectedRows[0].DataBoundItem as DataRowView;
                productosTable.Rows.Remove(row.Row);
            }
        }
    }
}