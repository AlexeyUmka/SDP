using System.Threading.Tasks;
using BrainstormSessions.Core.Interfaces;
using BrainstormSessions.ViewModels;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace BrainstormSessions.Controllers
{
    public class SessionController : Controller
    {
        private readonly IBrainstormSessionRepository _sessionRepository;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SessionController));

        public SessionController(IBrainstormSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<IActionResult> Index(int? id)
        {
            Logger.Debug($"{nameof(Index)} started processing request");
            
            if (!id.HasValue)
            {
                Logger.Debug("Id is not passed, redirecting to the Home/Index");
                return RedirectToAction(actionName: nameof(Index),
                    controllerName: "Home");
            }

            var session = await _sessionRepository.GetByIdAsync(id.Value);
            if (session == null)
            {
                Logger.Debug($"Session with id {id} wasn't found");
                return Content("Session not found.");
            }

            var viewModel = new StormSessionViewModel()
            {
                DateCreated = session.DateCreated,
                Name = session.Name,
                Id = session.Id
            };
            
            Logger.Debug($"{nameof(Index)} finished processing request");
            
            return View(viewModel);
        }
    }
}
