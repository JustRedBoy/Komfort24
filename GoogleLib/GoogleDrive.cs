using System;
using GData = Google.Apis.Drive.v3.Data;
using Google.Apis.Drive.v3;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using GoogleLib.Exceptions;
using Tools;

namespace GoogleLib
{
    public class GoogleDrive : IDisposable
    {
        private readonly DriveService _service;

        public GoogleDrive()
        {
            _service = GoogleServices.GetDriveService();
        }

        /// <summary>
        /// Get file names from Google Drive
        /// </summary>
        /// <returns>Collection of file names</returns>
        /// <exception cref="AccessDeniedException">Thrown when had problem with there was an access problem with Google Drive</exception>
        public async Task<IEnumerable<string>> GetFilesAsync()
        {
            FilesResource.ListRequest listRequest = _service.Files.List();
            listRequest.PageSize = 100;
            listRequest.Fields = "nextPageToken, files(id, name)";
            try
            {
                return (await listRequest.ExecuteAsync()).Files.Select(n => n.Name);
            }
            catch (Exception e)
            {
                throw AccessDeniedException.CreateException(e);
            }
        }

        /// <summary>
        /// Creating folder on Google Drive
        /// </summary>
        /// <param name="folderName">Folder name to create</param>
        /// <returns>New folder ID</returns>
        /// <exception cref="AccessDeniedException">Thrown when had problem with there was an access problem with Google Drive</exception>
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
            try
            {
                var file = await request.ExecuteAsync();
                return file.Id;
            }
            catch (Exception e)
            {
                throw AccessDeniedException.CreateException(e);
            }
        }

        /// <summary>
        /// Copy file to special folder
        /// </summary>
        /// <param name="originFileId">File ID to copy</param>
        /// <param name="copyName">Copied file name</param>
        /// <param name="folderId">Folder ID</param>
        /// <exception cref="AccessDeniedException">Thrown when had problem with there was an access problem with Google Drive</exception>
        public async Task CopyFileAsync(string originFileId, string copyName, string folderId = "root")
        {
            GData.File copiedFile = new GData.File
            {
                Name = copyName,
                Parents = new List<string>() { folderId }
            };
            try
            {
                await _service.Files.Copy(copiedFile, originFileId).ExecuteAsync();
            }
            catch (Exception e)
            {
                throw AccessDeniedException.CreateException(e);
            }
        }

        /// <summary>
        /// Copy files to special folder
        /// </summary>
        /// <param name="folderId">Folder to copy files</param>
        /// <returns></returns>
        public async Task CopyAllFilesAsync(string folderId)
        {
            await CopyFileAsync(Sheets.HeatingSpreadSheetId, $"Ведомость О ({Date.GetFullPrevMonth()})", folderId);
            await CopyFileAsync(Sheets.WerSpreadSheetId, $"Ведомость СД ({Date.GetFullPrevMonth()})", folderId);
        }

        public void Dispose()
        {
            _service.Dispose();
        }
    }
}