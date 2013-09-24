using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Forms;

namespace YalfPerver
{
    public partial class ConsoleOutput : UserControl
    {
        private readonly Subject<string> _pipe;
        private readonly IDisposable _outputDisposable;
        public ConsoleOutput()
        {
            InitializeComponent();

            _pipe = new Subject<string>();

            _outputDisposable = _pipe
                .Buffer(TimeSpan.FromMilliseconds(150))
                .Where(b => b.Count > 0)
                .ObserveOn(this)
                .Subscribe(WriteToOutput);
        }

        public void Write(string text)
        {
            _pipe.OnNext(text);
        }

        private void WriteToOutput(IList<string> lines)
        {
            var output = string.Concat(lines);
            tbConsole.AppendText(output);
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
            if (ParentForm != null)
                ParentForm.Closed += ParentFormOnClosed;
        }

        private void ParentFormOnClosed(object sender, EventArgs eventArgs)
        {
            _pipe.OnCompleted();
            _outputDisposable.Dispose();
            _pipe.Dispose();
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
