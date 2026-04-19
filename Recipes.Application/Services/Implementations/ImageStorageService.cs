using System.Net;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Recipes.Application.Options.Interfaces;
using Recipes.Application.Services.Interfaces;

namespace Recipes.Application.Services.Implementations;

public class ImageStorageService(IObjectStorageOptions objectStorageOptions, ILogger<ImageStorageService> logger)
    : IImageStorageService
{
    private readonly string _endpoint = objectStorageOptions.Endpoint;
    private readonly string _bucket = objectStorageOptions.Bucket;
    private readonly string _accessKey = objectStorageOptions.AccessKey;
    private readonly string _secretKey = objectStorageOptions.SecretKey;
    private readonly string _region = objectStorageOptions.Region;

    private AmazonS3Client S3Client
    {
        get
        {
            if (field == null)
            {
                var config = new AmazonS3Config
                {
                    ServiceURL = _endpoint,
                    RegionEndpoint = RegionEndpoint.GetBySystemName(_region),
                    ForcePathStyle = true
                };

                field = new AmazonS3Client(_accessKey, _secretKey, config);
            }

            return field;
        }
    }

    public async Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType)
    {
        try
        {
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";

            var request = new PutObjectRequest
            {
                BucketName = _bucket,
                Key = uniqueFileName,
                InputStream = fileStream,
                ContentType = contentType,
                CannedACL = S3CannedACL.PublicRead
            };

            await S3Client.PutObjectAsync(request);

            return uniqueFileName;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error uploading image to Object Storage");
            throw new InvalidOperationException("Failed to upload image to Object Storage", ex);
        }
    }

    public async Task<List<string>> UploadImagesAsync(
        IEnumerable<(Stream Stream, string FileName, string ContentType)> files)
    {
        var urls = new List<string>();

        foreach (var file in files)
        {
            var url = await UploadImageAsync(file.Stream, file.FileName, file.ContentType);
            urls.Add(url);
        }

        return urls;
    }

    public async Task DeleteImageAsync(string fileName)
    {
        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _bucket,
                Key = fileName
            };

            await S3Client.DeleteObjectAsync(request);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            logger.LogWarning("Image not found in Object Storage: {FileName}", fileName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting image from Object Storage");
            throw new InvalidOperationException("Failed to delete image from Object Storage", ex);
        }
    }

    public async Task DeleteImagesAsync(IEnumerable<string> fileNames)
    {
        foreach (var fileName in fileNames)
        {
            await DeleteImageAsync(fileName);
        }
    }

    public async Task<bool> ImageExistsAsync(string fileName)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = _bucket,
                Key = fileName
            };

            await S3Client.GetObjectMetadataAsync(request);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking image existence in Object Storage");
            return false;
        }
    }

    public string GetImageUrl(string fileName)
    {
        return $"{_endpoint}/{_bucket}/{fileName}";
    }
}