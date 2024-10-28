# OneScript Confluent.Kafka wrapper

[![Build status](https://ci.appveyor.com/api/projects/status/04fdarj9gfxw3il1/branch/main?svg=true)](https://ci.appveyor.com/project/APonkratov/oscript-confluentkafka-wrapper/branch/main)

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
