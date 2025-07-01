using DENEME.Kullanicilar;
using DevExpress.Skins;
using DevExpress.UserSkins;
using MySql.Data.MySqlClient;
using StokTakip.Kullanicilar;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DENEME
{
    internal static class Program
    {
        public static string AktifKullaniciRol = "";
        public static string AktifKullaniciAdi = "";

        [STAThread]
        static void Main()
        {

            string logFile = "log.txt";
            File.AppendAllText(logFile, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Başlatılıyor...{Environment.NewLine}");

            // [1/2] MySQL başlatılıyor
            Console.WriteLine("[1/2] MySQL başlatılıyor...");
            try
            {
                ProcessStartInfo mysqlStartInfo = new ProcessStartInfo
                {
                    FileName = "xampp\\mysql_start.bat",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                Process.Start(mysqlStartInfo);
                Thread.Sleep(5000); // 5 saniye bekle
            }
            catch (Exception ex)
            {
                File.AppendAllText(logFile, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] MySQL başlatılamadı: {ex.Message}{Environment.NewLine}");
            }

            // [2/2] Uygulama başlatılıyor
            Console.WriteLine("[2/2] Uygulama başlatılıyor...");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                if (!AdminVarMi())
                {
                    using (var kayitForm = new FrmKayit())
                    {
                        if (kayitForm.ShowDialog() != DialogResult.OK)
                        {
                            Application.Exit();
                            return;
                        }
                    }
                }

                using (var girisForm = new FrmGiris())
                {
                    if (girisForm.ShowDialog() == DialogResult.OK)
                    {
                        Application.Run(new frmMoreTech());
                    }
                    else
                    {
                        Application.Exit();
                    }
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(logFile, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Uygulama başlatılamadı: {ex.Message}{Environment.NewLine}");
                MessageBox.Show("Beklenmeyen bir hata oluştu:\n\n" + ex.ToString(),
                                "Uygulama Hatası",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private static bool AdminVarMi()
        {
            string connectionString = "server=localhost;user id=root;password=;database=stoktakip;SslMode=none;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM kullanicilar WHERE rol = 'admin'";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        int adminSayisi = Convert.ToInt32(cmd.ExecuteScalar());
                        return adminSayisi > 0;
                    }
                }
                catch
                {
                    MessageBox.Show("Veritabanına bağlanılamadı.");
                    return false;
                }
            }
        }
    }
}
