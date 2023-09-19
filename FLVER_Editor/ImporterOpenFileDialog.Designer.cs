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
            affectAllMeshesCheckbox = new CheckBox();
            meshSelector = new EasyCompletionComboBox();
            staticMeshCheckbox = new CheckBox();
            mtdSelectorGroupBox = new GroupBox();
            mtdSelector = new EasyCompletionComboBox();
            createDefaultBoneCheckbox = new CheckBox();
            boneWeightsMessage = new Label();
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
            meshSelectorGroupBox.Controls.Add(affectAllMeshesCheckbox);
            meshSelectorGroupBox.Controls.Add(meshSelector);
            meshSelectorGroupBox.Controls.Add(staticMeshCheckbox);
            meshSelectorGroupBox.Controls.Add(mtdSelectorGroupBox);
            meshSelectorGroupBox.Controls.Add(createDefaultBoneCheckbox);
            meshSelectorGroupBox.Location = new Point(6, 21);
            meshSelectorGroupBox.Name = "meshSelectorGroupBox";
            meshSelectorGroupBox.Size = new Size(528, 136);
            meshSelectorGroupBox.TabIndex = 5;
            meshSelectorGroupBox.TabStop = false;
            meshSelectorGroupBox.Text = "Mesh Selector";
            // 
            // affectAllMeshesCheckbox
            // 
            affectAllMeshesCheckbox.AutoSize = true;
            affectAllMeshesCheckbox.Location = new Point(398, 24);
            affectAllMeshesCheckbox.Name = "affectAllMeshesCheckbox";
            affectAllMeshesCheckbox.Size = new Size(118, 19);
            affectAllMeshesCheckbox.TabIndex = 2;
            affectAllMeshesCheckbox.Text = "Affect All Meshes";
            affectAllMeshesCheckbox.UseVisualStyleBackColor = true;
            // 
            // meshSelector
            // 
            meshSelector.FormattingEnabled = true;
            meshSelector.Location = new Point(6, 22);
            meshSelector.Name = "meshSelector";
            meshSelector.Size = new Size(384, 23);
            meshSelector.TabIndex = 0;
            // 
            // staticMeshCheckbox
            // 
            staticMeshCheckbox.AutoSize = true;
            staticMeshCheckbox.Location = new Point(140, 110);
            staticMeshCheckbox.Name = "staticMeshCheckbox";
            staticMeshCheckbox.Size = new Size(87, 19);
            staticMeshCheckbox.TabIndex = 4;
            staticMeshCheckbox.Text = "Static Mesh";
            staticMeshCheckbox.UseVisualStyleBackColor = true;
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
            // boneWeightsMessage
            // 
            boneWeightsMessage.AutoSize = true;
            boneWeightsMessage.ForeColor = Color.Red;
            boneWeightsMessage.Location = new Point(9, 172);
            boneWeightsMessage.Name = "boneWeightsMessage";
            boneWeightsMessage.Size = new Size(21, 15);
            boneWeightsMessage.TabIndex = 2;
            boneWeightsMessage.Text = "{0}";
            // 
            // ImporterOpenFileDialog
            // 
            Controls.Add(boneWeightsMessage);
            Controls.Add(importOptionsGroupBox);
            Name = "ImporterOpenFileDialog";
            importOptionsGroupBox.ResumeLayout(false);
            meshSelectorGroupBox.ResumeLayout(false);
            meshSelectorGroupBox.PerformLayout();
            mtdSelectorGroupBox.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.GroupBox importOptionsGroupBox;
        private System.Windows.Forms.GroupBox mtdSelectorGroupBox;
        private System.Windows.Forms.CheckBox staticMeshCheckbox;
        private System.Windows.Forms.CheckBox createDefaultBoneCheckbox;
        private System.Windows.Forms.GroupBox meshSelectorGroupBox;
        private EasyCompletionComboBox meshSelector;
        private EasyCompletionComboBox mtdSelector;
        private System.Windows.Forms.CheckBox affectAllMeshesCheckbox;
        private Label boneWeightsMessage;
    }
}
