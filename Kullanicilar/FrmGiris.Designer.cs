using DevExpress.XtraRichEdit.Model;

namespace DENEME.Kullanicilar
{
    partial class FrmGiris
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmGiris));
            btnTemizle = new DevExpress.XtraEditors.SimpleButton();
            btnKaydet = new DevExpress.XtraEditors.SimpleButton();
            txtSifre = new DevExpress.XtraEditors.TextEdit();
            labelControl2 = new DevExpress.XtraEditors.LabelControl();
            txtIsim = new DevExpress.XtraEditors.TextEdit();
            labelControl1 = new DevExpress.XtraEditors.LabelControl();
            checkBoxSifreGoster = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)txtSifre.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtIsim.Properties).BeginInit();
            SuspendLayout();
            // 
            // btnTemizle
            // 
            btnTemizle.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnTemizle.ImageOptions.Image");
            btnTemizle.Location = new System.Drawing.Point(296, 520);
            btnTemizle.Name = "btnTemizle";
            btnTemizle.Size = new System.Drawing.Size(100, 41);
            btnTemizle.TabIndex = 16;
            btnTemizle.Text = "Temizle";
            btnTemizle.Click += btnTemizle_Click;
            // 
            // btnKaydet
            // 
            btnKaydet.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnKaydet.ImageOptions.Image");
            btnKaydet.Location = new System.Drawing.Point(100, 520);
            btnKaydet.Name = "btnKaydet";
            btnKaydet.Size = new System.Drawing.Size(100, 41);
            btnKaydet.TabIndex = 15;
            btnKaydet.Text = "Giriş Yap";
            btnKaydet.Click += btnKaydet_Click;
            // 
            // txtSifre
            // 
            txtSifre.CausesValidation = false;
            txtSifre.EditValue = "";
            txtSifre.Location = new System.Drawing.Point(100, 376);
            txtSifre.Name = "txtSifre";
            txtSifre.Properties.ContextImageOptions.Alignment = DevExpress.XtraEditors.ContextImageAlignment.Far;
            txtSifre.Properties.ContextImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("txtSifre.Properties.ContextImageOptions.SvgImage");
            txtSifre.Properties.Name = "txtSifre";
            txtSifre.Properties.UseSystemPasswordChar = true;
            txtSifre.Size = new System.Drawing.Size(296, 44);
            txtSifre.TabIndex = 11;
            // 
            // labelControl2
            // 
            labelControl2.CausesValidation = false;
            labelControl2.Location = new System.Drawing.Point(100, 354);
            labelControl2.Name = "labelControl2";
            labelControl2.Size = new System.Drawing.Size(32, 16);
            labelControl2.TabIndex = 12;
            labelControl2.Text = "Şifre:";
            // 
            // txtIsim
            // 
            txtIsim.CausesValidation = false;
            txtIsim.Location = new System.Drawing.Point(100, 291);
            txtIsim.Name = "txtIsim";
            txtIsim.Properties.ContextImageOptions.Alignment = DevExpress.XtraEditors.ContextImageAlignment.Far;
            txtIsim.Properties.ContextImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("txtIsim.Properties.ContextImageOptions.SvgImage");
            txtIsim.Properties.Name = "txtIsim";
            txtIsim.Size = new System.Drawing.Size(296, 44);
            txtIsim.TabIndex = 8;
            // 
            // labelControl1
            // 
            labelControl1.CausesValidation = false;
            labelControl1.Location = new System.Drawing.Point(100, 260);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new System.Drawing.Size(73, 16);
            labelControl1.TabIndex = 10;
            labelControl1.Text = "Kullanıcı Adı:";
            // 
            // checkBoxSifreGoster
            // 
            checkBoxSifreGoster.AutoSize = true;
            checkBoxSifreGoster.Location = new System.Drawing.Point(299, 456);
            checkBoxSifreGoster.Name = "checkBoxSifreGoster";
            checkBoxSifreGoster.Size = new System.Drawing.Size(97, 20);
            checkBoxSifreGoster.TabIndex = 17;
            checkBoxSifreGoster.Text = "Şifre Göster";
            checkBoxSifreGoster.UseVisualStyleBackColor = true;
            checkBoxSifreGoster.CheckedChanged += checkBoxSifreGoster_CheckedChanged;
            // 
            // FrmGiris
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackgroundImageLayoutStore = System.Windows.Forms.ImageLayout.Tile;
            BackgroundImageStore = (System.Drawing.Image)resources.GetObject("$this.BackgroundImageStore");
            ClientSize = new System.Drawing.Size(498, 710);
            Controls.Add(checkBoxSifreGoster);
            Controls.Add(btnTemizle);
            Controls.Add(btnKaydet);
            Controls.Add(txtSifre);
            Controls.Add(labelControl2);
            Controls.Add(txtIsim);
            Controls.Add(labelControl1);
            IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("FrmGiris.IconOptions.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmGiris";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "GİRİŞ YAP";
            ((System.ComponentModel.ISupportInitialize)txtSifre.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtIsim.Properties).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnTemizle;
        private DevExpress.XtraEditors.SimpleButton btnKaydet;
        private DevExpress.XtraEditors.TextEdit txtSifre;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit txtIsim;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private System.Windows.Forms.CheckBox checkBoxSifreGoster;
    }
}