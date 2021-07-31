﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Universalis.Application.Common;
using Universalis.Application.Views;
using Universalis.DataTransformations;
using Universalis.DbAccess.MarketBoard;
using Universalis.DbAccess.Queries.MarketBoard;
using Universalis.Entities.MarketBoard;
using Universalis.GameData;

namespace Universalis.Application.Controllers.V1
{
    [ApiController]
    [Route("api/history/{worldOrDc}/{itemIds}")]
    public class HistoryController : WorldDcControllerBase
    {
        private readonly IHistoryDbAccess _historyDb;

        public HistoryController(IGameDataProvider gameData, IHistoryDbAccess historyDb) : base(gameData)
        {
            _historyDb = historyDb;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string itemIds, string worldOrDc, [FromQuery(Name = "entries")] string entriesToReturn)
        {
            // Parameter parsing
            var itemIdsArray = InputProcessing.ParseIdList(itemIds)
                .Take(100)
                .ToArray();

            if (!TryGetWorldDc(worldOrDc, out var worldDc))
            {
                return NotFound();
            }

            if (!TryGetWorldIds(worldDc, out var worldIds))
            {
                return NotFound();
            }

            var entries = 1800;
            if (int.TryParse(entriesToReturn, out var queryEntries))
            {
                entries = Math.Min(Math.Max(0, queryEntries), 999999);
            }

            if (itemIdsArray.Length == 1)
            {
                var itemId = itemIdsArray[0];

                if (!GameData.MarketableItemIds().Contains(itemId))
                {
                    return NotFound();
                }

                var (_, historyView) = await GetHistoryView(worldDc, worldIds, itemId, entries);
                return Ok(historyView);
            }

            // Multi-item handling
            var historyViewTasks = itemIdsArray
                .Select(itemId => GetHistoryView(worldDc, worldIds, itemId, entries))
                .ToList();
            var historyViews = await Task.WhenAll(historyViewTasks);
            var unresolvedItems = historyViews
                .Where(hv => !hv.Item1)
                .Select(hv => hv.Item2.ItemId)
                .ToArray();
            return Ok(new HistoryMultiView
            {
                ItemIds = itemIdsArray,
                Items = historyViews
                    .Where(hv => hv.Item1)
                    .Select(hv => hv.Item2)
                    .ToList(),
                WorldId = worldDc.IsWorld ? worldDc.WorldId : null,
                WorldName = worldDc.IsWorld ? worldDc.WorldName : null,
                DcName = worldDc.IsDc ? worldDc.DcName : null,
                UnresolvedItemIds = unresolvedItems,
            });
        }

        protected async Task<(bool, HistoryView)> GetHistoryView(WorldDc worldDc, uint[] worldIds, uint itemId, int entries)
        {
            var data = (await _historyDb.RetrieveMany(new HistoryManyQuery
            {
                WorldIds = worldIds,
                ItemId = itemId,
            })).ToList();

            var resolved = data.Count > 0;

            var worlds = GameData.AvailableWorlds();

            var history = data.Aggregate(new HistoryView(), (agg, next) =>
            {
                // Handle undefined arrays
                next.Sales ??= new List<MinimizedSale>();

                agg.Sales = next.Sales
                    .Select(s => new MinimizedSaleView
                    {
                        Hq = s.Hq,
                        PricePerUnit = s.PricePerUnit,
                        Quantity = s.Quantity,
                        TimestampUnixSeconds = s.SaleTimeUnixSeconds,
                        WorldId = worldDc.IsDc ? next.WorldId : null,
                        WorldName = worldDc.IsDc ? worlds[next.WorldId] : null,
                    })
                    .Concat(agg.Sales)
                    .ToList();
                agg.LastUploadTimeUnixMilliseconds = Math.Max(next.LastUploadTimeUnixMilliseconds, agg.LastUploadTimeUnixMilliseconds);

                return agg;
            });

            history.Sales.Sort((a, b) => (int)b.TimestampUnixSeconds - (int)a.TimestampUnixSeconds);
            history.Sales = history.Sales.Take(entries).ToList();

            var nqSales = history.Sales.Where(s => !s.Hq).ToList();
            var hqSales = history.Sales.Where(s => s.Hq).ToList();
            return (resolved, new HistoryView
            {
                Sales = history.Sales,
                ItemId = itemId,
                WorldId = worldDc.IsWorld ? worldDc.WorldId : null,
                WorldName = worldDc.IsWorld ? worldDc.WorldName : null,
                DcName = worldDc.IsDc ? worldDc.DcName : null,
                LastUploadTimeUnixMilliseconds = history.LastUploadTimeUnixMilliseconds,
                StackSizeHistogram = new SortedDictionary<int, int>(Statistics.GetDistribution(history.Sales
                    .Select(s => s.Quantity)
                    .Select(q => (int)q))),
                StackSizeHistogramNq = new SortedDictionary<int, int>(Statistics.GetDistribution(nqSales
                    .Select(s => s.Quantity)
                    .Select(q => (int)q))),
                StackSizeHistogramHq = new SortedDictionary<int, int>(Statistics.GetDistribution(hqSales
                    .Select(s => s.Quantity)
                    .Select(q => (int)q))),
                SaleVelocity = Statistics.WeekVelocityPerDay(history.Sales
                    .Select(s => (long)s.TimestampUnixSeconds * 1000)),
                SaleVelocityNq = Statistics.WeekVelocityPerDay(nqSales
                    .Select(s => (long)s.TimestampUnixSeconds * 1000)),
                SaleVelocityHq = Statistics.WeekVelocityPerDay(hqSales
                    .Select(s => (long)s.TimestampUnixSeconds * 1000)),
            });
        }
    }
}
