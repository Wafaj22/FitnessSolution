using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessSolution.Areas.Identity.Data;
using FitnessSolution.Helpers;
using FitnessSolution.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FitnessSolution.Controllers
{
    public class UsersController: Controller
    {
        private UserManager<FitnessSolutionUser> userManager;

        public UsersController(UserManager<FitnessSolutionUser> userManager)
        {
            this.userManager = userManager;
        }

        [Authorize(Roles = Constants.ROLE_ADMIN)]
        public IActionResult Index()
        {
            return View(userManager.Users);
        }

        [Authorize(Roles = Constants.ROLE_ADMIN)]
        public IActionResult RegisterUser()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = Constants.ROLE_ADMIN)]
        public async Task<IActionResult> RegisterUser(Register user)
        {
            if (ModelState.IsValid)
            {
                FitnessSolutionUser webUser = new FitnessSolutionUser
                {
                    UserName = user.Username,
                    Email = user.Username,
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber,
                    DOB = user.DOB,
                    EmailConfirmed = true,
                    Role = user.Role
                };

                IdentityResult result = await userManager.CreateAsync(webUser, user.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Users");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            };

            return View(user);
        }


        [Authorize(Roles = Constants.ROLE_ADMIN)]
        public async Task<IActionResult> Update(string id)
        {
            FitnessSolutionUser user = await userManager.FindByIdAsync(id);
            Boolean a = false;
            Boolean b = false;
            Boolean c = false;
            Boolean d = false;

            if (user.Role == Constants.ROLE_MEMBER)
            {
                a = true;
            }
            else if (user.Role == Constants.ROLE_TRAINER)
            {
                b = true;
            }
            else if (user.Role == Constants.ROLE_NUTRITIONIST)
            {
                c = true;
            }
            else
            {
                d = true;
            }

            ViewBag.users = new List<SelectListItem>
            {
                new SelectListItem{ Selected = d, Text ="SelectRole", Value = ""},
                new SelectListItem{ Selected = a, Text =Constants.ROLE_MEMBER, Value = Constants.ROLE_MEMBER},
                new SelectListItem{ Selected = b, Text =Constants.ROLE_TRAINER, Value = Constants.ROLE_TRAINER},
                new SelectListItem{ Selected = c, Text =Constants.ROLE_NUTRITIONIST, Value = Constants.ROLE_NUTRITIONIST}
            };

            if (user != null)
            {
                return View(user);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = Constants.ROLE_ADMIN)]
        public async Task<IActionResult> Update(string id, string phoneNumber, string Role)
        {
            FitnessSolutionUser user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                //user.DOB = dob;
                //user.Name = name;
                user.PhoneNumber = phoneNumber;
                user.Role = Role;

                IdentityResult result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Users");
                }
                else
                {
                    return View(user);
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View(user);
        }


        [Authorize(Roles = Constants.ROLE_ADMIN)]
        public async Task<IActionResult> Delete(string id)
        {
            FitnessSolutionUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                await userManager.DeleteAsync(user);
                return RedirectToAction("Index", "Users", new { msg = "User deleted!" });
            }
            else
            {
                return RedirectToAction("Index", "Users", new { msg = "Unable to delet user" });
            }
        }
    }
}
