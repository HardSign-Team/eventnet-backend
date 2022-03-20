using System;
using Eventnet.Domain.Selectors;
using FluentAssertions;
using NUnit.Framework;

namespace Eventnet.Domain.UnitTests;

public class TagsStartsWithNameSelectorTests
{
    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void Constructor_ShouldThrowException_WhenNameIncorrect(string name)
    {
        Assert.Throws<ArgumentException>(() => CreateSut(name));
    }
    
    [Test]
    public void Select_ShouldReturnEmptyCollection_WhenQueryEmpty()
    {
        var sut = CreateSut("A");
        var store = Array.Empty<TagName>();

        var result = sut.Select(store, 10);

        result.Should().HaveCount(0);
    }

    [Test]
    public void Select_ShouldReturnNonEmptyCollection_WhenFoundSimilarTags()
    {
        var sut = CreateSut("A");
        var store = new[]
        {
            new TagName(0, "A"),
            new TagName(1, "Aa"),
            new TagName(2, "bAa"),
        };

        var result = sut.Select(store, 10);

        result.Should().Contain(new[] { store[0], store[1] });
    }
    
    [Test]
    public void Select_ShouldBeCaseIndependent()
    {
        var sut = CreateSut("Aa");
        var store = new[]
        {
            new TagName(0, "AA"),
            new TagName(0, "aA"),
            new TagName(0, "Aa"),
            new TagName(0, "aa"),
        };

        var result = sut.Select(store, store.Length);

        result.Should().Contain(store);
    }


    [Test]
    public void Select_ShouldReturnNonEmptyCollection_AndCountLessThanMax_WhenFoundTooManySimilarTags()
    {
        const int maxCount = 3;
        var sut = CreateSut("A");
        var store = new[]
        {
            new TagName(0, "Aa"),
            new TagName(1, "Aaa"),
            new TagName(2, "Aaaa"),
            new TagName(3, "Aaaaa"),
            new TagName(4, "Aaaaaa"),
        };

        var result = sut.Select(store, maxCount);

        result.Should().HaveCount(maxCount);
    }
    
    private static TagsStartsWithNameSelector CreateSut(string name)
    {
        return new TagsStartsWithNameSelector(name);
    }
}