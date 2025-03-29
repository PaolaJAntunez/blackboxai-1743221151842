using System;
using System.Data;
using System.Data.SqlClient;
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
            // Apply standard window theming
            Utils.ApplyWindowTheme(this);
            this.Text = "Gestión de Productos";
            this.Size = new Size(800, 600);

            // Main layout
            var tableLayout = new TableLayoutPanel { Dock = DockStyle.Fill };
            tableLayout.ColumnCount = 1;
            tableLayout.RowCount = 2;

            // Configure DataGridView
            dgvProductos = new DataGridView { Dock = DockStyle.Fill };
            Utils.FormatGrid(dgvProductos);

            // Add columns
            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn { 
                HeaderText = "Código", 
                DataPropertyName = "Codigo",
                Width = 100 
            });

            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn {
                HeaderText = "Descripción",
                DataPropertyName = "Descripcion",
                Width = 250
            });

            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn {
                HeaderText = "Precio",
                DataPropertyName = "Precio",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle {
                    Format = "C2",
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvProductos.Columns.Add(new DataGridViewTextBoxColumn {
                HeaderText = "IVA %",
                DataPropertyName = "IVA",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle {
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            // Button panel
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft
            };

            var buttons = new[]
            {
                new Button { Text = "Nuevo", Width = 100, Click = (s,e) => AddProduct() },
                new Button { Text = "Editar", Width = 100, Click = (s,e) => EditProduct() },
                new Button { Text = "Eliminar", Width = 100, Click = (s,e) => DeleteProduct() },
                new Button { Text = "Cerrar", Width = 100, Click = (s,e) => Close() }
            };

            buttonPanel.Controls.AddRange(buttons);

            // Add controls
            tableLayout.Controls.Add(dgvProductos);
            tableLayout.Controls.Add(buttonPanel);
            this.Controls.Add(tableLayout);
        }

        private void LoadProducts()
        {
            try
            {
                string query = "SELECT Id, Codigo, Descripcion, Precio, IVA FROM Productos WHERE Activo = 1";
                productosTable = DatabaseHelper.ExecuteQuery(query);
                dgvProductos.DataSource = productosTable;
            }
            catch (Exception ex)
            {
                Utils.ShowError($"Error al cargar productos: {ex.Message}");
            }
        }

        private void AddProduct()
        {
            var frm = new frmProductoDetalle();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string query = @"INSERT INTO Productos (Codigo, Descripcion, Precio, IVA, Activo) 
                                   VALUES (@Codigo, @Descripcion, @Precio, @IVA, 1)";
                    
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@Codigo", frm.Codigo),
                        new SqlParameter("@Descripcion", frm.Descripcion),
                        new SqlParameter("@Precio", frm.Precio),
                        new SqlParameter("@IVA", frm.IVA)
                    };

                    if (DatabaseHelper.ExecuteNonQuery(query, parameters) > 0)
                    {
                        Utils.ShowInfo("Producto agregado correctamente");
                        LoadProducts();
                    }
                }
                catch (Exception ex)
                {
                    Utils.ShowError($"Error al agregar producto: {ex.Message}");
                }
            }
        }

        private void EditProduct()
        {
            if (dgvProductos.SelectedRows.Count == 0) return;

            var row = dgvProductos.SelectedRows[0].DataBoundItem as DataRowView;
            var frm = new frmProductoDetalle
            {
                Id = Convert.ToInt32(row["Id"]),
                Codigo = row["Codigo"].ToString(),
                Descripcion = row["Descripcion"].ToString(),
                Precio = Convert.ToDecimal(row["Precio"]),
                IVA = Convert.ToDecimal(row["IVA"])
            };

            if (frm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string query = @"UPDATE Productos SET 
                                   Codigo = @Codigo,
                                   Descripcion = @Descripcion, 
                                   Precio = @Precio, 
                                   IVA = @IVA 
                                   WHERE Id = @Id";

                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@Id", frm.Id),
                        new SqlParameter("@Codigo", frm.Codigo),
                        new SqlParameter("@Descripcion", frm.Descripcion),
                        new SqlParameter("@Precio", frm.Precio),
                        new SqlParameter("@IVA", frm.IVA)
                    };

                    if (DatabaseHelper.ExecuteNonQuery(query, parameters) > 0)
                    {
                        Utils.ShowInfo("Producto actualizado correctamente");
                        LoadProducts();
                    }
                }
                catch (Exception ex)
                {
                    Utils.ShowError($"Error al actualizar producto: {ex.Message}");
                }
            }
        }

        private void DeleteProduct()
        {
            if (dgvProductos.SelectedRows.Count == 0) return;

            if (Utils.ShowQuestion("¿Eliminar este producto?") == DialogResult.Yes)
            {
                try
                {
                    int id = Convert.ToInt32(dgvProductos.SelectedRows[0].Cells["Id"].Value);
                    string query = "UPDATE Productos SET Activo = 0 WHERE Id = @Id";
                    
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@Id", id)
                    };

                    if (DatabaseHelper.ExecuteNonQuery(query, parameters) > 0)
                    {
                        Utils.ShowInfo("Producto eliminado correctamente");
                        LoadProducts();
                    }
                }
                catch (Exception ex)
                {
                    Utils.ShowError($"Error al eliminar producto: {ex.Message}");
                }
            }
        }
    }
}