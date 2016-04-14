using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NJN1DBackup
{
    [DataContract]
    public class CreateItemObj
    {
        [DataMember]
        public String id { get; set; }

        [DataMember]
        public String name { get; set; }

        [DataMember]
        public ItemFolderObj folder { get; set; }

        [DataMember]
        public String size { get; set; }

        //future get hash as well to check if proper file is uploaded
    }

    public class ItemObj
    {
        public String id { get; set; }
        public String name { get; set; }
        public String size { get; set; }
        public String lastModifiedDateTime { get; set; }
        public ItemFileObj file { get; set; }
        public ItemFolderObj folder { get; set; }
    }

    public class errorObj
    {
        public String code { get; set; }
        public String message { get; set; }
        public innerErrorObj innnererror { get; set; }
    }

    public class innerErrorObj
    {
        public String code { get; set; }
    }

    public class ItemFolderObj
    {
        public String childCount { get; set; }
    }

    public class ItemFileObj
    {
        public ItemFileHashesObj hashes { get; set; }

        public class ItemFileHashesObj
        {
            public String crc32Hash { get; set; }
            public String sha1Hash { get; set; }
        }
    }

    public class OneDriveFolderObj
    {
        public OneDriveFolderObj(string id, string name,string size, string childcount, string lastModifiedDateTime)
        {
            this.ID = id;
            this.Name = name;
            this.Childcount = Convert.ToInt16(childcount);
            this.lastModifiedDateTime = Convert.ToDateTime(lastModifiedDateTime);
            this.Size = Convert.ToInt64(size);
        }
        //private string _id;
        //private string _name;
       // private long _size;
       // private int _childcount;
        //private DateTime _lastModifiedDateTime;

        public String ID
        {
            get;
        }

        public String Name
        {
            get;
        }

        public int Childcount
        {
            get;
        }

        public DateTime lastModifiedDateTime
        {
            get;
        }

        public long Size
        {
            get;
        }
    }

    public class OneDriveFileObj
    {
        public OneDriveFileObj(string id, string name, string size, string lastModifiedDateTime,ItemFileObj.ItemFileHashesObj hashes)
        {
            this.ID = id;
            this.Name = name;
            this.lastModifiedDateTime = Convert.ToDateTime(lastModifiedDateTime);
            this.Size = Convert.ToInt64(size);
            this.CRC32Hash = hashes.crc32Hash;
            this.SHA1Hash = hashes.sha1Hash;
        }
        //private string _id;
        //private string _name;
        //private long _size;
        //private string _crc32Hash;
        //private string _sha1Hash;

        //private DateTime _lastModifiedDateTime;

        public String ID
        {
            get;
        }

        public String Name
        {
            //get { return _name; }
            get;
        }

        public DateTime lastModifiedDateTime
        {
            //get { return _lastModifiedDateTime; }
            get;
        }

        public long Size
        {
            //get { return _size; }
            get;
        }

        public String CRC32Hash
        {
            //get { return _crc32Hash; }
            get;
        }

        public String SHA1Hash
        {
            //get { return _sha1Hash; }
            get;
        }
    }


    [DataContract]
    public class ItemObjArray
    {
        [DataMember]
        public List<ItemObj> value { get; set; }
    }

    [DataContract]
    public class ErrorObj1D
    {
        [DataMember]
        public errorObj error {get;set;}
    }


    [DataContract]
    public class UploadSessionResultObject
    {
        [DataMember]
        public String uploadUrl { get; set; }

        [DataMember]
        public List<string> nextExpectedRanges { get; set; }
    }

    [DataContract]
    public class Token
    {
        [DataMember(Name = "access_token")]
        public String accessToken { get; set; }

        [DataMember(Name = "refresh_token")]
        public String refreshToken { get; set; }

        [DataMember(Name = "token_type")]
        public String tokenType { get; set; }

        [DataMember(Name = "expires_in")]
        public String expiresIn { get; set; }

        [DataMember(Name = "scope")]
        public String scope { get; set; }
    }

    [DataContract]
    public class FolderCreation
    {
        [DataMember(Name = "name")]
        public string name { get; set; }

        [DataMember(Name ="folder")]
        public string folder { get; set; }

        [DataMember(Name = "@name.conflictBehavior")]
        public string nameconflictBehavior { get; set; }
    }
}
