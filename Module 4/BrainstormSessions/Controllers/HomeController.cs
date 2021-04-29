using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainstormSessions.Core.Interfaces;
using BrainstormSessions.Core.Model;
using BrainstormSessions.Helpers;
using BrainstormSessions.ViewModels;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace BrainstormSessions.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBrainstormSessionRepository _sessionRepository;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(HomeController));
        private const string LogsPath = "C:\\Temp\\";

        public HomeController(IBrainstormSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<IActionResult> Index()
        {
            Logger.Info($"{nameof(Index)} started processing request");

            var sessionList = await _sessionRepository.ListAsync();

            var model = sessionList.Select(session => new StormSessionViewModel()
            {
                Id = session.Id,
                DateCreated = session.DateCreated,
                Name = session.Name,
                IdeaCount = session.Ideas.Count
            });

            Logger.Info($"{nameof(Index)} finished processing request");
            
            return View(model);
        }

        public class NewSessionModel
        {
            [Required]
            public string SessionName { get; set; }

            public override string ToString()
            {
                return this.GetPropertiesAndFieldsStringRepresentation();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Index(NewSessionModel model)
        {
            if (!ModelState.IsValid)
            {
                Logger.Warn($"Passed model is not valid: {model}");
                return BadRequest(ModelState);
            }
            else
            {
                await _sessionRepository.AddAsync(new BrainstormSession()
                {
                    DateCreated = DateTimeOffset.Now,
                    Name = model.SessionName
                });
            }

            return RedirectToAction(actionName: nameof(Index));
        }
        
        [HttpGet]
        public async Task<IActionResult> Logs()
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, "Utils/LogParser.exe");
            var startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.FileName = filePath;
            startInfo.WorkingDirectory = $"{Environment.CurrentDirectory}\\Utils";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.Arguments = $"-i:TEXTLINE -e:1 \"SELECT Count(*) AS ErrorCount FROM {LogsPath}*.* WHERE Text LIKE \'%ERROR%\' \"\n ";
            var resultOutput = new StringBuilder();
            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    while (!exeProcess.StandardOutput.EndOfStream)
                    {
                        var line = await exeProcess.StandardOutput.ReadToEndAsync();
                        resultOutput.AppendLine($"{line}");
                    }
                    while (!exeProcess.StandardError.EndOfStream)
                    {
                        var line = await exeProcess.StandardError.ReadToEndAsync();
                        resultOutput.AppendLine($"{line}");
                    }
                    await exeProcess.WaitForExitAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Cannot get log report\n{ex.Message}");
                return BadRequest();
            }
            return Content(resultOutput.ToString());
        }
    }
}
