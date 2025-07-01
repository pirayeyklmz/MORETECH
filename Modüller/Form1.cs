using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using MORETECH;
using MySql.Data.MySqlClient;
using StokTakip.Kullanicilar;
using StokTakip.Musteriler;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;



namespace DENEME
{
    public partial class frmMoreTech : RibbonForm
    {
        private string cnnStr = "Server=localhost;Database=stoktakip;Uid=root;Pwd=;Allow Zero Datetime=True;Convert Zero Datetime=True;";

        public frmMoreTech()
        {
            InitializeComponent();

            using (MemoryStream ms = new MemoryStream(Properties.Resources.favico))
            {
                this.Icon = new Icon(ms);
            }
            //this.Icon = null;

            if (Program.AktifKullaniciRol != "admin")
            {
                foreach (DevExpress.XtraBars.Ribbon.RibbonPage page in ribbonControl1.Pages)
                {
                    if (page != rbpMoreTech)
                    {
                        page.Visible = false;
                    }
                }
            }

            this.Text = $"Stok Takip - Giriş Yapan: {Program.AktifKullaniciAdi} ({Program.AktifKullaniciRol})";

            this.IsMdiContainer = true;

            /*xtraTabbedMdiManager1 = new DevExpress.XtraTabbedMdi.XtraTabbedMdiManager();
            xtraTabbedMdiManager1.MdiParent = this;*/
        }

        private void frmMoreTech_Load(object sender, EventArgs e)
        {
            ImportDatabase();

            Form mevcutForm = Application.OpenForms["frmAnasayfa"];
            if (mevcutForm == null)
            {
                frmAnasayfa anaSayfaFormu = new frmAnasayfa
                {
                    MdiParent = this,
                    StartPosition = FormStartPosition.CenterScreen
                };
                anaSayfaFormu.Show();
            }
        }

        private void btnAnasayfa_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form mevcutForm = Application.OpenForms["frmAnasayfa"];
            if (mevcutForm == null)
            {
                frmAnasayfa anaSayfaFormu = new frmAnasayfa
                {
                    MdiParent = this,
                    StartPosition = FormStartPosition.CenterScreen
                };
                anaSayfaFormu.Show();
            }
            else
            {
                mevcutForm.BringToFront();
            }
        }

        private void btnUrunler_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form mevcutForm = Application.OpenForms["frmUrunler"];
            if (mevcutForm == null)
            {
                frmUrunler child = new frmUrunler
                {
                    MdiParent = this,
                    StartPosition = FormStartPosition.Manual
                };

                child.Load += (s, ev) => child.UrunleriListele();
                child.Show();
            }
            else
            {
                mevcutForm.BringToFront();
            }
        }

        private void btnMusteriler_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form mevcutForm = Application.OpenForms["Frmmusteriler"];
            if (mevcutForm == null)
            {
                Frmmusteriler child = new Frmmusteriler
                {
                    MdiParent = this,
                    StartPosition = FormStartPosition.CenterScreen
                };

                child.Show();
            }
            else
            {
                mevcutForm.BringToFront();
            }
        }

        private void btnSatislar_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form mevcutForm = Application.OpenForms["frmSatislar"];
            if (mevcutForm == null)
            {
                frmSatislar child = new frmSatislar
                {
                    MdiParent = this,
                    StartPosition = FormStartPosition.CenterScreen
                };
                child.Show();
            }
            else
            {
                mevcutForm.BringToFront();
            }
        }

        private void btnFatura_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form mevcutForm = Application.OpenForms["frmFaturalar"];
            if (mevcutForm == null)
            {
                frmFaturalar child = new frmFaturalar
                {
                    MdiParent = this,
                    StartPosition = FormStartPosition.CenterScreen
                };
                child.Show();
            }
            else
            {
                mevcutForm.BringToFront();
            }
        }

        private void btnStokHareketleri_ItemClick(object sender, ItemClickEventArgs e)
        {

            Form mevcutForm = Application.OpenForms["frmStokHareketleri"];
            if (mevcutForm == null)
            {
                frmStokHareketleri child = new frmStokHareketleri
                {
                    MdiParent = this,
                    StartPosition = FormStartPosition.CenterScreen
                };
                child.Show();
            }
            else
            {
                mevcutForm.BringToFront();
            }
        }

        private void btnUrunListele_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form mevcutForm = Application.OpenForms["frmUrunlerListesi"];
            if (mevcutForm == null)
            {
                frmUrunlerListesi child = new frmUrunlerListesi
                {
                    MdiParent = this,
                    StartPosition = FormStartPosition.CenterScreen
                };
                child.Show();
            }
            else
            {
                mevcutForm.BringToFront();
            }
        }

        private void btnMusteriListele_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form mevcutForm = Application.OpenForms["frmMusteriListesi"];
            if (mevcutForm == null)
            {
                frmMusteriListesi child = new frmMusteriListesi
                {
                    MdiParent = this,
                    StartPosition = FormStartPosition.CenterScreen
                };
                child.Show();
            }
            else
            {
                mevcutForm.BringToFront();
            }
        }

        private void btnCariKart_ItemClick(object sender, ItemClickEventArgs e)
        {
            {
                Form mevcutForm = Application.OpenForms["frmCariKart"];
                if (mevcutForm == null)
                {
                    FrmCariKart child = new FrmCariKart
                    {
                        MdiParent = this,
                        StartPosition = FormStartPosition.CenterScreen
                    };
                    child.Show();
                }
                else
                {
                    mevcutForm.BringToFront();
                }
            }
        }

        private void btnDestek_ItemClick(object sender, ItemClickEventArgs e)
        {
            Form mevcutForm = Application.OpenForms["frmDestek"];
            if (mevcutForm == null)
            {

                frmDestek child = new frmDestek
                {
                    MdiParent = this,
                    StartPosition = FormStartPosition.CenterScreen,
                };

                child.Show();
            }
            else
            {
                // Zaten açıksa en öne getir
                mevcutForm.BringToFront();
            }
        }

        private void btnKullanicilar_ItemClick(object sender, ItemClickEventArgs e)
        {
            foreach (Form frm in this.MdiChildren)
            {
                if (frm is FrmKullanicilar)
                {
                    frm.Activate();
                    return;
                }
            }

            FrmKullanicilar urunListeForm = new FrmKullanicilar();
            urunListeForm.MdiParent = this;
            urunListeForm.Show();
        }

        private void ImportDatabase()
        {
            string mysqlExePath = @"C:\xampp\mysql\bin\mysql.exe";
            string sqlFilePath = Path.Combine(Application.StartupPath, "stoktakip.sql");
            string user = "root";
            string password = ""; // XAMPP MySQL için genellikle şifresizdir
            string database = "stoktakip";

            // Veritabanı yoksa oluştur
            using (MySqlConnection conn = new MySqlConnection("server=localhost;user=root;"))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS {database};", conn);
                cmd.ExecuteNonQuery();
            }

            string args = $"-u {user} {(!string.IsNullOrEmpty(password) ? $"-p{password}" : "")} {database} < \"{sqlFilePath}\"";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c \"{mysqlExePath}\" {args}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            Process proc = Process.Start(psi);
            proc.WaitForExit();
        }


    }
}
