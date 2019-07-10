using System;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using CefSharp.WinForms.Internals;

namespace MinimalCef
{
    public partial class BrowserForm : Form
    {
        public BrowserForm()
        {
            InitializeComponent();

            Text = "CefSharp";
            WindowState = FormWindowState.Maximized;

            var browser = new ChromiumWebBrowser("www.google.com")
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(browser);

            browser.IsBrowserInitializedChanged += OnIsBrowserInitializedChanged;

            var bitness = Environment.Is64BitProcess ? "x64" : "x86";
            var version = string.Format("Chromium: {0}, CEF: {1}, CefSharp: {2}, Environment: {3}", Cef.ChromiumVersion,
                Cef.CefVersion, Cef.CefSharpVersion, bitness);
        }

        private void OnIsBrowserInitializedChanged(object sender, IsBrowserInitializedChangedEventArgs e)
        {
            if (e.IsBrowserInitialized)
            {
                var b = (ChromiumWebBrowser) sender;

                this.InvokeOnUiThreadIfRequired(() => b.Focus());
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}