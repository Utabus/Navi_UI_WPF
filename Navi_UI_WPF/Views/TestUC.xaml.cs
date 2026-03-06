using System;
using System.IO;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;

namespace Navi_UI_WPF.Views
{
    /// <summary>
    /// Interaction logic for TestUC.xaml
    /// </summary>
    public partial class TestUC : UserControl
    {
        SerialPort serialPort;
        public TestUC()
        {
            InitializeComponent(); SetupSerialPort();
        }
        private void SetupSerialPort()
        {
            serialPort = new SerialPort("COM5", 9600, Parity.None, 8, StopBits.One);

            serialPort.DataReceived += SerialPort_DataReceived;

            try
            {
                if (!serialPort.IsOpen)
                    serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("SerialPort Error: " + ex.Message);
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = serialPort.ReadExisting();

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    txtReceiver.AppendText(data);
                }));
            }
            catch (TimeoutException)
            {
                // ignore
            }
            catch (IOException ex)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    MessageBox.Show("Connection lost: " + ex.Message);
                }));
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }
    }
}