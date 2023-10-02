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
            weightingModeGroupBox = new GroupBox();
            weightingModeSelector = new ComboBox();
            createDefaultBoneCheckbox = new CheckBox();
            staticMeshCheckbox = new CheckBox();
            affectAllMeshesCheckbox = new CheckBox();
            meshSelector = new EasyCompletionComboBox();
            mtdSelectorGroupBox = new GroupBox();
            mtdSelector = new EasyCompletionComboBox();
            boneWeightsMessage = new Label();
            importOptionsGroupBox.SuspendLayout();
            meshSelectorGroupBox.SuspendLayout();
            weightingModeGroupBox.SuspendLayout();
            mtdSelectorGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // importOptionsGroupBox
            // 
            importOptionsGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            importOptionsGroupBox.Controls.Add(meshSelectorGroupBox);
            importOptionsGroupBox.Location = new Point(9, 6);
            importOptionsGroupBox.Name = "importOptionsGroupBox";
            importOptionsGroupBox.Size = new Size(540, 194);
            importOptionsGroupBox.TabIndex = 1;
            importOptionsGroupBox.TabStop = false;
            importOptionsGroupBox.Text = "Import Options";
            // 
            // meshSelectorGroupBox
            // 
            meshSelectorGroupBox.Controls.Add(weightingModeGroupBox);
            meshSelectorGroupBox.Controls.Add(createDefaultBoneCheckbox);
            meshSelectorGroupBox.Controls.Add(staticMeshCheckbox);
            meshSelectorGroupBox.Controls.Add(affectAllMeshesCheckbox);
            meshSelectorGroupBox.Controls.Add(meshSelector);
            meshSelectorGroupBox.Controls.Add(mtdSelectorGroupBox);
            meshSelectorGroupBox.Location = new Point(6, 21);
            meshSelectorGroupBox.Name = "meshSelectorGroupBox";
            meshSelectorGroupBox.Size = new Size(528, 167);
            meshSelectorGroupBox.TabIndex = 5;
            meshSelectorGroupBox.TabStop = false;
            meshSelectorGroupBox.Text = "Mesh Selector";
            // 
            // weightingModeGroupBox
            // 
            weightingModeGroupBox.Controls.Add(weightingModeSelector);
            weightingModeGroupBox.Location = new Point(6, 106);
            weightingModeGroupBox.Name = "weightingModeGroupBox";
            weightingModeGroupBox.Size = new Size(200, 54);
            weightingModeGroupBox.TabIndex = 4;
            weightingModeGroupBox.TabStop = false;
            weightingModeGroupBox.Text = "Weighting Mode";
            // 
            // weightingModeSelector
            // 
            weightingModeSelector.FormattingEnabled = true;
            weightingModeSelector.Location = new Point(6, 22);
            weightingModeSelector.Name = "weightingModeSelector";
            weightingModeSelector.Size = new Size(188, 23);
            weightingModeSelector.TabIndex = 3;
            // 
            // createDefaultBoneCheckbox
            // 
            createDefaultBoneCheckbox.AutoSize = true;
            createDefaultBoneCheckbox.Location = new Point(218, 126);
            createDefaultBoneCheckbox.Name = "createDefaultBoneCheckbox";
            createDefaultBoneCheckbox.Size = new Size(131, 19);
            createDefaultBoneCheckbox.TabIndex = 0;
            createDefaultBoneCheckbox.Text = "Create Default Bone";
            createDefaultBoneCheckbox.UseVisualStyleBackColor = true;
            // 
            // staticMeshCheckbox
            // 
            staticMeshCheckbox.AutoSize = true;
            staticMeshCheckbox.Location = new Point(351, 126);
            staticMeshCheckbox.Name = "staticMeshCheckbox";
            staticMeshCheckbox.Size = new Size(87, 19);
            staticMeshCheckbox.TabIndex = 4;
            staticMeshCheckbox.Text = "Static Mesh";
            staticMeshCheckbox.UseVisualStyleBackColor = true;
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
            // boneWeightsMessage
            // 
            boneWeightsMessage.AutoSize = true;
            boneWeightsMessage.ForeColor = Color.Red;
            boneWeightsMessage.Location = new Point(9, 202);
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
            weightingModeGroupBox.ResumeLayout(false);
            mtdSelectorGroupBox.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private GroupBox importOptionsGroupBox;
        private GroupBox mtdSelectorGroupBox;
        private CheckBox staticMeshCheckbox;
        private CheckBox createDefaultBoneCheckbox;
        private GroupBox meshSelectorGroupBox;
        private EasyCompletionComboBox meshSelector;
        private EasyCompletionComboBox mtdSelector;
        private CheckBox affectAllMeshesCheckbox;
        private Label boneWeightsMessage;
        private GroupBox weightingModeGroupBox;
        private ComboBox weightingModeSelector;
    }
}
