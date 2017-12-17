using SmartHub.UWP.Core;
using SmartHub.UWP.Plugins.Wemos.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace SmartHub.UWP.Plugins.Wemos.Monitors.Models
{
    public class WemosMonitorObservable : ObservableObject
    {
        #region Fields
        private WemosMonitorDto model;
        //private ThreadPoolTimer timer;
        private double updateIntervalSeconds = 3;

        private Task taskListen;
        private CancellationTokenSource ctsListen;
        private bool isListenActive = false;
        #endregion

        #region Properties
        public string ID
        {
            get { return model.ID; }
        }
        public string LineID
        {
            get { return model.LineID; }
        }
        public string LineName
        {
            get { return model.LineName; }
        }
        public WemosLineType LineType
        {
            get { return model.LineType; }
        }

        public float Factor
        {
            get { return model.Factor; }
            set
            {
                if (model.Factor != value)
                {
                    model.Factor = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public float Offset
        {
            get { return model.Offset; }
            set
            {
                if (model.Offset != value)
                {
                    model.Offset = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Units
        {
            get { return model.Units; }
            set
            {
                if (model.Units != value)
                {
                    model.Units = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public float Min
        {
            get { return model.Min; }
            set
            {
                if (model.Min != value)
                {
                    model.Min = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public float Max
        {
            get { return model.Max; }
            set
            {
                if (model.Max != value)
                {
                    model.Max = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int ValuesCount
        {
            get { return model.ValuesCount; }
            set
            {
                if (model.ValuesCount != value)
                {
                    model.ValuesCount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        // helper properties:
        public ObservableCollection<WemosLineValue> Values
        {
            get;
        } = new ObservableCollection<WemosLineValue>();
        public float LastValue
        {
            get { return Values.Any() ? Values.LastOrDefault().Value : float.NaN; }
        }
        public string LastValueText
        {
            get { return Values.Any() ? $"{LastValue} { (string.IsNullOrEmpty(Units) ? WemosPlugin.LineTypeToUnits(LineType) : Units) }" : "-"; }
        }
        public string LastTimeStamp
        {
            get { return Values.Any() ? $"{Values.LastOrDefault().TimeStamp.ToString("dd.MM.yy HH:mm:ss")}" : ""; }
        }
        #endregion

        #region Constructor
        public WemosMonitorObservable(WemosMonitorDto model)
        {
            this.model = model;

            //timer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(async (t) =>
            //{
            //    //await UpdateValues();
            //    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => { await UpdateValues(); });
            //}), TimeSpan.FromSeconds(updateIntervalSeconds));

            PropertyChanged += (s, e) =>
            {
                if (!e.PropertyName.Contains("Last"))
                    CoreUtils.RequestAsync<bool>("/api/wemos/monitors/update", model);
            };
        }
        #endregion

        #region Public methods
        public async Task UpdateValues()
        {
            var items = await CoreUtils.RequestAsync<List<WemosLineValue>>("/api/wemos/line/values", LineID, ValuesCount);

            Values.Clear();
            if (items != null)
                foreach (var item in items.Where(item => item != null).OrderBy(i => i.TimeStamp))
                {
                    item.Value = item.Value * Factor + Offset;
                    Values.Add(item);
                }

            NotifyPropertyChanged("LastValue");
            NotifyPropertyChanged("LastValueText");
            NotifyPropertyChanged("LastTimeStamp");
        }

        public void StartListen()
        {
            if (!isListenActive)
            {
                ctsListen = new CancellationTokenSource();

                taskListen = Task.Factory.StartNew(async () =>
                {
                    while (!ctsListen.IsCancellationRequested)
                    {
                        if (isListenActive)
                            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => { await UpdateValues(); });

                        await Task.Delay((int)updateIntervalSeconds * 1000);
                    }
                }, ctsListen.Token);

                isListenActive = true;
            }
        }
        public void StopListen()
        {
            //if (timer != null)
            //    timer.Cancel();

            if (isListenActive)
            {
                ctsListen?.Cancel();
                isListenActive = false;
            }
        }
        #endregion
    }
}
