using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL.Db;
using Domain;

namespace WebApp.Pages.CheckersOptions
{
    public class DeleteModel : PageModel
    {
        private readonly DAL.Db.AppDbContext _context;

        public DeleteModel(DAL.Db.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public CheckersOption CheckersOption { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || _context.CheckersOption == null)
            {
                return NotFound();
            }

            var checkersoption = await _context.CheckersOption
                .Include(c=>c.CheckersGames)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (checkersoption == null)
            {
                return NotFound();
            }
            else 
            {
                CheckersOption = checkersoption;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null || _context.CheckersOption == null)
            {
                return NotFound();
            }
            var checkersoption = await _context.CheckersOption.FindAsync(id);

            if (checkersoption != null)
            {
                CheckersOption = checkersoption;
                _context.CheckersOption.Remove(CheckersOption);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
