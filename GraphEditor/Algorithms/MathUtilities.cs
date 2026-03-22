namespace GraphEditor.Algorithms;
public static class MathUtilities
{
    public static int GetMaxPowerOfTwoLessThan(int max)
    {
        var powerOfTwo = 1;

        while (powerOfTwo * 2 <= max)
        {
            powerOfTwo *= 2;
        }

        return powerOfTwo;
    }
}