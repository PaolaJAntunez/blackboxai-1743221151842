using System;
using System.Data;
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
            // Apply standard window theming
            Utils.ApplyWindowTheme(this);
            this.Text = "Sistema de Facturación";
            this.WindowState = FormWindowState.Maximized;

            // Main menu setup
            var menuStrip = new MenuStrip();
            
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
            
            // Help menu
            var helpMenu = new ToolStripMenuItem("Ayuda");
            helpMenu.DropDownItems.Add("Acerca de...", null, ShowAbout);

            menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, modulesMenu, reportsMenu, helpMenu });
            this.Controls.Add(menuStrip);

            // Status bar
            var statusStrip = new StatusStrip();
            statusStrip.Items.Add($"Usuario: {Username}");
            statusStrip.Items.Add($"Rol: {userRoles.Rows[0]["Nombre"]}");
            statusStrip.Items.Add($"Fecha: {DateTime.Today:dd/MM/yyyy}");
            this.Controls.Add(statusStrip);
        }

        private void OpenClientForm(object sender, EventArgs e)
        {
            Utils.LogActivity(Username, "Acceso a módulo Clientes");
            var clientForm = new frmClientes_NEW();
            clientForm.ShowDialog();
        }

        private void OpenProductsForm(object sender, EventArgs e)
        {
            Utils.LogActivity(Username, "Acceso a módulo Productos");
            var productsForm = new frmProductos_NEW();
            productsForm.ShowDialog();
        }

        private void OpenBillingForm(object sender, EventArgs e)
        {
            Utils.LogActivity(Username, "Acceso a módulo Facturación");
            var billingForm = new frmFacturas_NEW();
            billingForm.ShowDialog();
        }

        private void OpenSalesReports(object sender, EventArgs e)
        {
            Utils.LogActivity(Username, "Acceso a reportes de ventas");
            var reportsForm = new frmReporteVentas();
            reportsForm.ShowDialog();
        }

        private void ShowAbout(object sender, EventArgs e)
        {
            Utils.ShowInfo("Sistema de Facturación v2.0\n© 2023");
        }
    }
}