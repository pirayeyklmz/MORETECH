using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Drawing;

namespace MORETECH
{
    public partial class frmMusteriListesi : DevExpress.XtraEditors.XtraForm
    {

        public class Musteri
        {
            public string Ad { get; set; }
            public string Soyad { get; set; }
            public string Sirket { get; set; }
            public string AdSoyadSirket => $"{Ad} {Soyad} - {Sirket}";
        }

        public frmMusteriListesi()
        {
            InitializeComponent();
            this.Activated += frmMusteriListesi_Activated;
            this.listBoxControl1.SelectedIndexChanged += new System.EventHandler(this.listBoxControl1_SelectedIndexChanged);
        }

        List<Musteri> musteriler = new List<Musteri>();
        private void frmMusteriListesi_Activated(object sender, EventArgs e)
        {
            // Form odaklandığında listeyi güncelleyin:
            MusteriListesiniYukle();
        }

        private void MusteriListesiniYukle()
        {
            musteriler.Clear();

            string connStr = "Server=localhost;Database=stoktakip;Uid=root;Pwd=;";
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT musteri_ad, musteri_soyad, musteri_sirket FROM musteriler", conn);
                MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    musteriler.Add(new Musteri
                    {
                        Ad = dr["musteri_ad"].ToString(),
                        Soyad = dr["musteri_soyad"].ToString(),
                        Sirket = dr["musteri_sirket"].ToString()
                    });
                }
            }

            listBoxControl1.DataSource = null;
            listBoxControl1.DataSource = musteriler;
            listBoxControl1.DisplayMember = "AdSoyadSirket";
        }


        private void frmMusteriListesi_Load(object sender, EventArgs e)
            
        {
            MusteriListesiniYukle();

            string connStr = "Server=localhost;Database=stoktakip;Uid=root;Pwd=;";
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT musteri_ad, musteri_soyad, musteri_sirket FROM musteriler", conn);
                MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    musteriler.Add(new Musteri
                    {
                        Ad = dr["musteri_ad"].ToString(),
                        Soyad = dr["musteri_soyad"].ToString(),
                        Sirket = dr["musteri_sirket"].ToString()
                    });
                }
            }

            listBoxControl1.DataSource = musteriler;
            listBoxControl1.DisplayMember = "AdSoyadSirket";

            MusteriListesiniYukle();


            txtEMusteriMaili.Properties.ContextImageOptions.SvgImageSize = new Size(16, 16);
            txtEMusteriTeli.Properties.ContextImageOptions.SvgImageSize = new Size(16, 16); ;
        }

        private void listBoxControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxControl1.SelectedItem is Musteri secilen)
            {
                string connStr = "Server=localhost;Database=stoktakip;Uid=root;Pwd=;";
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string sql = "SELECT * FROM musteriler WHERE musteri_ad = @ad AND musteri_soyad = @soyad AND musteri_sirket = @sirket";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@ad", secilen.Ad);
                    cmd.Parameters.AddWithValue("@soyad", secilen.Soyad);
                    cmd.Parameters.AddWithValue("@sirket", secilen.Sirket);

                    MySqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {

                        txtMusteriID.Text = dr["musteri_id"].ToString();
                        txtMusteriAdi.Text = dr["musteri_ad"].ToString();
                        txtMusteriSoyadi.Text = dr["musteri_soyad"].ToString();
                        txtMusteriTipi.Text = dr["musteri_tip"].ToString();
                        txtMusteriSirketi.Text = dr["musteri_sirket"].ToString();
                        txtMusteriVergiD.Text = dr["musteri_vergi"].ToString();
                        txtMusteriIli.Text = dr["musteri_il"].ToString();
                        txtMusteriIlcesi.Text = dr["musteri_ilce"].ToString();
                        txtMusteriAdresi.Text = dr["musteri_adres"].ToString();


                        txtEMusteriMaili.Text = dr["musteri_mail"].ToString();
                        txtEMusteriTeli.Text = dr["musteri_tel"].ToString();
                    }
                }
            }
        }
    }


}