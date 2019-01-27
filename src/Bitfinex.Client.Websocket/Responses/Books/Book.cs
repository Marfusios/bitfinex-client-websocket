using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Responses.Configurations;
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


        internal static void Handle(JToken token, SubscribedResponse subscription, ConfigurationState config, 
            Subject<Book> subject, Subject<Book[]> subjectSnapshot, Subject<ChecksumResponse> subjectChecksum)
        {
            var data = token[1];

            if (config.IsChecksumEnabled)
            {
                if (data.Type == JTokenType.String && data.Value<string>() == "cs")
                {
                    ChecksumResponse.Handle(token, subscription, config, subjectChecksum);
                    return;
                }
            }
            
            if (data.Type != JTokenType.Array)
            {
                return; // heartbeat, ignore
            }

            if(data.First.Type == JTokenType.Array)
            {
                // initial snapshot
                Handle(token, data.ToObject<Book[]>(), subscription, config, subject, subjectSnapshot);
                return;
            }

            var book = data.ToObject<Book>();
            book.Pair = subscription.Pair;
            book.ChanId = subscription.ChanId;
            SetGlobalData(book, config, token);
            subject.OnNext(book);
        }

        internal static void Handle(JToken token, Book[] books, SubscribedResponse subscription, ConfigurationState config, 
            Subject<Book> subject, Subject<Book[]> subjectSnapshot)
        {
            foreach (var book in books)
            {
                book.Pair = subscription.Pair;
                book.ChanId = subscription.ChanId;
                SetGlobalData(book, config, token);

                // raise as normal book stream
                subject.OnNext(book);
            }

            // raise as snapshot book stream
            subjectSnapshot.OnNext(books);
        }
    }
}
