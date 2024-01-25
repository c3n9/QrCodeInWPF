using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace QrCodeInWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BGenerate_Click(object sender, RoutedEventArgs e)
        {
            // Получаем текст для создания QR-кода из TextBox
            string qrText = TBLabel.Text;
            // Создаем объект-кодировщик QR-кода
            QRCodeEncoder encoder = new QRCodeEncoder();
            // Кодируем текст в QR-код
            Bitmap qrcode = encoder.Encode(qrText);
            // Сохраняем QR-код в память в формате BMP
            using (MemoryStream memory = new MemoryStream())
            {
                qrcode.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                // Создаем объект BitmapImage и загружаем в него QR-код из памяти
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                // Устанавливаем QR-код как источник для Image control (IQRCode)
                IQRCode.Source = bitmapimage;
            }
        }

        private void BLoad_Click(object sender, RoutedEventArgs e)
        {
            // Открываем диалог выбора файла
            var dialog = new OpenFileDialog() { Filter = ".png; .jpeg; .jpg; | *.png; *.jpeg; *.jpg;" };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                try
                {
                    // Считываем байты из выбранного файла
                    var imageBytes = File.ReadAllBytes(dialog.FileName);
                    // Создаем объект BitmapImage и загружаем в него изображение из байтов
                    using (MemoryStream memory = new MemoryStream(imageBytes))
                    {
                        var bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = memory;
                        bitmapImage.EndInit();

                        // Устанавливаем загруженное изображение как источник для Image control (IQRCode)
                        IQRCode.Source = bitmapImage;
                        // Создаем объект Bitmap из байтов для декодирования QR-кода
                        using (MemoryStream stream = new MemoryStream(imageBytes))
                        {
                            var bitmap = new System.Drawing.Bitmap(stream);
                            // Декодируем QR-код и устанавливаем результат в TextBox (TBLabel)
                            QRCodeDecoder decoder = new QRCodeDecoder();
                            TBLabel.Text = decoder.Decode(new QRCodeBitmapImage(bitmap));
                        }
                    
                    }
                }
                catch(Exception ex)
                {
                    
                }
            }
        }

        private void BSave_Click(object sender, RoutedEventArgs e)
        {
            // Открываем диалог сохранения файла
            var dialog = new SaveFileDialog() { Filter = ".png; .jpeg; .jpg; | *.png; *.jpeg; *.jpg;" };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                // Создаем файл и закрываем его, чтобы получить пустой файл
                var file = File.Create(dialog.FileName);
                file.Close();
                // Получаем BitmapSource изображения из Image control (IQRCode)
                BitmapSource bitmapSource = (BitmapSource)IQRCode.Source;
                // Создаем кодировщик изображения в формат JPEG
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                // Сохраняем изображение в файл в формате JPEG
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    encoder.Save(memoryStream);
                    File.WriteAllBytes(dialog.FileName, memoryStream.ToArray());
                }
            }
        }

    }
}