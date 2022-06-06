using System;
using System.IO;

namespace AudioMerger
{
    class UtilityFunctions
    {
        /// <summary>
        ///Удаляет папку
        /// </summary>
        /// <param name="pPath">Удаляемая директория</param>
        /// <returns>Пустая строка, при успехе</returns>
        public static String DeleteDir(String pPath)
        {
            String retval = "";

            if (Directory.Exists(pPath))
            {
                try
                {

                    String[] filenames = Directory.GetFiles(pPath);
                    foreach (String filename in filenames)
                        File.Delete(filename);

                    Directory.Delete(pPath, true);
                }
                catch (System.Exception exc)
                {
                    retval = exc.Message;
                }
            }

            return retval;
        }
        /// <summary>
        /// Удаляет файлы из директории
        /// </summary>
        /// <param name="pPath"> Путь до папки</param>
        /// <returns> Пустую строку, при успехе</returns>
        public static String DeleteFilesFromDir(String pPath)
        {
            String retval = "";

            if (Directory.Exists(pPath))
            {
                try
                {

                    String[] filenames = Directory.GetFiles(pPath);
                    foreach (String filename in filenames)
                        File.Delete(filename);

                }
                catch (System.Exception exc)
                {
                    retval = exc.Message;
                }
            }

            return retval;
        }
    }
}