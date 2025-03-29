using System;
using System.Data;
using System.Data.SqlClient;

namespace Facturacion
{
    public static class InvoiceService
    {
        public static DataTable GetClientList()
        {
            string query = "SELECT Id, Codigo, Nombre, RUC FROM Clientes WHERE Activo = 1 ORDER BY Nombre";
            return DatabaseHelper.ExecuteQuery(query);
        }

        public static DataTable GetProductList()
        {
            string query = "SELECT Id, Codigo, Descripcion, Precio, IVA FROM Productos ORDER BY Descripcion";
            return DatabaseHelper.ExecuteQuery(query);
        }

        public static string CreateInvoice(int clienteId, DateTime fecha, DataTable items)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseHelper.connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Insert invoice header
                    string invoiceNumber = DatabaseHelper.GetNextInvoiceNumber();
                    string insertInvoice = @"
                        INSERT INTO Facturas (Numero, Fecha, ClienteId, Subtotal, IVA, Descuento, Total, Estado)
                        VALUES (@Numero, @Fecha, @ClienteId, @Subtotal, @IVA, @Descuento, @Total, 'Pendiente');
                        SELECT SCOPE_IDENTITY();";

                    decimal subtotal = 0;
                    decimal iva = 0;
                    decimal descuento = 0;

                    // Calculate totals from items
                    foreach (DataRow row in items.Rows)
                    {
                        subtotal += Convert.ToDecimal(row["Precio"]) * Convert.ToInt32(row["Cantidad"]);
                        iva += Convert.ToDecimal(row["Precio"]) * (Convert.ToDecimal(row["IVA"]) / 100) * Convert.ToInt32(row["Cantidad"]);
                        descuento += Convert.ToDecimal(row["Descuento"]);
                    }

                    decimal total = subtotal + iva - descuento;

                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@Numero", invoiceNumber),
                        new SqlParameter("@Fecha", fecha),
                        new SqlParameter("@ClienteId", clienteId),
                        new SqlParameter("@Subtotal", subtotal),
                        new SqlParameter("@IVA", iva),
                        new SqlParameter("@Descuento", descuento),
                        new SqlParameter("@Total", total)
                    };

                    int invoiceId = Convert.ToInt32(DatabaseHelper.ExecuteScalar(insertInvoice, parameters, connection, transaction));

                    // Insert invoice items
                    foreach (DataRow row in items.Rows)
                    {
                        string insertItem = @"
                            INSERT INTO DetalleFactura (FacturaId, ProductoId, Cantidad, Precio, IVA, Descuento)
                            VALUES (@FacturaId, @ProductoId, @Cantidad, @Precio, @IVA, @Descuento)";

                        SqlParameter[] itemParams = new SqlParameter[]
                        {
                            new SqlParameter("@FacturaId", invoiceId),
                            new SqlParameter("@ProductoId", row["ProductoId"]),
                            new SqlParameter("@Cantidad", row["Cantidad"]),
                            new SqlParameter("@Precio", row["Precio"]),
                            new SqlParameter("@IVA", row["IVA"]),
                            new SqlParameter("@Descuento", row["Descuento"])
                        };

                        DatabaseHelper.ExecuteNonQuery(insertItem, itemParams, connection, transaction);
                    }

                    transaction.Commit();
                    return invoiceNumber;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Error al crear la factura: " + ex.Message, ex);
                }
            }
        }

        public static DataTable GetInvoiceReport(DateTime desde, DateTime hasta, int? clienteId = null)
        {
            string query = @"
                SELECT 
                    f.Numero, 
                    f.Fecha, 
                    c.Nombre AS Cliente,
                    f.Subtotal,
                    f.IVA,
                    f.Total
                FROM Facturas f
                INNER JOIN Clientes c ON f.ClienteId = c.Id
                WHERE f.Fecha BETWEEN @Desde AND @Hasta";

            if (clienteId.HasValue)
            {
                query += " AND f.ClienteId = @ClienteId";
            }

            query += " ORDER BY f.Fecha DESC";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Desde", desde),
                new SqlParameter("@Hasta", hasta)
            };

            if (clienteId.HasValue)
            {
                Array.Resize(ref parameters, 3);
                parameters[2] = new SqlParameter("@ClienteId", clienteId.Value);
            }

            return DatabaseHelper.ExecuteQuery(query, parameters);
        }

        public static DataTable GetInvoiceDetails(string numeroFactura)
        {
            string query = @"
                SELECT 
                    p.Codigo,
                    p.Descripcion,
                    df.Cantidad,
                    df.Precio,
                    df.IVA,
                    df.Descuento,
                    (df.Precio * df.Cantidad) + (df.Precio * df.IVA / 100 * df.Cantidad) - df.Descuento AS Total
                FROM DetalleFactura df
                INNER JOIN Productos p ON df.ProductoId = p.Id
                INNER JOIN Facturas f ON df.FacturaId = f.Id
                WHERE f.Numero = @NumeroFactura";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@NumeroFactura", numeroFactura)
            };

            return DatabaseHelper.ExecuteQuery(query, parameters);
        }
    }
}