using DevExpress.XtraEditors;
using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DENEME
{
    public partial class frmTalepOlustur : DevExpress.XtraEditors.XtraForm
    {
        private string secilenDosyaYolu = "";
        private string mysqlConnectionString = "server=localhost;database=stoktakip;uid=root;pwd=;";

        public frmTalepOlustur()
        {
            InitializeComponent();
            //this.Icon = new Icon("Resources/favico.ico"); // tam yol veya resource kullanımıyla
            this.Load += frmTalepOlustur_Load;
        }

        private void frmTalepOlustur_Load(object sender, EventArgs e)
        {
            comboTalepTuru.Properties.Items.Clear();
            comboTalepTuru.Properties.Items.AddRange(new string[] { "Ürün ekleyemiyorum", "Barkod çalışmıyor", "Rapor hatası", "Stok sayısı yanlış", "Diğer" });
            comboTalepTuru.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
        }

        private bool KullaniciVarMi(string kullaniciAdi)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(mysqlConnectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM kullanicilar WHERE kullanici_adi = @kullaniciAdi";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@kullaniciAdi", kullaniciAdi);

                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Kullanıcı kontrolü sırasında hata oluştu: " + ex.Message);
                return false;
            }
        }

        private bool EmailKontrol(string eposta)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(eposta, pattern);
        }

        private void btnDosyaSec_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    secilenDosyaYolu = ofd.FileName;
                    lblDosya.Text = Path.GetFileName(secilenDosyaYolu);
                }
            }
        }

        private void btnTalepOlustur_Click(object sender, EventArgs e)
        {
            string adSoyad = txtAdSoyad.Text.Trim();
            string kullaniciAdi = txtKullaniciAdi.Text.Trim();
            string eposta = txtIletisim.Text.Trim();
            string talepTuru = comboTalepTuru.Text;
            string aciklama = memoAciklama.Text.Trim();

            if (string.IsNullOrWhiteSpace(adSoyad) || string.IsNullOrWhiteSpace(kullaniciAdi) ||
                string.IsNullOrWhiteSpace(eposta) || string.IsNullOrWhiteSpace(talepTuru) || string.IsNullOrWhiteSpace(aciklama))
            {
                XtraMessageBox.Show("Lütfen tüm alanları doldurun.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!EmailKontrol(eposta))
            {
                XtraMessageBox.Show("Geçerli bir e-posta adresi giriniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!KullaniciVarMi(kullaniciAdi))
            {
                XtraMessageBox.Show("Bu kullanıcı adı sistemde kayıtlı değil.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                MailMessage mailMesaj = new MailMessage();
                mailMesaj.From = new MailAddress("moretech.technology@gmail.com", "Destek Talebi");
                mailMesaj.To.Add("moretech.technology@gmail.com");
                mailMesaj.Subject = $"Destek Talebi - {talepTuru}";
                mailMesaj.Body = $"Gönderen: {adSoyad} ({eposta})\n" +
                                 $"Kullanıcı Adı: {kullaniciAdi}\n" +
                                 $"Kategori: {talepTuru}\n" +
                                 $"Detay:\n{aciklama}\n\nTarih: {DateTime.Now}";

                if (!string.IsNullOrEmpty(secilenDosyaYolu) && File.Exists(secilenDosyaYolu))
                    mailMesaj.Attachments.Add(new Attachment(secilenDosyaYolu));

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("moretech.technology@gmail.com", "lbzw kenx efvf jxga"),
                    EnableSsl = true
                };

                smtp.Send(mailMesaj);

                // Onay maili gönder
                MailMessage onayMail = new MailMessage();
                onayMail.From = new MailAddress("moretech.technology@gmail.com", "Destek Ekibi");
                onayMail.To.Add(eposta);
                onayMail.Subject = "Destek Talebi Alındı";
                onayMail.Body = $"Merhaba {adSoyad},\n\n" +
                                $"Destek talebiniz alınmıştır. En kısa sürede size dönüş yapılacaktır.\n\n" +
                                $"Kategori: {talepTuru}\nTarih: {DateTime.Now}\n\n" +
                                $"İyi günler dileriz.\n- Destek Ekibi";

                smtp.Send(onayMail);

                // Veritabanına kayıt
                KaydetMySQL(adSoyad, kullaniciAdi, eposta, talepTuru, aciklama, secilenDosyaYolu);

                XtraMessageBox.Show("Talebiniz başarıyla gönderildi. E-posta adresinize bilgilendirme maili gönderildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                TemizleForm();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Mail gönderilemedi: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void KaydetMySQL(string adSoyad, string kullaniciAdi, string eposta, string talepTuru, string aciklama, string dosyaYolu)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(mysqlConnectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO talepler 
                        (ad_soyad, kullanici_adi, eposta, talep_turu, aciklama, dosya_yolu, tarih)
                        VALUES (@adSoyad, @kullaniciAdi, @eposta, @talepTuru, @aciklama, @dosyaYolu, NOW())";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@adSoyad", adSoyad);
                    cmd.Parameters.AddWithValue("@kullaniciAdi", kullaniciAdi);
                    cmd.Parameters.AddWithValue("@eposta", eposta);
                    cmd.Parameters.AddWithValue("@talepTuru", talepTuru);
                    cmd.Parameters.AddWithValue("@aciklama", aciklama);
                    cmd.Parameters.AddWithValue("@dosyaYolu", string.IsNullOrEmpty(dosyaYolu) ? DBNull.Value : (object)dosyaYolu);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Veritabanı kaydı sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TemizleForm()
        {
            txtAdSoyad.Text = "";
            txtKullaniciAdi.Text = "";
            txtIletisim.Text = "";
            comboTalepTuru.SelectedIndex = -1;
            memoAciklama.Text = "";
            lblDosya.Text = "";
            secilenDosyaYolu = "";
        }      


        private void btnDosyaEkle_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    secilenDosyaYolu = ofd.FileName;

                    Control[] lbls = this.Controls.Find("lblDosya", true);
                    if (lbls.Length > 0 && lbls[0] is Label lbl)
                    {
                        lbl.Text = Path.GetFileName(secilenDosyaYolu);
                    }
                }
            }
        }

    }
}
