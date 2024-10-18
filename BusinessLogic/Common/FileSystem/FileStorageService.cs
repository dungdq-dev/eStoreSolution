using Common.Constants;
using Microsoft.AspNetCore.Hosting;

namespace BusinessLogic.Common.FileSystem
{
    public class FileStorageService : IStorageService
    {
        private readonly string _userContentFolder;

        public FileStorageService(IHostingEnvironment webHostEnvironment)
        {
            _userContentFolder = Path.Combine(webHostEnvironment.WebRootPath, SystemConstants.UserContentFolderName);
        }

        public string GetFileUrl(string fileName)
        {
            return $"/{SystemConstants.UserContentFolderName}/{fileName}";
        }

        public async Task SaveFileAsync(Stream mediaBinaryStream, string fileName)
        {
            var filePath = Path.Combine(_userContentFolder, fileName);
            using var output = new FileStream(filePath, FileMode.Create);
            await mediaBinaryStream.CopyToAsync(output);
        }

        public async Task DeleteFileAsync(string fileName)
        {
            var filePath = Path.Combine(_userContentFolder, fileName);
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
        }
    }
}