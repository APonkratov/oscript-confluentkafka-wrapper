using Confluent.Kafka;
using OneScript.Contexts;
using OneScript.StandardLibrary.Collections;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Contexts;

namespace oscriptcomponent
{
    /// <summary>
	/// Класс КафкаКонсумер. Служит для чтения сообщений из топиков Kafka.
	/// </summary>
    [ContextClass("КафкаКонсумер", "KafkaConsumer")]
    internal class KafkaConsumer : AutoContext<KafkaConsumer>
    {
        private readonly IConsumer<string, string> _confluentConsumer;

        /// <summary>
		/// СписокБрокеров.
		/// </summary>
		[ContextProperty("СписокБрокеров")]
        public string BrokerList { get; }

        /// <summary>
        /// ГруппаПодписчиков.
        /// </summary>
        [ContextProperty("ГруппаПодписчиков")]
        public string GroupId { get; set; }

        /// <summary>
        /// Таймаут.
        /// </summary>
        [ContextProperty("Таймаут")]
        public int Timeout { get; set; }

        public KafkaConsumer(string brokerList, string groupId, MapImpl? properties = null)
        {
            BrokerList = brokerList;
            GroupId = groupId;
            Timeout = 3000;

            var configProperties = new Dictionary<string, string>
            {
                { "bootstrap.servers", brokerList },
                { "group.id", groupId }
            };

            if (properties != null)
            {
                foreach (var property in properties)
                {
                    configProperties.Add(property.Key.AsString(), property.Value.AsString());
                }
            }

            var confluentConsumerConfig = new ConsumerConfig(configProperties);
            var confluentConsumer = new ConsumerBuilder<string, string>(confluentConsumerConfig).Build();

            _confluentConsumer = confluentConsumer;
        }

        /// <summary>
		/// По умолчанию
		/// </summary>
        /// <param name="brokerList">Список брокеров</param>
        /// <param name="groupId">Группа подписчиков</param>
        /// <param name="properties">Параметры</param>
		/// <returns>КафкаКонсумер</returns>
        [ScriptConstructor]
        public static KafkaConsumer Constructor(string brokerList, string groupId, MapImpl? properties = null)
        {
            return new KafkaConsumer(brokerList, groupId, properties);
        }
        
        /// <summary>
		/// УстановитьТаймаут
		/// </summary>
        /// <param name="timeout">Таймаут</param>
		/// <returns>КафкаКонсумер</returns>
		[ContextMethod("УстановитьТаймаут", "SetTimeout")]
        public void SetTimeout(int timeout)
        {
            Timeout = timeout;
        }

        /// <summary>
        /// Подписаться 
        /// </summary>
        /// <param name="topic">Топик</param>
        [ContextMethod("Подписаться", "Subscribe")]
        public void Subscribe(string topic)
        {
            _confluentConsumer.Subscribe(topic);
        }

        /// <summary>
        /// Прочитать 
        /// </summary>
        [ContextMethod("Прочитать", "Consume")]
        public KafkaConsumeResult Consume()
        {
            var result = _confluentConsumer.Consume(Timeout);
            return new KafkaConsumeResult(result);
        }

        /// <summary>
        /// ЗафиксироватьСмещение 
        /// </summary>
        /// <param name="topic">Топик</param>
        /// <param name="partition">Раздел</param>
        /// <param name="offset">Смещение</param>
        [ContextMethod("ЗафиксироватьСмещение", "Commit")]
        public void Commit(string topic, int partition, int offset)
        {
            var confluentTopicPartitionOffsets = new TopicPartitionOffset[] { new(topic, new Partition(partition), new Offset(offset)) };
            _confluentConsumer.Commit(confluentTopicPartitionOffsets);
        }
    }
}
