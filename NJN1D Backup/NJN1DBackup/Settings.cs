using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace NJN1DBackup
{
    public static class Util
    {
        public static class Constants
        {
            public const String settingsFileName = "settings.json";
            public const int MAX_TRIES = 5;
            public const String exclusionFileName = "exclusion.list";
        }

        public static class Resources
        {
            public const String strClient_id = "client_id";
            public const String strClient_Secret = "client_secret";
            public const String strScope = "scope";
            public const String strResponseType = "response_type";
            public const String strCode = "code";
            public const String strGrantType = "grant_type";
            public const String strAuthorizationCode = "authorization_code";
            public const String strAccessToken = "access_token";
            public const String strRefreshToken = "refresh_token";
            public const String strUserId = "user_id";
            public const String strTokenType = "token_type";
            public const String strExpiresIn = "expires_in";
            public const String strRedirect_URI = "redirect_uri";
            public const String strNameConflictBehavior = "@name.conflictBehavior";
            public const String strFolder = "folder";
            public const String strName = "name";
            public const String strItem = "item";
            public const String strFetchChildrenParameter = ":/children?select=id,name,size,lastModifiedDateTime,file,folder";
            public const String strFetchFolderObj = "?select=id,name,size,childcount,lastModifiedDateTime";
            public const String strSize = "size";
            public const String strArgSilent = "-silent";
            public const String strArgQuick = "-quick";
        }

        public class Settings
        {
            public const String ClientID = "0000000044149410";
            public const String ClientSecretv1 = "qvPGGy1kmnCzjt6IqNGF4t6sIEsji7cS";
            public const String Scopes = "wl.signin wl.offline_access onedrive.readwrite";

            public static Token Token;
            public static bool isCloud = false;
            public static long uploadSize;

            public static bool silentBackup = false;
            public static bool thoroughBackup = true;
            /// <summary>
            /// 100MB is max which 1D allows. Reduce it if connection is slow and/or patchy
            /// </summary>
            public static long MaxUploadFileSize = 80 * 1024 * 1024;
            /// <summary>
            /// Max Chunk Size is 60MB. OneDrive will throw an error above this.
            /// </summary>
            public static long LargeFileChunkSize = 10 * 1024 * 1024;
            public static long FileStreamBuffer = 409600;

            public static ConsoleColor TempBackgroundColor;
            public static ConsoleColor TempForegroundColor;
        }
        
        
        
        public static Uri AddQuery(this Uri uri, string name, string value)
        {
            UriBuilder ub = new UriBuilder(uri);
            // decodes urlencoded pairs from uri.Query to HttpValueCollection
            System.Collections.Specialized.NameValueCollection httpValueCollection = HttpUtility.ParseQueryString(uri.Query);
            httpValueCollection.Add(name, value);
            // urlencodes the whole HttpValueCollection
            ub.Query = httpValueCollection.ToString();
            return ub.Uri;
            
        }

        public static class Method
        {
            public const String GET = "GET";
            public const String POST = "POST";
            public const String PUT = "PUT";
            public const String DELETE = "DELETE";
        }

        public static class Header
        {
            public const String Authorization = "Authorization";
            public const String ContentLength = "Content-Length";
            public const String ContentRange = "Content-Range";
        }

        public static class OneDriveItemResources
        {
            public const String strDrive_Root = "/drive/root";     
            public const String strDrive_Items = "/drive/items";
            public const String DriveURLRoot = "https://api.onedrive.com/v1.0";
            public const String OAuthURLRoot = "https://login.live.com/oauth20_authorize.srf";
            public const String TokenURLRoot = "https://login.live.com/oauth20_token.srf";
            public const String redirectURL = "https://login.live.com/oauth20_desktop.srf";
            public const String ErrorURL = "https://login.live.com/err.srf";
            public const String LogoutURL = "https://login.live.com/oauth20_logout.srf";
        }

        public static class HTTPContentTypes
        {
            public const String App_XUrlEncoded = "application/x-www-form-urlencoded";
            public const String AppJson = "application/json";
        }

        public static class NameConflictBehavior
        {
            public const String strFail = "fail";
            public const String strRename = "rename";
            public const String strReplace = "replace";
        }


        public static String GetSizeStr(long _size)
        {
            if (_size < 1024)
                return _size.ToString() + " B";
            else if (_size < (1024 * 1024))
                return Math.Round(((decimal)_size / 1024),2).ToString() + " KB";
            else if (_size < (1024 * 1024 * 1024))
                return Math.Round(((decimal)_size / 1024 / 1024), 2).ToString() + " MB";
            else
                return Math.Round(((decimal)_size / 1024 / 1024 / 1024), 2).ToString() + " GB";
        }

        public static void PrintError<T>(HttpWebResponse _webResponse, string optionalMessage = null, bool exitProgram = true)
        {
            DataContractJsonSerializer jsonObject = new DataContractJsonSerializer(typeof(T));
            ErrorObj1D errorObj = null;
            String errorObjectString = null;
            String statusCode = null;
            Stream _stream = null;

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Red;
            if (optionalMessage != null)
            {
                Console.WriteLine(optionalMessage);
            }
            if (_webResponse != null)
            {
                statusCode = ((int)_webResponse.StatusCode).ToString();
                
                if (statusCode != null)
                {
                    Console.WriteLine("Http Response Status Code: " + statusCode);
                }

                try
                {
                    _stream = _webResponse.GetResponseStream();
                    if (_stream != null)
                    {
                        if (typeof(T) == typeof(ErrorObj1D))
                        {
                            errorObj = (ErrorObj1D)jsonObject.ReadObject(_stream);
                            Console.WriteLine("Error Code: " + errorObj.error.code);
                            Console.WriteLine("Error Message: " + errorObj.error.message);
                            if (errorObj.error.innnererror != null)
                            {
                                Console.WriteLine("Inner Error Message: " + errorObj.error.innnererror.code);
                            }
                        }
                        else if (typeof(T) == typeof(String))
                        {
                            errorObjectString = (String)jsonObject.ReadObject(_stream);
                            Console.WriteLine(errorObjectString);
                        }
                    }
                }
                catch (SerializationException ex)
                {
                    _stream.Position = 0;
                    Console.WriteLine((new StreamReader(_stream)).ReadToEnd());
                    Console.WriteLine(ex.StackTrace);
                }
            }
            Console.ForegroundColor = ConsoleColor.Black;
            if (exitProgram)
            {
                Console.BackgroundColor = Util.Settings.TempBackgroundColor;
                Console.ForegroundColor = Util.Settings.TempForegroundColor;
                Console.WriteLine("Press enter to exit:");
                Console.ReadLine();
                Environment.Exit(1);
            }
        }

        public static SettingsJSON LoadSettings()
        {
            SettingsJSON obj = null;
            try
            {
                if (File.Exists(Util.Constants.settingsFileName))
                    obj = (new JavaScriptSerializer()).Deserialize<SettingsJSON>(File.ReadAllText(Util.Constants.settingsFileName));
            }
            catch(Exception e)
            {
                Util.PrintError<String>(null, e.Message, false);
            }
            return obj;
        }

        public static void SaveSettings(DateTime _TokenFetchTime)
        {
            SettingsJSON obj = new SettingsJSON();
            obj.token = Util.Settings.Token;
            obj.TokenFetchTime = _TokenFetchTime;
            //obj.FileChunkSizeinMB = (int)_Settings.LargeFileChunkSize/1024/1024;
            //obj.maxFileSizeinMB = (int)_Settings.LargeFileChunkSize/1024/1024;
            try
            {
                File.WriteAllText(Util.Constants.settingsFileName, (new JavaScriptSerializer()).Serialize(obj));
            }
            catch (Exception e)
            {
                Util.PrintError<String>(null, e.Message, false);
            }
        }

        public static long GetDirectorySize(String dir)
        {
            long size = 0;
            string[] fileArray = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);

            foreach (string item in fileArray)
            {
                size += (new FileInfo(item)).Length;
            }
            return size;
        }

        public static void logout()
        {
            Uri LogoutUri = new Uri(Util.OneDriveItemResources.LogoutURL).AddQuery(Util.Resources.strClient_id, Util.Settings.ClientID).AddQuery(Util.Resources.strRedirect_URI, Util.OneDriveItemResources.redirectURL);
            HttpWebRequest LogoutRequest = (HttpWebRequest)WebRequest.Create(LogoutUri);
            LogoutRequest.Method = Util.Method.GET;
            HttpWebResponse LogoutResponse = null;
            try
            {
                LogoutResponse = (HttpWebResponse)LogoutRequest.GetResponse();
            }
            catch (Exception e)
            {
                Util.PrintError<String>(null, e.Message + e.StackTrace, false);
            }
            finally
            {
                if (LogoutResponse != null) LogoutResponse.Close();
            }
            Console.WriteLine("Successfully Logged Out. \nPress Enter to Exit");
            Console.ReadLine();
            Environment.Exit(0);
        }

        /// <summary>
        /// Get new Access or Refresh token. Call it after expiration or just before it.
        /// </summary>
        public static void GetNewAccessToken()
        {
            Uri dummyURI = new Uri(Util.OneDriveItemResources.TokenURLRoot).AddQuery(Util.Resources.strClient_id, Util.Settings.ClientID).AddQuery(Util.Resources.strRedirect_URI, Util.OneDriveItemResources.redirectURL).AddQuery(Util.Resources.strClient_Secret, Util.Settings.ClientSecretv1).AddQuery(Util.Resources.strRefreshToken, Util.Settings.Token.refreshToken).AddQuery(Util.Resources.strGrantType, Util.Resources.strRefreshToken);
            Uri TokenUri = new Uri(Util.OneDriveItemResources.TokenURLRoot);
            HttpWebRequest TokenRequest = (HttpWebRequest)WebRequest.Create(TokenUri);
            TokenRequest.ContentType = Util.HTTPContentTypes.App_XUrlEncoded;
            TokenRequest.Method = Util.Method.POST;
            byte[] postContnt = Encoding.UTF8.GetBytes(dummyURI.Query.Substring(1));
            TokenRequest.ContentLength = postContnt.Length;
            using (Stream dataStream = TokenRequest.GetRequestStream())
            {
                dataStream.Write(postContnt, 0, postContnt.Length);
            }
            HttpWebResponse TokenResponse = (HttpWebResponse)TokenRequest.GetResponse();
            Dictionary<string, object> dict = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(new StreamReader(TokenResponse.GetResponseStream()).ReadToEnd());
            Util.Settings.Token.refreshToken = dict[Util.Resources.strRefreshToken].ToString();
            Util.Settings.Token.accessToken = dict[Util.Resources.strAccessToken].ToString();
            Util.SaveSettings(DateTime.UtcNow);
        }
    }
}
