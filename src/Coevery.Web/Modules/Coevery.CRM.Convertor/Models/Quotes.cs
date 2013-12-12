using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace Coevery.CRM.Convertor.Models {
    public class Quote {
        static readonly Random random = new Random();

        public string Symbol { get; set; }
        public Decimal Price { get; set; }
        public Decimal Change { get; set; }
        public Decimal DayMax { get; set; }
        public Decimal DayMin { get; set; }
        public DateTime LastUpdated { get; set; }

        public void Update() {
            if(LastUpdated.Day != DateTime.Now.Day) {
                DayMax = 0;
                DayMin = 0;
            }
            Change = (Decimal)((0.5 - random.NextDouble()) / 5.0);
            Decimal newPrice = Price + Price * Change;
            if(newPrice < 0) newPrice = 0;
            if(Price > 0)
                Change = (newPrice - Price) / Price;
            else
                Change = 0;
            Price = newPrice;
            LastUpdated = DateTime.Now;
            if(Price > DayMax || DayMax == 0)
                DayMax = Price;
            if(Price < DayMin || DayMin == 0)
                DayMin = Price;
        }
    }

    public class QuotesProvider {
        static string[] symbolsList = new string[] { "MSFT", "INTC", "CSCO", "SIRI", "AAPL", "HOKU", "ORCL", "AMAT", "YHOO", "LVLT", "DELL", "GOOG" };
        static string yahooUrl = "http://finance.yahoo.com/d/quotes.csv?s={0}&f=s0l1h0g0v0d1";
        static readonly Random random = new Random();

        static HttpSessionState Session {
            get { return HttpContext.Current.Session; }
        }

        public static List<Quote> GetQuotes() {
            if(Session["Quotes"] == null)
                Session["Quotes"] = LoadQuotes();
            var quotes = (List<Quote>)Session["Quotes"];
            UpdateQuotes(quotes);
            return quotes;
        }
        public static List<Quote> LoadQuotes() {
            var quotes = new List<Quote>();
            var url = string.Format(yahooUrl, string.Join("+", symbolsList));
            var request = HttpWebRequest.Create(url);
            using(var stream = request.GetResponse().GetResponseStream()) {
                using(var reader = new StreamReader(stream, Encoding.UTF8)) {
                    while(!reader.EndOfStream) {
                        var values = reader.ReadLine().Replace("\"", "").Split(new char[] { ',' });
                        Quote quote = new Quote();
                        quote.Symbol = values[0].Trim();
                        Decimal value;
                        if(Decimal.TryParse(values[1], out value))
                            quote.Price = value;
                        else
                            quote.Price = 0;
                        if(Decimal.TryParse(values[2], out value))
                            quote.DayMax = value;
                        else
                            quote.DayMax = 0;
                        if(Decimal.TryParse(values[3], out value))
                            quote.DayMin = value;
                        else
                            quote.DayMin = 0;
                        DateTime date;
                        if(DateTime.TryParse(values[4], out date))
                            quote.LastUpdated = date;
                        else
                            quote.LastUpdated = DateTime.Now;
                        quotes.Add(quote);
                    }

                }
            }
            return quotes;
        }

        private static void UpdateQuotes(List<Quote> quotes) {
            foreach(Quote quote in quotes) {
                if(random.NextDouble() >= 0.7)
                    quote.Update();
            }
        }
    }
}