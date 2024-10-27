using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using System.Collections.Generic;

namespace ELSA.Services.Interface
{
    public interface IFileService
    {
        Task<string[]> StoreFilesAsync(ObjectId uniqueId, IFormFile[] formFiles);
        Task<ImageUploadResult[]> StoreFilesAsync(ObjectId uniqueId, Dictionary<int, IFormFile> formFiles);
        Task<bool> CleanFilesAsync(string[] urls); 
    }
}
