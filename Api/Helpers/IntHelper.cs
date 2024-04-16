namespace MyMostUsedWords.Helpers
{
    public static class IntHelper
    {
        public static bool IsValidLetter(this int ch)
        {
            return IsValidLetter(ch);
        }

        public static bool IsValidLetter(this char ch)
        {
            //return ch < 65 || ch > 122;
            return (ch >= 'A' && ch <= 'z')
                || (ch >= 'À' && ch <= 'Ö')
                || (ch >= 'Ø' && ch <= 'ö')
                || (ch >= 'ø' && ch <= 'ÿ');
        }
    }
}
