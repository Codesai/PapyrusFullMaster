﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Papyrus.Business.Products;
using Papyrus.Business.Topics;

namespace Papyrus.Business.Exporters
{
    public class ExportableVersionRange
    {
        public List<ProductVersion> Versions { get; private set; } 
        public List<ExportableDocument> Documents { get; private set; }

        public ExportableVersionRange()
        {
            Versions = new List<ProductVersion>();
            Documents = new List<ExportableDocument>();
        }

        public void AddDocument(ExportableDocument document)
        {
            Documents.Add(document);
        }

        public ExportableDocument GetDocumentByLanguage(string language)
        {
            return Documents.First(d => d.Language == language);
        }

        public void AddVersion(ProductVersion version)
        {
            Versions.Add(version);
        }

        public List<string> Languages()
        {
            return Documents.Select(d => d.Language).ToList();
        }

        public async Task ExportDocumentForProductVersion(ProductVersion productVersion, DirectoryInfo directory, string extension) {
            var versionDirectory = directory.CreateSubdirectory(productVersion.VersionName);
            await CreateDocumentsStructureForEachLanguageIn(versionDirectory, extension);
        }

        private async Task CreateDocumentsStructureForEachLanguageIn(DirectoryInfo versionDirectory, string extension) {
            foreach (var language in Languages()) {
                await ConstructDocumentForLanguageInDirectory(language, versionDirectory, extension);
            }
        }

        private async Task ConstructDocumentForLanguageInDirectory(string language, DirectoryInfo versionDirectory, string extension)
        {
            var languageDirectory = versionDirectory.CreateSubdirectory(language);
            var documentName = GetDocumentByLanguage(language).Title + extension;
            var path = Path.Combine(languageDirectory.FullName, documentName);
            var documentContent = GetDocumentByLanguage(language).Content;
            await WriteTextAsync(path, documentContent);
        }

        private async Task WriteTextAsync(string filePath, string text)
        {
            byte[] encodedText = Encoding.UTF8.GetBytes(text);

            using (FileStream sourceStream = new FileStream(filePath,
                FileMode.Append, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            };
        }

        public async Task ExportVersionRangeIn(DirectoryInfo directory, string extension)
        {
            foreach (var productVersion in Versions)
            {
                await ExportDocumentForProductVersion(productVersion, directory, extension);
            }
        }

        public bool Contains(ProductVersion version) {
            return Versions.Contains(version);
        }
    }
}