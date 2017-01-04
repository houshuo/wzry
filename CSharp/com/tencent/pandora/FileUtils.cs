namespace com.tencent.pandora
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Text;
    using UnityEngine;

    internal class FileUtils
    {
        private static string FileCommErrorLogFile = "pcomm.log";
        private static string FilePreErrorLogFile = "elog.txt";
        public static string path = string.Empty;
        public static string strLogFileName = "pandora_log.txt";

        public static void AppendFile(string name, string info)
        {
            try
            {
                StreamWriter writer;
                InitPath();
                FileInfo info2 = new FileInfo(path + "//" + name);
                if (!info2.Exists)
                {
                    writer = info2.CreateText();
                }
                else
                {
                    writer = info2.AppendText();
                }
                writer.WriteLine(info);
                writer.Close();
                writer.Dispose();
            }
            catch (Exception)
            {
            }
        }

        public static void CleanFiles(int iDelDays)
        {
            try
            {
                InitPath();
                if (Directory.Exists(path))
                {
                    string[] strArray = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                    com.tencent.pandora.Logger.d("clean files length:" + strArray.Length);
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        com.tencent.pandora.Logger.d("file:" + strArray[i]);
                        FileInfo info = new FileInfo(strArray[i]);
                        com.tencent.pandora.Logger.d(info.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        TimeSpan span = (TimeSpan) (DateTime.Now - info.CreationTime);
                        com.tencent.pandora.Logger.d("has create days:" + span.Days);
                        if (span.Days >= iDelDays)
                        {
                            DeleteFile(strArray[i]);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.e("CleanFiles" + exception.Message);
            }
        }

        public static void DeleteFile(string file)
        {
            try
            {
                File.Delete(file);
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.e("DeleteFile" + exception.Message);
            }
        }

        public static byte[] GetBinData(string strFile)
        {
            try
            {
                if (File.Exists(path + "/" + strFile))
                {
                    FileStream stream = new FileStream(path + "/" + strFile, FileMode.Open);
                    byte[] array = new byte[stream.Length];
                    stream.Read(array, 0, array.Length);
                    stream.Close();
                    return array;
                }
                return null;
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.e("prase action cache fail:" + exception.Message);
                return null;
            }
        }

        public static ArrayList GetFileBakInfo(int iType)
        {
            if (iType == 1)
            {
                return LoadFile(FilePreErrorLogFile + ".bak");
            }
            if (iType == 2)
            {
                return LoadFile(FileCommErrorLogFile + ".bak");
            }
            return null;
        }

        public static int GetFlag(string strFlag)
        {
            try
            {
                InitPath();
                string str = "flag_" + strFlag + ".dat";
                if (File.Exists(path + str))
                {
                    return 1;
                }
                return 0;
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.d("ERROR:SetFalgErrr" + exception.Message);
                return -1;
            }
        }

        private static string GetPersistentFilePath()
        {
            string str;
            if (path != string.Empty)
            {
                return path;
            }
            if (((Application.platform == RuntimePlatform.OSXEditor) || (Application.platform == RuntimePlatform.OSXPlayer)) || ((Application.platform == RuntimePlatform.WindowsEditor) || (Application.platform == RuntimePlatform.WindowsPlayer)))
            {
                str = Application.temporaryCachePath + "/";
            }
            else if ((Application.platform == RuntimePlatform.IPhonePlayer) || (Application.platform == RuntimePlatform.Android))
            {
                str = Application.temporaryCachePath + "/";
            }
            else
            {
                str = Application.temporaryCachePath + "/";
            }
            str = str + "TPlayCache/";
            try
            {
                if (!Directory.Exists(str))
                {
                    Directory.CreateDirectory(str);
                }
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.d(exception.Message);
            }
            return str;
        }

        public static string GetReportFile()
        {
            try
            {
                string path = string.Empty;
                path = FileUtils.path + "/" + strLogFileName;
                string str2 = path + ".bak";
                if (File.Exists(str2))
                {
                    File.Delete(str2);
                }
                if (File.Exists(path))
                {
                    com.tencent.pandora.Logger.d("Move " + path);
                    File.Move(path, str2);
                    return str2;
                }
                return string.Empty;
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.d("Msg:" + exception.Message);
            }
            return string.Empty;
        }

        private static string GetTFlagFilePath()
        {
            if (!Directory.Exists(Application.temporaryCachePath + "/vercache/"))
            {
                Directory.CreateDirectory(Application.temporaryCachePath + "/vercache/");
            }
            return (Application.temporaryCachePath + "/vercache/tflag.dat");
        }

        public static void InitPath()
        {
            if (path == string.Empty)
            {
                path = GetPersistentFilePath();
            }
        }

        public static bool IsFileExists(int iType)
        {
            string path = string.Empty;
            if (iType == 1)
            {
                path = FileUtils.path + "/" + FilePreErrorLogFile;
            }
            else if (iType == 2)
            {
                path = FileUtils.path + "/" + FileCommErrorLogFile;
            }
            else
            {
                return false;
            }
            return File.Exists(path);
        }

        public static ArrayList LoadFile(string name)
        {
            try
            {
                string str;
                InitPath();
                StreamReader reader = null;
                try
                {
                    reader = File.OpenText(path + name);
                }
                catch (Exception exception)
                {
                    com.tencent.pandora.Logger.d(exception.Message);
                    return null;
                }
                ArrayList list = new ArrayList();
                while ((str = reader.ReadLine()) != null)
                {
                    list.Add(str);
                }
                reader.Close();
                reader.Dispose();
                return list;
            }
            catch (Exception exception2)
            {
                com.tencent.pandora.Logger.d(exception2.Message);
                return new ArrayList();
            }
        }

        public static string LoadTFlagFileFile()
        {
            try
            {
                StreamReader reader = null;
                try
                {
                    reader = File.OpenText(GetTFlagFilePath());
                }
                catch (Exception exception)
                {
                    com.tencent.pandora.Logger.d(exception.Message);
                    return string.Empty;
                }
                string str = reader.ReadLine();
                if (str == null)
                {
                }
                reader.Close();
                reader.Dispose();
                return str;
            }
            catch (Exception exception2)
            {
                com.tencent.pandora.Logger.d(exception2.Message);
                return string.Empty;
            }
        }

        public static void LoggCommInfo(string strLog)
        {
            LoggInfo_file("@@@" + strLog, FileCommErrorLogFile);
        }

        public static void LoggInfo_file(string strLog, string strFile)
        {
            try
            {
                string fileName = FileUtils.path + "/" + strFile;
                FileInfo info = new FileInfo(fileName);
                if (!info.Exists)
                {
                    File.Create(fileName).Close();
                    info = new FileInfo(fileName);
                }
                if ((info.Length > 0x19000L) && File.Exists(fileName))
                {
                    string path = fileName + "_bak";
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    File.Move(fileName, fileName + "_bak");
                }
                using (FileStream stream2 = info.OpenWrite())
                {
                    StreamWriter writer = new StreamWriter(stream2);
                    writer.BaseStream.Seek(0L, SeekOrigin.End);
                    writer.Write(strLog + "|" + DateTime.Now.ToString());
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception)
            {
            }
        }

        public static void LoggPreErrorInfo(int iErrorCode, string strLog)
        {
            LoggInfo_file("@@@" + iErrorCode.ToString() + "###" + strLog, FilePreErrorLogFile);
        }

        public static void MoveToBak(int iType)
        {
            try
            {
                string path = string.Empty;
                if (iType == 1)
                {
                    path = FileUtils.path + "/" + FilePreErrorLogFile;
                }
                else if (iType == 2)
                {
                    path = FileUtils.path + "/" + FileCommErrorLogFile;
                }
                else
                {
                    return;
                }
                if (File.Exists(path + ".bak"))
                {
                    File.Delete(path + ".bak");
                }
                if (File.Exists(path))
                {
                    com.tencent.pandora.Logger.d("Move " + path);
                    File.Move(path, path + ".bak");
                }
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.d("Msg:" + exception.Message);
            }
        }

        public static int SetFlag(string strFlag)
        {
            try
            {
                InitPath();
                string name = "flag_" + strFlag + ".dat";
                if (File.Exists(path + name))
                {
                    return 0;
                }
                WriteFile(name, "1");
                return 1;
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.d("ERROR:SetFalgErrr" + exception.Message);
                return -1;
            }
        }

        public static bool TFlagFileExists()
        {
            if (!File.Exists(GetTFlagFilePath()))
            {
                return false;
            }
            return true;
        }

        public static void WriteBinDataToFile(byte[] byteData, string strFile, int iLen)
        {
            try
            {
                InitPath();
                try
                {
                    FileStream output = new FileStream(path + "/" + strFile, FileMode.Create);
                    if (output != null)
                    {
                        BinaryWriter writer = new BinaryWriter(output);
                        for (int i = 0; i < iLen; i++)
                        {
                            writer.Write(byteData[i]);
                            writer.Flush();
                        }
                        writer.Close();
                        output.Close();
                        com.tencent.pandora.Logger.d("write finish");
                    }
                }
                catch (Exception exception)
                {
                    com.tencent.pandora.Logger.d("writer" + exception.ToString());
                }
            }
            catch (Exception exception2)
            {
                com.tencent.pandora.Logger.d(exception2.Message);
            }
        }

        public static void WriteFile(string name, string info)
        {
            try
            {
                InitPath();
                FileStream stream = new FileStream(path + name, FileMode.Create);
                if (stream != null)
                {
                    StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
                    writer.WriteLine(info);
                    writer.Close();
                    writer.Dispose();
                }
            }
            catch (Exception exception)
            {
                Debug.Log("WriteFile:" + exception.Message);
            }
        }

        public static bool WriteTFlgFile(string info)
        {
            try
            {
                FileStream stream = new FileStream(GetTFlagFilePath(), FileMode.Create);
                if (stream == null)
                {
                    return false;
                }
                StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
                writer.WriteLine(info);
                writer.Close();
                writer.Dispose();
                return true;
            }
            catch (Exception exception)
            {
                com.tencent.pandora.Logger.e("SetFlagError:" + exception.Message);
                return false;
            }
        }

        public enum FileType
        {
            CommErrorFile = 2,
            ErrorFile = 1
        }
    }
}

