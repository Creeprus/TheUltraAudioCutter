using AudioMerger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.MediaProperties;
using Windows.Media.Transcoding;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace TheUltraAudioCutter.Assets.Pages
{
    /// <summary>
    /// Страница, отвечающая за окно обрезки аудио файлов
    /// </summary>
    public sealed partial class AudioCutter : Page
    {

        public AudioCutter()
        {
            this.InitializeComponent();
        }
        private StorageFile currentFile, sourceFile;
        private PlottingGraphImg imgFile;
        private AudioDataEditor editor;
        /// <summary>
        /// Конвертация аудио файла в формат .wav
        /// </summary>
        /// <param name="sourceFile"> Аудио файл</param>
        public async Task ConvertToWaveFile(StorageFile sourceFile)
        {
            MediaTranscoder transcoder = new MediaTranscoder();
            MediaEncodingProfile profile = MediaEncodingProfile.CreateWav(AudioEncodingQuality.Medium);
            CancellationTokenSource cts = new CancellationTokenSource();
            //Create temporary file in temporary folder
            string fileName = String.Format("{0}.wav", Guid.NewGuid());
            StorageFile temporaryFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(fileName);
            currentFile = temporaryFile;
            if (sourceFile == null || temporaryFile == null)
            {
                return;
            }
            try
            {
                var preparedTranscodeResult = await transcoder.PrepareFileTranscodeAsync(sourceFile, temporaryFile, profile);
                if (preparedTranscodeResult.CanTranscode)
                {
                    var progress = new Progress<double>((percent) => { Debug.WriteLine("Converting file: " + percent + "%"); });
                    await preparedTranscodeResult.TranscodeAsync().AsTask(cts.Token, progress);
                }
                else
                {
                    Debug.WriteLine("Error: Convert fail");
                }
            }
            catch
            {
                Debug.WriteLine("Error: Exception in ConvertToWaveFile");
            }
        }
        /// <summary>
        /// Открытие диалогового окна с выбором файла из форматов .flac, .mp3 и .wav
        /// </summary>
        private async Task OpenFileDialog()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.SuggestedStartLocation =
Windows.Storage.Pickers.PickerLocationId.MusicLibrary;
            picker.FileTypeFilter.Add(".flac");
            picker.FileTypeFilter.Add(".mp3");
            picker.FileTypeFilter.Add(".wav");

            sourceFile = await picker.PickSingleFileAsync();
            if (sourceFile == null) await OpenFileDialog();
        }
        /// <summary>
        /// Конвертирует аудио файл в формат .wav. Используется, если параметр sourcefile ненужно указывать
        /// </summary>
        /// <returns></returns>
        public async Task ConvertToWaveFile()
        {
            OpenLoadWindow(true);
            MediaTranscoder transcoder = new MediaTranscoder();
            MediaEncodingProfile profile =
MediaEncodingProfile.CreateWav(AudioEncodingQuality.Medium);
            CancellationTokenSource cts = new CancellationTokenSource();

            string fileName = String.Format("{0}_{1}.wav", sourceFile.DisplayName,
Guid.NewGuid());
            currentFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(fileName); // здесь сохраняется файл
            Debug.WriteLine(currentFile.Path.ToString());
            try
            {
                var preparedTranscodeResult = await transcoder.PrepareFileTranscodeAsync(sourceFile, currentFile, profile);
                if (preparedTranscodeResult.CanTranscode)
                {
                    var progress = new Progress<double>((percent) =>
                    { Debug.WriteLine("Converting file: " + percent + "%"); });
                    await preparedTranscodeResult.TranscodeAsync().AsTask(cts.Token, progress);
                }
                else
                {
                    Debug.WriteLine("Error: Convert fail");
                }
            }
            catch
            {
                Debug.WriteLine("Error: Exception in ConvertToWaveFile");
            }
            OpenLoadWindow(false);
        }
        /// <summary>
        /// Метод, при помощи которого выбирается файл из диалогового окна, конвертируется в .wav файл и строиться аудиограф из аудио файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void LoadAudioFile(object sender, RoutedEventArgs e)
        {
            await OpenFileDialog();
            await ConvertToWaveFile();
            await BuildImageFile();

        }
        /// <summary>
        /// Сохраняет аудиограф в виде .jpg файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BuildAndSaveImageFile_Click(object sender, RoutedEventArgs e)
        {
            WavFile wavFile = new WavFile(currentFile.Path.ToString());
            imgFile = new PlottingGraphImg(wavFile, 1000, 100);
            FileSavePicker fileSavePicker = new FileSavePicker();
            fileSavePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            fileSavePicker.FileTypeChoices.Add("JPEG files", new List<string>() { ".jpg" });
            fileSavePicker.SuggestedFileName = "image";
            var outputFile = await fileSavePicker.PickSaveFileAsync();
            if (outputFile == null)
            {
                // The user cancelled the picking operation
                return;
            }
            await imgFile.SaveGraphicFile(outputFile);
        }
        /// <summary>
        /// Задает источник для MediaElement
        /// </summary>
        /// <param name="file"> Аудио файл</param>
        private async Task SetAudioClip(StorageFile file)
        {
            var stream = await file.OpenReadAsync();
            Player.SetSource(stream, "");

        }
        /// <summary>
        /// Отвечает за остановку и воспроизведения аудио
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Playback_Click(object sender, RoutedEventArgs e)
        {
            if (Player.CurrentState == MediaElementState.Playing)
            {
                Player.Stop();
            }
            else
            {
                Player.Play();
            }
        }
        /// <summary>
        /// Метод, который строит аудиограф из аудио файла
        /// </summary>
        /// <returns></returns>
        private async Task BuildImageFile()
        {
            editor = new AudioDataEditor(currentFile.Path.ToString());
            await Update();
        }
        /// <summary>
        /// Метод обновляющий аудиограф, когда аудио отредактировали
        /// </summary>
        /// <returns></returns>
        private async Task Update()
        {
            imgFile = new PlottingGraphImg(editor,
(int)AudioEditorControl.ActualWidth, (int)AudioEditorControl.ActualHeight);
            AudioEditorControl.ImageSource = await imgFile.GetImage();
            await SetAudioClip(currentFile);

        }
        /// <summary>
        /// Отвечает за отображения окна загрузки, при выборе аудио файла
        /// </summary>
        /// <param name="enable"></param>
        private void OpenLoadWindow(bool enable) => LoadDummy.Visibility = enable ? Visibility.Visible : Visibility.Collapsed;
        /// <summary>
        /// Копирует выбранный на аудиографе отрезок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (AudioEditorControl.EnableSelection)
                editor.Copy(AudioEditorControl.RelativeLeftPos, AudioEditorControl.RelativeRightPos);
        }
        /// <summary>
        /// Обрезка выбранного на аудиографе отрезка, сохраняет отрезок в буффере программы для метода Paste 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            if (AudioEditorControl.EnableSelection)
                editor.Cut(AudioEditorControl.RelativeLeftPos, AudioEditorControl.RelativeRightPos);
            Update();
        }
        /// <summary>
        /// Вставляет в выбранную на аудиографе начальную точку обрезанный или скопированный отрезок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            editor.Paste(AudioEditorControl.RelativeLeftPos);
            Update();
        }
        /// <summary>
        /// Сохраняет отредактированный файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StorageFile WhereTo = await KnownFolders.DocumentsLibrary.GetFileAsync("SavingFolderAudioCutter.txt");
                string oof = await FileIO.ReadTextAsync(WhereTo);
                StorageFolder dest = await StorageFolder.GetFolderFromPathAsync(oof);
                StorageFile FromWhere = await ApplicationData.Current.TemporaryFolder.GetFileAsync(currentFile.Name);
                await FromWhere.MoveAsync(dest);
                UtilityFunctions.DeleteFilesFromDir(ApplicationData.Current.TemporaryFolder.Path);
            }
            catch
            {
                MessageDialog dialog = new MessageDialog("The folder is read-only or incorrect choosen path");

                await dialog.ShowAsync();
            }
            Update();

        }
        /// <summary>
        /// Меняет папку, в которой будет сохраненный файл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ChangeFolder_Btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StorageFile File = await KnownFolders.DocumentsLibrary.GetFileAsync("SavingFolderAudioCutter.txt");
                if (KnownFolders.DocumentsLibrary.GetFileAsync("SavingFolderAudioCutter.txt") == null)
                {
                    await KnownFolders.DocumentsLibrary.CreateFileAsync("SavingFolderAudioCutter.txt");
                }
                var folderPicker = new Windows.Storage.Pickers.FolderPicker();
                folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
                folderPicker.FileTypeFilter.Add("*");
                Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();
                if (folder != null)
                    await FileIO.WriteTextAsync(File, folder.Path);
            }
            catch
            {
                MessageDialog dialog = new MessageDialog("File system denied. Allow the programm to work with files");
                await dialog.ShowAsync();
                bool result = await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-broadfilesystemaccess"));
            }
        }
        /// <summary>
        /// Меняет значение слайдера, отвечающий за отображения текущей длительности трека, в зависимости от значения аудио
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Player_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            CurrentProgressTrack.Value = Player.Position.TotalMilliseconds;

        }
        /// <summary>
        /// При начале проигрывания аудио файла, ставит максимальное значение слайдера в зависимости от аудио файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            CurrentProgressTrack.Maximum = Player.NaturalDuration.TimeSpan.TotalMilliseconds;

        }
        /// <summary>
        /// Удаляет выбранный на аудиографе отрезок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (AudioEditorControl.EnableSelection)
                editor.Delete(AudioEditorControl.RelativeLeftPos, AudioEditorControl.RelativeRightPos);
            Update();
        }
    }
}
