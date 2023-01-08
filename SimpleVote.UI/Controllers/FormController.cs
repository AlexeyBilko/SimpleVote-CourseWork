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
        ParticipantService participantService;

        public FormController(FormService _formService, UserService user, VoteService vs, ParticipantService ps)
        {
            formService = _formService;
            userManager = user;
            voteService = vs;
            participantService = ps;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateForm()
        {
            return View();
        }


        [Route("participate")]
        public IActionResult Participate(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ParticipateViewModel vm = new ParticipateViewModel()
            {
                FormId = (int)id
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ParticipateSubmit(ParticipateViewModel vm)
        {
            var allowedParticipants =
                (await participantService.GetAllAsync()).Where(x => x.FormId == vm.FormId).ToList();
            if (allowedParticipants.Select(x => x.Email).ToList().Contains(vm.Email))
            {
                var participant = allowedParticipants.Find(x => x.Email == vm.Email);
                HttpContext.Session.SetString("participantId", participant.Id.ToString());
                return RedirectToAction("Form", "Form", new { id = vm.FormId });
            }

            TempData["Message"] =
                "Нажаль учасник з такою електронною поштою не може приймати участь в данному опитуванні";
            return RedirectToAction("Participate", "Form", new { id = vm.FormId });
        }

        [Route("form")]
        public async Task<IActionResult> Form(int? id = 1)
        {
            try
            {
                FormDTO toDisplay = await formService.GetAsync((int)id);
                if (toDisplay.Type == false)
                {
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
                else
                {
                    var allowedParticipants =
                        (await participantService.GetAllAsync()).Where(x => x.FormId == toDisplay.Id).ToList();
                    if (HttpContext.Session.GetString("participantId") != null
                        && allowedParticipants.Select(x=>x.Id).ToList().Contains(int.Parse(HttpContext.Session.GetString("participantId"))))
                    {
                        int ParticipantId = int.Parse(HttpContext.Session.GetString("participantId"));
                        List<List<string>> emptyVotes = new List<List<string>>();
                        for (int i = 0; i < toDisplay.Questions.Count(); i++)
                        {
                            emptyVotes.Add(new List<string>());
                            emptyVotes[i].Add("");
                        }
                        ShowFormViewModel vm = new ShowFormViewModel()
                        {
                            toShow = toDisplay,
                            votes = emptyVotes,
                            Participant = allowedParticipants.Find(x=>x.Id == ParticipantId)
                        };
                        return View(vm);
                    }
                    else
                    {
                        return RedirectToAction("Participate", "Form", new { id = toDisplay.Id });
                    }
                }
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
                    int ParticipantId = int.Parse(HttpContext.Session.GetString("participantId"));
                    var participant = (await participantService.GetAllAsync()).First(x => x.Id == ParticipantId);
                    await voteService.AddAsync(new VoteDTO()
                    {
                        QuestionId = form.Questions.ToList()[i].Id,
                        SubmitedAnswer = vm.votes[i][0],
                        Participant = participant
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
