using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace NJN1DBackup
{
    public partial class Signin : Form
    {
        OAuthFetch _fetch;
        public Signin(OAuthFetch fetch)
        {
            InitializeComponent();
            //wbSignin.Navigate(fetch.OAuthUri);
            _fetch = fetch;
            wbSignin.Url = _fetch.OAuthUri;
            _fetch.isError = false;
        }

        private void wbSignin_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {

             
        }

        private void wbSignin_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {   
            if (e.Url.AbsoluteUri.Contains(Util.OneDriveItemResources.ErrorURL))
            {
                _fetch.isError = true;
                _fetch.errorString = e.Url.Query.Substring(1);
                this.Close();
            }
            else if (e.Url.AbsoluteUri.Contains(Util.OneDriveItemResources.redirectURL))
            {
                _fetch.OAuthResonseUri = e.Url;
                _fetch.isError = false;
                _fetch.errorString = null;
                this.Close();
            }
        }
    }
}
