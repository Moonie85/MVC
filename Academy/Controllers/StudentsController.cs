
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Academy.Models;

public class StudentsController : Controller
{
    private readonly AcademyContext _context;

    public StudentsController(AcademyContext context)
    {
        _context = context;
    }

    // GET: STUDENTS
    public async Task<IActionResult> Index(string sortOrder)
    {
        // Переключатель сортировки для ФИО в вашем стиле (_asc / _desc)
        ViewData["FullNameSortParam"] = sortOrder == "full_name_desc" ? "full_name_asc" : "full_name_desc";

        // Загружаем студентов вместе со связанной таблицей Group (.Include)
        IQueryable<Academy.Models.Student> students = from s in _context.Students.Include(s => s.Group)
                                                      select s;

        // Сортируем по ФИО (так как full_name вычисляется в C#, сортируем в БД по Фамилии -> Имени -> Отчеству)
        switch (sortOrder)
        {
            case "full_name_asc":
                students = students.OrderBy(s => s.last_name)
                                   .ThenBy(s => s.first_name)
                                   .ThenBy(s => s.middle_name);
                break;
            case "full_name_desc":
                students = students.OrderByDescending(s => s.last_name)
                                             .ThenByDescending(s => s.first_name)
                                             .ThenByDescending(s => s.middle_name);
                break;
        }

        return View(await students.ToListAsync());
        //return View(await _context.Students.ToListAsync());
    }

    // GET: STUDENTS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var student = await _context.Students
            .Include(s => s.Group)
            .FirstOrDefaultAsync(m => m.stud_id == id);
        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    // GET: STUDENTS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: STUDENTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("stud_id,group,Group,last_name,first_name,middle_name,birth_date,email,phone,photo,full_name")] Student student)
    {
        if (ModelState.IsValid)
        {
            _context.Add(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(student);
    }

    // GET: STUDENTS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            return NotFound();
        }
        return View(student);
    }

    // POST: STUDENTS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("stud_id,group,Group,last_name,first_name,middle_name,birth_date,email,phone,photo,full_name")] Student student)
    {
        if (id != student.stud_id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(student);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(student.stud_id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(student);
    }

    // GET: STUDENTS/Delete/5
    public async Task<IActionResult> Delete(int? stud_id)
    {
        if (stud_id == null)
        {
            return NotFound();
        }

        var student = await _context.Students
            .FirstOrDefaultAsync(m => m.stud_id == stud_id);
        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }

    // POST: STUDENTS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? stud_id)
    {
        var student = await _context.Students.FindAsync(stud_id);
        if (student != null)
        {
            _context.Students.Remove(student);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool StudentExists(int? stud_id)
    {
        return _context.Students.Any(e => e.stud_id == stud_id);
    }
}
