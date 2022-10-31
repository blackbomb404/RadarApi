using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using static System.IO.Path;

namespace RadarApi.Helpers
{
    public class FileUploadUtil
    {
        public static readonly string uploadDirectory = "X:\\_uploaded";
        public static readonly string requestPath = "/public";

        private static readonly Dictionary<string, List<byte[]>> _fileSignatures =
            new Dictionary<string, List<byte[]>>
            {
                { ".jpg", new List<byte[]>
                    {
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
                    }
                },
                {
                    ".png", new List<byte[]>
                    {
                        new byte[] {0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A}
                    }
                }
            };

        /// <summary>
        /// Asynchronously persists an IFormFile instance on physical storage.
        /// </summary>
        /// <param name="file"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static async Task<string> SaveAsync(IFormFile file)
        {
            string extension = GetExtension(file.FileName).ToLowerInvariant();

            if (!IsExtensionAllowed(extension) || !IsFileSignatureValid(extension, file))
            {
                throw new ArgumentException();
            }

            string randomFileName = GetFileNameWithoutExtension(GetRandomFileName()) + extension;
            string path = Combine(uploadDirectory, randomFileName);

            using (var stream = File.Create(path))
            {
                await file.CopyToAsync(stream);
            }

            return randomFileName;
        }

        /// <summary>
        /// Checks if a file with non-zero length was provided.
        /// </summary>
        /// <param name="file"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void CheckUploadedFile(IFormFile file)
        {
            if (file == null)
                throw new ArgumentNullException(message: "No file was uploaded.", paramName: "file");

            if (file.Length == 0)
                throw new ArgumentException(message: "Zero-length file was uploaded.", paramName: "file");
        }

        /// <summary>
        /// Checks if the provided extension is allowed.
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="path"></param>
        /// <returns>True if the extension is allowed, otherwise it returns false.</returns>
        private static bool IsExtensionAllowed(string extension)
        {
            var allowedExtensions = _fileSignatures.Keys;

            return !string.IsNullOrEmpty(extension) && allowedExtensions.Contains(extension);
        }

        /// <summary>
        /// Checks if the provided <paramref name="file"/> has a valid file signature.
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="stream"></param>
        /// <returns>True if the file signature is valid, otherwise it returns false.</returns>
        private static bool IsFileSignatureValid(string extension, IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);

                List<byte[]> signatures = _fileSignatures[extension];
                var firstBytesOfFile = memoryStream.ToArray().Take(signatures.Max(s => s.Length));

                return signatures.Any(signature => firstBytesOfFile.Take(signature.Length).SequenceEqual(signature));
            }
        }

    }
}
