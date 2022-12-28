using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL.Db;
using Domain;

namespace WebApp.Pages.CheckersOptions
{
    public class NewGameOptions : PageModel
    {
        private readonly DAL.Db.AppDbContext _context;

        public NewGameOptions(DAL.Db.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public CheckersOption CheckersOption { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.CheckersOption == null || CheckersOption == null)
          {
              return Page();
          }

          _context.CheckersOption.Add(CheckersOption);
          await _context.SaveChangesAsync();

          return RedirectToPage("../CheckersGames/Create", new {optionsId = CheckersOption.Id});
        }
    }
}
