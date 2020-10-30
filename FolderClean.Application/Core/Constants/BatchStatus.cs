namespace FolderClean.Application.Core.Constants
{
    public class BatchStatus
    {
        // Ready                                               - Page Stats have been loaded.
        //     Waiting for Page Stats                   - Waiting for the page stats to be loaded.
        //     Completed                                       - Transferred to SBStats Tables correctly.
        //     Batch Already Exists                       - The Batch already exists in SBStats Tables.
        public const string Ready = "Ready";
        public const string Waiting = "Waiting for Page Stats";
        public const string Completed = "Completed";
        public const string AlreadyExists = "Batch Already Exists";
    }
}