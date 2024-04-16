namespace MyMostUsedWords.Helpers
{
    public static class IntHelper
    {
        public static bool IsEndOfWordCharacter(this int ch)
        {
            //return ch < 65 || ch > 122;
            return ch < 'A' || ch > 'z';
        }
    }
}
