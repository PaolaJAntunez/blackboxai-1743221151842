using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;

namespace Facturacion
{
    public static class SecurityHelper
    {
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static bool AuthenticateUser(string username, string password)
        {
            string hashedPassword = HashPassword(password);
            string query = "SELECT COUNT(*) FROM Usuarios WHERE Usuario = @Usuario AND Password = @Password AND Activo = 1";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Usuario", username),
                new SqlParameter("@Password", hashedPassword)
            };

            int count = Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, parameters));
            return count > 0;
        }

        public static DataTable GetUserRoles(string username)
        {
            string query = @"
                SELECT r.Nombre 
                FROM Roles r
                INNER JOIN UsuarioRoles ur ON r.Id = ur.RolId
                INNER JOIN Usuarios u ON ur.UsuarioId = u.Id
                WHERE u.Usuario = @Usuario";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Usuario", username)
            };

            return DatabaseHelper.ExecuteQuery(query, parameters);
        }

        public static bool UserHasRole(string username, string roleName)
        {
            string query = @"
                SELECT COUNT(*) 
                FROM Roles r
                INNER JOIN UsuarioRoles ur ON r.Id = ur.RolId
                INNER JOIN Usuarios u ON ur.UsuarioId = u.Id
                WHERE u.Usuario = @Usuario AND r.Nombre = @Rol";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Usuario", username),
                new SqlParameter("@Rol", roleName)
            };

            int count = Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, parameters));
            return count > 0;
        }

        public static void LogAccess(string username, string action)
        {
            string query = @"
                INSERT INTO Auditoria (Usuario, Accion, Fecha, DireccionIP)
                VALUES (@Usuario, @Accion, @Fecha, @DireccionIP)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Usuario", username),
                new SqlParameter("@Accion", action),
                new SqlParameter("@Fecha", DateTime.Now),
                new SqlParameter("@DireccionIP", GetLocalIPAddress())
            };

            DatabaseHelper.ExecuteNonQuery(query, parameters);
        }

        private static string GetLocalIPAddress()
        {
            // TODO: Implement actual IP address retrieval
            return "127.0.0.1";
        }

        public static bool ChangePassword(string username, string currentPassword, string newPassword)
        {
            if (!AuthenticateUser(username, currentPassword))
                return false;

            string hashedNewPassword = HashPassword(newPassword);
            string query = "UPDATE Usuarios SET Password = @Password WHERE Usuario = @Usuario";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Password", hashedNewPassword),
                new SqlParameter("@Usuario", username)
            };

            int affected = DatabaseHelper.ExecuteNonQuery(query, parameters);
            return affected > 0;
        }
    }
}