using MyMostUsedWords.Buffers;

namespace MyMostUsedWords.Helpers
{
    public static class CharArrayBufferHelper
    {
        public static string GetWord(this ArrayBuffer<char> wordBuffer)
        {
            var word = new string(wordBuffer.ToArray());
            word = word.ToLower();
            wordBuffer.Clear();

            return word;
        }
    }
}
