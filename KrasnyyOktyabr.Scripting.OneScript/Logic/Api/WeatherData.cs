using ScriptEngine.Machine.Contexts;

namespace MyService.BusinessLogic.Api
{
    [ContextClass("ДанныеПогоды", "WeatherData")]
    public class WeatherData : AutoContext<WeatherData>
    {
        /// <summary>
        /// Температура воздуха. CanWrite=false говорит о том, что в BSL свойство будет только для чтения
        /// Можно не писать CanWrite и просто убрать сеттер set, но тогда свойство будет только для чтения и на уровне C#.
        /// </summary>
        [ContextProperty("Температура", "Temperature", CanWrite = false)]
        public decimal Temperature { get; set; }

        /// <summary>
        /// Влажность воздуха.
        /// </summary>
        [ContextProperty("Влажность", "Humidity", CanWrite = false)]
        public decimal Humidity { get; set; }

        /// <summary>
        /// Атмосферное давление
        /// </summary>
        [ContextProperty("Давление", "Pressure", CanWrite = false)]
        public decimal Pressure { get; set; }
    }
}
