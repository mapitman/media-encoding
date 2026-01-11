namespace RipSharp;

/// <summary>
/// Interface for detecting the type of content on a disc (movie vs TV series).
/// </summary>
public interface IDiscTypeDetector
{
    /// <summary>
    /// Detects whether a disc contains a movie or TV series based on its structure.
    /// </summary>
    /// <param name="discInfo">The disc information containing titles and metadata.</param>
    /// <returns>True if detected as TV series, false if detected as movie, null if detection is uncertain.</returns>
    bool? DetectContentType(DiscInfo discInfo);

    /// <summary>
    /// Gets the confidence score of the last detection (0.0 to 1.0).
    /// </summary>
    double LastDetectionConfidence { get; }
}
