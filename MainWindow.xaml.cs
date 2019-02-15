using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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

namespace TcpClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeSettings();
            txtOutput.TextChanged += (se, ev) =>
            {
                txtOutput.ScrollToEnd();
            };
        }

        void SaveSettings()
        {
            Properties.Settings.Default.IP = txtIp.Text;
            var port = Properties.Settings.Default.Port;
            if (int.TryParse(txtPort.Text, out port))
            {
                Properties.Settings.Default.Port = port;
            }
            Properties.Settings.Default.EncodingType = ((ComboBoxItem)(cmbEncodingType.SelectedValue)).Content.ToString();
            Properties.Settings.Default.Input = txtInput.Text;
            Properties.Settings.Default.Save();
        }

        private void InitializeSettings()
        {
            txtIp.Text = Properties.Settings.Default.IP;
            txtPort.Text = Properties.Settings.Default.Port.ToString();
            foreach (var item in cmbEncodingType.Items)
            {
                if (((ComboBoxItem)item).Content.ToString() == Properties.Settings.Default.EncodingType)
                    cmbEncodingType.SelectedItem = item;
            }
            txtInput.Text = Properties.Settings.Default.Input;
        }

        private void ClearTextInput(object sender, RoutedEventArgs e)
        {
            txtInput.Text = string.Empty;
        }
        private void ClearTextOutput(object sender, RoutedEventArgs e)
        {
            txtOutput.Text = string.Empty;
        }
        private Encoding Enc {
            get {
                return Enco(((ComboBoxItem)(cmbEncodingType.SelectedItem)).Content.ToString());
            }
        }
        private Encoding Enco(string encoding)
        {
            switch (encoding)
            {
                case "UTF-8":
                    return Encoding.UTF8;
                case "ASCII":
                    return Encoding.ASCII;
                case "Unicode":
                    return Encoding.Unicode;
                case "UTF-7":
                    return Encoding.UTF7;
                case "UTF-32":
                    return Encoding.UTF32;
            }
            return Encoding.UTF8;
        }

        private void Log(string message)
        {
            message = Environment.NewLine
                        + $"[{DateTime.Now.ToString("HH:mm:ss")}]: "
                        + message
                        + Environment.NewLine;
            txtOutput.Text += message;
        }

       
        private void SendMessage(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            try
            {
                var hostname = txtIp.Text;
                var port = Convert.ToInt32(txtPort.Text);
                var client = new System.Net.Sockets.TcpClient(hostname, port);
                Log($"Connected to {hostname}:{port}");

                string message = txtInput.Text;
                Byte[] data = Enc.GetBytes(message);
                NetworkStream stream = client.GetStream();

                stream.Write(data, 0, data.Length);

                Log($"Sent: {message}");

                data = new Byte[256];
                String responseData = String.Empty;

                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = Enc.GetString(data, 0, bytes);
                Log($"Received: {responseData}");

                stream.Close();

            }
            catch (ArgumentNullException ex)
            {
                Log($"ArgumentNullException: {ex}");
            }
            catch (SocketException ex)
            {
                Log($"SocketException: {ex}");
            }
        }

        private void Port_KeyDown(object sender, KeyEventArgs e)
        {
            var allowedKeys = new List<Key>()
            {
                Key.Back,
                Key.D0,
                Key.D1,
                Key.D2,
                Key.D3,
                Key.D4,
                Key.D5,
                Key.D6,
                Key.D7,
                Key.D8,
                Key.D9,
                Key.NumPad0,
                Key.NumPad1,
                Key.NumPad2,
                Key.NumPad3,
                Key.NumPad4,
                Key.NumPad5,
                Key.NumPad6,
                Key.NumPad7,
                Key.NumPad8,
                Key.NumPad9,
            };
            if (!allowedKeys.Contains(e.Key))
                e.Handled = true;
        }

        private void Port_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPort.Text))
                txtPort.Text = "0";
        }
    }
}
