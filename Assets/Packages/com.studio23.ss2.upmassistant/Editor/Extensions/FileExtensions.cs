using System.IO;
using Cysharp.Threading.Tasks;

namespace Studio23.SS2.UPMAssistant.Editor.Utilities
{
    public static class FileExtensions
    {
        public static async UniTask<string> ReadFileAsync(this string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                return await reader.ReadToEndAsync().AsUniTask();
            }
        }

        public static async UniTask WriteFileAsync(this string filePath, string content)
        {
            using (var writer = new StreamWriter(filePath))
            {
                await writer.WriteAsync(content).AsUniTask();
            }
        }
    }
}