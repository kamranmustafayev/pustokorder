namespace PustokProj.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await Task.FromResult(0));
        }
    }
}
