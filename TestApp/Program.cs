
// Исполняемое приложение для запуска компоненты под отладчиком

// В проекте TestApp в "Ссылки" ("References") должен быть добавлен проект компоненты
// В проекте TestApp должны быть подключены NuGet пакеты OneScript, OneScript.Hosting и OneScript.StandardLibrary

using System;
using OneScript.StandardLibrary;
using OneScript.StandardLibrary.Collections;
using ScriptEngine.HostedScript;
using ScriptEngine.HostedScript.Extensions;
using ScriptEngine.Hosting;

namespace TestApp
{
    class MainClass : IHostApplication
    {

        static readonly string SCRIPT = @"
                // Отладочный скрипт
                // в котором уже подключена наша компонента
                КафкаПродюсер = Новый КафкаПродюсер(""events-bus-kafka-bootstrap-1s.k8s.qa.lamoda.tech:9094"");
                КафкаПродюсер.Отправить(""my-example-topic"", ""my-example-key"", ""my-important-message"");

                Заголовки = Новый Соответствие;
                Заголовки.Вставить(""Header1"", ""Value1"");
                Заголовки.Вставить(""Header2"", ""Value2"");
                КафкаПродюсер.Отправить(""my-example-topic"", ""my-example-key"", ""my-important-message"", Заголовки);
                
                // Чтение
                Параметры = Новый Соответствие;
                Параметры.Вставить(""auto.offset.reset"", ""earliest"");   
	            Параметры.Вставить(""enable.auto.commit"", ""false"");     
	            Параметры.Вставить(""enable.auto.offset.store"", ""false""); 
	            Параметры.Вставить(""enable.partition.eof"", ""true"");
                
                КафкаКонсумер = Новый КафкаКонсумер(""events-bus-kafka-bootstrap-1s.k8s.qa.lamoda.tech:9094"", ""my-group-id"", Параметры);
                КафкаКонсумер.Подписаться(""my-example-topic"");
                РезультатЧтения = КафкаКонсумер.Прочитать();

                Сообщить(РезультатЧтения.Топик);
                Сообщить(РезультатЧтения.Раздел);
                Сообщить(РезультатЧтения.Смещение);
                Сообщить(РезультатЧтения.Сообщение.Ключ);
                Сообщить(РезультатЧтения.Сообщение.Значение);

                // Фиксация смещения
                КафкаКонсумер.ЗафиксироватьСмещение(РезультатЧтения.Топик, РезультатЧтения.Раздел, РезультатЧтения.Смещение);
                "
            ;

        public static HostedScriptEngine StartEngine()
        {

            var builder = DefaultEngineBuilder.Create();
            builder.SetupConfiguration(providers => { });
            builder.SetDefaultOptions()
                .UseImports()
                .UseNativeRuntime()
                .UseFileSystemLibraries()
                ;

            var engine = builder.Build();

            // Регистрируем сборку по имени любого из стандартных классов движка
            engine.AttachAssembly(System.Reflection.Assembly.GetAssembly(typeof(ArrayImpl)));

            // Тут можно указать любой класс из компоненты
            engine.AttachExternalAssembly(System.Reflection.Assembly.GetAssembly(typeof(oscriptcomponent.KafkaProducer)));
            // Если проектов компонент несколько, то надо взять по классу из каждой из них
            // engine.AttachAssembly(System.Reflection.Assembly.GetAssembly(typeof(oscriptcomponent_2.MyClass_2)));
            // engine.AttachAssembly(System.Reflection.Assembly.GetAssembly(typeof(oscriptcomponent_3.MyClass_3)));

            var hostedScriptEngine = new HostedScriptEngine(engine);
            hostedScriptEngine.Initialize();

            return hostedScriptEngine;
        }

        public static void Main(string[] args)
        {

            var engine = StartEngine();
            var script = engine.Loader.FromString(SCRIPT);
            var process = engine.CreateProcess(new MainClass(), script);

            var result = process.Start(); // Запускаем наш тестовый скрипт

            Console.WriteLine("Result = {0}", result);

            // ВАЖНО: движок перехватывает исключения, для отладки можно пользоваться только точками останова.
        }

        public void Echo(string str, MessageStatusEnum status = MessageStatusEnum.Ordinary)
        {
            Console.WriteLine(str);
        }

        public void ShowExceptionInfo(Exception exc)
        {
            Console.WriteLine(exc.ToString());
        }

        public bool InputString(out string result, string prompt, int maxLen, bool multiline)
        {
            throw new NotImplementedException();
        }

        public string[] GetCommandLineArguments()
        {
            return new string[] { "1", "2", "3" }; // Здесь можно зашить список аргументов командной строки
        }
    }
}
