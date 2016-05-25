using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Json;
using System.Diagnostics;
using System.Threading;
using System.Security.Cryptography;


// max file size for upload in settings
//go and see if each and every variable is used or not

//make a combine app which does both

//http errors - 401 refresh token
//insufficient storage - halt program
//server error - retry
//status error - increase timeout till 2000 seconds then decrease chunksize and maxsmallfile upload size
//other status error - retry 

//1 action should not affect other action
//log file : 1 general purpose log file : api calls timestamp
//estimated time remaining

//exclusion list
//unauthorized error - refresh token object
//https://msdn.microsoft.com/en-us/library/system.net.webexceptionstatus%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396

////see if this code works notfound : DeleteItem() Method , all switch cases check
namespace NJN1DBackup
{
    public class DifferentialBackup
    {

        private ItemObj GetObjDetails(String path)
        {
       
            HttpWebResponse _webResponse = null;
            ItemObj _dirData = null;

            Uri _requestURL = new Uri(Util.OneDriveItemResources.DriveURLRoot + Util.OneDriveItemResources.strDrive_Root + ":" + HttpUtility.UrlEncode(path) + Util.Resources.strFetchFolderObj);
            HttpWebRequest _webRequest = (HttpWebRequest)WebRequest.Create(_requestURL);
            Debug.Print("GetFolderDetails: " + path);

            _webRequest.Headers.Add(Util.Header.Authorization, Util.Settings.Token.tokenType + " " + Util.Settings.Token.accessToken);

            try
            {
                _webResponse = (HttpWebResponse)_webRequest.GetResponse();
                if (_webResponse.StatusCode == HttpStatusCode.OK)
                {
                    DataContractJsonSerializer _jsonSerializer = new DataContractJsonSerializer(typeof(ItemObj));
                    _dirData = (ItemObj)_jsonSerializer.ReadObject(_webResponse.GetResponseStream());
                }
                else
                {
                    Util.PrintError<String>(_webResponse, "Fetching Directory: " + path);
                }
            }
            catch (WebException e)
            {
                Util.PrintError<ErrorObj1D>((HttpWebResponse)e.Response, "Fetching Folder Properties: " + path);
            }
            finally
            {
                if (_webResponse != null) _webResponse.Close();
            }
            return _dirData;
        }
        private CreateItemObj CreateFolder(string _path)
        {
            DataContractJsonSerializer _jsonSerializer;
            CreateItemObj _returnObject = null;
            JavaScriptSerializer _jsSerializer = new JavaScriptSerializer();
            Dictionary<string, object> _dictObject = new Dictionary<string, object>();
            Dictionary<string, object> _folderDictObject = new Dictionary<string, object>();
            HttpWebResponse _webResponse = null;
            String id = "";
            String _folderName = _path.Substring(_path.LastIndexOf('/') + 1);
            //HttpUtility.UrlEncode(_path));
            String parentpath = _path.Substring(0,_path.LastIndexOf("/"));

            //foreach (var item in lstFolder)
            //{
            //    Debug.Print(item.folder);
            //}
            foreach (var item in lstFolder)
            {
                if (item.folder == parentpath)
                {
                    id = item.obj.id;
                    break;
                }
            }
            if (id == "")
            {
                ItemObj _obj = GetObjDetails(parentpath);
                id = _obj.id;
            }
            if (id == "")
            {
                Debug.Print("Parent Not Found: "+parentpath);
                return null;
            }
            Uri _requestURL = new Uri(Util.OneDriveItemResources.DriveURLRoot + Util.OneDriveItemResources.strDrive_Items + "/" + id + "/children");
            HttpWebRequest _webRequest = (HttpWebRequest)WebRequest.Create(_requestURL);
            Debug.Print("Create Folder: " + _path);
            _webRequest.Method = Util.Method.POST;
            _webRequest.ContentType = Util.HTTPContentTypes.AppJson;
            _webRequest.Headers.Add(Util.Header.Authorization, Util.Settings.Token.tokenType + " " + Util.Settings.Token.accessToken);

            
            _dictObject.Add(Util.Resources.strName, _folderName);     
            _dictObject.Add(Util.Resources.strFolder, _folderDictObject);
            _dictObject.Add(Util.Resources.strNameConflictBehavior, Util.NameConflictBehavior.strReplace);



            string _content = _jsSerializer.Serialize(_dictObject);
            byte[] _contentBytes = Encoding.UTF8.GetBytes(_content);
            _webRequest.ContentLength = _contentBytes.Length;
            using (Stream dataStream = _webRequest.GetRequestStream())
            {
                dataStream.Write(_contentBytes, 0, _contentBytes.Length);
            }

            try
            {
                _webResponse = (HttpWebResponse)_webRequest.GetResponse();
                if ((_webResponse.StatusCode == HttpStatusCode.Created)|| (_webResponse.StatusCode == HttpStatusCode.OK))
                {
                    _jsonSerializer = new DataContractJsonSerializer(typeof(CreateItemObj));
                    _returnObject = (CreateItemObj)_jsonSerializer.ReadObject(_webResponse.GetResponseStream());

                    if ((_folderName != _returnObject.name) || (_returnObject.folder == null))
                        Util.PrintError<String>(_webResponse, "Creating Directory: " + _path);
                }
                //else if (_webResponse.StatusCode == HttpStatusCode.OK)
                //{
                //    DataContractJsonSerializer jsonObject = new DataContractJsonSerializer(typeof(String));
                //    long i = _webResponse.ContentLength;
                //    var s = _webResponse.GetResponseStream();
                //    String data = (String)jsonObject.ReadObject(s);
                    
                //}
                else
                {
                    Util.PrintError<String>(_webResponse, "Creating Directory: " + _path);
                }
            }
            catch(WebException e)
            {
                Util.PrintError<ErrorObj1D>((HttpWebResponse)e.Response, "Creating Directory: " + _path);
            }
            finally
            {
                if (_webResponse != null) _webResponse.Close();
            }
            return _returnObject;
        }

        private void Retry<T1,T2>(Action<T1,T2> func)
        {
           // int attemptsleft = _Settings.MAX_TRIES;
            while (true)
            {
                //try
                //{
                //   // func(T1, T2);
                //}
                //catch(WebException e)
                //{
                //    //e.Response.
                //    //_Settings(e);
                //    attemptsleft--;
                //}
            }
        }

        private void RetrySmallFileUpload(string _srcFile, string _dstnFile)
        {
            //int attemptsleft = _Settings.MAX_TRIES;
            while (true)
            {
                //try
                //{
                //    UploadSmallFile(_srcFile, _dstnFile);
                //}
                //catch (WebException e)
                //{
                //    //e.sponse.
                //    //_Settings(e);
                //    attemptsleft--;
                //}
            }
        }

        private void RetryLargeFileUpload()
        {

        }

        private void DeleteUploadURL(String _uploadurl)
        {
            HttpWebResponse _deletewebResponse = null;
            HttpWebRequest _deleteuploadSession = (HttpWebRequest)WebRequest.Create(_uploadurl);
            _deleteuploadSession.Method = Util.Method.DELETE;
            _deleteuploadSession.Headers.Add(Util.Header.Authorization, Util.Settings.Token.tokenType + " " + Util.Settings.Token.accessToken);
            try
            {
                _deletewebResponse = (HttpWebResponse)_deleteuploadSession.GetResponse();
                if (_deletewebResponse.StatusCode != HttpStatusCode.NoContent)
                {
                    Util.PrintError<String>(_deletewebResponse, "Deleting Upload URL:  " + _uploadurl);
                }
            }
            catch (WebException e)
            {
                Util.PrintError<ErrorObj1D>((HttpWebResponse)e.Response, "Deleting Upload URL:  " + _uploadurl);
            }
            finally
            {
                if (_deletewebResponse != null) _deletewebResponse.Close();
            }
        }

        public void DeleteItem(string _path)
        {
            HttpWebResponse _webResponse = null;
            Uri _requestURL = new Uri(Util.OneDriveItemResources.DriveURLRoot + Util.OneDriveItemResources.strDrive_Root + ":" + HttpUtility.UrlEncode(_path));
            HttpWebRequest _webRequest = (HttpWebRequest)WebRequest.Create(_requestURL);
            Debug.Print("Delete Folder: " + _path);
            _webRequest.Method = Util.Method.DELETE;
            _webRequest.Headers.Add(Util.Header.Authorization, Util.Settings.Token.tokenType + " " + Util.Settings.Token.accessToken);

            try
            {
                _webResponse = (HttpWebResponse)_webRequest.GetResponse();
                switch (_webResponse.StatusCode)
                {
                    case HttpStatusCode.NoContent:
                        break;
                    case HttpStatusCode.NotFound:
                        Util.PrintError<String>(_webResponse, "Item to be deleted not found:  " + _path,false);
                        break;
                    default:
                        Util.PrintError<String>(_webResponse, "Error Deleting Item:  " + _path);
                        break;
                }
            }
            catch(WebException e)
            {
                Util.PrintError<ErrorObj1D>((HttpWebResponse)e.Response, "Deleting Item: " + _path );
            }
            finally
            {
                if (_webResponse != null) _webResponse.Close();
            }
        }

        public ItemObjArray GetDirectory(String _dirName)
        {
            HttpWebResponse _webResponse = null;
            ItemObjArray _dirData = null;

            Uri _requestURL = new Uri(Util.OneDriveItemResources.DriveURLRoot + Util.OneDriveItemResources.strDrive_Root + ":" + HttpUtility.UrlEncode(_dirName) + Util.Resources.strFetchChildrenParameter);
            HttpWebRequest _webRequest = (HttpWebRequest)WebRequest.Create(_requestURL);
            Debug.Print("GetData: " + _dirName);

            _webRequest.Headers.Add(Util.Header.Authorization, Util.Settings.Token.tokenType + " " + Util.Settings.Token.accessToken);
            
            try
            {
                _webResponse = (HttpWebResponse)_webRequest.GetResponse();
                if (_webResponse.StatusCode == HttpStatusCode.OK)
                {
                    DataContractJsonSerializer _jsonSerializer = new DataContractJsonSerializer(typeof(ItemObjArray));
                    _dirData = (ItemObjArray)_jsonSerializer.ReadObject(_webResponse.GetResponseStream());
                }
                else
                {
                    Util.PrintError<String>(_webResponse, "Fetching Directory: " + _dirName);
                }
            }
            catch (WebException e)
            {
                Util.PrintError<ErrorObj1D>((HttpWebResponse)e.Response, "Fetching Directory: " + _dirName);
            }
            finally
            {
                if (_webResponse != null) _webResponse.Close();
            }
            foreach (var item in _dirData.value)
            {
                if (item.folder == null) continue;
                FolderList f = new FolderList();
                f.folder = _dirName + "/"+ item.name;
                f.obj = item;
                lstFolder.Add(f);
            }
            if (_dirData.nextLink != null)
            {
                ItemObjArray _dirDatanextLink = GetDirectorynextLinkData(_dirData.nextLink, _dirName);
                foreach (var item in _dirDatanextLink.value)
                {
                    _dirData.value.Add(item);
                }
            }
            
            return _dirData;
        }

        public  ItemObjArray GetDirectorynextLinkData(String nextLink, String _dirName)
        {
            HttpWebResponse _webResponse = null;
            ItemObjArray _dirData = null;
            Uri _requestURL = new Uri(nextLink);
            HttpWebRequest _webRequest = (HttpWebRequest)WebRequest.Create(_requestURL);
            Debug.Print("Get More Data: " + _dirName);

            _webRequest.Headers.Add(Util.Header.Authorization, Util.Settings.Token.tokenType + " " + Util.Settings.Token.accessToken);

            try
            {
                _webResponse = (HttpWebResponse)_webRequest.GetResponse();
                if (_webResponse.StatusCode == HttpStatusCode.OK)
                {
                    DataContractJsonSerializer _jsonSerializer = new DataContractJsonSerializer(typeof(ItemObjArray));
                    _dirData = (ItemObjArray)_jsonSerializer.ReadObject(_webResponse.GetResponseStream());
                }
                else
                {
                    Util.PrintError<String>(_webResponse, "Fetching Directory: " + _dirName);
                }
            }
            catch (WebException e)
            {
                Util.PrintError<ErrorObj1D>((HttpWebResponse)e.Response, "Fetching Directory: " + _dirName);
            }
            finally
            {
                if (_webResponse != null) _webResponse.Close();
            }
            foreach (var item in _dirData.value)
            {
                if (item.folder == null) continue;
                FolderList f = new FolderList();
                f.folder = _dirName + "/" + item.name;
                f.obj = item;
                lstFolder.Add(f);
            }
            if (_dirData.nextLink != null)
            {
                ItemObjArray _dirDatanextLink = GetDirectorynextLinkData(_dirData.nextLink, _dirName);
                foreach (var item in _dirDatanextLink.value)
                {
                    _dirData.value.Add(item);
                }
            }
            return _dirData;
        }

        public string SourceDirectory { get; private set; }
        public string DestinationDirectory { get; private set; }
        List<Action> lstAction = new List<Action>();
        List<String> lstAllFiles = new List<String>();
        List<String> lstAllDirectories = new List<String>();
        static List<FolderList> lstFolder = new List<FolderList>();

        private struct FolderList
        {
            public string folder;
            public ItemObj obj;
        }

        public DifferentialBackup(string src, string dstn)
        {
            this.SourceDirectory = src;
            this.DestinationDirectory = dstn;
            Util.Settings.TempBackgroundColor = Console.BackgroundColor;
            Util.Settings.TempForegroundColor = Console.ForegroundColor;
        }
        public bool Analyze()
        {
            Console.WriteLine("Analyzing...");
            if (Util.Settings.isCloud)
                this.AnalyzeRecursive1D(SourceDirectory, DestinationDirectory);
            else
                this.AnalyzeRecursive(SourceDirectory, DestinationDirectory);
            this.PrintAnalysisResult();
            if (lstAction.Count > 0)
                return true;
            else
            {
                Console.WriteLine("Press Enter to Exit.");
                Console.ReadLine();
                return false;
            }
        }

        private void PrintAnalysisResult()
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            
            long deleteSize = 0;

            if (lstAction.Count == 0)
            {
                Console.WriteLine("Folders Matched !!!");
                Console.WriteLine("Source Dir Size : " + Util.GetSizeStr(Util.GetDirectorySize(this.SourceDirectory)));
                if (!Util.Settings.isCloud)
                    Console.WriteLine("Destination Dir Size : " + Util.GetSizeStr(Util.GetDirectorySize(this.DestinationDirectory)));

                Console.BackgroundColor = Util.Settings.TempBackgroundColor;
                Console.ForegroundColor = Util.Settings.TempForegroundColor;
                return;
            }
            this.lstAction = this.lstAction.OrderBy(o => o.ActiononFile).ToList();
            foreach (Action item in lstAction)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(Enum.GetName(typeof(ItemAction), item.ActiononFile) + " : ");
                Console.ForegroundColor = ConsoleColor.Black;
                //if ((item.ActiononFile == ItemAction.DELETEFILE) || (item.ActiononFile == ItemAction.DELETEDIRECTORY))
                //    Console.Write("Destination: " + item.DestinationPath);
                //else
                //    Console.Write("Source: " + item.SourcePath);

                switch (item.ActiononFile)
                {
                    case ItemAction.COPYFILE:
                        Util.Settings.uploadSize += new FileInfo(item.SourcePath).Length;
                        
                        Console.Write("Source: " + item.SourcePath);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("  Size: " + Util.GetSizeStr(new FileInfo(item.SourcePath).Length));
                        Console.ForegroundColor = ConsoleColor.Black;
                        break;
                    case ItemAction.DELETEFILE:
                        deleteSize += item.DestinationObjSize;
                        
                        Console.Write("Detination: " + item.DestinationPath);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("  Size: " + Util.GetSizeStr(item.DestinationObjSize));
                        Console.ForegroundColor = ConsoleColor.Black;
                        break;
                    case ItemAction.COPYDIRECTORY:
                        Util.Settings.uploadSize += Util.GetDirectorySize(item.SourcePath);
                        Console.Write("Source: " + item.SourcePath);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("  Size: " + Util.GetSizeStr(Util.GetDirectorySize(item.SourcePath)));
                        Console.ForegroundColor = ConsoleColor.Black;
                        break;
                    case ItemAction.DELETEDIRECTORY:
                        deleteSize += item.DestinationObjSize;
                        //Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("Detination: " + item.DestinationPath + "  Size: " + Util.GetSizeStr(item.DestinationObjSize));
                        break;
                    case ItemAction.OVERWRITE:
                        deleteSize += item.DestinationObjSize;
                        Util.Settings.uploadSize += new FileInfo(item.SourcePath).Length;
                        //Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("Detination: " + item.DestinationPath);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("  Old Size: " + Util.GetSizeStr(item.DestinationObjSize));
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine();
                        Console.Write("Source: " + item.SourcePath);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("  New Size: " + Util.GetSizeStr(new FileInfo(item.SourcePath).Length));
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine();
                        Console.Write("Overwrite Reason: " + item.OverWriteReason);
                        break;
                    default:
                        break;
                }

            }

            Console.WriteLine();
            Console.BackgroundColor = Util.Settings.TempBackgroundColor;
            Console.ForegroundColor = Util.Settings.TempForegroundColor;
            Console.WriteLine("Total data to be uploaded: " + Util.GetSizeStr(Util.Settings.uploadSize));
            Console.WriteLine("Total data to be deleted in skydrive: " + Util.GetSizeStr(deleteSize));
        }


        private void AnalyzeRecursive1D(string _src, string _dstn)
        {
            //put the loading of exclusion in Util and call it before this function is called
            HashSet<string> exclusionlist = new HashSet<string>();
            try
            {
                if (File.Exists(Util.Constants.exclusionFileName))
                    File.ReadAllLines(Util.Constants.exclusionFileName).ToList().ForEach(item => exclusionlist.Add(item));
            }
            finally { }

            DirectoryInfo dirSrc = new DirectoryInfo(_src);
            
            HashSet<string> hashSrcDir = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            HashSet<OneDriveFolderObj> hashDstn1DFolder = new HashSet<OneDriveFolderObj>();
            HashSet<string> hashDstnDir = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            HashSet<string> hashSrcFile = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            HashSet<OneDriveFileObj> hashDstn1DFile = new HashSet<OneDriveFileObj>();
            HashSet<string> hashDstnFile = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            dirSrc.GetFiles().ToList().ForEach(i => hashSrcFile.Add(i.Name));
            dirSrc.GetDirectories().ToList().ForEach(i => hashSrcDir.Add(i.Name));

            ItemObjArray dirInfo = this.GetDirectory(_dstn);

            foreach (var item in dirInfo.value)
            {
                if (item.file == null)
                    hashDstn1DFolder.Add(new OneDriveFolderObj(item.id, item.name, item.size, item.folder.childCount, item.lastModifiedDateTime));
                else
                    hashDstn1DFile.Add(new OneDriveFileObj(item.id, item.name, item.size, item.lastModifiedDateTime, item.file.hashes));
            }
            hashDstn1DFolder.ToList().ForEach(i => hashDstnDir.Add(i.Name));
            hashDstn1DFile.ToList().ForEach(i => hashDstnFile.Add(i.Name));

            hashSrcDir.Except(hashDstnDir, StringComparer.InvariantCultureIgnoreCase).ToList().ForEach(item => lstAction.Add(new Action(ItemAction.COPYDIRECTORY, Path.Combine(_src, item), _dstn + "/" + item)));
            hashDstnDir.Except(hashSrcDir, StringComparer.InvariantCultureIgnoreCase).ToList().ForEach(item => lstAction.Add(new Action(ItemAction.DELETEDIRECTORY, String.Empty, _dstn + "/" + item, "", hashDstn1DFolder.Single(i => i.Name == item).Size)));

            hashSrcFile.Except(hashDstnFile, StringComparer.InvariantCultureIgnoreCase).ToList().ForEach(item => lstAction.Add(new Action(ItemAction.COPYFILE, Path.Combine(_src, item), _dstn + "/" + item)));
            hashDstnFile.Except(hashSrcFile, StringComparer.InvariantCultureIgnoreCase).ToList().ForEach(item =>
            {
                lstAction.Add(new Action(ItemAction.DELETEFILE, String.Empty, _dstn + "/" + item,"", hashDstn1DFile.Single(i => i.Name == item).Size));
            }
            );

            hashSrcFile.Intersect(hashDstnFile, StringComparer.InvariantCultureIgnoreCase).ToList().ForEach(item =>
            {
                string reason;
                OneDriveFileObj fileobj = hashDstn1DFile.ToList().Single(i => i.Name.Equals(item, StringComparison.InvariantCultureIgnoreCase));
                if (!this.CompareFiles1D(Path.Combine(_src, item), fileobj, out reason))
                    lstAction.Add(new Action(ItemAction.OVERWRITE, Path.Combine(_src, item), _dstn + "/" + item, reason, hashDstn1DFile.Single(i => i.Name == item).Size));
            }
                );
            hashSrcDir.Intersect(hashDstnDir, StringComparer.InvariantCultureIgnoreCase).ToList().ForEach(item => this.AnalyzeRecursive1D(Path.Combine(_src, item), _dstn+"/"+item));
        }

        private void AnalyzeRecursive(string _src, string _dstn)
        {
            DirectoryInfo dirSrc = new DirectoryInfo(_src);
            DirectoryInfo dirDstn = new DirectoryInfo(_dstn);
            HashSet<String> hashSrcDir = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            HashSet<String> hashDstnDir = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            HashSet<String> hashSrcFile = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            HashSet<String> hashDstnFile = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            dirSrc.GetDirectories().ToList().ForEach(i => hashSrcDir.Add(i.Name));
            dirDstn.GetDirectories().ToList().ForEach(i => hashDstnDir.Add(i.Name));
            //get the directory list
            // make sure that file name matches are case insensitive
            dirSrc.GetFiles().ToList().ForEach(i => hashSrcFile.Add(i.Name));
            dirDstn.GetFiles().ToList().ForEach(i => hashDstnFile.Add(i.Name));

            hashSrcDir.Except(hashDstnDir).ToList().ForEach(item => lstAction.Add(new Action(ItemAction.COPYDIRECTORY, Path.Combine(_src, item), Path.Combine(_dstn, item))));
            hashDstnDir.Except(hashSrcDir).ToList().ForEach(item => lstAction.Add(new Action(ItemAction.DELETEDIRECTORY, String.Empty, Path.Combine(_dstn, item))));

            hashSrcFile.Except(hashDstnFile).ToList().ForEach(item => lstAction.Add(new Action(ItemAction.COPYFILE, Path.Combine(_src, item), Path.Combine(_dstn, item))));
            hashDstnFile.Except(hashSrcFile).ToList().ForEach(item => lstAction.Add(new Action(ItemAction.DELETEFILE, String.Empty, Path.Combine(_dstn, item))));

            hashSrcFile.Intersect(hashDstnFile).ToList().ForEach(item =>
            {
                string reason;
                if (!this.ComapareFiles(Path.Combine(_src, item), Path.Combine(_dstn, item), out reason))
                    lstAction.Add(new Action(ItemAction.OVERWRITE, Path.Combine(_src, item), Path.Combine(_dstn, item), reason));
            }
                );
            hashSrcDir.Intersect(hashDstnDir).ToList().ForEach(item => this.AnalyzeRecursive(Path.Combine(_src, item), Path.Combine(_dstn, item)));
        }

        private bool ComapareFiles(string _srcFile, string _dstnFile, out string reason)
        {
            FileInfo src = new FileInfo(_srcFile);
            FileInfo dstn = new FileInfo(_dstnFile);
            if (src.Length != dstn.Length)
            {
                reason = "Length Mismatch";
                return false;
            }
            if (src.LastWriteTime.CompareTo(dstn.LastWriteTime) != 0)
            {
                reason = "Last Modified Date Mismatch";
                return false;
            }
            reason = "";
            return true;
        }

        private bool CompareFiles1D(string _srcFile, OneDriveFileObj _dstnFile, out string reason)
{
            FileInfo src = new FileInfo(_srcFile);
            if (src.Length != _dstnFile.Size)
            {
                //onedrive bug for small png files less than 256 bytes in size
                if ((src.Extension != ".png") || (src.Length > 256))
                {
                    reason = "Length Mismatch";
                    return false;
                }
            }

            if (!Util.Settings.thoroughBackup)
            {
                reason = "";
                return true;
            }
            String _fileSHAHash = "";
            
            SHA1 _sha = SHA1.Create();
            FileStream _fs = null;
            try
            {
                _fs = new FileStream(_srcFile, FileMode.Open,FileAccess.Read);
                //abc = new FileStream(,FileMode.)
                foreach (var i in _sha.ComputeHash(_fs))
                {
                    _fileSHAHash += i.ToString("x2").ToUpper();
                }
                if (!_fileSHAHash.Equals(_dstnFile.SHA1Hash, StringComparison.CurrentCultureIgnoreCase))
                {
                    reason = "File Data Mismatch";
                    return false;
                }
            }
            finally
            {
                if (_fs != null) _fs.Close();
            }
            
            reason = "";
            return true;
        }

        public void ExecuteBackup()
        {
            DateTime init = DateTime.Now;
            Util.Settings.TempBackgroundColor = Console.BackgroundColor;
            Util.Settings.TempForegroundColor = Console.ForegroundColor;
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Backup Started..." + DateTime.Now.ToString());
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            foreach (Action item in lstAction)
            {
                try
                {
                    switch (item.ActiononFile)
                    {
                        case ItemAction.COPYFILE:
                            try
                            {
                                File.Copy(item.SourcePath, item.DestinationPath, true);
                                Console.WriteLine("File Copied : " + item.SourcePath);
                            }
                            catch (UnauthorizedAccessException ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(ex.Message);
                                Console.WriteLine(ex.StackTrace);
                                Console.ForegroundColor = ConsoleColor.Black;
                            }
                            break;
                        case ItemAction.DELETEFILE:
                            try
                            {
                                File.SetAttributes(item.DestinationPath, FileAttributes.Normal);
                                //File.Delete(item.SourcePath);
                                Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(item.DestinationPath, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                                Console.WriteLine("File Deleted : " + item.DestinationPath);
                            }
                            catch (UnauthorizedAccessException ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(ex.Message);
                                Console.WriteLine(ex.StackTrace);
                                Console.ForegroundColor = ConsoleColor.Black;
                            }
                            break;
                        case ItemAction.COPYDIRECTORY:
                            Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(item.SourcePath, item.DestinationPath, true);
                            Console.WriteLine("Directory Copied : " + item.SourcePath + " | " + Util.GetSizeStr(Util.GetDirectorySize(item.SourcePath)));
                            break;
                        case ItemAction.DELETEDIRECTORY:
                            DeleteDirectory(item);
                            break;
                        case ItemAction.OVERWRITE:
                            try
                            {
                                File.SetAttributes(item.DestinationPath, FileAttributes.Normal);
                                File.Copy(item.SourcePath, item.DestinationPath, true);
                                Console.WriteLine("File Overwritten : " + item.SourcePath + " | " + item.OverWriteReason);
                            }
                            catch (UnauthorizedAccessException ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(ex.Message);
                                Console.WriteLine(ex.StackTrace);
                                Console.ForegroundColor = ConsoleColor.Black;
                            }
                            break;
                        default:
                            break;
                    }
                }
                catch (DirectoryNotFoundException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                catch (FileNotFoundException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                catch (IOException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    Console.ForegroundColor = ConsoleColor.Black;
                }
            }
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Backup Finished !!! Phew...." + DateTime.Now.ToString());
            Console.BackgroundColor = Util.Settings.TempBackgroundColor;
            Console.ForegroundColor = Util.Settings.TempForegroundColor;

        }

        private void DeleteDirectory(Action item)
        {

            try
            {
                //Directory.Delete(item.SourcePath, true);
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(item.DestinationPath, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                Console.WriteLine("Directory Deleted : " + item.DestinationPath);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("UnAuthorizedAccessException occurred");
                this.TryDeleteDirectory(item);
            }
            catch (IOException)
            {
                Console.WriteLine("IOException occurred");
                this.TryDeleteDirectory(item);
            }
        }

        private void TryDeleteDirectory(Action item)
        {
            try
            {
                foreach (string s in Directory.GetFiles(item.DestinationPath, "*.*", SearchOption.AllDirectories))
                {
                    File.SetAttributes(s, FileAttributes.Normal);
                    Console.WriteLine("Attributes Normalized : " + s);
                }
                foreach (string s in Directory.GetDirectories(item.SourcePath, "*.*", SearchOption.AllDirectories))
                {
                    new DirectoryInfo(s) { Attributes = FileAttributes.Normal };
                    Console.WriteLine("Attributes Normalized : " + s);
                }

                new DirectoryInfo(item.SourcePath) { Attributes = FileAttributes.Normal };
                //Directory.Delete(item.SourcePath, true);
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(item.SourcePath, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                Console.WriteLine("Directory Deleted : " + item.SourcePath);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.GetType().ToString());
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("Delete Directory Manually :" + item.SourcePath);
            }

        }

        

        private void UploadSmallFile(string _srcFile, string _dstnFile)
        {
            FileInfo _srcFileInfo = new FileInfo(_srcFile);
            HttpWebResponse _webResponse = null;
            Uri _requestURL = new Uri(Util.OneDriveItemResources.DriveURLRoot + Util.OneDriveItemResources.strDrive_Root + ":" + HttpUtility.UrlEncode(_dstnFile) + ":/content");
            HttpWebRequest _webRequest = (HttpWebRequest)WebRequest.Create(_requestURL);
            _webRequest.Headers.Add(Util.Header.Authorization, Util.Settings.Token.tokenType + " " + Util.Settings.Token.accessToken);
            _webRequest.Method = Util.Method.PUT;
            _webRequest.Timeout = 1000000;
            _webRequest.ContentLength = _srcFileInfo.Length;
            Debug.Print("Upload: "+ _dstnFile);
            using (FileStream _fileStream = new FileStream(_srcFile, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[Util.Settings.FileStreamBuffer];
                int bytesRead = 0;
                Stream ds = _webRequest.GetRequestStream();
                int progress = 1;
                long totalBytesRead = 0;
                while ((bytesRead = _fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    ds.Write(buffer, 0, bytesRead);
                    totalBytesRead += bytesRead;
                    if (_fileStream.Length > 1024*1024)
                    {
                        if (((decimal)totalBytesRead / (decimal)_fileStream.Length) > (decimal)(progress * 0.1))
                        {
                            Debug.Print("File Upload Progress " + (((decimal)totalBytesRead / (decimal)_fileStream.Length * 100)).ToString("0.00") + "% " + Util.GetSizeStr(totalBytesRead) );
                            progress++;
                            Thread.Sleep(50);
                        }
                    }
                }
            }
            try
            {
                _webResponse = (HttpWebResponse)_webRequest.GetResponse();
                //var sr = new StreamReader(_webResponse.GetResponseStream()).ReadToEnd();
                if ((_webResponse.StatusCode == HttpStatusCode.Created)||(_webResponse.StatusCode == HttpStatusCode.OK))
                {
                    DataContractJsonSerializer _jsonSerializer = new DataContractJsonSerializer(typeof(CreateItemObj));
                    CreateItemObj _fileDataObject = (CreateItemObj)_jsonSerializer.ReadObject(_webResponse.GetResponseStream());
                    if ((_fileDataObject.size != _srcFileInfo.Length.ToString()) || (_fileDataObject.name != _srcFileInfo.Name) || (_fileDataObject.id == null))
                    {
                        Util.PrintError<String>(null, "Error Encountered after Successful Upload. \nUploading File: " + _srcFile + " @ " + _dstnFile + "\nHttpStatus Code : " + _webResponse.StatusCode.ToString() + "\n Return FileObject Details:\n" + _fileDataObject.id + "\n" + _fileDataObject.name + "\n" + Util.GetSizeStr(Convert.ToInt64(_fileDataObject.size)),false);
                    }
                }
                else
                {
                    Util.PrintError<String>(_webResponse, "Uploading File: " + _srcFile + " @ " + _dstnFile);
                }
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.Timeout)
                {

                }
                Util.PrintError<ErrorObj1D>((HttpWebResponse)e.Response, "Uploading File: " + _srcFile + " @ " + _dstnFile + "\n" + e.Message + e.StackTrace);
            }
            catch (Exception e)
            {

                Util.PrintError<String>(null, e.Message + e.StackTrace);
            }
            finally
            {
                if (_webResponse != null) _webResponse.Close();
            }
        }

        private void UploadLargeFile(string _srcFile,string _dstnFile)
        {
            FileInfo _srcFileInfo = new FileInfo(_srcFile);
            HttpWebResponse _webResponse = null;
            Uri _uploadurl = null;
            Uri _reqURL = new Uri(Util.OneDriveItemResources.DriveURLRoot + Util.OneDriveItemResources.strDrive_Root + ":" + HttpUtility.UrlEncode(_dstnFile) + ":/upload.createSession");
            HttpWebRequest _webreq = (HttpWebRequest)WebRequest.Create(_reqURL);
            
            _webreq.Method = Util.Method.POST;
            Debug.Print(_webreq.Method + " : " +  _dstnFile);
            _webreq.ContentType = Util.HTTPContentTypes.AppJson;
            _webreq.Headers.Add(Util.Header.Authorization, Util.Settings.Token.tokenType + " " + Util.Settings.Token.accessToken);

            Dictionary<string, object> _itemObject = new Dictionary<string, object>();
            JavaScriptSerializer _jsSerializer = new JavaScriptSerializer();
            Dictionary<string, object> _dictObject = new Dictionary<string, object>();
            _itemObject.Add(Util.Resources.strName, _dstnFile.Substring(_dstnFile.LastIndexOf('/') + 1));
            _itemObject.Add(Util.Resources.strNameConflictBehavior, Util.NameConflictBehavior.strReplace);
            _dictObject.Add(Util.Resources.strItem, _itemObject);

            string _content = _jsSerializer.Serialize(_dictObject);
            byte[] _contentBytes = Encoding.UTF8.GetBytes(_content);

            using (Stream dataStream = _webreq.GetRequestStream())
            {
                dataStream.Write(_contentBytes, 0, _contentBytes.Length);
            }

            try
            {
                _webResponse = (HttpWebResponse)_webreq.GetResponse();
                if (_webResponse.StatusCode == HttpStatusCode.OK)
                {
                    DataContractJsonSerializer json2 = new DataContractJsonSerializer(typeof(UploadSessionResultObject));
                    UploadSessionResultObject obj = (UploadSessionResultObject)json2.ReadObject(_webResponse.GetResponseStream());
                    string range = obj.nextExpectedRanges[0];
                    String[] arr_range = range.Split('-');
                    if (Convert.ToInt64(arr_range[0]) != 0)
                    {
                        DeleteUploadURL(obj.uploadUrl);
                        UploadFile1D(_srcFile, _dstnFile);
                        return;
                    }
                    _uploadurl = new Uri(obj.uploadUrl);
                }
                else
                {
                    Util.PrintError<String>(_webResponse, "Uploading File: " + _srcFile + " @ " + _dstnFile);
                }
            }
            catch (WebException e)
            {
                Util.PrintError<ErrorObj1D>((HttpWebResponse)e.Response, "Uploading Multipart File: " + _srcFile + " @ " + _dstnFile);
            }
            catch (Exception e)
            {
                Util.PrintError<String>(null, e.Message + e.StackTrace);
            }
            finally
            {
                if (_webResponse != null) _webResponse.Close();
            }

            Debug.Print("UploadURL: " + _uploadurl.AbsoluteUri);
            long startCounter = 0;
            long endCounter = Math.Min(Util.Settings.LargeFileChunkSize, _srcFileInfo.Length - 1);
            while (true)
            {
                long contentlength = Math.Min(Util.Settings.LargeFileChunkSize, endCounter - startCounter + 1);

                HttpWebRequest _webuploadreq = (HttpWebRequest)WebRequest.Create(_uploadurl);
                _webuploadreq.Headers.Add(Util.Header.Authorization, Util.Settings.Token.tokenType + " " + Util.Settings.Token.accessToken);
                _webuploadreq.ContentLength = contentlength;
                _webuploadreq.Headers.Add(Util.Header.ContentRange, "bytes " + startCounter.ToString() + "-" + (startCounter + contentlength - 1).ToString() + "/" + _srcFileInfo.Length.ToString());
                _webuploadreq.Method = Util.Method.PUT;
                _webuploadreq.Timeout = 1000000;

                using (FileStream _fs = new FileStream(_srcFile, FileMode.Open, FileAccess.Read))
                {
                    byte[] buffer = new byte[Util.Settings.FileStreamBuffer];
                    int bytesRead = 0;
                    long totalBytesRead = 0;
                    Stream ds = _webuploadreq.GetRequestStream();
                    _fs.Position = startCounter;
                    int progress = 1;
                    while ((bytesRead = _fs.Read(buffer, 0, (int)Math.Min(buffer.Length, (contentlength - totalBytesRead)))) != 0)
                    {
                        ds.Write(buffer, 0, bytesRead);
                        totalBytesRead += bytesRead;
                        if (((decimal)totalBytesRead / (decimal)_fs.Length) > (decimal)(progress * 0.1))
                        {
                            Debug.Print("File Upload Progress " + (((decimal)(totalBytesRead+startCounter) / (decimal)_fs.Length * 100)).ToString("0.00") + "% " + Util.GetSizeStr(totalBytesRead+startCounter));
                            progress++;
                        }
                    }
                }

                try
                {
                    HttpWebResponse _webuploadresponse = (HttpWebResponse)_webuploadreq.GetResponse();
                    if (_webuploadresponse.StatusCode == HttpStatusCode.Accepted)
                    {

                        DataContractJsonSerializer json2 = new DataContractJsonSerializer(typeof(UploadSessionResultObject));
                        UploadSessionResultObject obj = (UploadSessionResultObject)json2.ReadObject(_webuploadresponse.GetResponseStream());
                        string range = obj.nextExpectedRanges[0];
                        String[] arr_range = range.Split('-');
                        startCounter = Convert.ToInt64(arr_range[0]);
                        endCounter = Convert.ToInt64(arr_range[1]);
                        Debug.Print("Chunk Accepted." + Util.GetSizeStr(startCounter) + "uploaded.  " + _dstnFile);
                        
                    }
                    else if (_webuploadresponse.StatusCode == HttpStatusCode.Created)
                    {
                        DataContractJsonSerializer json;
                        json = new DataContractJsonSerializer(typeof(CreateItemObj));
                        CreateItemObj FolderData = (CreateItemObj)json.ReadObject(_webuploadresponse.GetResponseStream());
                        if ((FolderData.size == _srcFileInfo.Length.ToString()) && (FolderData.name == _srcFileInfo.Name) && (FolderData.id != null))
                            break;
                    }
                    else
                    {
                        Util.PrintError<String>(_webuploadresponse, "Uploading File: " + _srcFile + " @ " + _dstnFile);
                    }
                }
                catch (WebException e)
                {
                    Util.PrintError<ErrorObj1D>((HttpWebResponse)e.Response, "Uploading Multipart File: " + _srcFile + " @ " + _dstnFile + "bytes sent: " + startCounter.ToString());
                }
                finally
                {
                    //do something here
                }
            }
        }

        private void UploadFile1D(string _srcFile, string _dstnFile)
        {
            if ((new FileInfo(_srcFile)).Length < Util.Settings.MaxUploadFileSize)
                UploadSmallFile(_srcFile,_dstnFile);  
            else
                UploadLargeFile(_srcFile, _dstnFile);
        }

        
        private void CopyDirectory1D(String _src, String _dstn)
        {
            DirectoryInfo _srcObject = new DirectoryInfo(_src);
            List<String> duplicateFolders = new List<string>();

            lstAllDirectories.Clear();
            lstAllFiles.Clear();

            lstAllDirectories.Add(_src);
            GetFileFoldersList(_srcObject);

            foreach (var dir in lstAllDirectories)
            {
                foreach (var file in lstAllFiles)
                {
                    if (file.Contains(dir))
                    {
                        //lstAllDirectories.Remove(dir);
                        duplicateFolders.Add(dir);
                        break;
                    }
                }
            }
            foreach (var item in duplicateFolders)
            {
                lstAllDirectories.Remove(item);
            }

            lstAllFiles.ForEach(i =>
            {
                String _dstnpath = i.Replace(_src, _dstn).Replace('\\','/');
                UploadFile1D(i,_dstnpath);
            });

            lstAllDirectories.ForEach(i => CreateFolder(i.Replace(_src,_dstn).Replace('\\','/')));
        }

        private void GetFileFoldersList(DirectoryInfo _directory)
        {
            _directory.GetFiles().ToList().ForEach(i => lstAllFiles.Add(i.FullName));
            _directory.GetDirectories().ToList().ForEach(i => lstAllDirectories.Add(i.FullName));
            _directory.GetDirectories().ToList().ForEach(i => GetFileFoldersList(i));
        }

        public void ExecuteBackup1D()
        {
            DateTime init = DateTime.Now;
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Backup Started..." + DateTime.Now.ToString());
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            
            foreach (Action item in lstAction)
            {
                try
                {
                    switch (item.ActiononFile)
                    {
                        case ItemAction.COPYFILE:
                            try
                            {
                                UploadFile1D(item.SourcePath, item.DestinationPath);
                                Console.WriteLine("File Copied : " + item.SourcePath);
                            }
                            catch (Exception ex)
                            {
                                Util.PrintError<String>(null, ex.Message + "\n" + ex.StackTrace);
                            }
                            break;
                        case ItemAction.DELETEFILE:
                            DeleteItem(item.DestinationPath);
                            Console.WriteLine("File Deleted : " + item.DestinationPath);
                            break;
                        case ItemAction.COPYDIRECTORY:
                            
                            CopyDirectory1D(item.SourcePath, item.DestinationPath);
                            Console.WriteLine("Directory Copied : " + item.SourcePath + " | " + Util.GetSizeStr(Util.GetDirectorySize(item.SourcePath)));
                            break;
                        case ItemAction.DELETEDIRECTORY:
                            DeleteItem(item.DestinationPath);
                            Console.WriteLine("Directory Deleted : " + item.DestinationPath);
                            break;
                        case ItemAction.OVERWRITE:
                            try
                            {
                                //DeleteItem(item.DestinationPath);
                                UploadFile1D(item.SourcePath, item.DestinationPath);
                                Console.WriteLine("File Overwritten : " + item.SourcePath + " | " + item.OverWriteReason);
                            }
                            catch (Exception ex)
                            {
                                Util.PrintError<String>(null, ex.Message + "\n" + ex.StackTrace);
                            }
                            break;
                        default:
                            break;
                    }
                }
                catch (DirectoryNotFoundException ex)
                {
                    Util.PrintError<String>(null, ex.Message + "\n" + ex.StackTrace,false);
                }
                catch (FileNotFoundException ex)
                {
                    Util.PrintError<String>(null, ex.Message + "\n" + ex.StackTrace,false);
                }
                catch (IOException ex)
                {
                    Util.PrintError<String>(null, ex.Message + "\n" + ex.StackTrace,false);
                }
                catch (Exception ex)
                {
                    Util.PrintError<String>(null, ex.Message + "\n" + ex.StackTrace, true);
                }
            }
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Backup Finished !!! Phew...." + DateTime.Now.ToString());
            Console.BackgroundColor = Util.Settings.TempBackgroundColor;
            Console.ForegroundColor = Util.Settings.TempForegroundColor;
        }
    }
}
