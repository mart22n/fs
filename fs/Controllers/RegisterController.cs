using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fs.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minio;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace fs.Controllers
{
    public class registerController : Controller
    {
        // GET: Registration
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(User user)
        {
            //UsersEntities usersEntities = new UsersEntities();
            //usersEntities.Users.Add(user);
            //usersEntities.SaveChanges();
            string message = string.Empty;
            switch (user.Id)
            {
                case -1:
                    message = "Username already exists.\\nPlease choose a different username.";
                    break;
                case -2:
                    message = "Supplied email address has already been used.";
                    break;
                default:
                    message = "Registration successful.\\nUser Id: " + user.Id.ToString();
                    break;
            }
            ViewBag.Message = message;
            TestContext tc = new TestContext();
            int ret = tc.Database.ExecuteSqlRaw(
                "INSERT INTO users (fullname, nickname, password, email) VALUES ({0}, {1}, {2}, {3});", user.Fullname,
                user.Nickname, user.Password, user.Email);
            var result = tc.Users.FromSqlRaw("SELECT * FROM users;").ToList();
            return View(user);
        }
    }

}