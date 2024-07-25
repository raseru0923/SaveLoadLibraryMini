using System.IO;
using System.Text;
using System.Linq;
using UnityEngine;

namespace JBSaveLoadLib
{
    public static class JBSaveLoad
    {
        private static readonly byte KEY = 0b0001_1010;

        // プライベートメンバ関数
        /// <summary>
        /// 必要領域を確保
        /// </summary>
        /// <param name="path"></param>
        private static void Partitioning(string path)
        {
            string tempPath = Path.GetDirectoryName(path);
            if (!Directory.Exists(tempPath)) { Directory.CreateDirectory(tempPath); }
            tempPath = Path.GetFileName(path);
            if (!File.Exists(tempPath)) { using (var file = File.Create(tempPath)) { } }
        }

        // パブリック関数
        /// <summary>
        /// データをセーブします
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="filePath"></param>
        public static void Save<T>(T data, string filePath)
        {
            // 必要領域確保
            Partitioning(filePath);

            // json形式にシリアライズ
            string serializeData = JsonUtility.ToJson(data);

            using (var stream = File.OpenWrite(filePath))
            {
                stream.SetLength(0);    // ファイルリセット
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    var bytes = Encoding.UTF8.GetBytes(serializeData);
                    // 暗号化
                    bytes = bytes.Select(x => (byte)(x ^ KEY)).ToArray();
                    // 書き込み
                    writer.Write(bytes);

                    var byteContent = "bytes";
                    Encoding.UTF8.GetBytes(serializeData).Select(x => x.ToString("x")).ToList().ForEach(x => { byteContent += ',' + x; });
                }
            }
        }
        /// <summary>
        /// データをロードします
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="filePath"></param>
        /// <returns>true=読み込み成功 false=読み込み失敗</returns>
        public static bool Load<T>(out T data, string filePath)
        {
            try
            {
                using (var fileStream = File.OpenRead(filePath))
                {
                    using (BinaryReader reader = new BinaryReader(fileStream))
                    {
                        var bytes = new byte[fileStream.Length];
                        fileStream.Read(bytes, 0, (int)fileStream.Length);
                        for (int i = 0; i < bytes.Length; ++i) { bytes[i] ^= KEY; }
                        // ファイル読み込みと文字化け防止
                        var text = Encoding.UTF8.GetString(bytes);
                        // 情報がない場合はデータのデシリアライズを行わない
                        if (text == "" || text == null)
                        {
                            data = default;
                            Debug.LogError("データが存在しません");
                            return false;
                        }
                        var byteContent = "bytes";
                        bytes.Select(x => x.ToString("x")).ToList().ForEach(x => { byteContent += ',' + x; });
                        data = JsonUtility.FromJson<T>(text);
                    }
                }
            }
            catch
            {
                data = default;
                Debug.LogError("エラーが発生しました");
                return false;
            }
            return true;
        }
    }
}