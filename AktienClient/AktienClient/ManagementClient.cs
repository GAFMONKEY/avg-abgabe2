using Apache.NMS;
using Apache.NMS.AMQP;

//Hilfsklasse für die Kommunikation mit dem Message Broker
public class ManagementClient
{
    private IConnection connection;
    private ISession session;
    private IMessageConsumer stockPriceConsumer;
    private IMessageProducer orderProducer;
    private IMessageConsumer orderListener;
    private bool orderListenerActive = false;
    
    // Verbindung zum Message Broker herstellen und auf Nachrichten auf dem stockprices topic lauschen
    public async Task ListenToStockPrices(Uri brokerUri, string topicName, MessageListener ml)
    {
        try
        {
            var factory = new NmsConnectionFactory(brokerUri);
            connection = await Task.Run(() => factory.CreateConnection("artemis", "artemis")); // Connect in a background thread
            await Task.Run(() => connection.Start());
            session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
            IDestination destination = session.GetTopic(topicName);
            stockPriceConsumer = session.CreateConsumer(destination);
            stockPriceConsumer.Listener += new MessageListener(ml);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error connecting to message broker: " + ex.Message);
        }
    }

    // Sendet die Order an die jeweilige Order Queue
    public async Task SendOrderToQueueAsync(string targetQueueName, string replyQueueName, Guid orderId, string orderString)
    {
        try
        {
            if (connection == null)
            {
                throw new InvalidOperationException("Connection has not been established.");
            }
            if (session == null)
            {
                session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
            }

            IDestination queueDestination = session.GetQueue(targetQueueName);
            IDestination replyQueueDestination = session.GetQueue(replyQueueName);
            orderProducer = session.CreateProducer(queueDestination);
            
            ITextMessage message = orderProducer.CreateTextMessage();
            message.NMSReplyTo = replyQueueDestination;
            message.NMSCorrelationID = orderId.ToString();
            message.Text = orderString;

            await Task.Run(() => orderProducer.Send(message));
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error sending message to the queue: " + ex.Message);
        }
    }

    // Lauscht auf die Antwortnachrichten der Order Queue
    public void ListenForOrderResponses(string targetQueueName, MessageListener onResponseReceived)
    {
        try
        {
            if (orderListenerActive) return;
            if (connection == null)
            {
                throw new InvalidOperationException("Connection has not been established.");
            }
            if (session == null)
            {
                session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
            }

            IDestination queueDestination = session.GetQueue(targetQueueName);

            orderListener = session.CreateConsumer(queueDestination);

            orderListener.Listener += new MessageListener(onResponseReceived);
            orderListenerActive = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error sending message to the queue: " + ex.Message);
        }
    }

    // Schließt alle offenen Verbindungen
    public void Close()
    {
        stockPriceConsumer?.Close();
        orderProducer?.Close();
        orderListener?.Close();
        session?.Close();
        connection?.Close();
    }
}
