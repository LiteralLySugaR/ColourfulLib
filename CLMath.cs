namespace ColourfulLib
{
    internal class CLMath
    {
        // For some reasons, Microsoft's Math doesnt
        // include a Clamp function ...
        internal static float Clamp(float min, float value, float max)
        {
            if (value < min)
            {
                return min;
            }
            if (value > max)
            {
                return max;
            }
            return value;
        }
        internal static int Clamp(int min, int value, int max)
        {
            if (value < min)
            {
                return min;
            }
            if (value > max)
            {
                return max;
            }
            return value;
        }
        internal static float NormaliseByte(byte value)
        {
            return value / 255;
        }
    }
}