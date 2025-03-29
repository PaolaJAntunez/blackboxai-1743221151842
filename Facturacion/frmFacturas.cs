using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Facturacion
{
    public partial class frmFacturas : Form
    {
        private DataTable facturaItems = new DataTable();
        private decimal subtotal = 0;
        private decimal ivaTotal = 0;
        private decimal descuentoTotal = 0;
        private decimal total = 0;

        public frmFacturas()
        {
            InitializeComponent();
            SetupForm();
            InitializeInvoiceItems();
        }

        private void SetupForm()
        {
            this.Text = "Facturación";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Main Table Layout
            var tableLayout = new TableLayoutPanel();
            tableLayout.Dock = DockStyle.Fill;
            tableLayout.ColumnCount = 1;
            tableLayout.RowCount = 3;
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 70));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30));

            // Header Panel
            var headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Fill;
            headerPanel.BackColor = Color.White;

            var headerTable = new TableLayoutPanel();
            headerTable.Dock = DockStyle.Fill;
            headerTable.ColumnCount = 2;
            headerTable.RowCount = 3;

            // Client Info
            var lblCliente = new Label { Text = "Cliente:", Font = new Font("Arial", 9, FontStyle.Bold) };
            cmbClientes = new ComboBox { Dock = DockStyle.Fill };
            cmbClientes.DropDownStyle = ComboBoxStyle.DropDownList;

            // Invoice Info
            var lblNumero = new Label { Text = "Factura #:", Font = new Font("Arial", 9, FontStyle.Bold) };
            txtNumero = new TextBox { ReadOnly = true, BackColor = Color.White };
            var lblFecha = new Label { Text = "Fecha:", Font = new Font("Arial", 9, FontStyle.Bold) };
            dtpFecha = new DateTimePicker { Format = DateTimePickerFormat.Short };

            headerTable.Controls.Add(lblCliente, 0, 0);
            headerTable.Controls.Add(cmbClientes, 1, 0);
            headerTable.Controls.Add(lblNumero, 0, 1);
            headerTable.Controls.Add(txtNumero, 1, 1);
            headerTable.Controls.Add(lblFecha, 0, 2);
            headerTable.Controls.Add(dtpFecha, 1, 2);

            headerPanel.Controls.Add(headerTable);

            // Items Grid
            dgvItems = new DataGridView();
            dgvItems.Dock = DockStyle.Fill;
            dgvItems.AllowUserToAddRows = false;
            dgvItems.AutoGenerateColumns = false;
            dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Footer Panel
            var footerPanel = new Panel();
            footerPanel.Dock = DockStyle.Fill;

            var footerTable = new TableLayoutPanel();
            footerTable.Dock = DockStyle.Fill;
            footerTable.ColumnCount = 2;
            footerTable.RowCount = 5;

            // Totals
            footerTable.Controls.Add(new Label { Text = "Subtotal:", TextAlign = ContentAlignment.MiddleRight, Font = new Font("Arial", 9, FontStyle.Bold) }, 0, 0);
            lblSubtotal = new Label { Text = "0.00", TextAlign = ContentAlignment.MiddleRight };
            footerTable.Controls.Add(lblSubtotal, 1, 0);

            footerTable.Controls.Add(new Label { Text = "IVA:", TextAlign = ContentAlignment.MiddleRight, Font = new Font("Arial", 9, FontStyle.Bold) }, 0, 1);
            lblIVA = new Label { Text = "0.00", TextAlign = ContentAlignment.MiddleRight };
            footerTable.Controls.Add(lblIVA, 1, 1);

            footerTable.Controls.Add(new Label { Text = "Descuento:", TextAlign = ContentAlignment.MiddleRight, Font = new Font("Arial", 9, FontStyle.Bold) }, 0, 2);
            lblDescuento = new Label { Text = "0.00", TextAlign = ContentAlignment.MiddleRight };
            footerTable.Controls.Add(lblDescuento, 1, 2);

            footerTable.Controls.Add(new Label { Text = "Total:", TextAlign = ContentAlignment.MiddleRight, Font = new Font("Arial", 10, FontStyle.Bold) }, 0, 3);
            lblTotal = new Label { Text = "0.00", TextAlign = ContentAlignment.MiddleRight, Font = new Font("Arial", 10, FontStyle.Bold) };
            footerTable.Controls.Add(lblTotal, 1, 3);

            // Buttons
            var buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonPanel.Height = 40;

            var btnAgregar = new Button { Text = "Agregar Producto", Width = 120 };
            var btnEliminar = new Button { Text = "Eliminar", Width = 80 };
            var btnGuardar = new Button { Text = "Guardar Factura", Width = 120 };
            var btnImprimir = new Button { Text = "Imprimir", Width = 80 };
            var btnCerrar = new Button { Text = "Cerrar", Width = 80 };

            btnAgregar.Click += (s, e) => AddProduct();
            btnEliminar.Click += (s, e) => RemoveProduct();
            btnGuardar.Click += (s, e) => SaveInvoice();
            btnImprimir.Click += (s, e) => PrintInvoice();
            btnCerrar.Click += (s, e) => this.Close();

            buttonPanel.Controls.AddRange(new Control[] { btnCerrar, btnImprimir, btnGuardar, btnEliminar, btnAgregar });

            footerPanel.Controls.Add(footerTable);
            footerPanel.Controls.Add(buttonPanel);

            // Add controls to main layout
            tableLayout.Controls.Add(headerPanel, 0, 0);
            tableLayout.Controls.Add(dgvItems, 0, 1);
            tableLayout.Controls.Add(footerPanel, 0, 2);

            this.Controls.Add(tableLayout);

            // Generate invoice number
            txtNumero.Text = GenerateInvoiceNumber();
            dtpFecha.Value = DateTime.Today;
        }

        private void InitializeInvoiceItems()
        {
            facturaItems.Columns.Add("Codigo");
            facturaItems.Columns.Add("Descripcion");
            facturaItems.Columns.Add("Cantidad", typeof(int));
            facturaItems.Columns.Add("Precio", typeof(decimal));
            facturaItems.Columns.Add("IVA", typeof(decimal));
            facturaItems.Columns.Add("Descuento", typeof(decimal));
            facturaItems.Columns.Add("Total", typeof(decimal));

            dgvItems.DataSource = facturaItems;

            // Configure columns
            dgvItems.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Código",
                DataPropertyName = "Codigo",
                Width = 100
            });

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Descripción",
                DataPropertyName = "Descripcion",
                Width = 250
            });

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Cantidad",
                DataPropertyName = "Cantidad",
                Width = 80
            });

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn()
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

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "IVA %",
                DataPropertyName = "IVA",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle()
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Descuento",
                DataPropertyName = "Descuento",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle()
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Total",
                DataPropertyName = "Total",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle()
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Font = new Font("Arial", 9, FontStyle.Bold)
                }
            });

            // Load sample clients
            cmbClientes.Items.Add("CLI001 - Juan Pérez");
            cmbClientes.Items.Add("CLI002 - María Gómez");
            cmbClientes.SelectedIndex = 0;
        }

        private string GenerateInvoiceNumber()
        {
            // TODO: Replace with actual sequence from database
            return $"FACT-{DateTime.Today:yyyyMMdd}-001";
        }

        private void AddProduct()
        {
            var frm = new frmSeleccionProducto();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                // Calculate item total
                decimal precio = frm.Precio;
                decimal iva = precio * (frm.IVA / 100);
                decimal descuento = precio * (frm.Descuento / 100);
                decimal totalItem = (precio + iva - descuento) * frm.Cantidad;

                // Add to invoice
                facturaItems.Rows.Add(
                    frm.Codigo,
                    frm.Descripcion,
                    frm.Cantidad,
                    precio,
                    frm.IVA,
                    descuento * frm.Cantidad,
                    totalItem);

                UpdateTotals();
            }
        }

        private void RemoveProduct()
        {
            if (dgvItems.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvItems.SelectedRows)
                {
                    facturaItems.Rows.RemoveAt(row.Index);
                }
                UpdateTotals();
            }
        }

        private void UpdateTotals()
        {
            subtotal = 0;
            ivaTotal = 0;
            descuentoTotal = 0;
            total = 0;

            foreach (DataRow row in facturaItems.Rows)
            {
                subtotal += Convert.ToDecimal(row["Precio"]) * Convert.ToInt32(row["Cantidad"]);
                ivaTotal += Convert.ToDecimal(row["Precio"]) * (Convert.ToDecimal(row["IVA"]) / 100) * Convert.ToInt32(row["Cantidad"]);
                descuentoTotal += Convert.ToDecimal(row["Descuento"]);
            }

            total = subtotal + ivaTotal - descuentoTotal;

            lblSubtotal.Text = subtotal.ToString("C2");
            lblIVA.Text = ivaTotal.ToString("C2");
            lblDescuento.Text = descuentoTotal.ToString("C2");
            lblTotal.Text = total.ToString("C2");
        }

        private void SaveInvoice()
        {
            if (facturaItems.Rows.Count == 0)
            {
                MessageBox.Show("Debe agregar al menos un producto a la factura", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // TODO: Save to database
            MessageBox.Show("Factura guardada correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void PrintInvoice()
        {
            if (facturaItems.Rows.Count == 0)
            {
                MessageBox.Show("No hay productos para imprimir", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // TODO: Implement PDF generation
            MessageBox.Show("Función de impresión en desarrollo", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}