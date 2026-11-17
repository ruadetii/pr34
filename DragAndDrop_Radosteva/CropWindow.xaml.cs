using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace DragAndDrop_Radosteva
{
    public partial class CropWindow : Window
    {
        private BitmapImage bitmap;
        private bool isMoving;    
        private bool isResizing;  
        private Point start;      

        public CropWindow()
        {
            InitializeComponent();
            SetPhoto(new BitmapImage(new Uri("pack://application:,,,/Image/ic-button.png")));
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Изображения|*.png;*.jpg;*.jpeg;*.bmp";
            if (dialog.ShowDialog() == true)
                SetPhoto(new BitmapImage(new Uri(dialog.FileName)));
        }

        private void SetPhoto(BitmapImage image)
        {
            bitmap = image;
            double scale = Math.Min(canvas.Width / bitmap.PixelWidth, canvas.Height / bitmap.PixelHeight);
            photo.Source = bitmap;
            photo.Width = bitmap.PixelWidth * scale;
            photo.Height = bitmap.PixelHeight * scale;

            frame.Width = photo.Width / 2;
            frame.Height = photo.Height / 2;
            Canvas.SetLeft(frame, photo.Width / 4);
            Canvas.SetTop(frame, photo.Height / 4);
            UpdateHandle();
            result.Source = null;
        }

        private void Frame_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isMoving = true;
            start = e.GetPosition(frame);
            canvas.CaptureMouse();
        }

        private void Handle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isResizing = true;
            canvas.CaptureMouse();
            e.Handled = true;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(canvas);
            double left = Canvas.GetLeft(frame);
            double top = Canvas.GetTop(frame);

            if (isMoving)
            {
                left = Math.Max(0, Math.Min(p.X - start.X, photo.Width - frame.Width));
                top = Math.Max(0, Math.Min(p.Y - start.Y, photo.Height - frame.Height));
                Canvas.SetLeft(frame, left);
                Canvas.SetTop(frame, top);
            }
            else if (isResizing)
            {
                frame.Width = Math.Max(20, Math.Min(p.X, photo.Width) - left);
                frame.Height = Math.Max(20, Math.Min(p.Y, photo.Height) - top);
            }
            UpdateHandle();
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isMoving = false;
            isResizing = false;
            canvas.ReleaseMouseCapture();
        }

        private void UpdateHandle()
        {
            Canvas.SetLeft(handle, Canvas.GetLeft(frame) + frame.Width - handle.Width / 2);
            Canvas.SetTop(handle, Canvas.GetTop(frame) + frame.Height - handle.Height / 2);
        }

        private void Crop_Click(object sender, RoutedEventArgs e)
        {
            double k = bitmap.PixelWidth / photo.Width;
            int x = (int)(Canvas.GetLeft(frame) * k);
            int y = (int)(Canvas.GetTop(frame) * k);
            int w = Math.Min((int)(frame.Width * k), bitmap.PixelWidth - x);
            int h = Math.Min((int)(frame.Height * k), bitmap.PixelHeight - y);
            if (w <= 0 || h <= 0) return;

            result.Source = new CroppedBitmap(bitmap, new Int32Rect(x, y, w, h));
        }
    }
}
