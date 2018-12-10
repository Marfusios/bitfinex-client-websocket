using System;
using System.Collections.Generic;
using System.Linq;
using Bitfinex.Client.Websocket.Responses.Books;

namespace Bitmex.Client.Websocket.Sample.WinForms.Statistics
{
    class OrderBookStatsComputer
    {
        private readonly Dictionary<double, Book> _bids = new Dictionary<double, Book>();
        private readonly Dictionary<double, Book> _asks = new Dictionary<double, Book>();


        public void HandleOrderBook(Book book)
        {
            if (book.Count > 0)
            {
                InsertOrUpdateBook(book);
                return;
            }

            if (book.Count <= 0)
            {
                RemoveBook(book);
            }
        }

        public OrderBookStats GetStats()
        {
            var bids = _bids.OrderByDescending(x => x.Value.Price).ToArray();
            var asks = _asks.OrderBy(x => x.Value.Price).ToArray();

            if(!bids.Any() || !asks.Any())
                return OrderBookStats.NULL;

            var bidAmounts = bids.Take(20).Sum(x => Math.Abs(x.Value.Amount) * x.Value.Price);
            var askAmounts = asks.Take(20).Sum(x => Math.Abs(x.Value.Amount) * x.Value.Price);

            var total = bidAmounts + askAmounts + 0.0;

            var bidsPerc = bidAmounts / total * 100;
            var asksPerc = askAmounts / total * 100;

            return new OrderBookStats(
                bids[0].Value.Price,
                asks[0].Value.Price,
                bidsPerc,
                asksPerc,
                bidAmounts,
                askAmounts
                );
        }

        private void InsertOrUpdateBook(Book book)
        {
            var id = book.Price;

            if (book.Amount > 0)
            {
                _bids[id] = book;
                return;
            }

            _asks[id] = book;
        }

        private void RemoveBook(Book book)
        {
            var id = book.Price;
            if (_bids.ContainsKey(id))
                _bids.Remove(id);
            if (_asks.ContainsKey(id))
                _asks.Remove(id);
        }
    }

    class OrderBookStats
    {
        public static readonly OrderBookStats NULL = new OrderBookStats(0, 0, 0, 0, 0, 0);

        public OrderBookStats(double bid, double ask, double bidAmountPerc, double askAmountPerc, 
            double bidAmount, double askAmount)
        {
            Bid = bid;
            Ask = ask;
            BidAmountPerc = bidAmountPerc;
            AskAmountPerc = askAmountPerc;
            BidAmount = bidAmount;
            AskAmount = askAmount;
        }

        public double Bid { get; }
        public double Ask { get; }

        public double BidAmountPerc { get; }
        public double AskAmountPerc { get; }

        public double BidAmount { get; }
        public double AskAmount { get; }
    }
}
