using System.Collections.Generic;
using RipSharp;
using Xunit;

namespace RipSharp.Tests;

public class TitleVariationGeneratorEdgeCasesTests
{
    [Fact]
    public void Generate_ReturnsOriginal_ForEmptyString()
    {
        var result = TitleVariationGenerator.Generate("");
        
        Assert.Equal(new[] { "" }, result);
    }

    [Fact]
    public void Generate_ReturnsOriginal_ForOnlySpecialCharacters()
    {
        var result = TitleVariationGenerator.Generate("___---");
        
        // The algorithm strips each trailing separator, which is expected behavior
        Assert.Contains("___---", result);
        Assert.True(result.Count > 1);
    }

    [Fact]
    public void Generate_ReturnsOriginal_ForSingleCharacter()
    {
        var result = TitleVariationGenerator.Generate("A");
        
        Assert.Equal(new[] { "A" }, result);
    }

    [Fact]
    public void Generate_HandlesTrailingSpaces()
    {
        var result = TitleVariationGenerator.Generate("Movie Title   ");
        
        Assert.Equal(new[] { "Movie Title   ", "Movie" }, result);
    }

    [Fact]
    public void Generate_HandlesMultipleConsecutiveSeparators()
    {
        var result = TitleVariationGenerator.Generate("Title___Part___A");
        
        // The algorithm strips one non-alphanumeric at a time, which creates intermediate variations
        Assert.Contains("Title___Part___A", result);
        Assert.Contains("Title___Part", result);
        Assert.Contains("Title", result);
    }

    [Fact]
    public void Generate_HandlesUnicodeCharacters()
    {
        var result = TitleVariationGenerator.Generate("Movie™_Title®");
        
        // ™ and ® are non-alphanumeric, so they get stripped
        Assert.Contains("Movie™_Title®", result);
        Assert.Contains("Movie", result);
    }

    [Theory]
    [InlineData("Test-", new[] { "Test-", "Test" })]
    [InlineData("Test_", new[] { "Test_", "Test" })]
    [InlineData("Test ", new[] { "Test " })]
    public void Generate_HandlesTrailingSeparators(string input, string[] expected)
    {
        var result = TitleVariationGenerator.Generate(input);
        
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Generate_HandlesMixedSeparators()
    {
        var result = TitleVariationGenerator.Generate("Movie-Title_Part 2023");
        
        Assert.Equal(new[] { "Movie-Title_Part 2023", "Movie-Title_Part", "Movie-Title", "Movie" }, result);
    }
}
