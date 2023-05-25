using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decen.Common.Helpers
{
    /// <summary>
    /// 文件操作类
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// 验证文件是否存在
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool FileExists(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 创建文件目录
        /// <param name="create">如果没有文件夹，就自动创建，默认：自动创建</param>
        /// </summary>
        public static string Create(string path, bool create = true)
        {
            string dicPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            if (!Directory.Exists(dicPath))
            {
                Directory.CreateDirectory(dicPath);
            }
            return dicPath;
        }

        /// <summary>
        /// 获取文件目录
        /// </summary>
        /// <param name="filePath">Upload/File</param>
        /// <param name="create">如果没有文件夹，就自动创建，默认：自动创建</param>
        /// <returns></returns>
        public static string GetDirectory(string filePath, bool create = true)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new Exception("文件路径不能为空！");
            }
            string dicPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
            if (create && !Directory.Exists(dicPath))
            {
                Directory.CreateDirectory(dicPath);
                return dicPath;
            }
            return dicPath;
        }

        /// <summary>
        /// 文件路径
        /// </summary>
        /// <param name="filePath">被移动文件路径</param>
        /// <param name="newFilePath">移动文件的新路径(不需要文件名称)</param>
        public static void Move(string filePath, string newFilePath)
        {
            if (File.Exists(filePath))
            {
                string Extension = Path.GetExtension(filePath);
                File.Move(filePath, Path.Combine(newFilePath, DateTime.Now.ToString("yyyy-MM-dd-hhmmss") + Extension));
            }
        }

        /// <summary>
        /// 将现有的文件，复制到新的目录中
        /// </summary>
        /// <param name="filePaths">需要复制的文件路径集合</param>
        /// <param name="newFilePath">需要复制到新的文件夹路径</param>
        public static void Copy(List<string> filePaths, string newFilePath)
        {
            string fileName = null;
            foreach (var item in filePaths)
            {
                if (File.Exists(item))
                {
                    fileName = Path.GetFileName(item);
                    if (fileName.Length > 36)
                    {
                        string guid = fileName.Substring(0, 36);
                        fileName = fileName.Substring(36);
                    }
                    File.Copy(item, Path.Combine(newFilePath, fileName), true);
                }
            }
        }

        /// <summary>
        /// 将现有的文件，复制到新的目录中
        /// </summary>
        /// <param name="filePaths">需要复制的文件路径</param>
        /// <param name="newFilePath">需要复制到新的文件夹路径</param>
        public static void Copy(string filePaths, string newFilePath)
        {
            if (File.Exists(filePaths))
            {
                File.Copy(filePaths, newFilePath, true);
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public static void Delete(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// 批量删除文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public static void Delete(List<string> filePath)
        {
            foreach (var item in filePath)
            {
                if (File.Exists(item))
                {
                    File.Delete(item);
                }
            }
        }

        /// <summary>
        /// 读取
        /// 记事本文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>返回文件内容</returns>
        public static string ReadText(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                throw new Exception("文件不存在！");
            }
            try
            {
                using (StreamReader sr = new StreamReader(path, Encoding.Default))
                {
                    StringBuilder sb = new StringBuilder();
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        sb.Append(line.ToString());
                    }
                    return sb.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 获取文件类型
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileType(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                string fileType = path.Substring(path.LastIndexOf("."), path.Length);
                return fileType;
            }
            return "";
        }

        public static string CopyToSavePath(string path)
        {

            return "";

        }

    }
}
