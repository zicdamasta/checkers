using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DAL.Db;
using Domain;

namespace WebApp.Pages.CheckersGameStates
{
    public class CreateModel : PageModel
    {
        private readonly DAL.Db.AppDbContext _context;

        public CreateModel(DAL.Db.AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["CheckersGameId"] = new SelectList(_context.CheckersGame, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public CheckersGameState CheckersGameState { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.CheckersGameState == null || CheckersGameState == null)
            {
                return Page();
            }

            _context.CheckersGameState.Add(CheckersGameState);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
