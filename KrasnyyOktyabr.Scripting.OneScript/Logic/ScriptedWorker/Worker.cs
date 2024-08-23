using ScriptEngine.Machine;
using ScriptEngine;
using ScriptEngine.Machine.Contexts;
using System;
using System.Collections.Generic;
using System.Text;
using OneScript.Language;
using MyService.BusinessLogic.Api;
using ScriptEngine.Environment;

namespace MyService.BusinessLogic.ScriptedWorker
{
    [ContextClass("Обработчик", "Worker")]
    public class Worker : AutoScriptDrivenObject<Worker>
    {
        private Worker(LoadedModule module) : base(module)
        {
        }

        /// <summary>
        /// Вызывает в скрипте метод ОбработатьПорциюДанных/ProcessDataPortion
        /// Метод должен иметь сигнатуру Функция ОбработатьПорциюДанных(СервисПогоды, Отказ)
        /// и возвращать булево
        /// </summary>
        /// <returns>Если true то обработка успешна, если false, то нет</returns>
        /// <exception cref="RuntimeException">Если произошла ошибка выполнения скрипта</exception>
        /// <exception cref="OperationCanceledException">Выбрасывает исключение, если в обработчике выставили Отказ = Истина</exception>
        public bool ProccessWeather()
        {
            var handlerId = GetScriptMethod("ОбработатьПорциюДанных", "ProcessDataPortion");
            if (handlerId == -1)
            {
                throw new RuntimeException("Не найден обработчик \"ОбработатьПорциюДанных\"(\"ProcessDataPortion\")");
            }

            var methodInfo = Module.Methods[handlerId];
            if (!methodInfo.Signature.IsFunction || methodInfo.Signature.Params.Length != 2)
            {
                throw new RuntimeException("Обработчик должен быть функцией с двумя параметрами");
            }

            if (methodInfo.Signature.Params[1].IsByValue)
            {
                throw new RuntimeException("Параметр Отказ не должен иметь признак Знач");
            }

            // Передаем в метод 2 параметра. Второй - выходной параметр "Отказ"
            var weatherService = new WeatherProvider();
            
            // Переменная Отказ должна быть типом "Переменная" и передаваться по ссылке, чтобы
            // в нее можно было записать значение в скрипте и получить на уровне C#
            var cancelArg = Variable.Create(ValueFactory.Create(false), "Отказ");

            var returned = CallScriptMethod(handlerId, new[] { weatherService });
            if (returned.DataType != DataType.Boolean)
            {
                throw new RuntimeException("Обработчик должен вернуть Булево");
            }

            // Проверка на Отказ. Если в переменной Истина - ее установили в скрипте через Отказ = Истина
            if (cancelArg.Value.AsBoolean())
            {
                throw new OperationCanceledException();
            }

            // Универсальный конвертер из bsl-значения в значение C#
            // В данном случае можно не использовать, т.к. тип точно уже булево, проверен выше.
            //var marshalledBool = ContextValuesMarshaller.ConvertReturnValue(returned);

            return returned.AsBoolean();
        }

        /// <summary>
        /// Создать из файлового скрипта
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="processingScript"></param>
        /// <returns></returns>
        public static Worker Create(ScriptingEngine engine, string processingScript)
        {
            var code = engine.Loader.FromString(processingScript);
            return Create(engine, code);
        }

        /// <summary>
        /// Создать из скрипта, который взялся откуда-то еще
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static Worker Create(ScriptingEngine engine, ICodeSource code)
        {
            var compiler = engine.GetCompilerService();

            // Можно добавить символы, видимые компилятору в рамках именно этого модуля
            // Наследование от AutoScriptDrivenObject автоматически добавляет свойства и методы класса
            //compiler.DefineMethod();
            //compiler.DefineVariable();
            //compiler.DefinePreprocessorValue();
            
            var module = CompileModule(compiler, code);
            var loadedModule = engine.LoadModuleImage(module);

            return new Worker(loadedModule);
        }
    }
}
