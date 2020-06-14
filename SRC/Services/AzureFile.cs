using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Nibo.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Nibo.Services.Blob {

    public class AzureFile {

        private readonly CloudStorageAccount AzureStorageAccount = CloudStorageAccount.Parse(Startup.AzureStorage);

        /// <summary>
        /// Save file all transactions
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public string SaveCSV(List<TransactionViewModel> list) {
            try {
                var fileName = DateTime.Now.ToString("dd-MM-yyyy - hh-mm-ss") + ".csv";
                using (MemoryStream ms = new MemoryStream()) {
                    using (StreamWriter writer = new StreamWriter(ms)) {
                        using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.CurrentCulture)) {
                            csv.Configuration.RegisterClassMap<TransactionDefinitionMap>();
                            csv.Configuration.Delimiter = ";";
                            csv.WriteHeader<TransactionViewModel>();
                            csv.NextRecord();
                            csv.WriteRecords(list);
                        }
                    }
                    UploadFileAzure("duplicates", fileName, ms.ToArray());
                }
                return fileName;
            } catch (Exception ex) {
                throw ex;
            }
        }


        /// <summary>
        /// Save file on container with extensions defined
        /// </summary>
        /// <param name="file"></param>
        /// <param name="containerName"></param>
        /// <param name="extensions"></param>
        /// <returns>file name</returns>
        public string SaveFile(IFormFile file, string containerName, List<string> extensions) {
            string toReturn = "";
            if (file != null && file.Length > 0) {
                var extension = Path.GetExtension(file.FileName);
                if (extensions.Any(e => e.Equals(extension, StringComparison.CurrentCultureIgnoreCase))) {
                    using (var ms = new MemoryStream()) {
                        file.CopyTo(ms);
                        toReturn = DateTime.Now.ToString("dd-MM-yyyy - hh-mm-ss") + extension;
                        UploadFileAzure(containerName, toReturn, ms.ToArray());
                    }
                }
            }
            return toReturn;
        }

        /// <summary>
        /// Get file on container
        /// </summary>
        /// <param name="file"></param>
        /// <param name="blobName"></param>
        /// <returns>file name</returns>
        public string GetFile(string file, string blobName) {
            try {
                var blobfile = GetFileBlob(blobName, file);
                return blobfile.DownloadTextAsync().Result;
            } catch (Exception) {
                throw;
            }
        }

        /// <summary>
        /// Save file on container
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        /// <returns>Uri file</returns>
        public string UploadFileAzure(string containerName, string fileName, byte[] data) {
            var blobClient = AzureStorageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            container.CreateIfNotExistsAsync().GetAwaiter().GetResult();

            var permissions = container.GetPermissionsAsync().GetAwaiter().GetResult();
            if (permissions.PublicAccess == BlobContainerPublicAccessType.Off) {
                permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                container.SetPermissionsAsync(permissions).GetAwaiter().GetResult();
            }

            var relatorioBlob = container.GetBlockBlobReference(fileName);
            relatorioBlob.UploadFromByteArrayAsync(data, 0, data.Length).GetAwaiter().GetResult();

            return relatorioBlob.Uri.AbsoluteUri;
        }

        /// <summary>
        /// Get file from blob
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="file"></param>
        /// <returns>file</returns>
        public CloudBlockBlob GetFileBlob(string containerName, string file) {
            var blobClient = AzureStorageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            container.CreateIfNotExistsAsync().GetAwaiter().GetResult();
            var fileblob = container.GetBlockBlobReference(file);
            return fileblob;
        }
    }
}
