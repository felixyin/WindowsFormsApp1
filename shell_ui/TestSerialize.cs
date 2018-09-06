using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace felixyin
{
    class TestSerialize
    {

        static void Main(string[] args)
        {

            //用于序列化和反序列化的对象
            IFormatter serializer = new BinaryFormatter();

            //反序列化
            try
            {
                FileStream loadFile = new FileStream("serial", FileMode.Open, FileAccess.Read);
                string tests2 = serializer.Deserialize(loadFile) as string;
                Console.WriteLine(tests2);
            }catch(FileNotFoundException e)
            {

            }

            //开始序列化
            FileStream saveFile = new FileStream("serial", FileMode.Create, FileAccess.Write);
            SoftReg sr = new SoftReg();
            string auth_code = "fc4be3a30def3b4821f893fff7ae63ce";
            string write_str = sr.MD5Encrypt(auth_code);
            serializer.Serialize(saveFile, write_str);
            saveFile.Close();

       

        }

    }
}
