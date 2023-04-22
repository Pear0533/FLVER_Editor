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
        /// Creates a input dialog box to be used
        /// </summary>
        /// <param name="input">A string to be displayed in the input dialog box</param>
        /// <returns></returns>
        public static DialogResult ShowInputDialog(ref string input)
        {
            var size = new Size(200, 70);
            var inputBox = new Form
            {
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ClientSize = size,
                Text = "Name"
            };

            var textBox = new TextBox
            {
                Size = new Size(size.Width - 10, 23),
                Location = new Point(5, 5),
                Text = input
            };
            inputBox.Controls.Add(textBox);

            var okButton = new Button
            {
                DialogResult = DialogResult.OK,
                Name = "okButton",
                Size = new Size(75, 23),
                Text = "&OK",
                Location = new Point(size.Width - 80 - 80, 39)
            };
            inputBox.Controls.Add(okButton);

            var cancelButton = new Button
            {
                DialogResult = DialogResult.Cancel,
                Name = "cancelButton",
                Size = new Size(75, 23),
                Text = "&Cancel",
                Location = new Point(size.Width - 80, 39)
            };
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;
            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
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