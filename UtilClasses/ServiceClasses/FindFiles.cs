using System;
using System.IO;

namespace UtilClasses.ServiceClasses
{
    public class FindFiles
    {
        /// <summary>
        /// Получает в качестве аргументов путь на репозитарий
        /// и короткое наименование приложения
        /// находит если есть в хранилище  txt файл с меткой времени
        /// этого приложения и возвращает полный путь и имя  файла с меткой времени
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="selectedAppName">короткое имя приложения</param>
        /// <param name="ext"></param>
        /// <returns>полный путь и имя  файла с меткой времени</returns>
        public static string SearchFileAppInStore(string folderPath, string selectedAppName, string ext)
        {
            if (folderPath == null) throw new ArgumentNullException(nameof(folderPath));
            if (selectedAppName == null) throw new ArgumentNullException(nameof(selectedAppName));

            foreach (var file in Directory.GetFiles(folderPath, ext))
            {
                var mFile = Path.GetFileNameWithoutExtension(file);

                var mFile2 = mFile.Substring(0, mFile.Length - 18);

                if (String.CompareOrdinal(selectedAppName, mFile2) == 0)
                {
                    return file;
                }
            }

            return "";
        }
    }
}
