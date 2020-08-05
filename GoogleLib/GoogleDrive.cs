using System;
using GData = Google.Apis.Drive.v3.Data;
using Google.Apis.Drive.v3;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace GoogleLib
{
    public class GoogleDrive
    {
        private readonly DriveService _service;

        public GoogleDrive()
        {
            _service = GoogleServices.GetDriveService();
        }

        public async Task<IEnumerable<string>> GetFilesAsync()
        {
            FilesResource.ListRequest listRequest = _service.Files.List();
            listRequest.PageSize = 100;
            listRequest.Fields = "nextPageToken, files(id, name)";

             return (await listRequest.ExecuteAsync()).Files.Select(n => n.Name);
        }

        public async Task<string> CreateFolderAsync(string folderName)
        {
            GData.File FileMetaData = new GData.File
            {
                Name = folderName,
                MimeType = "application/vnd.google-apps.folder"
            };
            FilesResource.CreateRequest request;

            request = _service.Files.Create(FileMetaData);
            request.Fields = "id";
            var file = await request.ExecuteAsync();
            return file.Id;
        }

        public async Task<GData.File> CopyFileAsync(string originFileId, string copyName, 
            string folderId = "root")
        {
            GData.File copiedFile = new GData.File
            {
                Name = copyName,
                Parents = new List<string>() { folderId }
            };
            try
            {
                return await _service.Files.Copy(copiedFile, originFileId).ExecuteAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}