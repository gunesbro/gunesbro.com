using Microsoft.Web.Helpers;
using PagedList;
using PersonelBlog.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace PersonelBlog.Controllers
{
    public class HomeController : Controller
    {
        DbModel Model = new DbModel();
        // GET: Home

        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ChangeLang (string language)
        {
            HttpCookie cookie;
            cookie = new HttpCookie("SelectedLanguage");
            Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
            cookie.Value = language;
            Response.SetCookie(cookie);
            if (Request.UrlReferrer != null) return Redirect(Request.UrlReferrer.LocalPath);
            return Redirect("/Home/Index");
           
        }

        #region Login - Logout - Register

        public ActionResult UserLogin()
        {
            if (Request.Cookies.AllKeys.Contains("SiteUser"))
            {
                if (HttpContext.Request.Cookies["SiteUser"].Value != "")
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult UserLogin(string user, string pass)
        {
            foreach (var item in Model.SiteUser)
            {
                if (item.NickName == user && item.Password == pass)
                {
                    string cookieName = "SiteUser";
                    string cookieValue = item.NickName;
                    HttpCookie cookie = new HttpCookie(cookieName, cookieValue);
                    HttpContext.Response.Cookies.Add(cookie);

                    string cookieName1 = "AnketDurum";
                    string cookieValue1 = "false";
                    HttpCookie cookie1 = new HttpCookie(cookieName1, cookieValue1);
                    HttpContext.Response.Cookies.Add(cookie1);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = "Username or Password is not valid";
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            Response.Cookies["SiteUser"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["AnketDurum"].Expires = DateTime.Now.AddDays(-1);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            if (Request.Cookies.AllKeys.Contains("SiteUser"))
            {
                if (HttpContext.Request.Cookies["SiteUser"].Value != "")
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult Register(string name, string surname, string email, string nickName, string pass1, string pass2)
        {
            if (name != "" && surname != "" && email != "" && nickName != "" && pass1 != "" && pass2 != "")
            {
                if (pass1 == pass2)
                {
                    SiteUser newUser = new SiteUser()
                    {
                        Name = name,
                        Surname = surname,
                        Email = email,
                        NickName = nickName,
                        Password = pass2
                    };
                    Model.SiteUser.Add(newUser);
                    Model.SaveChanges();

                    return RedirectToAction("UserLogin", "Home");
                }
                else
                {
                    ViewBag.Error = "Passwords are not match!";
                }

            }
            else
            {
                ViewBag.Error = "Empty place detected!";
            }

            return View();
        }
        #endregion

        #region Recent Posts
        public PartialViewResult RecentPosts(int? page)
        {


            int dataPerPage = 3;
            return PartialView(Model.Content.OrderByDescending(o => o.PostDate).ToPagedList<Content>(page ?? 1, dataPerPage));
        }
        #endregion

        #region Categories
        public ActionResult Page(int? id)
        {
            if (id != null)
            {
                var CategroyList = (from c in Model.Pages
                                    where c.PageId == id
                                    select c).ToList();
                return View(CategroyList);
            }

            return View();
        }
        public PartialViewResult PageContents(int? id)
        {
            if (id != null)
            {
                var CategroyList = (from c in Model.Content
                                    where c.PageId == id
                                    select c).ToList();
                return PartialView(CategroyList);
            }
            else
            {
                return PartialView("Page", "Home");
            }

        }
        #endregion

        #region Tags
        public ActionResult TagSearch()
        {
            return View();
        }
        public PartialViewResult TagName(int? id)
        {
            if (id != null)
            {
                var Tag = (from c in Model.ContentWithTags
                           where c.Tags.TagId == id
                           select c).ToList();
                return PartialView(Tag);
            }
            return PartialView();

        }
        public PartialViewResult TagContents(int? id)
        {
            if (id != null)
            {
                var TagList = (from c in Model.ContentWithTags
                               where c.TagsId == id
                               select c).ToList();
                return PartialView(TagList);
            }
            return PartialView();
        }
        #endregion

        #region Contents
        public ActionResult Content(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.PostId = id;

            var f = Model.Content.Find(id);
            ViewBag.CategoryId = f.PageId;
            f.HitCounter++;
            Model.SaveChanges();

            return View();
        }

        public PartialViewResult ContentInfo(int? id)
        {
            var find = (from f in Model.Content
                        where f.ContentId == id
                        select f).ToList();
            return PartialView(find);
        }
        public PartialViewResult ContentTags(int? id)
        {
            var find = (from f in Model.ContentWithTags
                        where f.ContentId == id
                        select f).ToList();
            return PartialView(find);

        }
        #endregion

        #region Partials
        public PartialViewResult Header()
        {
            return PartialView();
        }

        public PartialViewResult Banner()
        {
            return PartialView(Model.Pages.OrderBy(r => Guid.NewGuid()));
        }

        public PartialViewResult AboutMe()
        {
            return PartialView(Model.AboutMe.ToList());
        }

        public PartialViewResult FollowUs()
        {
            return PartialView(Model.FollowUs.ToList());
        }

        public PartialViewResult PopularPosts()
        {
            return PartialView(Model.Content.OrderByDescending(o => o.HitCounter).ToList());
        }

        public PartialViewResult Categories()
        {
            return PartialView(Model.Pages.ToList());
        }

        public PartialViewResult Advertısement()
        {
            return PartialView(Model.ADS.ToList());
        }

        public PartialViewResult Instagram()
        {
            return PartialView(Model.InstagramEmbed.OrderByDescending(o => o.Id).ToList());
        }


        #endregion

        public ActionResult YouMightAlsoLike(int categoryId,int postId)
        {
            var youMightAlsoLike = (from y in Model.Content
                                    where y.PageId == categoryId && y.ContentId != postId
                                    select y).ToList().OrderBy(o => o.HitCounter).Take(3);
            return PartialView(youMightAlsoLike);
        }

        #region Comments - Publish Comments
        public ActionResult Comments(int PostId)
        {
            var find = (from f in Model.Comment
                        where f.ContentId == PostId
                        select f).OrderBy(o => o.CommentDate).ToList();

            //ViewBag.TheIsNoComment = "There is No Comment For this Post";

            return PartialView(find);
        }

        public ActionResult SingleComment(int id)
        {
            var find = (from f in Model.Comment
                        where f.CommentId == id
                        select f).FirstOrDefault();
            return PartialView("_SingleComment", find);
        }

        public ActionResult LeaveComment(int PostId)
        {
            ViewBag.PostId = PostId;
            return PartialView();
        }

        
        [HttpPost]
        public JsonResult LeaveComment(string pId, string Comment)
        {
            int cId=0;
            if (Comment != null)
            {
                Comment c = new Comment()
                {
                    from = Request.Cookies["SiteUser"].Value.ToString(),
                    CommentText = Comment,
                    ContentId = int.Parse(pId),
                    CommentDate = DateTime.Now

                };
                Model.Comment.Add(c);

                Model.SaveChanges();
                cId = c.CommentId;
            }

            return Json(new { Status = 0,id = cId});
        }

        #endregion

        #region Q - A
        public ActionResult QuestionAnswer()
        {
            return View(Model.Question.OrderByDescending(o => o.QuestionPostTime).ToList());
        }

        public ActionResult AskQuestion()
        {
            if (!Request.Cookies.AllKeys.Contains("SiteUser"))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        int questionId;

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AskQuestion(string txtHeader, string txtContext)
        {
            if (Request.Cookies.AllKeys.Contains("SiteUser"))
            {
                if (HttpContext.Request.Cookies["SiteUser"].Value != "")
                {
                    string name = HttpContext.Request.Cookies["SiteUser"].Value;

                    foreach (var item in Model.SiteUser)
                    {
                        if (item.NickName == name)
                        {
                            questionId = item.SiteUserId;
                        }
                    }

                    Question q = new Question()
                    {
                        QuestionSummary = txtHeader,
                        QuestionContext = txtContext,
                        QuestionPostTime = DateTime.Now,
                        SiteUserId = questionId

                    };
                    Model.Question.Add(q);
                    Model.SaveChanges();

                    return RedirectToAction("QuestionAnswer", "Home");
                }
            }

            return View();
        }

        public ActionResult ViewQusetion(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("QuestionAnswer", "Home");
            }
            else
            {
                var find = (from c in Model.Question
                            where c.QuestionId == id
                            select c).ToList(); 
                var f = Model.Question.Find(id);
                ViewBag.QuestionId = f.QuestionId;
                return View(find);
            }
        }

        public ActionResult Answers(int? QuestionId)
        {
            var find = (from f in Model.QuestionComment
                        where f.QuestionId == QuestionId
                        select f).OrderBy(o => o.QuestionCommentId).ToList();

            return PartialView(find.ToList());
        }

        public ActionResult SingleAnswer(int id)
        {
            var find = (from f in Model.QuestionComment
                        where f.QuestionCommentId == id
                        select f).FirstOrDefault();
            return PartialView("_SingleAnswer", find);
        }

        public ActionResult LeaveQuestionComment(int? QuestionId)
        {
            ViewBag.QuestionId = QuestionId;
            return PartialView();
        }

        int userId;
        [HttpPost]
        public JsonResult LeaveQuestionComment(string pId, string Comment)
        {
            int aId = 0;
            if (Comment != null)
            {
                string loggedınUser = Request.Cookies["SiteUser"].Value.ToString();

                foreach (var item in Model.SiteUser)
                {
                    if (item.NickName == loggedınUser)
                    {
                        userId = item.SiteUserId;
                        break;
                    }
                }
                QuestionComment c = new QuestionComment()
                {
                    QuestionUserId = userId,
                    QuestionAnswerContext = Comment,
                    QuestionId = int.Parse(pId)
                };
                Model.QuestionComment.Add(c);
                Model.SaveChanges();
                aId = c.QuestionCommentId;
            }
            return Json(new { Status = 0, id = aId });
        }

        #endregion


        public ActionResult NavigationBar()
        {
            return PartialView(Model.Pages.ToList());
        }

        public ActionResult PopularPostsFooter()
        {
            return PartialView(Model.Content.OrderByDescending(o => o.HitCounter).ToList());
        }
        public ActionResult TagsFooter()
        {
            return PartialView(Model.Tags.ToList());
        }

        #region Profile


        public ActionResult Profile()
        {
            return View();
        }

        int findId;
        public ActionResult _AccountSettings()
        {
            string loggedinUser = Request.Cookies["SiteUser"].Value.ToString();

            foreach (var item in Model.SiteUser)
            {
                if (item.NickName == loggedinUser)
                {
                    findId = item.SiteUserId;
                    break;
                }
            }
            var found = Model.SiteUser.Find(findId);
            ViewBag.Name = found.Name;
            ViewBag.Surname = found.Surname;
            ViewBag.Email = found.Email;
            return PartialView();
        }

        int f;
        [HttpPost]
        public ActionResult _AccountSettings(string txtName, string txtSurname, string txtEmail)
        {
            try
            {
                string loggedinUser = Request.Cookies["SiteUser"].Value.ToString();

                foreach (var item in Model.SiteUser)
                {
                    if (item.NickName == loggedinUser)
                    {
                        f = item.SiteUserId;
                        break;
                    }
                }

                var found = Model.SiteUser.Find(f);
                found.Name = txtName;
                found.Surname = txtSurname;
                found.Email = txtEmail;
                Model.SaveChanges();
            }
            catch (Exception)
            {

            }

            return RedirectToAction("Profile", "Home");
        }

        public ActionResult _ProfileQuestionList()
        {
            string loggedinUser = Request.Cookies["SiteUser"].Value.ToString();

            foreach (var item in Model.SiteUser)
            {
                if (item.NickName == loggedinUser)
                {
                    findId = item.SiteUserId;
                    break;
                }
            }
            var found = Model.SiteUser.Find(findId);

            var q = (from a in Model.Question
                     where a.SiteUserId == found.SiteUserId
                     select a).OrderByDescending(o => o.QuestionPostTime).ToList();
            return PartialView(q);
        }

        public ActionResult DeleteQuestion(int? gelenId)
        {
            var found = Model.Question.Find(gelenId);

            foreach (var item in Model.QuestionComment)
            {
                if (item.QuestionId == gelenId)
                {
                    Model.QuestionComment.Remove(item);
                }
            }

            Model.Question.Remove(found);
            Model.SaveChanges();
            return RedirectToAction("Profile", "Home");
        }
        #endregion

        #region Questionnaire

        public ActionResult _Questionnaire()
        {
            //Cookieye durumu kaydet ordan oku.
            var select = from s in Model.Questionnaire
                         select s;
            foreach (var item in Model.UsersQuestionnaireSelection)
            {
                if (Request.Cookies.AllKeys.Contains("AnketDurum"))
                {
                    if (item.SiteUser.NickName == HttpContext.Request.Cookies["SiteUser"].Value)
                    {
                        HttpContext.Request.Cookies["AnketDurum"].Value = "true";
                    }
                }
            }
            return PartialView(select.OrderByDescending(o=>o.Id).Take(1));

        }

        int qUserId;
        [HttpPost]
        public JsonResult _Questionnaire(FormCollection frm)
        {
            string user = Request.Cookies["SiteUser"].Value.ToString();
            foreach (var item in Model.SiteUser)
            {
                if (item.NickName == user)
                {
                    qUserId = item.SiteUserId;
                }
            }
            string selectedRdbtn = frm["Option"].ToString();
            UsersQuestionnaireSelection n = new UsersQuestionnaireSelection()
            {
                Selection = selectedRdbtn,
                QuestionnaireId = 1,
                SiteUserId = qUserId
            };
            Model.UsersQuestionnaireSelection.Add(n);
            Model.SaveChanges();

            HttpContext.Request.Cookies["AnketDurum"].Value = "true";
            return Json(JsonRequestBehavior.AllowGet);

        }

        #endregion
    }
}