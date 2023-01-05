using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.DTO;
using ServiceLayer.Service.Realization;
using ServiceLayer.Service.Realization.IdentityServices;
using SimpleVote.UI.Models.ViewModels;

namespace SimpleVote.UI.Controllers
{
    [Authorize]
    public class FormController : Controller
    {
        FormService formService;
        UserService userManager;

        public FormController(FormService _formService, UserService user)
        {
            formService = _formService;
            userManager = user;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateForm()
        {
            return View();
        }

        [Route("form")]
        public async Task<IActionResult> Form(int? id)
        {
            return View();
        }
        
        public async Task<IActionResult> CreateQuestion(int formId)
        {
            QuestionViewModel vm = new QuestionViewModel()
            {
                FormId = formId,
                Answers = new List<string>()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuestion(QuestionViewModel form)
        {
            var answers = new List<AnswerDTO>();
            foreach (var answer in form.Answers)
            {
                answers.Add(new AnswerDTO()
                {
                    Value = answer
                });
            }
            var questionDTO = new QuestionDTO()
            {
                Title = form.Title,
                Type = form.Type,
                FormId = form.FormId,
                Answers = answers,
                Votes = new List<VoteDTO>()
            };
            var res = await formService.AddQuestion(questionDTO);
            return RedirectToAction("CreateQuestion", "Form", new { formId = form.FormId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateForm(FormViewModel form)
        {
            //if (ModelState.IsValid)
            //{
                if (form != null)
                {
                    List<ParticipantDTO> people = new List<ParticipantDTO>();
                    if (form.participantsFile != null && form.Type == "2")
                    {
                        using (var reader = new StreamReader(form.participantsFile.OpenReadStream()))
                        {
                            string? line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                string[] values = line.Split(',');
                                ParticipantDTO person = new ParticipantDTO
                                {
                                    Email = values[0],
                                    Name = values[1]
                                };

                                people.Add(person);
                            }
                        }
                    }
                    FormDTO _form = new FormDTO()
                    {
                        User = await userManager.GetUser(User),
                        Name = form.Name,
                        Type = form.Type != "1",
                        TotalVoters = 0,
                        Questions = form.Questions,
                        Participants = people,
                        Finished = false
                    };
                    var res = await formService.AddAsync(_form); // TO DO - WHEN ADD FORM ADD PARTICIPANTS
                    return RedirectToAction("CreateQuestion", "Form", new { formId = res.Id } );
                }
            //}

            return RedirectToAction("CreateForm", "Form");
        }
    }
}
