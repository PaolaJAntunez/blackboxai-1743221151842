using System;
using System.Drawing;
using System.Windows.Forms;

namespace Facturacion
{
    public static class Utils
    {
        public static void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void ShowInfo(string message)
        {
            MessageBox.Show(message, "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static DialogResult ShowQuestion(string message)
        {
            return MessageBox.Show(message, "Confirmación", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        public static void CenterControl(Control control)
        {
            control.Left = (control.Parent.Width - control.Width) / 2;
            control.Top = (control.Parent.Height - control.Height) / 2;
        }

        public static void FormatGrid(DataGridView grid)
        {
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.RowHeadersVisible = false;
            grid.BackgroundColor = SystemColors.Window;
            grid.BorderStyle = BorderStyle.None;
        }

        public static void ApplyWindowTheme(Form form)
        {
            form.BackColor = Color.White;
            form.Font = new Font("Segoe UI", 9);
            form.StartPosition = FormStartPosition.CenterScreen;
        }

        public static string FormatCurrency(decimal value)
        {
            return value.ToString("C2");
        }

        public static string FormatDate(DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }

        public static bool ValidateRequiredField(Control control, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(control.Text))
            {
                ShowError($"El campo {fieldName} es requerido");
                control.Focus();
                return false;
            }
            return true;
        }

        public static void LogActivity(string username, string action)
        {
            try
            {
                SecurityHelper.LogAccess(username, action);
            }
            catch
            {
                // Silently fail if logging fails
            }
        }
    }
}