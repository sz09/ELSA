using Microsoft.AspNetCore.Http;
using ELSA.Config;
using ELSA.Services.Interface;
using Azure.Storage.Blobs;
using ELSA.Utilities;
using MongoDB.Bson;
using MongoDB.Driver;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using System;

namespace ELSA.Services
{
    public class ImageUploadResult
    {
        public int Index { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string Result { get; set; }
    }
    public class FileService : IFileService
    {
        private readonly Lazy<BlobContainerClient> _containerClient;
        public FileService(IApplicationConfig config)
        {

            _containerClient = new Lazy<BlobContainerClient>(() => new BlobContainerClient(config.Blob.AzureBlobConnectionString, config.Blob.AzureBlobContainer));
        }
        public async Task<bool> CleanFilesAsync(string[] urls)
        {
            var urlsToProcess = urls.Select(x => x.Trim()).Where(d => !string.IsNullOrWhiteSpace(d)).ToArray();
            if (urlsToProcess.Length == 0)
            {
                return true;
            }

            try
            {
                var result = await urlsToProcess.ConcurrentExecForEachAsync(DeleteAsync);
                if (result.Any(d => !d))
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> DeleteAsync(string blobFile)
        {
            if (string.IsNullOrEmpty(blobFile))
            {
                return true;
            }

            try
            {
                var blobFileUri = blobFile.Replace(_containerClient.Value.Uri.ToString() + "/", string.Empty);
                BlobClient blob = _containerClient.Value.GetBlobClient(blobFileUri);
                if (await blob.ExistsAsync())
                {
                    await blob.DeleteAsync();
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<string[]> StoreFilesAsync(ObjectId uniqueId, IFormFile[] formFiles)
        {
            var result = new List<ImageUploadResult>();
            if (formFiles == null || formFiles.Length == 0)
            {
                return [];
            }

            var orderingFormFiles = formFiles.Select((d, i) =>
            {
                return new { Index = i, File = d };
            }).ToArray();
            var uploadResults = await orderingFormFiles.ConcurrentExecForEachAsync(async d =>
            {
                var formFile = d.File;
                var blobFileName = $"{uniqueId}_{formFile.FileName}";
                BlobClient blob = _containerClient.Value.GetBlobClient(blobFileName);
                using (var stream = new MemoryStream())
                {
                    formFile.CopyTo(stream);
                    stream.Position = 0;
                    var uploadResult = await blob.UploadAsync(stream, true);
                    if (uploadResult.GetRawResponse().IsError)
                    {
                        return new ImageUploadResult
                        {
                            Index = d.Index,
                            IsSuccess = false,
                        };
                    }

                    return new ImageUploadResult
                    {
                        Index = d.Index,
                        Result = blob.Uri.OriginalString
                    };
                }

            });
            if(uploadResults.Any(d => !d.IsSuccess))
            {
                throw new Exception($"Store files {result.Where(d => !d.IsSuccess).Select(d => d.Index)} failed");
            }

            return [.. uploadResults.OrderBy(d => d.Index).Select(d => d.Result)];
        }

        public async Task<ImageUploadResult[]> StoreFilesAsync(ObjectId uniqueId, Dictionary<int, IFormFile> formFiles)
        {
            if (formFiles == null || !formFiles.Any())
            {
                return [];
            }

            var uploadResults = await formFiles.ConcurrentExecForEachAsync(async d =>
            {
                var formFile = d.Value;
                var blobFileName = $"{uniqueId}_{formFile.FileName}";
                BlobClient blob = _containerClient.Value.GetBlobClient(blobFileName);
                using (var stream = new MemoryStream())
                {
                    formFile.CopyTo(stream);
                    stream.Position = 0;
                    var uploadResult = await blob.UploadAsync(stream, true);
                    if (uploadResult.GetRawResponse().IsError)
                    {
                        return new ImageUploadResult
                        {
                            Index = d.Key,
                            IsSuccess = false,
                        };
                    }

                    return new ImageUploadResult
                    {
                        Index = d.Key,
                        Result = blob.Uri.OriginalString
                    };
                }

            });
            if (uploadResults.Any(d => !d.IsSuccess))
            {
                throw new Exception($"Store files {uploadResults.Where(d => !d.IsSuccess).Select(d => d.Index)} failed");
            }
            return uploadResults.OrderBy(d => d.Index).ToArray();
        }
    }
}
