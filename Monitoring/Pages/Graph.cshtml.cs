using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Monitoring.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

public class GraphModel : PageModel
{
    private readonly WebAppDbContext _context;

    public GraphModel(WebAppDbContext context)
    {
        _context = context;
    }

    public List<string> Labels { get; set; }
    public List<int> FinalTargetData { get; set; }
    public List<int> NowTargetData { get; set; }
    public List<int> ResultData { get; set; }
    public string CurrentTime { get; set; }
    public string ShiftStart { get; set; }
    public string ShiftEnd { get; set; }

    [BindProperty]
    public Production NewProduction { get; set; }

    public async Task OnGetAsync()
    {
        await LoadDataAsync();
    }

    public async Task<IActionResult> OnPostAsync([FromBody] Production newProduction)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        newProduction.Date = DateTime.Now;

        _context.Produces.Add(newProduction);
        await _context.SaveChangesAsync();

        await LoadDataAsync();

        return new JsonResult(new
        {
            labels = Labels,
            finalTargetData = FinalTargetData,
            nowTargetData = NowTargetData,
            resultData = ResultData,
            shiftStart = ShiftStart,
            shiftEnd = ShiftEnd
        });
    }

    private async Task LoadDataAsync()
    {
        var now = DateTime.Now;
        DateTime start, end;

        if (now.Hour >= 6 && now.Hour < 14)
        {
            start = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0);
            end = new DateTime(now.Year, now.Month, now.Day, 14, 0, 0);
        }
        else if (now.Hour >= 14 && now.Hour < 22)
        {
            start = new DateTime(now.Year, now.Month, now.Day, 14, 0, 0);
            end = new DateTime(now.Year, now.Month, now.Day, 22, 0, 0);
        }
        else
        {
            if (now.Hour >= 22)
            {
                start = new DateTime(now.Year, now.Month, now.Day, 22, 0, 0);
                end = start.AddDays(1).AddHours(6);
            }
            else
            {
                start = new DateTime(now.Year, now.Month, now.Day, 22, 0, 0).AddDays(-1);
                end = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0);
            }
        }

        ShiftStart = start.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        ShiftEnd = end.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

        var productions = await _context.Produces
            .Where(p => p.Date >= start && p.Date <= end)
            .OrderBy(p => p.Date)
            .ToListAsync();

        Labels = productions.Select(p => p.Date.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture)).ToList();
        FinalTargetData = productions.Select(p => p.FinalTarget).ToList();
        NowTargetData = productions.Select(p => p.NowTarget).ToList();
        ResultData = productions.Select(p => p.Result).ToList();
    }
}