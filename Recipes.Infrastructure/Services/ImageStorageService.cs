using System.Net;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Recipes.Application.Options.Interfaces;
using Recipes.Application.Services.Interfaces;

namespace Recipes.Infrastructure.Services;

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
                field = CreateS3Client();
            }

            return field;
        }
    }

    public async Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType)
    {
        try
        {
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            await S3Client.PutObjectAsync(CreatePutObjectRequest(uniqueFileName, fileStream, contentType));

            return uniqueFileName;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error uploading image to Object Storage");
            throw new InvalidOperationException("Failed to upload image to Object Storage", ex);
        }
    }

    public async Task DeleteImageAsync(string fileName)
    {
        try
        {
            await S3Client.DeleteObjectAsync(CreateDeleteObjectRequest(fileName));
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

    public string GetImageUrl(string fileName)
    {
        return $"{_endpoint}/{_bucket}/{fileName}";
    }

    private AmazonS3Client CreateS3Client()
    {
        return new AmazonS3Client(_accessKey, _secretKey, CreateS3Config());
    }

    private AmazonS3Config CreateS3Config()
    {
        return new AmazonS3Config
        {
            ServiceURL = _endpoint,
            RegionEndpoint = RegionEndpoint.GetBySystemName(_region),
            ForcePathStyle = true
        };
    }

    private PutObjectRequest CreatePutObjectRequest(string fileName, Stream fileStream, string contentType)
    {
        return new PutObjectRequest
        {
            BucketName = _bucket,
            Key = fileName,
            InputStream = fileStream,
            ContentType = contentType,
            CannedACL = S3CannedACL.PublicRead
        };
    }

    private DeleteObjectRequest CreateDeleteObjectRequest(string fileName)
    {
        return new DeleteObjectRequest
        {
            BucketName = _bucket,
            Key = fileName
        };
    }

}
