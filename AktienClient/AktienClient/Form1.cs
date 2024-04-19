using Apache.NMS;
using System.Text.Json;

namespace UserClient
{
    public partial class Form1 : Form
    {
        ManagementClient managementClient;
        Guid clientId = Guid.NewGuid();
        public static string stockExchange = "";
        private List<Stock> stocks = new List<Stock>()
        {
            new Stock("AAPL"),
            new Stock("MSFT"),
            new Stock("AMZN"),
            new Stock("NVDA"),
            new Stock("AMD")
        };
        private List<TextBox> textBoxList = new List<TextBox>();
        private List<Button> buyButtonList = new List<Button>();
        private List<Button> sellButtonList = new List<Button>();
        private int Offset { get { return textBoxList.Count; } }
        private List<Order> orders = new List<Order>();

        public Form1()
        {
            InitializeComponent();
            managementClient = new ManagementClient();
            Uri uri = new Uri("amqp://localhost:5672");
            managementClient.ListenToStockPrices(uri, "stockprices", OnPricesReceived);
        }

        // Event handler um die Preise der Aktien zu aktualisieren
        private void OnPricesReceived(IMessage message)
        {
            if (message is ITextMessage textMessage)
            {
                try
                {
                    var stockPrices = JsonSerializer.Deserialize<Dictionary<string, double>>(textMessage.Text);
                    if (stockPrices == null) return;

                    this.Invoke((MethodInvoker)delegate
                    {
                        textBoxList.ForEach(textbox =>
                        {
                            string price = stockPrices[textbox.Name].ToString();
                            textbox.Text = textbox.Name + ": " + price;
                        });

                        stocks.ForEach(stock => stock.Price = stockPrices[stock.Name]);
                    });
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error deserializing message: " + textMessage.Text);
                    return;
                }
            }
        }

        // Event handler um die Antwortnachrichten der Order Queue zu verarbeiten
        private void OnOrderResponseReceived(IMessage message)
        {
            if (message is ITextMessage textMessage)
            {
                var orderId = textMessage.NMSCorrelationID;
                if (orderId == null) return;

                orders.ForEach(o =>
                    {
                        if (o.Id.ToString() == orderId)
                        {
                            o.Status = "successful";
                        }
                    });

                this.Invoke((MethodInvoker)delegate
                {
                    updateOrderList();
                });
            }
        }

        // Event handler um eine Aktie zu kaufen
        private void OnBuy(object sender, EventArgs e) 
        { 
            OrderTransaction(sender, "buy");
        }

        // Event handler um eine Aktie zu verkaufen
        private void OnSell(object sender, EventArgs e)
        {
            OrderTransaction(sender, "sell");
        }

        // Funktion um eine Order zu erstellen und an die Order Queue zu senden
        private async void OrderTransaction(object sender, string buyOrSell)
        {
            try
            {
                if (stockExchange == "")
                {
                    MessageBox.Show("Please select a stock exchange first.");
                    return;
                }
                var button = (Button)sender;
                string aktienName = button.Name;
                var stock = stocks.Find(s => s.Name == aktienName);
                var order = new Order(stock.Name, stock.Price, buyOrSell);
                await managementClient.SendOrderToQueueAsync($"orders.{order.StockExchange.ToLower()}", clientId.ToString(), order.Id, order.ToString());
                orders.Add(order);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error selling stock: " + ex.Message);
            }
            updateOrderList();
        }

        // Funktion um die Orderliste in der UI zu aktualisieren
        private void updateOrderList()
        {
            string text = "";
            orders.ForEach(order => text += order.ToString());
            transactionsDisplay.Text = text;
        }

        // Event handler um die Auswahl der Börse zu speichern
        private void onRadioButtonClick(object sender, EventArgs e) 
        {
            var radioButton = (RadioButton)sender;
            stockExchange = radioButton.Text;
        }

        // Event handler um die ausgewählten Aktien zu aktualisieren
        private void button1_Click(object sender, EventArgs e)
        {
            updateControls();
            textBoxList.ForEach(textbox =>
            {
                var stock = this.stocks.Find(s => s.Name == textbox.Name);
                string price = stock?.Price == 0 ? "Waiting for update" : stock.Price.ToString();
                textbox.Text = textbox.Name + ": " + price;
            });
            managementClient.ListenForOrderResponses(clientId.ToString(), OnOrderResponseReceived);
        }

        // Event handler um das Formular zu schließen
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            managementClient.Close();
        }

        // Funktion um die Aktienliste in der UI zu aktualisieren
        private void updateControls()
        {
            ResetControls();
            foreach (var item in checkedListBox1.CheckedItems)
            {
                addStockTrackerAndButton(item.ToString());
            }
        }

        // Funktion um eine Aktie und die dazugehörigen Buttons in der UI hinzuzufügen
        private void addStockTrackerAndButton(String stockname)
        {
            var textbox = new TextBox();
            var buyButton = new Button();
            var sellButton = new Button();

            //Textbox
            textbox.Location = new Point(288, 56 + Offset * 29);
            textbox.Size = new Size(260, 35);
            textbox.Text = stockname+ ":";
            textbox.Name = stockname;
            textbox.ReadOnly = true;

            //buyButton
            buyButton.Location = new Point(549, 56 + Offset * 29);
            buyButton.Size = new Size(131, 35);
            buyButton.Text = "Buy";
            buyButton.UseVisualStyleBackColor = true;
            buyButton.Name = stockname;
            buyButton.Click += new EventHandler(OnBuy);

            //buyButton
            sellButton.Location = new Point(681, 56 + Offset * 29);
            sellButton.Size = new Size(131, 35);
            sellButton.Text = "Sell";
            sellButton.UseVisualStyleBackColor = true;
            sellButton.Name = stockname;
            sellButton.Click += new EventHandler(OnSell);

            this.AddControl(textbox);
            this.AddControl(buyButton);
            this.AddControl(sellButton);
            textBoxList.Add(textbox);
            buyButtonList.Add(buyButton);
            sellButtonList.Add(sellButton);

        }

        // Funktion um alle Aktien und Buttons in der UI zu entfernen
        private void ResetControls()
        {
            for (int i = 0; i < textBoxList.Count; i++)
            {
                this.RemoveControl(textBoxList[i]);
                this.RemoveControl(buyButtonList[i]);
                this.RemoveControl(sellButtonList[i]);
            }
            textBoxList = new List<TextBox>();
            buyButtonList = new List<Button>();
            sellButtonList = new List<Button>();
        }

        // Hilfsfunktion um UI Elemente hinzuzufügen
        public void AddControl(Control control)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => AddControl(control)));
            }
            else
            {
                this.Controls.Add(control);
            }
        }

        // Hilfsfunktion um UI Elemente zu entfernen
        public void RemoveControl(Control control)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => RemoveControl(control)));
            }
            else
            {
                this.Controls.Remove(control);
                control.Dispose();
            }
        }
    }

    // Klasse um eine Aktie zu repräsentieren
    internal class Stock
    {
        public string Name { get; }
        public double Price { get; set; }
        public Stock(string name)
        {
            Name = name;
        }
    }

    // Klasse um eine Order zu repräsentieren
    class Order
    {
        public Guid Id { get;}
        public string StockName { get; set; }
        public double Price { get; set; }
        public string Status { get; set; }
        public string StockExchange { get; set; }
        public string Type { get; }

        public Order(string stockName, double price, string type)
        {
            Id = Guid.NewGuid();
            StockName = stockName;
            Price = price;
            Status = "pending";
            StockExchange = Form1.stockExchange;
            this.Type = type;
        }

        public override string ToString()
        {
            return $"Order ID: {Id}, Stock: {StockName}, Type: {Type}, Price: {Price}, Stock exchange: {StockExchange}, Status: {Status}\n";
        }
    }
}
