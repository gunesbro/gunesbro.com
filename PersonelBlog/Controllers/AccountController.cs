using PagedList;
using PersonelBlog.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace PersonelBlog.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account

        [UserAuthorize]
        public ActionResult Index()
        {
            return View();
        }
        DbModel Model = new DbModel();


        #region Login - Logout
        public ActionResult Login()
        {
            if (HttpContext.Request.Cookies["userAccess"] != null)
            {
                return RedirectToAction("Index", "Account");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string txtUserName, string txtUserPassword)
        {
            txtUserName.Replace("/", " ");
            txtUserPassword.Replace("/", " ");

            string crypto;
            crypto = FormsAuthentication.HashPasswordForStoringInConfigFile(txtUserPassword, "sha1");

            var login = from l in Model.User
                        select l;

            foreach (var item in login)
            {
                if (item.UserName == txtUserName.ToString() && item.UserPassword == crypto.ToString())
                {
                    string cookieName = "userAccess";
                    string cookieValue = item.UserName;
                    HttpCookie cookie = new HttpCookie(cookieName, cookieValue);
                    HttpCookie mail = new HttpCookie("email", item.UserInfo.Email);
                    cookie.Expires = DateTime.Now.AddHours(1); // Süre(1 saat)
                    HttpContext.Response.Cookies.Add(cookie);
                    HttpContext.Response.Cookies.Add(mail);
                    return RedirectToAction("Index", "Account");
                }
                else
                {
                    ViewBag.Error = "Geçersiz kullanıcı adı veya şifre.";
                }
            }
            return View();
        }

        public ActionResult LogOut()
        {
            try
            {
                string cookieName = "userAccess";
                HttpCookie myCookie = new HttpCookie(cookieName);
                myCookie.Expires = DateTime.Now.AddHours(-1); // Süreyi  1 saat azaltıyoruz
                Response.Cookies.Add(myCookie);
                return RedirectToAction("Login", "Account");
            }
            catch (Exception)
            {
                return View();
            }

        }
        #endregion

        #region Categories
        [UserAuthorize]
        public ActionResult Pages(int? page)
        {
            int dataPerPage = 4;
            var PageList = (from l in Model.Pages
                            select l).ToList().ToPagedList<Pages>(page ?? 1, dataPerPage);
            return View(PageList);
        }
        [UserAuthorize]
        public ActionResult NewPage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewPage(string txtCatName, string txtCatInfo, HttpPostedFileBase foto)
        {

            if (foto != null && txtCatName != "" && txtCatInfo != "")
            {
                WebImage img = new WebImage(foto.InputStream);
                FileInfo fotoInfo = new FileInfo(foto.FileName);

                newFoto = Guid.NewGuid().ToString() + fotoInfo.Extension;
                img.Resize(1080, 720);
                img.Save("~/Image/" + newFoto);

                Pages newPage = new Pages()
                {
                    PageName = txtCatName,
                    PageTitle = txtCatInfo,
                    PageImage = "/Image/" + newFoto
                };
                Model.Pages.Add(newPage);
                Model.SaveChanges();
                return RedirectToAction("Pages");
            }
            else
            {
                ViewBag.Error = "Empty place detected!";
            }
            return View();
        }


        [UserAuthorize]
        public ActionResult EditPage(int? editId)
        {
            if (editId != null)
            {
                var find = Model.Pages.Find(editId);
                ViewBag.CatName = find.PageName;
                ViewBag.CatInfo = find.PageTitle;
                ViewBag.EditId = find.PageId;
                ViewBag.ImageSource = find.PageImage;

            }
            else
            {
                return RedirectToAction("Pages", "Account");
            }
            return View();
        }

        [HttpPost]
        public ActionResult EditPage(string txtCatName, string txtCatInfo, HttpPostedFileBase foto, string txtId)
        {
            if (foto != null)
            {
                if (txtCatName != "" && txtCatInfo != "")
                {
                    WebImage img = new WebImage(foto.InputStream);
                    FileInfo fotoInfo = new FileInfo(foto.FileName);

                    newFoto = Guid.NewGuid().ToString() + fotoInfo.Extension;
                    img.Resize(1080, 720);
                    img.Save("~/Image/" + newFoto);

                    int id = int.Parse(txtId);
                    var edit = Model.Pages.Find(id);
                    edit.PageName = txtCatName;
                    edit.PageTitle = txtCatInfo;
                    edit.PageImage = "/Image/" + newFoto;

                    Model.SaveChanges();

                    return RedirectToAction("Pages", "Account");
                }
                else
                {
                    ViewBag.Error = "Empty field detected!";
                }
            }
            else
            {
                if (txtCatName != "" && txtCatInfo != "")
                {
                    int id = int.Parse(txtId);
                    var edit = Model.Pages.Find(id);
                    edit.PageName = txtCatName;
                    edit.PageTitle = txtCatInfo;

                    Model.SaveChanges();
                    return RedirectToAction("Pages", "Account");
                }
                else
                {
                    ViewBag.Error = "Empty field detected!";
                }
            }
            return View();
        }

        public ActionResult DeletePage(int delId)
        {
            Pages del = Model.Pages.Find(delId);
            var findContents = (from f in Model.Content
                                where f.PageId == delId
                                select f).ToList();
            //Sırasıyla postun tagını,postu, ve posta ait resmi siler.
            foreach (var item in findContents)
            {
                foreach (var item1 in Model.ContentWithTags)
                {
                    if (item.ContentId == item1.ContentId)
                    {
                        Model.ContentWithTags.Remove(item1);
                        Model.Content.Remove(item);

                        //Silinen postun resmini dosyalardan silmek için...
                        string fullPath = Request.MapPath("~" + item.ContentImage);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }
            }
            //Kategoriyi siler.
            Model.Pages.Remove(del);
            Model.SaveChanges();
            return RedirectToAction("Pages");
        }
        #endregion

        #region About Me
        [UserAuthorize]
        public ActionResult AboutMe()
        {
            string loggedInUser = HttpContext.Request.Cookies["userAccess"].Value;
            foreach (var item in Model.User)
            {
                if (item.UserName == loggedInUser)
                {
                    if (item.Role.Role1 == "Admin")
                    {
                        var AboutMe = (from l in Model.AboutMe
                                       select l).ToList();
                        return View(AboutMe);
                    }
                }
            }
            return RedirectToAction("Index", "Account");

        }

        [UserAuthorize]
        public ActionResult EditAboutMe(int? gelenId)
        {
            AboutMe me = Model.AboutMe.Find(gelenId);
            ViewBag.AboutId = me.Id;
            ViewBag.Context = me.AboutMeText;
            return View();
        }

        [UserAuthorize]
        public ActionResult SaveAboutMeChanges(string txtAboutMe, string txtAboutId)
        {
            if (txtAboutMe != "")
            {
                var edit = Model.AboutMe.Find(int.Parse(txtAboutId));
                edit.AboutMeText = txtAboutMe;
                Model.SaveChanges();
                return RedirectToAction("AboutMe", "Account");
            }
            else
            {
                ViewBag.Error = "Empty place detected! Fill it!";
                return RedirectToAction("AboutMe", "Account");
            }


        }
        #endregion

        #region Follow Us
        [UserAuthorize]
        public ActionResult FollowUs()
        {
            return View(Model.FollowUs.ToList());
        }

        [UserAuthorize]
        public ActionResult EditFollowUs(string editFollowUsId, string facebook, string twitter, string instagram, string linkedin)
        {
            if (editFollowUsId != "")
            {
                var find = Model.FollowUs.Find(int.Parse(editFollowUsId));
                find.FacebookLink = facebook;
                find.TwitterLink = twitter;
                find.InstagramLink = instagram;
                find.LinkedInLink = linkedin;
                Model.SaveChanges();
            }

            return RedirectToAction("FollowUs", "Account");
        }

        #endregion

        #region Tags

        [UserAuthorize]
        public ActionResult Tags()
        {
            return View(Model.Tags.OrderByDescending(o => o.TagId).ToList());
        }

        [UserAuthorize]
        public ActionResult AddTag(string txtTag)
        {
            Tags newTag = new Tags()
            {
                TagName = txtTag
            };
            Model.Tags.Add(newTag);
            Model.SaveChanges();

            return RedirectToAction("Tags", "Account");
        }

        [UserAuthorize]
        public ActionResult DeleteTag(int? gelenId)
        {
            if (gelenId != null)
            {
                Tags del = Model.Tags.Find(gelenId);

                Model.Tags.Remove(del);
                Model.SaveChanges();

                return RedirectToAction("Tags", "Account");
            }
            return View();
        }

        #endregion

        #region ADS
        [UserAuthorize]
        public ActionResult Ads()
        {
            return View(Model.ADS.ToList());
        }

        public ActionResult NewAdd()
        {
            return View();
        }
        [HttpPost]
        public ActionResult NewAdd(string txtCompanyName, string txtLink, HttpPostedFileBase foto)
        {
            if (foto != null)
            {
                if (txtCompanyName != "" && txtLink != "")
                {
                    WebImage img = new WebImage(foto.InputStream);
                    FileInfo fotoInfo = new FileInfo(foto.FileName);

                    newFoto = Guid.NewGuid().ToString() + fotoInfo.Extension;
                    img.Resize(1080, 720);
                    img.Save("~/Image/" + newFoto);

                    ADS newAd = new ADS()
                    {
                        AdCompany = txtCompanyName,
                        AdLink = txtLink,
                        AdImage = "/Image/" + newFoto
                    };
                    Model.ADS.Add(newAd);
                    Model.SaveChanges();

                    return RedirectToAction("Ads", "Account");
                }
                else
                {
                    ViewBag.Error = "Empty field detected!";
                }
            }
            else
            {
                ViewBag.Error = "Empty field detected!";
            }

            return View();
        }

        [UserAuthorize]
        public ActionResult DeleteAd(int? AddId)
        {
            var find = Model.ADS.Find(AddId);
            Model.ADS.Remove(find);
            Model.SaveChanges();
            return RedirectToAction("Ads", "Account");
        }
        #endregion

        #region Content
        [UserAuthorize]
        public ActionResult Content(int? page)
        {
            int dataPerPage = 5;
            var PageList = (from l in Model.Content
                            select l).ToList().OrderByDescending(o => o.PostDate).ToPagedList<Content>(page ?? 1, dataPerPage);
            return View(PageList);
        }

        [UserAuthorize]
        public ActionResult NewContent()
        {
            return View(Model.Pages.ToList());
        }

        string selectedTag;
        public ActionResult EditContent(int? editContentId)
        {
            if (editContentId != null)
            {
                var e = Model.Content.Find(editContentId);

                //foreach (var item in Model.ContentWithTags)
                //{
                //    if (item.ContentId == editContentId)
                //    {
                //        selectedTag = item.Tags.TagName;
                //    }
                //}

                ViewBag.Context = e.ContentText;
                ViewBag.Header = e.ContentHeader;
                ViewBag.Selected = e.Pages.PageName;
                ViewBag.EditId = e.ContentId;
                return View(Model.Pages.ToList());
            }
            else
            {
                return RedirectToAction("Content", "Account");
            }

        }

        int editCategoryId;
        [UserAuthorize]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditContent(string txtHeader, string cmbCategory, HttpPostedFileBase foto, string txtContext, string editId)
        {
            //Finding Selected Category's id
            var pageId = (from p in Model.Pages
                          where cmbCategory == p.PageName
                          select p).ToList();
            foreach (var item in pageId)
            {
                editCategoryId = item.PageId;
            }

            if (foto != null)
            {
                if (cmbCategory != "" && txtHeader != "" && txtContext != "")
                {
                    WebImage img = new WebImage(foto.InputStream);
                    FileInfo fotoInfo = new FileInfo(foto.FileName);

                    newFoto = Guid.NewGuid().ToString() + fotoInfo.Extension;
                    img.Resize(1080, 720);
                    img.Save("~/Image/" + newFoto);

                    int id = int.Parse(editId);
                    var edit = Model.Content.Find(id);
                    edit.ContentHeader = txtHeader;
                    edit.PageId = editCategoryId;
                    edit.ContentText = txtContext;
                    edit.ContentImage = "/Image/" + newFoto;

                    Model.SaveChanges();

                    return RedirectToAction("Content", "Account");
                }
                else
                {
                    ViewBag.Error = "Empty field detected!";
                }
            }
            else
            {
                if (cmbCategory != "" && txtHeader != "" && txtContext != "")
                {
                    int id = int.Parse(editId);
                    var edit = Model.Content.Find(id);
                    edit.ContentHeader = txtHeader;
                    edit.PageId = editCategoryId;
                    edit.ContentText = txtContext;

                    Model.SaveChanges();
                    return RedirectToAction("Content", "Account");
                }
                else
                {
                    ViewBag.Error = "Empty field detected!";
                }
            }
            return View();
        }

        public ActionResult ReadTagsPartial()
        {
            return PartialView(Model.Tags.ToList());
        }

        int id;
        int categroyId;
        int contentTagId;
        string newFoto;
        [UserAuthorize]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateContent(string txtHeader, string cmbCategory, string cmbTag, string txtContext, HttpPostedFileBase foto)
        {
            #region Nesesery Things

            //Finding Logged in User's id
            string userName = Request.Cookies["userAccess"].Value.ToString();
            var loggedUser = (from u in Model.User
                              where userName == u.UserName
                              select u).ToList();
            foreach (var item in loggedUser)
            {
                id = item.UserId;
            }

            //Finding Selected Category's id
            var pageId = (from p in Model.Pages
                          where cmbCategory == p.PageName
                          select p).ToList();
            foreach (var item in pageId)
            {
                categroyId = item.PageId;
            }

            //Finding Selected Tag's id
            var tagId = (from t in Model.Tags
                         where cmbTag == t.TagName
                         select t).ToList();
            foreach (var item in tagId)
            {
                contentTagId = item.TagId;
            }
            #endregion

            if (foto != null)
            {
                WebImage img = new WebImage(foto.InputStream);
                FileInfo fotoInfo = new FileInfo(foto.FileName);

                newFoto = Guid.NewGuid().ToString() + fotoInfo.Extension;
                img.Resize(1080, 720);
                img.Save("~/Image/" + newFoto);
            }

            Content create = new Content()
            {
                ContentHeader = txtHeader,
                ContentText = txtContext,
                ContentImage = "/Image/" + newFoto,
                PageId = categroyId,
                UserId = id,
                HitCounter = 0,
                PostDate = DateTime.Now,

            };

            ContentWithTags cwt = new ContentWithTags()
            {
                ContentId = create.ContentId,
                TagsId = contentTagId
            };

            Model.ContentWithTags.Add(cwt);
            Model.Content.Add(create);
            Model.SaveChanges();
            return RedirectToAction("Content", "Account");
        }

        [UserAuthorize]
        public ActionResult DeleteContent(int? gelenId)
        {
            Content del = Model.Content.Find(gelenId);

            //Content tablosu ContentWithTags tablosu ile ilişkili olduğundan 
            //Silme işlemi yapılırken önce ContentWithTags tablosundaki alan silinmelidir.
            //Bu sebepten aşşağıdaki kodu yazdım.
            if (gelenId != null)
            {
                foreach (var item in Model.ContentWithTags)
                {
                    if (item.ContentId == del.ContentId)
                    {
                        Model.ContentWithTags.Remove(item);
                    }
                }
                //Silinen postun resmini dosyalardan silmek için...
                string fullPath = Request.MapPath("~" + del.ContentImage);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                Model.Content.Remove(del);
                Model.SaveChanges();

                return RedirectToAction("Content", "Account");
            }

            return View();
        }
        #endregion

        #region Instagram Embed
        [UserAuthorize]
        public ActionResult Instagram()
        {
            return View(Model.InstagramEmbed.OrderByDescending(o => o.Id).ToList());
        }

        [UserAuthorize]
        [ValidateInput(false)]
        public ActionResult SaveInstagramEmbed(string embedInsta)
        {
            if (embedInsta != "")
            {
                InstagramEmbed embed = new InstagramEmbed()
                {
                    InstagramEmbed1 = embedInsta
                };
                Model.InstagramEmbed.Add(embed);
                Model.SaveChanges();
            }
            return RedirectToAction("Instagram", "Account");
        }

        [UserAuthorize]
        public ActionResult DeleteInstagramEmbed(string embedId)
        {
            var find = Model.InstagramEmbed.Find(int.Parse(embedId));
            Model.InstagramEmbed.Remove(find);
            Model.SaveChanges();
            return RedirectToAction("Instagram");
        }

        #endregion

        #region Panel Layout Page Side Bar(Sekmeler/Aboutme,Content,Instagram etc.)

        public ActionResult PanelSideBar()
        {
            string loggedInUser = HttpContext.Request.Cookies["userAccess"].Value;
            var find = (from f in Model.User
                        where f.UserName == loggedInUser
                        select f).ToList();
            return PartialView(find);
        }

        #endregion

        #region ADDING - DELETING EDITOR
        [UserAuthorize]
        public ActionResult Editor()
        {
            string loggedInUser = HttpContext.Request.Cookies["userAccess"].Value;
            foreach (var item in Model.User)
            {
                if (item.UserName == loggedInUser)
                {
                    if (item.Role.Role1 == "Admin")
                    {
                        string user = HttpContext.Request.Cookies["userAccess"].Value;
                        var users = (from u in Model.User
                                     where u.UserName != user
                                     select u).ToList();
                        return View(users);
                    }
                }
            }
            return RedirectToAction("Index", "Account");


        }

        [UserAuthorize]
        public ActionResult CreateEditor()
        {
            string loggedInUser = HttpContext.Request.Cookies["userAccess"].Value;
            foreach (var item in Model.User)
            {
                if (item.UserName == loggedInUser)
                {
                    if (item.Role.Role1 == "Admin")
                    {
                        return View();
                    }
                }
            }
            return RedirectToAction("Index", "Account");
        }

        [HttpPost]
        public ActionResult CreateEditor(string txtName, string txtLastName, string txtBirthDate, string txtEmail, string txtUserName, string txtPass, string txtPass2)
        {
            if (txtName != "" && txtLastName != "" && txtBirthDate != "" && txtEmail != "" && txtUserName != "" && txtPass != "" && txtPass2 != "")
            {
                string crypto;
                crypto = FormsAuthentication.HashPasswordForStoringInConfigFile(txtPass2, "sha1");

                UserInfo userInfo = new UserInfo()
                {
                    FirstName = txtName,
                    LastName = txtLastName,
                    BirthDate = DateTime.Parse(txtBirthDate),
                    Gender = "whatevs",
                    RegistrationDate = DateTime.Now,
                    Email = txtEmail
                };
                Model.UserInfo.Add(userInfo);
                Model.SaveChanges();

                var find = (from f in Model.UserInfo
                            where f.Email == txtEmail
                            select f).ToList().First();

                var found = Model.UserInfo.Find(find.UserInfoId);

                User user = new User()
                {
                    UserName = txtUserName,
                    UserPassword = crypto,
                    UserInfoId = found.UserInfoId,
                    RoleId = 2
                };
                Model.User.Add(user);
                Model.SaveChanges();
                return RedirectToAction("Editor", "Account");
            }
            else
            {
                ViewBag.Error = "Empty place detected!Fill it!!";
            }
            return View();
        }

        public ActionResult MakeAdmin(int? gelenId)
        {
            try
            {
                var found = Model.User.Find(gelenId);
                found.RoleId = 1;
                Model.SaveChanges();
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Editor", "Account");
        }

        public ActionResult MakeUser(int? gelenId)
        {
            try
            {
                var found = Model.User.Find(gelenId);
                found.RoleId = 2;
                Model.SaveChanges();
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Editor", "Account");
        }

        int findUserInfoId;
        public ActionResult DeleteUser(int? gelenId)
        {
            try
            {
                var found = Model.User.Find(gelenId);
                foreach (var item in Model.User)
                {
                    if (item.UserId == gelenId)
                    {
                        findUserInfoId = item.UserInfoId;
                        break;
                    }
                }

                var findUser = Model.UserInfo.Find(findUserInfoId);
                Model.UserInfo.Remove(findUser);
                Model.User.Remove(found);
                Model.SaveChanges();
            }
            catch (Exception)
            {
            }
            return RedirectToAction("Editor", "Account");
        }

        #endregion

        #region Questionnaire

        public ActionResult Questionnaire(int? page)
        {
            int dataPerPage = 10;
            var PageList = (from l in Model.UsersQuestionnaireSelection
                            select l).ToList().OrderByDescending(o => o.Questionnaire.Question).ToPagedList<UsersQuestionnaireSelection>(page ?? 1, dataPerPage);
            return View(PageList);
        }

        public ActionResult NewQuestionnaire()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewQuestionnaire(string txtQuestion, string txtOption1, string txtOption2)
        {
            if (txtQuestion != "" && txtOption1 != "" && txtOption2 != "")
            {
                Questionnaire newQuest = new Questionnaire()
                {
                    Question = txtQuestion,
                    Option1 = txtOption1,
                    Option2 = txtOption2
                };
                Model.Questionnaire.Add(newQuest);
                Model.SaveChanges();
                //Response.Cookies["AnketDurum"].Value = "false";
                return RedirectToAction("Questionnaire", "Account");
            }
            else
            {
                ViewBag.Error = "Empty place detected! Fill It!";
                return View();
            }

        }
        #endregion
    }
}