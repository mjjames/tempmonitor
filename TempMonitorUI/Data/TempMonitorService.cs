using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TempMonitorUI.Models;

namespace TempMonitorUI.Data
{
    public class TempMonitorService
    {
        private readonly TempMonitorContext _context;

        public TempMonitorService(TempMonitorContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }

        public async Task<AtmosphericCondition[]> GetForDate(DateTime startDate)
        {
            return await _context.AtmosphericConditions
                                .Where(ac => ac.TimeStamp >= startDate.Date)
                                .ToArrayAsync();
        }

        internal async Task Add(AtmosphericCondition condition)
        {
            await _context.AddAsync(condition);
            await _context.SaveChangesAsync();
        }
    }
}
