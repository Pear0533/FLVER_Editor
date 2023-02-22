namespace FLVER_Editor
{
    partial class MainWindow
    {
        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.ribbon = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.mergeToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.presetsFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.browseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.materialsToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.dummiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.materialsToolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.dummiesToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.mergeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.materialsToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.dummiesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.loadJSONToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bonesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.materialsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportJSONToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bonesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.materialsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleTextureRefreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleAutoSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoSaveIntervalSelector = new System.Windows.Forms.ToolStripTextBox();
            this.dummyThicknessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dummyThicknessSelector = new System.Windows.Forms.ToolStripComboBox();
            this.toggleDummyIDsVisibilityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.supportPearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.patreonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.payPalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multiSelectMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.multiSelectMenuTextbox = new System.Windows.Forms.ToolStripTextBox();
            this.meshTabPage = new System.Windows.Forms.TabPage();
            this.functionsGroupBox = new System.Windows.Forms.GroupBox();
            this.displayFemaleBodyButton = new System.Windows.Forms.Button();
            this.displayMaleBodyButton = new System.Windows.Forms.Button();
            this.setAllBBsMaxSizeButton = new System.Windows.Forms.Button();
            this.hideAllMeshesButton = new System.Windows.Forms.Button();
            this.selectAllMeshesButton = new System.Windows.Forms.Button();
            this.selectAllDummiesButton = new System.Windows.Forms.Button();
            this.solveAllBBsButton = new System.Windows.Forms.Button();
            this.meshPagePanelsContainer = new System.Windows.Forms.SplitContainer();
            this.meshPageTablesContainer = new System.Windows.Forms.SplitContainer();
            this.meshTable = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column21 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.label4 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.addDummyButton = new System.Windows.Forms.Button();
            this.addAllDummiesToPresetsButton = new System.Windows.Forms.Button();
            this.dummiesTable = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column17 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewCheckBoxColumn1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column18 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.dummiesTableOKButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.dummyPresetsSelector = new System.Windows.Forms.ComboBox();
            this.meshModifiersContainer = new System.Windows.Forms.GroupBox();
            this.zAxisLabel = new System.Windows.Forms.Label();
            this.yAxisLabel = new System.Windows.Forms.Label();
            this.xAxisLabel = new System.Windows.Forms.Label();
            this.mirrorZCheckbox = new System.Windows.Forms.CheckBox();
            this.mirrorYCheckbox = new System.Windows.Forms.CheckBox();
            this.mirrorXCheckbox = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.deleteFacesetsCheckbox = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.centerToWorldButton = new System.Windows.Forms.Button();
            this.uniformScaleCheckbox = new System.Windows.Forms.CheckBox();
            this.reverseFacesetsCheckbox = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.deleteSelectedButton = new System.Windows.Forms.Button();
            this.meshModifiersNumBoxesContainer = new System.Windows.Forms.TableLayoutPanel();
            this.rotZNumBox = new System.Windows.Forms.NumericUpDown();
            this.scaleZNumBox = new System.Windows.Forms.NumericUpDown();
            this.rotYNumBox = new System.Windows.Forms.NumericUpDown();
            this.transZNumBox = new System.Windows.Forms.NumericUpDown();
            this.rotXNumBox = new System.Windows.Forms.NumericUpDown();
            this.scaleYNumBox = new System.Windows.Forms.NumericUpDown();
            this.scaleXNumBox = new System.Windows.Forms.NumericUpDown();
            this.transXNumBox = new System.Windows.Forms.NumericUpDown();
            this.transYNumBox = new System.Windows.Forms.NumericUpDown();
            this.toggleBackfacesCheckbox = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.reverseNormalsCheckbox = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.materialsTabPage = new System.Windows.Forms.TabPage();
            this.deleteAllMaterialsButton = new System.Windows.Forms.Button();
            this.materialsTableOKButton = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.materialsPagePanelsContainer = new System.Windows.Forms.SplitContainer();
            this.label9 = new System.Windows.Forms.Label();
            this.materialsTable = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Column20 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Column16 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.applyPresetToAllMaterialsButton = new System.Windows.Forms.Button();
            this.materialPresetsSelector = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.applyMatBinTexturesButton = new System.Windows.Forms.Button();
            this.texturesTable = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewButtonColumn1 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.bonesTabPage = new System.Windows.Forms.TabPage();
            this.bonesTable = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabWindow = new System.Windows.Forms.TabControl();
            this.copyrightStr = new System.Windows.Forms.Label();
            this.versionStr = new System.Windows.Forms.Label();
            this.autoSaveTimer = new System.Windows.Forms.Timer(this.components);
            this.selectAllMeshButtonTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.selectAllDummiesButtonTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.solveAllBBsButtonTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.setAllBBsMaxSizeButtonTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.hideAllMeshesButtonTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.uniformScaleTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.searchBox = new System.Windows.Forms.TextBox();
            this.searchBoxStr = new System.Windows.Forms.Label();
            this.meshTabDataTableSelector = new System.Windows.Forms.ComboBox();
            this.presetsBoxTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.xLeftRightTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.xHorizontalTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.xSwivelTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.yUpDownTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.yVerticalTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.yRollTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.zInOutTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.zThicknessTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.zCartwheelTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.displayMaleBodyTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.displayFemaleBodyTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.ribbon.SuspendLayout();
            this.multiSelectMenu.SuspendLayout();
            this.meshTabPage.SuspendLayout();
            this.functionsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.meshPagePanelsContainer)).BeginInit();
            this.meshPagePanelsContainer.Panel1.SuspendLayout();
            this.meshPagePanelsContainer.Panel2.SuspendLayout();
            this.meshPagePanelsContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.meshPageTablesContainer)).BeginInit();
            this.meshPageTablesContainer.Panel1.SuspendLayout();
            this.meshPageTablesContainer.Panel2.SuspendLayout();
            this.meshPageTablesContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.meshTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dummiesTable)).BeginInit();
            this.meshModifiersContainer.SuspendLayout();
            this.meshModifiersNumBoxesContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rotZNumBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleZNumBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotYNumBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.transZNumBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotXNumBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleYNumBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleXNumBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.transXNumBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.transYNumBox)).BeginInit();
            this.materialsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.materialsPagePanelsContainer)).BeginInit();
            this.materialsPagePanelsContainer.Panel1.SuspendLayout();
            this.materialsPagePanelsContainer.Panel2.SuspendLayout();
            this.materialsPagePanelsContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.materialsTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.texturesTable)).BeginInit();
            this.bonesTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bonesTable)).BeginInit();
            this.tabWindow.SuspendLayout();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.ribbon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.preferencesToolStripMenuItem,
            this.supportPearToolStripMenuItem});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.Name = "ribbon";
            this.ribbon.Padding = new System.Windows.Forms.Padding(4, 1, 0, 1);
            this.ribbon.Size = new System.Drawing.Size(1034, 24);
            this.ribbon.TabIndex = 0;
            this.ribbon.Text = "menuStrip1";
            this.ribbon.Click += new System.EventHandler(this.DefocusSearchBox);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.modelToolStripMenuItem,
            this.presetsFileToolStripMenuItem,
            this.loadJSONToolStripMenuItem,
            this.exportJSONToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 22);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.openToolStripMenuItem.Text = "Open (Ctrl+O)";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenButtonClicked);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.saveToolStripMenuItem.Text = "Save (Ctrl+S)";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveButtonClicked);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Enabled = false;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.saveAsToolStripMenuItem.Text = "Save As (Ctrl+Shift+S)";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsButtonClicked);
            // 
            // modelToolStripMenuItem
            // 
            this.modelToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem1,
            this.exportToolStripMenuItem2,
            this.mergeToolStripMenuItem2});
            this.modelToolStripMenuItem.Name = "modelToolStripMenuItem";
            this.modelToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.modelToolStripMenuItem.Text = "Model";
            // 
            // importToolStripMenuItem1
            // 
            this.importToolStripMenuItem1.Name = "importToolStripMenuItem1";
            this.importToolStripMenuItem1.Size = new System.Drawing.Size(110, 22);
            this.importToolStripMenuItem1.Text = "Import";
            this.importToolStripMenuItem1.Click += new System.EventHandler(this.ImportToolStripMenuItemClicked);
            // 
            // exportToolStripMenuItem2
            // 
            this.exportToolStripMenuItem2.Name = "exportToolStripMenuItem2";
            this.exportToolStripMenuItem2.Size = new System.Drawing.Size(110, 22);
            this.exportToolStripMenuItem2.Text = "Export";
            this.exportToolStripMenuItem2.Click += new System.EventHandler(this.ExportToolStripMenuItemClicked);
            // 
            // mergeToolStripMenuItem2
            // 
            this.mergeToolStripMenuItem2.Name = "mergeToolStripMenuItem2";
            this.mergeToolStripMenuItem2.Size = new System.Drawing.Size(110, 22);
            this.mergeToolStripMenuItem2.Text = "Merge";
            this.mergeToolStripMenuItem2.Click += new System.EventHandler(this.MergeToolStripMenuItemClicked);
            // 
            // presetsFileToolStripMenuItem
            // 
            this.presetsFileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.browseToolStripMenuItem,
            this.exportToolStripMenuItem1,
            this.mergeToolStripMenuItem1});
            this.presetsFileToolStripMenuItem.Name = "presetsFileToolStripMenuItem";
            this.presetsFileToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.presetsFileToolStripMenuItem.Text = "Presets File";
            // 
            // browseToolStripMenuItem
            // 
            this.browseToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.materialsToolStripMenuItem2,
            this.dummiesToolStripMenuItem});
            this.browseToolStripMenuItem.Name = "browseToolStripMenuItem";
            this.browseToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.browseToolStripMenuItem.Text = "Import";
            // 
            // materialsToolStripMenuItem2
            // 
            this.materialsToolStripMenuItem2.Name = "materialsToolStripMenuItem2";
            this.materialsToolStripMenuItem2.Size = new System.Drawing.Size(125, 22);
            this.materialsToolStripMenuItem2.Text = "Materials";
            this.materialsToolStripMenuItem2.Click += new System.EventHandler(this.BrowseMaterialPresetsFileButtonClicked);
            // 
            // dummiesToolStripMenuItem
            // 
            this.dummiesToolStripMenuItem.Name = "dummiesToolStripMenuItem";
            this.dummiesToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.dummiesToolStripMenuItem.Text = "Dummies";
            this.dummiesToolStripMenuItem.Click += new System.EventHandler(this.BrowseDummyPresetsFileButtonClicked);
            // 
            // exportToolStripMenuItem1
            // 
            this.exportToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.materialsToolStripMenuItem4,
            this.dummiesToolStripMenuItem2});
            this.exportToolStripMenuItem1.Name = "exportToolStripMenuItem1";
            this.exportToolStripMenuItem1.Size = new System.Drawing.Size(110, 22);
            this.exportToolStripMenuItem1.Text = "Export";
            // 
            // materialsToolStripMenuItem4
            // 
            this.materialsToolStripMenuItem4.Name = "materialsToolStripMenuItem4";
            this.materialsToolStripMenuItem4.Size = new System.Drawing.Size(125, 22);
            this.materialsToolStripMenuItem4.Text = "Materials";
            this.materialsToolStripMenuItem4.Click += new System.EventHandler(this.ExportMaterialPresetsFileButtonClick);
            // 
            // dummiesToolStripMenuItem2
            // 
            this.dummiesToolStripMenuItem2.Name = "dummiesToolStripMenuItem2";
            this.dummiesToolStripMenuItem2.Size = new System.Drawing.Size(125, 22);
            this.dummiesToolStripMenuItem2.Text = "Dummies";
            this.dummiesToolStripMenuItem2.Click += new System.EventHandler(this.ExportDummiesPresetFileButtonClick);
            // 
            // mergeToolStripMenuItem1
            // 
            this.mergeToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.materialsToolStripMenuItem3,
            this.dummiesToolStripMenuItem1});
            this.mergeToolStripMenuItem1.Name = "mergeToolStripMenuItem1";
            this.mergeToolStripMenuItem1.Size = new System.Drawing.Size(110, 22);
            this.mergeToolStripMenuItem1.Text = "Merge";
            // 
            // materialsToolStripMenuItem3
            // 
            this.materialsToolStripMenuItem3.Name = "materialsToolStripMenuItem3";
            this.materialsToolStripMenuItem3.Size = new System.Drawing.Size(125, 22);
            this.materialsToolStripMenuItem3.Text = "Materials";
            this.materialsToolStripMenuItem3.Click += new System.EventHandler(this.MergeMaterialPresetsButtonClicked);
            // 
            // dummiesToolStripMenuItem1
            // 
            this.dummiesToolStripMenuItem1.Name = "dummiesToolStripMenuItem1";
            this.dummiesToolStripMenuItem1.Size = new System.Drawing.Size(125, 22);
            this.dummiesToolStripMenuItem1.Text = "Dummies";
            this.dummiesToolStripMenuItem1.Click += new System.EventHandler(this.MergeDummyPresetsButtonClicked);
            // 
            // loadJSONToolStripMenuItem
            // 
            this.loadJSONToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bonesToolStripMenuItem,
            this.materialsToolStripMenuItem});
            this.loadJSONToolStripMenuItem.Name = "loadJSONToolStripMenuItem";
            this.loadJSONToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.loadJSONToolStripMenuItem.Text = "Load JSON";
            // 
            // bonesToolStripMenuItem
            // 
            this.bonesToolStripMenuItem.Name = "bonesToolStripMenuItem";
            this.bonesToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.bonesToolStripMenuItem.Text = "Bones";
            this.bonesToolStripMenuItem.Click += new System.EventHandler(this.LoadBonesJSONButtonClicked);
            // 
            // materialsToolStripMenuItem
            // 
            this.materialsToolStripMenuItem.Name = "materialsToolStripMenuItem";
            this.materialsToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.materialsToolStripMenuItem.Text = "Materials";
            this.materialsToolStripMenuItem.Click += new System.EventHandler(this.LoadMaterialsJSONButtonClicked);
            // 
            // exportJSONToolStripMenuItem
            // 
            this.exportJSONToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bonesToolStripMenuItem1,
            this.materialsToolStripMenuItem1});
            this.exportJSONToolStripMenuItem.Name = "exportJSONToolStripMenuItem";
            this.exportJSONToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.exportJSONToolStripMenuItem.Text = "Export JSON";
            // 
            // bonesToolStripMenuItem1
            // 
            this.bonesToolStripMenuItem1.Name = "bonesToolStripMenuItem1";
            this.bonesToolStripMenuItem1.Size = new System.Drawing.Size(122, 22);
            this.bonesToolStripMenuItem1.Text = "Bones";
            this.bonesToolStripMenuItem1.Click += new System.EventHandler(this.ExportBonesJSONButtonClicked);
            // 
            // materialsToolStripMenuItem1
            // 
            this.materialsToolStripMenuItem1.Name = "materialsToolStripMenuItem1";
            this.materialsToolStripMenuItem1.Size = new System.Drawing.Size(122, 22);
            this.materialsToolStripMenuItem1.Text = "Materials";
            this.materialsToolStripMenuItem1.Click += new System.EventHandler(this.ExportMaterialsJSONButtonClicked);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toggleTextureRefreshToolStripMenuItem,
            this.toggleAutoSaveToolStripMenuItem,
            this.dummyThicknessToolStripMenuItem,
            this.toggleDummyIDsVisibilityToolStripMenuItem});
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(80, 22);
            this.preferencesToolStripMenuItem.Text = "Preferences";
            // 
            // toggleTextureRefreshToolStripMenuItem
            // 
            this.toggleTextureRefreshToolStripMenuItem.Name = "toggleTextureRefreshToolStripMenuItem";
            this.toggleTextureRefreshToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.toggleTextureRefreshToolStripMenuItem.Text = "Toggle Texture Refresh";
            this.toggleTextureRefreshToolStripMenuItem.Click += new System.EventHandler(this.ToggleTextureRefreshButtonClicked);
            // 
            // toggleAutoSaveToolStripMenuItem
            // 
            this.toggleAutoSaveToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoSaveIntervalSelector});
            this.toggleAutoSaveToolStripMenuItem.Name = "toggleAutoSaveToolStripMenuItem";
            this.toggleAutoSaveToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.toggleAutoSaveToolStripMenuItem.Text = "Toggle AutoSave (Minutes)";
            this.toggleAutoSaveToolStripMenuItem.Click += new System.EventHandler(this.ToggleAutoSaveToolStripMenuItemClick);
            // 
            // autoSaveIntervalSelector
            // 
            this.autoSaveIntervalSelector.Name = "autoSaveIntervalSelector";
            this.autoSaveIntervalSelector.Size = new System.Drawing.Size(100, 23);
            this.autoSaveIntervalSelector.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AutoSaveIntervalSelectorKeyDown);
            this.autoSaveIntervalSelector.TextChanged += new System.EventHandler(this.AutoSaveIntervalSelectorTextChanged);
            // 
            // dummyThicknessToolStripMenuItem
            // 
            this.dummyThicknessToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dummyThicknessSelector});
            this.dummyThicknessToolStripMenuItem.Name = "dummyThicknessToolStripMenuItem";
            this.dummyThicknessToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.dummyThicknessToolStripMenuItem.Text = "Dummy Thickness";
            // 
            // dummyThicknessSelector
            // 
            this.dummyThicknessSelector.DropDownHeight = 50;
            this.dummyThicknessSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dummyThicknessSelector.IntegralHeight = false;
            this.dummyThicknessSelector.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15"});
            this.dummyThicknessSelector.Name = "dummyThicknessSelector";
            this.dummyThicknessSelector.Size = new System.Drawing.Size(121, 23);
            this.dummyThicknessSelector.SelectedIndexChanged += new System.EventHandler(this.DummyThicknessSelectorSelectedIndexChanged);
            // 
            // toggleDummyIDsVisibilityToolStripMenuItem
            // 
            this.toggleDummyIDsVisibilityToolStripMenuItem.Name = "toggleDummyIDsVisibilityToolStripMenuItem";
            this.toggleDummyIDsVisibilityToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.toggleDummyIDsVisibilityToolStripMenuItem.Text = "Toggle Dummy IDs Visibility";
            this.toggleDummyIDsVisibilityToolStripMenuItem.Click += new System.EventHandler(this.ToggleDummyIDsVisibilityToolStripMenuItem_Click);
            // 
            // supportPearToolStripMenuItem
            // 
            this.supportPearToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.patreonToolStripMenuItem,
            this.payPalToolStripMenuItem});
            this.supportPearToolStripMenuItem.Name = "supportPearToolStripMenuItem";
            this.supportPearToolStripMenuItem.Size = new System.Drawing.Size(87, 22);
            this.supportPearToolStripMenuItem.Text = "Support Pear";
            // 
            // patreonToolStripMenuItem
            // 
            this.patreonToolStripMenuItem.Name = "patreonToolStripMenuItem";
            this.patreonToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.patreonToolStripMenuItem.Text = "Patreon";
            this.patreonToolStripMenuItem.Click += new System.EventHandler(this.PatreonToolStripMenuItem_Click);
            // 
            // payPalToolStripMenuItem
            // 
            this.payPalToolStripMenuItem.Name = "payPalToolStripMenuItem";
            this.payPalToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.payPalToolStripMenuItem.Text = "PayPal";
            this.payPalToolStripMenuItem.Click += new System.EventHandler(this.PayPalToolStripMenuItem_Click);
            // 
            // multiSelectMenu
            // 
            this.multiSelectMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.multiSelectMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.multiSelectMenuTextbox});
            this.multiSelectMenu.Name = "boneTableMenu";
            this.multiSelectMenu.Size = new System.Drawing.Size(161, 29);
            // 
            // multiSelectMenuTextbox
            // 
            this.multiSelectMenuTextbox.Name = "multiSelectMenuTextbox";
            this.multiSelectMenuTextbox.Size = new System.Drawing.Size(100, 23);
            // 
            // meshTabPage
            // 
            this.meshTabPage.Controls.Add(this.functionsGroupBox);
            this.meshTabPage.Controls.Add(this.meshPagePanelsContainer);
            this.meshTabPage.Location = new System.Drawing.Point(4, 22);
            this.meshTabPage.Name = "meshTabPage";
            this.meshTabPage.Size = new System.Drawing.Size(1027, 462);
            this.meshTabPage.TabIndex = 2;
            this.meshTabPage.Text = "Mesh";
            this.meshTabPage.UseVisualStyleBackColor = true;
            // 
            // functionsGroupBox
            // 
            this.functionsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.functionsGroupBox.Controls.Add(this.displayFemaleBodyButton);
            this.functionsGroupBox.Controls.Add(this.displayMaleBodyButton);
            this.functionsGroupBox.Controls.Add(this.setAllBBsMaxSizeButton);
            this.functionsGroupBox.Controls.Add(this.hideAllMeshesButton);
            this.functionsGroupBox.Controls.Add(this.selectAllMeshesButton);
            this.functionsGroupBox.Controls.Add(this.selectAllDummiesButton);
            this.functionsGroupBox.Controls.Add(this.solveAllBBsButton);
            this.functionsGroupBox.Location = new System.Drawing.Point(9, 5);
            this.functionsGroupBox.Name = "functionsGroupBox";
            this.functionsGroupBox.Size = new System.Drawing.Size(1008, 63);
            this.functionsGroupBox.TabIndex = 17;
            this.functionsGroupBox.TabStop = false;
            this.functionsGroupBox.Text = "Functions";
            // 
            // displayFemaleBodyButton
            // 
            this.displayFemaleBodyButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("displayFemaleBodyButton.BackgroundImage")));
            this.displayFemaleBodyButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.displayFemaleBodyButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.displayFemaleBodyButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.displayFemaleBodyButton.Location = new System.Drawing.Point(267, 16);
            this.displayFemaleBodyButton.Name = "displayFemaleBodyButton";
            this.displayFemaleBodyButton.Size = new System.Drawing.Size(40, 40);
            this.displayFemaleBodyButton.TabIndex = 18;
            this.displayFemaleBodyTooltip.SetToolTip(this.displayFemaleBodyButton, "Display Female Body");
            this.displayFemaleBodyButton.UseVisualStyleBackColor = true;
            this.displayFemaleBodyButton.Click += new System.EventHandler(this.DisplayFemaleBodyButton_Click);
            // 
            // displayMaleBodyButton
            // 
            this.displayMaleBodyButton.BackgroundImage = global::FLVER_Editor.Properties.Resources.display_male_body_icon;
            this.displayMaleBodyButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.displayMaleBodyButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.displayMaleBodyButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.displayMaleBodyButton.Location = new System.Drawing.Point(223, 16);
            this.displayMaleBodyButton.Name = "displayMaleBodyButton";
            this.displayMaleBodyButton.Size = new System.Drawing.Size(40, 40);
            this.displayMaleBodyButton.TabIndex = 17;
            this.displayMaleBodyTooltip.SetToolTip(this.displayMaleBodyButton, "Display Male Body");
            this.displayMaleBodyButton.UseVisualStyleBackColor = true;
            this.displayMaleBodyButton.Click += new System.EventHandler(this.DisplayMaleBodyButton_Click);
            // 
            // setAllBBsMaxSizeButton
            // 
            this.setAllBBsMaxSizeButton.BackgroundImage = global::FLVER_Editor.Properties.Resources.solve_all_bbs_max_size_icon;
            this.setAllBBsMaxSizeButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.setAllBBsMaxSizeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.setAllBBsMaxSizeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.setAllBBsMaxSizeButton.Location = new System.Drawing.Point(179, 16);
            this.setAllBBsMaxSizeButton.Name = "setAllBBsMaxSizeButton";
            this.setAllBBsMaxSizeButton.Size = new System.Drawing.Size(40, 40);
            this.setAllBBsMaxSizeButton.TabIndex = 15;
            this.setAllBBsMaxSizeButtonTooltip.SetToolTip(this.setAllBBsMaxSizeButton, "Set All Bounding Boxes Max Size");
            this.setAllBBsMaxSizeButton.UseVisualStyleBackColor = true;
            this.setAllBBsMaxSizeButton.Click += new System.EventHandler(this.SetAllBBsMaxSizeButtonClicked);
            // 
            // hideAllMeshesButton
            // 
            this.hideAllMeshesButton.BackgroundImage = global::FLVER_Editor.Properties.Resources.hide_all_mesh_icon;
            this.hideAllMeshesButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.hideAllMeshesButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.hideAllMeshesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.hideAllMeshesButton.Location = new System.Drawing.Point(50, 16);
            this.hideAllMeshesButton.Name = "hideAllMeshesButton";
            this.hideAllMeshesButton.Size = new System.Drawing.Size(40, 40);
            this.hideAllMeshesButton.TabIndex = 16;
            this.hideAllMeshesButtonTooltip.SetToolTip(this.hideAllMeshesButton, "Hide All Mesh");
            this.hideAllMeshesButton.UseVisualStyleBackColor = true;
            this.hideAllMeshesButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.HideAllMeshesButton_MouseClick);
            // 
            // selectAllMeshesButton
            // 
            this.selectAllMeshesButton.BackgroundImage = global::FLVER_Editor.Properties.Resources.select_all_mesh_icon;
            this.selectAllMeshesButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.selectAllMeshesButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.selectAllMeshesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.selectAllMeshesButton.Location = new System.Drawing.Point(7, 16);
            this.selectAllMeshesButton.Name = "selectAllMeshesButton";
            this.selectAllMeshesButton.Size = new System.Drawing.Size(40, 40);
            this.selectAllMeshesButton.TabIndex = 9;
            this.selectAllMeshButtonTooltip.SetToolTip(this.selectAllMeshesButton, "Select All Mesh");
            this.selectAllMeshesButton.UseVisualStyleBackColor = true;
            this.selectAllMeshesButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SelectAllMeshesButtonClicked);
            // 
            // selectAllDummiesButton
            // 
            this.selectAllDummiesButton.BackgroundImage = global::FLVER_Editor.Properties.Resources.select_all_dummies_icon;
            this.selectAllDummiesButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.selectAllDummiesButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.selectAllDummiesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.selectAllDummiesButton.Location = new System.Drawing.Point(93, 16);
            this.selectAllDummiesButton.Name = "selectAllDummiesButton";
            this.selectAllDummiesButton.Size = new System.Drawing.Size(40, 40);
            this.selectAllDummiesButton.TabIndex = 13;
            this.selectAllDummiesButtonTooltip.SetToolTip(this.selectAllDummiesButton, "Select All Dummies");
            this.selectAllDummiesButton.UseVisualStyleBackColor = true;
            this.selectAllDummiesButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SelectAllDummiesButtonClicked);
            // 
            // solveAllBBsButton
            // 
            this.solveAllBBsButton.BackgroundImage = global::FLVER_Editor.Properties.Resources.solve_all_bbs_icon;
            this.solveAllBBsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.solveAllBBsButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.solveAllBBsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.solveAllBBsButton.Location = new System.Drawing.Point(136, 16);
            this.solveAllBBsButton.Name = "solveAllBBsButton";
            this.solveAllBBsButton.Size = new System.Drawing.Size(40, 40);
            this.solveAllBBsButton.TabIndex = 14;
            this.solveAllBBsButtonTooltip.SetToolTip(this.solveAllBBsButton, "Solve All Bounding Boxes");
            this.solveAllBBsButton.UseVisualStyleBackColor = true;
            this.solveAllBBsButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SolveAllBBsButtonClicked);
            // 
            // meshPagePanelsContainer
            // 
            this.meshPagePanelsContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.meshPagePanelsContainer.Location = new System.Drawing.Point(3, 70);
            this.meshPagePanelsContainer.Name = "meshPagePanelsContainer";
            // 
            // meshPagePanelsContainer.Panel1
            // 
            this.meshPagePanelsContainer.Panel1.Controls.Add(this.meshPageTablesContainer);
            // 
            // meshPagePanelsContainer.Panel2
            // 
            this.meshPagePanelsContainer.Panel2.Controls.Add(this.meshModifiersContainer);
            this.meshPagePanelsContainer.Size = new System.Drawing.Size(1014, 389);
            this.meshPagePanelsContainer.SplitterDistance = 504;
            this.meshPagePanelsContainer.TabIndex = 0;
            // 
            // meshPageTablesContainer
            // 
            this.meshPageTablesContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.meshPageTablesContainer.Location = new System.Drawing.Point(3, 3);
            this.meshPageTablesContainer.Name = "meshPageTablesContainer";
            this.meshPageTablesContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // meshPageTablesContainer.Panel1
            // 
            this.meshPageTablesContainer.Panel1.Controls.Add(this.meshTable);
            this.meshPageTablesContainer.Panel1.Controls.Add(this.label4);
            // 
            // meshPageTablesContainer.Panel2
            // 
            this.meshPageTablesContainer.Panel2.Controls.Add(this.label13);
            this.meshPageTablesContainer.Panel2.Controls.Add(this.addDummyButton);
            this.meshPageTablesContainer.Panel2.Controls.Add(this.addAllDummiesToPresetsButton);
            this.meshPageTablesContainer.Panel2.Controls.Add(this.dummiesTable);
            this.meshPageTablesContainer.Panel2.Controls.Add(this.dummiesTableOKButton);
            this.meshPageTablesContainer.Panel2.Controls.Add(this.label5);
            this.meshPageTablesContainer.Panel2.Controls.Add(this.dummyPresetsSelector);
            this.meshPageTablesContainer.Size = new System.Drawing.Size(498, 383);
            this.meshPageTablesContainer.SplitterDistance = 184;
            this.meshPageTablesContainer.TabIndex = 12;
            // 
            // meshTable
            // 
            this.meshTable.AllowUserToAddRows = false;
            this.meshTable.AllowUserToResizeColumns = false;
            this.meshTable.AllowUserToResizeRows = false;
            this.meshTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.meshTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.meshTable.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.meshTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.meshTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column21});
            this.meshTable.Location = new System.Drawing.Point(3, 18);
            this.meshTable.Name = "meshTable";
            this.meshTable.RowHeadersVisible = false;
            this.meshTable.RowHeadersWidth = 62;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.meshTable.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.meshTable.Size = new System.Drawing.Size(492, 163);
            this.meshTable.TabIndex = 11;
            this.meshTable.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.MeshTableSelectCheckboxClicked);
            this.meshTable.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.MeshTableCellValueChanged);
            // 
            // Column1
            // 
            this.Column1.Frozen = true;
            this.Column1.HeaderText = "Index";
            this.Column1.MinimumWidth = 8;
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 58;
            // 
            // Column2
            // 
            this.Column2.Frozen = true;
            this.Column2.HeaderText = "Mesh Name";
            this.Column2.MinimumWidth = 8;
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 89;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Simple Skin";
            this.Column3.MinimumWidth = 8;
            this.Column3.Name = "Column3";
            this.Column3.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Column3.Width = 87;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Select";
            this.Column4.MinimumWidth = 8;
            this.Column4.Name = "Column4";
            this.Column4.Width = 43;
            // 
            // Column21
            // 
            this.Column21.HeaderText = "Hide";
            this.Column21.Name = "Column21";
            this.Column21.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column21.Width = 35;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 1);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Mesh:";
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(161, 5);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(45, 13);
            this.label13.TabIndex = 19;
            this.label13.Text = "Presets:";
            // 
            // addDummyButton
            // 
            this.addDummyButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.addDummyButton.Location = new System.Drawing.Point(64, 0);
            this.addDummyButton.MaximumSize = new System.Drawing.Size(95, 21);
            this.addDummyButton.MinimumSize = new System.Drawing.Size(95, 21);
            this.addDummyButton.Name = "addDummyButton";
            this.addDummyButton.Size = new System.Drawing.Size(95, 21);
            this.addDummyButton.TabIndex = 18;
            this.addDummyButton.Text = "New Dummy";
            this.addDummyButton.UseVisualStyleBackColor = true;
            this.addDummyButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.AddDummyButtonClicked);
            // 
            // addAllDummiesToPresetsButton
            // 
            this.addAllDummiesToPresetsButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.addAllDummiesToPresetsButton.Location = new System.Drawing.Point(385, 0);
            this.addAllDummiesToPresetsButton.MaximumSize = new System.Drawing.Size(110, 21);
            this.addAllDummiesToPresetsButton.MinimumSize = new System.Drawing.Size(110, 21);
            this.addAllDummiesToPresetsButton.Name = "addAllDummiesToPresetsButton";
            this.addAllDummiesToPresetsButton.Size = new System.Drawing.Size(110, 21);
            this.addAllDummiesToPresetsButton.TabIndex = 17;
            this.addAllDummiesToPresetsButton.Text = "Add All As Preset";
            this.addAllDummiesToPresetsButton.UseVisualStyleBackColor = true;
            this.addAllDummiesToPresetsButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.AddAllDummiesToPresetsButtonClicked);
            // 
            // dummiesTable
            // 
            this.dummiesTable.AllowUserToAddRows = false;
            this.dummiesTable.AllowUserToResizeColumns = false;
            this.dummiesTable.AllowUserToResizeRows = false;
            this.dummiesTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dummiesTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dummiesTable.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dummiesTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dummiesTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn11,
            this.Column15,
            this.Column17,
            this.dataGridViewCheckBoxColumn1,
            this.Column18});
            this.dummiesTable.Location = new System.Drawing.Point(3, 21);
            this.dummiesTable.Name = "dummiesTable";
            this.dummiesTable.RowHeadersVisible = false;
            this.dummiesTable.RowHeadersWidth = 62;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dummiesTable.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dummiesTable.Size = new System.Drawing.Size(492, 171);
            this.dummiesTable.TabIndex = 12;
            this.dummiesTable.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DummiesTableSelectCheckboxClicked);
            this.dummiesTable.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DummiesTableCellValueChanged);
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.Frozen = true;
            this.dataGridViewTextBoxColumn3.HeaderText = "Index";
            this.dataGridViewTextBoxColumn3.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 58;
            // 
            // dataGridViewTextBoxColumn11
            // 
            this.dataGridViewTextBoxColumn11.Frozen = true;
            this.dataGridViewTextBoxColumn11.HeaderText = "Ref. ID";
            this.dataGridViewTextBoxColumn11.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn11.Name = "dataGridViewTextBoxColumn11";
            this.dataGridViewTextBoxColumn11.Width = 52;
            // 
            // Column15
            // 
            this.Column15.HeaderText = "Attach Bone Index";
            this.Column15.MinimumWidth = 8;
            this.Column15.Name = "Column15";
            this.Column15.Width = 87;
            // 
            // Column17
            // 
            this.Column17.HeaderText = "Parent Bone Index";
            this.Column17.MinimumWidth = 8;
            this.Column17.Name = "Column17";
            this.Column17.Width = 87;
            // 
            // dataGridViewCheckBoxColumn1
            // 
            this.dataGridViewCheckBoxColumn1.HeaderText = "Select";
            this.dataGridViewCheckBoxColumn1.MinimumWidth = 8;
            this.dataGridViewCheckBoxColumn1.Name = "dataGridViewCheckBoxColumn1";
            this.dataGridViewCheckBoxColumn1.Width = 43;
            // 
            // Column18
            // 
            this.Column18.HeaderText = "Duplicate";
            this.Column18.Name = "Column18";
            this.Column18.Width = 58;
            // 
            // dummiesTableOKButton
            // 
            this.dummiesTableOKButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dummiesTableOKButton.Location = new System.Drawing.Point(335, 0);
            this.dummiesTableOKButton.MaximumSize = new System.Drawing.Size(45, 21);
            this.dummiesTableOKButton.MinimumSize = new System.Drawing.Size(45, 21);
            this.dummiesTableOKButton.Name = "dummiesTableOKButton";
            this.dummiesTableOKButton.Size = new System.Drawing.Size(45, 21);
            this.dummiesTableOKButton.TabIndex = 16;
            this.dummiesTableOKButton.Text = "OK";
            this.dummiesTableOKButton.UseVisualStyleBackColor = true;
            this.dummiesTableOKButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.DummiesTableOKButtonClicked);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1, 4);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Dummies:";
            // 
            // dummyPresetsSelector
            // 
            this.dummyPresetsSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dummyPresetsSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dummyPresetsSelector.FormattingEnabled = true;
            this.dummyPresetsSelector.Location = new System.Drawing.Point(209, 0);
            this.dummyPresetsSelector.Name = "dummyPresetsSelector";
            this.dummyPresetsSelector.Size = new System.Drawing.Size(121, 21);
            this.dummyPresetsSelector.TabIndex = 15;
            this.presetsBoxTooltip.SetToolTip(this.dummyPresetsSelector, "Right-click the selector with a preset loaded to delete it");
            this.dummyPresetsSelector.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DummyPresetsSelector_MouseDown);
            // 
            // meshModifiersContainer
            // 
            this.meshModifiersContainer.Controls.Add(this.zAxisLabel);
            this.meshModifiersContainer.Controls.Add(this.yAxisLabel);
            this.meshModifiersContainer.Controls.Add(this.xAxisLabel);
            this.meshModifiersContainer.Controls.Add(this.mirrorZCheckbox);
            this.meshModifiersContainer.Controls.Add(this.mirrorYCheckbox);
            this.meshModifiersContainer.Controls.Add(this.mirrorXCheckbox);
            this.meshModifiersContainer.Controls.Add(this.label7);
            this.meshModifiersContainer.Controls.Add(this.deleteFacesetsCheckbox);
            this.meshModifiersContainer.Controls.Add(this.label6);
            this.meshModifiersContainer.Controls.Add(this.centerToWorldButton);
            this.meshModifiersContainer.Controls.Add(this.uniformScaleCheckbox);
            this.meshModifiersContainer.Controls.Add(this.reverseFacesetsCheckbox);
            this.meshModifiersContainer.Controls.Add(this.label8);
            this.meshModifiersContainer.Controls.Add(this.deleteSelectedButton);
            this.meshModifiersContainer.Controls.Add(this.meshModifiersNumBoxesContainer);
            this.meshModifiersContainer.Controls.Add(this.toggleBackfacesCheckbox);
            this.meshModifiersContainer.Controls.Add(this.label11);
            this.meshModifiersContainer.Controls.Add(this.reverseNormalsCheckbox);
            this.meshModifiersContainer.Controls.Add(this.label12);
            this.meshModifiersContainer.Controls.Add(this.label3);
            this.meshModifiersContainer.Controls.Add(this.label1);
            this.meshModifiersContainer.Controls.Add(this.label2);
            this.meshModifiersContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.meshModifiersContainer.Location = new System.Drawing.Point(0, 0);
            this.meshModifiersContainer.Name = "meshModifiersContainer";
            this.meshModifiersContainer.Size = new System.Drawing.Size(506, 389);
            this.meshModifiersContainer.TabIndex = 12;
            this.meshModifiersContainer.TabStop = false;
            this.meshModifiersContainer.Text = "Modifiers";
            // 
            // zAxisLabel
            // 
            this.zAxisLabel.AutoSize = true;
            this.zAxisLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.zAxisLabel.Location = new System.Drawing.Point(258, 17);
            this.zAxisLabel.Name = "zAxisLabel";
            this.zAxisLabel.Size = new System.Drawing.Size(14, 13);
            this.zAxisLabel.TabIndex = 63;
            this.zAxisLabel.Text = "Z";
            // 
            // yAxisLabel
            // 
            this.yAxisLabel.AutoSize = true;
            this.yAxisLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.yAxisLabel.Location = new System.Drawing.Point(178, 17);
            this.yAxisLabel.Name = "yAxisLabel";
            this.yAxisLabel.Size = new System.Drawing.Size(14, 13);
            this.yAxisLabel.TabIndex = 62;
            this.yAxisLabel.Text = "Y";
            // 
            // xAxisLabel
            // 
            this.xAxisLabel.AutoSize = true;
            this.xAxisLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.xAxisLabel.Location = new System.Drawing.Point(98, 17);
            this.xAxisLabel.Name = "xAxisLabel";
            this.xAxisLabel.Size = new System.Drawing.Size(14, 13);
            this.xAxisLabel.TabIndex = 61;
            this.xAxisLabel.Text = "X";
            // 
            // mirrorZCheckbox
            // 
            this.mirrorZCheckbox.AutoSize = true;
            this.mirrorZCheckbox.Location = new System.Drawing.Point(116, 126);
            this.mirrorZCheckbox.Name = "mirrorZCheckbox";
            this.mirrorZCheckbox.Size = new System.Drawing.Size(33, 17);
            this.mirrorZCheckbox.TabIndex = 60;
            this.mirrorZCheckbox.Text = "Z";
            this.mirrorZCheckbox.UseVisualStyleBackColor = true;
            this.mirrorZCheckbox.CheckedChanged += new System.EventHandler(this.MirrorZCheckboxCheckedChanged);
            // 
            // mirrorYCheckbox
            // 
            this.mirrorYCheckbox.AutoSize = true;
            this.mirrorYCheckbox.Location = new System.Drawing.Point(83, 126);
            this.mirrorYCheckbox.Name = "mirrorYCheckbox";
            this.mirrorYCheckbox.Size = new System.Drawing.Size(33, 17);
            this.mirrorYCheckbox.TabIndex = 59;
            this.mirrorYCheckbox.Text = "Y";
            this.mirrorYCheckbox.UseVisualStyleBackColor = true;
            this.mirrorYCheckbox.CheckedChanged += new System.EventHandler(this.MirrorYCheckboxCheckedChanged);
            // 
            // mirrorXCheckbox
            // 
            this.mirrorXCheckbox.AutoSize = true;
            this.mirrorXCheckbox.Location = new System.Drawing.Point(50, 126);
            this.mirrorXCheckbox.Name = "mirrorXCheckbox";
            this.mirrorXCheckbox.Size = new System.Drawing.Size(33, 17);
            this.mirrorXCheckbox.TabIndex = 58;
            this.mirrorXCheckbox.Text = "X";
            this.mirrorXCheckbox.UseVisualStyleBackColor = true;
            this.mirrorXCheckbox.CheckedChanged += new System.EventHandler(this.MirrorXCheckboxCheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 126);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(36, 13);
            this.label7.TabIndex = 57;
            this.label7.Text = "Mirror:";
            // 
            // deleteFacesetsCheckbox
            // 
            this.deleteFacesetsCheckbox.AutoSize = true;
            this.deleteFacesetsCheckbox.Location = new System.Drawing.Point(9, 226);
            this.deleteFacesetsCheckbox.Name = "deleteFacesetsCheckbox";
            this.deleteFacesetsCheckbox.Size = new System.Drawing.Size(15, 14);
            this.deleteFacesetsCheckbox.TabIndex = 45;
            this.deleteFacesetsCheckbox.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(25, 226);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(108, 13);
            this.label6.TabIndex = 44;
            this.label6.Text = "Only Delete Facesets";
            // 
            // centerToWorldButton
            // 
            this.centerToWorldButton.Location = new System.Drawing.Point(8, 246);
            this.centerToWorldButton.Name = "centerToWorldButton";
            this.centerToWorldButton.Size = new System.Drawing.Size(104, 22);
            this.centerToWorldButton.TabIndex = 43;
            this.centerToWorldButton.Text = "Center To World";
            this.centerToWorldButton.UseVisualStyleBackColor = true;
            this.centerToWorldButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CenterToWorldButtonClicked);
            // 
            // uniformScaleCheckbox
            // 
            this.uniformScaleCheckbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.uniformScaleCheckbox.Location = new System.Drawing.Point(49, 64);
            this.uniformScaleCheckbox.Name = "uniformScaleCheckbox";
            this.uniformScaleCheckbox.Size = new System.Drawing.Size(19, 24);
            this.uniformScaleCheckbox.TabIndex = 42;
            this.uniformScaleCheckbox.Text = "U";
            this.uniformScaleCheckbox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.uniformScaleTooltip.SetToolTip(this.uniformScaleCheckbox, "Uniform Scale");
            this.uniformScaleCheckbox.UseVisualStyleBackColor = true;
            // 
            // reverseFacesetsCheckbox
            // 
            this.reverseFacesetsCheckbox.AutoSize = true;
            this.reverseFacesetsCheckbox.Location = new System.Drawing.Point(9, 151);
            this.reverseFacesetsCheckbox.Name = "reverseFacesetsCheckbox";
            this.reverseFacesetsCheckbox.Size = new System.Drawing.Size(15, 14);
            this.reverseFacesetsCheckbox.TabIndex = 39;
            this.reverseFacesetsCheckbox.UseVisualStyleBackColor = true;
            this.reverseFacesetsCheckbox.CheckedChanged += new System.EventHandler(this.ReverseFaceSetsCheckboxChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(25, 151);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(93, 13);
            this.label8.TabIndex = 38;
            this.label8.Text = "Reverse Facesets";
            // 
            // deleteSelectedButton
            // 
            this.deleteSelectedButton.Location = new System.Drawing.Point(115, 246);
            this.deleteSelectedButton.Name = "deleteSelectedButton";
            this.deleteSelectedButton.Size = new System.Drawing.Size(104, 22);
            this.deleteSelectedButton.TabIndex = 10;
            this.deleteSelectedButton.Text = "Delete Selected";
            this.deleteSelectedButton.UseVisualStyleBackColor = true;
            this.deleteSelectedButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.DeleteSelectedButtonClicked);
            // 
            // meshModifiersNumBoxesContainer
            // 
            this.meshModifiersNumBoxesContainer.AutoSize = true;
            this.meshModifiersNumBoxesContainer.ColumnCount = 3;
            this.meshModifiersNumBoxesContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.meshModifiersNumBoxesContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.meshModifiersNumBoxesContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.meshModifiersNumBoxesContainer.Controls.Add(this.rotZNumBox, 2, 2);
            this.meshModifiersNumBoxesContainer.Controls.Add(this.scaleZNumBox, 2, 1);
            this.meshModifiersNumBoxesContainer.Controls.Add(this.rotYNumBox, 1, 2);
            this.meshModifiersNumBoxesContainer.Controls.Add(this.transZNumBox, 2, 0);
            this.meshModifiersNumBoxesContainer.Controls.Add(this.rotXNumBox, 0, 2);
            this.meshModifiersNumBoxesContainer.Controls.Add(this.scaleYNumBox, 1, 1);
            this.meshModifiersNumBoxesContainer.Controls.Add(this.scaleXNumBox, 0, 1);
            this.meshModifiersNumBoxesContainer.Controls.Add(this.transXNumBox, 0, 0);
            this.meshModifiersNumBoxesContainer.Controls.Add(this.transYNumBox, 1, 0);
            this.meshModifiersNumBoxesContainer.Location = new System.Drawing.Point(70, 31);
            this.meshModifiersNumBoxesContainer.Name = "meshModifiersNumBoxesContainer";
            this.meshModifiersNumBoxesContainer.RowCount = 3;
            this.meshModifiersNumBoxesContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.meshModifiersNumBoxesContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.meshModifiersNumBoxesContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.meshModifiersNumBoxesContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.meshModifiersNumBoxesContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.meshModifiersNumBoxesContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.meshModifiersNumBoxesContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.meshModifiersNumBoxesContainer.Size = new System.Drawing.Size(237, 90);
            this.meshModifiersNumBoxesContainer.TabIndex = 36;
            // 
            // rotZNumBox
            // 
            this.rotZNumBox.DecimalPlaces = 3;
            this.rotZNumBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rotZNumBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.rotZNumBox.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.rotZNumBox.Location = new System.Drawing.Point(161, 63);
            this.rotZNumBox.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.rotZNumBox.Minimum = new decimal(new int[] {
            -1,
            -1,
            -1,
            -2147483648});
            this.rotZNumBox.Name = "rotZNumBox";
            this.rotZNumBox.Size = new System.Drawing.Size(73, 24);
            this.rotZNumBox.TabIndex = 13;
            this.zCartwheelTooltip.SetToolTip(this.rotZNumBox, "Z (Cartwheel)");
            this.rotZNumBox.ValueChanged += new System.EventHandler(this.ModifierNumBoxValueChanged);
            this.rotZNumBox.Enter += new System.EventHandler(this.ModifierNumBoxFocused);
            this.rotZNumBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ModifierNumBoxEnterPressed);
            // 
            // scaleZNumBox
            // 
            this.scaleZNumBox.DecimalPlaces = 3;
            this.scaleZNumBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scaleZNumBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.scaleZNumBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.scaleZNumBox.Location = new System.Drawing.Point(161, 33);
            this.scaleZNumBox.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.scaleZNumBox.Minimum = new decimal(new int[] {
            -1,
            -1,
            -1,
            -2147483648});
            this.scaleZNumBox.Name = "scaleZNumBox";
            this.scaleZNumBox.Size = new System.Drawing.Size(73, 24);
            this.scaleZNumBox.TabIndex = 13;
            this.zThicknessTooltip.SetToolTip(this.scaleZNumBox, "Z (Thickness)");
            this.scaleZNumBox.ValueChanged += new System.EventHandler(this.ModifierNumBoxValueChanged);
            this.scaleZNumBox.Enter += new System.EventHandler(this.ModifierNumBoxFocused);
            this.scaleZNumBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ModifierNumBoxEnterPressed);
            // 
            // rotYNumBox
            // 
            this.rotYNumBox.DecimalPlaces = 3;
            this.rotYNumBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rotYNumBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.rotYNumBox.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.rotYNumBox.Location = new System.Drawing.Point(82, 63);
            this.rotYNumBox.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.rotYNumBox.Minimum = new decimal(new int[] {
            -1,
            -1,
            -1,
            -2147483648});
            this.rotYNumBox.Name = "rotYNumBox";
            this.rotYNumBox.Size = new System.Drawing.Size(73, 24);
            this.rotYNumBox.TabIndex = 12;
            this.yRollTooltip.SetToolTip(this.rotYNumBox, "Y (Roll)");
            this.rotYNumBox.ValueChanged += new System.EventHandler(this.ModifierNumBoxValueChanged);
            this.rotYNumBox.Enter += new System.EventHandler(this.ModifierNumBoxFocused);
            this.rotYNumBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ModifierNumBoxEnterPressed);
            // 
            // transZNumBox
            // 
            this.transZNumBox.DecimalPlaces = 3;
            this.transZNumBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.transZNumBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.transZNumBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.transZNumBox.Location = new System.Drawing.Point(161, 3);
            this.transZNumBox.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.transZNumBox.Minimum = new decimal(new int[] {
            -1,
            -1,
            -1,
            -2147483648});
            this.transZNumBox.Name = "transZNumBox";
            this.transZNumBox.Size = new System.Drawing.Size(73, 24);
            this.transZNumBox.TabIndex = 13;
            this.zInOutTooltip.SetToolTip(this.transZNumBox, "Z (In/Out)");
            this.transZNumBox.ValueChanged += new System.EventHandler(this.ModifierNumBoxValueChanged);
            this.transZNumBox.Enter += new System.EventHandler(this.ModifierNumBoxFocused);
            this.transZNumBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ModifierNumBoxEnterPressed);
            // 
            // rotXNumBox
            // 
            this.rotXNumBox.DecimalPlaces = 3;
            this.rotXNumBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rotXNumBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.rotXNumBox.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.rotXNumBox.Location = new System.Drawing.Point(3, 63);
            this.rotXNumBox.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.rotXNumBox.Minimum = new decimal(new int[] {
            -1,
            -1,
            -1,
            -2147483648});
            this.rotXNumBox.Name = "rotXNumBox";
            this.rotXNumBox.Size = new System.Drawing.Size(73, 24);
            this.rotXNumBox.TabIndex = 11;
            this.xSwivelTooltip.SetToolTip(this.rotXNumBox, "X (Swivel)");
            this.rotXNumBox.ValueChanged += new System.EventHandler(this.ModifierNumBoxValueChanged);
            this.rotXNumBox.Enter += new System.EventHandler(this.ModifierNumBoxFocused);
            this.rotXNumBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ModifierNumBoxEnterPressed);
            // 
            // scaleYNumBox
            // 
            this.scaleYNumBox.DecimalPlaces = 3;
            this.scaleYNumBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scaleYNumBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.scaleYNumBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.scaleYNumBox.Location = new System.Drawing.Point(82, 33);
            this.scaleYNumBox.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.scaleYNumBox.Minimum = new decimal(new int[] {
            -1,
            -1,
            -1,
            -2147483648});
            this.scaleYNumBox.Name = "scaleYNumBox";
            this.scaleYNumBox.Size = new System.Drawing.Size(73, 24);
            this.scaleYNumBox.TabIndex = 12;
            this.yVerticalTooltip.SetToolTip(this.scaleYNumBox, "Y (Vertical)");
            this.scaleYNumBox.ValueChanged += new System.EventHandler(this.ModifierNumBoxValueChanged);
            this.scaleYNumBox.Enter += new System.EventHandler(this.ModifierNumBoxFocused);
            this.scaleYNumBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ModifierNumBoxEnterPressed);
            // 
            // scaleXNumBox
            // 
            this.scaleXNumBox.DecimalPlaces = 3;
            this.scaleXNumBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scaleXNumBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.scaleXNumBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.scaleXNumBox.Location = new System.Drawing.Point(3, 33);
            this.scaleXNumBox.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.scaleXNumBox.Minimum = new decimal(new int[] {
            -1,
            -1,
            -1,
            -2147483648});
            this.scaleXNumBox.Name = "scaleXNumBox";
            this.scaleXNumBox.Size = new System.Drawing.Size(73, 24);
            this.scaleXNumBox.TabIndex = 11;
            this.xHorizontalTooltip.SetToolTip(this.scaleXNumBox, "X (Horizontal)");
            this.scaleXNumBox.ValueChanged += new System.EventHandler(this.ModifierNumBoxValueChanged);
            this.scaleXNumBox.Enter += new System.EventHandler(this.ModifierNumBoxFocused);
            this.scaleXNumBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ModifierNumBoxEnterPressed);
            // 
            // transXNumBox
            // 
            this.transXNumBox.DecimalPlaces = 3;
            this.transXNumBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.transXNumBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.transXNumBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.transXNumBox.Location = new System.Drawing.Point(3, 3);
            this.transXNumBox.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.transXNumBox.Minimum = new decimal(new int[] {
            -1,
            -1,
            -1,
            -2147483648});
            this.transXNumBox.Name = "transXNumBox";
            this.transXNumBox.Size = new System.Drawing.Size(73, 24);
            this.transXNumBox.TabIndex = 11;
            this.xLeftRightTooltip.SetToolTip(this.transXNumBox, "X (Left/Right)");
            this.transXNumBox.ValueChanged += new System.EventHandler(this.ModifierNumBoxValueChanged);
            this.transXNumBox.Enter += new System.EventHandler(this.ModifierNumBoxFocused);
            this.transXNumBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ModifierNumBoxEnterPressed);
            // 
            // transYNumBox
            // 
            this.transYNumBox.DecimalPlaces = 3;
            this.transYNumBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.transYNumBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.transYNumBox.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.transYNumBox.Location = new System.Drawing.Point(82, 3);
            this.transYNumBox.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.transYNumBox.Minimum = new decimal(new int[] {
            -1,
            -1,
            -1,
            -2147483648});
            this.transYNumBox.Name = "transYNumBox";
            this.transYNumBox.Size = new System.Drawing.Size(73, 24);
            this.transYNumBox.TabIndex = 12;
            this.yUpDownTooltip.SetToolTip(this.transYNumBox, "Y (Up/Down)");
            this.transYNumBox.ValueChanged += new System.EventHandler(this.ModifierNumBoxValueChanged);
            this.transYNumBox.Enter += new System.EventHandler(this.ModifierNumBoxFocused);
            this.transYNumBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ModifierNumBoxEnterPressed);
            // 
            // toggleBackfacesCheckbox
            // 
            this.toggleBackfacesCheckbox.AutoSize = true;
            this.toggleBackfacesCheckbox.Location = new System.Drawing.Point(9, 201);
            this.toggleBackfacesCheckbox.Name = "toggleBackfacesCheckbox";
            this.toggleBackfacesCheckbox.Size = new System.Drawing.Size(15, 14);
            this.toggleBackfacesCheckbox.TabIndex = 29;
            this.toggleBackfacesCheckbox.UseVisualStyleBackColor = true;
            this.toggleBackfacesCheckbox.CheckedChanged += new System.EventHandler(this.ToggleBackFacesCheckboxChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(25, 201);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(123, 13);
            this.label11.TabIndex = 28;
            this.label11.Text = "Toggle Backface Culling";
            // 
            // reverseNormalsCheckbox
            // 
            this.reverseNormalsCheckbox.AutoSize = true;
            this.reverseNormalsCheckbox.Location = new System.Drawing.Point(9, 176);
            this.reverseNormalsCheckbox.Name = "reverseNormalsCheckbox";
            this.reverseNormalsCheckbox.Size = new System.Drawing.Size(15, 14);
            this.reverseNormalsCheckbox.TabIndex = 27;
            this.reverseNormalsCheckbox.UseVisualStyleBackColor = true;
            this.reverseNormalsCheckbox.CheckedChanged += new System.EventHandler(this.ReverseNormalsCheckboxChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(25, 176);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(88, 13);
            this.label12.TabIndex = 26;
            this.label12.Text = "Reverse Normals";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Rotation:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Scale:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Translation:";
            // 
            // materialsTabPage
            // 
            this.materialsTabPage.Controls.Add(this.deleteAllMaterialsButton);
            this.materialsTabPage.Controls.Add(this.materialsTableOKButton);
            this.materialsTabPage.Controls.Add(this.label14);
            this.materialsTabPage.Controls.Add(this.materialsPagePanelsContainer);
            this.materialsTabPage.Location = new System.Drawing.Point(4, 22);
            this.materialsTabPage.Name = "materialsTabPage";
            this.materialsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.materialsTabPage.Size = new System.Drawing.Size(1027, 462);
            this.materialsTabPage.TabIndex = 1;
            this.materialsTabPage.Text = "Materials";
            this.materialsTabPage.UseVisualStyleBackColor = true;
            // 
            // deleteAllMaterialsButton
            // 
            this.deleteAllMaterialsButton.Location = new System.Drawing.Point(283, 21);
            this.deleteAllMaterialsButton.Name = "deleteAllMaterialsButton";
            this.deleteAllMaterialsButton.Size = new System.Drawing.Size(84, 23);
            this.deleteAllMaterialsButton.TabIndex = 8;
            this.deleteAllMaterialsButton.Text = "Delete All";
            this.deleteAllMaterialsButton.UseVisualStyleBackColor = true;
            this.deleteAllMaterialsButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MaterialsTableDeleteAllButtonClicked);
            // 
            // materialsTableOKButton
            // 
            this.materialsTableOKButton.Location = new System.Drawing.Point(370, 21);
            this.materialsTableOKButton.Name = "materialsTableOKButton";
            this.materialsTableOKButton.Size = new System.Drawing.Size(45, 23);
            this.materialsTableOKButton.TabIndex = 7;
            this.materialsTableOKButton.Text = "OK";
            this.materialsTableOKButton.UseVisualStyleBackColor = true;
            this.materialsTableOKButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MaterialsTableOkButtonClicked);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(4, 5);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(45, 13);
            this.label14.TabIndex = 3;
            this.label14.Text = "Presets:";
            // 
            // materialsPagePanelsContainer
            // 
            this.materialsPagePanelsContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.materialsPagePanelsContainer.Location = new System.Drawing.Point(3, 3);
            this.materialsPagePanelsContainer.Name = "materialsPagePanelsContainer";
            // 
            // materialsPagePanelsContainer.Panel1
            // 
            this.materialsPagePanelsContainer.Panel1.Controls.Add(this.label9);
            this.materialsPagePanelsContainer.Panel1.Controls.Add(this.materialsTable);
            this.materialsPagePanelsContainer.Panel1.Controls.Add(this.applyPresetToAllMaterialsButton);
            this.materialsPagePanelsContainer.Panel1.Controls.Add(this.materialPresetsSelector);
            // 
            // materialsPagePanelsContainer.Panel2
            // 
            this.materialsPagePanelsContainer.Panel2.Controls.Add(this.label10);
            this.materialsPagePanelsContainer.Panel2.Controls.Add(this.applyMatBinTexturesButton);
            this.materialsPagePanelsContainer.Panel2.Controls.Add(this.texturesTable);
            this.materialsPagePanelsContainer.Size = new System.Drawing.Size(1021, 456);
            this.materialsPagePanelsContainer.SplitterDistance = 475;
            this.materialsPagePanelsContainer.TabIndex = 6;
            this.materialsPagePanelsContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.MaterialsPagePanelsContainerSplitterMoved);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(2, 43);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "Materials:";
            // 
            // materialsTable
            // 
            this.materialsTable.AllowUserToAddRows = false;
            this.materialsTable.AllowUserToResizeColumns = false;
            this.materialsTable.AllowUserToResizeRows = false;
            this.materialsTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.materialsTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.materialsTable.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.materialsTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.materialsTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.Column14,
            this.Column5,
            this.dataGridViewTextBoxColumn4,
            this.Column6,
            this.Column20,
            this.Column16,
            this.Column7,
            this.Column8});
            this.materialsTable.Location = new System.Drawing.Point(4, 59);
            this.materialsTable.Name = "materialsTable";
            this.materialsTable.RowHeadersVisible = false;
            this.materialsTable.RowHeadersWidth = 62;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.materialsTable.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.materialsTable.Size = new System.Drawing.Size(468, 394);
            this.materialsTable.TabIndex = 6;
            this.materialsTable.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.MaterialsTableButtonClicked);
            this.materialsTable.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.MaterialsTableCellValueChanged);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.Frozen = true;
            this.dataGridViewTextBoxColumn1.HeaderText = "Index";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 58;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.Frozen = true;
            this.dataGridViewTextBoxColumn2.HeaderText = "Name";
            this.dataGridViewTextBoxColumn2.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 60;
            // 
            // Column14
            // 
            this.Column14.HeaderText = "Flags";
            this.Column14.MinimumWidth = 8;
            this.Column14.Name = "Column14";
            this.Column14.Width = 57;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "MTD Path";
            this.Column5.MinimumWidth = 8;
            this.Column5.Name = "Column5";
            this.Column5.Width = 75;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Internal Index";
            this.dataGridViewTextBoxColumn4.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Width = 88;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "Edit Textures";
            this.Column6.MinimumWidth = 8;
            this.Column6.Name = "Column6";
            this.Column6.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column6.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Column6.Text = "";
            this.Column6.Width = 87;
            // 
            // Column20
            // 
            this.Column20.HeaderText = "Viewer Highlight";
            this.Column20.Name = "Column20";
            this.Column20.Width = 80;
            // 
            // Column16
            // 
            this.Column16.HeaderText = "Add Preset";
            this.Column16.MinimumWidth = 8;
            this.Column16.Name = "Column16";
            this.Column16.Width = 59;
            // 
            // Column7
            // 
            this.Column7.HeaderText = "Apply Preset";
            this.Column7.MinimumWidth = 8;
            this.Column7.Name = "Column7";
            this.Column7.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column7.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Column7.Width = 84;
            // 
            // Column8
            // 
            this.Column8.HeaderText = "Delete";
            this.Column8.MinimumWidth = 8;
            this.Column8.Name = "Column8";
            this.Column8.Width = 44;
            // 
            // applyPresetToAllMaterialsButton
            // 
            this.applyPresetToAllMaterialsButton.Location = new System.Drawing.Point(193, 18);
            this.applyPresetToAllMaterialsButton.Name = "applyPresetToAllMaterialsButton";
            this.applyPresetToAllMaterialsButton.Size = new System.Drawing.Size(84, 23);
            this.applyPresetToAllMaterialsButton.TabIndex = 4;
            this.applyPresetToAllMaterialsButton.Text = "Apply To All";
            this.applyPresetToAllMaterialsButton.UseVisualStyleBackColor = true;
            this.applyPresetToAllMaterialsButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MaterialsTableApplyToAllButtonClicked);
            // 
            // materialPresetsSelector
            // 
            this.materialPresetsSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.materialPresetsSelector.FormattingEnabled = true;
            this.materialPresetsSelector.Location = new System.Drawing.Point(4, 19);
            this.materialPresetsSelector.Name = "materialPresetsSelector";
            this.materialPresetsSelector.Size = new System.Drawing.Size(185, 21);
            this.materialPresetsSelector.TabIndex = 2;
            this.presetsBoxTooltip.SetToolTip(this.materialPresetsSelector, "Right-click the selector with a preset loaded to delete it");
            this.materialPresetsSelector.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MaterialPresetsSelector_MouseDown);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(0, 43);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(51, 13);
            this.label10.TabIndex = 11;
            this.label10.Text = "Textures:";
            // 
            // applyMatBinTexturesButton
            // 
            this.applyMatBinTexturesButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.applyMatBinTexturesButton.Location = new System.Drawing.Point(3, 17);
            this.applyMatBinTexturesButton.MaximumSize = new System.Drawing.Size(135, 233);
            this.applyMatBinTexturesButton.MinimumSize = new System.Drawing.Size(135, 23);
            this.applyMatBinTexturesButton.Name = "applyMatBinTexturesButton";
            this.applyMatBinTexturesButton.Size = new System.Drawing.Size(135, 23);
            this.applyMatBinTexturesButton.TabIndex = 10;
            this.applyMatBinTexturesButton.Text = "Apply MATBIN Textures";
            this.applyMatBinTexturesButton.UseVisualStyleBackColor = true;
            this.applyMatBinTexturesButton.Click += new System.EventHandler(this.ApplyMATBINTexturesButtonClicked);
            // 
            // texturesTable
            // 
            this.texturesTable.AllowUserToAddRows = false;
            this.texturesTable.AllowUserToResizeColumns = false;
            this.texturesTable.AllowUserToResizeRows = false;
            this.texturesTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.texturesTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.texturesTable.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.texturesTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.texturesTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6,
            this.dataGridViewButtonColumn1});
            this.texturesTable.Location = new System.Drawing.Point(3, 59);
            this.texturesTable.Name = "texturesTable";
            this.texturesTable.RowHeadersVisible = false;
            this.texturesTable.RowHeadersWidth = 62;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.texturesTable.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.texturesTable.Size = new System.Drawing.Size(536, 394);
            this.texturesTable.TabIndex = 7;
            this.texturesTable.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.TexturesTableButtonClicked);
            this.texturesTable.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.TexturesTableCellValueChanged);
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "Type";
            this.dataGridViewTextBoxColumn5.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.Width = 56;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.HeaderText = "Path";
            this.dataGridViewTextBoxColumn6.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.Width = 54;
            // 
            // dataGridViewButtonColumn1
            // 
            this.dataGridViewButtonColumn1.HeaderText = "Browse";
            this.dataGridViewButtonColumn1.MinimumWidth = 8;
            this.dataGridViewButtonColumn1.Name = "dataGridViewButtonColumn1";
            this.dataGridViewButtonColumn1.Width = 48;
            // 
            // bonesTabPage
            // 
            this.bonesTabPage.Controls.Add(this.bonesTable);
            this.bonesTabPage.Location = new System.Drawing.Point(4, 22);
            this.bonesTabPage.Name = "bonesTabPage";
            this.bonesTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.bonesTabPage.Size = new System.Drawing.Size(1027, 462);
            this.bonesTabPage.TabIndex = 0;
            this.bonesTabPage.Text = "Bones";
            this.bonesTabPage.UseVisualStyleBackColor = true;
            // 
            // bonesTable
            // 
            this.bonesTable.AllowUserToAddRows = false;
            this.bonesTable.AllowUserToResizeColumns = false;
            this.bonesTable.AllowUserToResizeRows = false;
            this.bonesTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bonesTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.bonesTable.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.bonesTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.bonesTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn7,
            this.dataGridViewTextBoxColumn8,
            this.dataGridViewTextBoxColumn9,
            this.dataGridViewTextBoxColumn10,
            this.Column9,
            this.Column10,
            this.Column11,
            this.Column12,
            this.Column13});
            this.bonesTable.Location = new System.Drawing.Point(3, 3);
            this.bonesTable.Name = "bonesTable";
            this.bonesTable.RowHeadersVisible = false;
            this.bonesTable.RowHeadersWidth = 62;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.bonesTable.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.bonesTable.Size = new System.Drawing.Size(1021, 456);
            this.bonesTable.TabIndex = 0;
            this.bonesTable.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.BonesTableCellValueChanged);
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.Frozen = true;
            this.dataGridViewTextBoxColumn7.HeaderText = "Index";
            this.dataGridViewTextBoxColumn7.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            this.dataGridViewTextBoxColumn7.Width = 58;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.Frozen = true;
            this.dataGridViewTextBoxColumn8.HeaderText = "Name";
            this.dataGridViewTextBoxColumn8.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.Width = 60;
            // 
            // dataGridViewTextBoxColumn9
            // 
            this.dataGridViewTextBoxColumn9.HeaderText = "ParentID";
            this.dataGridViewTextBoxColumn9.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            this.dataGridViewTextBoxColumn9.Width = 74;
            // 
            // dataGridViewTextBoxColumn10
            // 
            this.dataGridViewTextBoxColumn10.HeaderText = "ChildID";
            this.dataGridViewTextBoxColumn10.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            this.dataGridViewTextBoxColumn10.Width = 66;
            // 
            // Column9
            // 
            this.Column9.HeaderText = "Translation";
            this.Column9.MinimumWidth = 8;
            this.Column9.Name = "Column9";
            this.Column9.Width = 84;
            // 
            // Column10
            // 
            this.Column10.HeaderText = "Scale";
            this.Column10.MinimumWidth = 8;
            this.Column10.Name = "Column10";
            this.Column10.Width = 59;
            // 
            // Column11
            // 
            this.Column11.HeaderText = "Rotation";
            this.Column11.MinimumWidth = 8;
            this.Column11.Name = "Column11";
            this.Column11.Width = 72;
            // 
            // Column12
            // 
            this.Column12.HeaderText = "BB Min";
            this.Column12.MinimumWidth = 8;
            this.Column12.Name = "Column12";
            this.Column12.Width = 66;
            // 
            // Column13
            // 
            this.Column13.HeaderText = "BB Max";
            this.Column13.MinimumWidth = 8;
            this.Column13.Name = "Column13";
            this.Column13.Width = 69;
            // 
            // tabWindow
            // 
            this.tabWindow.AllowDrop = true;
            this.tabWindow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabWindow.Controls.Add(this.bonesTabPage);
            this.tabWindow.Controls.Add(this.materialsTabPage);
            this.tabWindow.Controls.Add(this.meshTabPage);
            this.tabWindow.Location = new System.Drawing.Point(0, 24);
            this.tabWindow.Name = "tabWindow";
            this.tabWindow.SelectedIndex = 0;
            this.tabWindow.Size = new System.Drawing.Size(1035, 488);
            this.tabWindow.TabIndex = 1;
            this.tabWindow.SelectedIndexChanged += new System.EventHandler(this.TabWindow_SelectedIndexChanged);
            this.tabWindow.Click += new System.EventHandler(this.DefocusSearchBox);
            this.tabWindow.DragDrop += new System.Windows.Forms.DragEventHandler(this.TabWindowDragDrop);
            this.tabWindow.DragEnter += new System.Windows.Forms.DragEventHandler(this.TabWindowDragEnter);
            // 
            // copyrightStr
            // 
            this.copyrightStr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.copyrightStr.AutoSize = true;
            this.copyrightStr.ForeColor = System.Drawing.Color.DarkGray;
            this.copyrightStr.Location = new System.Drawing.Point(860, 30);
            this.copyrightStr.Name = "copyrightStr";
            this.copyrightStr.Size = new System.Drawing.Size(160, 13);
            this.copyrightStr.TabIndex = 1;
            this.copyrightStr.Text = "© Pear, 2023 All rights reserved.";
            // 
            // versionStr
            // 
            this.versionStr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.versionStr.AutoSize = true;
            this.versionStr.ForeColor = System.Drawing.Color.DarkGray;
            this.versionStr.Location = new System.Drawing.Point(783, 30);
            this.versionStr.Name = "versionStr";
            this.versionStr.Size = new System.Drawing.Size(45, 13);
            this.versionStr.TabIndex = 2;
            this.versionStr.Text = "Version:";
            // 
            // autoSaveTimer
            // 
            this.autoSaveTimer.Interval = 60000;
            this.autoSaveTimer.Tick += new System.EventHandler(this.AutoSaveTimerTick);
            // 
            // searchBox
            // 
            this.searchBox.Location = new System.Drawing.Point(193, 25);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(226, 20);
            this.searchBox.TabIndex = 3;
            this.searchBox.TextChanged += new System.EventHandler(this.SearchBox_TextChanged);
            // 
            // searchBoxStr
            // 
            this.searchBoxStr.AutoSize = true;
            this.searchBoxStr.Location = new System.Drawing.Point(146, 28);
            this.searchBoxStr.Name = "searchBoxStr";
            this.searchBoxStr.Size = new System.Drawing.Size(44, 13);
            this.searchBoxStr.TabIndex = 1;
            this.searchBoxStr.Text = "Search:";
            // 
            // meshTabDataTableSelector
            // 
            this.meshTabDataTableSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.meshTabDataTableSelector.FormattingEnabled = true;
            this.meshTabDataTableSelector.Items.AddRange(new object[] {
            "Mesh",
            "Dummies"});
            this.meshTabDataTableSelector.Location = new System.Drawing.Point(424, 24);
            this.meshTabDataTableSelector.Name = "meshTabDataTableSelector";
            this.meshTabDataTableSelector.Size = new System.Drawing.Size(124, 21);
            this.meshTabDataTableSelector.TabIndex = 1;
            this.meshTabDataTableSelector.SelectedIndexChanged += new System.EventHandler(this.MeshTabDataTableSelector_SelectedIndexChanged);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1034, 511);
            this.Controls.Add(this.searchBox);
            this.Controls.Add(this.copyrightStr);
            this.Controls.Add(this.versionStr);
            this.Controls.Add(this.searchBoxStr);
            this.Controls.Add(this.meshTabDataTableSelector);
            this.Controls.Add(this.tabWindow);
            this.Controls.Add(this.ribbon);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.ribbon;
            this.MinimumSize = new System.Drawing.Size(1050, 450);
            this.Name = "MainWindow";
            this.Text = "FLVER Editor";
            this.TransparencyKey = System.Drawing.Color.Maroon;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindowClosing);
            this.Load += new System.EventHandler(this.MainWindowLoad);
            this.Shown += new System.EventHandler(this.DefocusSearchBox);
            this.LocationChanged += new System.EventHandler(this.MainWindow_LocationChanged);
            this.SizeChanged += new System.EventHandler(this.MainWindow_SizeChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainWindowKeyDown);
            this.ribbon.ResumeLayout(false);
            this.ribbon.PerformLayout();
            this.multiSelectMenu.ResumeLayout(false);
            this.multiSelectMenu.PerformLayout();
            this.meshTabPage.ResumeLayout(false);
            this.functionsGroupBox.ResumeLayout(false);
            this.meshPagePanelsContainer.Panel1.ResumeLayout(false);
            this.meshPagePanelsContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.meshPagePanelsContainer)).EndInit();
            this.meshPagePanelsContainer.ResumeLayout(false);
            this.meshPageTablesContainer.Panel1.ResumeLayout(false);
            this.meshPageTablesContainer.Panel1.PerformLayout();
            this.meshPageTablesContainer.Panel2.ResumeLayout(false);
            this.meshPageTablesContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.meshPageTablesContainer)).EndInit();
            this.meshPageTablesContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.meshTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dummiesTable)).EndInit();
            this.meshModifiersContainer.ResumeLayout(false);
            this.meshModifiersContainer.PerformLayout();
            this.meshModifiersNumBoxesContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rotZNumBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleZNumBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotYNumBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.transZNumBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rotXNumBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleYNumBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scaleXNumBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.transXNumBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.transYNumBox)).EndInit();
            this.materialsTabPage.ResumeLayout(false);
            this.materialsTabPage.PerformLayout();
            this.materialsPagePanelsContainer.Panel1.ResumeLayout(false);
            this.materialsPagePanelsContainer.Panel1.PerformLayout();
            this.materialsPagePanelsContainer.Panel2.ResumeLayout(false);
            this.materialsPagePanelsContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.materialsPagePanelsContainer)).EndInit();
            this.materialsPagePanelsContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.materialsTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.texturesTable)).EndInit();
            this.bonesTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bonesTable)).EndInit();
            this.tabWindow.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.MenuStrip ribbon;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip multiSelectMenu;
        private System.Windows.Forms.ToolStripTextBox multiSelectMenuTextbox;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.TabPage meshTabPage;
        private System.Windows.Forms.TabPage materialsTabPage;
        private System.Windows.Forms.Button applyPresetToAllMaterialsButton;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox materialPresetsSelector;
        private System.Windows.Forms.TabPage bonesTabPage;
        private System.Windows.Forms.DataGridView bonesTable;
        private System.Windows.Forms.TabControl tabWindow;
        private System.Windows.Forms.Label copyrightStr;
        private System.Windows.Forms.SplitContainer materialsPagePanelsContainer;
        private System.Windows.Forms.DataGridView materialsTable;
        private System.Windows.Forms.DataGridView texturesTable;
        private System.Windows.Forms.SplitContainer meshPagePanelsContainer;
        private System.Windows.Forms.DataGridView meshTable;
        private System.Windows.Forms.GroupBox meshModifiersContainer;
        private System.Windows.Forms.TableLayoutPanel meshModifiersNumBoxesContainer;
        private System.Windows.Forms.NumericUpDown rotZNumBox;
        private System.Windows.Forms.NumericUpDown scaleZNumBox;
        private System.Windows.Forms.NumericUpDown rotYNumBox;
        private System.Windows.Forms.NumericUpDown transZNumBox;
        private System.Windows.Forms.NumericUpDown rotXNumBox;
        private System.Windows.Forms.NumericUpDown scaleYNumBox;
        private System.Windows.Forms.NumericUpDown scaleXNumBox;
        private System.Windows.Forms.NumericUpDown transXNumBox;
        private System.Windows.Forms.NumericUpDown transYNumBox;
        private System.Windows.Forms.CheckBox toggleBackfacesCheckbox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox reverseNormalsCheckbox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button materialsTableOKButton;
        private System.Windows.Forms.Button deleteAllMaterialsButton;
        private System.Windows.Forms.Button selectAllMeshesButton;
        private System.Windows.Forms.Button deleteSelectedButton;
        private System.Windows.Forms.CheckBox reverseFacesetsCheckbox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DataGridView dummiesTable;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox dummyPresetsSelector;
        private System.Windows.Forms.Button dummiesTableOKButton;
        private System.Windows.Forms.Button addAllDummiesToPresetsButton;
        private System.Windows.Forms.SplitContainer meshPageTablesContainer;
        private System.Windows.Forms.Label versionStr;
        private System.Windows.Forms.Button addDummyButton;
        private System.Windows.Forms.CheckBox uniformScaleCheckbox;
        private System.Windows.Forms.Button centerToWorldButton;
        private System.Windows.Forms.CheckBox deleteFacesetsCheckbox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ToolStripMenuItem loadJSONToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bonesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem materialsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportJSONToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bonesToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem materialsToolStripMenuItem1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column11;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column12;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column13;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn11;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column15;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column17;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn1;
        private System.Windows.Forms.DataGridViewButtonColumn Column18;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toggleTextureRefreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem presetsFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem browseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mergeToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem materialsToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem dummiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem materialsToolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem dummiesToolStripMenuItem1;
        private System.Windows.Forms.Button applyMatBinTexturesButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewButtonColumn dataGridViewButtonColumn1;
        private System.Windows.Forms.ToolStripMenuItem dummyThicknessToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox dummyThicknessSelector;
        private System.Windows.Forms.CheckBox mirrorZCheckbox;
        private System.Windows.Forms.CheckBox mirrorYCheckbox;
        private System.Windows.Forms.CheckBox mirrorXCheckbox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ToolStripMenuItem toggleAutoSaveToolStripMenuItem;
        private System.Windows.Forms.Timer autoSaveTimer;
        private System.Windows.Forms.ToolStripTextBox autoSaveIntervalSelector;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem materialsToolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem dummiesToolStripMenuItem2;
        private System.Windows.Forms.Button selectAllDummiesButton;
        private System.Windows.Forms.Button solveAllBBsButton;
        private System.Windows.Forms.Button setAllBBsMaxSizeButton;
        private System.Windows.Forms.ToolTip selectAllMeshButtonTooltip;
        private System.Windows.Forms.ToolTip selectAllDummiesButtonTooltip;
        private System.Windows.Forms.ToolTip solveAllBBsButtonTooltip;
        private System.Windows.Forms.ToolTip setAllBBsMaxSizeButtonTooltip;
        private System.Windows.Forms.Button hideAllMeshesButton;
        private System.Windows.Forms.ToolTip hideAllMeshesButtonTooltip;
        private System.Windows.Forms.GroupBox functionsGroupBox;
        private System.Windows.Forms.ToolStripMenuItem supportPearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem patreonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem payPalToolStripMenuItem;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ToolTip uniformScaleTooltip;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewButtonColumn Column3;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column4;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column21;
        private System.Windows.Forms.ToolStripMenuItem toggleDummyIDsVisibilityToolStripMenuItem;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.Label searchBoxStr;
        private System.Windows.Forms.ComboBox meshTabDataTableSelector;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column14;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewButtonColumn Column6;
        private System.Windows.Forms.DataGridViewButtonColumn Column20;
        private System.Windows.Forms.DataGridViewButtonColumn Column16;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column7;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column8;
        private System.Windows.Forms.ToolTip presetsBoxTooltip;
        private System.Windows.Forms.ToolTip zCartwheelTooltip;
        private System.Windows.Forms.ToolTip zThicknessTooltip;
        private System.Windows.Forms.ToolTip yRollTooltip;
        private System.Windows.Forms.ToolTip zInOutTooltip;
        private System.Windows.Forms.ToolTip xSwivelTooltip;
        private System.Windows.Forms.ToolTip yVerticalTooltip;
        private System.Windows.Forms.ToolTip xHorizontalTooltip;
        private System.Windows.Forms.ToolTip xLeftRightTooltip;
        private System.Windows.Forms.ToolTip yUpDownTooltip;
        private System.Windows.Forms.Label xAxisLabel;
        private System.Windows.Forms.Label zAxisLabel;
        private System.Windows.Forms.Label yAxisLabel;
        private System.Windows.Forms.ToolStripMenuItem modelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mergeToolStripMenuItem2;
        private System.Windows.Forms.Button displayMaleBodyButton;
        private System.Windows.Forms.Button displayFemaleBodyButton;
        private System.Windows.Forms.ToolTip displayMaleBodyTooltip;
        private System.Windows.Forms.ToolTip displayFemaleBodyTooltip;
    }
}