using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;
using Newmark_Stage_Control;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Newmark_Stage_Control
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        [DllImport(@"PerformaxCom.dll")]
        public static extern int fnPerformaxComGetNumDevices(out UInt32 lpwdNumDevices);

        newmark nm = new newmark();
        IntPtr hUSBDevice;
        private SerialPort serialPortArduino=new SerialPort();
        private string arduinoTrgID="trg01";
        private string arduinoPortName="";

        private void Form1_Load(object sender, EventArgs e)
        {
            uint deviceNumber;
            int status = fnPerformaxComGetNumDevices(out deviceNumber);

            label1.Text = deviceNumber.ToString();
            label2.Text = string.Join(" ", nm.GetDeviceList());

            timerArduino.Start();
        }

        private void btnConnectStage_Click(object sender, EventArgs e)
        {
            // Add your button click logic here
            connectStage();
        }

        private void connectStage()
        {
            
            string device = getNewmarkDevice();

            if (device != "")
            {
                bool connectionStatus = nm.connectToDevice(nm.resolveDeviceIndex(device), out hUSBDevice);

                if (connectionStatus == false)
                {
                    MessageBox.Show("Failed to connect with the device... Please check the power or USB connection");
                }
                else  // true status.. connected to the device...
                {
                    btnConnectStage.Text = "Disconnect";
                    //lb_statusStrip.Text = CB_DeviceList.SelectedItem.ToString() + " is connected successfully!";
                }
            }
        }

        private string getNewmarkDevice()
        {
            string[] deviceList = nm.GetDeviceList();

            uint deviceNumber;
            int status = fnPerformaxComGetNumDevices(out deviceNumber);


            string[] device = { "" };

            if (deviceNumber > 0)
            {
                device = nm.GetDeviceList();
            }
            //MessageBox.Show(device[0]);
            return device[0];
        }

        private string sendCommand(string commandStr)
        {
            string reply = "";
            string tmpString = "";
            bool status;
            status = nm.performaxSendAndGetReply(hUSBDevice, commandStr, out tmpString);
            if (status)
            {
                reply= tmpString ;
            }
            else
            {
                MessageBox.Show("Warning.... send command error ~~");
            }
            return reply;
        }

        private void btnAbs_Click(object sender, EventArgs e)
        {
            moveAbs();
        }
        private void moveAbs()
        {
            string abs=txtbxAbs.Text;
            bool isNumber = double.TryParse(abs, out double result);
            if (isNumber)
            {
                string command = "X" + result.ToString();
                sendCommand(command);
            }
        }

        private void txtbxAbs_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnAbs.PerformClick();
            }
        }

        private void timerArduino_Tick(object sender, EventArgs e)
        {
            SerialPort.GetPortNames();
            SerialPort.GetPortNames();
            listBox1.Items.Clear();
            foreach (string dummyPort in SerialPort.GetPortNames())
            {
                listBox1.Items.Add(dummyPort);
            }


            //int dwFlagsAndAttributes = 0x40000000;
            //SafeFileHandle hFile = CreateFile(@"\\.\" + arduinoPortName, -1073741824, 0, IntPtr.Zero, 3, dwFlagsAndAttributes, IntPtr.Zero);
            bool isPortValid = SerialPort.GetPortNames().Any(x => string.Compare(x, arduinoPortName, true) == 0);
            lblSerialPort.Text = isPortValid.ToString();
            if (!isPortValid)
            {
                arduinoPortName = "";
                pnlArduinoConn.BackColor = Color.DarkGray;

                if (connectArduino())
                    pnlArduinoConn.BackColor = Color.Green;
                else
                {
                    pnlArduinoConn.BackColor = Color.DarkGray;
                }
            }
            else
            {
                //pnlArduinoConn.BackColor = Color.Green;
            }
        }

        private bool connectArduino()
        {
            string[] avlportNames = SerialPort.GetPortNames();
            int baudrate = 9600;
            foreach (string portName in avlportNames)
            {
                try
                {
                    serialPortArduino = new SerialPort(portName, baudrate);
                    serialPortArduino.Open();
                    serialPortArduino.WriteLine(arduinoTrgID);
                    Thread.Sleep(100);
                    MessageBox.Show("Connected");
                    // Check arduino correct
                    arduinoPortName = portName;
                    return true;
                }
                catch { 

                }

            }
            return false;
        }

       
    }
}
