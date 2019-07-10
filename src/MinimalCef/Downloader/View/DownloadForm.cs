using System;
using System.Windows.Forms;
using MinimalCef.Downloader.Services;
using MinimalCef.Models;

namespace MinimalCef.Downloader.View
{
    public partial class DownloadForm : Form
    {
        private readonly Registry _reg;

        public DownloadForm(Registry reg)
        {
            _reg = reg;
            InitializeComponent();
        }


        private void Log(string message)
        {
            LogBox.AppendText($"{message}{Environment.NewLine}");
        }


        private void Progress(int value)
        {
         
            progressBar.Value = value;
        }

        private async void DownloadForm_Load(object sender, EventArgs e)
        {
            progressBar.Maximum = 100;
            progressBar.Minimum = 0;

         
            PackageName.Text = _reg.CefSharpPackageName;
            CefSharpPath.Text = _reg.GlobalCefSharpEnvPath;
            var cefSharpEnvBuilder = new CefSharpEnvBuilder(_reg, Log, Progress);
            await cefSharpEnvBuilder.Do();
            Close();
        }
    }
}