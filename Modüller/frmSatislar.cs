using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace DENEME
{
    public partial class frmSatislar : Form
    {
        string cnnStr = "Server=localhost;Database=stoktakip;Uid=root;Pwd=;SslMode=none;";

        public frmSatislar()
        {
            InitializeComponent();
            this.Load += frmSatislar_Load;
            this.Activated += frmSatislar_Activated;

            cmbUrun.SelectedValueChanged += cmbUrun_SelectedValueChanged;
            this.gridView1.FocusedRowChanged += gridView1_FocusedRowChanged;
        }

        private void frmSatislar_Load(object sender, EventArgs e)
        {
            UrunleriGetir();
            MusterileriGetir();

            SatislariGetir();
            numAdet.ReadOnly = false;
        }



        public void MusterileriGetir()
        {
            using var cnn = new MySqlConnection(cnnStr);
            var da = new MySqlDataAdapter("SELECT musteri_id, CONCAT(musteri_ad, ' ', musteri_soyad) AS musteri_adi FROM musteriler", cnn);
            DataTable dt = new();
            da.Fill(dt);
            cmbMusteri.DataSource = dt;
            cmbMusteri.DisplayMember = "musteri_adi";
            cmbMusteri.ValueMember = "musteri_id";
            cmbMusteri.SelectedIndex = -1;
        }

        public void UrunleriGetir()
        {
            using var cnn = new MySqlConnection(cnnStr);
            var da = new MySqlDataAdapter(@"
        SELECT urun_id, CONCAT(urun_ad, ' (Stok: ', urun_adet, ')') AS urun_ad
        FROM urunler", cnn);
            DataTable dt = new();
            da.Fill(dt);
            cmbUrun.DataSource = null;
            cmbUrun.DataSource = dt;
            cmbUrun.DisplayMember = "urun_ad";
            cmbUrun.ValueMember = "urun_id";
            cmbUrun.SelectedIndex = -1;
            txtFiyat.Text = "0.00";
        }


        private void cmbUrun_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbUrun.SelectedValue == null) return;
            using var cnn = new MySqlConnection(cnnStr);
            cnn.Open();
            using var cmd = new MySqlCommand("SELECT urun_satisfiyat FROM urunler WHERE urun_id = @id", cnn);
            cmd.Parameters.AddWithValue("@id", cmbUrun.SelectedValue);
            var fiyat = cmd.ExecuteScalar();
            txtFiyat.Text = fiyat != null ? Convert.ToDecimal(fiyat).ToString("0.00") : "0.00";
        }

        private void SatislariGetir()
        {
            using var cnn = new MySqlConnection(cnnStr);
            var da = new MySqlDataAdapter(@"
        SELECT s.satis_id,
               m.musteri_id,
               CONCAT(m.musteri_ad, ' ', m.musteri_soyad) AS musteri_adi,
               u.urun_id,
               u.urun_barkod,
               u.urun_ad,
               s.satis_adet,
               s.birim_fiyat,
               s.toplam_fiyat,
               COALESCE(s.iade_adet, 0) AS iade_miktar,
               s.satis_tarih
        FROM satislar s
        JOIN musteriler m ON s.musteri_id = m.musteri_id
        JOIN urunler u ON s.urun_id = u.urun_id
        ORDER BY s.satis_tarih DESC", cnn);

            DataTable dt = new DataTable();
            da.Fill(dt);

            gridControl1.DataSource = dt;
            gridView1.Columns.Clear();
            gridView1.PopulateColumns();

            gridView1.Columns["satis_id"].Visible = false;

            gridView1.Columns["musteri_adi"].Caption = "Müşteri Adı";
            gridView1.Columns["urun_id"].Caption = "Ürün ID";
            gridView1.Columns["urun_barkod"].Caption = "Ürün Barkodu";
            gridView1.Columns["urun_ad"].Caption = "Ürün Adı";
            gridView1.Columns["satis_adet"].Caption = "Satış Adet";
            gridView1.Columns["birim_fiyat"].Caption = "Birim Fiyat";
            gridView1.Columns["toplam_fiyat"].Caption = "Toplam Fiyat";
            gridView1.Columns["iade_miktar"].Caption = "İade Miktarı";
            gridView1.Columns["satis_tarih"].Caption = "Tarih";

            gridView1.BestFitColumns();

            gridView1.Columns["iade_miktar"].OptionsColumn.AllowEdit = true;
            gridView1.Columns["iade_miktar"].OptionsColumn.ReadOnly = false;
        }

        private void btnSatisYap_Click(object sender, EventArgs e)
        {
            if (cmbMusteri.SelectedValue == null || cmbUrun.SelectedValue == null || numAdet.Value <= 0)
            {
                MessageBox.Show("Tüm alanları doldurun.");
                return;
            }

            int musteriId = Convert.ToInt32(cmbMusteri.SelectedValue);
            int urunId = Convert.ToInt32(cmbUrun.SelectedValue);
            int satis_adet = (int)numAdet.Value;

            using var cnn = new MySqlConnection(cnnStr);
            cnn.Open();

            var stokCmd = new MySqlCommand("SELECT urun_adet, urun_satisfiyat FROM urunler WHERE urun_id = @id", cnn);
            stokCmd.Parameters.AddWithValue("@id", urunId);
            using var rdr = stokCmd.ExecuteReader();
            if (!rdr.Read() || Convert.ToInt32(rdr["urun_adet"]) < satis_adet)
            {
                MessageBox.Show("Yetersiz stok.");
                return;
            }
            decimal satisfiyat = Convert.ToDecimal(rdr["urun_satisfiyat"]);
            rdr.Close();

            decimal toplamFiyat = satisfiyat * satis_adet;

            var trans = cnn.BeginTransaction();
            try
            {
                var cmd = new MySqlCommand("INSERT INTO satislar (musteri_id, urun_id, satis_adet, birim_fiyat, toplam_fiyat, satis_tarih) VALUES (@m,@u,@a,@bf,@tf,@t)", cnn, trans);
                cmd.Parameters.AddWithValue("@m", musteriId);
                cmd.Parameters.AddWithValue("@u", urunId);
                cmd.Parameters.AddWithValue("@a", satis_adet);
                cmd.Parameters.AddWithValue("@bf", satisfiyat);
                cmd.Parameters.AddWithValue("@tf", toplamFiyat);
                cmd.Parameters.AddWithValue("@t", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.ExecuteNonQuery();

                var stokGuncelle = new MySqlCommand("UPDATE urunler SET urun_adet = urun_adet - @a WHERE urun_id = @id", cnn, trans);
                stokGuncelle.Parameters.AddWithValue("@a", satis_adet);
                stokGuncelle.Parameters.AddWithValue("@id", urunId);
                stokGuncelle.ExecuteNonQuery();

                trans.Commit();
                MessageBox.Show("Satış yapıldı.");
            }
            catch (Exception ex)
            {
                trans.Rollback();
                MessageBox.Show("Satış hatası: " + ex.Message);
            }

            SatislariGetir();
            UrunleriGetir();
            MusterileriGetir();
            cmbUrun.SelectedIndex = -1;
            Temizle();

            foreach (Form form in Application.OpenForms)
            {
                if (form is frmUrunler urunForm)
                {
                    urunForm.UrunleriListele();
                    break;
                }
            }
        }

        // Geri kalan metotlar aynı mantıkla birim_fiyat kullanacak şekilde güncellenmeli

        private void btnIade_Click(object sender, EventArgs e)
        {
            gridView1.CloseEditor();
            gridView1.UpdateCurrentRow();

            if (gridView1.FocusedRowHandle < 0)
            {
                MessageBox.Show("Lütfen iade yapılacak satırı seçin.");
                return;
            }

            // Artık iade miktarını txtIadeAdet üzerinden alıyoruz
            if (!int.TryParse(txtIadeAdet.Text, out int iade_miktar) || iade_miktar <= 0)
            {
                MessageBox.Show("Geçerli bir iade miktarı girin.");
                return;
            }

            DataRow row = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            if (row == null)
            {
                MessageBox.Show("Satır verisi okunamadı.");
                return;
            }

            int satis_adet = Convert.ToInt32(row["satis_adet"]);

            if (iade_miktar > satis_adet)
            {
                MessageBox.Show("İade miktarı satış adedinden fazla olamaz.");
                return;
            }

            if (MessageBox.Show($"{iade_miktar} adet iade yapılacak. Onaylıyor musunuz?", "İade Onayı", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            int satisId = Convert.ToInt32(row["satis_id"]);
            int urunId = Convert.ToInt32(row["urun_id"]);

            using var cnn = new MySqlConnection(cnnStr);
            cnn.Open();
            var trans = cnn.BeginTransaction();

            try
            {
                var iadeGuncelle = new MySqlCommand(
                    "UPDATE satislar SET iade_adet = COALESCE(iade_adet, 0) + @iade WHERE satis_id = @id", cnn, trans);
                iadeGuncelle.Parameters.AddWithValue("@iade", iade_miktar);
                iadeGuncelle.Parameters.AddWithValue("@id", satisId);
                iadeGuncelle.ExecuteNonQuery();

                var stokGuncelle = new MySqlCommand(
                    "UPDATE urunler SET urun_adet = urun_adet + @adet WHERE urun_id = @id", cnn, trans);
                stokGuncelle.Parameters.AddWithValue("@adet", iade_miktar);
                stokGuncelle.Parameters.AddWithValue("@id", urunId);
                stokGuncelle.ExecuteNonQuery();

                trans.Commit();

                MessageBox.Show($"İade tamamlandı.");
            }
            catch (Exception ex)
            {
                trans.Rollback();
                MessageBox.Show("İade hatası: " + ex.Message);
            }

            SatislariGetir();
            UrunleriGetir();
            MusterileriGetir();
            cmbUrun.SelectedIndex = -1;
            Temizle();

            foreach (Form form in Application.OpenForms)
            {
                if (form is frmUrunler urunForm)
                {
                    urunForm.UrunleriListele();
                    break;
                }
            }
        }






        private void Temizle()
        {
            cmbMusteri.SelectedIndex = -1;

            cmbUrun.SelectedIndex = -1;

            txtFiyat.Text = "0.00";

            numAdet.Value = 1;

            txtIadeAdet.Text = "0";

            txtKarZarar.Text = "0.00";

            gridView1.ClearSelection();
        }



        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow row = gridView1.GetDataRow(e.FocusedRowHandle);
            if (row != null)
            {
                // MÜŞTERİ ID'YE GÖRE SEÇ
                if (row.Table.Columns.Contains("musteri_id") && row["musteri_id"] != DBNull.Value)
                {
                    cmbMusteri.SelectedValue = row["musteri_id"];
                }
                else
                {
                    cmbMusteri.SelectedIndex = -1;
                }

                // ÜRÜN ID'YE GÖRE SEÇ
                if (row.Table.Columns.Contains("urun_id") && row["urun_id"] != DBNull.Value)
                {
                    cmbUrun.SelectedValue = row["urun_id"];
                }
                else
                {
                    cmbUrun.SelectedIndex = -1;
                }

                // SATIŞ ADET
                if (row.Table.Columns.Contains("satis_adet") && row["satis_adet"] != DBNull.Value)
                {
                    numAdet.Value = Convert.ToDecimal(row["satis_adet"]);
                }
                else
                {
                    numAdet.Value = 1;
                }

                // TOPLAM FİYAT
                txtFiyat.Text = row.Table.Columns.Contains("toplam_fiyat") && row["toplam_fiyat"] != DBNull.Value
                                ? Convert.ToDecimal(row["toplam_fiyat"]).ToString("0.00")
                                : "0.00";

                // İADE MİKTAR (DÜZELTİLDİ: iade_adet yerine iade_miktar)
                txtIadeAdet.Text = row.Table.Columns.Contains("iade_miktar") && row["iade_miktar"] != DBNull.Value
                                   ? row["iade_miktar"].ToString()
                                   : "0";

                // KAR/ZARAR
                txtKarZarar.Text = row.Table.Columns.Contains("urun_karzarar") && row["urun_karzarar"] != DBNull.Value
                                   ? Convert.ToDecimal(row["urun_karzarar"]).ToString("0.00")
                                   : "0.00";

                // TARİH
                if (row.Table.Columns.Contains("satis_tarih") && row["satis_tarih"] != DBNull.Value)
                {
                    dateTarih.Text = Convert.ToDateTime(row["satis_tarih"]).ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    dateTarih.Text = "";
                }
            }
            else
            {
                Temizle();
            }
        }




        private void btnGüncelle_Click(object sender, EventArgs e)
        {
            if (gridView1.FocusedRowHandle < 0)
            {
                MessageBox.Show("Lütfen güncellenecek bir satış seçin.");
                return;
            }

            if (cmbMusteri.SelectedValue == null || cmbUrun.SelectedValue == null || numAdet.Value <= 0)
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.");
                return;
            }

            if (!int.TryParse(txtIadeAdet.Text, out int iadeAdet) || iadeAdet < 0)
            {
                MessageBox.Show("Geçerli bir iade miktarı giriniz.");
                return;
            }

            DataRow row = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            if (row == null)
            {
                MessageBox.Show("Seçili kayıt okunamadı.");
                return;
            }

            int satisId = Convert.ToInt32(row["satis_id"]);
            int eskiAdet = Convert.ToInt32(row["satis_adet"]);
            int eskiIadeAdet = Convert.ToInt32(row["iade_miktar"]);
            int yeniAdet = (int)numAdet.Value;
            int farkAdet = yeniAdet - eskiAdet;
            int iadeFarki = iadeAdet - eskiIadeAdet;

            if (iadeAdet > yeniAdet)
            {
                MessageBox.Show("İade miktarı satış adedinden fazla olamaz.");
                return;
            }

            int urunId = Convert.ToInt32(cmbUrun.SelectedValue);
            int musteriId = Convert.ToInt32(cmbMusteri.SelectedValue);

            using var cnn = new MySqlConnection(cnnStr);
            cnn.Open();

            var stokCmd = new MySqlCommand("SELECT urun_adet, urun_satisfiyat, urun_alisfiyat FROM urunler WHERE urun_id = @id", cnn);
            stokCmd.Parameters.AddWithValue("@id", urunId);
            using var rdr = stokCmd.ExecuteReader();
            if (!rdr.Read())
            {
                MessageBox.Show("Ürün bilgisi okunamadı.");
                return;
            }

            int mevcutStok = Convert.ToInt32(rdr["urun_adet"]);
            decimal satisfiyat = Convert.ToDecimal(rdr["urun_satisfiyat"]);
            decimal alisfiyat = Convert.ToDecimal(rdr["urun_alisfiyat"]);
            rdr.Close();

            if (farkAdet > 0 && mevcutStok < farkAdet)
            {
                MessageBox.Show("Yetersiz stok. Güncelleme için yeterli ürün yok.");
                return;
            }

            decimal toplamFiyat = satisfiyat * yeniAdet;
            decimal kar = (satisfiyat - alisfiyat) * yeniAdet;

            var trans = cnn.BeginTransaction();
            try
            {
                var cmd = new MySqlCommand(@"
            UPDATE satislar 
            SET musteri_id = @m, 
                urun_id = @u, 
                satis_adet = @a, 
                toplam_fiyat = @f, 
                urun_karzarar = @k, 
                satis_tarih = @t,
                iade_adet = @iade
            WHERE satis_id = @id", cnn, trans);

                cmd.Parameters.AddWithValue("@m", musteriId);
                cmd.Parameters.AddWithValue("@u", urunId);
                cmd.Parameters.AddWithValue("@a", yeniAdet);
                cmd.Parameters.AddWithValue("@f", toplamFiyat);
                cmd.Parameters.AddWithValue("@k", kar);
                cmd.Parameters.AddWithValue("@t", DateTime.Now);
                cmd.Parameters.AddWithValue("@iade", iadeAdet);
                cmd.Parameters.AddWithValue("@id", satisId);
                cmd.ExecuteNonQuery();

                // Doğru stok güncelleme: sadece satış farkı ve iade farkı kadar
                var stokGuncelle = new MySqlCommand(
                    "UPDATE urunler SET urun_adet = urun_adet - @fark + @iadeFarki WHERE urun_id = @id", cnn, trans);
                stokGuncelle.Parameters.AddWithValue("@fark", farkAdet);
                stokGuncelle.Parameters.AddWithValue("@iadeFarki", iadeFarki);
                stokGuncelle.Parameters.AddWithValue("@id", urunId);
                stokGuncelle.ExecuteNonQuery();

                trans.Commit();
                MessageBox.Show("Satış bilgisi güncellendi.");
            }
            catch (Exception ex)
            {
                trans.Rollback();
                MessageBox.Show("Güncelleme hatası: " + ex.Message);
            }

            SatislariGetir();
            UrunleriGetir();
            MusterileriGetir();
            cmbUrun.SelectedIndex = -1;
            Temizle();

            foreach (Form form in Application.OpenForms)
            {
                if (form is frmUrunler urunForm)
                {
                    urunForm.UrunleriListele();
                    break;
                }
            }
        }




        private void btnTemizle_Click(object sender, EventArgs e)
        {
            Temizle();
        }

        private void frmSatislar_Activated(object sender, EventArgs e)
        {
            UrunleriGetir();
            MusterileriGetir();
            SatislariGetir();
        }
    }
}