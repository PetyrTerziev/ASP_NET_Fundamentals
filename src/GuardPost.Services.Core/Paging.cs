namespace GuardPost.Services.Core;

internal static class Paging
{
    public static int Normalize(int page, int totalCount, int pageSize)
    {
        int totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        return Math.Clamp(page, 1, totalPages);
    }
}
