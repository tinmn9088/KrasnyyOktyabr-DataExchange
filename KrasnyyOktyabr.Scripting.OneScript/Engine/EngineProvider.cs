/*----------------------------------------------------------
This Source Code Form is subject to the terms of the
Mozilla Public License, v.2.0. If a copy of the MPL
was not distributed with this file, You can obtain one
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using OneScript.DebugServices;
using ScriptEngine;
using ScriptEngine.HostedScript;
using ScriptEngine.HostedScript.Library;

namespace MyService.OneScriptBridge
{
    /// <summary>
    /// Основной инициализатор движка OneScript
    /// </summary>
    public static class EngineProvider
    {
        public static ScriptingEngine CreateEngine(IHostApplication hostApplication)
        {
            var engine = new ScriptingEngine();
            engine.Environment = new RuntimeEnvironment();

            engine.AttachAssembly(typeof(ArrayImpl).Assembly, engine.Environment);

            var symbols = new SymbolsContext();
        
            engine.Environment.InjectGlobalProperty(symbols, "Символы", true);
            engine.Environment.InjectGlobalProperty(symbols, "Symbols", true);

            var globalCtx = new OneScriptAdoptionContext(engine, hostApplication);
        
            engine.Environment.InjectObject(globalCtx, false);
            engine.AttachAssembly(typeof(EngineProvider).Assembly, engine.Environment);
            
            Locale.SystemLanguageISOName = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

            return engine;
        }

        public static void SetupDebugger(this ScriptingEngine engine, int debugPort, bool waitForConnect = false)
        {
            var server = new BinaryTcpDebugServer(debugPort);
            engine.DebugController = server.CreateDebugController();
            engine.DebugController.Init();
            engine.DebugController.AttachToThread();

            if (waitForConnect)
            {
                // Сразу же заморозить поток и ждать, пока не подключится IDE с отладкой
                engine.DebugController.Wait();
            }
        }
    }
}