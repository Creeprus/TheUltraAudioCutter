using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace TheUltraAudioCutter
{
    /// <summary>
    /// Класс, отвечающий за запись в .wav файл данных
    /// </summary>
    public class WavFile
    {
        public string PathAudioFile { get; }
        public string FileName { get; }
        private TimeSpan duration;
        public TimeSpan Duration { get { return duration; } }
        private const int ticksInSecond = 10000000;
        #region HeadData 
        int headSize;

        int chunkID;
        int fileSize;
        int riffType;

        int fmtID;
        int fmtSize;
        int fmtCode;

        int channels;
        int sampleRate;
        int byteRate;
        int fmtBlockAlign;
        int bitDepth;
        int fmtExtraSize;

        int dataID;
        int dataSize;

        public int Channels { get { return channels; } }
        public int SampleRate { get { return sampleRate; } }
        #endregion
        #region AudioData 
        private List<float> floatAudioBuffer = new List<float>();
        #endregion
        public WavFile(string _path)
        {
            PathAudioFile = _path;
            FileName = Path.GetFileName(PathAudioFile);
            ReadWavFile(_path);
        }
        /// <summary>
        /// Получает buffer аудиофайла
        /// </summary>
        /// <returns></returns>
        public float[] GetFloatBuffer()
        {
            return floatAudioBuffer.ToArray();
        }
        /// <summary>
        /// Задает buffer аудиофайлу
        /// </summary>
        /// <param name="_buffer"> buffer аудиофайла</param>
        public void SetFloatBuffer(float[] _buffer)
        {
            floatAudioBuffer.Clear();
            floatAudioBuffer.AddRange(_buffer);
            CalculateDurationTrack();
            CalculateDataSize();
            CalculateFileSize();
        }
        /// <summary>
        /// рассчитывает длительность аудиофайла
        /// </summary>
        public void CalculateDurationTrack() => duration =
TimeSpan.FromTicks((long)(((double)floatAudioBuffer.Count / SampleRate / Channels) *
ticksInSecond));
        public void CalculateDataSize() => dataSize = floatAudioBuffer.Count * sizeof(Int16);
        public void CalculateFileSize() => fileSize = headSize + dataSize;
        /// <summary>
        /// Читает .wav файл
        /// </summary>
        /// <param name="filename"> Путь до аудиофайла</param>
        void ReadWavFile(string filename)
        {
            try
            {
                using (FileStream fileStream = File.Open(filename, FileMode.Open))
                {
                    BinaryReader reader = new BinaryReader(fileStream);
                    chunkID = reader.ReadInt32();
                    fileSize = reader.ReadInt32();
                    riffType = reader.ReadInt32();

                    long _position = reader.BaseStream.Position;
                    int zeroChunkSize = 0;
                    while (_position != reader.BaseStream.Length - 1)
                    {
                        reader.BaseStream.Position = _position;
                        int _fmtId = reader.ReadInt32();
                        if (_fmtId == 544501094)
                        {
                            fmtID = _fmtId;
                            break;
                        }
                        else
                        {
                            _position++;
                            zeroChunkSize++;
                        }
                    }

                    fmtSize = reader.ReadInt32();
                    fmtCode = reader.ReadInt16();
                    channels = reader.ReadInt16();
                    sampleRate = reader.ReadInt32();
                    byteRate = reader.ReadInt32();
                    fmtBlockAlign = reader.ReadInt16();
                    bitDepth = reader.ReadInt16();
                    if (fmtSize == 18)
                    {
                        fmtExtraSize = reader.ReadInt16();
                        reader.ReadBytes(fmtExtraSize);
                    }
                    dataID = reader.ReadInt32();
                    dataSize = reader.ReadInt32();

                    headSize = fileSize - dataSize - zeroChunkSize;
                    byte[] byteArray = reader.ReadBytes(dataSize);
                    int bytesInSample = bitDepth / 8;
                    int sampleAmount = dataSize / bytesInSample;
                    float[] tempArray = null;
                    switch (bitDepth)
                    {
                        case 16:
                            Int16[] int16Array = new Int16[sampleAmount];
                            System.Buffer.BlockCopy(byteArray, 0, int16Array, 0, dataSize);
                            IEnumerable<float> tempInt16 =
                                from i in int16Array
                                select i / (float)Int16.MaxValue;
                            tempArray = tempInt16.ToArray();
                            break;
                        default:
                            return;
                    }
                    floatAudioBuffer.AddRange(tempArray);
                    CalculateDurationTrack();
                }
            }
            catch
            {
                Debug.WriteLine("File error");
            }
        }
        /// <summary>
        /// Записывает данные в .wav файл
        /// </summary>
        public void WriteWavFile()
        {
            WriteWavFile(PathAudioFile);
        }
        /// <summary>
        /// Записывает в .wav файл аудиоданные
        /// </summary>
        /// <param name="filename"> Путь до аудиофайла</param>
        public void WriteWavFile(string filename)
        {
            using (FileStream fs = File.Create(filename))
            {
                fs.Write(BitConverter.GetBytes(chunkID), 0, sizeof(Int32));
                fs.Write(BitConverter.GetBytes(fileSize), 0, sizeof(Int32));
                fs.Write(BitConverter.GetBytes(riffType), 0, sizeof(Int32));

                fs.Write(BitConverter.GetBytes(fmtID), 0, sizeof(Int32));
                fs.Write(BitConverter.GetBytes(fmtSize), 0, sizeof(Int32));
                fs.Write(BitConverter.GetBytes(fmtCode), 0, sizeof(Int16));

                fs.Write(BitConverter.GetBytes(channels), 0, sizeof(Int16));
                fs.Write(BitConverter.GetBytes(sampleRate), 0, sizeof(Int32));
                fs.Write(BitConverter.GetBytes(byteRate), 0, sizeof(Int32));
                fs.Write(BitConverter.GetBytes(fmtBlockAlign), 0, sizeof(Int16));
                fs.Write(BitConverter.GetBytes(bitDepth), 0, sizeof(Int16));
                if (fmtSize == 18)
                    fs.Write(BitConverter.GetBytes(fmtExtraSize), 0, sizeof(Int16));

                fs.Write(BitConverter.GetBytes(dataID), 0, sizeof(Int32));

                float[] audioBuffer;
                audioBuffer = floatAudioBuffer.ToArray();

                fs.Write(BitConverter.GetBytes(dataSize), 0, sizeof(Int32));

                // Add Audio Data to wav file 
                byte[] byteBuffer = new byte[dataSize];

                Int16[] asInt16 = new Int16[audioBuffer.Length];

                IEnumerable<Int16> temp =
                                from g in audioBuffer
                                select (Int16)(g * (float)Int16.MaxValue);
                asInt16 = temp.ToArray();
                Buffer.BlockCopy(asInt16, 0, byteBuffer, 0, dataSize);
                fs.Write(byteBuffer, 0, dataSize);
            }
        }

    }
}
