﻿namespace MyForum.Services.Data
{
    using System.IO;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;

    public class UnitOfWork : IUnitOfWork
    {
        private IHostingEnvironment hostingEnvironment;

        public UnitOfWork(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        public async void UploadImage(IFormFile file)
        {
            var totalBytes = file.Length;
            var fileName = file.FileName.Trim('"');
            fileName = EnsureFileName(fileName);
            var buffer = new byte[16 * 1024];

            using (FileStream output = File.Create(GetPathAndFileName(fileName)))
            {
                using (Stream input = file.OpenReadStream())
                {
                    int readBytes;
                    while ((readBytes = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        await output.WriteAsync(buffer, 0, readBytes);
                        totalBytes += readBytes;
                    }
                }
            }
        }

        private string GetPathAndFileName(string fileName)
        {
            var path = hostingEnvironment.WebRootPath + "\\uploads\\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path + fileName;
        }

        private string EnsureFileName(string fileName)
        {
            if (fileName.Contains("\\"))
            {
                fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
            }

            return fileName;
        }
    }
}
