using DevExpress.XtraEditors;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DENEME
{
    public partial class frmUrunlerListesi : DevExpress.XtraEditors.XtraForm
    {
        private readonly string cnnStr = "Server=localhost;Database=stoktakip;Uid=root;Pwd=;";
        List<Urun> urunler = new List<Urun>();

        public frmUrunlerListesi()
        {
            InitializeComponent();
            this.Activated += FrmUrunlerListesi_Activated;
            this.listBoxControl1.SelectedIndexChanged += new EventHandler(this.listBoxControl1_SelectedIndexChanged);
        }

        public void ListeyiYenile()
        {
            urunler.Clear();
            using (MySqlConnection cnn = new MySqlConnection(cnnStr))
            {
                cnn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT urun_id, urun_ad, urun_barkod FROM urunler", cnn);
                MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    urunler.Add(new Urun
                    {
                        Id = Convert.ToInt32(dr["urun_id"]),
                        Ad = dr["urun_ad"].ToString(),
                        Barkod = dr["urun_barkod"].ToString()
                    });
                }
            }

            listBoxControl1.DataSource = null;
            listBoxControl1.DataSource = urunler;
            listBoxControl1.DisplayMember = "AdBarkod";
            listBoxControl1.ValueMember = "Id";
        }

        private void FrmUrunlerListesi_Activated(object sender, EventArgs e)
        {
            ListeyiYenile();
        }

        public void FotoYukleVeAyarla(Image img)
        {
            pictureEdit1.Image = img;
            pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
        }

        private void frmUrunlerListesi_Load(object sender, EventArgs e)
        {

            urunler.Clear();
            ListeyiYenile();
            using (MySqlConnection cnn = new MySqlConnection(cnnStr))
            {
                cnn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT urun_id, urun_ad, urun_barkod FROM urunler", cnn);
                MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    urunler.Add(new Urun
                    {
                        Id = Convert.ToInt32(dr["urun_id"]),
                        Ad = dr["urun_ad"].ToString(),
                        Barkod = dr["urun_barkod"].ToString()
                    });
                }
            }

            listBoxControl1.DataSource = urunler;
            listBoxControl1.DisplayMember = "AdBarkod";
            listBoxControl1.ValueMember = "Id";
        }

        private void listBoxControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxControl1.SelectedItem is Urun secilen)
            {
                using (MySqlConnection cnn = new MySqlConnection(cnnStr))
                {
                    cnn.Open();
                    string sql = "SELECT * FROM urunler WHERE urun_id = @id";
                    MySqlCommand cmd = new MySqlCommand(sql, cnn);
                    cmd.Parameters.AddWithValue("@id", secilen.Id);

                    MySqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        txtUrunID.Text = dr["urun_id"].ToString();
                        txtUrunBarkod.Text = dr["urun_barkod"].ToString();
                        txtUrunAdi.Text = dr["urun_ad"].ToString();
                        txtUrunAdedi.Text = dr["urun_adet"].ToString();
                        txtUrunAlisFiyati.Text = dr["urun_alisfiyat"].ToString();
                        txtUrunSatisFiyati.Text = dr["urun_satisfiyat"].ToString();
                        txtUrunDetayi.Text = dr["urun_detay"].ToString();
                    }
                }

                UrunYukle(secilen.Id);
            }
        }

        public void UrunYukle(int urunId)
        {
            using var cnn = new MySqlConnection(cnnStr);
            cnn.Open();

            var cmd = new MySqlCommand("SELECT urunFoto FROM urunler WHERE urun_id = @id", cnn);
            cmd.Parameters.AddWithValue("@id", urunId);

            var result = cmd.ExecuteScalar();

            if (result != null && result != DBNull.Value)
            {
                byte[] resimBytes = (byte[])result;
                using var ms = new MemoryStream(resimBytes);

                pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
                pictureEdit1.Image = Image.FromStream(ms);
            }
            else
            {
                pictureEdit1.Image = null;
            }
        }
    }

    public class Urun
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Barkod { get; set; }
        public string AdBarkod => $"{Ad} - {Barkod}";
    }
}
