using Apache.NMS;
using Apache.NMS.AMQP;
using System;

//Hilfsklasse für die Kommunikation mit dem Message Broker
namespace BörsenServer
{
    public class ManagementClient
    {
        private IConnection connection;
        private ISession session;
        private IMessageProducer stockPriceProducer;
        private IMessageConsumer orderRequestListener;
        private IMessageProducer orderResponseProducer;

        private bool orderListenerActive = false;

        // Funktion um eine Textnachricht an das stockprices Topic zu senden
        public async Task SendToStockPrices(Uri brokerUri, string textMessage)
        {
            try
            {
                if (connection == null)
                {
                    var factory = new NmsConnectionFactory(brokerUri);
                    connection = await Task.Run(() => factory.CreateConnection("artemis", "artemis")); // Connect in a background thread
                    await Task.Run(() => connection.Start());
                }
                if(session == null)
                {
                    session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
                }
                IDestination destination = session.GetTopic("stockprices");
                stockPriceProducer = session.CreateProducer(destination);
                ITextMessage message = stockPriceProducer.CreateTextMessage(textMessage);
                await Task.Run(() => stockPriceProducer.Send(message));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to message broker: " + ex.Message);
            }
        }

        // Funktion um nach Order Requests zu lauschen
        public void ListenForOrderRequests(string targetQueueName, MessageListener onResponseReceived)
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

                orderRequestListener = session.CreateConsumer(queueDestination);

                orderRequestListener.Listener += new MessageListener(onResponseReceived);
                orderListenerActive = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending message to the queue: " + ex.Message);
            }
        }

        // Funktion um Responses zu den Orders zu senden
        public async Task SendOrderResponse(IDestination targetQueue, string orderId)
        {
            try
            {
                orderResponseProducer = session.CreateProducer(targetQueue);

                ITextMessage message = orderResponseProducer.CreateTextMessage();
                message.NMSCorrelationID = orderId.ToString();
                await Task.Run(() => orderResponseProducer.Send(message));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending message to the queue: " + ex.Message);
            }
        }
    }
}
