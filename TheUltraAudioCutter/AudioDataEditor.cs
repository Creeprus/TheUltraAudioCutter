using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheUltraAudioCutter
{
    /// <summary>
    /// Класс, отвечающий за обрезку выбранного промежутка на аудиографе
    /// </summary>
    class AudioDataEditor : WavFile
    {
        private List<float> audioBufferInMemory = new List<float>();
        public List<float> AudioBufferInMemory
        {
            get { return audioBufferInMemory; }
            set { audioBufferInMemory = value; }
        }
        public AudioDataEditor(string path) : base(path) { }
        /// <summary>
        /// Задаёт переменным значения первого и второго конца выбранного промежутка на аудиографе, также получает длину промежутка
        /// </summary>
        /// <param name="startPosition"> Первый конец промежутка</param>
        /// <param name="endPosition"> Второй конец промежутка</param>
        /// <param name="length"> Длина промежутка</param>
        private void SetExactPosition(ref double startPosition, ref double endPosition,
 int length)
        {
            SetExactPosition(ref startPosition, length);
            SetExactPosition(ref endPosition, length);
        }
        /// <summary>
        /// Задает позицию выбранного промежутка
        /// </summary>
        /// <param name="position"> Позиция промежутка</param>
        /// <param name="length"> Длина промежутка</param>
        private void SetExactPosition(ref double position, int length)
        {
            position = (int)(length * position);

            if (Channels == 2)
            {
                if (position % 2 == 0)
                {
                    if (position + 1 >= length)
                        position--;
                    else
                        position++;
                }
            }
        }
        /// <summary>
        /// Копирование выбранного промежутка
        /// </summary>
        /// <param name="relativeStartPos"> Начало промежутка</param>
        /// <param name="relativeEndPos"> Конец промежутка</param>
        public void Copy(double relativeStartPos, double relativeEndPos)
        {
            float[] audioData = GetFloatBuffer();
            AudioBufferInMemory.Clear();
            double startPosition = relativeStartPos;
            double endPosition = relativeEndPos;
            SetExactPosition(ref startPosition, ref endPosition, audioData.Length);
            float[] temp = new float[(int)(endPosition - startPosition)];
            Array.Copy(audioData.ToArray(), (int)startPosition, temp, 0, temp.Length);
            AudioBufferInMemory.AddRange(temp);
        }
        /// <summary>
        /// Вырезка выбранного промежутка
        /// </summary>
        /// <param name="relativeStartPos"> Начало промежутка</param>
        /// <param name="relativeEndPos"> Конец промежутка</param>
        public void Cut(double relativeStartPos, double relativeEndPos)
        {
            Copy(relativeStartPos, relativeEndPos);
            Delete(relativeStartPos, relativeEndPos);
        }
        /// <summary>
        /// Вставляет обрезанный или копированный промежуток на аудиограф
        /// </summary>
        /// <param name="relativeStartPos"> Начало промежутка</param>
        public void Paste(double relativeStartPos)
        {
            if (AudioBufferInMemory.Count > 0)
            {
                List<float> temp = new List<float>();
                temp.AddRange(GetFloatBuffer());
                double startPosition = relativeStartPos;
                SetExactPosition(ref startPosition, temp.Count);
                temp.InsertRange((int)startPosition, AudioBufferInMemory);
                SetFloatBuffer(temp.ToArray());
                WriteWavFile();
            }
        }
        /// <summary>
        /// Удаляет выбранный промежуток на аудиографе
        /// </summary>
        /// <param name="relativeStartPos"> Начало промежутка</param>
        /// <param name="relativeEndPos"> Конец промежутка</param>
        public void Delete(double relativeStartPos, double relativeEndPos)
        {
            List<float> _temp = new List<float>();
            _temp.AddRange(GetFloatBuffer());
            double startPosition = relativeStartPos;
            double endPosition = relativeEndPos;
            SetExactPosition(ref startPosition, ref endPosition, _temp.Count);
            _temp.RemoveRange((int)startPosition, (int)(endPosition - startPosition));
            SetFloatBuffer(_temp.ToArray());
            WriteWavFile();
        }
    }
}
