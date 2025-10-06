using Azure;
using Azure.Storage.Files.Shares;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST10281918_NDIVHUWONDOU_CLDV6212_PART1_Function.Services
{
    public class FileShareStorageService
    {
        private readonly ShareClient _shareClient;

        public FileShareStorageService(string storageAccount, string shareName)
        {
            var shareServiceClient = new ShareServiceClient(storageAccount);
            _shareClient = shareServiceClient.GetShareClient(shareName);
            _shareClient.CreateIfNotExists();
        }

        public async Task UpLoadFile(string fileName, Stream fileStream)
        {
            var directoryClient = _shareClient.GetRootDirectoryClient();
            var fileClient = directoryClient.GetFileClient(fileName);
            await fileClient.CreateAsync(fileStream.Length);

            long position = 0;
            int bufferSize = 4 * 1024 * 1024;
            byte[] buffer = new byte[bufferSize];
            int bytesRead;

            while ((bytesRead = await fileStream.ReadAsync(buffer, 0, bufferSize)) > 0)
            {
                using var memoryStream = new MemoryStream(buffer, 0, bytesRead);
                await fileClient.UploadRangeAsync(
                    Azure.Storage.Files.Shares.Models.ShareFileRangeWriteType.Update,
                    new HttpRange(position, bytesRead), memoryStream
                );
                position += bytesRead;
            }
        }

    }
}
