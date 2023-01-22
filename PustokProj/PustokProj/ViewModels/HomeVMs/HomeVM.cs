namespace PustokProj.ViewModels.HomeVMs
{
    public class HomeVM
    {
        public List<Slider> Sliders { get; set; }
        public List<Feature> Features { get; set; }
        public List<Brand> Brands { get; set; }
        public List<Book> FeaturedBooks { get; set; }
        public List<Book> NewBooks { get; set; }
        public List<Book> BooksOnDiscount { get; set; }
        public List<Book> ClassicBooks { get; set; }
        public List<BookImage> BookImages { get; set; }
    }
}
