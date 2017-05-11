namespace Bosel.Vsto
{
    partial class RibbonEdit : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public RibbonEdit()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.tab1 = this.Factory.CreateRibbonTab();
            this.group2 = this.Factory.CreateRibbonGroup();
            this.btnOpen = this.Factory.CreateRibbonButton();
            this.btnSave = this.Factory.CreateRibbonButton();
            this.group1 = this.Factory.CreateRibbonGroup();
            this.cbxCategories = this.Factory.CreateRibbonComboBox();
            this.cbxCategoriesCodes = this.Factory.CreateRibbonComboBox();
            this.btnAddCategory = this.Factory.CreateRibbonButton();
            this.btnAddCode = this.Factory.CreateRibbonButton();
            this.chxUpperTxt = this.Factory.CreateRibbonCheckBox();
            this.btnDelCategory = this.Factory.CreateRibbonButton();
            this.btnDelCode = this.Factory.CreateRibbonButton();
            this.openFileDlg = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDlg = new System.Windows.Forms.SaveFileDialog();
            this.group3 = this.Factory.CreateRibbonGroup();
            this.group4 = this.Factory.CreateRibbonGroup();
            this.tab1.SuspendLayout();
            this.group2.SuspendLayout();
            this.group1.SuspendLayout();
            this.group3.SuspendLayout();
            this.group4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.group2);
            this.tab1.Groups.Add(this.group1);
            this.tab1.Groups.Add(this.group3);
            this.tab1.Groups.Add(this.group4);
            this.tab1.Label = "TabAddIns";
            this.tab1.Name = "tab1";
            // 
            // group2
            // 
            this.group2.Items.Add(this.btnOpen);
            this.group2.Items.Add(this.btnSave);
            this.group2.Label = "Options";
            this.group2.Name = "group2";
            // 
            // btnOpen
            // 
            this.btnOpen.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnOpen.Label = "Open";
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.OfficeImageId = "FileOpen";
            this.btnOpen.ShowImage = true;
            // 
            // btnSave
            // 
            this.btnSave.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnSave.Label = "Save";
            this.btnSave.Name = "btnSave";
            this.btnSave.OfficeImageId = "FileSaveAs";
            this.btnSave.ShowImage = true;
            // 
            // group1
            // 
            this.group1.Items.Add(this.cbxCategories);
            this.group1.Items.Add(this.cbxCategoriesCodes);
            this.group1.Label = "Categories";
            this.group1.Name = "group1";
            // 
            // cbxCategories
            // 
            this.cbxCategories.Label = "Categories";
            this.cbxCategories.Name = "cbxCategories";
            this.cbxCategories.SizeString = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
            this.cbxCategories.Text = null;
            // 
            // cbxCategoriesCodes
            // 
            this.cbxCategoriesCodes.Label = "Codes";
            this.cbxCategoriesCodes.Name = "cbxCategoriesCodes";
            this.cbxCategoriesCodes.SizeString = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
            this.cbxCategoriesCodes.Text = null;
            // 
            // btnAddCategory
            // 
            this.btnAddCategory.Label = "Add";
            this.btnAddCategory.Name = "btnAddCategory";
            this.btnAddCategory.OfficeImageId = "AcceptInvitation";
            this.btnAddCategory.ScreenTip = "Add a category";
            this.btnAddCategory.ShowImage = true;
            // 
            // btnAddCode
            // 
            this.btnAddCode.Label = "Add";
            this.btnAddCode.Name = "btnAddCode";
            this.btnAddCode.OfficeImageId = "AcceptInvitation";
            this.btnAddCode.ScreenTip = "Add a category code";
            this.btnAddCode.ShowImage = true;
            // 
            // chxUpperTxt
            // 
            this.chxUpperTxt.Label = "Upper";
            this.chxUpperTxt.Name = "chxUpperTxt";
            this.chxUpperTxt.ScreenTip = "Auto upper text when selected";
            // 
            // btnDelCategory
            // 
            this.btnDelCategory.Label = "Delete";
            this.btnDelCategory.Name = "btnDelCategory";
            this.btnDelCategory.OfficeImageId = "Delete";
            this.btnDelCategory.ScreenTip = "Delete a category";
            this.btnDelCategory.ShowImage = true;
            // 
            // btnDelCode
            // 
            this.btnDelCode.Label = "Delete";
            this.btnDelCode.Name = "btnDelCode";
            this.btnDelCode.OfficeImageId = "Delete";
            this.btnDelCode.ScreenTip = "Delete a code";
            this.btnDelCode.ShowImage = true;
            // 
            // openFileDlg
            // 
            this.openFileDlg.FileName = "Categories";
            this.openFileDlg.Filter = "Txt|*.txt|Json|*.json";
            this.openFileDlg.RestoreDirectory = true;
            this.openFileDlg.Title = "select your source";
            // 
            // saveFileDlg
            // 
            this.saveFileDlg.DefaultExt = "txt";
            this.saveFileDlg.FileName = "Categories";
            this.saveFileDlg.Filter = "Txt|*.txt|Json|*.json";
            this.saveFileDlg.RestoreDirectory = true;
            this.saveFileDlg.Title = "Save the file";
            // 
            // group3
            // 
            this.group3.Items.Add(this.btnAddCategory);
            this.group3.Items.Add(this.btnAddCode);
            this.group3.Items.Add(this.chxUpperTxt);
            this.group3.Label = "Add";
            this.group3.Name = "group3";
            // 
            // group4
            // 
            this.group4.Items.Add(this.btnDelCategory);
            this.group4.Items.Add(this.btnDelCode);
            this.group4.Label = "Delete";
            this.group4.Name = "group4";
            // 
            // RibbonEdit
            // 
            this.Name = "RibbonEdit";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.RibbonEdit_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.group2.ResumeLayout(false);
            this.group2.PerformLayout();
            this.group1.ResumeLayout(false);
            this.group1.PerformLayout();
            this.group3.ResumeLayout(false);
            this.group3.PerformLayout();
            this.group4.ResumeLayout(false);
            this.group4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group1;
        internal Microsoft.Office.Tools.Ribbon.RibbonComboBox cbxCategories;
        internal Microsoft.Office.Tools.Ribbon.RibbonComboBox cbxCategoriesCodes;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnAddCategory;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group2;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnOpen;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnSave;
        private System.Windows.Forms.OpenFileDialog openFileDlg;
        private System.Windows.Forms.SaveFileDialog saveFileDlg;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnAddCode;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnDelCategory;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnDelCode;
        internal Microsoft.Office.Tools.Ribbon.RibbonCheckBox chxUpperTxt;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group3;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group4;
    }

    partial class ThisRibbonCollection
    {
        internal RibbonEdit RibbonEdit
        {
            get { return this.GetRibbon<RibbonEdit>(); }
        }
    }
}
