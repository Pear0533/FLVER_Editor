using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ComboBox = System.Windows.Forms.ComboBox;

namespace FLVER_Editor;

/// <summary>
/// This is a combobox with a suggestion list à la "Sublime Text"
/// 
/// Searches are made against the pattern in the combo textbox by matching
/// all the characters in the pattern in the right order but not consecutively
/// </summary>
public class EasyCompletionComboBox : ComboBox
{
    #region fields and properties
    /// <summary>our custom drowp down control</summary>
    private readonly DropdownControl m_dropDown;
    /// <summary>the suggestion list inside the drop down control</summary>
    private readonly ListBox m_suggestionList;
    /// <summary>the bold font used for drawing strings in the listbox</summary>
    private Font m_boldFont;
    /// <summary>Allows to know if the last text change is triggered by the keyboard</summary>
    private bool m_fromKeyboard;
    /// <summary>How do we match user input to strings?</summary>
    private StringMatchingMethod m_matchingMethod;
    #endregion

    /// <summary>
    /// constructor
    /// </summary>
    public EasyCompletionComboBox()
    {
        m_matchingMethod = StringMatchingMethod.NoWildcards;
        // we're overriding these
        DropDownStyle = ComboBoxStyle.DropDown;
        AutoCompleteMode = AutoCompleteMode.None;
        // let's build our suggestion list
        m_suggestionList = new ListBox
        {
            DisplayMember = "Text",
            TabStop = false, 
            Dock = DockStyle.Fill,
            DrawMode = DrawMode.OwnerDrawFixed,
            IntegralHeight = true,
            Sorted = Sorted,
        };
        m_suggestionList.Click += onSuggestionListClick;
        m_suggestionList.DrawItem += onSuggestionListDrawItem;
        FontChanged += onFontChanged;
        m_suggestionList.MouseMove += onSuggestionListMouseMove;
        m_dropDown = new DropdownControl(m_suggestionList);
        onFontChanged(null, null);
    }

    /// <summary>
    /// <see cref="ComboBox.Dispose(bool)"/>
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (m_boldFont != null)
            {
                m_boldFont.Dispose();
            }
            m_dropDown.Dispose();
        }
        base.Dispose(disposing);
    }

    #region size and position of suggest box
    /// <summary>
    /// <see cref="ComboBox.OnLocationChanged(EventArgs)"/>
    /// </summary>
    protected override void OnLocationChanged(EventArgs e)
    {
        base.OnLocationChanged(e);
        hideDropDown();
    }

    /// <summary>
    /// <see cref="ComboBox.OnSizeChanged(EventArgs)"/>
    /// </summary>
    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        m_dropDown.Width = Width;
    }
    #endregion

    #region visibility of suggest box
    /// <summary>
    /// Shows the drop down.
    /// </summary>
    public void showDropDown()
    {
        if (DesignMode)
        {
            return;
        }
        // Hide the "standard" drop down if any
        if (base.DroppedDown)
        {
            BeginUpdate();
            // setting DroppedDown to false may select an item
            // so we save the editbox state
            string oText = Text;
            int selStart = SelectionStart;
            int selLen = SelectionLength;

            // close the "standard" dropdown
            base.DroppedDown = false;
                
            // and restore the contents of the editbox
            Text = oText;
            Select(selStart, selLen);
            EndUpdate();
        }
        // pop it up and resize it
        int h = Math.Min(MaxDropDownItems, m_suggestionList.Items.Count) * m_suggestionList.ItemHeight;
        m_dropDown.Show(this, new Size(DropDownWidth, h));
    }

    /// <summary>
    /// Hides the drop down.
    /// </summary>
    public void hideDropDown()
    {
        if (m_dropDown.Visible)
        {
            m_dropDown.Close();
        }
    }

    /// <summary>
    /// <see cref="ComboBox.OnLostFocus(EventArgs)"/>
    /// </summary>
    protected override void OnLostFocus(EventArgs e)
    {
        if (!m_dropDown.Focused && !m_suggestionList.Focused)
        {
            hideDropDown();
        }
        base.OnLostFocus(e);
    }

    /// <summary>
    /// <see cref="ComboBox.OnDropDown(EventArgs)"/>
    /// </summary>
    protected override void OnDropDown(EventArgs e)
    {
        hideDropDown();
        base.OnDropDown(e);
    }
    #endregion

    #region keystroke and mouse events
    /// <summary>
    /// Called when the user clicks on an item in the list
    /// </summary>
    private void onSuggestionListClick(object sender, EventArgs e)
    {
        m_fromKeyboard = false;
        StringMatch sel = (StringMatch)m_suggestionList.SelectedItem;
        Text = sel.Text;
        Select(0, Text.Length);
        Focus();
    }

    /// <summary>
    /// Process command keys
    /// </summary>
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if ((keyData == Keys.Tab) && (m_dropDown.Visible))
        {
            // we change the selection but will also allow the navigation to the next control
            if (m_suggestionList.Text.Length != 0)
            {
                Text = m_suggestionList.Text;
            }
            Select(0, Text.Length);
            hideDropDown();
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    /// <summary>
    /// if the dropdown is visible some keystrokes
    /// should behave in a custom way
    /// </summary>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        m_fromKeyboard = true;

        if (!m_dropDown.Visible)
        {
            base.OnKeyDown(e);
            return;
        }
        switch (e.KeyCode)
        {
            case Keys.Down:
                if (m_suggestionList.SelectedIndex < 0)
                {
                    m_suggestionList.SelectedIndex = 0;
                }
                else if (m_suggestionList.SelectedIndex < m_suggestionList.Items.Count - 1)
                {
                    m_suggestionList.SelectedIndex++;
                }
                break;
            case Keys.Up:
                if (m_suggestionList.SelectedIndex > 0)
                {
                    m_suggestionList.SelectedIndex--;
                }
                else if (m_suggestionList.SelectedIndex < 0)
                {
                    m_suggestionList.SelectedIndex = m_suggestionList.Items.Count - 1;
                }
                break;
            case Keys.Enter:
                if (m_suggestionList.Text.Length != 0)
                {
                    Text = m_suggestionList.Text;
                }
                Select(0, Text.Length);
                hideDropDown();
                break;
            case Keys.Escape:
                hideDropDown();
                break;
            default:
                base.OnKeyDown(e);
                return;
        }
        e.Handled = true;
        e.SuppressKeyPress = true;
    }

    /// <summary>
    /// We need to know if the last text changed event was due to one of the dropdowns 
    /// or to the keyboard
    /// </summary>
    /// <param name="e"></param>
    protected override void OnDropDownClosed(EventArgs e)
    {
        m_fromKeyboard = false;
        base.OnDropDownClosed(e);
    }

    /// <summary>
    /// this were we can make suggestions
    /// </summary>
    /// <param name="e"></param>
    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);


        if (!m_fromKeyboard || !Focused)
        {
            return;
        }

        m_suggestionList.BeginUpdate();
        m_suggestionList.Items.Clear();
        StringMatcher matcher = new StringMatcher(MatchingMethod, Text);
        foreach (object item in Items)
        {
            StringMatch sm = matcher.Match(GetItemText(item));
            if (sm != null)
            {
                m_suggestionList.Items.Add(sm);
            }
        }
        m_suggestionList.EndUpdate();

        bool visible = m_suggestionList.Items.Count != 0;

        if (m_suggestionList.Items.Count == 1 && ((StringMatch)m_suggestionList.Items[0]).Text.Length == Text.Trim().Length)
        {
            StringMatch sel = (StringMatch)m_suggestionList.Items[0];
            Text = sel.Text;
            Select(0, Text.Length);
            visible = false;
        }

        if (visible)
        {
            showDropDown();
        }
        else
        {
            hideDropDown();                
        }
            
        m_fromKeyboard = false;
    }

    /// <summary>
    /// We highlight the selection under the mouse in the suggestion listbox
    /// </summary>
    private void onSuggestionListMouseMove(object sender, MouseEventArgs e)
    {
        int idx = m_suggestionList.IndexFromPoint(e.Location);
        if ((idx >= 0) && (idx != m_suggestionList.SelectedIndex))
        {
            m_suggestionList.SelectedIndex = idx;
        }
    }
    #endregion

    #region owner drawn
    /// <summary>
    /// We keep track of system settings changes for the font
    /// </summary>
    private void onFontChanged(object sender, EventArgs e)
    {
        if (m_boldFont != null)
        {
            m_boldFont.Dispose();
        }
        m_suggestionList.Font = Font;
        m_boldFont = new Font(Font, FontStyle.Bold);
        m_suggestionList.ItemHeight = m_boldFont.Height + 2;
    }

    /// <summary>
    /// Draw a segment of a string and updates the bound rectangle for being used for the next segment drawing
    /// </summary>
    private static void DrawString(Graphics g, Color color, ref Rectangle rect, string text, Font font)
    {
        Size proposedSize = new Size(int.MaxValue, int.MaxValue);
        Size sz = TextRenderer.MeasureText(g, text, font, proposedSize, TextFormatFlags.NoPadding);
        TextRenderer.DrawText(g, text, font, rect, color, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
        rect.X += sz.Width;
        rect.Width -= sz.Width;
    }

    /// <summary>
    /// Draw an item in the suggestion listbox
    /// </summary>
    private void onSuggestionListDrawItem(object sender, DrawItemEventArgs e)
    {
        StringMatch sm = (StringMatch) m_suggestionList.Items[e.Index];

        e.DrawBackground();
            
        bool isBold = sm.StartsOnMatch;
        Rectangle rBounds = e.Bounds;

        foreach (string s in sm.Segments)
        {
            Font f = isBold ? m_boldFont : Font;
            DrawString(e.Graphics, e.ForeColor, ref rBounds, s, f);
            isBold = !isBold;
        }

        e.DrawFocusRectangle();
    }
    #endregion
        
    #region misc
    [Category("Behavior"), DefaultValue(false), Description("Specifies whether items in the list portion of the combobo are sorted.")]
    public new bool Sorted
    {
        get { return base.Sorted; }
        set
        {
            m_suggestionList.Sorted = value;
            base.Sorted = value;
        }
    }

    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new bool DroppedDown
    {
        get { return base.DroppedDown || m_dropDown.Visible; }
        set 
        { 
            m_dropDown.Visible = false;
            base.DroppedDown = value;
        }
    }
    #endregion

    #region New properties
    [
        DefaultValue(StringMatchingMethod.NoWildcards),
        Description("How strings are matched against the user input"),
        Browsable(true),
        EditorBrowsable(EditorBrowsableState.Always),
        Category("Behavior")
    ]
    public StringMatchingMethod MatchingMethod
    {
        get { return m_matchingMethod;  }
        set
        {
            if (m_matchingMethod != value)
            {
                m_matchingMethod = value;
                if (m_dropDown.Visible)
                {
                    // recalculate the matches
                    showDropDown();
                }
            }
        }
    }
    #endregion
        
    #region Hidden inherited properties
    /// <summary>This property is not relevant for this class.</summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
    public new AutoCompleteSource AutoCompleteSource
    {
        get { return base.AutoCompleteSource; }
        set { base.AutoCompleteSource = value; }
    }
    /// <summary>This property is not relevant for this class.</summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
    public new AutoCompleteStringCollection AutoCompleteCustomSource 
    {
        get { return base.AutoCompleteCustomSource; }
        set { base.AutoCompleteCustomSource = value; }
    }
    /// <summary>This property is not relevant for this class.</summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
    public new AutoCompleteMode AutoCompleteMode
    {
        get { return base.AutoCompleteMode; }
        set { base.AutoCompleteMode = value; }
    }
    /// <summary>This property is not relevant for this class.</summary>
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
    public new ComboBoxStyle DropDownStyle
    {
        get { return base.DropDownStyle; }
        set { base.DropDownStyle = value; }
    }
    #endregion
}