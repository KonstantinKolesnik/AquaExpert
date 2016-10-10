using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.Win8.Wave.WaveOutputs;
using SmartHub.UWP.Core.Plugins;
using System;
using System.IO;
using Windows.Storage;
using Windows.Storage.Streams;

namespace SmartHub.UWP.Plugins.Audio
{
    public class AudioPlugin : PluginBase, IDisposable
    {
        #region Fields
        private bool disposed = false;
        //private WaveOut waveOut;
        //private readonly object lockObject = new object();
        private WasapiOutRT audioOutput;
        #endregion

        #region Plugin overrides
        public override void InitPlugin()
        {
            //waveOut = new WaveOut();
            audioOutput = new WasapiOutRT(AudioClientShareMode.Shared, 200);
        }
        public override void StopPlugin()
        {
            CloseAudioOutput();
            //waveOut.Dispose();
            //waveOut = null;
        }
        #endregion

        #region Public methods
        //public /*async*/ IPlayback Play(Stream stream, int loop = 0)
        //public IPlayback Play(Stream stream, int loop = 0)
        public void Play(IRandomAccessStream stream)
        {
            //lock (lockObject)
            //{
            //    var reader = new WaveFileReader(stream);
            //    var loopStream = new LoopStream(/*Logger, */reader, loop);

            //    waveOut.Init(loopStream);
            //    waveOut.Play();

            //    return loopStream;
            //}




            //IStorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(path, UriKind.Absolute));
            //var s = await file.OpenAsync(FileAccessMode.Read);

            //audioOutput.Init(() =>
            //{
            //    var waveChannel32 = new WaveChannel32(new MediaFoundationReaderUniversal(s));
            //    var mixer = new MixingSampleProvider(new ISampleProvider[] { waveChannel32.ToSampleProvider() });
            //    return mixer.ToWaveProvider();
            //});

            //audioOutput.Play();
        }
        public void Stop()
        {
            if (audioOutput != null)
                audioOutput.Stop();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Private methods
        private void CloseAudioOutput()
        {
            if (audioOutput != null)
                audioOutput.Dispose();

            audioOutput = null;
        }
        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                    CloseAudioOutput();

                disposed = true;
            }
        }
        #endregion
    }
}
