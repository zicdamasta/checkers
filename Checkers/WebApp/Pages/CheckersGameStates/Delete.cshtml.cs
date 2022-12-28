using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL.Db;
using Domain;

namespace WebApp.Pages.CheckersGameStates
{
    public class DeleteModel : PageModel
    {
        private readonly DAL.Db.AppDbContext _context;

        public DeleteModel(DAL.Db.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public CheckersGameState CheckersGameState { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || _context.CheckersGameState == null)
            {
                return NotFound();
            }

            var checkersgamestate = await _context.CheckersGameState.FirstOrDefaultAsync(m => m.Id == id);

            if (checkersgamestate == null)
            {
                return NotFound();
            }
            else 
            {
                CheckersGameState = checkersgamestate;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null || _context.CheckersGameState == null)
            {
                return NotFound();
            }
            var checkersgamestate = await _context.CheckersGameState.FindAsync(id);

            if (checkersgamestate != null)
            {
                CheckersGameState = checkersgamestate;
                _context.CheckersGameState.Remove(CheckersGameState);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
