using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using SmartHub.Core.Plugins;
using SmartHub.Core.Plugins.Utils;
using SmartHub.Plugins.AlarmClock.Data;
using SmartHub.Plugins.Audio;
using SmartHub.Plugins.Audio.Core;
using SmartHub.Plugins.HttpListener.Api;
using SmartHub.Plugins.HttpListener.Attributes;
using SmartHub.Plugins.Scripts;
using SmartHub.Plugins.Scripts.Attributes;
using SmartHub.Plugins.Scripts.Data;
using SmartHub.Plugins.SignalR;
using SmartHub.Plugins.Timer.Attributes;
using SmartHub.Plugins.WebUI.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace SmartHub.Plugins.AlarmClock
{
    // list
    [AppSection("Периодические задачи", SectionType.Common, "/webapp/alarm-clock/list.js", "SmartHub.Plugins.AlarmClock.Resources.alarm-list.js", TileTypeFullName = "SmartHub.Plugins.AlarmClock.AlarmClockTile")]
    [JavaScriptResource("/webapp/alarm-clock/list-model.js", "SmartHub.Plugins.AlarmClock.Resources.alarm-list-model.js")]
    [JavaScriptResource("/webapp/alarm-clock/list-view.js", "SmartHub.Plugins.AlarmClock.Resources.alarm-list-view.js")]
    [HttpResource("/webapp/alarm-clock/list.tpl", "SmartHub.Plugins.AlarmClock.Resources.alarm-list.tpl")]
    [HttpResource("/webapp/alarm-clock/list-item.tpl", "SmartHub.Plugins.AlarmClock.Resources.alarm-list-item.tpl")]

    // editor
    [JavaScriptResource("/webapp/alarm-clock/editor.js", "SmartHub.Plugins.AlarmClock.Resources.alarm-editor.js")]
    [JavaScriptResource("/webapp/alarm-clock/editor-model.js", "SmartHub.Plugins.AlarmClock.Resources.alarm-editor-model.js")]
    [JavaScriptResource("/webapp/alarm-clock/editor-view.js", "SmartHub.Plugins.AlarmClock.Resources.alarm-editor-view.js")]
    [HttpResource("/webapp/alarm-clock/editor.tpl", "SmartHub.Plugins.AlarmClock.Resources.alarm-editor.tpl")]

    [Plugin]
    public class AlarmClockPlugin : PluginBase
    {
        #region Fields
        private readonly object lockObject = new object();
        private readonly object lockObjectForSound = new object();
        private DateTime lastAlarmTime = DateTime.MinValue;
        private List<AlarmTime> times;
        private IPlayback playback;
        #endregion

        #region Import
        [ImportMany("0917789F-A980-4224-B43F-A820DEE093C8")]
        public Action<Guid>[] AlarmStartedForPlugins { get; set; }

        [ScriptEvent("alarmClock.alarmStarted")]
        public ScriptEventHandlerDelegate[] AlarmStartedForScripts { get; set; }
        #endregion

        #region SignalR events
        private void NotifyForSignalR(object msg)
        {
            Context.GetPlugin<SignalRPlugin>().Broadcast(msg);
        }
        #endregion

        #region Plugin overrides
        public override void InitDbModel(ModelMapper mapper)
        {
            mapper.Class<AlarmTime>(cfg => cfg.Table("AlarmClock_AlarmTime"));
        }
        #endregion

        #region Public methods
        public DateTime[] GetNextAlarmTimes(DateTime now)
        {
            lock (lockObject)
            {
                LoadTimes();

                return times
                    .Select(t => GetDateTime(t, now, lastAlarmTime))
                    .OrderBy(t => t)
                    .ToArray();
            }
        }
        public string[] GetNextAlarmsTimesAndNames(DateTime now)
        {
            lock (lockObject)
            {
                LoadTimes();

                return times
                    .Select(t => new { DT = GetDateTime(t, now, lastAlarmTime), Name = t.Name })
                    .OrderBy(t => t.DT)
                    .Select(t => t.DT.ToShortTimeString() + " - " + t.Name)
                    .ToArray();
            }
        }

        public void ReloadTimes()
        {
            lock (lockObject)
            {
                times = null;
                LoadTimes();
            }
        }
        public static DateTime GetDateTime(AlarmTime time, DateTime now, DateTime lastAlarm)
        {
            var date = now.Date.AddHours(time.Hours).AddMinutes(time.Minutes);

            if (date < lastAlarm || date.AddMinutes(5) < now)
                date = date.AddDays(1);

            return date;
        }

        public void PlaySound()
        {
            lock (lockObjectForSound)
            {
                StopSound();

                Logger.Info("Play sound");
                playback = Context.GetPlugin<AudioPlugin>().Play(SoundResources.Ring02, 25);
            }
        }
        public void StopSound()
        {
            lock (lockObjectForSound)
            {
                if (playback != null)
                {
                    Logger.Info("Stop all sounds");
                    playback.Stop();
                    playback = null;
                }
            }
        }

        public string BuildTileContent()
        {
            //var times = GetNextAlarmTimes(DateTime.Now).Take(4);
            //var strTimes = times.Select(t => t.ToShortTimeString()).ToArray();

            var strTimes = GetNextAlarmsTimesAndNames(DateTime.Now).Take(6);
            return strTimes.Any() ? string.Join(Environment.NewLine, strTimes) : "Нет активных оповещений";
        }
        public string BuildSignalRReceiveHandler()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("if (data.MsgId == 'AlarmClockTileContent') { ");
            sb.Append("model.tileModel.set({ 'content': data.Data }); ");
            sb.Append("}");
            return sb.ToString();
        }
        #endregion

        #region Private methods
        private void LoadTimes()
        {
            if (times == null)
            {
                using (var session = Context.OpenSession())
                {
                    times = session.Query<AlarmTime>()
                        .Fetch(a => a.UserScript)
                        .Where(t => t.Enabled)
                        .ToList();

                    Logger.Info("Loaded {0} alarm times", times.Count);
                }
            }
        }
        private static bool CheckTime(AlarmTime time, DateTime now, DateTime lastAlarm)
        {
            // если прошло время звонка будильника
            // и от этого времени не прошло 5 минут
            // и будильник сегодня еще не звонил
            var date = GetDateTime(time, now, lastAlarm);
            return lastAlarm < date && date < now;
        }
        private void Alarm(AlarmTime[] alarms)
        {
            Logger.Info("ALARM!");

            if (alarms.Any(a => a.PlaySound))
                PlaySound();

            foreach (var alarm in alarms)
            {
                Logger.Info("Run event handlers: {0} ({1})", alarm.Name, alarm.Id);

                Run(AlarmStartedForPlugins, x => x(alarm.Id));

                if (alarm.UserScript != null)
                {
                    Logger.Info("Run script: {0} ({1})", alarm.UserScript.Name, alarm.UserScript.Id);
                    Context.GetPlugin<ScriptsPlugin>().ExecuteScript(alarm.UserScript);
                }
            }

            Logger.Info("Run subscribed scripts");
            this.RaiseScriptEvent(x => x.AlarmStartedForScripts);
        }
        #endregion

        #region Event handlers
        [Timer_10sec_Elapsed]
        public void OnTimerElapsed(DateTime now)
        {
            lock (lockObject)
            {
                LoadTimes();
                var alarms = times.Where(x => CheckTime(x, now, lastAlarmTime)).ToArray();
                if (alarms.Any())
                {
                    lastAlarmTime = now;
                    Alarm(alarms);

                    NotifyForSignalR(new { MsgId = "AlarmClockTileContent", Data = BuildTileContent() });
                }
            }
        }
        #endregion

        #region Web API

        #region api list
        [HttpCommand("/api/alarm-clock/list")]
        public object GetAlarmList(HttpRequestParams request)
        {
            using (var session = Context.OpenSession())
            {
                var list = session.Query<AlarmTime>().ToList();

                var model = list
                    .Select(alarm => new
                    {
                        id = alarm.Id,
                        name = alarm.Name,
                        hours = alarm.Hours,
                        minutes = alarm.Minutes,
                        enabled = alarm.Enabled,
                        scriptId = alarm.UserScript.GetValueOrDefault(obj => (Guid?)obj.Id),
                        scriptName = alarm.UserScript.GetValueOrDefault(obj => obj.Name)
                    })
                    .ToArray();

                return model;
            }
        }

        [HttpCommand("/api/alarm-clock/set-state")]
        public object SetAlarmState(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");
            var enabled = request.GetRequiredBool("enabled");

            using (var session = Context.OpenSession())
            {
                var alarmTime = session.Get<AlarmTime>(id);
                alarmTime.Enabled = enabled;

                session.Save(alarmTime);
                session.Flush();
            }

            ReloadTimes();
            return null;
        }

        [HttpCommand("/api/alarm-clock/delete")]
        public object DeleteAlarm(HttpRequestParams request)
        {
            var id = request.GetRequiredGuid("id");

            using (var session = Context.OpenSession())
            {
                var alarmTime = session.Load<AlarmTime>(id);
                session.Delete(alarmTime);
                session.Flush();
            }

            ReloadTimes();
            return null;
        }

        [HttpCommand("/api/alarm-clock/stop")]
        public object StopAlarm(HttpRequestParams request)
        {
            StopSound();
            return null;
        }
        #endregion

        #region api editor
        [HttpCommand("/api/alarm-clock/save")]
        public object SaveAlarm(HttpRequestParams request)
        {
            var id = request.GetGuid("id");
            var name = request.GetString("name");
            var hours = request.GetRequiredInt32("hours");
            var minutes = request.GetRequiredInt32("minutes");
            var scriptId = request.GetGuid("scriptId");

            using (var session = Context.OpenSession())
            {
                var alarmTime = id.HasValue
                    ? session.Get<AlarmTime>(id.Value)
                    : new AlarmTime { Id = Guid.NewGuid() };

                var script = scriptId.HasValue
                    ? session.Load<UserScript>(scriptId.Value)
                    : null;

                alarmTime.Name = name;
                alarmTime.Hours = hours;
                alarmTime.Minutes = minutes;
                alarmTime.UserScript = script;
                alarmTime.Enabled = true;

                session.Save(alarmTime);
                session.Flush();
            }

            Context.GetPlugin<AlarmClockPlugin>().ReloadTimes();
            return null;
        }

        [HttpCommand("/api/alarm-clock/editor")]
        public object LoadEditor(HttpRequestParams request)
        {
            var id = request.GetGuid("id");

            using (var session = Context.OpenSession())
            {
                var scriptList = session
                    .Query<UserScript>()
                    .Select(s => new { id = s.Id, name = s.Name })
                    .ToArray();

                if (id.HasValue)
                {
                    var alarm = session.Get<AlarmTime>(id.Value);

                    return BuildEditorModel(
                        scriptList,
                        alarm.Id,
                        alarm.Name,
                        alarm.Hours,
                        alarm.Minutes,
                        alarm.Enabled,
                        alarm.UserScript.GetValueOrDefault(obj => (Guid?)obj.Id)
                    );
                }

                return BuildEditorModel(scriptList);
            }
        }

        private static object BuildEditorModel(object scripts, Guid? id = null, string name = null, int hours = 0, int minutes = 0, bool enabled = false, Guid? scriptId = null)
        {
            return new { id, name, hours, minutes, enabled, scriptId, scripts };
        }
        #endregion

        #endregion
    }
}
