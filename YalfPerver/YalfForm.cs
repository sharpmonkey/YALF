using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Yalf.LogEntries;
using System.Linq;
using System.IO;
using Yalf.Reporting;
using Yalf.Reporting.Formatters;
using Yalf.Reporting.OutputHandlers;
using YalfPerver.Utilities;

namespace YalfPerver
{
    public partial class YalfForm : Form
    {
        private const String TIMESTAMP_FORMAT = "d/M/yy HH:mm:ss.fff";

        private enum OptionGlyph { open = 224, close = 223 }

        //private readonly LogPrinter _logPrinter = new LogPrinter();
        private FilterableLogEntryList _filteredEntries;
        private bool _selectAllNodes = false;
        // TODO: add save/load buttons for saving dumps and later analysis

        public YalfForm()
        {
            InitializeComponent();
        }

        private void btnDump_Click(object sender, EventArgs e)
        {
            this.InitialiseLogEntryDisplay(this.GetCurrentYalfEntries());
        }

        private void YalfForm_Load(object sender, EventArgs e)
        {
            this.lblFirstLogEntry.BorderStyle = BorderStyle.None;
            this.lblLastLogEntry.BorderStyle = BorderStyle.None;

            this.InitialiseLogEntryDisplay(this.GetCurrentYalfEntries());
        }

        private void btnClean_Click(object sender, EventArgs e)
        {
            Yalf.Log.Clear();
            this.InitialiseLogEntryDisplay(this.GetCurrentYalfEntries());
        }

        private void InitialiseLogEntryDisplay(BaseEntry[] entries)
        {
            this.LoadEntries(entries);
            this.BuildTreeView();
            this.ShowThreadCounts();
            this.lblFirstLogEntry.Text = _filteredEntries.FirstLogTime.ToString(TIMESTAMP_FORMAT);
            this.lblLastLogEntry.Text = _filteredEntries.LastLogTime.ToString(TIMESTAMP_FORMAT);

            this.RefreshDisplay();
        }

        private void ShowThreadCounts()
        {
            this.txtStatus.Text = string.Empty;

            StringBuilder output = new StringBuilder();

            this.BuildThreadCountOutput(ref output, _filteredEntries.GetEntries(), -1, 0);
            this.txtStatus.Text = output.ToString();
        }

        private void BuildThreadCountOutput(ref StringBuilder output, IEnumerable<BaseEntry> entries, int parentThreadId, int level)
        {
            foreach (ThreadData threadEntry in entries.OfType<ThreadData>().Select(entry => (entry as ThreadData)))
            {
                output.Append(' ', (level * 2));
                if (parentThreadId < 0)
                    output.AppendFormat("Entries on thread [{0:000}] = {1,8}", threadEntry.ThreadId, threadEntry.Entries.Length);
                else
                    output.AppendFormat("Entries on thread [{0:000}] = {1,8} (parent [{2}]) ", threadEntry.ThreadId, threadEntry.Entries.Length, parentThreadId);

                output.AppendLine();

                this.BuildThreadCountOutput(ref output, threadEntry.Entries, threadEntry.ThreadId, (level + 1));
            }
        }

        private BaseEntry[] GetCurrentYalfEntries()
        {
            this.Text = string.Format("Yalf dump on {0:HH:mm:ss}", DateTime.Now);
            return Yalf.Log.DumpInMemory();
        }

        private void LoadEntries(BaseEntry[] dump)
        {
            _filteredEntries = (_filteredEntries == null) ? new FilterableLogEntryList(dump, this.GetFiltersFromUiControls()) : _filteredEntries.CopyWithUpdatedLogEntries(dump);
        }

        private void DisplayLogEntries()
        {
            consoleOutput.Clear();

            var formatter = _filteredEntries.Filters.SingleLineFormat ? (ILogFormatter)new SingleLineFormatter() : new DefaultFormatter();

            try
            {
                var output = LogReporter.Report(_filteredEntries, new DefaultOutputHandler(_filteredEntries.Filters, formatter));
                consoleOutput.Write((output as DefaultOutputHandler).GetReport());
            }
            catch (Exception ex)
            {
                consoleOutput.Write(string.Concat("There was an error formatting the yalf logs - ", ex.ToString()));
            }
        }

        private void BuildTreeView()
        {
            var root = new List<TreeNode>();
            foreach (var kvp in _filteredEntries.GetItemCheckedStateList())
            {
                var tn = new TreeNode(kvp.Key);
                tn.Checked = kvp.Value;
                tn.Tag = kvp.Key;
                root.Add(tn);
            }

            tvFilter.Nodes.Clear();
            tvFilter.Nodes.AddRange(root.ToArray());

        }

        private void tvFilter_AfterCheck(object sender, TreeViewEventArgs e)
        {
            var node = e.Node;
            if (node == null)
                return;

            var key = node.Tag.ToString();

            _filteredEntries.SetSelectedState(key, node.Checked);

            if (!_refreshingTreeView)
                this.DisplayLogEntries();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_filteredEntries == null)
            {
                MessageBox.Show(@"Nothing to save, try dumping first :)", @"Save", MessageBoxButtons.OK);
                return;
            }

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var binData = Yalf.Log.DumpToBinary();
                File.WriteAllBytes(saveFileDialog.FileName, binData);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var filename = openFileDialog.FileName;
                var binData = File.ReadAllBytes(filename);
                var dump = Yalf.Log.DumpFromBinary(binData);
                this.InitialiseLogEntryDisplay(dump);

                this.Text = filename;
            }
        }

        private void tvFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                e.Handled = true;
                this.ToggleSelectState();
            }
        }

        private bool _refreshingTreeView = false;
        private void ToggleSelectState()
        {
            if (_refreshingTreeView) return;
            _refreshingTreeView = true;

            try
            {
                _selectAllNodes = !_selectAllNodes;
                this.tvFilter.SuspendLayout();

                foreach (TreeNode node in this.tvFilter.Nodes)
                {
                    node.Checked = _selectAllNodes;
                }

                this.DisplayLogEntries();
            }
            finally
            {
                _refreshingTreeView = false;
                this.tvFilter.ResumeLayout(true);
            }
        }

        private void SynchroniseTreeView()
        {
            if (_refreshingTreeView) return;
            _refreshingTreeView = true;
            try
            {
                var selectedValues = _filteredEntries.GetItemCheckedStateDictionary();

                foreach (TreeNode node in this.tvFilter.Nodes)
                {
                    bool selected = false;
                    if (selectedValues.ContainsKey((string)node.Tag))
                        selected = selectedValues[(string)node.Tag];

                    node.Checked = selected;
                }
            }
            finally
            {
                _refreshingTreeView = false;
            }
        }

        private void btnApplyFilters_Click(object sender, EventArgs e)
        {
            this.RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            this.ApplyCurrentFilters();
            this.SynchroniseTreeView();
            this.DisplayLogEntries();
        }

        private static IList<Tuple<string, int, string>> ValidateRegex(IList<string> expressions)
        {
            var validationErrors = new List<Tuple<string, int, string>>();

            for (int i = 0; i < expressions.Count; i++)
            {
                var expression = expressions[i];
                try
                {
                    var rg = new Regex(expression);
                }
                catch (Exception ex)
                {
                    validationErrors.Add(Tuple.Create(expression, i + 1, ex.Message));
                }
            }

            return validationErrors.AsReadOnly();
        }

        private void ApplyCurrentFilters()
        {
            var filters = this.GetFiltersFromUiControls();
            if (filters == null)
                return;

            _filteredEntries.Filters = filters;
            _filteredEntries.Refresh();
        }

        private ILogFilters GetFiltersFromUiControls()
        {
            var excluded = this.ExcludedKeyList.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var included = this.IncludedKeyList.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();

            var excludedValidation = ValidateRegex(excluded);
            var includedValidation = ValidateRegex(included);
            if (excludedValidation.Any() || includedValidation.Any())
            {
                var includedMessage = "";
                var excludedMessage = "";
                if (includedValidation.Any())
                    includedMessage = "Included:" + Environment.NewLine + string.Join(Environment.NewLine, includedValidation.Select(t => string.Format("[{0}] {1} => {2}", t.Item2, t.Item1, t.Item3)));

                if (excludedValidation.Any())
                    excludedMessage = "Excluded:" + Environment.NewLine + string.Join(Environment.NewLine, excludedValidation.Select(t => string.Format("[{0}] {1} => {2}", t.Item2, t.Item1, t.Item3)));

                MessageBox.Show(includedMessage + Environment.NewLine + excludedMessage, "Error compiling regular expression(s)", MessageBoxButtons.OK);
                return null;
            }

            var builder = new LogFiltersBuilder();

            builder.ExcludedKeysExpressionList = excluded;
            builder.IncludedKeysExpressionList = included;
            builder.HideEnterMethodLogs = this.chkHideEnterMethod.Checked;
            builder.HideExitMethodLogs = this.chkHideExitMethod.Checked;
            builder.HideMethodDuration = this.chkHideDuration.Checked;
            builder.HideMethodParameters = this.chkHodeMethodParameters.Checked;
            builder.HideTimeStampInMethod = this.chkHideTimestamp.Checked;
            builder.HideMethodReturnValue = this.chkHideMethodReturnValue.Checked;
            builder.SingleLineFormat = this.chkSingleLineFormat.Checked;
            builder.IgnoreCaseInFilter = this.chkIgnoreCase.Checked;
            builder.TimeStampFrom = (String.IsNullOrWhiteSpace(this.txtTimeStampFrom.Text)) ? DateTime.MinValue : DateTime.Parse(this.txtTimeStampFrom.Text);
            builder.TimeStampTo = (String.IsNullOrWhiteSpace(this.txtTimeStampTo.Text)) ? DateTime.MaxValue : DateTime.Parse(this.txtTimeStampTo.Text);

            return builder.Build();
        }

        private void RefreshScreenControlsFromPresenterFilters()
        {
            this.ExcludedKeyList.Text = string.Join(Environment.NewLine, _filteredEntries.Filters.ExcludedKeysExpressionList.ToArray());
            this.IncludedKeyList.Text = string.Join(Environment.NewLine, _filteredEntries.Filters.IncludedKeysExpressionList.ToArray());

            this.chkHideEnterMethod.Checked = _filteredEntries.Filters.HideEnterMethodLogs;
            this.chkHideExitMethod.Checked = _filteredEntries.Filters.HideExitMethodLogs;
            this.chkHideDuration.Checked = _filteredEntries.Filters.HideMethodDuration;
            this.chkHodeMethodParameters.Checked = _filteredEntries.Filters.HideMethodParameters;
            this.chkHideTimestamp.Checked = _filteredEntries.Filters.HideTimeStampInMethod;
            this.chkHideMethodReturnValue.Checked = _filteredEntries.Filters.HideMethodReturnValue;
            this.chkSingleLineFormat.Checked = _filteredEntries.Filters.SingleLineFormat;
            this.chkIgnoreCase.Checked = _filteredEntries.Filters.IgnoreCaseInFilter;

            this.txtTimeStampFrom.Text = (_filteredEntries.Filters.TimeStampFrom == DateTime.MinValue) ? "" : _filteredEntries.Filters.TimeStampFrom.ToString(TIMESTAMP_FORMAT);
            this.txtTimeStampTo.Text = (_filteredEntries.Filters.TimeStampTo == DateTime.MaxValue) ? "" : _filteredEntries.Filters.TimeStampTo.ToString(TIMESTAMP_FORMAT);
        }


        private void YalfForm_KeyUp(object sender, KeyEventArgs e)
        {
            this.ProcessShortCutKey(e);
        }

        private void ProcessShortCutKey(KeyEventArgs keyEventArgs)
        {
            keyEventArgs.Handled = true;

            switch (keyEventArgs.KeyCode)
            {
                case Keys.F5:
                    this.RefreshDisplay();
                    break;

                case Keys.F12:
                    this.OutputCsvFile();
                    break;

                default:
                    keyEventArgs.Handled = false;
                    break;
            }
        }

        private void chkSingleLineFormat_CheckedChanged(object sender, EventArgs e)
        {
            this.chkHideEnterMethod.Enabled = !this.chkSingleLineFormat.Checked;
            this.chkHideExitMethod.Enabled = !this.chkSingleLineFormat.Checked;
        }

        private void btnCsvDump_Click(object sender, EventArgs e)
        {
            this.OutputCsvFile();
        }

        private void OutputCsvFile()
        {
            String outputPath = this.txtCsvFolder.Text;

            if (!Directory.Exists(outputPath))
            {
                MessageBox.Show(String.Format("Output folder '{0}' does not exist, you need to manually create this first.", outputPath));
                return;
            }

            this.ApplyCurrentFilters();

            outputPath = Path.Combine(outputPath, String.Concat("YalfDump_", this.txtLogContext.Text, ".csv"));

            ILogOutputHandler outputHandler = new CsvFileOutputHandler(_filteredEntries.Filters, new DelimitedValuesFormatter(this.txtLogContext.Text, ","), outputPath);
            LogReporter.Report(_filteredEntries, outputHandler);

            this.txtStatus.Text = String.Concat(this.txtStatus.Text, Environment.NewLine, "Yalf dump output to '", outputPath, "'.", Environment.NewLine);
            this.txtStatus.SelectionStart = this.txtStatus.Text.Length;

            MessageBox.Show("All done");
        }

        private void btnLoadFilters_Click(object sender, EventArgs e)
        {
            this.LoadFilters();
        }

        private void LoadFilters()
        {
            if (this.openSettingsFileDialog.ShowDialog(this) != DialogResult.OK)
                return;

            _filteredEntries.Filters = LogFilterFileHandler.Load(this.openSettingsFileDialog.FileName);
            this.RefreshScreenControlsFromPresenterFilters();
        }

        private void btnSaveFilters_Click(object sender, EventArgs e)
        {
            this.SaveFilters();
        }

        private void SaveFilters()
        {
            if (this.saveSettingsFileDialog.ShowDialog(this) != DialogResult.OK)
                return;

            var filters = this.GetFiltersFromUiControls();
            LogFilterFileHandler.Save(this.saveSettingsFileDialog.FileName, filters);
        }
    }


    //Treat a double click event as two single click events
    class MyTreeView : TreeView
    {
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0203)
            {
                m.Msg = 0x0201;
            }
            base.WndProc(ref m);
        }
    }
}
