using System;

namespace NJN1DBackup
{
    public enum ItemAction
    {
        DELETEFILE = 1,
        DELETEDIRECTORY = 2,
        OVERWRITE = 4,
        COPYFILE = 8,
        COPYDIRECTORY = 16
    };

    public class Action
    {
        public Action(ItemAction _action, string _src, string _dstn, string _reason = "", long _destinationObjectSize = 0)
        {
            this.ActiononFile = _action;
            this.SourcePath = _src;
            this.DestinationPath = _dstn;
            this.OverWriteReason = _reason;
            this.DestinationObjSize = _destinationObjectSize;
        }

        public ItemAction ActiononFile { get; }
        public String SourcePath { get; }
        public String DestinationPath { get; }
        public String OverWriteReason { get; }
        public long DestinationObjSize { get; }
    }
}
