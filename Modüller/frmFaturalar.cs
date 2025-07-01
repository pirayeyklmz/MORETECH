using DENEME.Reports;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraReports.UI;
using MORETECH;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;

namespace DENEME
{
    public partial class frmFaturalar : Form
    {
        // *** merkezî bağlantı cümlesi
        private readonly string cnnStr = "Server=localhost;Database=stoktakip;Uid=root;Pwd=;" + "SslMode=none;Connection Timeout=30;Charset=utf8;";

        public frmFaturalar()
        {
            InitializeComponent();
            txtFaturaNo.KeyPress += txtFaturaNo_KeyPress;
            this.Activated += frmFaturalar_Activated;
            this.Load += frmFaturalar_Load;

            //this.gridViewFaturalar.FocusedRowChanged += gridViewFaturalar_FocusedRowChanged;
        }

        #region === EKRAN AÇILIŞI ===
        private void frmFaturalar_Load(object sender, EventArgs e)
        {
            FaturalariGetir();      // grid için
            MusterileriGetir();     // comboboxlar
            OdemeSekilleriniYukle();
            FaturaTipleriniYukle();
            KdvOranlariniYukle();
            UrunleriGetir();

            //LoadInvoices();

            DataTable dtFaturalar = FaturalariGetir();
            gridControlFaturalar.DataSource = dtFaturalar;
            gridViewFaturalar.PopulateColumns();
            gridViewFaturalar.OptionsCustomization.AllowColumnMoving = false;

            ColumnNamesForGrid();
            gridViewFaturalar.BestFitColumns();
            gridViewFaturalar.RefreshData();


        }
        #endregion

        private void ColumnNamesForGrid()
        {
            var map = new Dictionary<string, string>
            {
                ["fatura_id"] = "FATURA ID",
                ["fatura_no"] = "FATURA NO",
                ["fatura_tipi"] = "FATURA TİPİ",
                ["tarih"] = "TARİH",
                ["musteri_id"] = "MÜŞTERİ ID",
                ["musteri_ad"] = "MÜŞTERİ ADI",
                ["musteri_tip"] = "MÜŞTERİ TİPİ",
                ["urun_adi"] = "ÜRÜN ADI",
                ["kdv"] = "KDV ORANI",
                ["odeme_sekli"] = "ÖDEME ŞEKLİ",
                ["miktar"] = "MİKTAR",
                ["toplam_tutar"] = "TOPLAM TUTAR"
            };
            foreach (var kv in map)
                if (gridViewFaturalar.Columns[kv.Key] is { } col) col.Caption = kv.Value;
        }

        #region === YARDIMCI VERİ ÇAĞIRANLAR ===
        private void UrunleriGetir()
        {
            try
            {
                using var cnn = new MySqlConnection(cnnStr);
                using var da = new MySqlDataAdapter(@"
                    SELECT DISTINCT u.urun_id, u.urun_ad 
                    FROM urunler u
                    INNER JOIN satislar s ON u.urun_id = s.urun_id", cnn);

                DataTable dt = new();
                da.Fill(dt);

                cmbUrunAdi.DataSource = dt;
                cmbUrunAdi.DisplayMember = "urun_ad";
                cmbUrunAdi.ValueMember = "urun_id";
                cmbUrunAdi.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Satışı yapılan ürünler getirilirken hata oluştu: " + ex.Message);
            }
        }

        /// <summary>Grid dolduran temel SELECT (tüm gerekli kolonlar var)</summary>
        private DataTable FaturalariGetir()
        {
            DataTable dt = new();
            try
            {
                string query = @"
                    SELECT 
                        f.fatura_id,
                        f.fatura_no,
                        f.fatura_tipi,
                        f.tarih,
                        m.musteri_id,
                        f.musteri_ad,
                        f.musteri_tip,
                        f.urun_adi,
                        f.kdv,
                        f.odeme_sekli,
                        f.miktar,
                        f.toplam_tutar
                    FROM faturalar f
                    JOIN musteriler m ON f.musteri_id = m.musteri_id;";

                using var cnn = new MySqlConnection(cnnStr);
                using var cmd = new MySqlCommand(query, cnn);
                using var adp = new MySqlDataAdapter(cmd);
                adp.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı hatası: " + ex.Message);
            }
            return dt;
        }

        private void MusterileriGetir()
        {
            try
            {
                using var cnn = new MySqlConnection(cnnStr);
                using var da = new MySqlDataAdapter(
                    @"SELECT musteri_id, CONCAT(musteri_ad, ' ', musteri_soyad) AS musteri_adi, 
                             musteri_tip, musteri_ad
                      FROM musteriler", cnn);

                DataTable dt = new();
                da.Fill(dt);

                cmbMusteri.DataSource = dt;
                cmbMusteri.DisplayMember = "musteri_adi";
                cmbMusteri.ValueMember = "musteri_id";
                cmbMusteri.SelectedIndex = -1;

                var musteriTipleri = dt.AsEnumerable()
                    .Select(r => r.Field<string>("musteri_tip"))
                    .Where(t => !string.IsNullOrEmpty(t))
                    .Distinct()
                    .ToList();

                cmbMusteriTipi.DataSource = musteriTipleri;
                cmbMusteriTipi.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Müşteriler getirilirken hata oluştu: {ex.Message}");
            }
        }

        private void OdemeSekilleriniYukle() =>
            cmbOdemeSekli.DataSource = new List<string> { "Nakit", "Kredi Kartı", "Havale/EFT", "Çek", "Online Ödeme" };

        private void FaturaTipleriniYukle()
        {
            cmbFaturaTipi.Items.Clear();
            cmbFaturaTipi.Items.AddRange(new[] { "Satış", "Alış", "İade" });
        }

        private void KdvOranlariniYukle()
        {
            try
            {
                using var cnn = new MySqlConnection(cnnStr);
                using var da = new MySqlDataAdapter(
                    "SELECT DISTINCT urun_kdv FROM urunler WHERE urun_kdv IS NOT NULL ORDER BY urun_kdv", cnn);

                DataTable dt = new();
                da.Fill(dt);

                cmbKDV.DataSource = dt;
                cmbKDV.DisplayMember = "urun_kdv";
                cmbKDV.ValueMember = "urun_kdv";
                cmbKDV.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"KDV oranları getirilirken hata oluştu: {ex.Message}");
            }
        }
        #endregion

        #region === YENİ FATURA EKLE ===
        private void btnFaturaEkle_Click(object sender, EventArgs e)
        {
            // -- 1) Temel validasyon
            string faturaNo = txtFaturaNo.Text.Trim();

            // 13 haneli mi ve tamamen sayısal mı kontrolü
            if (faturaNo.Length != 13 || !long.TryParse(faturaNo, out _))
            {
                MessageBox.Show("Lütfen 13 haneli geçerli bir sayısal fatura numarası giriniz.");
                return;
            }

            if (cmbMusteri.SelectedValue == null || cmbMusteriTipi.SelectedItem == null ||
                cmbOdemeSekli.SelectedItem == null || cmbFaturaTipi.SelectedItem == null ||
                cmbUrunAdi.SelectedValue == null || dtpFaturaTarihi.Value == default(DateTime) ||
                    string.IsNullOrWhiteSpace(cmbMiktar.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.");
                return;
            }

            using var cnn = new MySqlConnection(cnnStr);
            cnn.Open();

            // -- 2) Fatura numarası benzersiz mi?
            using (var chkCmd = new MySqlCommand("SELECT COUNT(*) FROM faturalar WHERE fatura_no=@no", cnn))
            {
                chkCmd.Parameters.AddWithValue("@no", faturaNo);
                if (Convert.ToInt32(chkCmd.ExecuteScalar()) > 0)
                {
                    MessageBox.Show("Bu fatura numarası zaten kullanılmış. Lütfen farklı bir numara giriniz.");
                    return;
                }
            }

            // -- 3) Gerekli değişkenler
            int musteriId = Convert.ToInt32(cmbMusteri.SelectedValue);
            string musteriTip = cmbMusteriTipi.SelectedItem.ToString();
            string musteriAd = GetMusteriAd(musteriId);
            DateTime tarih = dtpFaturaTarihi.Value;
            string odemeSekli = cmbOdemeSekli.SelectedItem.ToString();
            string faturaTipi = cmbFaturaTipi.SelectedItem.ToString();
            int urunId = Convert.ToInt32(cmbUrunAdi.SelectedValue);
            string urunAdi = cmbUrunAdi.Text;

            // Faturalanacak miktar
            if (!int.TryParse(cmbMiktar.Text, out int faturaAdet) || faturaAdet <= 0)
            {
                MessageBox.Show("Lütfen geçerli bir fatura miktarı giriniz.");
                return;
            }

            // Toplam satış miktarına karşı kontrol
            int toplamSatisAdet = GetSatisMiktari(musteriId, urunId, cnn);
            if (faturaAdet > toplamSatisAdet)
            {
                MessageBox.Show($"Bu müşteri için maksimum {toplamSatisAdet} adet ürün satılmış. Daha fazla miktar faturalandırılamaz.");
                return;
            }

            // ürün fiyat/KDV
            (decimal satisFiyat, decimal kdvOrani) = GetUrunFiyatKdv(urunId, cnn);
            decimal araToplam = satisFiyat * faturaAdet;
            decimal toplamTutar = araToplam + kdvOrani;  // KDV tutarını doğrudan ekliyoruz

            // -- 4) Faturayı ekle
            long faturaId;
            string insertFatura = @"
        INSERT INTO faturalar
          (fatura_no, musteri_id, musteri_ad, musteri_tip,
           tarih, urun_id, urun_adi, toplam_tutar,
           kdv, odeme_sekli, fatura_tipi, miktar)
        VALUES (@no,@mid,@mad,@mtip,@tarih,
                @uid,@uadi,@top,@kdv,@odeme,@ftip,@miktar)";
            using (var cmd = new MySqlCommand(insertFatura, cnn))
            {
                cmd.Parameters.AddWithValue("@no", faturaNo);
                cmd.Parameters.AddWithValue("@mid", musteriId);
                cmd.Parameters.AddWithValue("@mad", musteriAd);
                cmd.Parameters.AddWithValue("@mtip", musteriTip);
                cmd.Parameters.AddWithValue("@tarih", tarih);
                cmd.Parameters.AddWithValue("@uid", urunId);
                cmd.Parameters.AddWithValue("@uadi", urunAdi);
                cmd.Parameters.AddWithValue("@top", toplamTutar);
                cmd.Parameters.AddWithValue("@kdv", kdvOrani); // KDV oranı değil, doğrudan tutar
                cmd.Parameters.AddWithValue("@odeme", odemeSekli);
                cmd.Parameters.AddWithValue("@ftip", faturaTipi);
                cmd.Parameters.AddWithValue("@miktar", faturaAdet);

                cmd.ExecuteNonQuery();
                faturaId = cmd.LastInsertedId;
            }

            // -- 5) Detay satırını ekle
            string insertDetay = @"
        INSERT INTO fatura_icerik
          (fatura_id, fatura_no, urun_id, urun_adi,
           urun_toplam, miktar, toplam_tutar, kdv)
        VALUES (@fid,@no,@uid,@uadi,@urunToplam,@miktar,@toplam,@kdv)";
            using (var cmd = new MySqlCommand(insertDetay, cnn))
            {
                cmd.Parameters.AddWithValue("@fid", faturaId);
                cmd.Parameters.AddWithValue("@no", faturaNo);
                cmd.Parameters.AddWithValue("@uid", urunId);
                cmd.Parameters.AddWithValue("@uadi", urunAdi);
                cmd.Parameters.AddWithValue("@urunToplam", araToplam);
                cmd.Parameters.AddWithValue("@miktar", faturaAdet);
                cmd.Parameters.AddWithValue("@toplam", satisFiyat); // birim fiyat
                cmd.Parameters.AddWithValue("@kdv", kdvOrani); // KDV tutarı
                cmd.ExecuteNonQuery();
            }

            // -- 6) Stok güncelle
            StokGuncelle(faturaTipi, urunId, faturaAdet, cnn);

            MessageBox.Show("Fatura başarıyla kaydedildi.");
            gridControlFaturalar.DataSource = FaturalariGetir();
            gridViewFaturalar.PopulateColumns();
            gridViewFaturalar.BestFitColumns();

            UrunleriGetir();
            MusterileriGetir();
            Temizle();
        }

        private int GetSatisMiktari(int musteriId, int urunId, MySqlConnection cnn)
        {
            using var cmd = new MySqlCommand(
                @"SELECT SUM(satis_adet) FROM satislar 
                  WHERE urun_id=@uid AND musteri_id=@mid", cnn);
            cmd.Parameters.AddWithValue("@uid", urunId);
            cmd.Parameters.AddWithValue("@mid", musteriId);
            object res = cmd.ExecuteScalar();
            return (res != DBNull.Value && res != null) ? Convert.ToInt32(res) : 0;
        }

        private (decimal satisFiyat, decimal kdvOrani) GetUrunFiyatKdv(int urunId, MySqlConnection cnn)
        {
            decimal fiyat = 0, kdv = 0;
            using var cmd = new MySqlCommand(
                "SELECT urun_satisfiyat, urun_kdv FROM urunler WHERE urun_id=@uid", cnn);
            cmd.Parameters.AddWithValue("@uid", urunId);
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                fiyat = rdr.GetDecimal("urun_satisfiyat");
                kdv = rdr.GetDecimal("urun_kdv");
            }
            return (fiyat, kdv);
        }

        private void StokGuncelle(string faturaTipi, int urunId, int miktar, MySqlConnection cnn)
        {
            string stokQuery = faturaTipi switch
            {
                "Alış" or "İade" => "UPDATE urunler SET urun_adet = urun_adet + @adet WHERE urun_id=@uid",
                "Satış" => "UPDATE urunler SET urun_adet = urun_adet - @adet WHERE urun_id=@uid",
                _ => null
            };

            if (stokQuery != null)
            {
                using var cmd = new MySqlCommand(stokQuery, cnn);
                cmd.Parameters.AddWithValue("@adet", miktar);
                cmd.Parameters.AddWithValue("@uid", urunId);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region === GRİD / SEÇİM ===
        private void gridViewFaturalar_RowClick(object sender, RowClickEventArgs e)
        {
            if (gridViewFaturalar.FocusedRowHandle < 0) return;

            DataRow row = gridViewFaturalar.GetDataRow(gridViewFaturalar.FocusedRowHandle);
            if (row == null) return;

            txtFaturaNo.Text = row["fatura_no"].ToString();
            cmbMusteri.SelectedValue = row["musteri_id"];
            cmbMusteriTipi.SelectedItem = row["musteri_tip"].ToString();
            dtpFaturaTarihi.Value = Convert.ToDateTime(row["tarih"]);
            cmbOdemeSekli.SelectedItem = row["odeme_sekli"].ToString();
            cmbFaturaTipi.SelectedItem = row["fatura_tipi"].ToString();
            cmbUrunAdi.Text = row["urun_adi"].ToString();
            cmbKDV.Text = row["kdv"].ToString();

            decimal toplam = GetToplamTutarFromFaturaIcerik(Convert.ToInt32(row["fatura_id"]));
            txtToplamTutar.Text = toplam.ToString("N2");

            ToplamTutariHesapla();
        }
        #endregion

        #region === FATURA ÖNİZLEME / SİL / TOPLAM ===

        private void btnFaturaSil_Click(object sender, EventArgs e)
        {
            if (gridViewFaturalar.FocusedRowHandle < 0)
            {
                MessageBox.Show("Silinecek bir fatura seçmediniz.");
                return;
            }

            string faturaNo = gridViewFaturalar.GetFocusedRowCellValue("fatura_no")?.ToString();
            if (string.IsNullOrEmpty(faturaNo))
            {
                MessageBox.Show("Fatura numarası alınamadı.");
                return;
            }

            if (MessageBox.Show("Faturayı silmek istediğinize emin misiniz?",
                                "Fatura Sil", MessageBoxButtons.YesNo) == DialogResult.No) return;

            try
            {
                using var cnn = new MySqlConnection(cnnStr);
                cnn.Open();

                int rows = new MySqlCommand("DELETE FROM faturalar WHERE fatura_no=@no", cnn)
                {
                    Parameters = { new MySqlParameter("@no", faturaNo) }
                }.ExecuteNonQuery();

                if (rows > 0)
                {
                    MessageBox.Show("Fatura başarıyla silindi.");
                    gridControlFaturalar.DataSource = FaturalariGetir();
                    gridViewFaturalar.PopulateColumns();
                    gridViewFaturalar.BestFitColumns();
                }
                else MessageBox.Show("Silinecek fatura bulunamadı.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fatura silinirken hata oluştu: " + ex.Message);
            }
        }

        private void ToplamTutariHesapla()
        {
            decimal toplam = 0;
            for (int i = 0; i < gridViewFaturalar.RowCount; i++)
            {
                if (decimal.TryParse(gridViewFaturalar.GetRowCellValue(i, "toplam_tutar")?.ToString(), out decimal tutar))
                    toplam += tutar;
            }
            txtToplamTutar.Text = toplam.ToString("C2");
        }
        #endregion

        #region === VERİ ÇEKEN YARDIMCI FONKSİYONLAR ===
        private int GetSelectedInvoiceId()
        {
            DataRow row = gridViewFaturalar.GetFocusedDataRow();
            return row != null && int.TryParse(row["fatura_id"].ToString(), out int id) ? id : 0;
        }

        private decimal GetToplamTutarFromFaturaIcerik(int faturaId)
        {
            using var cnn = new MySqlConnection(cnnStr);
            cnn.Open();
            using var cmd = new MySqlCommand(
                "SELECT SUM(toplam_tutar) FROM fatura_icerik WHERE fatura_id=@id", cnn);
            cmd.Parameters.AddWithValue("@id", faturaId);
            object res = cmd.ExecuteScalar();
            return (res != DBNull.Value && res != null) ? Convert.ToDecimal(res) : 0;
        }

        private string GetMusteriAd(int musteriId)
        {
            using var cnn = new MySqlConnection(cnnStr);
            cnn.Open();
            using var cmd = new MySqlCommand(
                "SELECT musteri_ad FROM musteriler WHERE musteri_id=@id", cnn);
            cmd.Parameters.AddWithValue("@id", musteriId);
            return cmd.ExecuteScalar()?.ToString() ?? "";
        }
        #endregion

        #region === FATURA DETAY LİSTESİ ===
        private List<InvoiceItem> GetInvoiceItemsFromDatabase(int faturaId)
        {
            List<InvoiceItem> list = new List<InvoiceItem>();

            string query = @"
SELECT 
    fi.fatura_id,
    fi.fatura_no,
    fi.urun_adi,
    u.urun_kdv AS urun_kdv,
    fi.miktar AS urun_adet,
    u.urun_satisfiyat AS birim_fiyat,
    fi.toplam_tutar,
    f.fatura_tipi,
    f.tarih,
    f.musteri_id,
    f.musteri_ad,
    f.musteri_tip,
    f.odeme_sekli
FROM fatura_icerik fi
JOIN faturalar f ON fi.fatura_id = f.fatura_id
JOIN urunler u ON u.urun_ad = fi.urun_adi
WHERE fi.fatura_id = @faturaId";

            using (MySqlConnection conn = new MySqlConnection(cnnStr))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@faturaId", faturaId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new InvoiceItem
                            {
                                FaturaID = reader.GetInt32("fatura_id"),
                                FaturaNo = reader["fatura_no"].ToString(),
                                UrunAdi = reader["urun_adi"].ToString(),
                                KdvOrani = reader.GetDecimal("urun_kdv"),
                                Adet = reader.GetInt32("urun_adet"),
                                BirimFiyat = reader.IsDBNull(reader.GetOrdinal("birim_fiyat"))
                                             ? 0
                                             : Convert.ToDecimal(reader["birim_fiyat"]),
                                ToplamTutar = reader.IsDBNull(reader.GetOrdinal("toplam_tutar"))
                                              ? 0
                                              : Convert.ToDecimal(reader["toplam_tutar"]),
                                FaturaTipi = reader["fatura_tipi"].ToString(),
                                Tarih = reader.GetDateTime("tarih"),
                                MusteriID = reader.GetInt32("musteri_id"),
                                MusteriAdi = reader["musteri_ad"].ToString(),
                                MusteriTipi = reader["musteri_tip"].ToString(),
                                OdemeSekli = reader["odeme_sekli"].ToString()
                            });
                        }
                    }
                }
            }

            return list;
        }


        #endregion

        #region === EKSİK KÜÇÜK EVENT’LER ===
        private void Temizle()
        {
            txtFaturaNo.Clear();
            dtpFaturaTarihi.Value = DateTime.Now;
            cmbFaturaTipi.SelectedIndex =
            cmbOdemeSekli.SelectedIndex =
            cmbUrunAdi.SelectedIndex =
            cmbMusteri.SelectedIndex =
            cmbMusteriTipi.SelectedIndex = -1;
            cmbKDV.Text = string.Empty;
            cmbMiktar.Items.Clear();
            txtToplamTutar.Clear();
        }

        private void cmbUrunAdi_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbUrunAdi.SelectedValue == null || cmbMusteri.SelectedValue == null) return;

            int urunId = Convert.ToInt32(cmbUrunAdi.SelectedValue);
            int musteriId = Convert.ToInt32(cmbMusteri.SelectedValue);

            using var cnn = new MySqlConnection(cnnStr);
            cnn.Open();
            using var cmd = new MySqlCommand(
                @"SELECT SUM(satis_adet) FROM satislar 
                  WHERE urun_id=@uid AND musteri_id=@mid", cnn);
            cmd.Parameters.AddWithValue("@uid", urunId);
            cmd.Parameters.AddWithValue("@mid", musteriId);

            object res = cmd.ExecuteScalar();
            int adet = (res != DBNull.Value && res != null) ? Convert.ToInt32(res) : 1;

            cmbMiktar.Items.Clear();
            for (int i = 1; i <= adet; i++) cmbMiktar.Items.Add(i);
            cmbMiktar.SelectedIndex = 0;
        }

        private void cmbMiktar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbUrunAdi.SelectedValue == null || cmbMiktar.SelectedItem == null) return;

            int urunId = Convert.ToInt32(cmbUrunAdi.SelectedValue);
            int miktar = Convert.ToInt32(cmbMiktar.SelectedItem);

            using var cnn = new MySqlConnection(cnnStr);
            cnn.Open();
            using var cmd = new MySqlCommand(
                "SELECT urun_satisfiyat, urun_kdv FROM urunler WHERE urun_id=@uid", cnn);
            cmd.Parameters.AddWithValue("@uid", urunId);

            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                decimal fiyat = rdr.GetDecimal("urun_satisfiyat");
                decimal kdvTutar = rdr.GetDecimal("urun_kdv");
                cmbKDV.Text = kdvTutar.ToString("0.00");
            }

        }
        private void btnGoruntule_Click(object sender, EventArgs e)
        {
            int selectedRow = gridViewFaturalar.FocusedRowHandle;

            if (selectedRow < 0)
            {
                MessageBox.Show("Geçerli bir fatura seçilmedi.");
                return;
            }

            object cellValue = gridViewFaturalar.GetRowCellValue(selectedRow, "fatura_id");
            if (cellValue == null || cellValue == DBNull.Value)
            {
                MessageBox.Show("Seçilen satırda fatura ID bulunamadı.");
                return;
            }

            if (!int.TryParse(cellValue.ToString(), out int faturaId))
            {
                MessageBox.Show("Geçersiz fatura ID.");
                return;
            }

            // Veritabanından fatura detaylarını al
            List<InvoiceItem> urunler = GetInvoiceItemsFromDatabase(faturaId);

            if (urunler == null || urunler.Count == 0)
            {
                MessageBox.Show("Bu faturaya ait ürün satırı bulunamadı.");
                return;
            }

            InvoiceItem ilkSatir = urunler.First();

            InvoiceReport report = new InvoiceReport();

            // Parametreleri ata
            report.Parameters["FaturaID"].Value = ilkSatir.FaturaID;
            report.Parameters["FaturaNo"].Value = ilkSatir.FaturaNo;
            report.Parameters["FaturaTipi"].Value = ilkSatir.FaturaTipi;
            report.Parameters["Tarih"].Value = ilkSatir.Tarih;
            report.Parameters["MusteriID"].Value = ilkSatir.MusteriID;
            report.Parameters["MusteriAdi"].Value = ilkSatir.MusteriAdi;
            report.Parameters["MusteriTipi"].Value = ilkSatir.MusteriTipi;
            report.Parameters["OdemeSekli"].Value = ilkSatir.OdemeSekli;

            decimal toplamTutar = urunler
                .Where(x => x.BirimFiyat > 0 && x.Adet > 0)
                .Sum(x => x.BirimFiyat * x.Adet);

            report.Parameters["ToplamTutar"].Value = toplamTutar;

            // Parameters panelini tamamen gizle
            foreach (var param in report.Parameters)
                param.Visible = false;

            report.RequestParameters = false;

            report.SetData(urunler); // Rapor veri kaynağı atanıyor

            ReportPrintTool tool = new ReportPrintTool(report);
            tool.ShowPreviewDialog();
        }


        #endregion

        private void frmFaturalar_Activated(object sender, EventArgs e)
        {
            FaturalariGetir();
            MusterileriGetir();
            OdemeSekilleriniYukle();
            FaturaTipleriniYukle();
            KdvOranlariniYukle();
            UrunleriGetir();
        }

        private void txtFaturaNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Tuşu iptal et
            }
        }

        private void dtpFaturaTarihi_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}