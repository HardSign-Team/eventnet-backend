using AutoMapper;
using Eventnet.Api.Helpers;
using Eventnet.DataAccess;
using Eventnet.Domain.Selectors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eventnet.Api.Controllers;

[Route("api/tags")]
public class TagsController : Controller
{
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;

    public TagsController(ApplicationDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    [HttpGet("search/name/{name}")]
    public IActionResult GetTagsByName(string name, [FromQuery(Name = "mc")] int maxCount = 30)
    {
        maxCount = NumberHelper.Normalize(maxCount, 1, 30);

        if (string.IsNullOrWhiteSpace(name))
            return UnprocessableEntity("Expected name is non empty string");

        var selector = new TagsStartsWithNameSelector(name);
        var tagNames = mapper.ProjectTo<TagName>(context.Tags.AsNoTracking());
        var result = selector.Select(tagNames, maxCount);

        return Ok(result);
    }
}