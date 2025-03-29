using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Facturacion
{
    public partial class frmAsignarRoles : Form
    {
        public int UsuarioId { get; set; }
        public string Usuario { get; set; }
        private DataTable allRoles;
        private DataTable userRoles;

        public frmAsignarRoles()
        {
            InitializeComponent();
            SetupForm();
            LoadRoles();
        }

        private void SetupForm()
        {
            this.Text = $"Asignar Roles a {Usuario}";
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Main Table Layout
            var tableLayout = new TableLayoutPanel();
            tableLayout.Dock = DockStyle.Fill;
            tableLayout.ColumnCount = 1;
            tableLayout.RowCount = 3;
            tableLayout.Padding = new Padding(10);

            // Roles ListBox
            lstRoles = new CheckedListBox();
            lstRoles.Dock = DockStyle.Fill;
            lstRoles.CheckOnClick = true;
            tableLayout.Controls.Add(lstRoles, 0, 0);

            // Button Panel
            var buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonPanel.Height = 40;

            var btnGuardar = new Button { Text = "Guardar", Width = 100 };
            var btnCancelar = new Button { Text = "Cancelar", Width = 100 };

            btnGuardar.Click += (s, e) => SaveRoles();
            btnCancelar.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            buttonPanel.Controls.AddRange(new Control[] { btnCancelar, btnGuardar });
            tableLayout.Controls.Add(buttonPanel, 0, 1);

            // Add controls to form
            this.Controls.Add(tableLayout);
        }

        private void LoadRoles()
        {
            // Get all available roles
            string query = "SELECT Id, Nombre FROM Roles ORDER BY Nombre";
            allRoles = DatabaseHelper.ExecuteQuery(query);

            // Get user's current roles
            query = "SELECT RolId FROM UsuarioRoles WHERE UsuarioId = @UsuarioId";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UsuarioId", UsuarioId)
            };
            userRoles = DatabaseHelper.ExecuteQuery(query, parameters);

            // Populate checklistbox
            foreach (DataRow row in allRoles.Rows)
            {
                int roleId = Convert.ToInt32(row["Id"]);
                bool isChecked = false;

                foreach (DataRow userRole in userRoles.Rows)
                {
                    if (Convert.ToInt32(userRole["RolId"]) == roleId)
                    {
                        isChecked = true;
                        break;
                    }
                }

                lstRoles.Items.Add(row["Nombre"].ToString(), isChecked);
            }
        }

        private void SaveRoles()
        {
            using (SqlConnection connection = new SqlConnection(DatabaseHelper.connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // First remove all existing roles
                    string deleteQuery = "DELETE FROM UsuarioRoles WHERE UsuarioId = @UsuarioId";
                    SqlParameter[] deleteParams = new SqlParameter[]
                    {
                        new SqlParameter("@UsuarioId", UsuarioId)
                    };
                    DatabaseHelper.ExecuteNonQuery(deleteQuery, deleteParams, connection, transaction);

                    // Add selected roles
                    for (int i = 0; i < lstRoles.Items.Count; i++)
                    {
                        if (lstRoles.GetItemChecked(i))
                        {
                            string insertQuery = "INSERT INTO UsuarioRoles (UsuarioId, RolId) VALUES (@UsuarioId, @RolId)";
                            SqlParameter[] insertParams = new SqlParameter[]
                            {
                                new SqlParameter("@UsuarioId", UsuarioId),
                                new SqlParameter("@RolId", allRoles.Rows[i]["Id"])
                            };
                            DatabaseHelper.ExecuteNonQuery(insertQuery, insertParams, connection, transaction);
                        }
                    }

                    transaction.Commit();
                    MessageBox.Show("Roles actualizados correctamente", "Éxito", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Error al actualizar roles: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.None;
                }
            }
        }
    }
}