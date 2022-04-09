using System;
using System.Threading;
using Eventnet.Domain;
using Eventnet.Infrastructure.PhotoServices;

namespace Eventnet.Api.IntegrationTests.Mocks;

public class PhotoToStorageSaveServiceMock : IPhotoToStorageSaveService
{
    public void Save(Photo photo, Guid photoId)
    {
        Thread.Sleep(100);
    }
}