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
        private readonly string File = "master.csv";
        private readonly string Container = "master";

        /// <summary>
        /// Save file all transactions
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public string SaveMasterOFXFile(List<TransactionViewModel> list) {
            try {
                var fileblob = GetFileBlob(Container, File);
                using (StreamWriter writer = new StreamWriter(fileblob.OpenWrite(), Encoding.Default)) {
                    using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.CurrentCulture)) {
                        csv.Configuration.RegisterClassMap<TransactionDefinitionMap>();
                        csv.Configuration.Delimiter = ";";
                        csv.WriteHeader<TransactionViewModel>();
                        csv.NextRecord();
                        csv.WriteRecords(list);
                    }
                }
                return File;
            } catch (Exception ex) {
                throw ex;
            }
        }

        public List<TransactionViewModel> GetMasterOFXFile() {
            try {
                var toReturn = new List<TransactionViewModel>();
                var fileblob = GetFileBlob(Container, File);
                if (!fileblob.Exists())
                    return toReturn;
                using (StreamReader read = new StreamReader(fileblob.OpenRead(), Encoding.Default)) {
                    using (var csv = new CsvReader(read, System.Globalization.CultureInfo.CurrentCulture)) {
                        csv.Configuration.RegisterClassMap<TransactionDefinitionMap>();
                        csv.Configuration.Delimiter = ";";
                        csv.Configuration.Encoding = Encoding.Default;
                        toReturn = csv.GetRecords<TransactionViewModel>().ToList();
                    }
                }
                return toReturn;
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
        /// <returns>Url file blob</returns>
        public string SaveFile(IFormFile file, string containerName, List<string> extensions) {
            string toReturn = "";
            if (file != null && file.Length > 0) {
                var extension = Path.GetExtension(file.FileName);
                if (extensions.Any(e => e.Equals(extension, StringComparison.CurrentCultureIgnoreCase))) {
                    using (var ms = new MemoryStream()) {
                        file.CopyTo(ms);
                        toReturn = UploadFileAzure(containerName, DateTime.Now.ToString("dd-MM-yyyy - hh-mm-ss") + extension, ms.ToArray());
                    }
                }
            }
            return toReturn;
        }


        public string GetFileError(string file, string blobName) {
            try {
                var blobfile = GetFileBlob(blobName, file);
                return blobfile.DownloadTextAsync().Result;
            } catch (Exception) {
                throw;
            }
        }


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

        public CloudBlockBlob GetFileBlob(string containerName, string file) {
            var blobClient = AzureStorageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            container.CreateIfNotExistsAsync().GetAwaiter().GetResult();
            var fileblob = container.GetBlockBlobReference(file);
            return fileblob;
        }

        public bool RemoveBLob(string filename, string containerName) {
            var blobClient = AzureStorageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            var blobReference = container.GetBlockBlobReference(filename);
            if (blobReference.ExistsAsync().GetAwaiter().GetResult()) {
                blobReference.DeleteAsync().GetAwaiter().GetResult();
                return true;
            }
            return false;
        }
    }
}
