using System;
using GData = Google.Apis.Drive.v3.Data;
using Google.Apis.Drive.v3;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoogleLib.Tools;

namespace GoogleLib
{
    public class GoogleDrive
    {
        private readonly DriveService _service;

        public GoogleDrive()
        {
            _service = GoogleServices.GetDriveService();
        }

        public async Task CreateFolderAndCopyFilesAsync()
        {
            string folderId = await CreateFolderAsync(Date.GetPrevDate());
            await CopyFileAsync(Sheets.HeatingSpreadSheetId, $"Ведомость О ({Date.GetPrevDate()})", folderId);
            await CopyFileAsync(Sheets.WerSpreadSheetId, $"Ведомость СД ({Date.GetPrevDate()})", folderId);
        }

        public async Task<bool> TransitionCheck()
        {
            FilesResource.ListRequest listRequest = _service.Files.List();
            listRequest.PageSize = 100;
            listRequest.Fields = "nextPageToken, files(id, name)";

            IList<GData.File> files = (await listRequest.ExecuteAsync()).Files;
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    if (file.Name == Date.GetPrevDate())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private async Task<string> CreateFolderAsync(string folderName)
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

        private async Task<GData.File> CopyFileAsync(string originFileId, string copyName, 
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