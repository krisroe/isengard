using System.Drawing;
namespace IsengardClient
{
    internal class UIShared
    {
        internal static Color GetColorForRealm(RealmType realm)
        {
            Color ret = Color.Transparent;
            switch (realm)
            {
                case RealmType.Earth:
                    ret = Color.Tan;
                    break;
                case RealmType.Fire:
                    ret = Color.LightSalmon;
                    break;
                case RealmType.Water:
                    ret = Color.LightBlue;
                    break;
                case RealmType.Wind:
                    ret = Color.LightGray;
                    break;
            }
            return ret;
        }

        internal static void GetForegroundColor(byte r, byte g, byte b, out byte forer, out byte foreg, out byte foreb)
        {
            forer = (byte)(r <= 128 ? 255 : 0);
            foreg = (byte)(g <= 128 ? 255 : 0);
            foreb = (byte)(b <= 128 ? 255 : 0);
        }
    }
}
