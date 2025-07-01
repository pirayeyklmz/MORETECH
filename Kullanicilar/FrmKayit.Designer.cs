using DevExpress.Utils.Svg;
using DevExpress.XtraRichEdit.Model;
namespace StokTakip.Kullanicilar
{
    partial class FrmKayit
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmKayit));
            labelControl1 = new DevExpress.XtraEditors.LabelControl();
            txtIsim = new DevExpress.XtraEditors.TextEdit();
            txtSifre = new DevExpress.XtraEditors.TextEdit();
            labelControl2 = new DevExpress.XtraEditors.LabelControl();
            txtSifreTekrari = new DevExpress.XtraEditors.TextEdit();
            labelControl3 = new DevExpress.XtraEditors.LabelControl();
            btnKaydet = new DevExpress.XtraEditors.SimpleButton();
            btnTemizle = new DevExpress.XtraEditors.SimpleButton();
            checkBoxSifreGoster = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)txtIsim.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtSifre.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtSifreTekrari.Properties).BeginInit();
            SuspendLayout();
            // 
            // labelControl1
            // 
            labelControl1.CausesValidation = false;
            labelControl1.Location = new System.Drawing.Point(100, 246);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new System.Drawing.Size(73, 16);
            labelControl1.TabIndex = 1;
            labelControl1.Text = "Kullanıcı Adı:";
            // 
            // txtIsim
            // 
            txtIsim.CausesValidation = false;
            txtIsim.Location = new System.Drawing.Point(100, 268);
            txtIsim.Name = "txtIsim";
            txtIsim.Properties.ContextImageOptions.Alignment = DevExpress.XtraEditors.ContextImageAlignment.Far;
            txtIsim.Properties.ContextImageOptions.SvgImage = (SvgImage)resources.GetObject("txtIsim.Properties.ContextImageOptions.SvgImage");
            txtIsim.Properties.Name = "txtIsim";
            txtIsim.Size = new System.Drawing.Size(296, 44);
            txtIsim.TabIndex = 0;
            // 
            // txtSifre
            // 
            txtSifre.CausesValidation = false;
            txtSifre.EditValue = "";
            txtSifre.Location = new System.Drawing.Point(100, 340);
            txtSifre.Name = "txtSifre";
            txtSifre.Properties.ContextImageOptions.Alignment = DevExpress.XtraEditors.ContextImageAlignment.Far;
            txtSifre.Properties.ContextImageOptions.SvgImage = (SvgImage)resources.GetObject("txtSifre.Properties.ContextImageOptions.SvgImage");
            txtSifre.Properties.Name = "txtSifre";
            txtSifre.Properties.UseSystemPasswordChar = true;
            txtSifre.Size = new System.Drawing.Size(296, 44);
            txtSifre.TabIndex = 2;
            // 
            // labelControl2
            // 
            labelControl2.CausesValidation = false;
            labelControl2.Location = new System.Drawing.Point(100, 318);
            labelControl2.Name = "labelControl2";
            labelControl2.Size = new System.Drawing.Size(32, 16);
            labelControl2.TabIndex = 3;
            labelControl2.Text = "Şifre:";
            // 
            // txtSifreTekrari
            // 
            txtSifreTekrari.CausesValidation = false;
            txtSifreTekrari.EditValue = "";
            txtSifreTekrari.Location = new System.Drawing.Point(100, 429);
            txtSifreTekrari.Name = "txtSifreTekrari";
            txtSifreTekrari.Properties.ContextImageOptions.Alignment = DevExpress.XtraEditors.ContextImageAlignment.Far;
            txtSifreTekrari.Properties.ContextImageOptions.SvgImage = (SvgImage)resources.GetObject("txtSifreTekrari.Properties.ContextImageOptions.SvgImage");
            txtSifreTekrari.Properties.Name = "txtSifreTekrari";
            txtSifreTekrari.Properties.UseSystemPasswordChar = true;
            txtSifreTekrari.Size = new System.Drawing.Size(296, 44);
            txtSifreTekrari.TabIndex = 4;
            // 
            // labelControl3
            // 
            labelControl3.CausesValidation = false;
            labelControl3.Location = new System.Drawing.Point(100, 407);
            labelControl3.Name = "labelControl3";
            labelControl3.Size = new System.Drawing.Size(77, 16);
            labelControl3.TabIndex = 5;
            labelControl3.Text = "Şifre Tekrarı:";
            // 
            // btnKaydet
            // 
            btnKaydet.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnKaydet.ImageOptions.Image");
            btnKaydet.Location = new System.Drawing.Point(100, 527);
            btnKaydet.Name = "btnKaydet";
            btnKaydet.Size = new System.Drawing.Size(100, 41);
            btnKaydet.TabIndex = 6;
            btnKaydet.Text = "Kayıt Ol";
            btnKaydet.Click += btnKaydet_Click;
            // 
            // btnTemizle
            // 
            btnTemizle.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnTemizle.ImageOptions.Image");
            btnTemizle.Location = new System.Drawing.Point(296, 527);
            btnTemizle.Name = "btnTemizle";
            btnTemizle.Size = new System.Drawing.Size(100, 41);
            btnTemizle.TabIndex = 7;
            btnTemizle.Text = "Temizle";
            btnTemizle.Click += btnTemizle_Click;
            // 
            // checkBoxSifreGoster
            // 
            checkBoxSifreGoster.AutoSize = true;
            checkBoxSifreGoster.Location = new System.Drawing.Point(299, 390);
            checkBoxSifreGoster.Name = "checkBoxSifreGoster";
            checkBoxSifreGoster.Size = new System.Drawing.Size(97, 20);
            checkBoxSifreGoster.TabIndex = 8;
            checkBoxSifreGoster.Text = "Şifre Göster";
            checkBoxSifreGoster.UseVisualStyleBackColor = true;
            checkBoxSifreGoster.CheckedChanged += checkBoxSifreGoster_CheckedChanged;
            // 
            // FrmKayit
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackgroundImageLayoutStore = System.Windows.Forms.ImageLayout.Tile;
            BackgroundImageStore = (System.Drawing.Image)resources.GetObject("$this.BackgroundImageStore");
            ClientSize = new System.Drawing.Size(498, 710);
            Controls.Add(checkBoxSifreGoster);
            Controls.Add(btnTemizle);
            Controls.Add(btnKaydet);
            Controls.Add(txtSifreTekrari);
            Controls.Add(labelControl3);
            Controls.Add(txtSifre);
            Controls.Add(labelControl2);
            Controls.Add(txtIsim);
            Controls.Add(labelControl1);
            IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("FrmKayit.IconOptions.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmKayit";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "KAYIT OL";
            Load += FrmKayit_Load;
            ((System.ComponentModel.ISupportInitialize)txtIsim.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtSifre.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtSifreTekrari.Properties).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtIsim;
        private DevExpress.XtraEditors.TextEdit txtSifre;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit txtSifreTekrari;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.SimpleButton btnKaydet;
        private DevExpress.XtraEditors.SimpleButton btnTemizle;
        private System.Windows.Forms.CheckBox checkBoxSifreGoster;
    }
}