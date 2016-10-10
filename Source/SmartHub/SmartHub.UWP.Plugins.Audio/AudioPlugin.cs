using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.Win8.Wave.WaveOutputs;
using SmartHub.UWP.Core.Plugins;
using System;
using System.IO;
using Windows.Storage;

namespace SmartHub.UWP.Plugins.Audio
{
    public class AudioPlugin : PluginBase
    {
        #region Fields
        //private WaveOut waveOut;
        private readonly object lockObject = new object();
        private WasapiOutRT _audioOutput;
        #endregion

        #region Plugin overrides
        public override void InitPlugin()
        {
            //waveOut = new WaveOut();
        }
        public override void StopPlugin()
        {
            //waveOut.Dispose();
            //waveOut = null;
        }
        #endregion

        #region API
        //public /*async*/ IPlayback Play(Stream stream, int loop = 0)
        //{
        //    //lock (lockObject)
        //    //{
        //    //    var reader = new WaveFileReader(stream);
        //    //    var loopStream = new LoopStream(/*Logger, */reader, loop);

        //    //    waveOut.Init(loopStream);
        //    //    waveOut.Play();

        //    //    return loopStream;
        //    //}




        //    //IStorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(path, UriKind.Absolute));
        //    //var s = await file.OpenAsync(FileAccessMode.Read);

        //    //_audioOutput = new WasapiOutRT(AudioClientShareMode.Shared, 200);
        //    //_audioOutput.Init(() =>
        //    //{
        //    //    var waveChannel32 = new WaveChannel32(new MediaFoundationReaderUniversal(s));
        //    //    var mixer = new MixingSampleProvider(new ISampleProvider[] { waveChannel32.ToSampleProvider() });
        //    //    return mixer.ToWaveProvider();
        //    //});
        //    //_audioOutput.Play();
        //}
        #endregion

    }
}
