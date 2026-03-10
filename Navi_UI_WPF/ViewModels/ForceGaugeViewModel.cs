using System;
using System.IO;
using System.IO.Ports;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveCharts;
using LiveCharts.Wpf;

namespace Navi_UI_WPF.ViewModels
{
    public class ForceGaugeViewModel : ObservableObject
    {
        // ─── Chart ───────────────────────────────────────────────────────────

        private const int MaxPoints = 100;
        private int _pointIndex = 0;

        public SeriesCollection SeriesCollection { get; set; }

        /// Formatter cho trục X — hiển thị index mỗi điểm
        public Func<double, string> XFormatter { get; set; } = val => val.ToString("N0");

        // ─── Serial Port ─────────────────────────────────────────────────────

        private SerialPort _serialPort;

        // Frame protocol: ASCII hex text, each frame = 14 characters
        //   "4159" = header
        //   chars 4-5 = mode: "02"=dương(+), "12"=âm(-)
        //   chars 6-11 = "000000" padding
        //   chars 12-13 = force value hex, BCD-encoded ("13"→13g, "48"→48g)
        private const string FrameHeader = "4159";
        private const int    FrameLen    = 14;  // 14 ASCII hex chars = 7 bytes
        private string _strBuffer = string.Empty;

        // ─── Thông tin hàng ──────────────────────────────────────────────────

        private string _poNumber;
        public string PoNumber
        {
            get => _poNumber;
            set => SetProperty(ref _poNumber, value);
        }

        private string _productName;
        public string ProductName
        {
            get => _productName;
            set => SetProperty(ref _productName, value);
        }

        private string _lotNumber;
        public string LotNumber
        {
            get => _lotNumber;
            set => SetProperty(ref _lotNumber, value);
        }

        private string _quantity;
        public string Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }

        // ─── Block 1 ─────────────────────────────────────────────────────────

        private string _block1SizeBall;
        public string Block1SizeBall
        {
            get => _block1SizeBall;
            set => SetProperty(ref _block1SizeBall, value);
        }

        private string _block1LucMin;
        public string Block1LucMin
        {
            get => _block1LucMin;
            set => SetProperty(ref _block1LucMin, value);
        }

        private string _block1LucMax;
        public string Block1LucMax
        {
            get => _block1LucMax;
            set => SetProperty(ref _block1LucMax, value);
        }

        // ─── Block 2 ─────────────────────────────────────────────────────────

        private string _block2SizeBall;
        public string Block2SizeBall
        {
            get => _block2SizeBall;
            set => SetProperty(ref _block2SizeBall, value);
        }

        private string _block2LucMin;
        public string Block2LucMin
        {
            get => _block2LucMin;
            set => SetProperty(ref _block2LucMin, value);
        }

        private string _block2LucMax;
        public string Block2LucMax
        {
            get => _block2LucMax;
            set => SetProperty(ref _block2LucMax, value);
        }

        // ─── Kết nối ─────────────────────────────────────────────────────────

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (SetProperty(ref _isConnected, value))
                {
                    OnPropertyChanged(nameof(ConnectionStatus));
                    OnPropertyChanged(nameof(ConnectionButtonColor));
                }
            }
        }

        public string ConnectionStatus      => IsConnected ? "CONNECTED"  : "DISCONNECTED";
        public string ConnectionButtonColor => IsConnected ? "#4CAF50"    : "#616161";

        private string _result1 = "";
        public string Result1
        {
            get => _result1;
            set => SetProperty(ref _result1, value);
        }

        private string _result2 = "";
        public string Result2
        {
            get => _result2;
            set => SetProperty(ref _result2, value);
        }

        // ─── Commands ────────────────────────────────────────────────────────

        public ICommand ConnectCommand { get; }
        public ICommand SaveCommand    { get; }

        // ─── Constructor ─────────────────────────────────────────────────────

        public ForceGaugeViewModel()
        {
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title           = "Lực (g)",
                    Values          = new ChartValues<double>(),
                    PointGeometry   = null,
                    StrokeThickness = 2
                }
            };

            ConnectCommand = new RelayCommand(Connect);
            SaveCommand    = new RelayCommand(Save);
        }

        // ─── Connect / Disconnect ────────────────────────────────────────────

        private void Connect()
        {
            if (!IsConnected) OpenPort();
            else              ClosePort();
        }

        private void OpenPort()
        {
            try
            {
                _strBuffer = string.Empty;
                _pointIndex = 0;
                ((ChartValues<double>)SeriesCollection[0].Values).Clear();

                _serialPort = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One)
                {
                    ReadTimeout  = 500,
                    WriteTimeout = 500
                };
                _serialPort.DataReceived += SerialPort_DataReceived;

                if (!_serialPort.IsOpen)
                    _serialPort.Open();

                IsConnected = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("SerialPort Error: " + ex.Message, "Lỗi kết nối",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                CleanupPort();
            }
        }

        private void ClosePort()
        {
            CleanupPort();
            IsConnected = false;
        }

        private void CleanupPort()
        {
            if (_serialPort == null) return;
            try
            {
                _serialPort.DataReceived -= SerialPort_DataReceived;
                if (_serialPort.IsOpen)
                    _serialPort.Close();
            }
            catch { /* ignore */ }
            finally
            {
                _serialPort.Dispose();
                _serialPort = null;
            }
        }

        // ─── Serial → byte buffer ────────────────────────────────────────────

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = _serialPort.ReadExisting();
                if (!string.IsNullOrEmpty(data))
                    ParseFrames(data);
            }
            catch (TimeoutException) { }
            catch (IOException ex)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    MessageBox.Show("Mất kết nối: " + ex.Message, "Lỗi",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    ClosePort();
                }));
            }
        }

        // ─── Frame parser (ASCII hex text) ──────────────────────────────────

        /// <summary>
        /// Frame format (14 ASCII hex chars = 7 bytes):
        ///   "4159" + "XX" + "000000" + "YY"
        ///   mode XX: "02" = dương (+), "12" = âm (-)
        ///   value YY: BCD hex  e.g. "13"->13g, "48"->48g
        /// </summary>
        private void ParseFrames(string incoming)
        {
            _strBuffer += incoming;

            while (_strBuffer.Length >= FrameLen)
            {
                // Tìm header "4159"
                int idx = _strBuffer.IndexOf(FrameHeader, System.StringComparison.Ordinal);

                if (idx < 0)
                {
                    // Không có header — giữ lại 3 ký tự cuối phòng trường hợp header bị chia cắt
                    _strBuffer = _strBuffer.Length > 3
                        ? _strBuffer.Substring(_strBuffer.Length - 3)                                           
                        : _strBuffer;
                    break;
                }

                if (idx > 0)
                {
                    // Bỏ rác trước header
                    _strBuffer = _strBuffer.Substring(idx);
                }

                if (_strBuffer.Length < FrameLen) break;

                // Lấy đủ một frame
                string frame = _strBuffer.Substring(0, FrameLen);
                _strBuffer = _strBuffer.Substring(FrameLen);

                string modeHex  = frame.Substring(4, 2);   // "02" or "12"
                string valueHex = frame.Substring(12, 2);  // e.g. "13", "48"

                // Parse value BCD
                if (byte.TryParse(valueHex, System.Globalization.NumberStyles.HexNumber,
                                  null, out byte valueByte))
                {
                    int hi = (valueByte >> 4) & 0x0F;
                    int lo =  valueByte       & 0x0F;
                    double force = hi * 10.0 + lo;
                    if (modeHex == "12") force = -force;  // âm

                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var values = (ChartValues<double>)SeriesCollection[0].Values;
                        if (values.Count >= MaxPoints)
                            values.RemoveAt(0);
                        _pointIndex++;
                        values.Add(force);
                    }));
                }
            }
        }

        // ─── Save ────────────────────────────────────────────────────────────

        private void Save()
        {
            // TODO: implement save logic
        }
    }
}
