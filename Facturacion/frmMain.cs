using System;
using System.Drawing;
using System.Windows.Forms;

namespace Facturacion
{
    public partial class frmMain : Form
    {
        public string Username { get; set; }
        private DataTable userRoles;

        public frmMain()
        {
            InitializeComponent();
            LoadUserRoles();
            SetupUI();
        }

        private void LoadUserRoles()
        {
            userRoles = SecurityHelper.GetUserRoles(Username);
        }

        private bool UserHasAccess(string requiredRole)
        {
            foreach (DataRow row in userRoles.Rows)
            {
                if (row["Nombre"].ToString() == requiredRole)
                    return true;
            }
            return false;
        }

        private void SetupUI()
        {
            // Form configuration
            this.Text = "Sistema de Facturación";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 240, 240);
            
            // Main menu setup
            var menuStrip = new MenuStrip();
            menuStrip.BackColor = Color.FromArgb(51, 51, 51);
            menuStrip.ForeColor = Color.White;
            
            // File menu
            var fileMenu = new ToolStripMenuItem("Archivo");
            fileMenu.DropDownItems.Add("Salir", null, (s, e) => Application.Exit());
            
            // Modules menu
            var modulesMenu = new ToolStripMenuItem("Módulos");
            
            var clientesItem = new ToolStripMenuItem("Clientes", null, OpenClientForm);
            clientesItem.Enabled = UserHasAccess("Administrador") || UserHasAccess("Ventas");
            modulesMenu.DropDownItems.Add(clientesItem);
            
            var productosItem = new ToolStripMenuItem("Productos", null, OpenProductsForm);
            productosItem.Enabled = UserHasAccess("Administrador") || UserHasAccess("Inventario");
            modulesMenu.DropDownItems.Add(productosItem);
            
            var facturacionItem = new ToolStripMenuItem("Facturación", null, OpenBillingForm);
            facturacionItem.Enabled = UserHasAccess("Administrador") || UserHasAccess("Ventas");
            modulesMenu.DropDownItems.Add(facturacionItem);
            
            // Reports menu
            var reportsMenu = new ToolStripMenuItem("Reportes");
            
            var ventasItem = new ToolStripMenuItem("Ventas", null, OpenSalesReports);
            ventasItem.Enabled = UserHasAccess("Administrador") || UserHasAccess("Reportes");
            reportsMenu.DropDownItems.Add(ventasItem);
            
            var clientesReportItem = new ToolStripMenuItem("Clientes", null, OpenClientReports);
            clientesReportItem.Enabled = UserHasAccess("Administrador") || UserHasAccess("Reportes");
            reportsMenu.DropDownItems.Add(clientesReportItem);
            
            // Administration menu (only for admins)
            if (UserHasAccess("Administrador"))
            {
                var adminMenu = new ToolStripMenuItem("Administración");
                
                var usuariosItem = new ToolStripMenuItem("Usuarios", null, OpenUsersForm);
                adminMenu.DropDownItems.Add(usuariosItem);
                
                var backupItem = new ToolStripMenuItem("Respaldo", null, OpenBackupForm);
                adminMenu.DropDownItems.Add(backupItem);
                
                menuStrip.Items.Add(adminMenu);
            }
            
            // Help menu
            var helpMenu = new ToolStripMenuItem("Ayuda");
            helpMenu.DropDownItems.Add("Acerca de...", null, ShowAbout);
            
            // Status bar with user info
            var statusStrip = new StatusStrip();
            statusStrip.Items.Add($"Usuario: {Username}");
            statusStrip.Items.Add($"Rol: {userRoles.Rows[0]["Nombre"]}");
            statusStrip.Items.Add($"Fecha: {DateTime.Today.ToShortDateString()}");
            
            menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, modulesMenu, reportsMenu, helpMenu });
            this.Controls.Add(menuStrip);
            this.Controls.Add(statusStrip);
        }

        private void OpenClientForm(object sender, EventArgs e)
        {
            var clientForm = new frmClientes();
            clientForm.ShowDialog();
        }

        private void OpenProductsForm(object sender, EventArgs e)
        {
            var productsForm = new frmProductos();
            productsForm.ShowDialog();
        }

        private void OpenBillingForm(object sender, EventArgs e)
        {
            var billingForm = new frmFacturas();
            billingForm.ShowDialog();
        }

        private void OpenSalesReports(object sender, EventArgs e)
        {
            var salesReport = new frmReporteVentas();
            salesReport.ShowDialog();
        }

        private void OpenClientReports(object sender, EventArgs e)
        {
            var clientReport = new frmReporteClientes();
            clientReport.ShowDialog();
        }

        private void ShowAbout(object sender, EventArgs e)
        {
            MessageBox.Show("Sistema de Facturación v1.0\n© 2023", "Acerca de", 
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}