using System.Threading.Tasks;
using System.Web.Mvc;
using TrelloCard.Contracts;

namespace TrelloCard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITrelloService _trelloService;
        public HomeController(ITrelloService trelloService)
        {
            _trelloService = trelloService;
        }

        public async Task<ActionResult> Index()
        {
            await _trelloService.CreateCardInBoardAsync();

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}