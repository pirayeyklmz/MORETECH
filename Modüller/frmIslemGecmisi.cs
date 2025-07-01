using DevExpress.XtraEditors;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace DENEME
{
    public partial class frmIslemGecmisi : DevExpress.XtraEditors.XtraForm
    {
        private bool isAdmin;

        public frmIslemGecmisi(bool isAdmin)
        {
            InitializeComponent();
            //this.Icon = new Icon("Resources/favico.ico"); // tam yol veya resource kullanımıyla
            this.isAdmin = isAdmin;

            // 🛠 Load event bağlantısını garantiye al
            this.Load += frmIslemGecmisi_Load;
        }

        private void frmIslemGecmisi_Load(object sender, EventArgs e)
        {
            string query;

            if (isAdmin)
            {
                query = "SELECT * FROM talepler ORDER BY tarih DESC";
            }
            else
            {
                // 🛡 Kullanıcı adı boşsa uyarı ver
                if (string.IsNullOrEmpty(Program.AktifKullaniciAdi))
                {
                    MessageBox.Show("Kullanıcı adı boş! Giriş yapılmamış olabilir.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                query = "SELECT * FROM talepler WHERE kullanici_adi = @kullaniciAdi ORDER BY tarih DESC";
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection("server=localhost;database=stoktakip;uid=root;pwd=;"))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        if (!isAdmin)
                        {
                            // 🧩 Parametreli sorguya kullanıcı adını ekle
                            cmd.Parameters.AddWithValue("@kullaniciAdi", Program.AktifKullaniciAdi);
                        }

                        using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            // ✅ Grid'e veri bağla
                            gridControl1.DataSource = dt;
                            gridView1.BestFitColumns();

                            // 🧪 Debug: kaç kayıt geldi
                            Console.WriteLine($"Kayıt sayısı: {dt.Rows.Count}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri yüklenirken hata oluştu:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
