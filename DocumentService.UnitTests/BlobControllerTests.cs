using DMSMicroservice.DocumentManagementService.BlobService;
using DMSMicroservice.DocumentManagementService.Controllers;
using DMSMicroservice.DocumentManagementService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace DocumentService.UnitTests
{
    public class BlobControllerTests
    {
        private readonly Mock<IAzureBlobService> _blobServiceMock;
        private readonly BlobController _controller;

        public BlobControllerTests()
        {
            _blobServiceMock = new Mock<IAzureBlobService>();
            _controller = new BlobController(_blobServiceMock.Object);
        }

        [Fact]
        public async Task UploadFile_NullRequest_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.UploadFile(null);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid file upload request.", badRequest.Value);
        }

        [Fact]
        public async Task UploadFile_NullFile_ReturnsBadRequest()
        {
            // Arrange
            var request = new FileUploadRequest { File = null };

            // Act
            var result = await _controller.UploadFile(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid file upload request.", badRequest.Value);
        }

        [Fact]
        public async Task UploadFile_EmptyFile_ReturnsBadRequest()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);
            var request = new FileUploadRequest { File = fileMock.Object };

            // Act
            var result = await _controller.UploadFile(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid file upload request.", badRequest.Value);
        }

        [Fact]
        public async Task UploadFile_SuccessfulUpload_ReturnsOk()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(10);
            var request = new FileUploadRequest
            {
                File = fileMock.Object,
                BlobContainer = "container",
                DirectoryName = "dir",
                FileName = "file.txt",
                FileStream = new MemoryStream(new byte[] { 1, 2, 3 })
            };
            var response = new AzureBlobResponse { IsError = true };
            _blobServiceMock.Setup(s => s.UploadFileAsync(
                request.BlobContainer, request.DirectoryName, request.FileName, request.FileStream))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.UploadFile(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("File uploaded successfully.", okResult.Value);
        }

        [Fact]
        public async Task UploadFile_FailedUpload_ReturnsInternalServerError()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(10);
            var request = new FileUploadRequest
            {
                File = fileMock.Object,
                BlobContainer = "container",
                DirectoryName = "dir",
                FileName = "file.txt",
                FileStream = new MemoryStream(new byte[] { 1, 2, 3 })
            };
            var response = new AzureBlobResponse { IsError = false, ErrorMessage = "Failed" };
            _blobServiceMock.Setup(s => s.UploadFileAsync(
                request.BlobContainer, request.DirectoryName, request.FileName, request.FileStream))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.UploadFile(request);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.Equal("Failed", objectResult.Value);
        }

        [Fact]
        public async Task UploadFile_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(10);
            var request = new FileUploadRequest
            {
                File = fileMock.Object,
                BlobContainer = "container",
                DirectoryName = "dir",
                FileName = "file.txt",
                FileStream = new MemoryStream(new byte[] { 1, 2, 3 })
            };
            _blobServiceMock.Setup(s => s.UploadFileAsync(
                request.BlobContainer, request.DirectoryName, request.FileName, request.FileStream))
                .ThrowsAsync(new System.Exception("Some error"));

            // Act
            var result = await _controller.UploadFile(request);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.Contains("Some error", objectResult.Value.ToString());
        }
    }
}
