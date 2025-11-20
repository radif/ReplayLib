
namespace Replay.Utils
{
    public static class CharacterUtils
    {
        public static bool IsSpecialUnicodeCharacter(char c)
        {
            // ASCII characters are in the range 0-127
            // Characters above 127 are extended Unicode characters
            return (int)c > 127;
        }
    
        public static bool IsEmoji(string s)
        {
            // Check if string contains any surrogate pairs (which most emoji use)
            for (int i = 0; i < s.Length - 1; i++)
            {
                if (char.IsSurrogatePair(s[i], s[i + 1]))
                    return true;
            }
            return false;
        }
    }

}