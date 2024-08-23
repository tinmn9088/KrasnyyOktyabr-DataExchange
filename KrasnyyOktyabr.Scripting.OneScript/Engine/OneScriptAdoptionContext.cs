/*----------------------------------------------------------
This Source Code Form is subject to the terms of the
Mozilla Public License, v.2.0. If a copy of the MPL
was not distributed with this file, You can obtain one
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using ScriptEngine;
using ScriptEngine.HostedScript;
using ScriptEngine.HostedScript.Library;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Contexts;

namespace MyService.OneScriptBridge
{
    /// <summary>
    /// Замена типового глобального контекста. Отключает возможность запускать из скриптов внешние приложения
    /// Де-факто является копипастой стандартного класса SystemGlobalContext с удалением части методов.
    /// </summary>
    [GlobalContext(ManualRegistration = true)]
    public class OneScriptAdoptionContext : GlobalContextBase<OneScriptAdoptionContext>
    {
        public OneScriptAdoptionContext(ScriptingEngine engine, IHostApplication host)
        {
            EngineInstance = engine;
            ApplicationHost = host;
        }

        public IHostApplication ApplicationHost { get; }
        public ScriptingEngine EngineInstance { get; }


        [ContextMethod("Сообщить", "Message")]
        public void Echo(string message, MessageStatusEnum status = MessageStatusEnum.Ordinary)
        {
            ApplicationHost.Echo(message ?? "", status);
        }

        /// <summary>
        /// Подключает сторонний файл сценария к текущей системе типов.
        /// Подключенный сценарий выступает, как самостоятельный класс, создаваемый оператором Новый
        /// </summary>
        /// <param name="path">Путь к подключаемому сценарию</param>
        /// <param name="typeName">Имя типа, которое будет иметь новый класс. Экземпляры класса создаются оператором Новый. </param>
        /// <example>ПодключитьСценарий("C:\file.os", "МойОбъект");
        /// А = Новый МойОбъект();</example>
        [ContextMethod("ПодключитьСценарий", "AttachScript")]
        public void AttachScript(string path, string typeName)
        {
            var compiler = EngineInstance.GetCompilerService();
            EngineInstance.AttachedScriptsFactory.AttachByPath(compiler, path, typeName);
        }

        /// <summary>
        /// Создает экземпляр объекта на основании стороннего файла сценария.
        /// Загруженный сценарий возвращается, как самостоятельный объект. 
        /// Экспортные свойства и методы скрипта доступны для вызова.
        /// </summary>
        /// <param name="code">Текст сценария</param>
        /// <param name="externalContext">Структура. Глобальные свойства, которые будут инжектированы в область видимости загружаемого скрипта. (Необязательный)</param>
        /// <example>
        /// Контекст = Новый Структура("ЧислоПи", 3.1415); // 4 знака хватит всем
        /// ЗагрузитьСценарийИзСтроки("Сообщить(ЧислоПи);", Контекст);</example>
        [ContextMethod("ЗагрузитьСценарийИзСтроки", "LoadScriptFromString")]
        public IRuntimeContextInstance LoadScriptFromString(string code, StructureImpl externalContext = null)
        {
            var compiler = EngineInstance.GetCompilerService();
            if (externalContext == null)
                return EngineInstance.AttachedScriptsFactory.LoadFromString(compiler, code);
            else
            {
                var extData = new ExternalContextData();

                foreach (var item in externalContext)
                {
                    extData.Add(item.Key.AsString(), item.Value);
                }

                return EngineInstance.AttachedScriptsFactory.LoadFromString(compiler, code, extData);

            }
        }

        /// <summary>
        /// Создает экземпляр объекта на основании стороннего файла сценария.
        /// Загруженный сценарий возвращается, как самостоятельный объект. 
        /// Экспортные свойства и методы скрипта доступны для вызова.
        /// </summary>
        /// <param name="path">Путь к подключаемому сценарию</param>
        /// <param name="externalContext">Структура. Глобальные свойства, которые будут инжектированы в область видимости загружаемого скрипта. (Необязательный)</param>
        /// <example>
        /// Контекст = Новый Структура("ЧислоПи", 3.1415); // 4 знака хватит
        /// // В коде скрипта somescript.os будет доступна глобальная переменная "ЧислоПи"
        /// Объект = ЗагрузитьСценарий("somescript.os", Контекст);</example>
        [ContextMethod("ЗагрузитьСценарий", "LoadScript")]
        public IRuntimeContextInstance LoadScript(string path, StructureImpl externalContext = null)
        {
            var compiler = EngineInstance.GetCompilerService();
            if (externalContext == null)
                return EngineInstance.AttachedScriptsFactory.LoadFromPath(compiler, path);
            else
            {
                ExternalContextData extData = new ExternalContextData();

                foreach (var item in externalContext)
                {
                    extData.Add(item.Key.AsString(), item.Value);
                }

                return EngineInstance.AttachedScriptsFactory.LoadFromPath(compiler, path, extData);

            }
        }

        /// <summary>
        /// Приостанавливает выполнение скрипта.
        /// </summary>
        /// <param name="delay">Время приостановки в миллисекундах</param>
        [ContextMethod("Приостановить", "Sleep")]
        public void Sleep(int delay)
        {
            System.Threading.Thread.Sleep(delay);
        }

        /// <summary>
        /// Прерывает выполнение текущего скрипта.
        /// </summary>
        /// <param name="exitCode">Код возврата (ошибки), возвращаемый операционной системе.</param>
        [ContextMethod("ЗавершитьРаботу", "Exit")]
        public void Quit(int exitCode)
        {
            throw new ScriptInterruptionException(exitCode);
        }

        /// <summary>
        /// Явное освобождение ресурса через интерфейс IDisposable среды CLR.
        /// 
        /// OneScript не выполняет подсчет ссылок на объекты, а полагается на сборщик мусора CLR.
        /// Это значит, что объекты автоматически не освобождаются при выходе из области видимости. 
        /// 
        /// Метод ОсвободитьОбъект можно использовать для детерминированного освобождения ресурсов. Если объект поддерживает интерфейс IDisposable, то данный метод вызовет Dispose у данного объекта.
        /// 
        /// Как правило, интерфейс IDisposable реализуется различными ресурсами (файлами, соединениями с ИБ и т.п.)
        /// </summary>
        /// <param name="obj">Объект, ресурсы которого требуется освободить.</param>
        [ContextMethod("ОсвободитьОбъект", "FreeObject")]
        public void DisposeObject(IRuntimeContextInstance obj)
        {
            var disposable = obj as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// OneScript не выполняет подсчет ссылок на объекты, а полагается на сборщик мусора CLR.
        /// Это значит, что объекты автоматически не освобождаются при выходе из области видимости.
        /// 
        /// С помощью данного метода можно запустить принудительную сборку мусора среды CLR.
        /// Данные метод следует использовать обдуманно, поскольку вызов данного метода не гарантирует освобождение всех объектов.
        /// Локальные переменные, например, до завершения текущего метода очищены не будут,
        /// поскольку до завершения текущего метода CLR будет видеть, что они используются движком 1Script.
        /// 
        /// </summary>
        [ContextMethod("ВыполнитьСборкуМусора", "RunGarbageCollection")]
        public void RunGarbageCollection()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        [ContextMethod("КраткоеПредставлениеОшибки", "BriefErrorDescription")]
        public string BriefErrorDescription(ExceptionInfoContext errInfo)
        {
            return errInfo.Description;
        }

        [ContextMethod("ПодробноеПредставлениеОшибки", "DetailErrorDescription")]
        public string DetailErrorDescription(ExceptionInfoContext errInfo)
        {
            return errInfo.DetailedDescription;
        }

        [ContextMethod("ТекущаяУниверсальнаяДата", "CurrentUniversalDate")]
        public IValue CurrentUniversalDate()
        {
            return ValueFactory.Create(DateTime.UtcNow);
        }

        [ContextMethod("ТекущаяУниверсальнаяДатаВМиллисекундах", "CurrentUniversalDateInMilliseconds")]
        public long CurrentUniversalDateInMilliseconds()
        {
            return DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// Проверяет заполненность значения по принципу, заложенному в 1С:Предприятии
        /// </summary>
        /// <param name="inValue"></param>
        /// <returns></returns>
        [ContextMethod("ЗначениеЗаполнено", "ValueIsFilled")]
        public bool ValueIsFilled(IValue inValue)
        {
            var value = inValue?.GetRawValue();
            if (value == null)
            {
                return false;
            }
            if (value.DataType == DataType.Undefined)
                return false;
            else if (value.DataType == DataType.Boolean)
                return true;
            else if (value.DataType == DataType.String)
                return !string.IsNullOrWhiteSpace(value.AsString());
            else if (value.DataType == DataType.Number)
                return value.AsNumber() != 0;
            else if (value.DataType == DataType.Date)
            {
                var emptyDate = new DateTime(1, 1, 1, 0, 0, 0);
                return value.AsDate() != emptyDate;
            }
            else if (value is COMWrapperContext)
            {
                return true;
            }
            else if (value is ICollectionContext)
            {
                var col = value as ICollectionContext;
                return col.Count() != 0;
            }
            else if (ValueFactory.CreateNullValue().Equals(value))
            {
                return false;
            }
            else
                return true;

        }

        /// <summary>
        /// Заполняет одноименные значения свойств одного объекта из другого
        /// </summary>
        /// <param name="acceptor">Объект-приемник</param>
        /// <param name="source">Объект-источник</param>
        /// <param name="filledProperties">Заполняемые свойства (строка, через запятую)</param>
        /// <param name="ignoredProperties">Игнорируемые свойства (строка, через запятую)</param>
        [ContextMethod("ЗаполнитьЗначенияСвойств", "FillPropertyValues")]
        public void FillPropertyValues(IRuntimeContextInstance acceptor, IRuntimeContextInstance source, IValue filledProperties = null, IValue ignoredProperties = null)
        {
            string strFilled;
            string strIgnored;

            if (filledProperties == null || filledProperties.DataType == DataType.Undefined)
            {
                strFilled = null;
            }
            else if (filledProperties.DataType == DataType.String)
            {
                strFilled = filledProperties.AsString();
            }
            else
            {
                throw RuntimeException.InvalidArgumentType(3, nameof(filledProperties));
            }

            if (ignoredProperties == null || ignoredProperties.DataType == DataType.Undefined)
            {
                strIgnored = null;
            }
            else if (ignoredProperties.DataType == DataType.String)
            {
                strIgnored = ignoredProperties.AsString();
            }
            else
            {
                throw RuntimeException.InvalidArgumentType(4, nameof(ignoredProperties));
            }

            FillPropertyValuesStr(acceptor, source, strFilled, strIgnored);
        }

        public void FillPropertyValuesStr(IRuntimeContextInstance acceptor, IRuntimeContextInstance source, string filledProperties = null, string ignoredProperties = null)
        {
            IEnumerable<string> sourceProperties;

            if (filledProperties == null)
            {
                string[] names = new string[source.GetPropCount()];
                for (int i = 0; i < names.Length; i++)
                {
                    names[i] = source.GetPropName(i);
                }

                if (ignoredProperties == null)
                {
                    sourceProperties = names;
                }
                else
                {
                    IEnumerable<string> ignoredPropCollection = ignoredProperties.Split(',')
                        .Select(x => x.Trim())
                        .Where(x => x.Length > 0);

                    sourceProperties = names.Where(x => !ignoredPropCollection.Contains(x));
                }
            }
            else
            {
                sourceProperties = filledProperties.Split(',')
                    .Select(x => x.Trim())
                    .Where(x => x.Length > 0);

                // Проверка существования заявленных свойств
                foreach (var item in sourceProperties)
                {
                    acceptor.FindProperty(item); // бросает PropertyAccessException если свойства нет
                }
            }


            foreach (var srcProperty in sourceProperties)
            {
                try
                {
                    var srcPropIdx = source.FindProperty(srcProperty);
                    var accPropIdx = acceptor.FindProperty(srcProperty); // бросает PropertyAccessException если свойства нет

                    if (source.IsPropReadable(srcPropIdx) && acceptor.IsPropWritable(accPropIdx))
                        acceptor.SetPropValue(accPropIdx, source.GetPropValue(srcPropIdx));

                }
                catch (PropertyAccessException)
                {
                    // игнорировать свойства Источника, которых нет в Приемнике
                }
            }
        }
    }
}