using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;

namespace StokTakip.Kullanicilar
{
    public partial class FrmKullanicilar : DevExpress.XtraEditors.XtraForm
    {
        string connectionString = "server=localhost;database=stoktakip;uid=root;pwd=;";
        public FrmKullanicilar()
        {
            InitializeComponent();
            this.AcceptButton = BtnKullaniciKaydet;

            gridView1.FocusedRowChanged += gridView1_FocusedRowChanged;
            gridView1.CellValueChanged += gridView1_CellValueChanged;
        }

        private void FrmKullanicilar_Load(object sender, EventArgs e)
        {
            KullaniciListele();
            BtnKullaniciTemizle.PerformClick();

            comboBoxEditKullaniciRol.Properties.Items.Clear();
            comboBoxEditKullaniciRol.Properties.Items.Add("admin");
            comboBoxEditKullaniciRol.Properties.Items.Add("kullanici");
        }

        private void KullaniciListele()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM kullanicilar";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gridControl1.DataSource = dt;

                    gridView1.Columns.Clear();
                    gridView1.Columns.AddVisible("id", "Kullanıcı ID");
                    gridView1.Columns.AddVisible("kullanici_adi", "Kullanıcı Adı");
                    gridView1.Columns.AddVisible("sifre_hash", "Kullanıcı Şifresi");
                    gridView1.Columns.AddVisible("rol", "Kullanıcı Rolü");
                    gridView1.Columns.AddVisible("notlar", "Kullanıcı Hakkında Not");

                    gridView1.BestFitColumns();

                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Listeleme hatası: " + ex.Message);
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

        private bool SifreGecerliMi(string sifre)
        {
            if (sifre.Length < 8)
                return false;

            int harfSayisi = sifre.Count(char.IsLetter);
            int rakamSayisi = sifre.Count(char.IsDigit);
            int ozelKarakterSayisi = sifre.Count(c => !char.IsLetterOrDigit(c));

            return harfSayisi >= 2 && rakamSayisi >= 2 && ozelKarakterSayisi >= 1;
        }

        private void BtnKullaniciKaydet_Click(object sender, EventArgs e)
        {
            string ad = txtKullaniciAd.Text.Trim();
            string sifre = txtKullaniciSifre.Text;
            string rol = comboBoxEditKullaniciRol.Text;
            string notlar = memoEditNot.Text;

            if (string.IsNullOrEmpty(ad) || string.IsNullOrEmpty(sifre))
            {
                XtraMessageBox.Show("Kullanıcı adı ve şifre boş olamaz.");
                return;
            }

            if (!SifreGecerliMi(sifre))
            {
                XtraMessageBox.Show("Şifre en az 8 karakter, en az 2 harf, 2 rakam ve 1 özel karakter içermelidir.");
                return;
            }

            string sifreHash = Sha256Hashle(sifre);

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                if (rol == "admin")
                {
                    MySqlCommand adminKontrolCmd = new MySqlCommand("SELECT COUNT(*) FROM kullanicilar WHERE rol = 'admin'", conn);
                    int adminSayisi = Convert.ToInt32(adminKontrolCmd.ExecuteScalar());

                    if (adminSayisi >= 1)
                    {
                        XtraMessageBox.Show("Sistemde yalnızca bir adet admin olabilir.");
                        return;
                    }
                }

                MySqlCommand kontrolCmd = new MySqlCommand("SELECT COUNT(*) FROM kullanicilar WHERE kullanici_adi = @ad", conn);
                kontrolCmd.Parameters.AddWithValue("@ad", ad);
                int varMi = Convert.ToInt32(kontrolCmd.ExecuteScalar());

                if (varMi > 0)
                {
                    XtraMessageBox.Show("Bu kullanıcı adı zaten mevcut. Lütfen farklı bir ad seçin.");
                    return;
                }

                MySqlCommand cmd = new MySqlCommand("INSERT INTO kullanicilar (kullanici_adi, sifre_hash, rol, notlar) VALUES (@ad, @sifre, @rol, @notlar)", conn);
                cmd.Parameters.AddWithValue("@ad", ad);
                cmd.Parameters.AddWithValue("@sifre", sifreHash);
                cmd.Parameters.AddWithValue("@rol", rol);
                cmd.Parameters.AddWithValue("@notlar", notlar);
                cmd.ExecuteNonQuery();
            }

            KullaniciListele();
            Temizle();
            XtraMessageBox.Show("Kullanıcı kaydedildi.");
        }

        private void BtnKullaniciGuncelle_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtKullaniciId.Text))
            {
                XtraMessageBox.Show("Lütfen bir kullanıcı seçin.");
                return;
            }

            string ad = txtKullaniciAd.Text.Trim();
            string sifre = txtKullaniciSifre.Text;
            string rol = comboBoxEditKullaniciRol.Text;
            string notlar = memoEditNot.Text;
            int id = int.Parse(txtKullaniciId.Text);

            if (!SifreGecerliMi(sifre))
            {
                XtraMessageBox.Show("Şifre en az 8 karakter, en az 2 harf, 2 rakam ve 1 özel karakter içermelidir.");
                return;
            }

            string sifreHash = Sha256Hashle(sifre);

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                if (rol == "admin")
                {
                    MySqlCommand adminKontrolCmd = new MySqlCommand("SELECT COUNT(*) FROM kullanicilar WHERE rol = 'admin' AND id != @id", conn);
                    adminKontrolCmd.Parameters.AddWithValue("@id", id);
                    int digerAdminSayisi = Convert.ToInt32(adminKontrolCmd.ExecuteScalar());

                    if (digerAdminSayisi >= 1)
                    {
                        XtraMessageBox.Show("Zaten bir admin mevcut. Başka bir kullanıcı admin yapılamaz.");
                        return;
                    }
                }

                MySqlCommand kontrolCmd = new MySqlCommand("SELECT COUNT(*) FROM kullanicilar WHERE kullanici_adi = @ad AND id != @id", conn);
                kontrolCmd.Parameters.AddWithValue("@ad", ad);
                kontrolCmd.Parameters.AddWithValue("@id", id);
                int varMi = Convert.ToInt32(kontrolCmd.ExecuteScalar());

                if (varMi > 0)
                {
                    XtraMessageBox.Show("Bu kullanıcı adı başka bir kullanıcıya ait. Lütfen farklı bir ad seçin.");
                    return;
                }

                MySqlCommand cmd = new MySqlCommand("UPDATE kullanicilar SET kullanici_adi=@ad, sifre_hash=@sifre, rol=@rol, notlar=@notlar WHERE id=@id", conn);
                cmd.Parameters.AddWithValue("@ad", ad);
                cmd.Parameters.AddWithValue("@sifre", sifreHash);
                cmd.Parameters.AddWithValue("@rol", rol);
                cmd.Parameters.AddWithValue("@notlar", notlar);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }

            KullaniciListele();
            Temizle();
            XtraMessageBox.Show("Kullanıcı güncellendi.");
        }



        private void BtnKullaniciSil_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtKullaniciId.Text))
            {
                XtraMessageBox.Show("Lütfen bir kullanıcı seçin.");
                return;
            }

            int id = int.Parse(txtKullaniciId.Text);

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM kullanicilar WHERE id=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }

            KullaniciListele();
            Temizle();
            XtraMessageBox.Show("Kullanıcı silindi.");
        }

        private void BtnKullaniciTemizle_Click(object sender, EventArgs e)
        {
            Temizle();
        }

        private void Temizle()
        {
            txtKullaniciId.Text = string.Empty;
            txtKullaniciAd.Text = string.Empty;
            txtKullaniciSifre.Text = string.Empty;
            memoEditNot.Text = string.Empty;
            comboBoxEditKullaniciRol.Text = string.Empty;

            txtKullaniciId.Focus();
        }

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            txtKullaniciId.Text = gridView1.GetFocusedRowCellValue("id")?.ToString();
            comboBoxEditKullaniciRol.Text = gridView1.GetFocusedRowCellValue("rol")?.ToString();
            txtKullaniciAd.Text = gridView1.GetFocusedRowCellValue("kullanici_adi")?.ToString();
            txtKullaniciSifre.Text = gridView1.GetFocusedRowCellValue("sifre_hash")?.ToString();
            memoEditNot.Text = gridView1.GetFocusedRowCellValue("notlar")?.ToString();
        }

        private bool sifreGuncelleniyor = false;

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (sifreGuncelleniyor) return;

            string kullaniciId = gridView1.GetRowCellValue(e.RowHandle, "id")?.ToString();
            if (string.IsNullOrEmpty(kullaniciId)) return;

            string columnName = e.Column.FieldName;
            string newValue = e.Value?.ToString()?.Trim() ?? "";

            if (string.IsNullOrEmpty(newValue)) return;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;

                if (columnName == "kullanici_adi")
                {
                    // ✅ Aynı kullanıcı adı başka kullanıcıda var mı kontrolü
                    MySqlCommand kontrolCmd = new MySqlCommand("SELECT COUNT(*) FROM kullanicilar WHERE kullanici_adi = @ad AND id != @id", conn);
                    kontrolCmd.Parameters.AddWithValue("@ad", newValue);
                    kontrolCmd.Parameters.AddWithValue("@id", kullaniciId);
                    int varMi = Convert.ToInt32(kontrolCmd.ExecuteScalar());

                    if (varMi > 0)
                    {
                        XtraMessageBox.Show("Bu kullanıcı adı başka bir kullanıcıya ait. Lütfen farklı bir ad girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        KullaniciListele();
                        return;
                    }

                    cmd.CommandText = "UPDATE kullanicilar SET kullanici_adi = @deger WHERE id = @id";
                    cmd.Parameters.AddWithValue("@deger", newValue);
                    cmd.Parameters.AddWithValue("@id", kullaniciId);
                    cmd.ExecuteNonQuery();
                    XtraMessageBox.Show("Kullanıcı adı güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (columnName == "notlar")
                {
                    cmd.CommandText = "UPDATE kullanicilar SET notlar = @deger WHERE id = @id";
                    cmd.Parameters.AddWithValue("@deger", newValue);
                    cmd.Parameters.AddWithValue("@id", kullaniciId);
                    cmd.ExecuteNonQuery();
                    XtraMessageBox.Show("Not güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (columnName == "sifre_hash")
                {
                    if (!SifreGecerliMi(newValue))
                    {
                        XtraMessageBox.Show("Şifre en az 8 karakter, en az 2 harf, 2 rakam ve 1 özel karakter içermelidir.", "Geçersiz Şifre", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        KullaniciListele();
                        return;
                    }

                    sifreGuncelleniyor = true;
                    string hashed = Sha256Hashle(newValue);
                    cmd.CommandText = "UPDATE kullanicilar SET sifre_hash = @deger WHERE id = @id";
                    cmd.Parameters.AddWithValue("@deger", hashed);
                    cmd.Parameters.AddWithValue("@id", kullaniciId);
                    cmd.ExecuteNonQuery();

                    gridView1.SetRowCellValue(e.RowHandle, e.Column, hashed);
                    sifreGuncelleniyor = false;

                    XtraMessageBox.Show("Şifre hashlenerek güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

    }
}