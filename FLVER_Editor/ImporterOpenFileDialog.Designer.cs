namespace FLVER_Editor
{
    partial class ImporterOpenFileDialog
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            importOptionsGroupBox = new GroupBox();
            groupBox1 = new GroupBox();
            label1 = new Label();
            mtdSelectorGroupBox = new GroupBox();
            mtdSelector = new EasyCompletionComboBox();
            weightingModeGroupBox = new GroupBox();
            weightingModeSelector = new ComboBox();
            boneWeightsMessage = new Label();
            autoAssignMtdCheckbox = new CheckBox();
            meshSelectorGroupBox = new GroupBox();
            meshSelector = new ListBox();
            affectAllMeshesCheckbox = new CheckBox();
            contextMenuStrip1 = new ContextMenuStrip(components);
            flipFacesCheckbox = new CheckBox();
            importOptionsGroupBox.SuspendLayout();
            groupBox1.SuspendLayout();
            mtdSelectorGroupBox.SuspendLayout();
            weightingModeGroupBox.SuspendLayout();
            meshSelectorGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // importOptionsGroupBox
            // 
            importOptionsGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            importOptionsGroupBox.Controls.Add(groupBox1);
            importOptionsGroupBox.Controls.Add(boneWeightsMessage);
            importOptionsGroupBox.Controls.Add(autoAssignMtdCheckbox);
            importOptionsGroupBox.Controls.Add(meshSelectorGroupBox);
            importOptionsGroupBox.Location = new Point(9, 6);
            importOptionsGroupBox.Name = "importOptionsGroupBox";
            importOptionsGroupBox.Size = new Size(543, 376);
            importOptionsGroupBox.TabIndex = 1;
            importOptionsGroupBox.TabStop = false;
            importOptionsGroupBox.Text = "Import Options";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(flipFacesCheckbox);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(mtdSelectorGroupBox);
            groupBox1.Controls.Add(weightingModeGroupBox);
            groupBox1.Location = new Point(273, 22);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(264, 208);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "Mesh Specific";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            label1.ForeColor = SystemColors.ControlDarkDark;
            label1.Location = new Point(6, 25);
            label1.Name = "label1";
            label1.Size = new Size(240, 26);
            label1.TabIndex = 5;
            label1.Text = "Modify the settings of the current mesh \r\nor all Meshes if \"Affect All Meshes\" is checked";
            // 
            // mtdSelectorGroupBox
            // 
            mtdSelectorGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            mtdSelectorGroupBox.Controls.Add(mtdSelector);
            mtdSelectorGroupBox.Location = new Point(6, 60);
            mtdSelectorGroupBox.Name = "mtdSelectorGroupBox";
            mtdSelectorGroupBox.Size = new Size(252, 54);
            mtdSelectorGroupBox.TabIndex = 3;
            mtdSelectorGroupBox.TabStop = false;
            mtdSelectorGroupBox.Text = "MTD Selector";
            // 
            // mtdSelector
            // 
            mtdSelector.FormattingEnabled = true;
            mtdSelector.Location = new Point(6, 22);
            mtdSelector.Name = "mtdSelector";
            mtdSelector.Size = new Size(240, 23);
            mtdSelector.TabIndex = 1;
            // 
            // weightingModeGroupBox
            // 
            weightingModeGroupBox.Controls.Add(weightingModeSelector);
            weightingModeGroupBox.Location = new Point(6, 120);
            weightingModeGroupBox.Name = "weightingModeGroupBox";
            weightingModeGroupBox.Size = new Size(252, 56);
            weightingModeGroupBox.TabIndex = 4;
            weightingModeGroupBox.TabStop = false;
            weightingModeGroupBox.Text = "Weighting Mode";
            // 
            // weightingModeSelector
            // 
            weightingModeSelector.FormattingEnabled = true;
            weightingModeSelector.Location = new Point(6, 22);
            weightingModeSelector.Name = "weightingModeSelector";
            weightingModeSelector.Size = new Size(240, 23);
            weightingModeSelector.TabIndex = 3;
            // 
            // boneWeightsMessage
            // 
            boneWeightsMessage.AutoSize = true;
            boneWeightsMessage.ForeColor = Color.Red;
            boneWeightsMessage.Location = new Point(6, 332);
            boneWeightsMessage.Name = "boneWeightsMessage";
            boneWeightsMessage.Size = new Size(21, 15);
            boneWeightsMessage.TabIndex = 2;
            boneWeightsMessage.Text = "{0}";
            // 
            // autoAssignMtdCheckbox
            // 
            autoAssignMtdCheckbox.AutoSize = true;
            autoAssignMtdCheckbox.Location = new Point(273, 236);
            autoAssignMtdCheckbox.Name = "autoAssignMtdCheckbox";
            autoAssignMtdCheckbox.Size = new Size(216, 19);
            autoAssignMtdCheckbox.TabIndex = 5;
            autoAssignMtdCheckbox.Text = "Auto-Assign MTD from Mesh Name";
            autoAssignMtdCheckbox.UseVisualStyleBackColor = true;
            autoAssignMtdCheckbox.CheckedChanged += AutoAssignMtdCheckbox_CheckedChanged;
            // 
            // meshSelectorGroupBox
            // 
            meshSelectorGroupBox.Controls.Add(meshSelector);
            meshSelectorGroupBox.Controls.Add(affectAllMeshesCheckbox);
            meshSelectorGroupBox.Location = new Point(6, 22);
            meshSelectorGroupBox.Name = "meshSelectorGroupBox";
            meshSelectorGroupBox.Size = new Size(261, 307);
            meshSelectorGroupBox.TabIndex = 5;
            meshSelectorGroupBox.TabStop = false;
            meshSelectorGroupBox.Text = "Mesh Selector";
            // 
            // meshSelector
            // 
            meshSelector.FormattingEnabled = true;
            meshSelector.ItemHeight = 15;
            meshSelector.Location = new Point(6, 52);
            meshSelector.Name = "meshSelector";
            meshSelector.Size = new Size(243, 244);
            meshSelector.TabIndex = 3;
            // 
            // affectAllMeshesCheckbox
            // 
            affectAllMeshesCheckbox.AutoSize = true;
            affectAllMeshesCheckbox.Location = new Point(12, 24);
            affectAllMeshesCheckbox.Name = "affectAllMeshesCheckbox";
            affectAllMeshesCheckbox.Size = new Size(118, 19);
            affectAllMeshesCheckbox.TabIndex = 2;
            affectAllMeshesCheckbox.Text = "Affect All Meshes";
            affectAllMeshesCheckbox.UseVisualStyleBackColor = true;
            affectAllMeshesCheckbox.CheckedChanged += affectAllMeshesCheckbox_CheckedChanged;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(61, 4);
            // 
            // flipFacesCheckbox
            // 
            flipFacesCheckbox.AutoSize = true;
            flipFacesCheckbox.Location = new Point(6, 182);
            flipFacesCheckbox.Name = "flipFacesCheckbox";
            flipFacesCheckbox.Size = new Size(77, 19);
            flipFacesCheckbox.TabIndex = 6;
            flipFacesCheckbox.Text = "Flip Faces";
            flipFacesCheckbox.UseVisualStyleBackColor = true;
            // 
            // ImporterOpenFileDialog
            // 
            Controls.Add(importOptionsGroupBox);
            Name = "ImporterOpenFileDialog";
            importOptionsGroupBox.ResumeLayout(false);
            importOptionsGroupBox.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            mtdSelectorGroupBox.ResumeLayout(false);
            weightingModeGroupBox.ResumeLayout(false);
            meshSelectorGroupBox.ResumeLayout(false);
            meshSelectorGroupBox.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private GroupBox importOptionsGroupBox;
        private GroupBox mtdSelectorGroupBox;
        private GroupBox meshSelectorGroupBox;
        private EasyCompletionComboBox mtdSelector;
        private CheckBox affectAllMeshesCheckbox;
        private Label boneWeightsMessage;
        private GroupBox weightingModeGroupBox;
        private ComboBox weightingModeSelector;
        private CheckBox autoAssignMtdCheckbox;
        private ListBox meshSelector;
        private GroupBox groupBox1;
        private Label label1;
        private ContextMenuStrip contextMenuStrip1;
        private CheckBox flipFacesCheckbox;
    }
}
