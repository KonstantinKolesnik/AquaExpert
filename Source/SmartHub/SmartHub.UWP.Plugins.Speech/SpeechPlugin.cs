using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.Audio;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;
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
        //private string[] LoadAllCommands()
        //{
        //    using (var session = Context.OpenSession())
        //    {
        //        List<string> list = session.Query<VoiceCommand>().Select(cmd => cmd.CommandText).ToList();

        //        Logger.Info("Loaded commands: {0}", list.ToJson("[]"));

        //        list.Add(NAME);

        //        return list.ToArray();
        //    }
        //}
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
            speechSynthesizer.Voice = SpeechSynthesizer.AllVoices.Where(v => v.Gender == VoiceGender.Female).FirstOrDefault() ?? SpeechSynthesizer.DefaultVoice;
        }
        private void CloseSpeechSynthesizer()
        {
            if (speechSynthesizer != null)
                speechSynthesizer.Dispose();

            speechSynthesizer = null;
        }

        private void InitRecognitionEngine()
        {
            var tag = Language.CurrentInputMethodLanguageTag; // "en-US", "ru-RU"

            speechRecognizer = new SpeechRecognizer();
            //speechRecognizer = new SpeechRecognizer(tag);

            //speechRecognizer.Constraints.Add(new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.FormFilling, "Phone"));

            //speechRecognizer.Constraints.Add(new SpeechRecognitionListConstraint(new string[] { "left", "right", "up", "down" }, "tag1"));
            //speechRecognizer.Constraints.Add(new SpeechRecognitionListConstraint(new string[] { "over", "under", "behind", "in front" }, "tag2"));



            //await speechRecognizer.CompileConstraintsAsync();

            //SpeechRecognitionResult result = await speechRecognizer.RecognizeAsync();
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

            //    var commands = LoadAllCommands();
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
        public async void Say(string text, MediaElement mediaElement = null)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var stream = await SynthesizeTextToSpeechAsync(text);

                if (mediaElement != null)
                    await mediaElement.PlayStreamAsync(stream, true);
                else
                    Context.GetPlugin<AudioPlugin>()?.Play(stream);
            }
        }
        #endregion
        public async Task<IRandomAccessStream> SynthesizeTextToSpeechAsync(string text)
        {
            IRandomAccessStream stream = null;

            using (var synthesizer = new SpeechSynthesizer())
                stream = await synthesizer.SynthesizeTextToStreamAsync(text);

            return stream;
        }

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
