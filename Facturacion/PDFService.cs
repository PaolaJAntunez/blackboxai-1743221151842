using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;
using System.Data;
using System.Configuration;

namespace Facturacion
{
    public static class PDFService
    {
        private static readonly Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
        private static readonly Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
        private static readonly Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
        private static readonly Font boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9);

        public static void GenerateInvoicePDF(string invoiceNumber, DataTable invoiceHeader, DataTable invoiceItems, string outputPath)
        {
            Document document = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outputPath, FileMode.Create));

            document.Open();

            try
            {
                // Add company logo if exists
                string logoPath = ConfigurationManager.AppSettings["InvoiceLogoPath"];
                if (File.Exists(logoPath))
                {
                    Image logo = Image.GetInstance(logoPath);
                    logo.ScaleToFit(150f, 100f);
                    logo.Alignment = Element.ALIGN_RIGHT;
                    document.Add(logo);
                }

                // Add company info
                Paragraph companyInfo = new Paragraph();
                companyInfo.Add(new Chunk(ConfigurationManager.AppSettings["CompanyName"], titleFont));
                companyInfo.Add(new Chunk("\n" + ConfigurationManager.AppSettings["CompanyAddress"], normalFont));
                companyInfo.Add(new Chunk("\nTel: " + ConfigurationManager.AppSettings["CompanyPhone"], normalFont));
                companyInfo.Add(new Chunk("\nEmail: " + ConfigurationManager.AppSettings["CompanyEmail"], normalFont));
                document.Add(companyInfo);

                // Add invoice title
                Paragraph invoiceTitle = new Paragraph($"\nFACTURA #{invoiceNumber}", titleFont);
                invoiceTitle.Alignment = Element.ALIGN_CENTER;
                document.Add(invoiceTitle);

                // Add invoice header info
                PdfPTable headerTable = new PdfPTable(2);
                headerTable.WidthPercentage = 100;
                headerTable.SetWidths(new float[] { 1, 2 });

                AddHeaderCell(headerTable, "Fecha:");
                AddValueCell(headerTable, ((DateTime)invoiceHeader.Rows[0]["Fecha"]).ToShortDateString());
                
                AddHeaderCell(headerTable, "Cliente:");
                AddValueCell(headerTable, invoiceHeader.Rows[0]["Cliente"].ToString());
                
                AddHeaderCell(headerTable, "RUC:");
                AddValueCell(headerTable, invoiceHeader.Rows[0]["RUC"].ToString());
                
                AddHeaderCell(headerTable, "Dirección:");
                AddValueCell(headerTable, invoiceHeader.Rows[0]["Direccion"].ToString());

                document.Add(headerTable);

                // Add items table
                PdfPTable itemsTable = new PdfPTable(6);
                itemsTable.WidthPercentage = 100;
                itemsTable.SetWidths(new float[] { 1, 3, 1, 1, 1, 1 });

                // Add table headers
                AddTableHeaderCell(itemsTable, "Código");
                AddTableHeaderCell(itemsTable, "Descripción");
                AddTableHeaderCell(itemsTable, "Cantidad");
                AddTableHeaderCell(itemsTable, "Precio");
                AddTableHeaderCell(itemsTable, "IVA %");
                AddTableHeaderCell(itemsTable, "Total");

                // Add items
                foreach (DataRow row in invoiceItems.Rows)
                {
                    AddTableCell(itemsTable, row["Codigo"].ToString());
                    AddTableCell(itemsTable, row["Descripcion"].ToString());
                    AddTableCell(itemsTable, row["Cantidad"].ToString(), Element.ALIGN_RIGHT);
                    AddTableCell(itemsTable, Convert.ToDecimal(row["Precio"]).ToString("C2"), Element.ALIGN_RIGHT);
                    AddTableCell(itemsTable, row["IVA"].ToString(), Element.ALIGN_RIGHT);
                    AddTableCell(itemsTable, Convert.ToDecimal(row["Total"]).ToString("C2"), Element.ALIGN_RIGHT);
                }

                document.Add(itemsTable);

                // Add totals
                PdfPTable totalsTable = new PdfPTable(2);
                totalsTable.WidthPercentage = 50;
                totalsTable.HorizontalAlignment = Element.ALIGN_RIGHT;
                totalsTable.SetWidths(new float[] { 1, 1 });

                AddTotalCell(totalsTable, "Subtotal:", false);
                AddTotalCell(totalsTable, Convert.ToDecimal(invoiceHeader.Rows[0]["Subtotal"]).ToString("C2"), true);

                AddTotalCell(totalsTable, "IVA:", false);
                AddTotalCell(totalsTable, Convert.ToDecimal(invoiceHeader.Rows[0]["IVA"]).ToString("C2"), true);

                AddTotalCell(totalsTable, "Descuento:", false);
                AddTotalCell(totalsTable, Convert.ToDecimal(invoiceHeader.Rows[0]["Descuento"]).ToString("C2"), true);

                AddTotalCell(totalsTable, "TOTAL:", false);
                AddTotalCell(totalsTable, Convert.ToDecimal(invoiceHeader.Rows[0]["Total"]).ToString("C2"), true);

                document.Add(totalsTable);

                // Add footer
                Paragraph footer = new Paragraph("\n\nGracias por su compra!", normalFont);
                footer.Alignment = Element.ALIGN_CENTER;
                document.Add(footer);
            }
            finally
            {
                document.Close();
                writer.Close();
            }
        }

        private static void AddHeaderCell(PdfPTable table, string text)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, boldFont));
            cell.Border = PdfPCell.NO_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5;
            table.AddCell(cell);
        }

        private static void AddValueCell(PdfPTable table, string text)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, normalFont));
            cell.Border = PdfPCell.NO_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5;
            table.AddCell(cell);
        }

        private static void AddTableHeaderCell(PdfPTable table, string text)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, headerFont));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.BackgroundColor = new BaseColor(220, 220, 220);
            cell.Padding = 5;
            table.AddCell(cell);
        }

        private static void AddTableCell(PdfPTable table, string text, int alignment = Element.ALIGN_LEFT)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, normalFont));
            cell.HorizontalAlignment = alignment;
            cell.Padding = 5;
            table.AddCell(cell);
        }

        private static void AddTotalCell(PdfPTable table, string text, bool isBold)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, isBold ? boldFont : normalFont));
            cell.Border = PdfPCell.TOP_BORDER | PdfPCell.BOTTOM_BORDER;
            cell.BorderWidthTop = 0.5f;
            cell.BorderWidthBottom = 0.5f;
            cell.Padding = 5;
            table.AddCell(cell);
        }
    }
}