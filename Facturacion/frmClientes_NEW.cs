using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Facturacion
{
    public partial class frmClientes : Form
    {
        private DataTable clientesTable = new DataTable();

        public frmClientes()
        {
            InitializeComponent();
            SetupForm();
            LoadClients();
        }

        private void SetupForm()
        {
            // Apply standard window theming
            Utils.ApplyWindowTheme(this);
            this.Text = "Gestión de Clientes";
            this.Size = new Size(800, 600);

            // Main layout
            var tableLayout = new TableLayoutPanel { Dock = DockStyle.Fill };
            tableLayout.ColumnCount = 1;
            tableLayout.RowCount = 2;

            // Configure DataGridView
            dgvClientes = new DataGridView { Dock = DockStyle.Fill };
            Utils.FormatGrid(dgvClientes);

            // Add columns
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { 
                HeaderText = "Código", 
                DataPropertyName = "Codigo",
                Width = 100 
            });
            
            // Add other columns here...

            // Button panel
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft
            };

            var buttons = new[]
            {
                new Button { Text = "Nuevo", Width = 100, Click = (s,e) => AddClient() },
                new Button { Text = "Editar", Width = 100, Click = (s,e) => EditClient() },
                new Button { Text = "Eliminar", Width = 100, Click = (s,e) => DeleteClient() },
                new Button { Text = "Cerrar", Width = 100, Click = (s,e) => Close() }
            };

            buttonPanel.Controls.AddRange(buttons);

            // Add controls
            tableLayout.Controls.Add(dgvClientes);
            tableLayout.Controls.Add(buttonPanel);
            this.Controls.Add(tableLayout);
        }

        private void LoadClients()
        {
            try
            {
                string query = "SELECT Codigo, Nombre, RUC, Telefono FROM Clientes WHERE Activo = 1";
                clientesTable = DatabaseHelper.ExecuteQuery(query);
                dgvClientes.DataSource = clientesTable;
            }
            catch (Exception ex)
            {
                Utils.ShowError($"Error al cargar clientes: {ex.Message}");
            }
        }

        private void AddClient()
        {
            var frm = new frmClienteDetalle();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string query = @"INSERT INTO Clientes (Codigo, Nombre, RUC, Telefono, Activo) 
                                   VALUES (@Codigo, @Nombre, @RUC, @Telefono, 1)";
                    
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@Codigo", frm.Codigo),
                        new SqlParameter("@Nombre", frm.Nombre),
                        new SqlParameter("@RUC", frm.RUC),
                        new SqlParameter("@Telefono", frm.Telefono)
                    };

                    if (DatabaseHelper.ExecuteNonQuery(query, parameters) > 0)
                    {
                        Utils.ShowInfo("Cliente agregado correctamente");
                        LoadClients();
                    }
                }
                catch (Exception ex)
                {
                    Utils.ShowError($"Error al agregar cliente: {ex.Message}");
                }
            }
        }

        private void EditClient()
        {
            if (dgvClientes.SelectedRows.Count == 0) return;

            var row = dgvClientes.SelectedRows[0].DataBoundItem as DataRowView;
            var frm = new frmClienteDetalle
            {
                Codigo = row["Codigo"].ToString(),
                Nombre = row["Nombre"].ToString(),
                RUC = row["RUC"].ToString(),
                Telefono = row["Telefono"].ToString()
            };

            if (frm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string query = @"UPDATE Clientes SET 
                                   Nombre = @Nombre, 
                                   RUC = @RUC, 
                                   Telefono = @Telefono 
                                   WHERE Codigo = @Codigo";

                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@Codigo", frm.Codigo),
                        new SqlParameter("@Nombre", frm.Nombre),
                        new SqlParameter("@RUC", frm.RUC),
                        new SqlParameter("@Telefono", frm.Telefono)
                    };

                    if (DatabaseHelper.ExecuteNonQuery(query, parameters) > 0)
                    {
                        Utils.ShowInfo("Cliente actualizado correctamente");
                        LoadClients();
                    }
                }
                catch (Exception ex)
                {
                    Utils.ShowError($"Error al actualizar cliente: {ex.Message}");
                }
            }
        }

        private void DeleteClient()
        {
            if (dgvClientes.SelectedRows.Count == 0) return;

            if (Utils.ShowQuestion("¿Eliminar este cliente?") == DialogResult.Yes)
            {
                try
                {
                    string codigo = dgvClientes.SelectedRows[0].Cells["Codigo"].Value.ToString();
                    string query = "UPDATE Clientes SET Activo = 0 WHERE Codigo = @Codigo";
                    
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@Codigo", codigo)
                    };

                    if (DatabaseHelper.ExecuteNonQuery(query, parameters) > 0)
                    {
                        Utils.ShowInfo("Cliente eliminado correctamente");
                        LoadClients();
                    }
                }
                catch (Exception ex)
                {
                    Utils.ShowError($"Error al eliminar cliente: {ex.Message}");
                }
            }
        }
    }
}