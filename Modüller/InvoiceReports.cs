using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace DENEME.Reports
{
    public partial class InvoiceReport : XtraReport
    {
        public InvoiceReport()
        {
            InitializeComponent();
            CreateReportLayout();

            Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[]
            {
                new DevExpress.XtraReports.Parameters.Parameter() { Name = "FaturaID", Type = typeof(int), Visible = false },
                new DevExpress.XtraReports.Parameters.Parameter() { Name = "FaturaNo", Type = typeof(string), Visible = false },
                new DevExpress.XtraReports.Parameters.Parameter() { Name = "FaturaTipi", Type = typeof(string), Visible = false },
                new DevExpress.XtraReports.Parameters.Parameter() { Name = "Tarih", Type = typeof(DateTime), Visible = false },
                new DevExpress.XtraReports.Parameters.Parameter() { Name = "MusteriID", Type = typeof(int), Visible = false },
                new DevExpress.XtraReports.Parameters.Parameter() { Name = "MusteriAdi", Type = typeof(string), Visible = false },
                new DevExpress.XtraReports.Parameters.Parameter() { Name = "MusteriTipi", Type = typeof(string), Visible = false },
                new DevExpress.XtraReports.Parameters.Parameter() { Name = "OdemeSekli", Type = typeof(string), Visible = false },
                new DevExpress.XtraReports.Parameters.Parameter() { Name = "ToplamTutar", Type = typeof(decimal),  Visible = false }
            });
        }

        public void SetData(List<InvoiceItem> items)
        {
            this.DataSource = items;
        }

        private void CreateReportLayout()
        {
            this.PaperKind = (DevExpress.Drawing.Printing.DXPaperKind)System.Drawing.Printing.PaperKind.A4;
            this.Margins = new System.Drawing.Printing.Margins(50, 50, 50, 50);
            this.Landscape = false;

            float usableWidth = PageWidth - Margins.Left - Margins.Right;

            ReportHeaderBand header = new ReportHeaderBand() { HeightF = 220 };
            this.Bands.Add(header);

            XRLabel lblBaslik = new XRLabel()
            {
                Text = "FATURA",
                Font = new Font("Arial", 16, FontStyle.Bold),
                BoundsF = new RectangleF(0, 0, usableWidth, 40),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter
            };
            header.Controls.Add(lblBaslik);

            XRLabel lblFirma = new XRLabel()
            {
                Font = new Font("Arial", 10),
                LocationF = new PointF(0, 50),
                SizeF = new SizeF(400, 60),
                Multiline = true,
                Text = "CNR TOZ BOYA\nCNR BOYA YAPI KİMYASAL VE TİC.LTD.ŞTİ\nAnbar, 54. Cad. No:15/A, 38070 Melikgazi\nKAYSERİ\nTÜRKİYE\n(0352) 503 83 83"
            };
            header.Controls.Add(lblFirma);

            XRLabel lblFaturaBilgi = new XRLabel()
            {
                Font = new Font("Arial", 10),
                SizeF = new SizeF(350, 60),
                LocationF = new PointF(usableWidth - 350, 50),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight
            };
            lblFaturaBilgi.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text",
                "'Fatura No: ' + [Parameters.FaturaNo] + '\nTarih: ' + FormatString('{0:dd.MM.yyyy}', [Parameters.Tarih])"));
            header.Controls.Add(lblFaturaBilgi);

            XRLabel lblMusteri = new XRLabel()
            {
                LocationF = new PointF(0, 130),
                SizeF = new SizeF(usableWidth, 60),
                Font = new Font("Arial", 10),
                Multiline = true
            };
            lblMusteri.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text",
                "'MÜŞTERİ:\n' + [Parameters.MusteriAdi] + '\n' + [Parameters.MusteriTipi]"));
            header.Controls.Add(lblMusteri);

            PageHeaderBand headerBand = new PageHeaderBand() { HeightF = 30 };
            XRTable headerTable = new XRTable()
            {
                LocationF = new PointF(0, 0),
                WidthF = usableWidth,
                Borders = DevExpress.XtraPrinting.BorderSide.All
            };
            XRTableRow headerRow = new XRTableRow();
            headerRow.Cells.Add(CreateHeaderCell("Ürün Adı"));
            headerRow.Cells.Add(CreateHeaderCell("Adet"));
            headerRow.Cells.Add(CreateHeaderCell("Birim Fiyat"));
            headerRow.Cells.Add(CreateHeaderCell("KDV %"));
            headerRow.Cells.Add(CreateHeaderCell("Tutar"));
            headerTable.Rows.Add(headerRow);
            headerBand.Controls.Add(headerTable);
            this.Bands.Add(headerBand);

            this.Detail.HeightF = 25;
            XRTable detailTable = new XRTable()
            {
                LocationF = new PointF(0, 0),
                WidthF = usableWidth,
                Borders = DevExpress.XtraPrinting.BorderSide.All
            };
            XRTableRow detailRow = new XRTableRow();
            detailRow.Cells.Add(CreateCell("[UrunAdi]"));
            detailRow.Cells.Add(CreateCell("[Adet]"));
            detailRow.Cells.Add(CreateCell("FormatString('{0:n2}', [BirimFiyat])"));
            detailRow.Cells.Add(CreateCell("[KdvOrani]"));
            detailRow.Cells.Add(CreateCell("FormatString('{0:n2}', [BirimFiyat] * [Adet])"));
            detailTable.Rows.Add(detailRow);
            this.Detail.Controls.Add(detailTable);

            ReportFooterBand footerBand = new ReportFooterBand() { HeightF = 50 };
            XRLabel lblToplam = new XRLabel()
            {
                Font = new Font("Arial", 12, FontStyle.Bold),
                SizeF = new SizeF(300, 30),
                LocationF = new PointF(usableWidth - 300, 10),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight
            };
            lblToplam.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text",
                "'GENEL TOPLAM ₺: ' + FormatString('{0:n2}', Sum([BirimFiyat] * [Adet]))"));
            footerBand.Controls.Add(lblToplam);
            this.Bands.Add(footerBand);
        }

        private XRTableCell CreateHeaderCell(string text)
        {
            return new XRTableCell()
            {
                Text = text,
                Font = new Font("Arial", 10, FontStyle.Bold),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter,
                BackColor = Color.LightGray
            };
        }

        private XRTableCell CreateCell(string expression)
        {
            XRTableCell cell = new XRTableCell()
            {
                Font = new Font("Arial", 10),
                TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft
            };
            cell.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", expression));
            return cell;
        }
    }

    public class InvoiceItem
    {
        public int FaturaID { get; set; }
        public string FaturaTipi { get; set; }
        public string FaturaNo { get; set; }
        public DateTime Tarih { get; set; }
        public int MusteriID { get; set; }
        public string MusteriAdi { get; set; }
        public string MusteriTipi { get; set; }
        public string UrunAdi { get; set; }
        public decimal KdvOrani { get; set; }
        public string OdemeSekli { get; set; }
        public decimal ToplamTutar { get; set; }
        public int Adet { get; set; }
        public decimal BirimFiyat { get; set; }
    }
}