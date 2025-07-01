using DENEME;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace StokTakip.Kullanicilar
{
    public partial class FrmKayit : DevExpress.XtraEditors.XtraForm
    {
        public FrmKayit()
        {
            InitializeComponent();

            string iconPath = Path.Combine(Application.StartupPath, "favico.ico");
            if (File.Exists(iconPath))
            {
                this.Icon = new Icon(iconPath);
            }
            //this.Icon = null;

            this.AcceptButton = btnKaydet;

        }

        private void FrmKayit_Load(object sender, EventArgs e)
        {
            txtSifre.Properties.UseSystemPasswordChar = true;
            txtSifreTekrari.Properties.UseSystemPasswordChar = true;
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            string kullaniciAdi = txtIsim.Text.Trim();
            string sifre = txtSifre.Text;
            string sifreTekrar = txtSifreTekrari.Text;

            if (string.IsNullOrEmpty(kullaniciAdi) || string.IsNullOrEmpty(sifre) || string.IsNullOrEmpty(sifreTekrar))
            {
                XtraMessageBox.Show("Tüm alanları doldurunuz.");
                return;
            }

            if (sifre != sifreTekrar)
            {
                XtraMessageBox.Show("Şifreler uyuşmuyor.");
                return;
            }

            // 🔒 Şifre güvenlik kontrolü
            int harfSayisi = sifre.Count(char.IsLetter);
            int rakamSayisi = sifre.Count(char.IsDigit);
            int ozelKarakterSayisi = sifre.Count(c => !char.IsLetterOrDigit(c));

            if (sifre.Length < 8 || harfSayisi < 2 || rakamSayisi < 2 || ozelKarakterSayisi < 1)
            {
                XtraMessageBox.Show("Şifre en az 8 karakter uzunluğunda, en az 2 harf, 2 rakam ve 1 özel karakter içermelidir.",
                    "Geçersiz Şifre", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string sifreHash = Sha256Hashle(sifre);

            string connectionString = "server=localhost;user id=root;password=;database=stoktakip;SslMode=none;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "INSERT INTO kullanicilar (kullanici_adi, sifre_hash, rol) VALUES (@kullanici_adi, @sifre_hash, 'admin')";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@kullanici_adi", kullaniciAdi);
                    cmd.Parameters.AddWithValue("@sifre_hash", sifreHash);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        XtraMessageBox.Show("Kayıt başarılı. Giriş ekranına yönlendiriliyorsunuz...");

                        Program.AktifKullaniciAdi = kullaniciAdi;
                        Program.AktifKullaniciRol = "SÜPER ADMİN";
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    catch (MySqlException ex)
                    {
                        XtraMessageBox.Show("Hata: " + ex.Message);
                    }
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
            txtSifreTekrari.Text = "";
        }

        private void checkBoxSifreGoster_CheckedChanged(object sender, EventArgs e)
        {
            bool sifreGosterilsin = checkBoxSifreGoster.Checked;

            txtSifre.Properties.UseSystemPasswordChar = !sifreGosterilsin;
            txtSifreTekrari.Properties.UseSystemPasswordChar = !sifreGosterilsin;
        }

    }
}