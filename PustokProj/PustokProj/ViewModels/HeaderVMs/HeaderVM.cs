using PustokProj.ViewModels.BasketVMs;

namespace PustokProj.ViewModels.HeaderVMs
{
    public class HeaderVM
    {
        public List<Genre> Genres { get; set; }
        public List<BasketItemVM> BasketItems { get; set; }
        public AccountBlockVM Account { get; set; }
    }
}
