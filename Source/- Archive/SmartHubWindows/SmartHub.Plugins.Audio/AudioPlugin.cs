﻿using NAudio.Wave;
using SmartHub.Core.Plugins;
using SmartHub.Plugins.Audio.Core;
using System.IO;

namespace SmartHub.Plugins.Audio
{
    [Plugin]
    public class AudioPlugin : PluginBase
    {
        #region Fields
        private WaveOut waveOut;
        private readonly object lockObject = new object();
        #endregion

        #region Plugin overrides
        public override void InitPlugin()
        {
            waveOut = new WaveOut();
        }
        public override void StopPlugin()
        {
            waveOut.Dispose();
            waveOut = null;
        }
        #endregion

        #region API
        public IPlayback Play(Stream stream, int loop = 0)
        {
            lock (lockObject)
            {
                var reader = new WaveFileReader(stream);
                var loopStream = new LoopStream(Logger, reader, loop);

                waveOut.Init(loopStream);
                waveOut.Play();

                return loopStream;
            }
        }
        #endregion
    }
}
