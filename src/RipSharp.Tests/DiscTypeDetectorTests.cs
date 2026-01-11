using System.Collections.Generic;
using RipSharp;
using Xunit;

namespace RipSharp.Tests;

public class DiscTypeDetectorTests
{
    [Fact]
    public void DetectContentType_SingleLongTitle_DetectsMovieWithHighConfidence()
    {
        var detector = new DiscTypeDetector();
        var disc = new DiscInfo
        {
            Titles = new List<TitleInfo>
            {
                new TitleInfo { Id = 0, DurationSeconds = 5400 } // 90 minutes
            }
        };

        var result = detector.DetectContentType(disc);

        Assert.False(result);
        Assert.True(detector.LastDetectionConfidence >= 0.9);
    }

    [Fact]
    public void DetectContentType_TvLikeDurations_DetectsTv()
    {
        var detector = new DiscTypeDetector();
        var disc = new DiscInfo
        {
            Titles = new List<TitleInfo>
            {
                new TitleInfo { Id = 0, DurationSeconds = 1450 },
                new TitleInfo { Id = 1, DurationSeconds = 1500 },
                new TitleInfo { Id = 2, DurationSeconds = 1520 },
                new TitleInfo { Id = 3, DurationSeconds = 1480 }
            }
        };

        var result = detector.DetectContentType(disc);

        Assert.True(result);
        Assert.True(detector.LastDetectionConfidence >= 0.75);
    }

    [Fact]
    public void DetectContentType_MixedLongAndShort_TreatedAsMovie()
    {
        var detector = new DiscTypeDetector();
        var disc = new DiscInfo
        {
            Titles = new List<TitleInfo>
            {
                new TitleInfo { Id = 0, DurationSeconds = 5500 },
                new TitleInfo { Id = 1, DurationSeconds = 400 },
                new TitleInfo { Id = 2, DurationSeconds = 500 },
                new TitleInfo { Id = 3, DurationSeconds = 300 }
            }
        };

        var result = detector.DetectContentType(disc);

        Assert.False(result);
        Assert.True(detector.LastDetectionConfidence >= 0.7);
    }

    [Fact]
    public void DetectContentType_UncertainMixedDurations_ReturnsNull()
    {
        var detector = new DiscTypeDetector();
        var disc = new DiscInfo
        {
            Titles = new List<TitleInfo>
            {
                new TitleInfo { Id = 0, DurationSeconds = 1800 },
                new TitleInfo { Id = 1, DurationSeconds = 2400 },
                new TitleInfo { Id = 2, DurationSeconds = 4200 }
            }
        };

        var result = detector.DetectContentType(disc);

        Assert.Null(result);
        Assert.True(detector.LastDetectionConfidence <= 0.8);
    }

    [Fact]
    public void DetectContentType_LowConfidenceMovie_BelowPromptThreshold()
    {
        var detector = new DiscTypeDetector();
        // Create scenario with mixed durations that should give low confidence
        var disc = new DiscInfo
        {
            Titles = new List<TitleInfo>
            {
                new TitleInfo { Id = 0, DurationSeconds = 3000 }, // 50 min
                new TitleInfo { Id = 1, DurationSeconds = 600 },   // 10 min
                new TitleInfo { Id = 2, DurationSeconds = 700 }    // 11 min
            }
        };

        var result = detector.DetectContentType(disc);

        // Should detect as movie but with low confidence (below 70% threshold)
        Assert.False(result);
        Assert.True(detector.LastDetectionConfidence < 0.70, 
            $"Expected confidence < 0.70, but got {detector.LastDetectionConfidence}");
    }

    [Fact]
    public void DetectContentType_HighConfidenceMovie_AbovePromptThreshold()
    {
        var detector = new DiscTypeDetector();
        // Single long title should give high confidence
        var disc = new DiscInfo
        {
            Titles = new List<TitleInfo>
            {
                new TitleInfo { Id = 0, DurationSeconds = 6000 } // 100 minutes
            }
        };

        var result = detector.DetectContentType(disc);

        // Should detect as movie with high confidence (above 70% threshold)
        Assert.False(result);
        Assert.True(detector.LastDetectionConfidence >= 0.70,
            $"Expected confidence >= 0.70, but got {detector.LastDetectionConfidence}");
    }

    [Fact]
    public void DetectContentType_HighConfidenceTv_AbovePromptThreshold()
    {
        var detector = new DiscTypeDetector();
        // Many similar-length episodes should give high confidence
        var disc = new DiscInfo
        {
            Titles = new List<TitleInfo>
            {
                new TitleInfo { Id = 0, DurationSeconds = 1440 }, // 24 min
                new TitleInfo { Id = 1, DurationSeconds = 1445 },
                new TitleInfo { Id = 2, DurationSeconds = 1438 },
                new TitleInfo { Id = 3, DurationSeconds = 1442 },
                new TitleInfo { Id = 4, DurationSeconds = 1446 }
            }
        };

        var result = detector.DetectContentType(disc);

        // Should detect as TV with high confidence (above 70% threshold)
        Assert.True(result);
        Assert.True(detector.LastDetectionConfidence >= 0.70,
            $"Expected confidence >= 0.70, but got {detector.LastDetectionConfidence}");
    }
}
