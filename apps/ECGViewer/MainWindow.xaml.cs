using ECGConversion;
using ECGViewer.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Text;
using System.Collections;
using ECGConversion.ECGSignals;
using System.IO;
using System.Threading;
using ECGConversion.ECGGlobalMeasurements;
using ECGConversion.ECGDiagnostic;
using ECGViewer.Extensions;
using ECGViewer.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using Point = System.Windows.Point;

namespace ECGViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point _clickPoint = new Point(0, 0);

        public MainWindow()
        {
            InitializeComponent();
        }
        public MainWindowViewModel CurrentViewModel => Ioc.Default.GetRequiredService<MainWindowViewModel>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateSize();
        }

        private void UpdateSize()
        {
            CurrentViewModel.RenderWidth=(int)InnerECGPanel.ActualWidth;
            CurrentViewModel.RenderHeight=(int)InnerECGPanel.ActualHeight;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateSize();

        }

        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {

                var cabSender = sender  as IInputElement;
                //_clickPoint=new Point(scroll.ActualWidth/2,scroll.ActualHeight/2); 
                double x;
                double y;
                Point p = e.MouseDevice.GetPosition(cabSender);

                x = _clickPoint.X - p.X;
                y = _clickPoint.Y - p.Y;


                scroller.ScrollToHorizontalOffset(scroller.HorizontalOffset+x);

                scroller.ScrollToVerticalOffset(scroller.VerticalOffset+y);



            }
        }


        private void Content_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _clickPoint = e.GetPosition(sender as IInputElement);
            this.Cursor = Cursors.Hand;
        }

        private void Content_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }
    }

}

