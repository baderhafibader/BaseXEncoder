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
    }
}