using DevExpress.XtraEditors;
using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;

namespace DENEME
{
    public partial class frmGeriBildirim : DevExpress.XtraEditors.XtraForm
    {
        string connectionString = "Server=localhost;Database=stoktakip;Uid=root;Pwd=;"; 

        public frmGeriBildirim()
        {
            InitializeComponent();
            using (MemoryStream ms = new MemoryStream(Properties.Resources.favico))
            {
                this.Icon = new Icon(ms);
            }
            this.Icon = null;
        }

        private void btnGonder_Click(object sender, EventArgs e)
        {
            string kullaniciAdi = txtKullaniciAdi.Text.Trim();
            int puan = (int)ratingPuan.Rating;
            string aciklama = memoYorum.Text.Trim();

            if (string.IsNullOrEmpty(kullaniciAdi))
            {
                XtraMessageBox.Show("Lütfen kullanıcı adını giriniz.", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!KullaniciVarMi(kullaniciAdi))
            {
                XtraMessageBox.Show("Bu kullanıcı adı sistemde kayıtlı değil.", "Kullanıcı Bulunamadı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool dbBasarili = VeritabaninaKaydet(kullaniciAdi, puan, aciklama);

            bool mailBasarili = MailGonder(kullaniciAdi, puan, aciklama);

            if (dbBasarili && mailBasarili)
            {
                XtraMessageBox.Show("Geri bildiriminiz başarıyla gönderildi. Teşekkür ederiz!");
                FormuTemizle();
            }
        }

        private bool KullaniciVarMi(string kullaniciAdi)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM kullanicilar WHERE kullanici_adi = @kullaniciAdi";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@kullaniciAdi", kullaniciAdi);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("Kullanıcı kontrolü sırasında hata oluştu: " + ex.Message);
                    return false;
                }
            }
        }

        private bool VeritabaninaKaydet(string kullaniciAdi, int puan, string aciklama)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    string query = "INSERT INTO talepler (kullanici_adi, puan, aciklama) VALUES (@kullanici_adi, @puan, @aciklama)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@kullanici_adi", kullaniciAdi);
                    cmd.Parameters.AddWithValue("@puan", puan);
                    cmd.Parameters.AddWithValue("@aciklama", aciklama);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Veritabanı hatası: " + ex.Message);
                return false;
            }
        }

        private bool MailGonder(string kullaniciAdi, int puan, string aciklama)
        {
            try
            {
                MailMessage mailMesaj = new MailMessage();
                mailMesaj.From = new MailAddress("moretech.technology@gmail.com", "Geri Bildirim");
                mailMesaj.To.Add("moretech.technology@gmail.com");
                mailMesaj.Subject = $"Yeni Geri Bildirim - {kullaniciAdi}";
                mailMesaj.Body = $"Kullanıcı Adı: {kullaniciAdi}\nPuan: {puan}\nAçıklama:\n{aciklama}\n\nTarih: {DateTime.Now}";

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("moretech.technology@gmail.com", "lbzw kenx efvf jxga"); // Gmail uygulama şifresi
                smtp.EnableSsl = true;
                smtp.Send(mailMesaj);

                return true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("E-posta gönderme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }


        private void FormuTemizle()
        {
            txtKullaniciAdi.Text = "";
            ratingPuan.Rating = 0;
            memoYorum.Text = "";
        }
    }
}
