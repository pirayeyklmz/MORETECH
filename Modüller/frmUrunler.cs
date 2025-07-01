using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Controls;
using System.Windows.Forms;
using ComboBox = DevExpress.XtraEditors.ComboBox;
using DevExpress.XtraEditors.Controls;
using MORETECH;
using Image = System.Drawing.Image;
using System.Globalization;


namespace DENEME
{
    public partial class frmUrunler : XtraForm

    {
        private string cnnStr = "Server=localhost;Database=stoktakip;Uid=root;Pwd=;Allow Zero Datetime=True;Convert Zero Datetime=True;";

        public frmUrunler()
        {
            InitializeComponent();

            txtBarkod.KeyPress += txtBarkod_KeyPress;
            txtSatisFiyat.KeyPress += txtSatisFiyat_KeyPress;
            txtAlisFiyat.KeyPress += txtAlisFiyat_KeyPress;
            cmbKDV.KeyPress += cmbKDV_KeyPress;

            this.IsMdiContainer = false;

        }

        private void frmUrunler_Load(object sender, EventArgs e)
        {

            KDVGetir();
            Temizle();
            UrunleriListele();

            txtUrunId.ReadOnly = true;
            txtUrunId.BackColor = Color.LightGray;
            txtEditTarih.BackColor = Color.LightGray;

            BeginInvoke(new Action(() =>
            {
                gridView1.FocusedRowHandle = DevExpress.XtraGrid.GridControl.InvalidRowHandle;
            }));

        }


        private void KDVGetir()
        {
            cmbKDV.Properties.Items.Clear();
            cmbKDV.Properties.Items.Add("%0");
            cmbKDV.Properties.Items.Add("%1");
            cmbKDV.Properties.Items.Add("%10");
            cmbKDV.Properties.Items.Add("%20");

            cmbKDV.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;

            if (cmbKDV.Properties.Items.Count > 0)
            {
                cmbKDV.SelectedIndex = 0;
            }
            else
            {
                XtraMessageBox.Show("KDV değerleri yüklenemedi!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public void UrunleriListele()
        {
            using var cnn = new MySqlConnection(cnnStr);
            cnn.Open();
            var query = @"SELECT urun_id, urun_barkod, urun_ad, urun_adet, urun_alisfiyat, urun_satisfiyat, urun_detay, urun_kdv,
  CASE WHEN tarih = '0000-00-00 00:00:00' THEN NULL ELSE DATE_FORMAT(tarih, '%d/%m/%Y %H:%i:%s') END AS tarih
FROM urunler";

            var da = new MySqlDataAdapter(query, cnn);
            var dt = new DataTable();
            da.Fill(dt);

            gridControl1.DataSource = dt;
            gridView1.PopulateColumns();
            gridView1.BestFitColumns();

            gridView1.Columns["urun_id"].Caption = "ID";
            gridView1.Columns["urun_barkod"].Caption = "ÜRÜN BARKODU";
            gridView1.Columns["urun_ad"].Caption = "ÜRÜN ADI";
            gridView1.Columns["urun_alisfiyat"].Caption = "ALIŞ FİYATI";
            gridView1.Columns["urun_satisfiyat"].Caption = "SATIŞ FİYATI";
            gridView1.Columns["urun_adet"].Caption = "STOK ADETİ";
            gridView1.Columns["urun_kdv"].Caption = " SATIŞ KDV ORANI";
            gridView1.Columns["urun_detay"].Caption = "DETAY";
            gridView1.Columns["tarih"].Caption = "KAYIT TARİHİ";


            // tarih sütunu varsa ayarla
            if (gridView1.Columns["tarih"] != null)
            {
                gridView1.Columns["tarih"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                gridView1.Columns["tarih"].DisplayFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
                gridView1.Columns["tarih"].DisplayFormat.Format = new DateTimeFormatInfo
                {
                    ShortDatePattern = "dd/MM/yyyy",
                    LongTimePattern = "HH:mm:ss"
                };
            }

        }


        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle >= 0)
            {
                txtUrunId.Text = gridView1.GetRowCellValue(e.FocusedRowHandle, "urun_id")?.ToString();
                txtBarkod.Text = gridView1.GetRowCellValue(e.FocusedRowHandle, "urun_barkod")?.ToString();
                txtUrunadi.Text = gridView1.GetRowCellValue(e.FocusedRowHandle, "urun_ad")?.ToString();
                txtAlisFiyat.Text = gridView1.GetRowCellValue(e.FocusedRowHandle, "urun_alisfiyat")?.ToString();
                txtSatisFiyat.Text = gridView1.GetRowCellValue(e.FocusedRowHandle, "urun_satisfiyat")?.ToString();

                var kdvDegeri = gridView1.GetRowCellValue(e.FocusedRowHandle, "urun_kdv");


                if (kdvDegeri != null && cmbKDV.Properties.Items.Count > 0)
                {
                    string kdvText = "%" + Convert.ToDecimal(kdvDegeri).ToString();

                    if (cmbKDV.Properties.Items.Contains(kdvText))
                    {
                        cmbKDV.SelectedItem = kdvText;
                    }
                    else
                    {
                        MessageBox.Show($"KDV değeri listede bulunamadı: {kdvText}", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        cmbKDV.SelectedIndex = 0;
                    }
                }
                else
                {
                    if (cmbKDV.Properties.Items.Count > 0)
                    {
                        cmbKDV.SelectedIndex = 0;
                    }
                }

                object adetObj = gridView1.GetRowCellValue(e.FocusedRowHandle, "urun_adet");

                numAdet.Minimum = 0;

                int adet = 0;

                // Null kontrolü ve sayısal kontrol
                if (adetObj != null && int.TryParse(adetObj.ToString(), out adet))
                {
                    if (adet >= 0)
                    {
                        numAdet.Value = adet;
                    }
                    else
                    {
                        MessageBox.Show("Ürün adedi 0'dan küçük olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        numAdet.Value = 0; // veya önceki geçerli değeri
                    }
                }
                else
                {
                    MessageBox.Show("Geçersiz ürün adedi!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    numAdet.Value = 0;
                }

                memoDetay.Text = gridView1.GetRowCellValue(e.FocusedRowHandle, "urun_detay")?.ToString();
                var tarihStr = gridView1.GetRowCellValue(e.FocusedRowHandle, "tarih")?.ToString();

                if (!string.IsNullOrWhiteSpace(tarihStr) &&
                    DateTime.TryParseExact(tarihStr, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime tarih))
                {
                    txtEditTarih.Text = tarih.ToString("dd/MM/yyyy HH:mm:ss");
                }
                else
                {
                    txtEditTarih.Text = ""; // Veya "Geçersiz Tarih"
                }

            }

        }


        private void btnKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                dxErrorProvider1.ClearErrors(); // Tüm hataları temizle

                string urun_ad = txtUrunadi.Text.Trim();
                string urun_barkod = txtBarkod.Text.Trim();

                // 13 haneli mi ve tamamen sayısal mı kontrolü
                if (urun_barkod.Length != 13 || !long.TryParse(urun_barkod, out _))
                {
                    MessageBox.Show("Lütfen 13 haneli geçerli bir sayısal fatura numarası giriniz.");
                    return;
                }

                string urun_detay = memoDetay.Text.Trim();

                bool hatali = false;

                // Ürün adı kontrolü
                if (string.IsNullOrEmpty(urun_ad))
                {
                    dxErrorProvider1.SetError(txtUrunadi, "Ürün adı boş olamaz.");
                    hatali = true;
                }

                // Alış ve satış fiyatı kontrolü
                if (!decimal.TryParse(txtAlisFiyat.Text.Trim(), out decimal alisFiyat) || alisFiyat <= 0)
                {
                    dxErrorProvider1.SetError(txtAlisFiyat, "Geçerli bir alış fiyatı giriniz.");
                    hatali = true;
                }

                if (!decimal.TryParse(txtSatisFiyat.Text.Trim(), out decimal satisFiyat) || satisFiyat <= 0)
                {
                    dxErrorProvider1.SetError(txtSatisFiyat, "Geçerli bir satış fiyatı giriniz.");
                    hatali = true;
                }

                // KDV oranı kontrolü
                string kdvMetin = cmbKDV.Text.Replace("%", "").Trim();
                if (!int.TryParse(kdvMetin, out int kdvOrani) || kdvOrani < 0)
                {
                    dxErrorProvider1.SetError(cmbKDV, "Geçerli bir KDV oranı girin.");
                    hatali = true;
                }

                // Adet kontrolü
                int urun_adet = (int)numAdet.Value;
                if (urun_adet <= 0)
                {
                    dxErrorProvider1.SetError(numAdet, "Geçerli bir adet girin.");
                    hatali = true;
                }

                // Hata varsa işlemi durdur
                if (hatali)
                {
                    MessageBox.Show("Lütfen hatalı alanları düzeltin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Veritabanı kontrolü
                using var cnn = new MySqlConnection(cnnStr);
                cnn.Open();

                var kontrolCmd = new MySqlCommand("SELECT COUNT(*) FROM urunler WHERE urun_ad = @urun_ad OR urun_barkod = @urun_barkod", cnn);
                kontrolCmd.Parameters.AddWithValue("@urun_ad", urun_ad);
                kontrolCmd.Parameters.AddWithValue("@urun_barkod", urun_barkod);
                var mevcutSayi = Convert.ToInt32(kontrolCmd.ExecuteScalar());

                if (mevcutSayi > 0)
                {
                    dxErrorProvider1.SetError(txtUrunadi, "Bu ürün adı zaten mevcut.");
                    dxErrorProvider1.SetError(txtBarkod, "Bu barkod zaten mevcut.");
                    MessageBox.Show("Bu ürün adı veya barkod zaten mevcut.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kayıt ekleme
                var query = @"INSERT INTO urunler 
      (urun_barkod, urun_ad, urun_alisfiyat, urun_satisfiyat, urun_kdv, urun_adet, urun_detay, tarih, urunFoto) 
      VALUES 
      (@urun_barkod, @urun_ad, @urun_alisfiyat, @urun_satisfiyat, @urun_kdv, @urun_adet, @urun_detay, @tarih, @foto)";

                using var cmd = new MySqlCommand(query, cnn);
                cmd.Parameters.AddWithValue("@urun_barkod", urun_barkod);
                cmd.Parameters.AddWithValue("@urun_ad", urun_ad);
                cmd.Parameters.AddWithValue("@urun_alisfiyat", alisFiyat);
                cmd.Parameters.AddWithValue("@urun_satisfiyat", satisFiyat);
                cmd.Parameters.AddWithValue("@urun_kdv", kdvOrani);
                cmd.Parameters.AddWithValue("@urun_adet", urun_adet);
                cmd.Parameters.AddWithValue("@urun_detay", urun_detay);
                DateTime tarih;
                if (!DateTime.TryParseExact(txtEditTarih.Text.Trim(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out tarih))
                {
                    tarih = DateTime.Now;
                }
                cmd.Parameters.AddWithValue("@tarih", tarih.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));

                var paramFoto = new MySqlParameter("@foto", MySqlDbType.LongBlob);
                paramFoto.Value = (geciciResimBytes != null) ? geciciResimBytes : DBNull.Value;
                cmd.Parameters.Add(paramFoto);


                cmd.ExecuteNonQuery();

                MessageBox.Show("Ürün başarıyla eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Temizle();
                UrunleriListele();
                gridControl1.RefreshDataSource();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            UrunleriListele();
            foreach (Form form in Application.OpenForms)
            {
                if (form is frmUrunler urunForm)
                {
                    urunForm.UrunleriListele();
                    break;
                }
            }

        }


        private void btnGüncelle_Click(object sender, EventArgs e)
        {
            try
            {
                dxErrorProvider1.ClearErrors();

                int rowHandle = gridView1.FocusedRowHandle;
                if (rowHandle < 0)
                {
                    MessageBox.Show("Güncellemek için bir ürün seçin.");
                    return;
                }

                string urunAd = txtUrunadi.Text.Trim();
                string urunBarkod = txtBarkod.Text.Trim();
                string urunDetay = memoDetay.Text.Trim();

                // Ürün adı kontrolü
                if (string.IsNullOrEmpty(urunAd))
                {
                    dxErrorProvider1.SetError(txtUrunadi, "Ürün adı boş olamaz.");
                    return;
                }

                // Barkod kontrolü (13 haneli ve sayısal mı)
                if (urunBarkod.Length != 13 || !long.TryParse(urunBarkod, out _))
                {
                    dxErrorProvider1.SetError(txtBarkod, "Barkod 13 haneli sayısal bir değer olmalıdır.");
                    return;
                }

                // Fiyat kontrolleri
                if (!decimal.TryParse(txtAlisFiyat.Text.Trim(), out decimal alisFiyat) || alisFiyat <= 0 ||
                    !decimal.TryParse(txtSatisFiyat.Text.Trim(), out decimal satisFiyat) || satisFiyat <= 0)
                {
                    dxErrorProvider1.SetError(txtAlisFiyat, "Geçerli alış fiyatı girin.");
                    dxErrorProvider1.SetError(txtSatisFiyat, "Geçerli satış fiyatı girin.");
                    return;
                }

                // KDV kontrolü
                string kdvMetin = cmbKDV.Text.Replace("%", "").Trim();
                if (!int.TryParse(kdvMetin, out int kdvOrani) || kdvOrani < 0)
                {
                    dxErrorProvider1.SetError(cmbKDV, "Geçerli KDV oranı girin.");
                    return;
                }

                // Adet kontrolü
                int urunAdet = (int)numAdet.Value;
                if (urunAdet <= 0)
                {
                    dxErrorProvider1.SetError(numAdet, "Geçerli bir adet girin.");
                    return;
                }

                int urunId = Convert.ToInt32(gridView1.GetRowCellValue(rowHandle, "urun_id"));

                using var cnn = new MySqlConnection(cnnStr);
                cnn.Open();

                string query = @"UPDATE urunler 
                 SET urun_barkod = @urun_barkod, urun_ad = @urun_ad, urun_alisfiyat = @urun_alisfiyat,
                     urun_satisfiyat = @urun_satisfiyat, urun_kdv = @urun_kdv, urun_adet = @urun_adet,
                     urun_detay = @urun_detay, tarih = @tarih
                 WHERE urun_id = @urun_id";

                using var cmd = new MySqlCommand(query, cnn);
                cmd.Parameters.AddWithValue("@urun_barkod", urunBarkod);
                cmd.Parameters.AddWithValue("@urun_ad", urunAd);
                cmd.Parameters.AddWithValue("@urun_alisfiyat", alisFiyat);
                cmd.Parameters.AddWithValue("@urun_satisfiyat", satisFiyat);
                cmd.Parameters.AddWithValue("@urun_kdv", kdvOrani);
                cmd.Parameters.AddWithValue("@urun_adet", urunAdet);
                cmd.Parameters.AddWithValue("@urun_detay", urunDetay);
                DateTime tarih;
                if (!DateTime.TryParseExact(txtEditTarih.Text.Trim(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out tarih))
                {
                    tarih = DateTime.Now;
                }
                cmd.Parameters.AddWithValue("@tarih", tarih.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));

                cmd.Parameters.AddWithValue("@urun_id", urunId);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Ürün başarıyla güncellendi.");
                UrunleriListele();
                gridView1.RefreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Güncelleme hatası: " + ex.Message);
            }
        }


        private void btnSil_Click(object sender, EventArgs e)
        {
            int[] selectedRows = gridView1.GetSelectedRows();
            if
                (selectedRows.Length == 0)
            {
                MessageBox.Show("Silmek için bir ürün seçin.");
                return;
            }
            if (MessageBox.Show("Seçilen ürünü silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                using var cnn = new MySqlConnection(cnnStr);
                cnn.Open();
                foreach (int rowHandle in selectedRows)
                {
                    int urunId = Convert.ToInt32(gridView1.GetRowCellValue(rowHandle, "urun_id"));
                    var query = "DELETE FROM urunler WHERE urun_id = @id";
                    using var cmd = new MySqlCommand(query, cnn);
                    cmd.Parameters.AddWithValue("@id", urunId);
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Ürün(ler) başarıyla silindi.");
                UrunleriListele();
                gridControl1.RefreshDataSource();
            }
        }

        private void Temizle()
        {
            txtUrunId.Text = "";
            txtBarkod.Text = "";
            txtUrunadi.Text = "";
            txtAlisFiyat.Text = "0.00";
            txtSatisFiyat.Text = "0.00";
            txtEditTarih.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            memoDetay.Text = "";
            numAdet.Value = 1;

            if (cmbKDV.Properties.Items.Count > 0)
            {
                cmbKDV.SelectedIndex = 0;
            }
            else
            {
                cmbKDV.Text = "%0";
            }
        }


        private void btnTemizle_Click(object sender, EventArgs e) => Temizle();

        private void txtBarkod_TextChanged(object sender, EventArgs e)
        {
            if (txtBarkod.Text.Length > 13)
            {
                MessageBox.Show("Barkod numarası yalnızca 13 haneli olabilir.");
                txtBarkod.Text = txtBarkod.Text.Substring(0, 13);
            }
        }

        private void txtBarkod_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back) e.Handled = true;
        }

        private void txtAlisFiyat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != '.') e.Handled = true;
            if (e.KeyChar == '.' && txtAlisFiyat.Text.Contains(".")) e.Handled = true;
        }

        private void txtSatisFiyat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != '.') e.Handled = true;
            if (e.KeyChar == '.' && txtSatisFiyat.Text.Contains(".")) e.Handled = true;
        }

        private byte[] geciciResimBytes = null;
        private void btnFotoYukle_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Resim Dosyaları|.jpg;.jpeg;.png;.bmp";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    FileInfo fi = new FileInfo(ofd.FileName);
                    long maxBoyut = 16 * 1024 * 1024; // 16 MB

                    if (fi.Length > maxBoyut)
                    {
                        MessageBox.Show("Seçilen dosya 16 MB sınırını aşıyor.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    byte[] resimBytes = File.ReadAllBytes(ofd.FileName);

                    if (!int.TryParse(txtUrunId.Text, out int urunId) || urunId <= 0)
                    {
                        MessageBox.Show("Lütfen önce bir ürün seçin ve tekrar deneyin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    using var cnn = new MySqlConnection(cnnStr);
                    cnn.Open();

                    using var cmd = new MySqlCommand("UPDATE urunler SET urunFoto = @foto WHERE urun_id = @id", cnn);

                    var param = new MySqlParameter("@foto", MySqlDbType.LongBlob);
                    param.Value = resimBytes;
                    cmd.Parameters.Add(param);

                    cmd.Parameters.AddWithValue("@id", urunId);

                    int affectedRows = cmd.ExecuteNonQuery();

                    if (affectedRows > 0)
                    {
                        MessageBox.Show("Fotoğraf başarıyla yüklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Fotoğraf yüklenemedi, ürün bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fotoğraf yükleme sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbKDV_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back) e.Handled = true;

        }
    }

}