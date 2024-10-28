using Confluent.Kafka;
using OneScript.Contexts;
using OneScript.StandardLibrary.Collections;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Contexts;
using System.Text;

namespace oscriptcomponent
{
    /// <summary>
	/// Класс КафкаПродюсер. Служит для отправки сообщений в топики Kafka.
	/// </summary>
    [ContextClass("КафкаПродюсер", "KafkaProducer")]
    public class KafkaProducer : AutoContext<KafkaProducer>
    {
        private readonly IProducer<string, string> _confluentProducer;

        /// <summary>
		/// СписокБрокеров.
		/// </summary>
		[ContextProperty("СписокБрокеров")]
        public string BrokerList { get; }

        public KafkaProducer(string brokerList)
        {
            BrokerList = brokerList;
            
            var confluentProducerConfig = new ProducerConfig { BootstrapServers = BrokerList };
            var confluentProducer = new ProducerBuilder<string, string>(confluentProducerConfig).Build();

            _confluentProducer = confluentProducer;
        }

        /// <summary>
		/// По умолчанию
		/// </summary>
        /// <param name="brokerList">СписокБрокеров</param>
		/// <returns>КафкаПродюсер</returns>
        [ScriptConstructor]
        public static KafkaProducer Constructor(string brokerList = "")
        {
            return new KafkaProducer(brokerList);
        }

        /// <summary>
        /// Отправить сообщение в топик 
        /// </summary>
        /// <param name="topic">Топик</param>
        /// <param name="key">Ключ</param>
        /// <param name="message">Сообщение</param>
        /// <param name="headers">Заголовки</param>
        [ContextMethod("Отправить", "Send")]
        public void Produce(string topic, string key, string message, MapImpl? headers = null)
        {
            var confluentHeaders = new Headers();

            if (headers != null)
            {
                foreach (var kv in headers)
                {
                    confluentHeaders.Add(kv.Key.AsString(), Encoding.ASCII.GetBytes(kv.Value.AsString()));
                }
            }

            var confluentMessage = new Message<string, string>
            {
                Key = key,
                Value = message,
                Headers = confluentHeaders
            };

            _confluentProducer.Produce(topic, confluentMessage);
        }
    }
}
