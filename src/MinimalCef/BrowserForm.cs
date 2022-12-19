using System;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using CefSharp.WinForms.Internals;

namespace MinimalCef
{
    public partial class BrowserForm : Form
    {

       //public static string url = "https://www.whatismybrowser.com/detect/what-is-my-user-agent/";
       //public static string url = "https://membed.net/streaming.php?id=MjkxODQy&title=Bob%27s+Burgers+-+Season+10+Episode+5+-+Legends+of+the+Mall&typesub=SUB&sub=L2JvYnMtYnVyZ2Vycy1zZWFzb24tMTAtZXBpc29kZS01LWxlZ2VuZHMtb2YtdGhlLW1hbGwvYm9icy1idXJnZXJzLXNlYXNvbi0xMC1lcGlzb2RlLTUtbGVnZW5kcy1vZi10aGUtbWFsbC52dHQ=&cover=Y292ZXIvYm9icy1idXJnZXJzLXNlYXNvbi0xMC5wbmc=";
        //public static string url = "https://github.com/antoinevastel/fpscanner";
       // public static string url = "https://bot.sannysoft.com/";
        public static string url = "https://seriesyonkis.nu/episode/star-wars-andor-1x9/";
        //public static string url = "https://membed.net/streaming.php?id=MjkxODQy";
        private ChromiumWebBrowser _browser;

        public BrowserForm()
        {
            InitializeComponent();

            Text = "CefSharp";
            WindowState = FormWindowState.Maximized;

            _browser = new ChromiumWebBrowser(url)
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(_browser);

            _browser.IsBrowserInitializedChanged += OnIsBrowserInitializedChanged;

            var bitness = Environment.Is64BitProcess ? "x64" : "x86";
            var version = string.Format("Chromium: {0}, CEF: {1}, CefSharp: {2}, Environment: {3}", Cef.ChromiumVersion,
                Cef.CefVersion, Cef.CefSharpVersion, bitness);
           
        }

        private void OnIsBrowserInitializedChanged(object sender, EventArgs e)
        {
            //if (e.IsBrowserInitialized)
            //{
            //    var b = (ChromiumWebBrowser)sender;

            //    this.InvokeOnUiThreadIfRequired(() => b.Focus());
            //}

        }

      

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _browser.ShowDevTools();
           // _browser.LoadUrl("https://bot.sannysoft.com/");
            ///_browser.LoadUrl("https://membed.net/streaming.php?id=MjkxODQy&title=Bob%27s+Burgers+-+Season+10+Episode+5+-+Legends+of+the+Mall&typesub=SUB&sub=L2JvYnMtYnVyZ2Vycy1zZWFzb24tMTAtZXBpc29kZS01LWxlZ2VuZHMtb2YtdGhlLW");
        }
    }
}