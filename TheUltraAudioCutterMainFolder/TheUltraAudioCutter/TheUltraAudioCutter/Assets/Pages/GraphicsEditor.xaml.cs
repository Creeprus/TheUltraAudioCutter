using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пользовательский элемент управления" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234236

namespace TheUltraAudioCutter.Assets.Pages
{
    public sealed partial class GraphicsEditor : UserControl
    {
        /// <summary>
        /// Класс, отвечающий за вывод аудиографа на окне
        /// </summary>
        public GraphicsEditor()
        {
            this.InitializeComponent();
            DataContext = this;
            MainContainer.PointerPressed += new PointerEventHandler(PointerPressed);
            MainContainer.PointerMoved += new PointerEventHandler(PointerMoved);
            MainContainer.PointerReleased += new PointerEventHandler(PointerReleased);
            MainContainer.PointerExited += new PointerEventHandler(PointerReleased);
        }

        private double leftPos;
        private double rightPos;
        private double minSelectionWidth = 10;

        private bool isPressed = false;
        private double startSelectionPosition = 0;
        private double currentSelectionWidth = 0;
        public event EventHandler SelectionChanged = delegate { };
        private bool enableSelection = false;
        public bool EnableSelection
        {
            get { return enableSelection; }
            set
            {
                enableSelection = value;
            }
        }
        private SoftwareBitmapSource imageSource;
        public SoftwareBitmapSource ImageSource
        {
            get { return imageSource; }
            set
            {
                imageSource = value;
                BackgroundImage.Source = value;
                CurrentDuration.Source = value;
            }
        }
        public double LeftPos
        {
            get
            {
                return leftPos;
            }
            set
            {
                leftPos = value;
                SetRightSelectionMargin();
            }
        }
        public double RelativeLeftPos
        {
            get { return leftPos / ActualWidth; }
        }

        public double RightPos
        {
            get
            {
                return rightPos;
            }
            set
            {
                rightPos = value;
                SetRightSelectionMargin();
            }
        }
        public double RelativeRightPos
        {
            get { return rightPos / ActualWidth; }
        }
        /// <summary>
        /// Присваивает переменной значение правого конца выбранного отрезка на аудиографе
        /// </summary>
        private void SetRightSelectionMargin()
        {
            currentSelectionWidth = Math.Abs(rightPos - leftPos);
            ManageGraphics();
        }
        /// <summary>
        /// Задает ширину аудиографа
        /// </summary>
        private void ManageGraphics()
        {
            SelectedChunk.SetValue(Canvas.LeftProperty, LeftPos);
            SelectedChunk.Width = currentSelectionWidth;
        }
        /// <summary>
        /// При отжатии выделения на аудиографе, запоминает его значение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            isPressed = false;
            if (Math.Abs(startSelectionPosition - GetPointerPositionX(e)) < minSelectionWidth)
            {
                EnableSelection = false;
            }
        }
        /// <summary>
        /// Определяет значения левого конца выбранного отрезка и правого отрезка на аудиографе, если они передвинулись
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (isPressed)
            {
                EnableSelection = true;
                double xPosition = GetPointerPositionX(e);
                currentSelectionWidth = Math.Abs(startSelectionPosition - xPosition);

                if (currentSelectionWidth > minSelectionWidth)
                {

                    if (xPosition < startSelectionPosition)
                    {
                        if (xPosition < 0)
                        {
                            LeftPos = 0;
                        }
                        else if (xPosition < RightPos - minSelectionWidth)
                        {
                            LeftPos = xPosition;
                        }
                        RightPos = startSelectionPosition;
                    }
                    else if (xPosition > startSelectionPosition)
                    {
                        if (xPosition > this.ActualWidth)
                        {
                            RightPos = this.ActualWidth;
                        }
                        else if (xPosition > LeftPos + minSelectionWidth)
                        {
                            RightPos = xPosition;
                        }
                        LeftPos = startSelectionPosition;
                    }
                }
            }
        }
        /// <summary>
        /// Обозначает левый и правый конец выбранного отрезка на аудиографе
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            SelectedChunk.Visibility = Visibility.Visible;

            isPressed = true;
            startSelectionPosition = GetPointerPositionX(e);

            LeftPos = startSelectionPosition;
            RightPos = LeftPos + 2;
        }
        /// <summary>
        /// Получает значение выбранного отрезка на аудиографе
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private double GetPointerPositionX(PointerRoutedEventArgs e)
        {
            PointerPoint pt = e.GetCurrentPoint(MainContainer);
            Point position = pt.Position;
            return position.X;
        }
    }
}
