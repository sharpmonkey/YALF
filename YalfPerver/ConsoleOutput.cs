using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace YalfPerver
{
    public partial class ConsoleOutput : UserControl
    {
        public ConsoleOutput()
        {
            InitializeComponent();
        }

        public void Write(string text)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action(() => Write(text)));
                return;
            }

            WriteToOutput(text);
        }

        private void WriteToOutput(string text)
        {
            tbConsole.AppendText(text);
            ScrollToCaret();

            _currentFindCharacter = 0;
        }

        internal void Clear()
        {
            tbConsole.Clear();
        }

        private void btnScrollToCaret_Click(object sender, EventArgs e)
        {
            ScrollToCaret();
        }

        private void ScrollToCaret()
        {
            if (btnScrollToCaret.Checked)
                tbConsole.ScrollToCaret();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void ConsoleOutput_Load(object sender, EventArgs e)
        {
        }

        private int _currentFindCharacter = 0;
        private String _currentFindText = "";
        private void btnFind_Click(object sender, EventArgs e)
        {
            this.FindText();
        }

        private void FindText()
        {
            int charPos = this.tbConsole.Text.IndexOf(_currentFindText, _currentFindCharacter, StringComparison.InvariantCultureIgnoreCase);
            if (charPos < 0)
            {
                _currentFindCharacter = 0;
                MessageBox.Show("End of text, click Find again to start search from top");
                return;
            }

            _currentFindCharacter = charPos + _currentFindText.Length;
            this.tbConsole.Select(charPos, _currentFindText.Length);
        }

        private void txtFindText_TextChanged(object sender, EventArgs e)
        {
            _currentFindCharacter = 0;
            _currentFindText = this.txtFindText.Text;
        }
    }
}
