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
        VoteService voteService;

        public FormController(FormService _formService, UserService user, VoteService vs)
        {
            formService = _formService;
            userManager = user;
            voteService = vs;
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
        public async Task<IActionResult> Form(int? id = 1)
        {
            try
            {
                FormDTO toDisplay = await formService.GetAsync((int)id);
                List<List<string>> emptyVotes = new List<List<string>>();
                for (int i = 0; i < toDisplay.Questions.Count(); i++)
                {
                    emptyVotes.Add(new List<string>());
                    emptyVotes[i].Add("");
                }
                ShowFormViewModel vm = new ShowFormViewModel()
                {
                    toShow = toDisplay,
                    votes = emptyVotes
                };
                return View(vm);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Home");
            }

        }


        [HttpPost]
        public async Task<IActionResult> SubmitForm(ShowFormViewModel vm)
        {
            ShowFormViewModel _vm = new ShowFormViewModel();
            var form = await formService.GetAsync(vm.toShow.Id);
            for (int i = 0; i < vm.votes.Count; i++)
            {
                if (form.Questions.ToList()[i].Type == "2")
                {
                    string SubmitedAnswer = "";
                    foreach (var item in vm.votes[i])
                    {
                        SubmitedAnswer += ("///" + item);
                    }

                    await voteService.AddAsync(new VoteDTO()
                    {
                        QuestionId = form.Questions.ToList()[i].Id,
                        SubmitedAnswer = SubmitedAnswer
                    });
                }
                else
                {
                    await voteService.AddAsync(new VoteDTO()
                    {
                        QuestionId = form.Questions.ToList()[i].Id,
                        SubmitedAnswer = vm.votes[i][0]
                    });
                }
            }
            return RedirectToAction("Index", "Home");
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
