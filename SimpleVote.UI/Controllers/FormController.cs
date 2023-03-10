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


        public async Task<IActionResult> ChangeStatus(int? id)
        {
            try
            {
                var form = (await formService.GetAsync((int)id));
                form.Finished = form.Finished != true;
                await formService.UpdateAsync(form);
                TempData["Message"] =
                    "Статус опитування успішно змінено!";
                return RedirectToAction("MyForms", "Home");
            }
            catch (Exception ex)
            {
                TempData["Message"] =
                    "Зараз статус опитування не може бути змінений!";
                return RedirectToAction("MyForms", "Home");
            }
        }

        public async Task<IActionResult> Report(int? id)
        {
            try
            {
                if (User.IsInRole("User") == false)
                {
                    TempData["Message"] =
                        "Нажаль у вас немає доступу до цієї сторінки, будь-ласка зареєструйтеся або увійдіть в свій облікови запис";
                    return RedirectToAction("Index", "Home");
                }
                var form = (await formService.GetAsync((int)id));
                ReportViewModel vm = new ReportViewModel()
                {
                    FormTitle = form.Name,
                    Questions = form.Questions.ToList(),
                    FormType = form.Type
                };
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Message"] =
                    "Щось пішло не так :(";
                return RedirectToAction("Index", "Home");
            }
        }

        [AllowAnonymous]
        [Route("participate")]
        public IActionResult Participate(int? id)
        {
            try
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
            catch (Exception ex)
            {
                TempData["Message"] =
                    "Щось пішло не так :(";
                return RedirectToAction("Index", "Home");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ParticipateSubmit(ParticipateViewModel vm)
        {
            try
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
            catch (Exception ex)
            {
                TempData["Message"] =
                    "Щось пішло не так :(";
                return RedirectToAction("Index", "Home");
            }
        }

        [AllowAnonymous]
        [Route("form")]
        public async Task<IActionResult> Form(int? id = 1)
        {
            try
            {
                FormDTO toDisplay = await formService.GetAsync((int)id);
                if (toDisplay.Finished != true)
                {
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
                            && allowedParticipants.Select(x => x.Id).ToList()
                                .Contains(int.Parse(HttpContext.Session.GetString("participantId"))))
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
                                Participant = allowedParticipants.Find(x => x.Id == ParticipantId)
                            };
                            return View(vm);
                        }
                        else
                        {
                            return RedirectToAction("Participate", "Form", new { id = toDisplay.Id });
                        }
                    }
                }

                TempData["Message"] =
                    "Нажаль дане опитування було завершено його власником";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Home");
            }

        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SubmitForm(ShowFormViewModel vm)
        {
            try
            {
                ShowFormViewModel _vm = new ShowFormViewModel();
                var form = await formService.GetAsync(vm.toShow.Id);
                if (vm.votes.Count != form.Questions.Count())
                {
                    TempData["Message"] =
                        "Потрібно відповісти на всі питання";
                    return RedirectToAction("Form", "Form", new { id = form.Id });
                }
                if (form.Type == false)
                {
                    for (int i = 0; i < vm.votes.Count; i++)
                    {
                        string SubmitedAnswer = "";
                        if (form.Questions.ToList()[i].Type == "2")
                        {
                            foreach (var item in vm.votes[i])
                            {
                                SubmitedAnswer += ("///" + item);
                            }
                        }
                        else
                        {
                            SubmitedAnswer = vm.votes[i][0];
                        }

                        var part = (await participantService.GetAllAsync()).First(x => x.FormId == form.Id);
                        await voteService.AddAsync(new VoteDTO()
                        {
                            QuestionId = form.Questions.ToList()[i].Id,
                            SubmitedAnswer = SubmitedAnswer,
                            Participant = part
                        });
                    }

                    TempData["Message"] =
                        "Форма Успішно надіслана";
                    return RedirectToAction("Index", "Home");
                }

                for (int i = 0; i < vm.votes.Count; i++)
                {
                    string SubmitedAnswer = "";
                    if (form.Questions.ToList()[i].Type == "2")
                    {
                        foreach (var item in vm.votes[i])
                        {
                            SubmitedAnswer += ("///" + item);
                        }
                    }
                    else
                    {
                        SubmitedAnswer = vm.votes[i][0];
                    }

                    int ParticipantId = int.Parse(HttpContext.Session.GetString("participantId"));
                    var participant = (await participantService.GetAllAsync()).First(x => x.Id == ParticipantId);
                    await voteService.AddAsync(new VoteDTO()
                    {
                        QuestionId = form.Questions.ToList()[i].Id,
                        SubmitedAnswer = SubmitedAnswer,
                        Participant = participant
                    });
                    HttpContext.Session.SetString("participantId", "");
                }

                TempData["Message"] =
                    "Форма Успішно надіслана";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["Message"] =
                    "Форма була надіслана некоректно";
                return RedirectToAction("Index", "Home");
            }
        }


        public async Task<IActionResult> CreateQuestion(int formId)
        {
            try
            {
                if (User.IsInRole("User") == false)
                {
                    TempData["Message"] =
                        "Нажаль у вас немає доступу до цієї сторінки, будь-ласка зареєструйтеся або увійдіть в свій облікови запис";
                    return RedirectToAction("Index", "Home");
                }
                QuestionViewModel vm = new QuestionViewModel()
                {
                    FormId = formId,
                    Answers = new List<string>()
                };
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Message"] =
                    "Вказано неіснуючий ідентифікатор форми";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuestion(QuestionViewModel form)
        {
            try
            {
                var answers = new List<AnswerDTO>();
                foreach (var answer in form.Answers)
                {
                    answers.Add(new AnswerDTO()
                    {
                        Value = answer
                    });
                }

                if (answers.Count < 1 && form.Type != "3" && form.Type != "4")
                {
                    TempData["Message"] =
                        "Питання не було створено через помилку";
                    return RedirectToAction("CreateQuestion", "Form", new { formId = form.FormId });
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
            catch (Exception ex)
            {
                TempData["Message"] =
                    "Питання не було створено через помилку";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateForm(FormViewModel form)
        {
            //if (ModelState.IsValid)
            //{
            try
            {
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

                    var res = await formService.AddAsync(_form);

                    var part = await participantService.AddAsync(new ParticipantDTO()
                    {
                        Email = "Анонімне",
                        Name = "Анонімне",
                        FormId = res.Id
                    });

                    return RedirectToAction("CreateQuestion", "Form", new { formId = res.Id });
                }
                //}

                return RedirectToAction("CreateForm", "Form");
            }
            catch (Exception ex)
            {
                TempData["Message"] =
                    "Форма не була створена через помилку";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
