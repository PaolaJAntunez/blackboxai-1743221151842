using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Facturacion
{
    public partial class frmUsuarios : Form
    {
        private DataTable usuariosTable = new DataTable();

        public frmUsuarios()
        {
            InitializeComponent();
            SetupForm();
            LoadUsers();
        }

        private void SetupForm()
        {
            this.Text = "Gestión de Usuarios";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Main Table Layout
            var tableLayout = new TableLayoutPanel();
            tableLayout.Dock = DockStyle.Fill;
            tableLayout.ColumnCount = 1;
            tableLayout.RowCount = 2;
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 80));
            tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20));

            // DataGridView Configuration
            dgvUsuarios = new DataGridView();
            dgvUsuarios.Dock = DockStyle.Fill;
            dgvUsuarios.AutoGenerateColumns = false;
            dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsuarios.MultiSelect = false;
            dgvUsuarios.ReadOnly = true;

            // Add Columns
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Usuario",
                DataPropertyName = "Usuario",
                Width = 150
            });

            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "Nombre",
                DataPropertyName = "Nombre",
                Width = 200
            });

            dgvUsuarios.Columns.Add(new DataGridViewCheckBoxColumn()
            {
                HeaderText = "Activo",
                DataPropertyName = "Activo",
                Width = 60
            });

            // Button Panel
            var buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;

            var btnNuevo = new Button() { Text = "Nuevo", Width = 100 };
            var btnEditar = new Button() { Text = "Editar", Width = 100 };
            var btnRoles = new Button() { Text = "Asignar Roles", Width = 120 };
            var btnCerrar = new Button() { Text = "Cerrar", Width = 100 };

            // Event Handlers
            btnNuevo.Click += (s, e) => AddUser();
            btnEditar.Click += (s, e) => EditUser();
            btnRoles.Click += (s, e) => AssignRoles();
            btnCerrar.Click += (s, e) => this.Close();

            buttonPanel.Controls.AddRange(new Control[] { btnCerrar, btnRoles, btnEditar, btnNuevo });

            // Add controls to form
            tableLayout.Controls.Add(dgvUsuarios, 0, 0);
            tableLayout.Controls.Add(buttonPanel, 0, 1);
            this.Controls.Add(tableLayout);
        }

        private void LoadUsers()
        {
            string query = "SELECT Id, Usuario, Nombre, Activo FROM Usuarios ORDER BY Usuario";
            usuariosTable = DatabaseHelper.ExecuteQuery(query);
            dgvUsuarios.DataSource = usuariosTable;
        }

        private void AddUser()
        {
            var frm = new frmUsuarioDetalle();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                // TODO: Save to database
                LoadUsers();
            }
        }

        private void EditUser()
        {
            if (dgvUsuarios.SelectedRows.Count == 0) return;

            var row = dgvUsuarios.SelectedRows[0].DataBoundItem as DataRowView;
            var frm = new frmUsuarioDetalle()
            {
                Usuario = row["Usuario"].ToString(),
                Nombre = row["Nombre"].ToString(),
                Activo = Convert.ToBoolean(row["Activo"])
            };

            if (frm.ShowDialog() == DialogResult.OK)
            {
                // TODO: Update database
                LoadUsers();
            }
        }

        private void AssignRoles()
        {
            if (dgvUsuarios.SelectedRows.Count == 0) return;

            var row = dgvUsuarios.SelectedRows[0].DataBoundItem as DataRowView;
            var frm = new frmAsignarRoles()
            {
                UsuarioId = Convert.ToInt32(row["Id"]),
                Usuario = row["Usuario"].ToString()
            };

            frm.ShowDialog();
        }
    }
}