using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using loginRegister.Models;
using Microsoft.AspNetCore.Identity;

namespace loginRegister.Controllers;

public class HomeController : Controller
{
    private MyContext _context;
    private readonly ILogger<HomeController> _logger;


    public HomeController(ILogger<HomeController> logger,MyContext context)
    {
        _context=context;
        _logger = logger;
    }

    public IActionResult Index()
    {
        HttpContext.Session.Clear();
        return View();
    }

    [HttpPost("register")]
    public IActionResult Register(User newUser)
    {
        if(ModelState.IsValid){
            if(_context.Users.Any(a=>a.Email==newUser.Email)){
                ModelState.AddModelError("Email","Email is already in use");
                return View("Index");
            }
            PasswordHasher<User> Hasher=new PasswordHasher<User>();
            newUser.Password=Hasher.HashPassword(newUser,newUser.Password);
            _context.Add(newUser);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("user",newUser.UserId);
            return RedirectToAction("Success");
        }
        else{
            return View("Index");
        }
    }

    [HttpGet("Success")]
    public IActionResult Success()
    {
        if(HttpContext.Session.GetInt32("user")==null){
            return RedirectToAction("Index");
        }
        User loggedUser=_context.Users.FirstOrDefault(a=>a.UserId== (int)HttpContext.Session.GetInt32("user"));
        return View("Success",loggedUser);
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("logincheck")]
    public IActionResult LoginCheck(LoginUser loginUser)
    {
        if(ModelState.IsValid){
            User userInDb=_context.Users.FirstOrDefault(u=>u.Email==loginUser.Email);
            if(userInDb==null){
                ModelState.AddModelError("Email","Invalid Login");
                return View("Login");
            }
            PasswordHasher<LoginUser> hasher=new PasswordHasher<LoginUser>();
            var result = hasher.VerifyHashedPassword(loginUser, userInDb.Password, loginUser.Password);
            if(result==0){
                ModelState.AddModelError("Email","Invalid Login");
                return View("Login");
            }
            else{
                HttpContext.Session.SetInt32("user",userInDb.UserId);
            return RedirectToAction("success");
            }
        }else{
            return View("login");
        }

    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
