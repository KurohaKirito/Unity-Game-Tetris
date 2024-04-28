using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace Utility
{
    public static class JsonUtility
    {
        /// <summary>
        /// 存档文件路径
        /// </summary>
        private static string saveFilePath;

        /// <summary>
        /// 存档文件路径
        /// </summary>
        private static string SaveFilePath
        {
            get => saveFilePath;
            set
            {
                saveFilePath = value;

                if (Directory.Exists(value) == false)
                {
                    Directory.CreateDirectory(value);
                }
            }
        }

        /// <summary>
        /// 存档文件名称
        /// </summary>
        private static string saveFilePathName;

        /// <summary>
        /// 存档文件名称
        /// </summary>
        private static string SaveFilePathName
        {
            get => saveFilePathName;
            set
            {
                saveFilePathName = value;

                if (File.Exists(value))
                {
                    return;
                }

                using (File.Create(value)) { }
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            InitAndroid();
            InitWindows();
            InitEditor();
        }

        [Conditional("UNITY_ANDROID")]
        private static void InitAndroid()
        {
            SaveFilePath = Application.persistentDataPath + "/Saves/";
            SaveFilePathName = SaveFilePath + "Save.json";
        }

        [Conditional("UNITY_STANDALONE_WIN")]
        private static void InitWindows()
        {
            SaveFilePath = Application.dataPath + "/Saves/";
            SaveFilePathName = SaveFilePath + "Save.json";
        }

        [Conditional("UNITY_EDITOR")]
        private static void InitEditor()
        {
            SaveFilePath = Application.dataPath + "/Saves/";
            SaveFilePathName = SaveFilePath + "Save.json";
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="data">数据</param>
        public static void SaveData<T>(T data) where T : class
        {
            using var stringWriter = new StringWriter();
            var serializer = new JsonSerializer();
            serializer.Serialize(stringWriter, data);
            var jsonDataString = stringWriter.GetStringBuilder().ToString();

            File.WriteAllText(SaveFilePathName, jsonDataString, Encoding.UTF8);
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>返回的泛型数据</returns>
        public static T LoadData<T>() where T : class
        {
            var serializer = new JsonSerializer();
            var jsonString = File.ReadAllText(SaveFilePathName);

            using var stringReader = new StringReader(jsonString);
            using var jsonTextReader = new JsonTextReader(stringReader);

            var result = serializer.Deserialize(jsonTextReader, typeof(T)) as T;
            return result;
        }
    }
}