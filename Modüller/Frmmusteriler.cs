using DENEME;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StokTakip.Musteriler
{
    public partial class Frmmusteriler : DevExpress.XtraEditors.XtraForm
    {
        string myConnectionString = "server=localhost;database=stoktakip;uid=root;pwd=;";
        public Frmmusteriler()
        {
            InitializeComponent();
            gridView1.FocusedRowChanged += gridView1_FocusedRowChanged;
            this.gridView1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanged);
            this.Load += Frmmusteriler_Load;
        }

        private void Frmmusteriler_Load(object sender, EventArgs e)
        {
            UrunleriListele();
            BtnMusteriTemizle.PerformClick();

            lblControlAd.Text = "Ad: <color=red>*</color>";
            lblControlSoyad.Text = "Soyad: <color=red>*</color>";
            lblControlSirket.Text = "Şirket: <color=red>*</color>";
            lblControlVergi.Text = "Vergi No: <color=red>*</color>";
            lblControlMail.Text = "E-Posta: <color=red>*</color>";
            lblControlTel.Text = "Telefon: <color=red>*</color>";
            lblControlIl.Text = "İl: <color=red>*</color>";
            lblControlIlce.Text = "İlçe: <color=red>*</color>";

            RepositoryItemTextEdit maskedPhoneEdit = new RepositoryItemTextEdit();
            maskedPhoneEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Simple;
            maskedPhoneEdit.Mask.EditMask = "(999) 000-0000";
            maskedPhoneEdit.Mask.UseMaskAsDisplayFormat = true;

            maskedTextBoxMusterilertel.Mask = "(999) 000-0000";
            maskedTextBoxMusterilertel.PromptChar = '_'; // boş karakter için
            maskedTextBoxMusterilertel.ResetOnPrompt = false;
            maskedTextBoxMusterilertel.ResetOnSpace = false;

            gridView1.Columns["musteri_tel"].ColumnEdit = maskedPhoneEdit;

            // Alfabetik alanlar
            txtMusterilerAd.KeyPress += HarfAlanlari_KeyPress;
            txtMusterilerSoyad.KeyPress += HarfAlanlari_KeyPress;
            txtMusterilerIl.KeyPress += HarfAlanlari_KeyPress;
            txtMusterilerIlce.KeyPress += HarfAlanlari_KeyPress;

            // Şirket (harf + nokta)
            txtMusterilerSirket.KeyPress += SirketAlanlari_KeyPress;

            // Vergi numarası (sadece sayı)
            txtMusterilerVergi.KeyPress += SayisalAlanlar_KeyPress;
            txtMusterilerid.KeyPress += SayisalAlanlar_KeyPress;
        }
        private void UrunleriListele()
        {
            using (MySqlConnection conn = new MySqlConnection(myConnectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM musteriler";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gridControl1.DataSource = dt;

                    gridView1.Columns.Clear();
                    gridView1.Columns.AddVisible("musteri_id", "Müşteri ID");
                    gridView1.Columns.AddVisible("musteri_ad", "Müşteri Adı");
                    gridView1.Columns.AddVisible("musteri_soyad", "Müşteri Soyadı");
                    gridView1.Columns.AddVisible("musteri_tip", "Müşteri Tipi");
                    gridView1.Columns.AddVisible("musteri_sirket", "Müşteri Şirketi");
                    gridView1.Columns.AddVisible("musteri_vergi", "Müşteri Vergi Numarası");
                    gridView1.Columns.AddVisible("musteri_mail", "Müşteri mail'i");
                    gridView1.Columns.AddVisible("musteri_tel", "Müşteri Teli");
                    gridView1.Columns.AddVisible("musteri_il", "Müşteri ili");
                    gridView1.Columns.AddVisible("musteri_ilce", "Müşteri İlçesi");
                    gridView1.Columns.AddVisible("musteri_adres", "Müşteri Adresi");


                    gridView1.BestFitColumns();


                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Listeleme hatası: " + ex.Message);
                }
            }

        }

        private void BtnMusteriKaydet_Click(object sender, EventArgs e)
        {
            bool GecerliHarfAlani(string text, string alanAdi)
            {
                if (string.IsNullOrWhiteSpace(text) || !text.All(c => char.IsLetter(c) || c == ' '))
                {
                    MessageBox.Show($"{alanAdi} alanı boş bırakılamaz ve sadece harf içermelidir.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                return true;
            }

            // 1. Ad
            if (!GecerliHarfAlani(txtMusterilerAd.Text, "Ad")) return;

            // 2. Soyad
            if (!GecerliHarfAlani(txtMusterilerSoyad.Text, "Soyad")) return;

            // 3. Şirket
            if (string.IsNullOrWhiteSpace(txtMusterilerSirket.Text) ||
    !txtMusterilerSirket.Text.All(c => char.IsLetter(c) || c == '.' || c == ' '))
            {
                MessageBox.Show("Şirket alanı boş bırakılamaz ve sadece harf veya nokta (.) içerebilir.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 4. Vergi Numarası
            if (string.IsNullOrWhiteSpace(txtMusterilerVergi.Text) || !txtMusterilerVergi.Text.All(char.IsDigit))
            {
                MessageBox.Show("Vergi numarası boş bırakılamaz ve sadece sayısal karakterlerden oluşmalıdır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (MySqlConnection checkConn = new MySqlConnection(myConnectionString))
            {
                checkConn.Open();
                string checkQuery = "SELECT COUNT(*) FROM musteriler WHERE musteri_vergi = @musteriVerginumarasi";
                MySqlCommand checkCmd = new MySqlCommand(checkQuery, checkConn);
                checkCmd.Parameters.AddWithValue("@musteriVerginumarasi", txtMusterilerVergi.Text);

                int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                if (count > 0)
                {
                    MessageBox.Show("Bu vergi numarası zaten başka bir müşteriye ait. Lütfen farklı bir vergi numarası giriniz.");
                    return;
                }
            }

            // 5. E-Posta
            string mail = txtMusterilerMail.Text?.Trim();

            if (string.IsNullOrWhiteSpace(mail) ||
                !mail.Contains("@") ||
                mail.StartsWith("@") || mail.EndsWith("@") ||
                !mail.Contains(".") ||
                mail.LastIndexOf(".") < mail.IndexOf("@"))
            {
                MessageBox.Show("Geçerli bir e-posta adresi giriniz. '@' ve '.' işareti doğru konumda olmalı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 6. Telefon
            string temizTel = new string(maskedTextBoxMusterilertel.Text.Where(char.IsDigit).ToArray());
            if (temizTel.Length != 10)
            {
                MessageBox.Show("Telefon numarası girilmesi zorunludur ve 10 haneli olmalıdır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 7. İl
            if (!GecerliHarfAlani(txtMusterilerIl.Text, "İl")) return;

            // 8. İlçe
            if (!GecerliHarfAlani(txtMusterilerIlce.Text, "İlçe")) return;


            MySqlConnection cnn = new MySqlConnection(myConnectionString);
            try
            {
                cnn.Open();
                string query = "INSERT INTO musteriler ( musteri_tip, musteri_ad, musteri_soyad, musteri_tel, musteri_mail, musteri_sirket, musteri_vergi, musteri_il, musteri_ilce, musteri_adres) " +
                    "VALUES (@musteriTipi, @musteriAdi, @musteriSoyadi, @musteriTeli, @musteriMaili, @musteriSirketi, @musteriVerginumarasi, @musteriIli, @musteriIlcesi, @musteriAdresi );";
                MySqlCommand cmd = new MySqlCommand(query, cnn);

                cmd.Parameters.Add("@musteriTipi", MySqlDbType.VarChar).Value = comboBoxEditMusterilertip.Text;
                cmd.Parameters.Add("@musteriAdi", MySqlDbType.VarChar).Value = txtMusterilerAd.Text;
                cmd.Parameters.Add("@musteriSoyadi", MySqlDbType.VarChar).Value = txtMusterilerSoyad.Text;
                cmd.Parameters.Add("@musteriTeli", MySqlDbType.VarChar).Value = maskedTextBoxMusterilertel.Text;
                cmd.Parameters.Add("@musteriMaili", MySqlDbType.VarChar).Value = txtMusterilerMail.Text;
                cmd.Parameters.Add("@musteriSirketi", MySqlDbType.VarChar).Value = txtMusterilerSirket.Text;
                cmd.Parameters.Add("@musteriVerginumarasi", MySqlDbType.VarChar).Value = txtMusterilerVergi.Text;
                cmd.Parameters.Add("@musteriIli", MySqlDbType.VarChar).Value = txtMusterilerIl.Text;
                cmd.Parameters.Add("@musteriIlcesi", MySqlDbType.VarChar).Value = txtMusterilerIlce.Text;
                cmd.Parameters.Add("@musteriAdresi", MySqlDbType.VarChar).Value = memoEditAdres.Text;



                int i = cmd.ExecuteNonQuery();

                cnn.Close();

                if (i > 0)
                {
                    MessageBox.Show("Veri oluşturuldu");
                    UrunleriListele();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bağlantı kurulamadı: " + ex.Message);
            }
            int id = 0;

        }

        private void BtnMusteriGuncelle_Click(object sender, EventArgs e)
        {
            bool GecerliHarfAlani(string text, string alanAdi)
            {
                if (string.IsNullOrWhiteSpace(text) || !text.All(c => char.IsLetter(c) || c == ' '))
                {
                    MessageBox.Show($"{alanAdi} alanı boş bırakılamaz ve sadece harf içermelidir.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                return true;
            }

            bool GecerliHarfVeyaNokta(string deger, string alanAdi)
            {
                if (string.IsNullOrWhiteSpace(deger) || !deger.All(c => char.IsLetter(c) || c == '.' || c == ' '))
                {
                    MessageBox.Show($"{alanAdi} alanı boş olamaz ve sadece harf veya nokta (.) içerebilir.");
                    return false;
                }
                return true;
            }

            // 1. Ad
            if (!GecerliHarfAlani(txtMusterilerAd.Text, "Ad")) return;

            // 2. Soyad
            if (!GecerliHarfAlani(txtMusterilerSoyad.Text, "Soyad")) return;

            // 3. Şirket
            if (!GecerliHarfVeyaNokta(txtMusterilerSirket.Text, "Şirket")) return;

            // 4. Vergi No
            if (string.IsNullOrWhiteSpace(txtMusterilerVergi.Text) || !txtMusterilerVergi.Text.All(char.IsDigit))
            {
                MessageBox.Show("Vergi numarası boş bırakılamaz ve sadece sayısal karakterlerden oluşmalıdır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (MySqlConnection checkConn = new MySqlConnection(myConnectionString))
            {
                checkConn.Open();
                string checkQuery = "SELECT COUNT(*) FROM musteriler WHERE musteri_vergi = @musteriVerginumarasi AND musteri_id != @musteriid";
                MySqlCommand checkCmd = new MySqlCommand(checkQuery, checkConn);
                checkCmd.Parameters.AddWithValue("@musteriVerginumarasi", txtMusterilerVergi.Text);
                checkCmd.Parameters.AddWithValue("@musteriid", txtMusterilerid.Text);

                int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                if (count > 0)
                {
                    MessageBox.Show("Bu vergi numarası zaten başka bir müşteriye ait. Lütfen farklı bir vergi numarası giriniz.");
                    return;
                }
            }

            // 5. Mail
            string mail = txtMusterilerMail.Text?.Trim();

            if (string.IsNullOrWhiteSpace(mail) ||
                !mail.Contains("@") ||
                mail.StartsWith("@") || mail.EndsWith("@") ||
                !mail.Contains(".") ||
                mail.LastIndexOf(".") < mail.IndexOf("@"))
            {
                MessageBox.Show("Geçerli bir e-posta adresi giriniz. '@' ve '.' işareti doğru konumda olmalı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 6. Telefon
            string temizTel = new string(maskedTextBoxMusterilertel.Text.Where(char.IsDigit).ToArray());
            if (temizTel.Length != 10)
            {
                MessageBox.Show("Telefon numarası 10 haneli olmalıdır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 7. İl
            if (!GecerliHarfAlani(txtMusterilerIl.Text, "İl")) return;

            // 8. İlçe
            if (!GecerliHarfAlani(txtMusterilerIlce.Text, "İlçe")) return;


            using (MySqlConnection conn = new MySqlConnection(myConnectionString))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE musteriler SET " +
                                   "musteri_tip=@musteriTipi, " +
                                   "musteri_ad=@musteriAdi, " +
                                   "musteri_soyad=@musteriSoyadi, " +
                                   "musteri_tel=@musteriTeli, " +
                                   "musteri_mail=@musteriMaili, " +
                                   "musteri_sirket=@musteriSirketi, " +
                                   "musteri_vergi=@musteriVerginumarasi, " +
                                   "musteri_il=@musteriIli, " +
                                   "musteri_ilce=@musteriIlcesi, " +
                                   "musteri_adres=@musteriAdresi " +
                                   "WHERE musteri_id=@musteriId";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.Add("@musteriId", MySqlDbType.VarChar).Value = txtMusterilerid.Text;
                    cmd.Parameters.Add("@musteriTipi", MySqlDbType.VarChar).Value = comboBoxEditMusterilertip.Text;
                    cmd.Parameters.Add("@musteriAdi", MySqlDbType.VarChar).Value = txtMusterilerAd.Text;
                    cmd.Parameters.Add("@musteriSoyadi", MySqlDbType.VarChar).Value = txtMusterilerSoyad.Text;
                    cmd.Parameters.Add("@musteriTeli", MySqlDbType.VarChar).Value = maskedTextBoxMusterilertel.Text;
                    cmd.Parameters.Add("@musteriMaili", MySqlDbType.VarChar).Value = mail;
                    cmd.Parameters.Add("@musteriSirketi", MySqlDbType.VarChar).Value = txtMusterilerSirket.Text;
                    cmd.Parameters.Add("@musteriVerginumarasi", MySqlDbType.VarChar).Value = txtMusterilerVergi.Text;
                    cmd.Parameters.Add("@musteriIli", MySqlDbType.VarChar).Value = txtMusterilerIl.Text;
                    cmd.Parameters.Add("@musteriIlcesi", MySqlDbType.VarChar).Value = txtMusterilerIlce.Text;
                    cmd.Parameters.Add("@musteriAdresi", MySqlDbType.VarChar).Value = memoEditAdres.Text;

                    int affectedRows = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (affectedRows > 0)
                    {
                        MessageBox.Show("Müşteri bilgileri güncellendi.");
                        UrunleriListele();
                    }
                    else
                    {
                        MessageBox.Show("Güncelleme yapılamadı. Belirtilen müşteri ID bulunamadı.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Güncelleme hatası: " + ex.Message);
                }
            }

        }
        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            txtMusterilerid.Text = gridView1.GetFocusedRowCellValue("musteri_id")?.ToString();
            comboBoxEditMusterilertip.Text = gridView1.GetFocusedRowCellValue("musteri_tip")?.ToString();
            txtMusterilerAd.Text = gridView1.GetFocusedRowCellValue("musteri_ad")?.ToString();
            txtMusterilerSoyad.Text = gridView1.GetFocusedRowCellValue("musteri_soyad")?.ToString();

            string rawPhone = gridView1.GetFocusedRowCellValue("musteri_tel")?.ToString();
            if (!string.IsNullOrWhiteSpace(rawPhone))
            {
                string digitsOnly = new string(rawPhone.Where(char.IsDigit).ToArray());

                if (digitsOnly.Length == 10)
                {
                    maskedTextBoxMusterilertel.Text = Convert.ToUInt64(digitsOnly).ToString(@"(000) 000\-0000");
                }
                else
                {
                    maskedTextBoxMusterilertel.Clear();
                }
            }
            else
            {
                maskedTextBoxMusterilertel.Clear();
            }

            txtMusterilerMail.Text = gridView1.GetFocusedRowCellValue("musteri_mail")?.ToString();
            txtMusterilerSirket.Text = gridView1.GetFocusedRowCellValue("musteri_sirket")?.ToString();
            txtMusterilerVergi.Text = gridView1.GetFocusedRowCellValue("musteri_vergi")?.ToString();
            txtMusterilerIl.Text = gridView1.GetFocusedRowCellValue("musteri_il")?.ToString();
            txtMusterilerIlce.Text = gridView1.GetFocusedRowCellValue("musteri_ilce")?.ToString();
            memoEditAdres.Text = gridView1.GetFocusedRowCellValue("musteri_adres")?.ToString();
        }
        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            string musteriId = gridView1.GetRowCellValue(e.RowHandle, "musteri_id")?.ToString();

            if (string.IsNullOrEmpty(musteriId))
            {
                MessageBox.Show("Müşteri ID alınamadı.");
                return;
            }

            string columnName = e.Column.FieldName;
            string newValue = e.Value?.ToString()?.Trim() ?? "";

            bool GecerliHarf(string deger, string alanAdi)
            {
                if (string.IsNullOrWhiteSpace(deger) || !deger.All(c => char.IsLetter(c) || c == ' '))
                {
                    MessageBox.Show($"{alanAdi} alanı boş olamaz ve sadece harf içermelidir.");
                    return false;
                }
                return true;
            }

            switch (columnName)
            {
                case "musteri_ad":
                    if (!GecerliHarf(newValue, "Ad")) return;
                    break;

                case "musteri_soyad":
                    if (!GecerliHarf(newValue, "Soyad")) return;
                    break;

                case "musteri_sirket":
                    if (string.IsNullOrWhiteSpace(newValue) || !newValue.All(c => char.IsLetter(c) || c == '.' || c == ' '))
                    {
                        MessageBox.Show("Şirket alanı boş olamaz ve sadece harf veya nokta (.) içerebilir.");
                        return;
                    }
                    break;

                case "musteri_vergi":
                    if (string.IsNullOrWhiteSpace(newValue) || !newValue.All(char.IsDigit))
                    {
                        MessageBox.Show("Vergi numarası boş olamaz ve sadece sayısal karakterlerden oluşmalıdır.");
                        return;
                    }
                    using (MySqlConnection checkConn = new MySqlConnection(myConnectionString))
                    {
                        checkConn.Open();
                        string checkQuery = "SELECT COUNT(*) FROM musteriler WHERE musteri_vergi = @vergiNo AND musteri_id != @musteriId";

                        using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, checkConn))
                        {
                            checkCmd.Parameters.AddWithValue("@vergiNo", newValue);
                            checkCmd.Parameters.AddWithValue("@musteriId", musteriId);

                            int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                            if (count > 0)
                            {
                                MessageBox.Show("Bu vergi numarası zaten başka bir müşteriye ait. Lütfen farklı bir vergi numarası giriniz.");
                                return;
                            }
                        }
                    }
                    break;

                case "musteri_mail":
                    if (string.IsNullOrWhiteSpace(newValue) || !newValue.Contains("@") || newValue.StartsWith("@") || newValue.EndsWith("@"))
                    {
                        MessageBox.Show("Geçerli bir e-posta adresi giriniz. '@' işaretinden önce ve sonra karakter bulunmalıdır.");
                        return;
                    }
                    break;

                case "musteri_tel":
                    string girilenTel = newValue;

                    string sadeceRakam = new string(girilenTel.Where(char.IsDigit).ToArray());

                    if (sadeceRakam.Length != 10)
                    {
                        MessageBox.Show("Telefon numarası 10 haneli olmalıdır.");
                        return;
                    }
                    break;

                case "musteri_il":
                    if (!GecerliHarf(newValue, "İl")) return;
                    break;

                case "musteri_ilce":
                    if (!GecerliHarf(newValue, "İlçe")) return;
                    break;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(myConnectionString))
                {
                    conn.Open();
                    string query = $"UPDATE musteriler SET {columnName} = @newValue WHERE musteri_id = @musteriId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@newValue", newValue);
                        cmd.Parameters.AddWithValue("@musteriId", musteriId);

                        int affected = cmd.ExecuteNonQuery();
                        if (affected > 0)
                        {
                            MessageBox.Show("Güncelleme başarılı.");
                            UrunleriListele(); // tabloyu yenile
                        }
                        else
                        {
                            MessageBox.Show("Güncelleme başarısız. Belki de müşteri ID bulunamadı.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void BtnMusteriTemizle_Click(object sender, EventArgs e)
        {
            txtMusterilerid.Text = string.Empty;
            comboBoxEditMusterilertip.Text = string.Empty;
            txtMusterilerAd.Text = string.Empty;
            txtMusterilerSoyad.Text = string.Empty;
            maskedTextBoxMusterilertel.Text = string.Empty;
            txtMusterilerMail.Text = string.Empty;
            txtMusterilerSirket.Text = string.Empty;
            txtMusterilerVergi.Text = string.Empty;
            txtMusterilerIl.Text = string.Empty;
            txtMusterilerIlce.Text = string.Empty;
            memoEditAdres.Text = string.Empty;

            txtMusterilerid.Focus();
        }

        private void BtnMusteriSil_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMusterilerid.Text))
            {
                MessageBox.Show("Silmek için bir müşteri ID'si girin.");
                return;
            }

            DialogResult confirm = MessageBox.Show("Bu satırı silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                using (MySqlConnection cnn = new MySqlConnection(myConnectionString))
                {
                    try
                    {
                        cnn.Open();
                        string query = "DELETE FROM musteriler WHERE musteri_id=@id";
                        MySqlCommand cmd = new MySqlCommand(query, cnn);
                        cmd.Parameters.AddWithValue("@id", txtMusterilerid.Text);

                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            MessageBox.Show("müşteri başarıyla silindi.");
                            UrunleriListele();
                            BtnMusteriTemizle_Click(null, null);
                        }
                        else
                        {
                            MessageBox.Show("Müşteri bulunamadı.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Silme hatası: " + ex.Message);
                    }
                }
            }
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }

        // Sadece harf ve boşluk
        private void HarfAlanlari_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true;
            }
        }

        // Harf + boşluk + nokta
        private void SirketAlanlari_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ' ')
            {
                e.Handled = true;
            }
        }

        // Sadece rakam
        private void SayisalAlanlar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

    }
}