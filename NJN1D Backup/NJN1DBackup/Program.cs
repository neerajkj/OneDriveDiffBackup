using System;
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using System.Runtime.Serialization.Json;
//using Microsoft.OneDrive.Sdk;
//using Microsoft.OneDrive;
//using Microsoft.OneDrive.Sdk.WindowsForms;
namespace NJN1DBackup
{
    class Program
    {
        

        private static async void auth()
        {
           // var oneDriveClient = OneDriveClient.GetMicrosoftAccountClient(Util.Settings.ClientID, Util.OneDriveItemResources.redirectURL, Util.Settings.Scopes.Split(' '), webAuthenticationUi: new FormsWebAuthenticationUi());
            //oneDriveClient2 = OneDriveClient.GetAuthenticatedMicrosoftAccountClient("","",null,IAuthenticationProvider)
            //AccountSession x =  await oneDriveClient.AuthenticateAsync();

            //var oneDriveClient = OneDriveClientExtensions.GetClientUsingOnlineIdAuthenticator(scopes);

            //await oneDriveClient.AuthenticateAsync();
           // Console.WriteLine(x.AccessToken);
        }

        [STAThread]
        static void Main(string[] args)
        {
            //CredentialCache.DefaultCredentials = n
            //auth();


            //return;
            if ((args.Length < 2) || (args.Length > 4))
            {
                Console.WriteLine("Invalid Parameters.");
                Console.WriteLine("FileDifferentialBackup.exe source_dir destination_dir [-silent] [-quick]");
                return;
            }

            String src = args[0];
            String dstn = args[1];


            if (!Directory.Exists(src))
            {
                Console.WriteLine("Invalid Directory : " + src);
                return;
            }

            if (dstn.StartsWith("/")) Util.Settings.isCloud = true;

            if (!Util.Settings.isCloud)
            {
                if (!Directory.Exists(dstn))
                {
                    Console.WriteLine("Invalid Directory : " + dstn);
                    return;
                }
            }

            for (int i = 2; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case Util.Resources.strArgQuick:
                        Util.Settings.thoroughBackup = false;
                        break;
                    case Util.Resources.strArgSilent:
                        Util.Settings.silentBackup = true;
                        break;
                    default:
                        Console.WriteLine("Invalid Argument. Correct argument is \n FileDifferentialBackup.exe source_dir destination_dir [-silent] [-quick]");
                        return;
                }
            }

            dstn = dstn.TrimEnd('/');
            DifferentialBackup d = new DifferentialBackup(src, dstn);
            if (Util.Settings.isCloud) Initialize();
            if (!d.Analyze()) return;
            if (!Util.Settings.silentBackup)
            {
                Console.WriteLine("Press 'y' to execute the backup.\nPress any other key to exit :");
                if (Console.ReadLine().ToLower() == "y")
                {
                    if (Util.Settings.isCloud)
                        d.ExecuteBackup1D();
                    else
                        d.ExecuteBackup();
                }
            }
            else
            {
                if (Util.Settings.isCloud)
                    d.ExecuteBackup1D();
                else
                    d.ExecuteBackup();
            }
            Console.WriteLine("Press Enter to Exit:");
            Console.ReadLine();
            //if (_Settings.isCloud) logout();
        }


        static void Initialize()
        {
            try
            {
                SettingsJSON settingobj = Util.LoadSettings();
                if (settingobj != null)
                {
                    if ((DateTime.UtcNow - settingobj.TokenFetchTime).TotalSeconds < Convert.ToInt32(settingobj.token.expiresIn))
                    {
                        Util.Settings.Token = settingobj.token;
                        return;
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Setting File is Corrupt. \n" + e.Message + e.InnerException);
            }

            Uri OAuthUri = new Uri(Util.OneDriveItemResources.OAuthURLRoot).AddQuery(Util.Resources.strClient_id, Util.Settings.ClientID).AddQuery(Util.Resources.strScope, Util.Settings.Scopes).AddQuery(Util.Resources.strRedirect_URI, Util.OneDriveItemResources.redirectURL).AddQuery(Util.Resources.strResponseType, Util.Resources.strCode);
            OAuthFetch fetch = new OAuthFetch(OAuthUri);
            Signin signin = new Signin(fetch);
            signin.TopLevel = true;
            signin.ShowDialog();

            if (fetch.isError)
            {
                Util.PrintError<String>(null, "Error Fetching User Credentials. \n" + fetch.errorString);
            }
            
            if (fetch.OAuthResonseUri == null)
            {
                Util.PrintError<String>(null, "Error Fetching User Credentials. \n");
            }
            if (HttpUtility.ParseQueryString(fetch.OAuthResonseUri.Query).Get(Util.Resources.strCode) == null)
            {
                Util.PrintError<String>(null, "Error Fetching User Credentials. \n" + fetch.OAuthResonseUri);
            }
            String AuthCode = HttpUtility.ParseQueryString(fetch.OAuthResonseUri.Query).Get(Util.Resources.strCode);
            Uri dummyURI = new Uri(Util.OneDriveItemResources.TokenURLRoot).AddQuery(Util.Resources.strClient_id, Util.Settings.ClientID).AddQuery(Util.Resources.strRedirect_URI, Util.OneDriveItemResources.redirectURL).AddQuery(Util.Resources.strClient_Secret, Util.Settings.ClientSecretv1).AddQuery(Util.Resources.strCode, AuthCode).AddQuery(Util.Resources.strGrantType, Util.Resources.strAuthorizationCode);
            Uri TokenUri = new Uri(Util.OneDriveItemResources.TokenURLRoot);
            HttpWebRequest TokenRequest = (HttpWebRequest)WebRequest.Create(TokenUri);
            TokenRequest.ContentType = Util.HTTPContentTypes.App_XUrlEncoded;
            TokenRequest.Method = Util.Method.POST;
            string postContent = dummyURI.Query.Substring(1);
            byte[] postContnt = Encoding.UTF8.GetBytes(postContent);
            TokenRequest.ContentLength = postContnt.Length;
            using (Stream dataStream = TokenRequest.GetRequestStream())
            {
                dataStream.Write(postContnt, 0, postContnt.Length);
            }
            HttpWebResponse TokenResponse = null;
            
            try
            {
                TokenResponse = (HttpWebResponse)TokenRequest.GetResponse();
                if (TokenResponse.StatusCode == HttpStatusCode.OK)
                {
                    DataContractJsonSerializer objdata = new DataContractJsonSerializer(typeof(Token));
                    Util.Settings.Token = (Token)objdata.ReadObject(TokenResponse.GetResponseStream());
                    Util.SaveSettings(DateTime.UtcNow);
                }
                else
                {
                    Util.PrintError<String>(TokenResponse, "Error Fetching Token at Initialization");
                }
            }
            catch (WebException e)
            {
                Util.PrintError<String>((HttpWebResponse)e.Response, "Error in Fetching Token");
            }
            finally
            {
                if (TokenResponse != null) TokenResponse.Close();
            }
            //put calling using httpwebrequest & response under 1 method each for get/post/delete/put etc.
        }
    }

    public class OAuthFetch
    {
        public OAuthFetch(Uri OAuthUri)
        {
            this.OAuthUri = OAuthUri;
        }
        public Uri OAuthResonseUri { get; set; }
        public Uri OAuthUri { get; set; }
        public bool isError { get; set; }
        public String errorString { get; set; }
    }
}
