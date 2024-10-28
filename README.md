# OneScript Confluent.Kafka wrapper

## Поддержка Kafka для OneScript

## Установка

### Из хаба пакетов

`opm install ConfluentKafkaWrapper`

### Из релизов GitHub

...

## Использование

```bsl
#Использовать ConfluentKafkaWrapper

КафкаПродюсер = Новый КафкаПродюсер("my-example-host.local");
КафкаПродюсер.Отправить(""my-example-topic"", ""my-example-key"", ""my-important-message"");
```
