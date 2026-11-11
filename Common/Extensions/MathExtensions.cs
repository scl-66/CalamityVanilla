using Microsoft.Xna.Framework;

namespace CalamityVanilla.Common.Extensions;

public static class MathExtensions
{
    public static Color CycleThroughColors(this Color[] colors, int time, int fadeTime)
    {
        int index = (int)((time / fadeTime) % colors.Length);
        int nextIndex = (index + 1) % colors.Length;
        return Color.Lerp(Color.Lerp(colors[index], colors[nextIndex], (time % fadeTime) / (float)fadeTime), Color.White, 0.25f);
    }
}
