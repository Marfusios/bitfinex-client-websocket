using System.Reactive.Subjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Books
{
    /// <summary>
    /// The state of the Bitfinex order book
    /// </summary>
    [JsonConverter(typeof(BookConverter))]
    public class Book : ResponseBase
    {
        /// <summary>
        /// Price level
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Number of orders at that price level
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Total amount available at that price level. 
        /// Trading: if AMOUNT greater than 0 then bid else ask; 
        /// Funding: if AMOUNT lower than 0 then bid else ask;
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// Rate level
        /// </summary>
        public double Rate { get; set; }

        /// <summary>
        /// Period level
        /// </summary>
        public double Period { get; set; }

        /// <summary>
        /// Target pair
        /// </summary>
        [JsonIgnore]
        public string Pair { get; set; }


        internal static void Handle(JToken token, SubscribedResponse subscription, Subject<Book> subject, Subject<Book[]> subjectMulti)
        {
            var data = token[1];

            if (data.Type != JTokenType.Array)
            {
                return; // heartbeat, ignore
            }

            if(data.First.Type == JTokenType.Array)
            {
                // initial snapshot
                Handle(data.ToObject<Book[]>(), subscription, subject, subjectMulti);
                return;
            }

            var book = data.ToObject<Book>();
            book.Pair = subscription.Pair;
            book.ChanId = subscription.ChanId;
            subject.OnNext(book);
        }

        internal static void Handle(Book[] books, SubscribedResponse subscription, Subject<Book> subject, Subject<Book[]> subjectMulti)
        {
            foreach (var book in books)
            {
                book.Pair = subscription.Pair;
                book.ChanId = subscription.ChanId;

                // raise as normal book stream
                subject.OnNext(book);
            }

            // raise as snapshot book stream
            subjectMulti.OnNext(books);
        }
    }
}
