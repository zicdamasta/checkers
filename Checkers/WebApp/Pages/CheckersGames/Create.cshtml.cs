using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL.Db;
using Domain;

namespace WebApp.Pages.CheckersGames
{
    public class CreateModel : PageModel
    {
        private readonly DAL.Db.AppDbContext _context;

        public CreateModel(DAL.Db.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(string? optionsId)
        {
            if (optionsId == null)
            {
                ViewData["CheckerOptionId"] = new SelectList(_context.CheckersOption, "Id", "Name");
            }
            else
            {
                ViewData["CheckerOptionId"] = new SelectList(_context.CheckersOption, "Id", "Name", optionsId);
            }
            return Page();
        }

        [BindProperty]
        public CheckersGame CheckersGame { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.CheckersGame == null || CheckersGame == null)
            {
                return Page();
            }

            _context.CheckersGame.Add(CheckersGame);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Play", new {id = CheckersGame.Id});
        }
    }
}
