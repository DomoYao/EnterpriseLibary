﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using SignalR.Hubs;

namespace SignalR.StockTicker.SignalR.StockTicker
{
    public class StockTicker
    {
        private readonly static Lazy<StockTicker> _instance = new Lazy<StockTicker>(() => new StockTicker());
        private readonly static object _marketStateLock = new object();
        private readonly ConcurrentDictionary<string, Stock> _stocks = new ConcurrentDictionary<string, Stock>();
        private readonly double _rangePercent = .008; //stock can go up or down by a percentage of this factor on each change
        private readonly int _updateInterval = 250; //ms
        private Timer _timer;
        private readonly object _updateStockPricesLock = new object();
        private bool _updatingStockPrices = false;
        private readonly Random _updateOrNotRandom = new Random();
        private MarketState _marketState = MarketState.Closed;

        private StockTicker()
        {
            LoadDefaultStocks();
        }

        public static StockTicker Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        public MarketState MarketState
        {
            get { return _marketState; }
            private set { _marketState = value; }
        }

        public IEnumerable<Stock> GetAllStocks()
        {
            return _stocks.Values;
        }

        public void OpenMarket()
        {
            if (MarketState != MarketState.Open || MarketState != MarketState.Opening)
            {
                lock (_marketStateLock)
                {
                    if (MarketState != MarketState.Open || MarketState != MarketState.Opening)
                    {
                        MarketState = MarketState.Opening;
                        _timer = new Timer(UpdateStockPrices, null, _updateInterval, _updateInterval);
                        MarketState = MarketState.Open;
                        BroadcastMarketStateChange(MarketState.Open);
                    }
                }
            }
        }

        public void CloseMarket()
        {
            if (MarketState == MarketState.Open || MarketState == MarketState.Opening)
            {
                lock (_marketStateLock)
                {
                    if (MarketState == MarketState.Open || MarketState == MarketState.Opening)
                    {
                        MarketState = MarketState.Closing;
                        if (_timer != null)
                        {
                            _timer.Dispose();
                        }
                        MarketState = MarketState.Closed;
                        BroadcastMarketStateChange(MarketState.Closed);
                    }
                }
            }
        }

        public void Reset()
        {
            lock (_marketStateLock)
            {
                if (MarketState != MarketState.Closed)
                {
                    throw new InvalidOperationException("Market must be closed before it can be reset.");
                }
                _stocks.Clear();
                LoadDefaultStocks();
                BroadcastMarketStateChange(MarketState.Reset);
            }
        }

        private void LoadDefaultStocks()
        {
            new List<Stock>
            {
                new Stock { Symbol = "MSFT", Price = 26.31m },
                new Stock { Symbol = "APPL", Price = 404.18m },
                new Stock { Symbol = "GOOG", Price = 596.30m }
            }.ForEach(stock => _stocks.TryAdd(stock.Symbol, stock));
        }

        private void UpdateStockPrices(object state)
        {
            // This function must be re-entrant as it's running as a timer interval handler
            if (_updatingStockPrices)
            {
                return;
            }

            lock (_updateStockPricesLock)
            {
                if (!_updatingStockPrices)
                {
                    _updatingStockPrices = true;

                    foreach (var stock in _stocks.Values)
                    {
                        if (UpdateStockPrice(stock))
                        {
                            BroadcastStockPrice(stock);
                        }
                    }

                    _updatingStockPrices = false;
                }
            }
        }

        private bool UpdateStockPrice(Stock stock)
        {
            // Randomly choose whether to udpate this stock or not
            var r = _updateOrNotRandom.NextDouble();
            if (r > .1)
            {
                return false;
            }

            // Update the stock price by a random factor of the range percent
            var random = new Random((int)Math.Floor(stock.Price));
            var percentChange = random.NextDouble() * _rangePercent;
            var pos = random.NextDouble() > .51;
            var change = Math.Round(stock.Price * (decimal)percentChange, 2);
            change = pos ? change : -change;

            stock.Price += change;
            return true;
        }

        private static void BroadcastMarketStateChange(MarketState marketState)
        {
            var clients = Hub.GetClients<StockTickerHub>();
            switch (marketState)
            {
                case MarketState.Open:
                    clients.marketOpened();
                    break;
                case MarketState.Closed:
                    clients.marketClosed();
                    break;
                case MarketState.Reset:
                    clients.marketReset();
                    break;
                default:
                    break;
            }
        }

        public void WriterLog(string text, string fileName)
        {
            StreamWriter strW;
            string appPath = AppDomain.CurrentDomain.BaseDirectory + @"\Log";
            try
            {
                strW = new StreamWriter(appPath + @"\" + fileName + ".log", true);
                strW.WriteLine("时间：{0} 更新日志：{1} \r\n", DateTime.Now.ToString(), text);
                strW.Flush();
                strW.Close();
            }
            catch { }
        }

        private void BroadcastStockPrice(Stock stock)
        {
            Hub.GetClients<StockTickerHub>().updateStockPrice(stock);
        }

        ~StockTicker()
        {
            if (_timer != null)
            {
                _timer.Dispose();
            }
        }
    }

    public enum MarketState
    {
        Open,
        Opening,
        Closing,
        Closed,
        Reset
    }
}