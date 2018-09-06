using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace felixyin
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
            SoftReg sr = new SoftReg();
            string rnum = sr.getRNum();
            this.keyTextBox.Text = rnum;
        }

        private void copyKeyBtn_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(this.keyTextBox.Text.Trim());
            MessageBox.Show("复制成功");
        }

        private void serialTextBox_TextChanged(object sender, EventArgs e)
        {
            if (this.serialTextBox.Text.Trim() != "")
            {
                this.registerBtn.Enabled = true;
            }
        }

        private void registerBtn_Click(object sender, EventArgs e)
        {
            try
            {
                this.registerBtn.Enabled = false;
                SoftReg sr = new SoftReg();
                string auth_code_by_textbox = this.serialTextBox.Text.Trim();
                string auth_code = sr.MD5Encrypt(sr.getRNum());
                if(auth_code_by_textbox == "")
                {
                    DialogResult result = MessageBox.Show("请输入序列号！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    if (result == DialogResult.OK)
                    {
                    }
                    this.registerBtn.Enabled = true;
                    return;
                }

                if(auth_code_by_textbox == auth_code)
                {
                    string homePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
                       Environment.OSVersion.Platform == PlatformID.MacOSX)
                        ? Environment.GetEnvironmentVariable("HOME")
                        : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
                    
                    FileStream saveFile = new FileStream(homePath + "/.ap_serial", FileMode.Create, FileAccess.ReadWrite);
                    string write_str = sr.MD5Encrypt(auth_code_by_textbox);
                    IFormatter serializer = new BinaryFormatter();
                    serializer.Serialize(saveFile, write_str);
                    saveFile.Close();

                    DialogResult result = MessageBox.Show("激活成功，请重新打开应用！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (result == DialogResult.OK)
                    {
                        this.Close();
                        System.Environment.Exit(0);
                    }
                }
                else
                {
                    DialogResult result = MessageBox.Show("激活失败，请重新输入序列号！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    if (result == DialogResult.OK)
                    {
                        this.serialTextBox.Text = "";
                        this.registerBtn.Enabled = true;
                    }
                }
            }
            catch (IOException ee)
            {
                throw ee;
                DialogResult result = MessageBox.Show("激活时发生严重错误，请重启电脑后，重新激活！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (result == DialogResult.OK)
                {
                    this.Close();
                    System.Environment.Exit(0);
                }
            }
           
        }
    }
}
