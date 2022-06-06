using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TheUltraAudioCutter.Assets.Pages;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace TheUltraAudioCutter
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
     
        public MainPage()
        {
            this.InitializeComponent();
            AudioFrame.Navigate(typeof(AudioCutter));

        }

        private void AudioCutterButton_Click(object sender, RoutedEventArgs e)
        {
            AudioFrame.Navigate(typeof(AudioCutter));
        }

        private void AudioMergerButton_Click(object sender, RoutedEventArgs e)
        {
           AudioFrame.Navigate(typeof(TheUltraAudioCutter.Assets.Pages.AudioMerger));
        }

        private void AudioInfoButton_Click(object sender, RoutedEventArgs e)
        {
            AudioFrame.Navigate(typeof(TheUltraAudioCutter.Assets.Pages.AudioInfo));
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           
        }
    }
}
