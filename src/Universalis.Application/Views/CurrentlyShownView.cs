﻿using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Universalis.Application.Views
{
    public class CurrentlyShownView
    {
        /*
         * Note for anyone viewing this file: People rely on the field order (even though JSON is defined to be unordered).
         * Please do not edit the field order unless it is unavoidable.
         */

        /// <summary>
        /// The item ID.
        /// </summary>
        [JsonPropertyName("itemID")]
        public uint ItemId { get; init; }

        /// <summary>
        /// The world ID, if applicable.
        /// </summary>
        [JsonPropertyName("worldID")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public uint? WorldId { get; init; }

        /// <summary>
        /// The last upload time for this endpoint, in milliseconds since the UNIX epoch.
        /// </summary>
        [JsonPropertyName("lastUploadTime")]
        public long LastUploadTimeUnixMilliseconds { get; set; }

        /// <summary>
        /// The currently-shown listings.
        /// </summary>
        [JsonPropertyName("listings")]
        public List<ListingView> Listings { get; set; } = new();

        /// <summary>
        /// The currently-shown sales.
        /// </summary>
        [JsonPropertyName("recentHistory")]
        public List<SaleView> RecentHistory { get; set; } = new();

        /// <summary>
        /// The DC name, if applicable.
        /// </summary>
        [JsonPropertyName("dcName")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string DcName { get; init; }

        /// <summary>
        /// The average listing price, with outliers removed beyond 3 standard deviations of the mean.
        /// </summary>
        [JsonPropertyName("currentAveragePrice")]
        public float CurrentAveragePrice { get; set; }

        /// <summary>
        /// The average NQ listing price, with outliers removed beyond 3 standard deviations of the mean.
        /// </summary>
        [JsonPropertyName("currentAveragePriceNQ")]
        public float CurrentAveragePriceNq { get; set; }

        /// <summary>
        /// The average HQ listing price, with outliers removed beyond 3 standard deviations of the mean.
        /// </summary>
        [JsonPropertyName("currentAveragePriceHQ")]
        public float CurrentAveragePriceHq { get; set; }

        /// <summary>
        /// The average number of sales per day, over the past seven days (or the entirety of the shown sales, whichever comes first).
        /// This number will tend to be the same for every item, because the number of shown sales is the same and over the same period.
        /// This statistic is more useful in historical queries.
        /// </summary>
        [JsonPropertyName("regularSaleVelocity")]
        public float SaleVelocity { get; init; }

        /// <summary>
        /// The average number of NQ sales per day, over the past seven days (or the entirety of the shown sales, whichever comes first).
        /// This number will tend to be the same for every item, because the number of shown sales is the same and over the same period.
        /// This statistic is more useful in historical queries.
        /// </summary>
        [JsonPropertyName("nqSaleVelocity")]
        public float SaleVelocityNq { get; init; }

        /// <summary>
        /// The average number of HQ sales per day, over the past seven days (or the entirety of the shown sales, whichever comes first).
        /// This number will tend to be the same for every item, because the number of shown sales is the same and over the same period.
        /// This statistic is more useful in historical queries.
        /// </summary>
        [JsonPropertyName("hqSaleVelocity")]
        public float SaleVelocityHq { get; init; }

        /// <summary>
        /// The average sale price, with outliers removed beyond 3 standard deviations of the mean.
        /// </summary>
        [JsonPropertyName("averagePrice")]
        public float AveragePrice { get; set; }

        /// <summary>
        /// The average NQ sale price, with outliers removed beyond 3 standard deviations of the mean.
        /// </summary>
        [JsonPropertyName("averagePriceNQ")]
        public float AveragePriceNq { get; set; }

        /// <summary>
        /// The average HQ sale price, with outliers removed beyond 3 standard deviations of the mean.
        /// </summary>
        [JsonPropertyName("averagePriceHQ")]
        public float AveragePriceHq { get; set; }

        /// <summary>
        /// The minimum listing price.
        /// </summary>
        [JsonPropertyName("minPrice")]
        public uint MinPrice { get; set; }

        /// <summary>
        /// The minimum NQ listing price.
        /// </summary>
        [JsonPropertyName("minPriceNQ")]
        public uint MinPriceNq { get; set; }

        /// <summary>
        /// The minimum HQ listing price.
        /// </summary>
        [JsonPropertyName("minPriceHQ")]
        public uint MinPriceHq { get; set; }

        /// <summary>
        /// The maximum listing price.
        /// </summary>
        [JsonPropertyName("maxPrice")]
        public uint MaxPrice { get; set; }

        /// <summary>
        /// The maximum NQ listing price.
        /// </summary>
        [JsonPropertyName("maxPriceNQ")]
        public uint MaxPriceNq { get; set; }

        /// <summary>
        /// The maximum HQ listing price.
        /// </summary>
        [JsonPropertyName("maxPriceHQ")]
        public uint MaxPriceHq { get; set; }

        /// <summary>
        /// A map of quantities to listing counts, representing the number of listings of each quantity.
        /// </summary>
        [JsonPropertyName("stackSizeHistogram")]
        public SortedDictionary<int, int> StackSizeHistogram { get; init; } = new();

        /// <summary>
        /// A map of quantities to NQ listing counts, representing the number of listings of each quantity.
        /// </summary>
        [JsonPropertyName("stackSizeHistogramNQ")]
        public SortedDictionary<int, int> StackSizeHistogramNq { get; init; } = new();

        /// <summary>
        /// A map of quantities to HQ listing counts, representing the number of listings of each quantity.
        /// </summary>
        [JsonPropertyName("stackSizeHistogramHQ")]
        public SortedDictionary<int, int> StackSizeHistogramHq { get; init; } = new();

        /// <summary>
        /// The world name, if applicable.
        /// </summary>
        [JsonPropertyName("worldName")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string WorldName { get; init; }

        /// <summary>
        /// The last upload times in milliseconds since epoch for each world in the response, if this is a DC request.
        /// </summary>
        [JsonPropertyName("worldUploadTimes")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<uint, long> WorldUploadTimes { get; set; }
    }
}