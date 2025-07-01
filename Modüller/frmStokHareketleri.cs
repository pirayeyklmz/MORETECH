using DevExpress.CodeParser;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace MORETECH
{
    public partial class frmStokHareketleri : DevExpress.XtraEditors.XtraForm
    {
        string cnnStr = "Server=localhost;Database=stoktakip;Uid=root;Pwd=;SslMode=none;Convert Zero Datetime=True;Allow Zero Datetime=True;";

        public frmStokHareketleri()
        {
            InitializeComponent();

            // Uygulama genelinde Türkçe tarih/saat formatı (gün/ay/yıl)
            CultureInfo culture = new CultureInfo("tr-TR");
            culture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            culture.DateTimeFormat.LongTimePattern = "HH:mm:ss";
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            this.Activated += frmStokHareketleri_Activated;

            // Grid görünümünde tarih sütununu net şekilde biçimlendir
            gridViewStokHareket.CustomColumnDisplayText += (s, e) =>
            {
                if (e.Column.FieldName == "hareket_tarihi" && e.Value != null && e.Value != DBNull.Value)
                {
                    try
                    {
                        DateTime tarih = Convert.ToDateTime(e.Value);
                        e.DisplayText = tarih.ToString("dd/MM/yyyy HH:mm:ss"); // gün/ay/yıl
                    }
                    catch
                    {
                        e.DisplayText = e.Value.ToString();
                    }
                }
            };

            StokHareketleriniGetir();
        }

        private void StokHareketleriniGetir()
        {
            string sql = @"
SELECT  
    u.urun_id,
    u.urun_ad,
    s.satis_id          AS hareket_id,
    'CIKIS'             AS hareket_turu,
    s.satis_adet        AS adet,
    s.satis_tarih       AS hareket_tarihi,
    CONCAT('Satış - Müşteri: ', m.musteri_ad, ' ', m.musteri_soyad) AS aciklama
FROM satislar s
JOIN urunler    u ON u.urun_id    = s.urun_id
JOIN musteriler m ON m.musteri_id = s.musteri_id

UNION ALL

SELECT  
    u.urun_id,
    u.urun_ad,
    s.satis_id          AS hareket_id,
    'GIRIS'             AS hareket_turu,
    s.iade_adet         AS adet,
    s.satis_tarih       AS hareket_tarihi,
    CONCAT('İade - Müşteri: ', m.musteri_ad, ' ', m.musteri_soyad) AS aciklama
FROM satislar s
JOIN urunler    u ON u.urun_id    = s.urun_id
JOIN musteriler m ON m.musteri_id = s.musteri_id
WHERE s.iade_adet > 0
ORDER BY hareket_tarihi DESC;";

            DataTable dt = new();

            // 1) Veriyi oku
            using (var cnn = new MySqlConnection(cnnStr))
            {
                cnn.Open();
                using var cmd = new MySqlCommand(sql, cnn);
                using var adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
            }

            // 2) stok_hareketleri tablosuna yaz
            using (var insertConn = new MySqlConnection(cnnStr))
            {
                insertConn.Open();

                foreach (DataRow row in dt.Rows)
                {
                    using var insertCmd = new MySqlCommand(@"
INSERT INTO stok_hareketleri (urun_id, hareket_turu, adet, hareket_tarihi, aciklama)
VALUES (@urun_id, @hareket_turu, @adet, @hareket_tarihi, @aciklama);", insertConn);

                    insertCmd.Parameters.AddWithValue("@urun_id", row["urun_id"]);
                    insertCmd.Parameters.AddWithValue("@hareket_turu", row["hareket_turu"]);
                    insertCmd.Parameters.AddWithValue("@adet", row["adet"]);
                    insertCmd.Parameters.AddWithValue("@hareket_tarihi", Convert.ToDateTime(row["hareket_tarihi"]));
                    insertCmd.Parameters.AddWithValue("@aciklama", row["aciklama"]);

                    insertCmd.ExecuteNonQuery();
                }
            }

            // 3) Grid'e bağla
            gridControlStokHareket.DataSource = dt;
            gridViewStokHareket.PopulateColumns();

            gridViewStokHareket.Columns["urun_id"].Caption = "Ürün ID";
            gridViewStokHareket.Columns["urun_ad"].Caption = "Ürün Adı";
            gridViewStokHareket.Columns["hareket_id"].Caption = "Hareket ID";
            gridViewStokHareket.Columns["hareket_turu"].Caption = "Hareket Türü";
            gridViewStokHareket.Columns["adet"].Caption = "Adet";
            gridViewStokHareket.Columns["hareket_tarihi"].Caption = "Tarih";
            gridViewStokHareket.Columns["aciklama"].Caption = "Açıklama";

            // Formatı destekle (ama asıl biçim CustomColumnDisplayText ile olur)
            gridViewStokHareket.Columns["hareket_tarihi"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            gridViewStokHareket.Columns["hareket_tarihi"].DisplayFormat.FormatString = "dd/MM/yyyy HH:mm:ss";

            gridViewStokHareket.BestFitColumns();
            gridViewStokHareket.RefreshData();
        }

        private void frmStokHareketleri_Activated(object sender, EventArgs e)
        {
            StokHareketleriniGetir();
        }
    }
}