using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Newmark_Stage_Control
{
    class newmark
    {
        [DllImport(@"C:\WINDOWS\syswow64\PerformaxCom.dll")]
        public static extern int fnPerformaxComGetNumDevices(out UInt32 lpwdNumDevices);

        [DllImport(@"C:\WINDOWS\syswow64\PerformaxCom.dll")]
        public static extern int fnPerformaxComGetProductString(uint dwDeviceNum, out byte lpvDeviceString, uint dwFlags);

        [DllImport(@"C:\WINDOWS\syswow64\PerformaxCom.dll")]
        public static extern bool fnPerformaxComOpen(uint dwDeviceNum, out IntPtr pHandle);

        [DllImport(@"C:\WINDOWS\syswow64\PerformaxCom.dll")]
        public static extern bool fnPerformaxComClose(IntPtr pHandle);

        [DllImport(@"C:\WINDOWS\syswow64\PerformaxCom.dll")]
        public static extern bool fnPerformaxComSetTimeouts(uint dwReadTimeout, uint dwWriteTimeout);

        [DllImport(@"C:\WINDOWS\syswow64\PerformaxCom.dll")]
        public static extern bool fnPerformaxComSendRecv(IntPtr pHandle, byte[] wBuffer, uint dwNumBytesToWrite, uint dwNumBytesToRead, out byte rBuffer);


        public const byte PERFORMAX_SUPPORTED_DEVICE = 0x05;
        public const byte PERFORMAX_RETURN_SERIAL_NUMBER = 0x00;
        public const byte PERFORMAX_RETURN_DESCRIPTION = 0x01;
        public const byte PERFORMAX_MAX_DEVICE_STRLEN = 255;
        public const byte INVALID_HANDLE_VALUE = 0x01;
        public const byte PERFORMAX_CMD_RESPONSE_LENGTH = 64;


        public string[] GetDeviceList()
        {
            // declare useful variables:
            uint deviceNumber;
            uint deviceCounter;
            byte[] deviceStringArray = new byte[PERFORMAX_MAX_DEVICE_STRLEN];
            string[] newDeviceString = new string[PERFORMAX_SUPPORTED_DEVICE];
            int status;



            status = fnPerformaxComGetNumDevices(out deviceNumber);

            //            MessageBox.Show(deviceNumber.ToString());

            if (status == 1 && deviceNumber>0)
            {

                for (deviceCounter = 0; deviceCounter <= deviceNumber - 1; deviceCounter++)
                {

                    status = fnPerformaxComGetProductString(deviceCounter, out deviceStringArray[0], PERFORMAX_RETURN_SERIAL_NUMBER);

                    System.Text.ASCIIEncoding encByteArray = new System.Text.ASCIIEncoding();

                    newDeviceString[deviceCounter] = encByteArrayToString(deviceStringArray);
                    newDeviceString[deviceCounter] += ", Index=" + deviceCounter.ToString();

                    fnPerformaxComSetTimeouts(1000, 1000);

                }

            }
            else
            {
                // MessageBox.Show("No Performax Device is connected to USB!", "Connection Error ~~");
                newDeviceString[0] = "No Performax Device is connected to USB! Connection Error ~~";
            }

            return newDeviceString;

        }

        public bool connectToDevice(uint deviceNumber, out IntPtr hUSBDevice)
        {
            return fnPerformaxComOpen(deviceNumber, out hUSBDevice);
        }

        public bool disconnectToDevice(IntPtr hUSBDevice)
        {
            return fnPerformaxComClose(hUSBDevice);
        }

        public bool performaxSendAndGetReply(IntPtr hUSBDevice, string cmdStr, out string replyStr)
        {
            // Define byte array for sending cmd and getting reply response
            bool status;
            byte[] sendStr = new byte[PERFORMAX_CMD_RESPONSE_LENGTH];
            byte[] replyBuffer = new byte[PERFORMAX_CMD_RESPONSE_LENGTH];


            sendStr.Initialize();

            replyStr = "";

            if (cmdStr.Length > 64)
            {
                return false;
            }

            // Converting the cmdStr to 64 byte array ~~

            for (int tmpCount = 0; tmpCount < cmdStr.Length; tmpCount++)
            {
                sendStr[tmpCount] = Convert.ToByte(Convert.ToChar(cmdStr.Substring(tmpCount, 1)));
            }
            for (int tmpCount = sendStr.Length; tmpCount < PERFORMAX_CMD_RESPONSE_LENGTH; tmpCount++)
            {
                sendStr[tmpCount] = 0;
            }


            //String to byte array
            /*
            System.Text.ASCIIEncoding encStr = new System.Text.ASCIIEncoding();
            sendStr = encStr.GetBytes(cmdStr);

            MessageBox.Show(sendStr.Length.ToString());
            */


            // Send the cmd out and get using the fnPerformaxComSendRecv
            status = fnPerformaxComSendRecv(hUSBDevice, sendStr, 64, 64, out replyBuffer[0]);

            replyStr = encByteArrayToString(replyBuffer);

            return status;
        }

        private string encByteArrayToString(byte[] inputByteArray)
        {
            string tmpStr = "";
            for (uint i = 0; (i < inputByteArray.Length) & (inputByteArray[i] != 0); i++)
            {
                tmpStr += Convert.ToChar(inputByteArray[i]);
            }

            return tmpStr;
        }

        public uint resolveDeviceIndex(string inputDeviceString)
        {
            int deviceIndex = inputDeviceString.IndexOf("=") + 1;
            inputDeviceString = inputDeviceString.Substring(deviceIndex, inputDeviceString.Length - deviceIndex);
            deviceIndex = Convert.ToInt16(inputDeviceString);

            return Convert.ToUInt32(deviceIndex);

        }

    }
}
