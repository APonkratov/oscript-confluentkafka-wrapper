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
		/// Список брокеров.
		/// </summary>
		[ContextProperty("СписокБрокеров")]
        public string BrokerList { get; }

        /// <summary>
        /// Группа подписчиков.
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
            Timeout = 5000;

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
        /// <param name="properties">Параметры. Параметры для конфигурирования консумера</param>
		/// <returns>КафкаКонсумер</returns>
        [ScriptConstructor]
        public static KafkaConsumer Constructor(string brokerList, string groupId, MapImpl? properties = null)
        {
            return new KafkaConsumer(brokerList, groupId, properties);
        }
        
        /// <summary>
		/// Установить таймаут ожидания сообщения
		/// </summary>
        /// <param name="timeout">Таймаут. Таймаут ожидания в милисекундах</param>
		[ContextMethod("УстановитьТаймаут", "SetTimeout")]
        public void SetTimeout(int timeout)
        {
            Timeout = timeout;
        }

        /// <summary>
        /// Подписаться на топик 
        /// </summary>
        /// <param name="topic">Топик. Имя топика Kafka</param>
        [ContextMethod("Подписаться", "Subscribe")]
        public void Subscribe(string topic)
        {
            _confluentConsumer.Subscribe(topic);
        }

        /// <summary>
        /// Прочитать 
        /// </summary>
        /// <returns>РезультатЧтенияСообщения. Результат чтения сообщения или Неопределено, если не удалось прочитать</returns>
        [ContextMethod("Прочитать", "Consume")]
        public KafkaConsumeResult? Consume()
        {
            var confluentConsumeResult = _confluentConsumer.Consume(Timeout);
            return confluentConsumeResult == null ? null : new KafkaConsumeResult(confluentConsumeResult);
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
            var newOffset = offset += 1;
            var confluentTopicPartitionOffsets = new TopicPartitionOffset[] { new(topic, new Partition(partition), new Offset(newOffset)) };
            _confluentConsumer.Commit(confluentTopicPartitionOffsets);
        }
    }
}
