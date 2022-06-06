using System;
using System.IO;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace TheUltraAudioCutter.Assets.Pages
{
    /// <summary>
    /// Страница, отвечающая за отображение информации об аудиофайле
    /// </summary>
    public sealed partial class AudioInfo : Page
    {
        public AudioInfo()
        {
            this.InitializeComponent();
        }
        /// <summary>
        /// Определяет значения текстовых полей, если у аудиофайла отсутствует название трека, автор трека и названия альбома трека
        /// </summary>
        /// <param name="musicProperties"> Параметр musiProperties у аудио файла</param>
        private async void BoxSetterToEmpty(MusicProperties musicProperties)
        {
            if (musicProperties.Title == String.Empty)
            {
                TrackName.Text = "<No title>";

            }
            if (musicProperties.Artist == String.Empty)
            {
                Compositor.Text = "<No compositor>";
            }
            if (musicProperties.Album == String.Empty)
            {
                AlbumName.Text = "<No album>";
            }
        }
        /// <summary>
        /// Получает информацию об аудио и задает данную информацию текстовым полям
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AudioInfoGetter_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.SuggestedStartLocation =
    Windows.Storage.Pickers.PickerLocationId.MusicLibrary;
                picker.FileTypeFilter.Add(".flac");
                picker.FileTypeFilter.Add(".wav");
                picker.FileTypeFilter.Add(".mp3");
                StorageFile sourceFile = await picker.PickSingleFileAsync();

                MusicProperties musicProperties = await sourceFile.Properties.GetMusicPropertiesAsync();


                string dur = musicProperties.Duration.ToString();
                dur = dur.Remove(dur.Length - 8);
                TrackName.Text = musicProperties.Title;
                Compositor.Text = musicProperties.Artist;
                AlbumName.Text = musicProperties.Album;
                File_nameMusic.Text = Path.GetFileNameWithoutExtension(sourceFile.Path);
                Bitrate.Text = musicProperties.Bitrate.ToString();
                DurationTrack.Text = dur;
                PathMusic.Text = sourceFile.Path;
                BoxSetterToEmpty(musicProperties);
                var bitmap = new BitmapImage();
                AudioImage.Source = bitmap;
                using (StorageItemThumbnail thumbnail =
                    await sourceFile.GetThumbnailAsync(ThumbnailMode.MusicView, 300))
                {
                    if (thumbnail != null && thumbnail.Type == ThumbnailType.Image)
                    {
                        var bitmapImage = new BitmapImage();
                        bitmapImage.SetSource(thumbnail);
                        AudioImage.Source = bitmapImage;
                    }
                }
            }
            catch
            {
                MessageDialog dialog = new MessageDialog("Error");

                await dialog.ShowAsync();
            }
        }
    }
}
