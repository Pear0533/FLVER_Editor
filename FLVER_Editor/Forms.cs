using System.Drawing;
using System.Windows.Forms;

namespace FLVER_Editor
{
    /// <summary>
    /// A class that assists in form element creation for FLVER Editor
    /// </summary>
    public static class Forms
    {
        /// <summary>
        /// Shows an information dialog
        /// </summary>
        /// <param name="str">The message to show in the dialog</param>
        public static void ShowInformationDialog(string str)
        {
            System.Windows.Forms.MessageBox.Show(str, @"Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Shows an error dialog
        /// </summary>
        /// <param name="str">The message to show in the dialog</param>
        public static void ShowErrorDialog(string str)
        {
            System.Windows.Forms.MessageBox.Show(str, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Shows a question dialog
        /// </summary>
        /// <param name="str">The message to show in the dialog</param>
        /// <returns>True or false depending on whether or not the user pressed yes or no</returns>
        public static DialogResult ShowQuestionDialog(string str)
        {
            return System.Windows.Forms.MessageBox.Show(str, @"Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        /// <summary>
        /// Shows an input dialog
        /// </summary>
        /// <param name="text">The message to show in the dialog</param>
        /// <param name="caption">The caption to give this dialog</param>
        /// <returns>What the user inputted</returns>
        public static string ShowInputDialog(string text, string caption)
        {
            var prompt = new Form
            {
                Width = 240,
                Height = 125,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen,
                MaximizeBox = false
            };

            var textLabel = new Label { Left = 8, Top = 8, Width = 200, Text = text };
            var textBox = new TextBox { Left = 10, Top = 28, Width = 200 };

            var cancelButton = new Button { Text = @"Cancel", Left = 9, Width = 100, Top = 55, DialogResult = DialogResult.Cancel };
            cancelButton.Click += (sender, e) => { prompt.Close(); };
            var confirmation = new Button { Text = @"OK", Left = 112, Width = 100, Top = 55, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };

            prompt.Controls.Add(textBox);
            prompt.Controls.Add(cancelButton);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

        /// <summary>
        /// Makes a Label control
        /// </summary>
        /// <param name="size">The size of the control</param>
        /// <param name="location">The location of the control in X and Y</param>
        /// <param name="text">The text to display inside of the control</param>
        /// <returns>A Label control</returns>
        public static Label MakeLabel(Size size, Point location, string text)
        {
            return new Label
            {
                Size = size,
                Location = location,
                Text = text
            };
        }

        /// <summary>
        /// Makes a TextBox control
        /// </summary>
        /// <param name="size">The size of the control</param>
        /// <param name="location">The location of the control in X and Y</param>
        /// <param name="text">The text to display inside of the control</param>
        /// <returns>A TextBox control</returns>
        public static TextBox MakeTextBox(Size size, Point location, string text)
        {
            return new TextBox
            {
                Size = size,
                Location = location,
                Text = text
            };
        }

        /// <summary>
        /// Makes a CheckBox control
        /// </summary>
        /// <param name="size">The size of the control</param>
        /// <param name="location">The location of the control in X and Y</param>
        /// <param name="text">The text to display inside of the control</param>
        /// <returns>A CheckBox control</returns>
        public static CheckBox MakeCheckBox(Size size, Point location, string text)
        {
            return new CheckBox
            {
                Size = size,
                Location = location,
                Text = text
            };
        }
    }
}