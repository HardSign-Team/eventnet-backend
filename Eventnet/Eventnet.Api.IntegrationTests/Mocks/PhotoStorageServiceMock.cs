using System;
using System.Threading;
using Eventnet.Domain;
using Eventnet.Infrastructure.PhotoServices;

namespace Eventnet.Api.IntegrationTests.Mocks;

public class PhotoStorageServiceMock : IPhotoStorageService
{
    public void Save(Photo photo, Guid photoId)
    {
        Thread.Sleep(100);
    }

    public string GetPhotoPath(Guid arg) => arg.ToString();
    public void Delete(Guid photoId)
    {
        Thread.Sleep(100);
    }
}