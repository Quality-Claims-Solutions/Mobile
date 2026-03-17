using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mobile.Account;
using Mobile.Models;
using Mobile.Models.EntityModels;
using System.Security.Claims;

namespace Mobile.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Db _dbContext;

        // There are a lot of adjustments that have to be done to the database in Production before this will work.
        // Extra columns need to be added to a few of the ASPNET Identity tables, and values have to be updated as well.
        // Use the below as a template.

        // ALTER TABLE AspNetUsers ADD
        // NormalizedUserName NVARCHAR(256),
        // NormalizedEmail NVARCHAR(256),
        // ConcurrencyStamp NVARCHAR(MAX),
        // LockoutEnd DATETIMEOFFSET

        // ALTER TABLE AspNetRoles
        // ADD NormalizedName NVARCHAR(256) NULL,
        // ConcurrencyStamp NVARCHAR(MAX) NULL;

        // UPDATE AspNetUsers
        // SET NormalizedUserName = UPPER(UserName) where username = 'bob.vaselaar@qcsdirect.com'

        // UPDATE AspNetUsers
        // SET NormalizedEmail = UPPER(Email) where username = 'bob.vaselaar@qcsdirect.com'

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, Db dbContext)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {

            //return Content(result.ToString());

            //if (!ModelState.IsValid)
            //    return View(model);

            string userName = model.Email;

            if (!model.UseEmail)
            {
                //var user = _userManager.Users.FirstOrDefault(u => u.EmployeeId == model.EmployeeId);
                //if (user == null)
                //{
                //    model.ErrorMessage = "Incorrect username or password";
                //    return View(model);
                //}
                userName = "ImplementLater";
            }

            var result = await _signInManager.PasswordSignInAsync(userName, model.Password, model.RememberMe, lockoutOnFailure: true);

            var qcsUser = await _dbContext.User.FirstOrDefaultAsync(u => u.EmailAddress == userName);

            if (result.Succeeded)
            {
                var additionalClaimsResult = await AssignAdditionalIdentityClaims(userName);

                if (!(additionalClaimsResult is OkResult))
                {
                    await _signInManager.SignOutAsync();
                    model.ErrorMessage = "Failed to assign additional claims.";
                    return View(model);
                }

                return RedirectToLocal(returnUrl);
            }

            if (result.IsLockedOut)
            {
                return RedirectToAction("Lockout");
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToAction("TwoFactorAuthenticationSignIn", new { returnUrl, model.RememberMe });
            }

            model.ErrorMessage = "Incorrect username or password";

            return View(model);
        }

        /// <summary>
        /// This is a method that retrieves data from the User table that is useful during the lifecycle of the app.
        /// It loads the data into the Identity Claims for the user, so we don't have to query the database every time we need it.
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        private async Task<IActionResult> AssignAdditionalIdentityClaims(string emailAddress)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(emailAddress);

                if (user == null)
                {
                    return NotFound("User not found");
                }

                // Get basic QCS user information
                var qcsUser = await _dbContext.User.FirstOrDefaultAsync(u => u.EmailAddress == emailAddress);

                if (qcsUser == null)
                {
                    return NotFound("QCS User not found");
                }

                if (qcsUser.CarrierLocationID == null)
                {
                    return NotFound("User is not associated with a Carrier Location.");
                }

                // Grab the CarrierCompanyID through a join, since it's not directly on the User table
                int? carrierCompanyId = await ( from cc in _dbContext.CarrierCompany
                                               join cl in _dbContext.CarrierLocation on cc.CarrierCompanyID equals cl.CarrierCompanyID
                                               where cl.CarrierLocationID == qcsUser.CarrierLocationID
                                               select cc.CarrierCompanyID).FirstOrDefaultAsync();

                if (carrierCompanyId == null)
                {
                    return NotFound("User is not associated with a Carrier Company.");
                }

                // Step 1: Define the desired claims
                var desiredClaims = new HashSet<(string Type, string Value)>
                {
                    ("QCSUserId", qcsUser.UserID.ToString()),
                    ("CarrierLocationId", qcsUser.CarrierLocationID.ToString()),
                    ("CarrierCompanyId", carrierCompanyId.ToString())
                };

                if (!string.IsNullOrEmpty(qcsUser.DefaultProgramCode))
                {
                    desiredClaims.Add(("ProgramCode", qcsUser.DefaultProgramCode));
                }

                if (!string.IsNullOrEmpty(qcsUser.DefaultProgramCode))
                {
                    desiredClaims.Add(("DisplayName", qcsUser.Name));
                }

                // Step 2: Add claims from matchups
                var matchups = await _dbContext.User_AssociationMatchup
                    .Where(uam => uam.UserID == qcsUser.UserID && !uam.IsDeleted)
                    .ToListAsync();

                foreach (var matchup in matchups)
                {
                    if (matchup.CarrierLocationID.HasValue)
                    {
                        desiredClaims.Add(("CarrierLocationId", matchup.CarrierLocationID.Value.ToString()));
                    }

                    if (!string.IsNullOrEmpty(matchup.ProgramCode))
                    {
                        desiredClaims.Add(("ProgramCode", matchup.ProgramCode));
                    }
                }

                // Step 3: Load existing claims
                IList<Claim>? existingClaims = await _userManager.GetClaimsAsync(user);

                // Step 4: Remove stale claims
                var toRemove = existingClaims
                    .Where(c => !desiredClaims.Contains((c.Type, c.Value)))
                    .ToList();

                foreach (var claim in toRemove)
                {
                    await _userManager.RemoveClaimAsync(user, claim);
                }

                // Step 5: Add missing claims
                var toAdd = desiredClaims
                    .Where(d => !existingClaims.Any(c => c.Type == d.Type && c.Value == d.Value));

                foreach (var (type, value) in toAdd)
                {
                    await _userManager.AddClaimAsync(user, new Claim(type, value));
                }

                await _signInManager.RefreshSignInAsync(user);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(LoginViewModel model)
        {
            //if (!ModelState.IsValid)
            //    return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Optionally assign to a role
                //if (!string.IsNullOrEmpty(model.Role))
                //{
                //    await _userManager.AddToRoleAsync(user, model.Role);
                //}

                return RedirectToAction("Index", "Home");
            }

            // If failed, add errors to the model
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }
    }
}
