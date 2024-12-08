using OpenTK.Mathematics;

namespace ScreensaverTests
{
    internal class SpriteMath
    {

        public static Vector2 FromRelative(Vector2 relativePosition)
        {

            return relativePosition * (GlobalValues.WIDTH, GlobalValues.HEIGHT);

        }

    }
}
