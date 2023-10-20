using System.IO;
using Cysharp.Threading.Tasks;

namespace Studio23.SS2.UPMAssistant.Editor.Utilities
{

    public static class FileExtensions
    {
        public static async UniTask<string> ReadFileAsync(this string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                return await reader.ReadToEndAsync().AsUniTask();
            }
        }

        public static async UniTask WriteFileAsync(this string filePath, string content)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                await writer.WriteAsync(content).AsUniTask();
            }
        }
    }
}