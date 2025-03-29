using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Facturacion
{
    public partial class frmReporteVentas : Form
    {
        private DataTable reportData = new DataTable();

        public frmReporteVentas()
        {
            InitializeComponent();
            SetupForm();
            LoadSampleData();
        }

        private void SetupForm()
        {
            this.Text = "Reporte de Ventas";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Main Table Layout
            var tableLayout = new TableLayoutPanel();
            tableLayout.Dock = DockStyle.Fill;
            tableLayout.ColumnCount = 1;
            tableLayout.RowCount = 3;
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 80));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));

            // Filter Panel
            var filterPanel = new Panel();
            filterPanel.Dock = DockStyle.Fill;
            filterPanel.BackColor = Color.WhiteSmoke;

            var filterTable = new TableLayoutPanel();
            filterTable.Dock = DockStyle.Fill;
            filterTable.ColumnCount = 4;
            filterTable.RowCount = 2;
            filterTable.Padding = new Padding(10);

            // Date Filters
            filterTable.Controls.Add(new Label { Text = "Desde:", TextAlign = ContentAlignment.MiddleRight }, 0, 0);
            dtpDesde = new DateTimePicker { Format = DateTimePickerFormat.Short };
            filterTable.Controls.Add(dtpDesde, 1, 0);

            filterTable.Controls.Add(new Label { Text = "Hasta:", TextAlign = ContentAlignment.MiddleRight }, 2, 0);
            dtpHasta = new DateTimePicker { Format = DateTimePickerFormat.Short };
            filterTable.Controls.Add(dtpHasta, 3, 0);

            // Client Filter
            filterTable.Controls.Add(new Label { Text = "Cliente:", TextAlign = ContentAlignment.MiddleRight }, 0, 1);
            cmbClientes = new ComboBox { Dock = DockStyle.Fill };
            cmbClientes.Items.Add("Todos los clientes");
            cmbClientes.SelectedIndex = 0;
            filterTable.Controls.Add(cmbClientes, 1, 1);

            // Filter Button
            var btnFiltrar = new Button { Text = "Filtrar", Width = 100 };
            btnFiltrar.Click += (s, e) => FilterData();
            filterTable.Controls.Add(btnFiltrar, 3, 1);

            filterPanel.Controls.Add(filterTable);

            // Report Grid
            dgvReporte = new DataGridView();
            dgvReporte.Dock = DockStyle.Fill;
            dgvReporte.AutoGenerateColumns = false;
            dgvReporte.ReadOnly = true;
            dgvReporte.AllowUserToAddRows = false;
            dgvReporte.AllowUserToDeleteRows = false;
            dgvReporte.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Button Panel
            var buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;

            var btnExportarPDF = new Button { Text = "Exportar a PDF", Width = 120 };
            var btnExportarExcel = new Button { Text = "Exportar a Excel", Width = 120 };
            var btnCerrar = new Button { Text = "Cerrar", Width = 100 };

            btnExportarPDF.Click += (s, e) => ExportToPDF();
            btnExportarExcel.Click += (s, e) => ExportToExcel();
            btnCerrar.Click += (s, e) => this.Close();

            buttonPanel.Controls.AddRange(new Control[] { btnCerrar, btnExportarExcel, btnExportarPDF });

            // Add controls to main layout
            tableLayout.Controls.Add(filterPanel, 0, 0);
            tableLayout.Controls.Add(dgvReporte, 0, 1);
            tableLayout.Controls.Add(buttonPanel, 0, 2);

            this.Controls.Add(tableLayout);

            // Set default dates
            dtpDesde.Value = DateTime.Today.AddMonths(-1);
            dtpHasta.Value = DateTime.Today;
        }

        private void LoadSampleData()
        {
            // TODO: Replace with actual database call
            reportData.Columns.Add("Fecha", typeof(DateTime));
            reportData.Columns.Add("Factura");
            reportData.Columns.Add("Cliente");
            reportData.Columns.Add("Subtotal", typeof(decimal));
            reportData.Columns.Add("IVA", typeof(decimal));
            reportData.Columns.Add("Total", typeof(decimal));

            // Sample data
            reportData.Rows.Add(DateTime.Today.AddDays(-5), "FACT-001", "Juan Pérez", 1200.50m, 120.05m, 1320.55m);
            reportData.Rows.Add(DateTime.Today.AddDays(-3), "FACT-002", "María Gómez", 850.75m, 85.08m, 935.83m);
            reportData.Rows.Add(DateTime.Today, "FACT-003", "Carlos Ruiz", 450.25m, 45.03m, 495.28m);

            // Configure grid columns
            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Fecha",
                DataPropertyName = "Fecha",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle()
                {
                    Format = "d",
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Factura",
                DataPropertyName = "Factura",
                Width = 120
            });

            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Cliente",
                DataPropertyName = "Cliente",
                Width = 200
            });

            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Subtotal",
                DataPropertyName = "Subtotal",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle()
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "IVA",
                DataPropertyName = "IVA",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle()
                {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvReporte.Columns.Add(new DataGridViewTextBoxColumn()
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

            dgvReporte.DataSource = reportData;

            // Load clients for filter
            cmbClientes.Items.Add("Juan Pérez");
            cmbClientes.Items.Add("María Gómez");
            cmbClientes.Items.Add("Carlos Ruiz");
        }

        private void FilterData()
        {
            // TODO: Implement actual filtering from database
            var filtered = reportData.Clone();
            var desde = dtpDesde.Value.Date;
            var hasta = dtpHasta.Value.Date.AddDays(1).AddSeconds(-1);
            var cliente = cmbClientes.SelectedItem.ToString();

            foreach (DataRow row in reportData.Rows)
            {
                var fecha = (DateTime)row["Fecha"];
                var rowCliente = row["Cliente"].ToString();

                if (fecha >= desde && fecha <= hasta &&
                    (cliente == "Todos los clientes" || rowCliente == cliente))
                {
                    filtered.ImportRow(row);
                }
            }

            dgvReporte.DataSource = filtered;
        }

        private void ExportToPDF()
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "PDF Files (*.pdf)|*.pdf";
                saveDialog.FileName = $"Reporte_Ventas_{DateTime.Today:yyyyMMdd}.pdf";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Document document = new Document(PageSize.A4.Rotate());
                        PdfWriter.GetInstance(document, new FileStream(saveDialog.FileName, FileMode.Create));
                        document.Open();

                        // Add title
                        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                        var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                        var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);

                        document.Add(new Paragraph("Reporte de Ventas", titleFont));
                        document.Add(new Paragraph($"Fecha: {DateTime.Today.ToShortDateString()}"));
                        document.Add(new Paragraph("\n"));

                        // Create table
                        PdfPTable table = new PdfPTable(dgvReporte.Columns.Count);
                        table.WidthPercentage = 100;

                        // Add headers
                        foreach (DataGridViewColumn column in dgvReporte.Columns)
                        {
                            table.AddCell(new Phrase(column.HeaderText, headerFont));
                        }

                        // Add rows
                        foreach (DataGridViewRow row in dgvReporte.Rows)
                        {
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                table.AddCell(new Phrase(cell.Value?.ToString() ?? "", cellFont));
                            }
                        }

                        document.Add(table);
                        document.Close();

                        MessageBox.Show("Reporte exportado a PDF correctamente", "Éxito", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al exportar PDF: {ex.Message}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ExportToExcel()
        {
            // TODO: Implement Excel export using EPPlus or similar
            MessageBox.Show("Función de exportación a Excel en desarrollo", "Información", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}