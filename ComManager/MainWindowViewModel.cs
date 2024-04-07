using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO.Ports;
using Prism.Commands;

namespace ComManager
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private SerialPort serialPort;

        public ObservableCollection<string> AvailablePorts { get; set; }
        public ObservableCollection<int> BaudRates { get; set; }
        // Other settings like DataBits, StopBits, ParityBits can be added similarly

        private string _dataOut;
        public string DataOut
        {
            get { return _dataOut; }
            set
            {
                if (_dataOut != value)
                {
                    _dataOut = value;
                    OnPropertyChanged(nameof(DataOut));
                }
            }
        }
        public string SelectedPort { get; set; }
        public int SelectedBaudRate { get; set; }
        // SelectedDataBits, SelectedStopBits, SelectedParityBits

        public ICommand ConnectCommand { get; private set; }
        public ICommand DisconnectCommand { get; private set; }
        public ICommand SendDataCommand { get; private set; }



        public MainWindowViewModel()
        {
            AvailablePorts = new ObservableCollection<string>(SerialPort.GetPortNames());
            BaudRates = new ObservableCollection<int>(new[] { 9600, 19200, 38400, 57600, 115200 });

            ConnectCommand = new DelegateCommand(Connect);
            DisconnectCommand = new DelegateCommand(Disconnect);
            SendDataCommand = new DelegateCommand(SendData);

        }

        private void SendData()
        {
            if (serialPort.IsOpen)
            {
                Debug.WriteLine("Sending data..");
                serialPort.WriteLine(_dataOut);
            }

        }

        private void Connect()
        {
            if (serialPort != null && serialPort.IsOpen)
                return;

            serialPort = new SerialPort(SelectedPort, SelectedBaudRate);
            // Add other settings like DataBits, StopBits, Parity
            serialPort.DataReceived += SerialPort_DataReceived;
            serialPort.Open();
            Debug.WriteLine("Port opened.");
        }

        private void Disconnect()
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort.ReadExisting();
            // Handle received data
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}