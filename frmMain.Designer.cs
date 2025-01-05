namespace SMG.Desktop
{
    partial class frmMain
    {

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

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            DevExpress.XtraBars.Ribbon.ReduceOperation reduceOperation1 = new DevExpress.XtraBars.Ribbon.ReduceOperation();
            DevExpress.XtraBars.Ribbon.ReduceOperation reduceOperation2 = new DevExpress.XtraBars.Ribbon.ReduceOperation();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
            this.popupMenu1 = new DevExpress.XtraBars.PopupMenu();
            this.barButtonItem5 = new DevExpress.XtraBars.BarButtonItem();
            this.popupMenu2 = new DevExpress.XtraBars.PopupMenu();
            this.barButtonGroup1 = new DevExpress.XtraBars.BarButtonGroup();
            this.barButtonItem3 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem4 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem6 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem7 = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem8 = new DevExpress.XtraBars.BarButtonItem();
            this.bbtReport = new DevExpress.XtraBars.BarButtonItem();
            this.imageCollectionMenuIcon = new DevExpress.Utils.ImageCollection();
            this.Report = new DevExpress.XtraBars.Ribbon.RibbonMiniToolbar();
            this.ribbonPage = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonStatusBar1 = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.ribbonPageGroupFile = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroupEdit = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).BeginInit();
            this.layoutControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupMenu1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupMenu2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollectionMenuIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.layoutControl2);
            this.layoutControl1.Controls.Add(this.ribbonControl1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1125, 549);
            this.layoutControl1.TabIndex = 0;
            // 
            // layoutControl2
            // 
            this.layoutControl2.Controls.Add(this.xtraTabControl1);
            this.layoutControl2.Location = new System.Drawing.Point(2, 140);
            this.layoutControl2.Name = "layoutControl2";
            this.layoutControl2.Root = this.Root;
            this.layoutControl2.Size = new System.Drawing.Size(1121, 407);
            this.layoutControl2.TabIndex = 7;
            this.layoutControl2.Text = "layoutControl2";
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.xtraTabControl1.BorderStylePage = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.xtraTabControl1.ClosePageButtonShowMode = DevExpress.XtraTab.ClosePageButtonShowMode.InActiveTabPageHeader;
            this.xtraTabControl1.Location = new System.Drawing.Point(2, 2);
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.ShowHeaderFocus = DevExpress.Utils.DefaultBoolean.True;
            this.xtraTabControl1.ShowTabHeader = DevExpress.Utils.DefaultBoolean.True;
            this.xtraTabControl1.Size = new System.Drawing.Size(1117, 403);
            this.xtraTabControl1.TabIndex = 5;
            this.xtraTabControl1.CloseButtonClick += new System.EventHandler(this.xtraTabControl1_CloseButtonClick);
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2});
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.Root.Size = new System.Drawing.Size(1121, 407);
            this.Root.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.xtraTabControl1;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(1121, 407);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.Dock = System.Windows.Forms.DockStyle.None;
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.barButtonItem1,
            this.barButtonItem2,
            this.barButtonGroup1,
            this.barButtonItem3,
            this.barButtonItem4,
            this.barButtonItem5,
            this.barButtonItem6,
            this.barButtonItem7,
            this.barButtonItem8,
            this.bbtReport});
            this.ribbonControl1.LargeImages = this.imageCollectionMenuIcon;
            this.ribbonControl1.Location = new System.Drawing.Point(2, 2);
            this.ribbonControl1.MaximumSize = new System.Drawing.Size(0, 140);
            this.ribbonControl1.MaxItemId = 5;
            this.ribbonControl1.MiniToolbars.Add(this.Report);
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage});
            this.ribbonControl1.QuickToolbarItemLinks.Add(this.barButtonItem6);
            this.ribbonControl1.QuickToolbarItemLinks.Add(this.barButtonItem7);
            this.ribbonControl1.QuickToolbarItemLinks.Add(this.barButtonItem8);
            this.ribbonControl1.QuickToolbarItemLinks.Add(this.bbtReport);
            this.ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2013;
            this.ribbonControl1.Size = new System.Drawing.Size(1121, 140);
            this.ribbonControl1.StatusBar = this.ribbonStatusBar1;
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "Module 1";
            this.barButtonItem1.Id = 15;
            this.barButtonItem1.LargeImageIndex = 0;
            this.barButtonItem1.Name = "barButtonItem1";
            // 
            // barButtonItem2
            // 
            this.barButtonItem2.ActAsDropDown = true;
            this.barButtonItem2.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
            this.barButtonItem2.Caption = "Module 2";
            this.barButtonItem2.DropDownControl = this.popupMenu1;
            this.barButtonItem2.Id = 1;
            this.barButtonItem2.LargeImageIndex = 1;
            this.barButtonItem2.Name = "barButtonItem2";
            // 
            // popupMenu1
            // 
            this.popupMenu1.ItemLinks.Add(this.barButtonItem5);
            this.popupMenu1.Name = "popupMenu1";
            this.popupMenu1.Ribbon = this.ribbonControl1;
            // 
            // barButtonItem5
            // 
            this.barButtonItem5.ActAsDropDown = true;
            this.barButtonItem5.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
            this.barButtonItem5.Caption = "barButtonItem5";
            this.barButtonItem5.DropDownControl = this.popupMenu2;
            this.barButtonItem5.Id = 1;
            this.barButtonItem5.Name = "barButtonItem5";
            // 
            // popupMenu2
            // 
            this.popupMenu2.Name = "popupMenu2";
            this.popupMenu2.Ribbon = this.ribbonControl1;
            // 
            // barButtonGroup1
            // 
            this.barButtonGroup1.Caption = "barButtonGroup1";
            this.barButtonGroup1.Id = 2;
            this.barButtonGroup1.ImageUri.Uri = "ListNumbers";
            this.barButtonGroup1.Name = "barButtonGroup1";
            // 
            // barButtonItem3
            // 
            this.barButtonItem3.Caption = "Module 1";
            this.barButtonItem3.Id = 1;
            this.barButtonItem3.Name = "barButtonItem3";
            // 
            // barButtonItem4
            // 
            this.barButtonItem4.Caption = "Report";
            this.barButtonItem4.Id = 2;
            this.barButtonItem4.Name = "barButtonItem4";
            // 
            // barButtonItem6
            // 
            this.barButtonItem6.Caption = "Account";
            this.barButtonItem6.Glyph = ((System.Drawing.Image)(resources.GetObject("barButtonItem6.Glyph")));
            this.barButtonItem6.Id = 1;
            this.barButtonItem6.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("barButtonItem6.LargeGlyph")));
            this.barButtonItem6.Name = "barButtonItem6";
            // 
            // barButtonItem7
            // 
            this.barButtonItem7.Caption = "Cart";
            this.barButtonItem7.Glyph = ((System.Drawing.Image)(resources.GetObject("barButtonItem7.Glyph")));
            this.barButtonItem7.Id = 2;
            this.barButtonItem7.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("barButtonItem7.LargeGlyph")));
            this.barButtonItem7.Name = "barButtonItem7";
            // 
            // barButtonItem8
            // 
            this.barButtonItem8.Caption = "Logout";
            this.barButtonItem8.Glyph = global::SMG.Desktop.Properties.Resources.icons8_logout_20;
            this.barButtonItem8.Id = 3;
            this.barButtonItem8.Name = "barButtonItem8";
            // 
            // bbtReport
            // 
            this.bbtReport.Caption = "Report";
            this.bbtReport.Glyph = ((System.Drawing.Image)(resources.GetObject("bbtReport.Glyph")));
            this.bbtReport.Id = 4;
            this.bbtReport.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("bbtReport.LargeGlyph")));
            this.bbtReport.Name = "bbtReport";
            this.bbtReport.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtReport_ItemClick);
            // 
            // imageCollectionMenuIcon
            // 
            this.imageCollectionMenuIcon.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageCollectionMenuIcon.ImageStream")));
            this.imageCollectionMenuIcon.Images.SetKeyName(0, "Papirus-Team-Papirus-Apps-Y-ppa-manager.512.png");
            this.imageCollectionMenuIcon.Images.SetKeyName(1, "images.jpg");
            this.imageCollectionMenuIcon.Images.SetKeyName(2, "swe.png");
            // 
            // Report
            // 
            this.Report.ItemLinks.Add(this.barButtonItem4, true);
            // 
            // ribbonPage
            // 
            this.ribbonPage.Name = "ribbonPage";
            reduceOperation1.Behavior = DevExpress.XtraBars.Ribbon.ReduceOperationBehavior.Single;
            reduceOperation1.Group = null;
            reduceOperation1.ItemLinkIndex = 0;
            reduceOperation1.ItemLinksCount = 0;
            reduceOperation1.Operation = DevExpress.XtraBars.Ribbon.ReduceOperationType.Gallery;
            reduceOperation2.Behavior = DevExpress.XtraBars.Ribbon.ReduceOperationBehavior.Single;
            reduceOperation2.Group = null;
            reduceOperation2.ItemLinkIndex = 0;
            reduceOperation2.ItemLinksCount = 0;
            reduceOperation2.Operation = DevExpress.XtraBars.Ribbon.ReduceOperationType.LargeButtons;
            this.ribbonPage.ReduceOperations.Add(reduceOperation1);
            this.ribbonPage.ReduceOperations.Add(reduceOperation2);
            this.ribbonPage.Text = "Danh mục chung";
            // 
            // ribbonStatusBar1
            // 
            this.ribbonStatusBar1.Location = new System.Drawing.Point(1, 374);
            this.ribbonStatusBar1.Name = "ribbonStatusBar1";
            this.ribbonStatusBar1.Ribbon = this.ribbonControl1;
            this.ribbonStatusBar1.Size = new System.Drawing.Size(1115, 27);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem3});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1125, 549);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.ribbonControl1;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(1125, 138);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.layoutControl2;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 138);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(1125, 411);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // ribbonPageGroupFile
            // 
            this.ribbonPageGroupFile.Name = "ribbonPageGroupFile";
            // 
            // ribbonPageGroupEdit
            // 
            this.ribbonPageGroupEdit.Name = "ribbonPageGroupEdit";
            // 
            // frmMain
            // 
            this.ClientSize = new System.Drawing.Size(1125, 549);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Main Menu";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            this.layoutControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl2)).EndInit();
            this.layoutControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupMenu1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.popupMenu2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollectionMenuIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroupEdit;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroupFile;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem2;
        private DevExpress.XtraBars.BarButtonGroup barButtonGroup1;
        private DevExpress.XtraBars.PopupMenu popupMenu1;
        private DevExpress.XtraBars.BarButtonItem barButtonItem3;
        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.Utils.ImageCollection imageCollectionMenuIcon;
        private DevExpress.XtraBars.BarButtonItem barButtonItem4;
        private DevExpress.XtraBars.Ribbon.RibbonMiniToolbar Report;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar1;
        private System.ComponentModel.IContainer components;
        private DevExpress.XtraLayout.LayoutControl layoutControl2;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraBars.BarButtonItem barButtonItem5;
        private DevExpress.XtraBars.PopupMenu popupMenu2;
        private DevExpress.XtraBars.BarButtonItem barButtonItem6;
        private DevExpress.XtraBars.BarButtonItem barButtonItem7;
        private DevExpress.XtraBars.BarButtonItem barButtonItem8;
        private DevExpress.XtraBars.BarButtonItem bbtReport;
    }
}

