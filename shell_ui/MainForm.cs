using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.ComponentModel;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace felixyin
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            // FIXME 生产环境，需要修改为4404
            string url = "http://localhost:4404";
            int bv = GetBrowserVersion();
            if (bv >= 8)// 浏览器版本不能小于8
            {
                bool isOk = ResponseOk(url);
                if (isOk)
                {
                    SetWebBrowserFeatures(11);
                    InitializeComponent();
                    this.webBrowser1.ObjectForScripting = this;
                    this.webBrowser1.Url = new Uri(url);
                }
                else
                {
                    DialogResult result = MessageBox.Show("程序发生未知错误，请联系管理员：15965585803", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (result == DialogResult.OK)
                    {
                        System.Environment.Exit(0);
                    }
                }
            }
            else
            {
                DialogResult result = MessageBox.Show("程序使用了IE组件，要求IE版本不能小于8，请先升级您的IE浏览器", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    System.Environment.Exit(0);
                }
            }

        }

        /// <summary>   
        /// Get方式获取url地址输出内容   
        /// </summary> /// <param name="url">url</param>   
        public static bool ResponseOk(String url)
        {
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "GET";
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                Console.WriteLine(webResponse.StatusCode);
                return true;
            }
            catch
            {
                return false;
            }

        }

        /**
        * 遍历文件夹，查找返回所有avi视频文件
        */
        public void FindFile4(List<Dictionary<string, string>> list, string sSourcePath)
        {
            //遍历文件夹
            DirectoryInfo theFolder = new DirectoryInfo(sSourcePath);
            FileInfo[] thefileInfo = theFolder.GetFiles("*.*", SearchOption.TopDirectoryOnly);

            string lmi1 = "";
            string avi1 = "";
            foreach (FileInfo NextFile in thefileInfo)
            {  //遍历文件
                string fullName1 = NextFile.FullName;
                if (fullName1.Contains(".lmi"))
                {
                    lmi1 = fullName1;
                }
                if (fullName1.Contains(".avi"))
                {
                    avi1 = fullName1;
                }
            }
            if (lmi1 != "" && avi1 != "")
            {
                Dictionary<string, string> map = new Dictionary<string, string>();
                map.Add("avi_path", avi1);
                map.Add("lmi_path", lmi1);
                list.Add(map);
            }

            //遍历子文件夹
            DirectoryInfo[] dirInfo = theFolder.GetDirectories();
            foreach (DirectoryInfo NextFolder in dirInfo)
            {
                FindFile4(list, NextFolder.FullName);
            }
        }



        /**
         * 将从视频文件中获取的视频信息转化为json
         */
        public string selectFolder()
        {
            List<string> x = new List<string>();
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                if (this.folderBrowserDialog1.SelectedPath.Trim() != "")
                {
                    string folder = this.folderBrowserDialog1.SelectedPath.Trim();

                    List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
                    this.FindFile4(list, folder);

                    string jsonData = JsonConvert.SerializeObject(list);//序列化

                    Console.WriteLine(jsonData);
                    return jsonData;
                }
            }
            return "";
        }

        public string selectXlsx()
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (this.openFileDialog1.FileName.Trim() != "")
                {
                    string folder = this.openFileDialog1.FileName.Trim();
                    return folder;
                }
            }
            return "";
        }

        public string selectOtherVideo()
        {
            if (this.openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                if (this.openFileDialog2.FileName.Trim() != "")
                {
                    string folder = this.openFileDialog2.FileName.Trim();
                    return folder;
                }
            }
            return "";
        }

        /// <summary> 
        /// DataSet转换成Json格式 
        /// </summary> 
        /// <paramname="ds">DataSet</param> 
        ///<returns></returns> 
        public static string DatasetToJson(DataSet ds, int total = -1)
        {
            StringBuilder json = new StringBuilder();

            foreach (DataTable dt in ds.Tables)
            {
                //{"total":5,"rows":[ 
                json.Append("{\"total\":");
                if (total == -1)
                {
                    json.Append(dt.Rows.Count);
                }
                else
                {
                    json.Append(total);
                }
                json.Append(",\"rows\":[");
                json.Append(DataTableToJson(dt));
                json.Append("]}");
            }
            return json.ToString();
        }

        /// <summary> 
        /// dataTable转换成Json格式 
        /// </summary> 
        /// <paramname="dt"></param> 
        ///<returns></returns> 
        public static string DataTableToJson(DataTable dt)
        {
            StringBuilder jsonBuilder = new StringBuilder();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(dt.Rows[i][j].ToString());
                    jsonBuilder.Append("\",");
                }
                if (dt.Columns.Count > 0)
                {
                    jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                }
                jsonBuilder.Append("},");
            }
            if (dt.Rows.Count > 0)
            {
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            }

            return jsonBuilder.ToString();
        }



        /// <summary>  
        /// 修改注册表信息来兼容当前程序  
        ///   
        /// </summary>  
        static void SetWebBrowserFeatures(int ieVersion)
        {
            // don't change the registry if running in-proc inside Visual Studio  
            if (LicenseManager.UsageMode != LicenseUsageMode.Runtime)
                return;
            //获取程序及名称  
            var appName = System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            //得到浏览器的模式的值  
            UInt32 ieMode = GeoEmulationModee(ieVersion);
            var featureControlRegKey = @"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\";
            //设置浏览器对应用程序（appName）以什么模式（ieMode）运行  
            Registry.SetValue(featureControlRegKey + "FEATURE_BROWSER_EMULATION",
                appName, ieMode, RegistryValueKind.DWord);
            // enable the features which are "On" for the full Internet Explorer browser  
            //不晓得设置有什么用  
            Registry.SetValue(featureControlRegKey + "FEATURE_ENABLE_CLIPCHILDREN_OPTIMIZATION",
                appName, 1, RegistryValueKind.DWord);


            //Registry.SetValue(featureControlRegKey + "FEATURE_AJAX_CONNECTIONEVENTS",  
            //    appName, 1, RegistryValueKind.DWord);  


            //Registry.SetValue(featureControlRegKey + "FEATURE_GPU_RENDERING",  
            //    appName, 1, RegistryValueKind.DWord);  


            //Registry.SetValue(featureControlRegKey + "FEATURE_WEBOC_DOCUMENT_ZOOM",  
            //    appName, 1, RegistryValueKind.DWord);  


            //Registry.SetValue(featureControlRegKey + "FEATURE_NINPUT_LEGACYMODE",  
            //    appName, 0, RegistryValueKind.DWord);  
        }
        /// <summary>  
        /// 获取浏览器的版本  
        /// </summary>  
        /// <returns></returns>  
        static int GetBrowserVersion()
        {
            int browserVersion = 0;
            using (var ieKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer",
                RegistryKeyPermissionCheck.ReadSubTree,
                System.Security.AccessControl.RegistryRights.QueryValues))
            {
                var version = ieKey.GetValue("svcVersion");
                if (null == version)
                {
                    version = ieKey.GetValue("Version");
                    if (null == version)
                        throw new ApplicationException("Microsoft Internet Explorer is required!");
                }
                int.TryParse(version.ToString().Split('.')[0], out browserVersion);
            }
            //如果小于7  
            if (browserVersion < 7)
            {
                throw new ApplicationException("不支持的浏览器版本!");
            }
            return browserVersion;
        }
        /// <summary>  
        /// 通过版本得到浏览器模式的值  
        /// </summary>  
        /// <param name="browserVersion"></param>  
        /// <returns></returns>  
        static UInt32 GeoEmulationModee(int browserVersion)
        {
            UInt32 mode = 11000; // Internet Explorer 11. Webpages containing standards-based !DOCTYPE directives are displayed in IE11 Standards mode.   
            switch (browserVersion)
            {
                case 7:
                    mode = 7000; // Webpages containing standards-based !DOCTYPE directives are displayed in IE7 Standards mode.   
                    break;
                case 8:
                    mode = 8000; // Webpages containing standards-based !DOCTYPE directives are displayed in IE8 mode.   
                    break;
                case 9:
                    mode = 9000; // Internet Explorer 9. Webpages containing standards-based !DOCTYPE directives are displayed in IE9 mode.                      
                    break;
                case 10:
                    mode = 10000; // Internet Explorer 10.  
                    break;
                case 11:
                    mode = 11000; // Internet Explorer 11  
                    break;
            }
            return mode;
        }

        private void Form1_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            this.webBrowser1.Document.InvokeScript("beforeClose");
            DialogResult result = MessageBox.Show("您确定要关闭吗！", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.OK)
            {
                e.Cancel = false;  //点击OK   
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}
