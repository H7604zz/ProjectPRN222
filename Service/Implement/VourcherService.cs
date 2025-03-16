using ProjectPrn222.Models;
using ProjectPrn222.Service.Iterface;

namespace ProjectPrn222.Service.Implement
{
    public class VourcherService : IVourcherService
	{
		private readonly AppDbContext _context;
		public VourcherService(AppDbContext context)
		{
			_context = context;
		}
		public void AddVourcher(Vourcher vourcher)
		{
			_context.Vourchers.Add(vourcher);
			_context.SaveChanges();
		}

		public void UpdateVourcher(Vourcher vourcher)
		{
			_context.Vourchers.Update(vourcher);
			_context.SaveChanges();
		}

		public Vourcher? GetVourcher(string code)
		{
			return _context.Vourchers.FirstOrDefault(v => v.Code == code && v.IsActive);
		}

        public IQueryable<Vourcher> SearchVourcher(string keyword)
        {
			return _context.Vourchers.Where(v => v.Code.Contains(keyword));
        }

        public IQueryable<Vourcher> GetAllVourchers()
        {
			return _context.Vourchers;
        }
    }
}
