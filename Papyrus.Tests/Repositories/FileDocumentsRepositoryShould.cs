﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using Papyrus.Business.Domain.Documents;
using Papyrus.Business.Domain.Products;
using Papyrus.Infrastructure.Core;
using Papyrus.Infrastructure.Repositories;

namespace Papyrus.Tests.Repositories {
    [TestFixture]
    public class FileDocumentsRepositoryShould : FileRepositoryTest {
        private FileDocumentsRepository documentsRepository;
        private InsertableDocumentsBuilder given;

        [SetUp]
        public void Setup() {
            documentsRepository = new FileDocumentsRepository(new JsonFileSystemProvider(WorkingDirectoryPath));
            given = new InsertableDocumentsBuilder(WorkingDirectoryPath);
        }

        [Test]
        public void get_only_documents_for_a_concrete_version() {
            var product = AnyUniqueString();
            var version = AnyUniqueString();
            var expectedDocument = given.ADocumentFor(product, version);
            given.AnyOtherDocument();

            var documents = GetDocumentationFor(product, version);

            documents.Should().HaveCount(1);
            documents.Should().Contain(ADocumentEquivalentTo(expectedDocument));
        }

        [Test]
        public void create_a_document_for_a_concrete_version() {
            var documentToInsert = AnyDocument();

            documentsRepository.CreateDocumentFor(documentToInsert);

            var documents = GetAllDocumentsFrom(WorkingDirectoryPath).Single();
            documents.ShouldBeEquivalentTo(documentToInsert, option => 
                        option.Excluding(d => d.Id).Excluding(d => d.ProductId).Excluding(d => d.VersionId));
            var versionIdentifier = GetVersionIdFrom(documents);
            versionIdentifier.ShouldBeEquivalentTo(documentToInsert.VersionIdentifier);
        }

        private VersionIdentifier GetVersionIdFrom(SerializableDocument document) {
            var versionId = document.VersionId;
            var productId = document.ProductId;
            var versionIdentifier = new VersionIdentifier(productId, versionId);
            return versionIdentifier;
        }

        private List<SerializableDocument> GetAllDocumentsFrom(string documentsPath) {
            return new DirectoryInfo(documentsPath)
                .GetFiles()
                .Select(f => File.ReadAllText(f.FullName))
                .Select(JsonConvert.DeserializeObject<SerializableDocument>)
                .ToList();
        }

        private List<Document> GetDocumentationFor(string product, string version) {
            return documentsRepository
                .GetDocumentationFor(new VersionIdentifier(product, version))
                .AsDocumentsCollection()
                .ToList();
        }

        private static Document AnyDocument() {
            return new Document(
                new DocumentId(AnyUniqueString()), 
                AnyUniqueString(), 
                AnyUniqueString(), 
                AnyUniqueString(), 
                AnyUniqueString(), 
                new VersionIdentifier(AnyUniqueString(), AnyUniqueString()));
        }

        private static Expression<Func<Document, bool>> ADocumentEquivalentTo(SerializableDocument documentToInsert) {
            return document => 
                document.Title == documentToInsert.Title && 
                document.Description == documentToInsert.Description && 
                document.Content == documentToInsert.Content && 
                document.Language == documentToInsert.Language && 
                document.VersionIdentifier.ProductId == documentToInsert.ProductId &&
                document.VersionIdentifier.VersionId == documentToInsert.VersionId;
        }
    }
}