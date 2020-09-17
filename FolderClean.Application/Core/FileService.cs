using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FolderClean.Application.Core
{
    public static class FileService
    {
        /// <summary>
        /// Get All Files from the Existing Folder which contains "info" in its file name
        /// </summary>
        /// <param name="folderPath">The Source Folder Path</param>
        /// <returns>Returns Paths of all the "info" files</returns>
        /// <exception cref="Exception"></exception>
        public static List<string> GetAllFiles(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                throw new Exception("Folder doesn't Exist. Please Specify correct Source Folder : " + folderPath);
            }
            List<string> validFiles = new List<string>();
            var projectDirectories = Directory.GetDirectories(folderPath);
            foreach (var projectDirectory in projectDirectories)
            {
                var directories1 = Directory.GetDirectories(projectDirectory);
                foreach (var directory1 in directories1)

                {
                    /*
                    var directories2 = Directory.GetDirectories(directory1);
                    foreach (var directory2 in directories2)

                    { */

                        List<string> filesTemp = Directory.EnumerateFiles(directory1).ToList();
                        foreach (var file in filesTemp)
                        {
                            var fileInfo = new FileInfo(file);
                            if (fileInfo.Name.Contains("info"))
                            {
                                validFiles.Add(file);
                            }
                        }
                   /* } */
                
                }
            }
            return validFiles;
        }
        /// <summary>
        /// This Checks for the File if it passes business checks
        /// </summary>
        /// <param name="file">Source File Path</param>
        /// <param name="days">No of Days Older</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static (bool,string) IsBatchProcessedAndDateOlder(string file, int days)
        {
            if (days <= 0)
            {
                days = 5;
            }
            string[] batchStates = new[]
            {
                "Export Complete", 
                "PROCESSED",
                "Export Successful"
            };
            if (!File.Exists(file))
            {
                return(false,"");
            }

            string projectName = "";
            bool isBatchStateProcessed = false;
            bool isBatchEndDateOld = false;
            bool isBatchStartDateOld = false;
            var lines = File.ReadAllLines(file);
            // Check Batch State if it is on of "batchStates"
            var batchStateLine = lines.FirstOrDefault(p => p.ToLower().StartsWith("batch.state"));
            if (!string.IsNullOrWhiteSpace(batchStateLine))
            {
                var batchStateValue = batchStateLine.Split('=')[1].Trim();
                if (batchStates.Contains(batchStateValue))
                {
                    isBatchStateProcessed = true;
                }
            }

            // Check Batch Output Start date if it is older than input Days
            var batchOutputStartLine = lines.FirstOrDefault(p => p.ToLower().StartsWith("batch.outputstartdatetime"));
            if (!string.IsNullOrWhiteSpace(batchOutputStartLine))
            {
                var batchDateValue = batchOutputStartLine.Split('=')[1].Trim();
                if (!string.IsNullOrWhiteSpace(batchDateValue))
                {
                    var date = DateTime.Parse(batchDateValue);
                    if (DateTime.UtcNow - date.ToUniversalTime() >= TimeSpan.FromDays(days))
                    {
                        isBatchEndDateOld = true;
                    }
                }
            }
            // Check Batch Created Start date if it is older than input Days
            var batchCreatedLine = lines.FirstOrDefault(p => p.ToLower().StartsWith("batch.createddatetime"));
            if (!string.IsNullOrWhiteSpace(batchCreatedLine))
            {
                var batchDateValue = batchCreatedLine.Split('=')[1].Trim();
                if (!string.IsNullOrWhiteSpace(batchDateValue))
                {
                    var date = DateTime.Parse(batchDateValue);
                    if (DateTime.UtcNow - date.ToUniversalTime() >= TimeSpan.FromDays(days))
                    {
                        isBatchStartDateOld = true;
                    }
                }
            }

            var batchLocationLine = lines.FirstOrDefault(p => p.ToLower().StartsWith("batch.location"));
            if (!string.IsNullOrWhiteSpace(batchLocationLine))
            {
                //batch.Location = \\sbfs01\images\Renal\ACP\Source\Renal- UAT\Renal- UATBatch000000001
                projectName = batchLocationLine.Split('\\')[^2];
            }
            return (isBatchEndDateOld && isBatchStateProcessed && isBatchStartDateOld, projectName);
        }

        /// <summary>
        /// Moves the file parent directory to Destination folder
        /// </summary>
        /// <param name="file">Source File</param>
        /// <param name="destinationFolder">Destination folder</param>
        /// <param name="projectName">Project Name</param>
        public static void MoveFolder(string file, string destinationFolder, string projectName)
        {
            // Check if file exists
            if(File.Exists(file))
            {
                // Check file Info
                var fileInfo = new FileInfo(file);
                if (fileInfo.Directory != null)
                {
                    // Get folder name and Folder path
                    var folderName = fileInfo.Directory.Name;
                    var folderPath = fileInfo.Directory.FullName;
                    // Get Current Date
                    var dateTimeName = DateTime.Now.ToString("yyyy-MM-dd");
                    // Combine Destination folder with DateTime
                    destinationFolder = Path.Combine(destinationFolder, projectName, dateTimeName);
                    // Check if Directory Exits else Create it
                    if (!Directory.Exists(destinationFolder))
                    {
                        Directory.CreateDirectory(destinationFolder);
                    }
                    // Move Folder to Destination Folder
                    // Copy Directory and Paste to Destination Folder
                    CopyFolder(folderPath,Path.Combine(destinationFolder, folderName));
                    //Delete Folder
                    Directory.Delete(folderPath,true);
                }
            }
        }
        public static void CopyFolder(string sourceFolder, string destFolder )
        {
            if (!Directory.Exists( destFolder ))
                Directory.CreateDirectory( destFolder );
            string[] files = Directory.GetFiles( sourceFolder );
            foreach (string file in files)
            {
                string name = Path.GetFileName( file );
                string dest = Path.Combine( destFolder, name );
                File.Copy(file, dest, true);
            }
            string[] folders = Directory.GetDirectories( sourceFolder );
            foreach (string folder in folders)
            {
                string name = Path.GetFileName( folder );
                string dest = Path.Combine( destFolder, name );
                CopyFolder( folder, dest );
            }
        }
    }
}