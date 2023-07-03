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
            importOptionsGroupBox = new System.Windows.Forms.GroupBox();
            meshSelectorGroupBox = new System.Windows.Forms.GroupBox();
            modifyAllMeshesCheckbox = new System.Windows.Forms.CheckBox();
            meshSelector = new EasyCompletionComboBox();
            skinnedMeshCheckbox = new System.Windows.Forms.CheckBox();
            mtdSelectorGroupBox = new System.Windows.Forms.GroupBox();
            mtdSelector = new EasyCompletionComboBox();
            createDefaultBoneCheckbox = new System.Windows.Forms.CheckBox();
            mirrorXCheckbox = new System.Windows.Forms.CheckBox();
            importOptionsGroupBox.SuspendLayout();
            meshSelectorGroupBox.SuspendLayout();
            mtdSelectorGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // importOptionsGroupBox
            // 
            importOptionsGroupBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            importOptionsGroupBox.Controls.Add(meshSelectorGroupBox);
            importOptionsGroupBox.Location = new System.Drawing.Point(9, 6);
            importOptionsGroupBox.Name = "importOptionsGroupBox";
            importOptionsGroupBox.Size = new System.Drawing.Size(540, 163);
            importOptionsGroupBox.TabIndex = 1;
            importOptionsGroupBox.TabStop = false;
            importOptionsGroupBox.Text = "Import Options";
            // 
            // meshSelectorGroupBox
            // 
            meshSelectorGroupBox.Controls.Add(modifyAllMeshesCheckbox);
            meshSelectorGroupBox.Controls.Add(meshSelector);
            meshSelectorGroupBox.Controls.Add(skinnedMeshCheckbox);
            meshSelectorGroupBox.Controls.Add(mtdSelectorGroupBox);
            meshSelectorGroupBox.Controls.Add(createDefaultBoneCheckbox);
            meshSelectorGroupBox.Controls.Add(mirrorXCheckbox);
            meshSelectorGroupBox.Location = new System.Drawing.Point(6, 21);
            meshSelectorGroupBox.Name = "meshSelectorGroupBox";
            meshSelectorGroupBox.Size = new System.Drawing.Size(528, 136);
            meshSelectorGroupBox.TabIndex = 5;
            meshSelectorGroupBox.TabStop = false;
            meshSelectorGroupBox.Text = "Mesh Selector";
            // 
            // modifyAllMeshesCheckbox
            // 
            modifyAllMeshesCheckbox.AutoSize = true;
            modifyAllMeshesCheckbox.Location = new System.Drawing.Point(398, 24);
            modifyAllMeshesCheckbox.Name = "modifyAllMeshesCheckbox";
            modifyAllMeshesCheckbox.Size = new System.Drawing.Size(124, 19);
            modifyAllMeshesCheckbox.TabIndex = 2;
            modifyAllMeshesCheckbox.Text = "Modify All Meshes";
            modifyAllMeshesCheckbox.UseVisualStyleBackColor = true;
            // 
            // meshSelector
            // 
            meshSelector.FormattingEnabled = true;
            meshSelector.Location = new System.Drawing.Point(6, 22);
            meshSelector.Name = "meshSelector";
            meshSelector.Size = new System.Drawing.Size(384, 23);
            meshSelector.TabIndex = 0;
            // 
            // skinnedMeshCheckbox
            // 
            skinnedMeshCheckbox.AutoSize = true;
            skinnedMeshCheckbox.Location = new System.Drawing.Point(259, 110);
            skinnedMeshCheckbox.Name = "skinnedMeshCheckbox";
            skinnedMeshCheckbox.Size = new System.Drawing.Size(100, 19);
            skinnedMeshCheckbox.TabIndex = 4;
            skinnedMeshCheckbox.Text = "Skinned Mesh";
            skinnedMeshCheckbox.UseVisualStyleBackColor = true;
            // 
            // mtdSelectorGroupBox
            // 
            mtdSelectorGroupBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            mtdSelectorGroupBox.Controls.Add(mtdSelector);
            mtdSelectorGroupBox.Location = new System.Drawing.Point(6, 51);
            mtdSelectorGroupBox.Name = "mtdSelectorGroupBox";
            mtdSelectorGroupBox.Size = new System.Drawing.Size(516, 54);
            mtdSelectorGroupBox.TabIndex = 3;
            mtdSelectorGroupBox.TabStop = false;
            mtdSelectorGroupBox.Text = "MTD Selector";
            // 
            // mtdSelector
            // 
            mtdSelector.FormattingEnabled = true;
            mtdSelector.Location = new System.Drawing.Point(6, 22);
            mtdSelector.Name = "mtdSelector";
            mtdSelector.Size = new System.Drawing.Size(504, 23);
            mtdSelector.TabIndex = 1;
            // 
            // createDefaultBoneCheckbox
            // 
            createDefaultBoneCheckbox.AutoSize = true;
            createDefaultBoneCheckbox.Location = new System.Drawing.Point(7, 110);
            createDefaultBoneCheckbox.Name = "createDefaultBoneCheckbox";
            createDefaultBoneCheckbox.Size = new System.Drawing.Size(131, 19);
            createDefaultBoneCheckbox.TabIndex = 0;
            createDefaultBoneCheckbox.Text = "Create Default Bone";
            createDefaultBoneCheckbox.UseVisualStyleBackColor = true;
            // 
            // mirrorXCheckbox
            // 
            mirrorXCheckbox.AutoSize = true;
            mirrorXCheckbox.Location = new System.Drawing.Point(142, 110);
            mirrorXCheckbox.Name = "mirrorXCheckbox";
            mirrorXCheckbox.Size = new System.Drawing.Size(113, 19);
            mirrorXCheckbox.TabIndex = 1;
            mirrorXCheckbox.Text = "Mirror on X-Axis";
            mirrorXCheckbox.UseVisualStyleBackColor = true;
            // 
            // ImporterOpenFileDialog
            // 
            Controls.Add(importOptionsGroupBox);
            Name = "ImporterOpenFileDialog";
            importOptionsGroupBox.ResumeLayout(false);
            meshSelectorGroupBox.ResumeLayout(false);
            meshSelectorGroupBox.PerformLayout();
            mtdSelectorGroupBox.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.GroupBox importOptionsGroupBox;
        private System.Windows.Forms.CheckBox mirrorXCheckbox;
        private System.Windows.Forms.GroupBox mtdSelectorGroupBox;
        private System.Windows.Forms.CheckBox skinnedMeshCheckbox;
        private System.Windows.Forms.CheckBox createDefaultBoneCheckbox;
        private System.Windows.Forms.GroupBox meshSelectorGroupBox;
        private EasyCompletionComboBox meshSelector;
        private EasyCompletionComboBox mtdSelector;
        private System.Windows.Forms.CheckBox modifyAllMeshesCheckbox;
    }
}
