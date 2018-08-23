using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.Threading;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.webBrowser1.ObjectForScripting = this;
            this.webBrowser1.Url = new Uri("file://D:/avi_player/test4.html");
            /**
              
             // 调用JavaScript的messageBox方法，并传入参数
    object[] objects = new object[1];
    objects[0] = "c#diao javascript";
    webBrowser1.Document.InvokeScript("Messageaa", objects);
             */
        }

        public void FindFile2(List<String> list,string sSourcePath)

        {

            //遍历文件夹

            DirectoryInfo theFolder = new DirectoryInfo(sSourcePath);
 

            FileInfo[] thefileInfo = theFolder.GetFiles("*.*", SearchOption.TopDirectoryOnly);

            foreach (FileInfo NextFile in thefileInfo)  //遍历文件

                if(NextFile.FullName.Contains(".avi"))
                    list.Add(NextFile.FullName);


            //遍历子文件夹

            DirectoryInfo[] dirInfo = theFolder.GetDirectories();

            foreach (DirectoryInfo NextFolder in dirInfo)

            {

                //list.Add(NextFolder.ToString());

              

                FileInfo[] fileInfo = NextFolder.GetFiles("*.*", SearchOption.AllDirectories);

                foreach (FileInfo NextFile in fileInfo)  //遍历文件

                    if (NextFile.FullName.Contains(".avi"))
                        list.Add(NextFile.FullName);


            }
            
        }

        public string selectFolder(string message)
        {
            List<string> x = new List<string>();
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                if (this.folderBrowserDialog1.SelectedPath.Trim() != "")
                {
                    string folder = this.folderBrowserDialog1.SelectedPath.Trim();
                    List<String> list = new List<string>();
                    this.FindFile2(list,folder);
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    string jsonData = js.Serialize(list);//序列化
                    return jsonData;
                }
            }
            return "";
        }

        public void sleep(int t)
        {
            Thread.Sleep(t);
        }
    }
}
