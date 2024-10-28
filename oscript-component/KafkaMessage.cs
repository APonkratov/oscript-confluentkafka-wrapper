using Confluent.Kafka;
using OneScript.Contexts;
using OneScript.StandardLibrary.Collections;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Contexts;
using System.Text;

namespace oscriptcomponent
{
    /// <summary>
	/// Класс СообщениеКафка. Является представлением сообщения Kafka.
	/// </summary>
    [ContextClass("СообщениеКафка", "KafkaMessage")]
    internal class KafkaMessage : AutoContext<KafkaMessage>
    {
        /// <summary>
		/// Ключ сообщения.
		/// </summary>
		[ContextProperty("Ключ")]
        public string Key { get; }

        /// <summary>
		/// Значение сообщения.
		/// </summary>
		[ContextProperty("Значение")]
        public string Value { get; }

        /// <summary>
		/// Заголовки сообщения.
		/// </summary>
		[ContextProperty("Заголовки")]
        public MapImpl Headers { get; }

        public KafkaMessage(Message<string, string> message)
        {
            Key = message.Key;
            Value = message.Value;
            Headers = new MapImpl();

            if (message.Headers.Count == 0) return;
            foreach (var header in message.Headers)
            {
	            Headers.Insert(ValueFactory.Create(header.Key), ValueFactory.Create(Encoding.ASCII.GetString(header.GetValueBytes())));
            }
        }

        public KafkaMessage(string value, string key = "", MapImpl? headers = null)
        {
            Key = key;
            Value = value;
            Headers = headers ?? new MapImpl();
        }

        /// <summary>
		/// По умолчанию
		/// </summary>
        /// <param name="value">Значение сообщения</param>
        /// <param name="key">Ключ сообщения</param>
        /// <param name="headers">Заголовки сообщения</param>
		/// <returns></returns>
        [ScriptConstructor]
        public static KafkaMessage Constructor(string value, string key = "", MapImpl? headers = null)
        {
            return new KafkaMessage(value, key, headers);
        }
    }
}
