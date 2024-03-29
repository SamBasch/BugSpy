﻿using BugSpy.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace BugSpy.Services
{
    public class BTFileService : IBTFileService
    {
        #region Globals
        private readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };
        private readonly string _defaultBTUserImageSrc = "/dist/images/DefaultUserImage.png";
        private readonly string _defaultCompanyImageSrc = "/dist/images/DefaultCompanyImage.png";
        private readonly string _defaultProjectImageSrc = "/dist/images/DefaultProjectImage.png";
        #endregion
        #region Convert Byte Array to File
        public string ConvertByteArrayToFile(byte[] fileData, string extension, int imageType)
        {
            if ((fileData == null || fileData.Length == 0))
            {
                switch (imageType)
                {
                    // BTUser Image based on the 'DefaultImage' Enum
                    case 1: return _defaultBTUserImageSrc;
                    // Company Image based on the 'DefaultImage' Enum
                    case 2: return _defaultCompanyImageSrc;
                    // Project Image based on the 'DefaultImage' Enum
                    case 3: return _defaultProjectImageSrc;
                }
            }
            try
            {
                string imageBase64Data = Convert.ToBase64String(fileData!);
                return string.Format($"data:{extension};base64,{imageBase64Data}");
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region Convert File to Byte Array
        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            try
            {
                using MemoryStream memoryStream = new MemoryStream();

                await file.CopyToAsync(memoryStream);

                byte[] byteFile = memoryStream.ToArray();

                memoryStream.Close();

                return byteFile;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region Format File Size
        public string FormatFileSize(long bytes)
        {
            int counter = 0;
            decimal fileSize = bytes;
            while (Math.Round(fileSize / 1024) >= 1)
            {
                fileSize /= bytes;
                counter++;
            }
            return string.Format("{0:n1}{1}", fileSize, suffixes[counter]);
        }
        #endregion
        #region Get File Icon
        public string GetFileIcon(string file)
        {
            string fileImage = "default";
            if (!string.IsNullOrWhiteSpace(file))
            {
                string ext = Path.GetExtension(file).Replace(".", "");
				return $"/img/contenttype/{ext}.png";
			}
            return fileImage;
        }
        #endregion    
    }
}

