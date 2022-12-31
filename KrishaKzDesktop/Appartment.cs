using System;
using System.Diagnostics;
using System.Windows.Input;

namespace KrishaKzDesktop;

public partial class KrishaParser
{
    public class Appartment
    {
        public string Uri { get; set; }
        public string PreviewImage { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }

        public ICommand OpenInBrowserCommand { get; set; }

        public Appartment()
        {
            OpenInBrowserCommand = new LambdaCommand(OpenInBrowser);
        }

        private void OpenInBrowser(object obj)
        {
            Process.Start("explorer.exe", Uri);
        }
    }
}
