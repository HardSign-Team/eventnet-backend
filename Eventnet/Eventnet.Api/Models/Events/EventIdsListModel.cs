using Eventnet.Api.Binding;
using Microsoft.AspNetCore.Mvc;

namespace Eventnet.Api.Models.Events;

[ModelBinder(BinderType = typeof(FromBase64Binder))]
public record EventIdsListModel(Guid[] Ids);