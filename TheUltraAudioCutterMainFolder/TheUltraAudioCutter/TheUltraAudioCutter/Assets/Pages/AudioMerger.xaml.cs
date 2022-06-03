using AudioMerger;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.Editing;
using Windows.Media.MediaProperties;
using Windows.Media.Transcoding;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace TheUltraAudioCutter.Assets.Pages
{
    /// <summary>
    /// Страница отвечающая за объединение и наложение аудио файлов
    /// </summary>
    public sealed partial class AudioMerger : Page
    {
        public AudioMerger()
        {
            this.InitializeComponent();
        }
        /// <summary>
        /// Меняет папку сохранения аудио файла
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
                await Windows.Storage.FileIO.WriteTextAsync(File, folder.Path);
            }
            catch
            {
                MessageDialog dialog = new MessageDialog("File system denied. Allow the programm to work with files");
                await dialog.ShowAsync();
                bool result = await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-broadfilesystemaccess"));

            }

        }
        private MediaComposition composition;
        /// <summary>
        /// Выбор файла для соединения/наложения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ChooseFileMerger_Click(object sender, RoutedEventArgs e)
        {

            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.SuggestedStartLocation =
Windows.Storage.Pickers.PickerLocationId.MusicLibrary;
            picker.FileTypeFilter.Add(".flac");
            picker.FileTypeFilter.Add(".wav");
            picker.FileTypeFilter.Add(".mp3");
            StorageFile sourceFile = await picker.PickSingleFileAsync();
            Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(sourceFile);
            FileList.Items.Add(sourceFile.Path);



        }
        /// <summary>
        /// Объединения аудио файлов  в один
        /// </summary>
        /// <param name="outputFile"> Файл, который получиться после соединения аудио файлов</param>
        /// <param name="sourceFiles"> Файлы, которые стоит соединить</param>
        public static void Concatenate(string outputFile, IEnumerable<string> sourceFiles)
        {
            byte[] buffer = new byte[1024];
            WaveFileWriter waveFileWriter = null;

            try
            {
                foreach (string sourceFile in sourceFiles)
                {
                    WaveFileReader reader = null;

                    using (reader = new WaveFileReader(sourceFile))
                    {
                        if (waveFileWriter == null)
                        {
                            // first time in create new Writer
                            waveFileWriter = new WaveFileWriter(outputFile, reader.WaveFormat);
                        }
                        else
                        {
                            if (!reader.WaveFormat.Equals(waveFileWriter.WaveFormat))
                            {
                                throw new InvalidOperationException("Can't concatenate WAV Files that don't share the same format");
                            }
                        }

                        int read;
                        while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            waveFileWriter.WriteData(buffer, 0, read);
                        }
                    }
                }
            }
            finally
            {
                if (waveFileWriter != null)
                {
                    waveFileWriter.Dispose();
                }
            }

        }
        private StorageFile currentFile;
        /// <summary>
        /// Конвертация аудио файла в .wav формат
        /// </summary>
        /// <param name="sourceFile"> Аудио файл</param>
        /// <returns></returns>
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
        /// Соединение аудио файлов из списка в один
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Combine_Button_Click(object sender, RoutedEventArgs e)
        {
            var uniqueName = string.Format(@"UltraAudioCutterMixedFile_{0}", Guid.NewGuid());
            try
            {
                UtilityFunctions.DeleteFilesFromDir(ApplicationData.Current.TemporaryFolder.Path);
                var mixer = new WaveMixerStream32 { AutoStop = true };
                String[] audioFilenames = new String[FileList.Items.Count];
                String[] audioFilenamesTrue = new String[FileList.Items.Count];
                int i = 0;

                foreach (var items in FileList.Items)
                {
                    audioFilenames[i] = items.ToString();
                    // var wav1 = new WaveFileReader(audioFilenames[i]);
                    // mixer.AddInputStream(new WaveChannel32(wav1));


                    StorageFolder WhereTo = await StorageFolder.GetFolderFromPathAsync(ApplicationData.Current.TemporaryFolder.Path);

                    audioFilenamesTrue[i] = WhereTo.Path + @"\" + Path.GetFileName(audioFilenames[i]);
                    currentFile = await StorageFile.GetFileFromPathAsync(audioFilenames[i]);
                    await ConvertToWaveFile(currentFile);
                    audioFilenamesTrue[i] = currentFile.Path;
                    currentFile = await StorageFile.GetFileFromPathAsync(audioFilenamesTrue[i]);

                    i++;
                }

                Concatenate(ApplicationData.Current.TemporaryFolder.Path + $@"\{uniqueName}.wav", audioFilenamesTrue);
            }
            catch
            {

            }
            try
            {
                StorageFile WhereTo = await KnownFolders.DocumentsLibrary.GetFileAsync("SavingFolderAudioCutter.txt"); // this file contains a path to a folder
                string oof = await Windows.Storage.FileIO.ReadTextAsync(WhereTo); //getting a path
                StorageFolder dest = await StorageFolder.GetFolderFromPathAsync(oof); //setting StorageFolder path from a string 
                string path = ApplicationData.Current.TemporaryFolder.Path + $@"\{uniqueName}.wav"; // setting a name for a moved file
                StorageFile FromWhere = await StorageFile.GetFileFromPathAsync(path); // Original file location

                await FromWhere.MoveAsync(dest);

                UtilityFunctions.DeleteFilesFromDir(ApplicationData.Current.TemporaryFolder.Path); // Deleting files from a temporary folder
                FileList.Items.Clear();
            }
            catch
            {
                MessageDialog dialog = new MessageDialog("The folder is read-only or incorrect choosen path");

                await dialog.ShowAsync();
            }

        }
        /// <summary>
        /// Наложение аудио файлов из списка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Merge_Button_Click(object sender, RoutedEventArgs e)
        {
            var uniqueName = string.Format(@"UltraAudioCutterMixedFile_{0}", Guid.NewGuid());
            try
            {
                UtilityFunctions.DeleteFilesFromDir(ApplicationData.Current.TemporaryFolder.Path);
                var mixer = new WaveMixerStream32 { AutoStop = true };
                String[] audioFilenames = new String[FileList.Items.Count];
                String[] audioFilenamesTrue = new String[FileList.Items.Count];
                int i = 0;

                foreach (var items in FileList.Items)
                {
                    audioFilenames[i] = items.ToString();



                    StorageFolder WhereTo = await StorageFolder.GetFolderFromPathAsync(ApplicationData.Current.TemporaryFolder.Path);

                    audioFilenamesTrue[i] = WhereTo.Path + @"\" + Path.GetFileName(audioFilenames[i]);
                    currentFile = await StorageFile.GetFileFromPathAsync(audioFilenames[i]);
                    await ConvertToWaveFile(currentFile);
                    audioFilenamesTrue[i] = currentFile.Path;
                    var wav1 = new WaveFileReader(audioFilenamesTrue[i]);
                    mixer.AddInputStream(new WaveChannel32(wav1));
                    currentFile = await StorageFile.GetFileFromPathAsync(audioFilenamesTrue[i]);

                    i++;
                }

                WaveFileWriter.CreateWaveFile(ApplicationData.Current.TemporaryFolder.Path + $@"\{uniqueName}.wav", new Wave32To16Stream(mixer));
            }
            catch
            {

            }

            try
            {
                StorageFile WhereTo = await KnownFolders.DocumentsLibrary.GetFileAsync("SavingFolderAudioCutter.txt");
                string oof = await Windows.Storage.FileIO.ReadTextAsync(WhereTo);
                StorageFolder dest = await StorageFolder.GetFolderFromPathAsync(oof);
                string path = ApplicationData.Current.TemporaryFolder.Path + $@"\{uniqueName}.wav";
                StorageFile FromWhere = await StorageFile.GetFileFromPathAsync(path);

                await FromWhere.MoveAsync(dest);

                UtilityFunctions.DeleteFilesFromDir(ApplicationData.Current.TemporaryFolder.Path);
                FileList.Items.Clear();
            }
            catch
            {
                MessageDialog dialog = new MessageDialog("The folder is read-only or incorrect choosen path");

                await dialog.ShowAsync();
            }
        }
        /// <summary>
        /// Удаление аудио файла из списка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_From_List_Click(object sender, RoutedEventArgs e)
        {
            FileList.Items.RemoveAt(FileList.SelectedIndex);
        }
        /// <summary>
        /// Всплывающее меню, при нажатии правой кнопкой мыши на элемент в списке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuFlyout_Opening(object sender, object e)
        {
            if (FileList.SelectedIndex == -1)
            {
                MenuFlyout menu = (MenuFlyout)sender;
                menu.Hide();
            }

        }
        /// <summary>
        /// Передвигвает элемент в списке
        /// </summary>
        /// <param name="direction"> Численненная переменная, отвечающая за количество строк, на которое передвигается другая строка</param>
        public void MoveItem(int direction)
        {

            if (FileList.SelectedItem == null || FileList.SelectedIndex < 0)
                return;


            int newIndex = FileList.SelectedIndex + direction;

            if (newIndex < 0 || newIndex >= FileList.Items.Count)
                return;

            object selected = FileList.SelectedItem;


            FileList.Items.Remove(selected);

            FileList.Items.Insert(newIndex, selected);

        }
        /// <summary>
        /// Передвижение элемента наверх на один
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            MoveItem(1);
        }
        /// <summary>
        /// Передвижение элемента вниз на один
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            MoveItem(-1);
        }
    }
}
