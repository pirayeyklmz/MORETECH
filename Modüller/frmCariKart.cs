using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using MySql.Data.MySqlClient;

namespace MORETECH
{
    public partial class FrmCariKart : DevExpress.XtraEditors.XtraForm
    {
        private readonly string cnnStr = "Server=localhost;Database=stoktakip;Uid=root;Pwd=;SslMode=none;Connection Timeout=30;Charset=utf8;";

        public FrmCariKart()
        {
            InitializeComponent();
            this.Activated += FrmCariKart_Activated;
        }

        private void FrmCariKart_Activated(object sender, EventArgs e)
        {
            TumFaturalariGetir();
        }

        private void TumFaturalariGetir()
        {


            using (MySqlConnection cnn = new MySqlConnection(cnnStr))
            {
                string query = @"
SELECT 
    f.fatura_no AS 'FATURA NO',
    f.tarih AS 'FATURA TARİHİ',
    f.fatura_tipi AS 'FATURA TIPI',
    CONCAT(m.musteri_ad, ' ', m.musteri_soyad) AS 'MÜŞTERİ ADI',
    m.musteri_tip AS 'MÜŞTERİ TİPİ',
    f.odeme_sekli AS 'ÖDEME ŞEKLİ',
    f.kdv AS 'KDV',
    f.toplam_tutar AS 'TUTAR',
    f.fatura_id AS 'FATURA ID',
    f.aciklama AS 'AÇIKLAMA'
FROM faturalar f
LEFT JOIN musteriler m ON f.musteri_id = m.musteri_id   
ORDER BY f.tarih DESC";


                MySqlDataAdapter da = new MySqlDataAdapter(query, cnn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gridControl1.DataSource = dt;
                gridControl1.MainView = gridView1;
                gridView1.PopulateColumns();

                decimal borc = 0, alacak = 0;
                foreach (DataRow row in dt.Rows)
                {
                    decimal tutar = Convert.ToDecimal(row["TUTAR"]);
                    string faturaTipi = row["FATURA TIPI"].ToString();

                    if (faturaTipi == "Satış") alacak += tutar;
                    else borc += tutar;
                }
                txtBorc.Text = borc.ToString("C2");
                txtAlacak.Text = alacak.ToString("C2");
                txtBakiye.Text = (alacak - borc).ToString("C2");
            }

        }


        private void FrmCariKart_Load(object sender, EventArgs e)
        {
            TumFaturalariGetir();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (gridView1.GetSelectedRows().Length == 0)
            {
                MessageBox.Show("Lütfen açıklama eklemek için bir fatura seçin.");
                return;
            }

            int selectedRowHandle = gridView1.GetSelectedRows()[0];
            int faturaId = Convert.ToInt32(gridView1.GetRowCellValue(selectedRowHandle, "FATURA ID"));

            string yeniAciklama = txtAciklama.Text.Trim();

            if (string.IsNullOrEmpty(yeniAciklama))
            {
                MessageBox.Show("Açıklama alanı boş olamaz.");
                return;
            }

            string cnnStr = "Server=localhost;Database=stoktakip;Uid=root;Pwd=;"; 
            using (MySqlConnection cnn = new MySqlConnection(cnnStr))
            {
                cnn.Open();

                string updateQuery = "UPDATE faturalar SET aciklama = @aciklama WHERE fatura_id = @fatura_id";

                using (MySqlCommand cmd = new MySqlCommand(updateQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@aciklama", yeniAciklama);
                    cmd.Parameters.AddWithValue("@fatura_id", faturaId);

                    int sonuc = cmd.ExecuteNonQuery();

                    if (sonuc > 0)
                    {
                        MessageBox.Show("Açıklama başarıyla eklendi.");
                        TumFaturalariGetir(); // Tabloyu yenile
                        txtAciklama.Text = ""; // Açıklama kutusunu temizle
                    }
                    else
                    {
                        MessageBox.Show("Açıklama eklenemedi.");
                    }
                }
            }
        }
    }
}