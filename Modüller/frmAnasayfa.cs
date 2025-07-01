using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace DENEME
{
    public partial class frmAnasayfa : XtraForm
    {
        string cnnStr = "Server=localhost;Database=stoktakip;Uid=root;Pwd=;Allow Zero Datetime=True;Convert Zero Datetime=True;";

        public frmAnasayfa()
        {
            InitializeComponent();
            this.Load += Anasayfa_Load;
        }

        public DataTable GetUrunBazli()
        {
            DataTable dt = new DataTable();
            string query = @"
                SELECT u.urun_ad, SUM(s.satis_adet) AS urun_adet
                FROM satislar s
                JOIN urunler u ON s.urun_id = u.urun_id
                WHERE MONTH(s.satis_tarih) = MONTH(CURDATE()) 
                AND YEAR(s.satis_tarih) = YEAR(CURDATE())
                GROUP BY u.urun_ad;";

            using (MySqlConnection cnn = new MySqlConnection(cnnStr))
            {
                cnn.Open();
                using (MySqlDataAdapter da = new MySqlDataAdapter(query, cnn))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public DataTable GetMusteriBazli()
        {
            DataTable dt = new DataTable();
            string query = @"
                SELECT CONCAT(m.musteri_ad, ' ', m.musteri_soyad) AS musteri_ad, SUM(s.satis_adet) AS satis_adet
                FROM satislar s
                JOIN musteriler m ON s.musteri_id = m.musteri_id
                WHERE MONTH(s.satis_tarih) = MONTH(CURDATE()) 
                AND YEAR(s.satis_tarih) = YEAR(CURDATE())
                GROUP BY musteri_ad;";

            using (MySqlConnection cnn = new MySqlConnection(cnnStr))
            {
                cnn.Open();
                using (MySqlDataAdapter da = new MySqlDataAdapter(query, cnn))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        private void Anasayfa_Load(object sender, EventArgs e)
        {
            // Ürün Bazlı Grafik
            chartUrunBazli.Series.Clear();
            DataTable dtUrun = GetUrunBazli();

            Series seriesUrun = new Series("Ürün Satışı", ViewType.Bar)
            {
                LabelsVisibility = DevExpress.Utils.DefaultBoolean.True,
                LegendTextPattern = "{A}: {V}"
            };

            foreach (DataRow row in dtUrun.Rows)
            {
                string urunAd = row["urun_ad"].ToString();
                double adet = Convert.ToDouble(row["urun_adet"]);
                seriesUrun.Points.Add(new SeriesPoint(urunAd, adet));
            }

            chartUrunBazli.Series.Add(seriesUrun);

            // Müşteri Bazlı Grafik
            chartMusteriBazli.Series.Clear();
            DataTable dtMusteri = GetMusteriBazli();

            Series seriesMusteri = new Series("Müşteri Satışı", ViewType.Pie)
            {
                LabelsVisibility = DevExpress.Utils.DefaultBoolean.True,
                LegendTextPattern = "{A}: {V}",
                View = new PieSeriesView()
            };

            foreach (DataRow row in dtMusteri.Rows)
            {
                string musteriAd = row["musteri_ad"].ToString();
                double adet = Convert.ToDouble(row["satis_adet"]);
                seriesMusteri.Points.Add(new SeriesPoint(musteriAd, adet));
            }

            chartMusteriBazli.Series.Add(seriesMusteri);
        }
    }
}
