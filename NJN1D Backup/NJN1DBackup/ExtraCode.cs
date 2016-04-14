
namespace NJN1DBackup
{
    //if not used then delete it
    //public bool DeleteItembyID(string path)
    //{
    //    Uri _reqURL = new Uri(_Settings.DriveURLRoot + _Settings.Resource.strDrive_Items + "/" + path);
    //    HttpWebRequest _webreq = (HttpWebRequest)WebRequest.Create(_reqURL);
    //    _webreq.Method = _Settings.Method.DELETE;
    //    _webreq.Headers.Add(_Settings.strHeaderAuthorization, _Settings.Token.tokenType + " " + _Settings.Token.accessToken);
    //    HttpWebResponse GetResult = (HttpWebResponse)_webreq.GetResponse();
    //    if (GetResult.StatusCode != HttpStatusCode.NoContent)
    //    {
    //        _Settings.PrintError(GetResult.GetResponseStream());
    //        return false;
    //    }
    //    return true;
    //}

    //public static void PrintError(ErrorObj _errorObj)
    //{
    //    _errorObj.error.ToList<KeyValuePair<string, object>>().ForEach(i => Console.WriteLine(i.Key.ToString() + " " + i.Value.ToString()));
    //}

    // handle this error url with error code & error descriptionz
    //https://login.live.com/err.srf?lc=1033#error={error_code}&error_description={message}

    //String newurl = HttpUtility.UrlEncode(_requestURL.AbsoluteUri);
    //HttpWebRequest _webRequest2 = (HttpWebRequest)WebRequest.Create(newurl);
    //_requestURL = new Uri(newurl);
}
