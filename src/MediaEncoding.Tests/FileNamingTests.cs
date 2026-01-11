using System.IO;
using AwesomeAssertions;
using MediaEncoding;
using Xunit;

namespace MediaEncoding.Tests;

public class FileNamingTests
{
    [Fact]
    public void SanitizeFileName_RemovesAllInvalidChars()
    {
        var invalid = Path.GetInvalidFileNameChars();
        var input = "Start" + new string(invalid) + "End";

        var result = FileNaming.SanitizeFileName(input);

        result.Should().Be("StartEnd");
        foreach (var ch in invalid)
        {
            result.Should().NotContain(ch.ToString());
        }
    }

    [Theory]
    [InlineData("  Leading and trailing  ", "Leading and trailing")]
    public void SanitizeFileName_TrimsWhitespace(string input, string expected)
    {
        var result = FileNaming.SanitizeFileName(input);
        result.Should().Be(expected);
    }

    [Fact]
    public void RenameFile_IncludesSpaceBeforeSuffix()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, "test");
        var metadata = new Metadata { Title = "The Simpsons Movie", Year = 2007, Type = "movie" };
        var versionSuffix = " - title00";
        string? result = null;

        try
        {
            // Act
            result = FileNaming.RenameFile(tempFile, metadata, null, 1, versionSuffix);

            // Assert
            var filename = Path.GetFileName(result);
            filename.Should().Be("The Simpsons Movie (2007) - title00.mkv");
        }
        finally
        {
            // Cleanup
            if (result != null && File.Exists(result)) File.Delete(result);
        }
    }
}
