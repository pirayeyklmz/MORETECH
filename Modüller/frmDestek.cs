using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DENEME
{
    public partial class frmDestek : DevExpress.XtraEditors.XtraForm
    {
        public frmDestek()
        {
            InitializeComponent();
            //this.Icon = new Icon("Resources/favico.ico"); // tam yol veya resource kullanımıyla
            this.Load += frmDestek_Load;
            btnIslemGecmisi.Click += btnIslemGecmisi_Click;
        }

        private void frmDestek_Load(object sender, EventArgs e)
        {
            // Sadece admin görebilsin ve tıklayabilsin
            bool adminMi = Program.AktifKullaniciRol == "admin";
            btnIslemGecmisi.Visible = adminMi;
            btnIslemGecmisi.Enabled = adminMi;
        }
        private void btnTalepOlustur_Click(object sender, EventArgs e)
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm is frmTalepOlustur)
                {
                    frm.BringToFront();
                    frm.WindowState = FormWindowState.Normal;
                    return;
                }
            }

            frmTalepOlustur form = new frmTalepOlustur();
            form.MdiParent = this.MdiParent;
            form.Show();
        }

        private void btnIslemGecmisi_Click(object sender, EventArgs e)
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm is frmIslemGecmisi)
                {
                    frm.BringToFront();
                    frm.WindowState = FormWindowState.Normal;
                    return;
                }
            }

            bool adminMi = Program.AktifKullaniciRol == "admin";
            frmIslemGecmisi form = new frmIslemGecmisi(adminMi);
            form.MdiParent = this.MdiParent;
            form.Show();
        }

        private void btnGeriBildirim_Click(object sender, EventArgs e)
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm is frmGeriBildirim)
                {
                    frm.BringToFront();
                    frm.WindowState = FormWindowState.Normal;
                    return;
                }
            }

            frmGeriBildirim geriForm = new frmGeriBildirim();
            geriForm.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void labelControl2_Click(object sender, EventArgs e)
        {

        }
    }

}