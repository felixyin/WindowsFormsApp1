using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace felixyin
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //用于序列化和反序列化的对象
            IFormatter serializer = new BinaryFormatter();
            bool isRegister = false;
            //反序列化
            try
            {
                string homePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
                     Environment.OSVersion.Platform == PlatformID.MacOSX)
                      ? Environment.GetEnvironmentVariable("HOME")
                      : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");

                if (File.Exists(homePath+"/.ap_serial"))
                {
                    FileStream loadFile = new FileStream(homePath + "/.ap_serial", FileMode.Open, FileAccess.Read);
                    string auth_code_by_file = serializer.Deserialize(loadFile) as string;
                    loadFile.Close();
                    SoftReg sr = new SoftReg();
                    string serialText = sr.getRNum();
                    string auth_code = sr.MD5Encrypt(sr.MD5Encrypt(serialText));

                    isRegister = auth_code_by_file == auth_code;
                }
                else
                {
                    isRegister = false;
                }
            }
            catch (FileNotFoundException e)
            {
                isRegister = false;
            }

            if (isRegister)
            {
                MainForm mainForm = new MainForm();
                Application.Run(mainForm);
            }
            else// 如果没有注册,则弹出注册对话框
            {
                RegisterForm mainForm = new RegisterForm();
                Application.Run(mainForm);
            }
        }
    }
}
