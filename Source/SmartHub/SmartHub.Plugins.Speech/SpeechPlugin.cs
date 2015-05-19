using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using SmartHub.Core.Plugins;
using SmartHub.Core.Plugins.Utils;
using SmartHub.Plugins.Scripts;
using SmartHub.Plugins.Scripts.Attributes;
using SmartHub.Plugins.Speech.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

//using Microsoft.Speech.Recognition;
//using Microsoft.Speech.Synthesis;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace SmartHub.Plugins.Speech
{
    [Plugin]
    public class SpeechPlugin : PluginBase
    {
        #region Fields
        private SpeechSynthesizer speechSynthesizer;
        private SpeechRecognitionEngine recognitionEngine;

        private DateTime? readyDate;
        private const string NAME = "эй кампьютэр";
        private const string RESPONSE_READY = "слушаю!";
        private const int READY_PERIOD = 15; // seconds
        private const float CONFIDENCE_LIMIT = 0.5f;
        #endregion

        #region Plugin overrides
        public override void InitDbModel(ModelMapper mapper)
        {
            mapper.Class<VoiceCommand>(cfg => cfg.Table("Speech_VoiceCommand"));
        }
        public override void InitPlugin()
        {
            InitSpeechSynthesizer();
            InitRecognitionEngine();
        }
        public override void StopPlugin()
        {
            CloseSpeechSynthesizer();
            CloseRecognitionEngine();
        }
        #endregion

        #region Private methods
        private string[] LoadAllCommands()
        {
            using (var session = Context.OpenSession())
            {
                List<string> list = session
                    .Query<VoiceCommand>()
                    .Select(cmd => cmd.CommandText)
                    .ToList();

                Logger.Info("Loaded commands: {0}", list.ToJson("[]"));

                list.Add(NAME);

                return list.ToArray();
            }
        }
        private VoiceCommand GetCommand(string text)
        {
            using (var session = Context.OpenSession())
            {
                var command = session.Query<VoiceCommand>().FirstOrDefault(x => x.CommandText == text);

                if (command != null)
                    Logger.Info("Loaded command: {0} (script: {1})", command.CommandText, command.UserScript.Name);

                return command;
            }
        }

        private void InitSpeechSynthesizer()
        {
            speechSynthesizer = new SpeechSynthesizer();
            speechSynthesizer.SetOutputToDefaultAudioDevice();

            speechSynthesizer.Rate = 1;
            speechSynthesizer.Volume = 100;

            var voiceList = speechSynthesizer.GetInstalledVoices();
            string voiceName = voiceList[0].VoiceInfo.Name;   //  в панели управления во вкладке распознавание речи выставить Ivona
            speechSynthesizer.SelectVoice(voiceName);
        }
        private void CloseSpeechSynthesizer()
        {
            if (speechSynthesizer != null)
            speechSynthesizer.Dispose();
        }

        private void InitRecognitionEngine()
        {
            var cultureInfo = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            /*
            •en-GB. English (United Kingdom)
            •en-US. English (United States)
            •de-DE. German (Germany)
            •es-ES. Spanish (Spain)
            •fr-FR. French (France)
            •ja-JP. Japanese (Japan)
            •zh-CN. Chinese (China)
            •zh-TW. Chinese (Taiwan)
            */

            var commands = LoadAllCommands();
            var choices = new Choices(commands);
            var builder = new GrammarBuilder(choices);
            builder.Culture = cultureInfo;

            recognitionEngine = new SpeechRecognitionEngine(cultureInfo);
            recognitionEngine.SetInputToDefaultAudioDevice();
            recognitionEngine.UnloadAllGrammars();
            recognitionEngine.LoadGrammar(new Grammar(builder));
            //recognitionEngine.LoadGrammar(new DictationGrammar()); // любой текст

            recognitionEngine.SpeechRecognized += recognitionEngine_SpeechRecognized;
            recognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
        }
        private void CloseRecognitionEngine()
        {
            if (recognitionEngine != null)
                recognitionEngine.Dispose();
        }
        #endregion

        #region Event handlers
        private void recognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            var commandText = e.Result.Text;
            Logger.Info("Command: {0} ({1:0.00})", commandText, e.Result.Confidence);

            if (e.Result.Confidence < CONFIDENCE_LIMIT)
            {
                Logger.Info("Apply confidence limit");
                return;
            }

            var now = DateTime.Now;
            var isInPeriod = readyDate.GetValueOrDefault() > now;

            if (commandText == NAME)
            {
                Logger.Info("Command is COMPUTER NAME");
                readyDate = now.AddSeconds(READY_PERIOD);
                Say(RESPONSE_READY);
            }
            else
            {
                if (isInPeriod)
                {
                    try
                    {
                        //Debugger.Launch();
                        var command = GetCommand(commandText);
                        Logger.Info("Command info loaded");

                        Context.GetPlugin<ScriptsPlugin>().ExecuteScript(command.UserScript);

                        this.RaiseScriptEvent(x => x.OnCommandReceivedForScripts, commandText);

                        readyDate = null;
                    }
                    catch (Exception ex)
                    {
                        var msg = string.Format("Voice command error: '{0}'", commandText);
                        Logger.ErrorException(msg, ex);
                    }
                }
            }
        }
        #endregion

        #region Script events
        [ScriptEvent("speech.commandReceived")]
        public ScriptEventHandlerDelegate[] OnCommandReceivedForScripts { get; set; }
        #endregion

        #region Script commands
        [ScriptCommand("say")]
        public void Say(string text)
        {
            //speechSynthesizer.Speak(text);
            speechSynthesizer.SpeakAsync(text);
        }
        #endregion
    }
}
