

namespace SurfBoardManager
{
    //classe har en generic type parameter, som arver af List classe med en generic type parameter.
    // Klassen indeholder udelukkend een side, når næste side skal hentes, så konstrueres en helt ny PaginatedList.
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        // Constructor som tager en list, count, pageIndex og pageSize.
        // Sørger for at tilføje listen til sig selv, og udregne/sætte paginated relaterede variabler.
        private PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize); //??

            //Tilføjer "items" til den generiske liste.
            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;


        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            // Count items in list.
            var count = source.Count();
            // Calculate how many items to skip, based on index and pageSize.
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
