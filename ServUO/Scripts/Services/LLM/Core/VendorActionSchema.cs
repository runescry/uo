using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Server.Services.LLM.Core
{
    /// <summary>
    /// Structured vendor action output format for LLM responses.
    /// Allows more reliable intent parsing than brittle tag markers.
    /// </summary>
    public class VendorActionResponse
    {
        /// <summary>
        /// The vendor action to perform.
        /// </summary>
        public VendorAction Action { get; set; } = VendorAction.None;

        /// <summary>
        /// Optional context/reasoning for the action (not used by code, but useful for debugging).
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Optional metadata (future extensibility).
        /// </summary>
        public VendorActionMetadata Metadata { get; set; }
    }

    /// <summary>
    /// Metadata for vendor actions (future extensibility).
    /// </summary>
    public class VendorActionMetadata
    {
        /// <summary>
        /// Optional item types referenced (e.g., for specific buy/sell intent).
        /// </summary>
        public string[] Items { get; set; }

        /// <summary>
        /// Optional confidence score (0-1) if the LLM supports it.
        /// </summary>
        public double? Confidence { get; set; }
    }

    /// <summary>
    /// Vendor action types (mirrors the legacy enum for compatibility).
    /// </summary>
    public enum VendorAction
    {
        None,
        Buy,
        Sell
    }

    /// <summary>
    /// Tolerant parser for vendor action responses.
    /// Attempts to parse structured JSON, falls back to legacy tag parsing.
    /// </summary>
    public static class VendorActionParser
    {
        /// <summary>
        /// Parse vendor action from LLM response text.
        /// Tries structured JSON first, then falls back to legacy tag parsing.
        /// </summary>
        public static VendorActionResponse Parse(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                return new VendorActionResponse { Action = VendorAction.None };

            // 1. Try structured JSON parsing
            var structured = TryParseStructured(response);
            if (structured != null)
                return structured;

            // 2. Fallback to legacy tag parsing
            var legacyAction = TryParseLegacyTags(response);
            return new VendorActionResponse { Action = legacyAction };
        }

        /// <summary>
        /// Attempt to extract and parse JSON from the response.
        /// Tolerant to surrounding text and markdown fences.
        /// </summary>
        private static VendorActionResponse TryParseStructured(string response)
        {
            // Look for JSON blocks (with or without markdown fences)
            string jsonBlock = ExtractJsonBlock(response);
            if (jsonBlock == null)
                return null;

            try
            {
                return ParseJsonManually(jsonBlock);
            }
            catch (Exception)
            {
                // If JSON parsing fails, try to extract just the action field
                return TryExtractActionField(jsonBlock);
            }
        }

        /// <summary>
        /// Manual JSON parsing since ServUO doesn't have System.Text.Json
        /// </summary>
        private static VendorActionResponse ParseJsonManually(string json)
        {
            var response = new VendorActionResponse();
            
            // Extract action field
            var actionMatch = Regex.Match(json, @"""action""\s*:\s*""?([^""\s,}]+)""?", RegexOptions.IgnoreCase);
            if (actionMatch.Success && Enum.TryParse<VendorAction>(actionMatch.Groups[1].Value, true, out var action))
            {
                response.Action = action;
            }

            // Extract reason field
            var reasonMatch = Regex.Match(json, @"""reason""\s*:\s*""([^""]*)""", RegexOptions.IgnoreCase);
            if (reasonMatch.Success)
            {
                response.Reason = reasonMatch.Groups[1].Value;
            }

            // Extract metadata items
            var itemsMatch = Regex.Match(json, @"""items""\s*:\s*\[(.*?)\]", RegexOptions.IgnoreCase);
            if (itemsMatch.Success)
            {
                var itemsString = itemsMatch.Groups[1].Value;
                var itemMatches = Regex.Matches(itemsString, @"""([^""]*)""");
                if (itemMatches.Count > 0)
                {
                    response.Metadata = new VendorActionMetadata();
                    response.Metadata.Items = itemMatches.Cast<Match>().Select(m => m.Groups[1].Value).ToArray();
                }
            }

            // Extract confidence
            var confidenceMatch = Regex.Match(json, @"""confidence""\s*:\s*([\d.]+)", RegexOptions.IgnoreCase);
            if (confidenceMatch.Success && double.TryParse(confidenceMatch.Groups[1].Value, out var confidence))
            {
                if (response.Metadata == null)
                    response.Metadata = new VendorActionMetadata();
                response.Metadata.Confidence = confidence;
            }

            return response;
        }

        /// <summary>
        /// Extract JSON block from response, tolerant to markdown fences.
        /// Made public for reuse in text cleaning.
        /// </summary>
        public static string ExtractJsonBlock(string response)
        {
            // Try markdown code fences first
            var fenceMatch = System.Text.RegularExpressions.Regex.Match(
                response,
                @"```(?:json)?\s*(\{.*?\})\s*```",
                System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (fenceMatch.Success)
                return fenceMatch.Groups[1].Value.Trim();

            // Try bare JSON object (first { to matching })
            var braceMatch = System.Text.RegularExpressions.Regex.Match(
                response,
                @"(\{[^{}]*(?:\{[^{}]*\}[^{}]*)*\})",
                System.Text.RegularExpressions.RegexOptions.Singleline);

            if (braceMatch.Success)
                return braceMatch.Groups[1].Value.Trim();

            return null;
        }

        /// <summary>
        /// Fallback: try to extract just the "action" field from malformed JSON.
        /// </summary>
        private static VendorActionResponse TryExtractActionField(string jsonBlock)
        {
            var actionMatch = System.Text.RegularExpressions.Regex.Match(
                jsonBlock,
                @"""action""\s*:\s*""?([^""\s,}]+)""?",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (actionMatch.Success && Enum.TryParse<VendorAction>(actionMatch.Groups[1].Value, true, out var action))
            {
                return new VendorActionResponse { Action = action };
            }

            return null;
        }

        /// <summary>
        /// Legacy tag parsing fallback.
        /// </summary>
        private static VendorAction TryParseLegacyTags(string response)
        {
            string lower = response.ToLower();

            // Check for vendor command markers (both brackets and parentheses)
            if (lower.Contains("[vendor_buy]") || lower.Contains("(vendor_buy)"))
                return VendorAction.Buy;
            if (lower.Contains("[vendor_sell]") || lower.Contains("(vendor_sell)"))
                return VendorAction.Sell;

            return VendorAction.None;
        }
    }
}
