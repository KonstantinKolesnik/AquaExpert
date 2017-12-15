using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SmartHub.UWP.Plugins.Speech
{
    static class MediaElementExtensions
    {
        public static async Task PlayStreamAsync(this MediaElement mediaElement, IRandomAccessStream stream, bool disposeStream = true)
        {
            // bool is irrelevant here, just using this to flag task completion.
            TaskCompletionSource<bool> taskCompleted = new TaskCompletionSource<bool>();

            // Note that the MediaElement needs to be in the UI tree for events like MediaEnded to fire.
            RoutedEventHandler endOfPlayHandler = (s, e) =>
            {
                if (disposeStream)
                    stream.Dispose();

                taskCompleted.SetResult(true);
            };

            mediaElement.MediaEnded += endOfPlayHandler;

            mediaElement.SetSource(stream, (stream as SpeechSynthesisStream).ContentType);
            mediaElement.Volume = 1;
            mediaElement.IsMuted = false;
            mediaElement.Play();

            await taskCompleted.Task;

            mediaElement.MediaEnded -= endOfPlayHandler;
        }
    }
}
