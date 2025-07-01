namespace StokTakip.Kullanicilar
{
    partial class FrmKullanicilar
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmKullanicilar));
            groupControl1 = new DevExpress.XtraEditors.GroupControl();
            memoEditNot = new DevExpress.XtraEditors.MemoEdit();
            lblControlAdres = new DevExpress.XtraEditors.LabelControl();
            comboBoxEditKullaniciRol = new DevExpress.XtraEditors.ComboBoxEdit();
            lblControlId = new DevExpress.XtraEditors.LabelControl();
            BtnKullaniciTemizle = new DevExpress.XtraEditors.SimpleButton();
            lblControlMusteriTip = new DevExpress.XtraEditors.LabelControl();
            BtnKullaniciGuncelle = new DevExpress.XtraEditors.SimpleButton();
            lblControlAd = new DevExpress.XtraEditors.LabelControl();
            BtnKullaniciSil = new DevExpress.XtraEditors.SimpleButton();
            lblControlSoyad = new DevExpress.XtraEditors.LabelControl();
            BtnKullaniciKaydet = new DevExpress.XtraEditors.SimpleButton();
            txtKullaniciId = new System.Windows.Forms.TextBox();
            txtKullaniciAd = new System.Windows.Forms.TextBox();
            txtKullaniciSifre = new System.Windows.Forms.TextBox();
            gridControl1 = new DevExpress.XtraGrid.GridControl();
            gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)groupControl1).BeginInit();
            groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)memoEditNot.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)comboBoxEditKullaniciRol.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridControl1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).BeginInit();
            SuspendLayout();
            // 
            // groupControl1
            // 
            groupControl1.Controls.Add(memoEditNot);
            groupControl1.Controls.Add(lblControlAdres);
            groupControl1.Controls.Add(comboBoxEditKullaniciRol);
            groupControl1.Controls.Add(lblControlId);
            groupControl1.Controls.Add(BtnKullaniciTemizle);
            groupControl1.Controls.Add(lblControlMusteriTip);
            groupControl1.Controls.Add(BtnKullaniciGuncelle);
            groupControl1.Controls.Add(lblControlAd);
            groupControl1.Controls.Add(BtnKullaniciSil);
            groupControl1.Controls.Add(lblControlSoyad);
            groupControl1.Controls.Add(BtnKullaniciKaydet);
            groupControl1.Controls.Add(txtKullaniciId);
            groupControl1.Controls.Add(txtKullaniciAd);
            groupControl1.Controls.Add(txtKullaniciSifre);
            groupControl1.Location = new System.Drawing.Point(961, 0);
            groupControl1.Name = "groupControl1";
            groupControl1.Size = new System.Drawing.Size(313, 585);
            groupControl1.TabIndex = 30;
            groupControl1.Text = "Kullanıcı İşlemleri";
            // 
            // memoEditNot
            // 
            memoEditNot.Location = new System.Drawing.Point(92, 146);
            memoEditNot.Name = "memoEditNot";
            memoEditNot.Properties.MaxLength = 25;
            memoEditNot.Size = new System.Drawing.Size(200, 82);
            memoEditNot.TabIndex = 4;
            // 
            // lblControlAdres
            // 
            lblControlAdres.AllowHtmlString = true;
            lblControlAdres.Location = new System.Drawing.Point(13, 145);
            lblControlAdres.Name = "lblControlAdres";
            lblControlAdres.Size = new System.Drawing.Size(24, 16);
            lblControlAdres.TabIndex = 33;
            lblControlAdres.Text = "Not:";
            // 
            // comboBoxEditKullaniciRol
            // 
            comboBoxEditKullaniciRol.Location = new System.Drawing.Point(92, 60);
            comboBoxEditKullaniciRol.Name = "comboBoxEditKullaniciRol";
            comboBoxEditKullaniciRol.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            comboBoxEditKullaniciRol.Properties.Items.AddRange(new object[] { "admin", "kullanici" });
            comboBoxEditKullaniciRol.Size = new System.Drawing.Size(200, 22);
            comboBoxEditKullaniciRol.TabIndex = 1;
            // 
            // lblControlId
            // 
            lblControlId.AllowHtmlString = true;
            lblControlId.Location = new System.Drawing.Point(13, 38);
            lblControlId.Name = "lblControlId";
            lblControlId.Size = new System.Drawing.Size(16, 16);
            lblControlId.TabIndex = 0;
            lblControlId.Text = "Id:";
            // 
            // BtnKullaniciTemizle
            // 
            BtnKullaniciTemizle.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("BtnKullaniciTemizle.ImageOptions.Image");
            BtnKullaniciTemizle.Location = new System.Drawing.Point(186, 368);
            BtnKullaniciTemizle.Name = "BtnKullaniciTemizle";
            BtnKullaniciTemizle.Size = new System.Drawing.Size(106, 38);
            BtnKullaniciTemizle.TabIndex = 8;
            BtnKullaniciTemizle.Text = "Temizle";
            BtnKullaniciTemizle.Click += BtnKullaniciTemizle_Click;
            // 
            // lblControlMusteriTip
            // 
            lblControlMusteriTip.AllowHtmlString = true;
            lblControlMusteriTip.Location = new System.Drawing.Point(13, 124);
            lblControlMusteriTip.Name = "lblControlMusteriTip";
            lblControlMusteriTip.Size = new System.Drawing.Size(41, 16);
            lblControlMusteriTip.TabIndex = 1;
            lblControlMusteriTip.Text = "Şifresi:";
            // 
            // BtnKullaniciGuncelle
            // 
            BtnKullaniciGuncelle.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("BtnKullaniciGuncelle.ImageOptions.Image");
            BtnKullaniciGuncelle.Location = new System.Drawing.Point(186, 280);
            BtnKullaniciGuncelle.Name = "BtnKullaniciGuncelle";
            BtnKullaniciGuncelle.Size = new System.Drawing.Size(106, 38);
            BtnKullaniciGuncelle.TabIndex = 6;
            BtnKullaniciGuncelle.Text = "Güncelle";
            BtnKullaniciGuncelle.Click += BtnKullaniciGuncelle_Click;
            // 
            // lblControlAd
            // 
            lblControlAd.AllowHtmlString = true;
            lblControlAd.Location = new System.Drawing.Point(13, 66);
            lblControlAd.Name = "lblControlAd";
            lblControlAd.Size = new System.Drawing.Size(30, 16);
            lblControlAd.TabIndex = 2;
            lblControlAd.Text = "Rolü:";
            // 
            // BtnKullaniciSil
            // 
            BtnKullaniciSil.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("BtnKullaniciSil.ImageOptions.Image");
            BtnKullaniciSil.Location = new System.Drawing.Point(186, 324);
            BtnKullaniciSil.Name = "BtnKullaniciSil";
            BtnKullaniciSil.Size = new System.Drawing.Size(106, 38);
            BtnKullaniciSil.TabIndex = 7;
            BtnKullaniciSil.Text = "Sil";
            BtnKullaniciSil.Click += BtnKullaniciSil_Click;
            // 
            // lblControlSoyad
            // 
            lblControlSoyad.AllowHtmlString = true;
            lblControlSoyad.Location = new System.Drawing.Point(13, 95);
            lblControlSoyad.Name = "lblControlSoyad";
            lblControlSoyad.Size = new System.Drawing.Size(23, 16);
            lblControlSoyad.TabIndex = 3;
            lblControlSoyad.Text = "Adı:";
            // 
            // BtnKullaniciKaydet
            // 
            BtnKullaniciKaydet.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("BtnKullaniciKaydet.ImageOptions.Image");
            BtnKullaniciKaydet.Location = new System.Drawing.Point(186, 236);
            BtnKullaniciKaydet.Name = "BtnKullaniciKaydet";
            BtnKullaniciKaydet.Size = new System.Drawing.Size(106, 38);
            BtnKullaniciKaydet.TabIndex = 5;
            BtnKullaniciKaydet.Text = "Kaydet";
            BtnKullaniciKaydet.Click += BtnKullaniciKaydet_Click;
            // 
            // txtKullaniciId
            // 
            txtKullaniciId.Location = new System.Drawing.Point(92, 31);
            txtKullaniciId.Name = "txtKullaniciId";
            txtKullaniciId.ReadOnly = true;
            txtKullaniciId.Size = new System.Drawing.Size(200, 23);
            txtKullaniciId.TabIndex = 0;
            // 
            // txtKullaniciAd
            // 
            txtKullaniciAd.Location = new System.Drawing.Point(92, 88);
            txtKullaniciAd.Name = "txtKullaniciAd";
            txtKullaniciAd.Size = new System.Drawing.Size(200, 23);
            txtKullaniciAd.TabIndex = 2;
            // 
            // txtKullaniciSifre
            // 
            txtKullaniciSifre.Location = new System.Drawing.Point(92, 117);
            txtKullaniciSifre.Name = "txtKullaniciSifre";
            txtKullaniciSifre.Size = new System.Drawing.Size(200, 23);
            txtKullaniciSifre.TabIndex = 3;
            // 
            // gridControl1
            // 
            gridControl1.Dock = System.Windows.Forms.DockStyle.Left;
            gridControl1.Location = new System.Drawing.Point(0, 0);
            gridControl1.MainView = gridView1;
            gridControl1.Name = "gridControl1";
            gridControl1.Size = new System.Drawing.Size(955, 748);
            gridControl1.TabIndex = 31;
            gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gridView1 });
            gridControl1.Load += FrmKullanicilar_Load;
            // 
            // gridView1
            // 
            gridView1.GridControl = gridControl1;
            gridView1.Name = "gridView1";
            // 
            // FrmKullanicilar
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1284, 748);
            Controls.Add(gridControl1);
            Controls.Add(groupControl1);
            Name = "FrmKullanicilar";
            Text = "KULLANICILAR";
            Load += FrmKullanicilar_Load;
            ((System.ComponentModel.ISupportInitialize)groupControl1).EndInit();
            groupControl1.ResumeLayout(false);
            groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)memoEditNot.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)comboBoxEditKullaniciRol.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridControl1).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.MemoEdit memoEditNot;
        private DevExpress.XtraEditors.LabelControl lblControlAdres;
        private DevExpress.XtraEditors.ComboBoxEdit comboBoxEditKullaniciRol;
        private DevExpress.XtraEditors.LabelControl lblControlId;
        private DevExpress.XtraEditors.SimpleButton BtnKullaniciTemizle;
        private DevExpress.XtraEditors.LabelControl lblControlMusteriTip;
        private DevExpress.XtraEditors.SimpleButton BtnKullaniciGuncelle;
        private DevExpress.XtraEditors.LabelControl lblControlAd;
        private DevExpress.XtraEditors.SimpleButton BtnKullaniciSil;
        private DevExpress.XtraEditors.LabelControl lblControlSoyad;
        private DevExpress.XtraEditors.SimpleButton BtnKullaniciKaydet;
        private System.Windows.Forms.TextBox txtKullaniciId;
        private System.Windows.Forms.TextBox txtKullaniciAd;
        private System.Windows.Forms.TextBox txtKullaniciSifre;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
    }
}