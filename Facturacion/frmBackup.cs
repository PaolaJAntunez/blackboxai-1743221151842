using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace Facturacion
{
    public partial class frmBackup : Form
    {
        public frmBackup()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            this.Text = "Respaldo de Base de Datos";
            this.Size = new Size(500, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Main Table Layout
            var tableLayout = new TableLayoutPanel();
            tableLayout.Dock = DockStyle.Fill;
            tableLayout.ColumnCount = 1;
            tableLayout.RowCount = 4;
            tableLayout.Padding = new Padding(15);

            // Backup Location
            tableLayout.Controls.Add(new Label { Text = "Ubicación del Respaldo:", TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
            
            var pathPanel = new Panel();
            pathPanel.Dock = DockStyle.Fill;
            pathPanel.Height = 30;

            txtBackupPath = new TextBox { Dock = DockStyle.Fill };
            var btnBrowse = new Button { Text = "Examinar...", Width = 80, Dock = DockStyle.Right };
            btnBrowse.Click += (s, e) => SelectBackupLocation();

            pathPanel.Controls.Add(txtBackupPath);
            pathPanel.Controls.Add(btnBrowse);
            tableLayout.Controls.Add(pathPanel, 0, 1);

            // Progress Bar
            progressBar = new ProgressBar { Dock = DockStyle.Fill, Height = 20 };
            tableLayout.Controls.Add(progressBar, 0, 2);

            // Button Panel
            var buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonPanel.Height = 40;

            var btnBackup = new Button { Text = "Generar Respaldo", Width = 120 };
            var btnCancelar = new Button { Text = "Cancelar", Width = 100 };

            btnBackup.Click += (s, e) => PerformBackup();
            btnCancelar.Click += (s, e) => this.Close();

            buttonPanel.Controls.AddRange(new Control[] { btnCancelar, btnBackup });
            tableLayout.Controls.Add(buttonPanel, 0, 3);

            // Add controls to form
            this.Controls.Add(tableLayout);

            // Set default backup path
            txtBackupPath.Text = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                $"Facturacion_Backup_{DateTime.Now:yyyyMMdd}.bak");
        }

        private void SelectBackupLocation()
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Backup Files (*.bak)|*.bak";
                saveDialog.FileName = Path.GetFileName(txtBackupPath.Text);
                saveDialog.InitialDirectory = Path.GetDirectoryName(txtBackupPath.Text);

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    txtBackupPath.Text = saveDialog.FileName;
                }
            }
        }

        private void PerformBackup()
        {
            if (string.IsNullOrWhiteSpace(txtBackupPath.Text))
            {
                MessageBox.Show("Debe seleccionar una ubicación para el respaldo", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string backupPath = txtBackupPath.Text;
                string backupDir = Path.GetDirectoryName(backupPath);

                if (!Directory.Exists(backupDir))
                {
                    Directory.CreateDirectory(backupDir);
                }

                string backupQuery = @"
                    BACKUP DATABASE [FacturacionDB] 
                    TO DISK = @BackupPath
                    WITH NOFORMAT, INIT, 
                    NAME = 'FacturacionDB-Full Database Backup', 
                    SKIP, NOREWIND, NOUNLOAD, STATS = 10";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@BackupPath", backupPath)
                };

                progressBar.Value = 0;
                progressBar.Style = ProgressBarStyle.Marquee;

                // Run backup in background to avoid UI freezing
                backgroundWorker.RunWorkerAsync(new { Query = backupQuery, Parameters = parameters });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al realizar el respaldo: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                progressBar.Style = ProgressBarStyle.Continuous;
            }
        }

        private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            dynamic args = e.Argument;
            DatabaseHelper.ExecuteNonQuery(args.Query, args.Parameters);
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = 100;

            if (e.Error != null)
            {
                MessageBox.Show($"Error en el respaldo: {e.Error.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Respaldo completado exitosamente", "Éxito", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}