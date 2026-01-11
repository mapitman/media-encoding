using System;
using System.Collections.Generic;
using System.Linq;

namespace RipSharp;

/// <summary>
/// Detects the type of content on a disc (movie vs TV series) based on structural analysis.
/// </summary>
public class DiscTypeDetector : IDiscTypeDetector
{
    private double _lastDetectionConfidence;

    /// <summary>
    /// Gets the confidence score of the last detection (0.0 to 1.0).
    /// </summary>
    public double LastDetectionConfidence => _lastDetectionConfidence;

    /// <summary>
    /// Detects whether a disc contains a movie or TV series based on its structure.
    /// Movies are characterized by:
    /// - Fewer, longer titles (typically 1-3 main titles)
    /// - Significant duration differences between titles
    /// - Clear separation (main feature vs bonus content)
    /// </summary>
    /// <param name="discInfo">The disc information containing titles and metadata.</param>
    /// <returns>True if detected as TV series, false if detected as movie, null if detection is uncertain.</returns>
    public bool? DetectContentType(DiscInfo discInfo)
    {
        _lastDetectionConfidence = 0.0;

        if (discInfo.Titles == null || discInfo.Titles.Count == 0)
            return null;

        // Single title is almost always a movie
        if (discInfo.Titles.Count == 1)
        {
            _lastDetectionConfidence = 0.95;
            return false;
        }

        // Two titles are likely a movie (main feature + bonus)
        if (discInfo.Titles.Count == 2)
        {
            var (isMovie, confidence) = AnalyzeTwoTitles(discInfo.Titles);
            _lastDetectionConfidence = confidence;
            return isMovie ? false : null; // Return false for movie, null for uncertain
        }

        // For 3+ titles, analyze duration consistency and patterns
        return AnalyzeMultipleTitles(discInfo.Titles);
    }

    /// <summary>
    /// Analyzes a disc with exactly two titles to determine if it's a movie.
    /// Typically: main feature + bonus content.
    /// </summary>
    private (bool IsMovie, double Confidence) AnalyzeTwoTitles(List<TitleInfo> titles)
    {
        if (titles.Count != 2)
            return (false, 0.0);

        var durations = titles.OrderByDescending(t => t.DurationSeconds).ToList();
        var longerDuration = durations[0].DurationSeconds;
        var shorterDuration = durations[1].DurationSeconds;

        // If longer title is significantly longer (at least 3x), likely movie + bonus
        if (longerDuration >= shorterDuration * 3)
        {
            return (true, 0.85);
        }

        // If both are substantial (> 30 min each) and similar, might be movie with alternate cut
        if (longerDuration > 1800 && shorterDuration > 1800)
        {
            var ratio = longerDuration / (double)shorterDuration;
            if (ratio < 1.3) // Within 30% of each other
                return (true, 0.75); // Likely alternate cuts of same movie
        }

        // Uncertain case
        return (false, 0.5);
    }

    /// <summary>
    /// Analyzes a disc with 3 or more titles.
    /// Movies typically have 1-2 titles; TV series have multiple similar-length titles.
    /// </summary>
    private bool? AnalyzeMultipleTitles(List<TitleInfo> titles)
    {
        if (titles.Count < 3)
            return null;

        var durations = titles.Select(t => t.DurationSeconds).OrderBy(d => d).ToList();
        
        // Calculate duration statistics
        var avgDuration = durations.Average();
        var minDuration = durations.First();
        var maxDuration = durations.Last();
        var durationVariance = CalculateVariance(durations);
        var durationStdDev = Math.Sqrt(durationVariance);

        // TV Series characteristics:
        // - Most episodes have similar duration (low standard deviation)
        // - Minimum and maximum are reasonably close (ratio < 1.5)
        // - All episodes > 15 minutes (typically 20-45 min for TV)

        var durationRatio = maxDuration / (double)Math.Max(minDuration, 1);

        // Filter out very short titles (likely bonus content or chapters)
        var substantialTitles = titles.Where(t => t.DurationSeconds > 900).ToList(); // > 15 min
        var shortTitles = titles.Where(t => t.DurationSeconds <= 900).ToList();

        // All titles are substantial and similar length -> TV series
        if (substantialTitles.Count >= 3 && shortTitles.Count == 0)
        {
            var substantialDurations = substantialTitles.Select(t => t.DurationSeconds).ToList();
            var substantialStdDev = Math.Sqrt(CalculateVariance(substantialDurations));
            var substantialAvg = substantialDurations.Average();
            var coefficientOfVariation = substantialStdDev / substantialAvg; // Lower = more consistent

            // TV episodes typically have CV < 0.15 (15% variation)
            if (coefficientOfVariation < 0.15)
            {
                _lastDetectionConfidence = 0.90;
                return true;
            }

            // CV between 0.15-0.25 suggests TV but less confident
            if (coefficientOfVariation < 0.25)
            {
                _lastDetectionConfidence = 0.70;
                return true;
            }
        }

        // Movie with bonus content: most titles are short, 1-2 are long
        if (shortTitles.Count >= 2 && substantialTitles.Count <= 2)
        {
            _lastDetectionConfidence = 0.80;
            return false;
        }

        // Uncertain
        _lastDetectionConfidence = 0.5;
        return null;
    }

    /// <summary>
    /// Calculates the variance of a collection of values.
    /// </summary>
    private double CalculateVariance(IEnumerable<int> values)
    {
        var valueList = values.ToList();
        if (valueList.Count == 0)
            return 0.0;

        var mean = valueList.Average();
        var squaredDiffs = valueList.Select(v => Math.Pow(v - mean, 2));
        return squaredDiffs.Average();
    }
}
