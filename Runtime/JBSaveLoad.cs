using System.IO;
using System.Text;
using System.Linq;
using UnityEngine;

namespace JBSaveLoadLib
{
    public static class JBSaveLoad
    {
        private static readonly byte KEY = 0b0001_1010;

        // �v���C�x�[�g�����o�֐�
        /// <summary>
        /// �K�v�̈���m��
        /// </summary>
        /// <param name="path"></param>
        private static void Partitioning(string path)
        {
            string tempPath = Path.GetDirectoryName(path);
            if (!Directory.Exists(tempPath)) { Directory.CreateDirectory(tempPath); }
            tempPath = Path.GetFileName(path);
            if (!File.Exists(tempPath)) { using (var file = File.Create(tempPath)) { } }
        }

        // �p�u���b�N�֐�
        /// <summary>
        /// �f�[�^���Z�[�u���܂�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="filePath"></param>
        public static void Save<T>(T data, string filePath)
        {
            // �K�v�̈�m��
            Partitioning(filePath);

            // json�`���ɃV���A���C�Y
            string serializeData = JsonUtility.ToJson(data);

            using (var stream = File.OpenWrite(filePath))
            {
                stream.SetLength(0);    // �t�@�C�����Z�b�g
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    var bytes = Encoding.UTF8.GetBytes(serializeData);
                    // �Í���
                    bytes = bytes.Select(x => (byte)(x ^ KEY)).ToArray();
                    // ��������
                    writer.Write(bytes);

                    var byteContent = "bytes";
                    Encoding.UTF8.GetBytes(serializeData).Select(x => x.ToString("x")).ToList().ForEach(x => { byteContent += ',' + x; });
                }
            }
        }
        /// <summary>
        /// �f�[�^�����[�h���܂�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="filePath"></param>
        /// <returns>true=�ǂݍ��ݐ��� false=�ǂݍ��ݎ��s</returns>
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
                        // �t�@�C���ǂݍ��݂ƕ��������h�~
                        var text = Encoding.UTF8.GetString(bytes);
                        // ��񂪂Ȃ��ꍇ�̓f�[�^�̃f�V���A���C�Y���s��Ȃ�
                        if (text == "" || text == null)
                        {
                            data = default;
                            Debug.LogError("�f�[�^�����݂��܂���");
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
                Debug.LogError("�G���[���������܂���");
                return false;
            }
            return true;
        }
    }
}