using Confluent.Kafka;
using OneScript.Contexts;
using ScriptEngine.Machine.Contexts;

namespace oscriptcomponent
{
    /// <summary>
	/// Класс РезультатЧтенияСообщения. Служит для чтения сообщений из топиков Kafka.
	/// </summary>
    [ContextClass("РезультатЧтенияСообщения", "KafkaConsumeResult")]
    internal class KafkaConsumeResult : AutoContext<KafkaConsumeResult>
    {
        /// <summary>
		/// Топик.
		/// </summary>
		[ContextProperty("Топик")]
        public string Topic { get; }

        /// <summary>
		/// Раздел.
		/// </summary>
		[ContextProperty("Раздел")]
        public int Partition { get; }

        /// <summary>
		/// Смещение.
		/// </summary>
		[ContextProperty("Смещение")]
        public int Offset { get; }

        /// <summary>
		/// ЭтоКонецРаздела. Признак конца раздела
		/// </summary>
		[ContextProperty("ЭтоКонецРаздела")]
        public bool IsPartitionEof { get; }

        /// <summary>
        /// Сообщение.
        /// </summary>
        [ContextProperty("Сообщение")]
        public KafkaMessage? Message { get; }

        public KafkaConsumeResult(ConsumeResult<string, string> consumeResult)
        {
            Topic = consumeResult.Topic;
            Partition = consumeResult.Partition;
            Offset = (int)consumeResult.Offset.Value;
            IsPartitionEof = consumeResult.IsPartitionEOF;
            if (!consumeResult.IsPartitionEOF)
            {
                Message = new KafkaMessage(consumeResult.Message);
            }
        }
    }
}
