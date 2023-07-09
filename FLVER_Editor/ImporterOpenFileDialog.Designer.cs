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
            importOptionsGroupBox = new GroupBox();
            meshSelectorGroupBox = new GroupBox();
            modifyAllMeshesCheckbox = new CheckBox();
            meshSelector = new EasyCompletionComboBox();
            skinnedMeshCheckbox = new CheckBox();
            mtdSelectorGroupBox = new GroupBox();
            mtdSelector = new EasyCompletionComboBox();
            createDefaultBoneCheckbox = new CheckBox();
            mirrorXCheckbox = new CheckBox();
            mirrorZCheckbox = new CheckBox();
            importOptionsGroupBox.SuspendLayout();
            meshSelectorGroupBox.SuspendLayout();
            mtdSelectorGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // importOptionsGroupBox
            // 
            importOptionsGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            importOptionsGroupBox.Controls.Add(meshSelectorGroupBox);
            importOptionsGroupBox.Location = new Point(9, 6);
            importOptionsGroupBox.Name = "importOptionsGroupBox";
            importOptionsGroupBox.Size = new Size(540, 163);
            importOptionsGroupBox.TabIndex = 1;
            importOptionsGroupBox.TabStop = false;
            importOptionsGroupBox.Text = "Import Options";
            // 
            // meshSelectorGroupBox
            // 
            meshSelectorGroupBox.Controls.Add(mirrorZCheckbox);
            meshSelectorGroupBox.Controls.Add(modifyAllMeshesCheckbox);
            meshSelectorGroupBox.Controls.Add(meshSelector);
            meshSelectorGroupBox.Controls.Add(skinnedMeshCheckbox);
            meshSelectorGroupBox.Controls.Add(mtdSelectorGroupBox);
            meshSelectorGroupBox.Controls.Add(createDefaultBoneCheckbox);
            meshSelectorGroupBox.Controls.Add(mirrorXCheckbox);
            meshSelectorGroupBox.Location = new Point(6, 21);
            meshSelectorGroupBox.Name = "meshSelectorGroupBox";
            meshSelectorGroupBox.Size = new Size(528, 136);
            meshSelectorGroupBox.TabIndex = 5;
            meshSelectorGroupBox.TabStop = false;
            meshSelectorGroupBox.Text = "Mesh Selector";
            // 
            // modifyAllMeshesCheckbox
            // 
            modifyAllMeshesCheckbox.AutoSize = true;
            modifyAllMeshesCheckbox.Location = new Point(398, 24);
            modifyAllMeshesCheckbox.Name = "modifyAllMeshesCheckbox";
            modifyAllMeshesCheckbox.Size = new Size(124, 19);
            modifyAllMeshesCheckbox.TabIndex = 2;
            modifyAllMeshesCheckbox.Text = "Modify All Meshes";
            modifyAllMeshesCheckbox.UseVisualStyleBackColor = true;
            // 
            // meshSelector
            // 
            meshSelector.FormattingEnabled = true;
            meshSelector.Location = new Point(6, 22);
            meshSelector.Name = "meshSelector";
            meshSelector.Size = new Size(384, 23);
            meshSelector.TabIndex = 0;
            // 
            // skinnedMeshCheckbox
            // 
            skinnedMeshCheckbox.AutoSize = true;
            skinnedMeshCheckbox.Location = new Point(292, 110);
            skinnedMeshCheckbox.Name = "skinnedMeshCheckbox";
            skinnedMeshCheckbox.Size = new Size(100, 19);
            skinnedMeshCheckbox.TabIndex = 4;
            skinnedMeshCheckbox.Text = "Skinned Mesh";
            skinnedMeshCheckbox.UseVisualStyleBackColor = true;
            // 
            // mtdSelectorGroupBox
            // 
            mtdSelectorGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            mtdSelectorGroupBox.Controls.Add(mtdSelector);
            mtdSelectorGroupBox.Location = new Point(6, 51);
            mtdSelectorGroupBox.Name = "mtdSelectorGroupBox";
            mtdSelectorGroupBox.Size = new Size(516, 54);
            mtdSelectorGroupBox.TabIndex = 3;
            mtdSelectorGroupBox.TabStop = false;
            mtdSelectorGroupBox.Text = "MTD Selector";
            // 
            // mtdSelector
            // 
            mtdSelector.FormattingEnabled = true;
            mtdSelector.Location = new Point(6, 22);
            mtdSelector.Name = "mtdSelector";
            mtdSelector.Size = new Size(504, 23);
            mtdSelector.TabIndex = 1;
            // 
            // createDefaultBoneCheckbox
            // 
            createDefaultBoneCheckbox.AutoSize = true;
            createDefaultBoneCheckbox.Location = new Point(7, 110);
            createDefaultBoneCheckbox.Name = "createDefaultBoneCheckbox";
            createDefaultBoneCheckbox.Size = new Size(131, 19);
            createDefaultBoneCheckbox.TabIndex = 0;
            createDefaultBoneCheckbox.Text = "Create Default Bone";
            createDefaultBoneCheckbox.UseVisualStyleBackColor = true;
            // 
            // mirrorXCheckbox
            // 
            mirrorXCheckbox.AutoSize = true;
            mirrorXCheckbox.Location = new Point(142, 110);
            mirrorXCheckbox.Name = "mirrorXCheckbox";
            mirrorXCheckbox.Size = new Size(69, 19);
            mirrorXCheckbox.TabIndex = 1;
            mirrorXCheckbox.Text = "Mirror X";
            mirrorXCheckbox.UseVisualStyleBackColor = true;
            // 
            // mirrorZCheckbox
            // 
            mirrorZCheckbox.AutoSize = true;
            mirrorZCheckbox.Location = new Point(217, 110);
            mirrorZCheckbox.Name = "mirrorZCheckbox";
            mirrorZCheckbox.Size = new Size(69, 19);
            mirrorZCheckbox.TabIndex = 5;
            mirrorZCheckbox.Text = "Mirror Z";
            mirrorZCheckbox.UseVisualStyleBackColor = true;
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
        private System.Windows.Forms.CheckBox mirrorZCheckbox;
    }
}
