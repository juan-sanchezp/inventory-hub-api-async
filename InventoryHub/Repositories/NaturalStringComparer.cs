//Ordenación de mis codigos como lo requiere el inventario
namespace InventoryHub.Repositories
{
    public class NaturalStringComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            if (x == null) return y == null ? 0 : -1;
            if (y == null) return 1;

            int ix = 0, iy = 0;

            while (ix < x.Length && iy < y.Length)
            {
                if (char.IsDigit(x[ix]) && char.IsDigit(y[iy]))
                {
                    // Extraer número de x
                    int startX = ix;
                    while (ix < x.Length && char.IsDigit(x[ix])) ix++;
                    int numX = int.Parse(x.Substring(startX, ix - startX));

                    // Extraer número de y
                    int startY = iy;
                    while (iy < y.Length && char.IsDigit(y[iy])) iy++;
                    int numY = int.Parse(y.Substring(startY, iy - startY));

                    int cmp = numX.CompareTo(numY);
                    if (cmp != 0) return cmp;
                }
                else
                {
                    int cmp = x[ix].CompareTo(y[iy]);
                    if (cmp != 0) return cmp;
                    ix++;
                    iy++;
                }
            }

            return x.Length.CompareTo(y.Length);
        }
    }
}