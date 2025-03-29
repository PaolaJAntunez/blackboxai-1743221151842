using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Facturacion
{
    public partial class frmFacturas : Form
    {
        private DataTable facturasTable = new DataTable();
        private DataTable detalleTable = new DataTable();

        public frmFacturas()
        {
            InitializeComponent();
            SetupForm();
            LoadInvoices();
        }

        private void SetupForm()
        {
            // Apply standard window theming
            Utils.ApplyWindowTheme(this);
            this.Text = "Gestión de Facturas";
            this.Size = new Size(1000, 700);

            // Main split container
            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 300
            };

            // Invoices grid
            dgvFacturas = new DataGridView { Dock = DockStyle.Fill };
            Utils.FormatGrid(dgvFacturas);
            dgvFacturas.Columns.Add(new DataGridViewTextBoxColumn { 
                HeaderText = "Número", 
                DataPropertyName = "Numero",
                Width = 120 
            });
            dgvFacturas.Columns.Add(new DataGridViewTextBoxColumn {
                HeaderText = "Fecha",
                DataPropertyName = "Fecha",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle {
                    Format = "d",
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });
            dgvFacturas.Columns.Add(new DataGridViewTextBoxColumn {
                HeaderText = "Cliente",
                DataPropertyName = "Cliente",
                Width = 200
            });
            dgvFacturas.Columns.Add(new DataGridViewTextBoxColumn {
                HeaderText = "Total",
                DataPropertyName = "Total",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });
            dgvFacturas.SelectionChanged += (s, e) => LoadInvoiceDetails();

            // Invoice details grid
            dgvDetalle = new DataGridView { Dock = DockStyle.Fill };
            Utils.FormatGrid(dgvDetalle);
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn {
                HeaderText = "Producto",
                DataPropertyName = "Producto",
                Width = 200
            });
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn {
                HeaderText = "Cantidad",
                DataPropertyName = "Cantidad",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle {
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn {
                HeaderText = "Precio",
                DataPropertyName = "Precio",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn {
                HeaderText = "Total",
                DataPropertyName = "Total",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            // Button panel
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                Height = 40
            };

            var buttons = new[]
            {
                new Button { Text = "Nueva", Width = 100, Click = (s,e) => NewInvoice() },
                new Button { Text = "Imprimir", Width = 100, Click = (s,e) => PrintInvoice() },
                new Button { Text = "Anular", Width = 100, Click = (s,e) => CancelInvoice() },
                new Button { Text = "Cerrar", Width = 100, Click = (s,e) => Close() }
            };

            buttonPanel.Controls.AddRange(buttons);

            // Add controls
            splitContainer.Panel1.Controls.Add(dgvFacturas);
            splitContainer.Panel2.Controls.Add(dgvDetalle);
            this.Controls.Add(splitContainer);
            this.Controls.Add(buttonPanel);
        }

        private void LoadInvoices()
        {
            try
            {
                string query = @"SELECT f.Id, f.Numero, f.Fecha, c.Nombre AS Cliente, f.Total 
                               FROM Facturas f
                               INNER JOIN Clientes c ON f.ClienteId = c.Id
                               WHERE f.Estado = 'Activa'
                               ORDER BY f.Fecha DESC";
                facturasTable = DatabaseHelper.ExecuteQuery(query);
                dgvFacturas.DataSource = facturasTable;
            }
            catch (Exception ex)
            {
                Utils.ShowError($"Error al cargar facturas: {ex.Message}");
            }
        }

        private void LoadInvoiceDetails()
        {
            if (dgvFacturas.SelectedRows.Count == 0) return;

            try
            {
                int facturaId = Convert.ToInt32(dgvFacturas.SelectedRows[0].Cells["Id"].Value);
                string query = @"SELECT p.Descripcion AS Producto, df.Cantidad, df.Precio, 
                               (df.Cantidad * df.Precio) AS Total
                               FROM DetalleFactura df
                               INNER JOIN Productos p ON df.ProductoId = p.Id
                               WHERE df.FacturaId = @FacturaId";
                
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@FacturaId", facturaId)
                };

                detalleTable = DatabaseHelper.ExecuteQuery(query, parameters);
                dgvDetalle.DataSource = detalleTable;
            }
            catch (Exception ex)
            {
                Utils.ShowError($"Error al cargar detalle: {ex.Message}");
            }
        }

        private void NewInvoice()
        {
            var frm = new frmNuevaFactura();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                LoadInvoices();
                Utils.ShowInfo("Factura creada correctamente");
            }
        }

        private void PrintInvoice()
        {
            if (dgvFacturas.SelectedRows.Count == 0) return;

            try
            {
                string numeroFactura = dgvFacturas.SelectedRows[0].Cells["Numero"].Value.ToString();
                string tempPath = Path.Combine(Path.GetTempPath(), $"{numeroFactura}.pdf");
                
                // Get invoice data
                DataTable invoiceHeader = InvoiceService.GetInvoiceHeader(numeroFactura);
                DataTable invoiceItems = InvoiceService.GetInvoiceDetails(numeroFactura);
                
                // Generate PDF
                PDFService.GenerateInvoicePDF(numeroFactura, invoiceHeader, invoiceItems, tempPath);
                
                // Open PDF
                System.Diagnostics.Process.Start(tempPath);
            }
            catch (Exception ex)
            {
                Utils.ShowError($"Error al imprimir factura: {ex.Message}");
            }
        }

        private void CancelInvoice()
        {
            if (dgvFacturas.SelectedRows.Count == 0) return;

            if (Utils.ShowQuestion("¿Anular esta factura?") == DialogResult.Yes)
            {
                try
                {
                    int facturaId = Convert.ToInt32(dgvFacturas.SelectedRows[0].Cells["Id"].Value);
                    string query = "UPDATE Facturas SET Estado = 'Anulada' WHERE Id = @Id";
                    
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@Id", facturaId)
                    };

                    if (DatabaseHelper.ExecuteNonQuery(query, parameters) > 0)
                    {
                        Utils.ShowInfo("Factura anulada correctamente");
                        LoadInvoices();
                    }
                }
                catch (Exception ex)
                {
                    Utils.ShowError($"Error al anular factura: {ex.Message}");
                }
            }
        }
    }
}