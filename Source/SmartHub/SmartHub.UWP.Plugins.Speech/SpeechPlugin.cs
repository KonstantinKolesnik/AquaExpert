using SmartHub.UWP.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Globalization;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml.Controls;

namespace SmartHub.UWP.Plugins.Speech
{
    //[AppSection("Голосовые команды", SectionType.System, "/webapp/speech/settings.js", "SmartHub.Plugins.Speech.Resources.js.settings.js", TileTypeFullName = "SmartHub.Plugins.Speech.SpeechTile")]
    //[JavaScriptResource("/webapp/speech/settings-view.js", "SmartHub.Plugins.Speech.Resources.js.settings-view.js")]
    //[JavaScriptResource("/webapp/speech/settings-model.js", "SmartHub.Plugins.Speech.Resources.js.settings-model.js")]
    //[HttpResource("/webapp/speech/settings.html", "SmartHub.Plugins.Speech.Resources.js.settings.html")]

    public class SpeechPlugin : PluginBase, IDisposable
    {
        #region Fields
        private bool disposed = false;
        private SpeechSynthesizer speechSynthesizer;
        private SpeechRecognizer speechRecognizer;
        private string languageTag = Language.CurrentInputMethodLanguageTag;

        private DateTime? readyDate;
        private const string NAME = "эй кампьютэр";
        private const string RESPONSE_READY = "слушаю!";
        private const int READY_PERIOD = 15; // seconds
        private const float CONFIDENCE_LIMIT = 0.5f;
        #endregion

        #region Plugin overrides
        //public override void InitDbModel(ModelMapper mapper)
        //{
        //    mapper.Class<VoiceCommand>(cfg => cfg.Table("Speech_VoiceCommand"));
        //}
        public override void InitPlugin()
        {
            languageTag = "ru-RU"; // "en-US", "ru-RU"

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
        private string[] GetCommandsText()
        {
            //using (var session = Context.OpenSession())
            //{
            //    List<string> list = session.Query<VoiceCommand>().Select(cmd => cmd.CommandText).ToList();

            //    //Logger.Info("Loaded commands: {0}", list.ToJson("[]"));

            //    list.Add(NAME);

            //    return list.ToArray();
            //}

            return new List<string>().ToArray();
        }
        //private VoiceCommand GetCommand(string text)
        //{
        //    using (var session = Context.OpenSession())
        //    {
        //        var command = session.Query<VoiceCommand>().FirstOrDefault(x => x.CommandText == text);

        //        if (command != null)
        //            Logger.Info("Loaded command: {0} (script: {1})", command.CommandText, command.UserScript.Name);

        //        return command;
        //    }
        //}
        //private List<VoiceCommand> GetCommands()
        //{
        //    using (var session = Context.OpenSession())
        //        return session.Query<VoiceCommand>()
        //            .OrderBy(cmd => cmd.CommandText)
        //            .ToList();
        //}
        //private object BuildCommandWebModel(VoiceCommand cmd)
        //{
        //    if (cmd == null)
        //        return null;

        //    return new
        //    {
        //        Id = cmd.Id,
        //        CommandText = cmd.CommandText,
        //        ScriptName = Context.GetPlugin<ScriptsPlugin>().GetScript(cmd.UserScript.Id).Name
        //    };
        //}

        private void InitSpeechSynthesizer()
        {
            speechSynthesizer = new SpeechSynthesizer();

            var prefferedVoice = SpeechSynthesizer.AllVoices.Where(v => v.Gender == VoiceGender.Female && v.Language == languageTag).FirstOrDefault();
            speechSynthesizer.Voice = prefferedVoice ?? SpeechSynthesizer.DefaultVoice;
        }
        private void CloseSpeechSynthesizer()
        {
            if (speechSynthesizer != null)
                speechSynthesizer.Dispose();

            speechSynthesizer = null;
        }

        private async void InitRecognitionEngine()
        {
            try
            {
                speechRecognizer = new SpeechRecognizer(new Language(languageTag));
            }
            catch
            {
                speechRecognizer = new SpeechRecognizer();
            }

            speechRecognizer.Constraints.Add(new SpeechRecognitionListConstraint(GetCommandsText(), "tag1"));

            //var op = speechRecognizer.CompileConstraintsAsync();
            //op.AsTask().Wait();
            ////var a = op.GetResults();

            //var op2 = speechRecognizer.RecognizeAsync();
            //op2.AsTask().Wait();
            //SpeechRecognitionResult result = op2.GetResults();
            //if (result.Status == SpeechRecognitionResultStatus.Success)
            //{
            //}

            var a = await speechRecognizer.CompileConstraintsAsync();
            var b = a;
            SpeechRecognitionResult result = await speechRecognizer.RecognizeAsync();
            //if (result.Status == SpeechRecognitionResultStatus.Success)
            //    phoneNumber = result.Text;







            //    var cultureInfo = new CultureInfo("ru-RU");
            //    //var cultureInfo = new CultureInfo("en-US");
            //    Thread.CurrentThread.CurrentCulture = cultureInfo;
            //    Thread.CurrentThread.CurrentUICulture = cultureInfo;

            //    /*
            //    •en-GB. English (United Kingdom)
            //    •en-US. English (United States)
            //    •de-DE. German (Germany)
            //    •es-ES. Spanish (Spain)
            //    •fr-FR. French (France)
            //    •ja-JP. Japanese (Japan)
            //    •zh-CN. Chinese (China)
            //    •zh-TW. Chinese (Taiwan)
            //    */

            //    var commands = GetCommandsText();
            //    var choices = new Choices(commands);
            //    var builder = new GrammarBuilder(choices);
            //    builder.Culture = cultureInfo;

            //    recognitionEngine = new SpeechRecognitionEngine();// (cultureInfo);
            //    recognitionEngine.SetInputToDefaultAudioDevice();
            //    recognitionEngine.UnloadAllGrammars();
            //    recognitionEngine.LoadGrammar(new Grammar(builder));
            //    //recognitionEngine.LoadGrammar(new DictationGrammar()); // любой текст

            //    recognitionEngine.SpeechRecognized += recognitionEngine_SpeechRecognized;
            //    recognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
        }
        private void CloseRecognitionEngine()
        {
            if (speechRecognizer != null)
                speechRecognizer.Dispose();

            speechRecognizer = null;
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    CloseSpeechSynthesizer();
                    CloseRecognitionEngine();
                }

                disposed = true;
            }
        }
        #endregion

        #region Event handlers
        //private void recognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        //{
        //    var commandText = e.Result.Text;
        //    Logger.Info("Command: {0} ({1:0.00})", commandText, e.Result.Confidence);

        //    if (e.Result.Confidence < CONFIDENCE_LIMIT)
        //    {
        //        Logger.Info("Apply confidence limit");
        //        return;
        //    }

        //    var now = DateTime.Now;
        //    var isInPeriod = readyDate.GetValueOrDefault() > now;

        //    if (commandText == NAME)
        //    {
        //        Logger.Info("Command is COMPUTER NAME");
        //        readyDate = now.AddSeconds(READY_PERIOD);
        //        Say(RESPONSE_READY);
        //    }
        //    else
        //    {
        //        if (isInPeriod)
        //        {
        //            try
        //            {
        //                //Debugger.Launch();
        //                var command = GetCommand(commandText);
        //                Logger.Info("Command info loaded");

        //                Context.GetPlugin<ScriptsPlugin>().ExecuteScript(command.UserScript);

        //                this.RaiseScriptEvent(x => x.OnVoiceCommandReceivedForScripts, commandText);

        //                readyDate = null;
        //            }
        //            catch (Exception ex)
        //            {
        //                var msg = string.Format("Voice command error: '{0}'", commandText);
        //                Logger.Error(ex, msg);
        //            }
        //        }
        //    }
        //}
        #endregion

        //#region Script events
        //[ScriptEvent("speech.commandReceived")]
        //public ScriptEventHandlerDelegate[] OnVoiceCommandReceivedForScripts { get; set; }
        //#endregion

        #region Script commands
        //[ScriptCommand("say")]
        public async void Say(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var stream = await speechSynthesizer.SynthesizeTextToStreamAsync(text);
                await new MediaElement().PlayStreamAsync(stream, true);
            }
        }
        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //#region Web API
        //[HttpCommand("/api/speech/voicecommand/list")]
        //private object apiGetCommands(HttpRequestParams request)
        //{
        //    return GetCommands()
        //        .Select(BuildCommandWebModel)
        //        .Where(x => x != null)
        //        .ToArray();
        //}
        //[HttpCommand("/api/speech/voicecommand/add")]
        //private object apiAddCommand(HttpRequestParams request)
        //{
        //    var text = request.GetRequiredString("text");
        //    var scriptId = request.GetRequiredGuid("scriptId");

        //    using (var session = Context.OpenSession())
        //    {
        //        VoiceCommand cmd = new VoiceCommand()
        //        {
        //            Id = Guid.NewGuid(),
        //            CommandText = text,
        //            UserScript = Context.GetPlugin<ScriptsPlugin>().GetScript(scriptId)
        //        };

        //        session.Save(cmd);
        //        session.Flush();
        //    }

        //    //NotifyForSignalR(new
        //    //{
        //    //    MsgId = "SensorNameChanged",
        //    //    Data = new
        //    //    {
        //    //        Id = id,
        //    //        Name = name
        //    //    }
        //    //});

        //    return null;
        //}
        //[HttpCommand("/api/speech/voicecommand/settext")]
        //private object apiSetCommandName(HttpRequestParams request)
        //{
        //    var id = request.GetRequiredGuid("id");
        //    var text = request.GetRequiredString("text");

        //    using (var session = Context.OpenSession())
        //    {
        //        var cmd = session.Load<VoiceCommand>(id);
        //        cmd.CommandText = text;
        //        session.Flush();
        //    }

        //    CloseRecognitionEngine();
        //    InitRecognitionEngine();

        //    //NotifyForSignalR(new
        //    //{
        //    //    MsgId = "SensorNameChanged",
        //    //    Data = new
        //    //    {
        //    //        Id = id,
        //    //        Name = name
        //    //    }
        //    //});

        //    return null;
        //}
        //[HttpCommand("/api/speech/voicecommand/delete")]
        //private object apiDeleteMonitor(HttpRequestParams request)
        //{
        //    var id = request.GetRequiredGuid("Id");

        //    using (var session = Context.OpenSession())
        //    {
        //        var cmd = session.Load<VoiceCommand>(id);
        //        session.Delete(cmd);
        //        session.Flush();
        //    }

        //    //NotifyForSignalR(new { MsgId = "SensorDeleted", Data = new { Id = id } });

        //    return null;
        //}
        //#endregion
    }
}
