using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Management;
using System.Threading;
using System.Text.RegularExpressions;



namespace HYTETestTool
{
    internal class UIHelper
    {
        private static readonly Lazy<UIHelper> _lazy = new Lazy<UIHelper>(() => new UIHelper());
        public static UIHelper Instance => _lazy.Value;
        private const string PIDVIDCHECKER = "VID_3402&PID_0400";
        private Detect state;
        private Action ToNonePage;
        private Action ToControlPage;
        private Action<string> SnNumberShowToUI;
        DeviceSetting Device;

        public void RegistryNonPage(Action NonePage)
        {
            ToNonePage = NonePage;
        }

        public void RegistryControlPage(Action ContPage)
        {
            ToControlPage = ContPage;
        }

        public void RegistrySnNumberUI(Action<string> SnFunction)
        {
            SnNumberShowToUI = SnFunction;
        }

        public void ResetBnNumber()
        {

            try
            {
                Device.ResetDevice();
                SnNumberShowToUI?.Invoke(Device.SnNumber);
            }
            catch 
            {
                
            }
        }

        public void WriteSnNumber(string SnNumber)
        {
            List<byte> ListByte = new List<byte>();
            ListByte.Add(0xff);
            ListByte.Add(0xaa);
            ListByte.Add(0xbb);
            ListByte.Add(0x01);
            foreach (char c in SnNumber)
            {
                int unicode = c;
                ListByte.Add(Convert.ToByte(unicode));
            }
            Device.WriteSnNumber(ListByte.ToArray());
            SnNumberShowToUI?.Invoke(Device.SnNumber);
        }


        public void StartProcess()
        {
            ToNonePage?.Invoke();
            new Thread(() => {
                while (true)
                {
                    bool bmatch = false;
                    using (var USBControllerDeviceCollection = new ManagementObjectSearcher("SELECT * FROM Win32_USBControllerDevice").Get())
                    {
                        if (USBControllerDeviceCollection != null)
                        {
                            foreach (ManagementObject USBControllerDevice in USBControllerDeviceCollection)
                            {
                                string Dependent = USBControllerDevice["Dependent"].ToString().Split(new char[] { '=' })[1];
                                USBControllerDevice.Dispose();
                                Match match = Regex.Match(Dependent, PIDVIDCHECKER);
                                if (match.Success)
                                {
                                    bmatch = true;
                                    break;
                                }
                                Thread.Sleep(1);
                            }
                        }
                    }
                    if (bmatch && state == Detect.None)
                    {
                        state = Detect.One;
                        Device = new DeviceSetting(PIDVIDCHECKER);
                        SnNumberShowToUI?.Invoke(Device.SnNumber);
                        ToControlPage?.Invoke();
                    }
                    if (!bmatch && state == Detect.One)
                    {
                        state = Detect.None;
                        ToNonePage?.Invoke();
                        Device?.Dispose();
                        Device = null;
                    }
                    Thread.Sleep(100);
                }
            })
            { IsBackground = true }.Start();
        }


        public Detect GetHelperState()
        {
            return state;
        }

        public UIHelper() 
        {
            state = Detect.None;
        }
    }
    enum Detect
    {
        None,
        One,
    }
}
