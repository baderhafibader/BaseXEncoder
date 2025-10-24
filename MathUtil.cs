namespace StaticUtil {
    public static class MathUtil {
        public static int GCD(int x, int y) {
            while (y != 0) {
                int temp = y;
                y = x % y;
                x = temp;
            }
            return x;
        }
        public static (int, int) MinRatio(int x, int y) {
            var gcd = GCD(x, y);
            return (x / gcd, y / gcd);
        }
        public static int GetIntOfLast4Bytes(byte[] bytes) {

            // take last 4 (or pad with zeros if fewer)
            int count = bytes.Length;
            byte b0 = count > 0 ? bytes[^1] : (byte)0;
            byte b1 = count > 1 ? bytes[^2] : (byte)0;
            byte b2 = count > 2 ? bytes[^3] : (byte)0;
            byte b3 = count > 3 ? bytes[^4] : (byte)0;

            int value = (b3 << 24) | (b2 << 16) | (b1 << 8) | b0;
            return value;
        }
    }
}