using DENEME;
using DevExpress.XtraEditors;
using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace DENEME.Kullanicilar
{
    public partial class FrmGiris : DevExpress.XtraEditors.XtraForm
    {
        public FrmGiris()
        {
            try
            {
                InitializeComponent();

                using (MemoryStream ms = new MemoryStream(Properties.Resources.favico))
                {
                    this.Icon = new Icon(ms);
                }
             
                this.AcceptButton = btnKaydet;
            }
            catch (Exception ex)
            {
                MessageBox.Show("FrmGiris başlatılırken hata oluştu:\n\n" + ex.ToString(),
                                "Başlatma Hatası",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void FrmGiris_Load(object sender, EventArgs e)
        {
            try
            {
                txtSifre.Properties.UseSystemPasswordChar = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("FrmGiris yüklenirken hata oluştu:\n\n" + ex.ToString(),
                                "Yükleme Hatası",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            string kullaniciAdi = txtIsim.Text.Trim();
            string sifre = txtSifre.Text;

            if (string.IsNullOrEmpty(kullaniciAdi) || string.IsNullOrEmpty(sifre))
            {
                XtraMessageBox.Show("Kullanıcı adı ve şifre boş olamaz.");
                return;
            }

            string sifreHash = Sha256Hashle(sifre);
            string connectionString = "server=localhost;user id=root;password=;database=stoktakip;SslMode=none;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT rol FROM kullanicilar WHERE kullanici_adi = @kullanici_adi AND sifre_hash = @sifre_hash";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@kullanici_adi", kullaniciAdi);
                        cmd.Parameters.AddWithValue("@sifre_hash", sifreHash);

                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            string kullaniciRolu = result.ToString();

                            Program.AktifKullaniciAdi = kullaniciAdi;
                            Program.AktifKullaniciRol = kullaniciRolu;

                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            XtraMessageBox.Show("Kullanıcı adı veya şifre hatalı.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("Veritabanı bağlantı hatası: " + ex.Message);
                }
            }
        }

        private string Sha256Hashle(string veri)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(veri);
                byte[] hash = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        private void btnTemizle_Click(object sender, EventArgs e)
        {
            txtIsim.Text = "";
            txtSifre.Text = "";
        }

        private void checkBoxSifreGoster_CheckedChanged(object sender, EventArgs e)
        {
            bool sifreGosterilsin = checkBoxSifreGoster.Checked;
            txtSifre.Properties.UseSystemPasswordChar = !sifreGosterilsin;
        }
    }
}