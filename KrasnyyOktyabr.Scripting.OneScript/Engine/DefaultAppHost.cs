/*----------------------------------------------------------
This Source Code Form is subject to the terms of the
Mozilla Public License, v.2.0. If a copy of the MPL
was not distributed with this file, You can obtain one
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System;
using ScriptEngine.HostedScript;
using ScriptEngine.HostedScript.Library;

namespace MyService.OneScriptBridge
{
    /// <summary>
    /// Реализация хоста по умолчанию. Ничего не умеет, кроме как пересылать сообщения в абстрактный IMessager
    /// </summary>
    public class DefaultAppHost : IHostApplication
    {
        private readonly Action<string, MessageStatusEnum> _messager;

        public DefaultAppHost(Action<string, MessageStatusEnum> messager)
        {
            _messager = messager;
        }

        public void Echo(string str, MessageStatusEnum status = MessageStatusEnum.Ordinary)
        {
            _messager(str, status);
        }

        public void ShowExceptionInfo(Exception exc)
        {
            _messager(exc.Message, MessageStatusEnum.VeryImportant);
        }

        public bool InputString(out string result, string prompt, int maxLen, bool multiline)
        {
            throw new NotSupportedException();
        }

        public string[] GetCommandLineArguments()
        {
            throw new NotSupportedException();
        }
    }
}