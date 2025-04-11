#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Assignment1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Assignment1.Areas.Identity.Pages.Account.Manage
{
    public class ChangeNameModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ChangeNameModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     User's current first name
        /// </summary>
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        
        /// <summary>
        ///     User's current last name
        /// </summary>
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        /// <summary>
        ///     Status message for success/error notifications
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     Model for the form input
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     Input model for the form
        /// </summary>
        public class InputModel
        {
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
            [Display(Name = "New first name")]
            public string NewFirstName { get; set; }
            
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
            [Display(Name = "New last name")]
            public string NewLastName { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;

            Input = new InputModel
            {
                NewFirstName = "",
                NewLastName = ""
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            bool nameChanged = false;
            
            // Only update if the name has actually changed
            if (Input.NewFirstName != user.FirstName && !string.IsNullOrWhiteSpace(Input.NewFirstName))
            {
                user.FirstName = Input.NewFirstName;
                nameChanged = true;
            }
            
            if (Input.NewLastName != user.LastName && !string.IsNullOrWhiteSpace(Input.NewLastName))
            {
                user.LastName = Input.NewLastName;
                nameChanged = true;
            }

            if (nameChanged)
            {
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    await LoadAsync(user);
                    return Page();
                }
                
                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Your name has been updated";
            }
            else
            {
                StatusMessage = "Your name is unchanged";
            }
            
            return RedirectToPage();
        }
    }
}