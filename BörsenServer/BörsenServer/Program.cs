using Apache.NMS.AMQP;
using Apache.NMS;
using System.Text.Json;
using System.Reflection;
using System.Timers;

namespace BörsenServer
{
    internal class Program
    {
        static Dictionary<string, string> responses = new Dictionary<string, string>();
        static Dictionary<string, double> stockPrices = new Dictionary<string, double>();
        static List<string> stocks = new List<string> { "AAPL", "MSFT", "AMZN", "NVDA", "AMD" };
        static Börse börse;
        static ManagementClient managementClient = new ManagementClient();
        readonly static Uri BROKER_URI = new Uri("amqp://localhost:5672");
        static System.Timers.Timer timer;
        static void Main(string[] args)
        {
            Console.WriteLine("Börsen Name:");
            string börsenName = Console.ReadLine();
            börse = new Börse(börsenName);
            SendStockPrices(null, null);
            SetTimer();
            managementClient.ListenForOrderRequests($"orders.{börse.Name.ToLower()}", OnOrderRequestReceived);
            Console.ReadKey();
        }

        // Funktion um die aktuellen Aktienkurse and das stockprices Topic zu senden
        private static void SendStockPrices(Object source, ElapsedEventArgs e)
        {
            GetStockPrices();
            WriteStockPrices();
            var stockPricesJson = JsonSerializer.Serialize(stockPrices);
            Console.WriteLine("Sending stock prices: " + stockPricesJson);
            managementClient.SendToStockPrices(BROKER_URI, stockPricesJson).Wait();
        }

        // Funktion um die live Preise für ausgewählte Aktien zu bekommen
        private static void GetStockPrices()
        {
            stocks.ForEach(stock => responses[stock] = börse.SendRequest(stock).Result);
        }

        // Funktion um die Responses im JSON Format zu den Aktien Preisen als Dictionary zu bekommen
        private static double DeserializePrice(string stock)
        {
            return JsonSerializer.Deserialize<Dictionary<string, double>>(responses[stock])["c"];
        }

        // Funktion um die local gespeicherten Aktienpreise zu aktuallisieren
        private static void WriteStockPrices()
        {
            stocks.ForEach(stock =>
                stockPrices[stock] = Math.Round(DeserializePrice(stock), 2));
        }

        // Funktion die einen Timer startet welcher alle 30 Sekunden die SendStockPrices Funktion ausführt
        static void SetTimer()
        {
            timer = new System.Timers.Timer(30000); // Triggers every 30 seconds
            timer.Elapsed += SendStockPrices;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        // Eventhandler um eingehende Order Requests zu verarbeiten
        private static void OnOrderRequestReceived(IMessage message)
        {
            if (message is ITextMessage textMessage)
            {
                var orderId = textMessage.NMSCorrelationID;
                var replyQueue = textMessage.NMSReplyTo;
                Console.WriteLine($"Received order: {textMessage.Text}");
                if (orderId == null) return;
                Thread.Sleep(15000);
                managementClient.SendOrderResponse(replyQueue, orderId);
                Console.WriteLine($"Sent response to {replyQueue}.");
            }
        }

        // Klasse welche eine Börse repräsentiert
        class Börse
        {
            readonly HttpClient client;
            public string Name { get; }
            public Börse(string name)
            {
                Name = name;
                client = new HttpClient();
            }

            public async Task<string> SendRequest(string aktie)
            {
                try
                {
                    // Send a GET request to the specified Uri as an asynchronous operation.
                    HttpResponseMessage response = await client.GetAsync($"https://finnhub.io/api/v1/quote?symbol={aktie}&token=cob38rpr01qr8aa3oi8gcob38rpr01qr8aa3oi90");
                    response.EnsureSuccessStatusCode(); // Throw if not a success code.

                    // Read response content as a string asynchronously.
                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                    return null;
                }
            }
        }
    }
}
