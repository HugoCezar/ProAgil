using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProAgil.Domain;

namespace ProAgil.Repository
{
    public class ProAgilRepository : IProAgilRepository
    {
        private readonly ProAgilContext _context;

        public ProAgilRepository(ProAgilContext Context)
        {
            _context = Context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        //GERAIS
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            _context.Update(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        //EVENTOS
        public async Task<Evento[]> GetAllEventoAsync(bool includePalestrantes = false)
        {
            IQueryable<Evento> query = _context.Eventos
                .Include(c => c.Lotes)
                .Include(c => c.RedesSociais);

            if (includePalestrantes)
            {
                query = query.Include(pe => pe.PalestrantesEventos).ThenInclude(p => p.Palestrante);
            }

            query = query.AsNoTracking().OrderByDescending(c => c.DataEvento);

            return await query.ToArrayAsync();
        }

        public async Task<Evento> GetAllEventoAsyncById(int EventoId, bool includePalestrantes = false)
        {
            IQueryable<Evento> query = _context.Eventos
                .Include(c => c.Lotes)
                .Include(c => c.RedesSociais);

            if (includePalestrantes)
            {
                query = query.Include(pe => pe.PalestrantesEventos).ThenInclude(p => p.Palestrante);
            }

            query = query.AsNoTracking().OrderByDescending(c => c.DataEvento).Where(c => c.Id == EventoId);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<Evento[]> GetAllEventoAsyncByTema(string Tema, bool includePalestrantes = false)
        {
            IQueryable<Evento> query = _context.Eventos
                .Include(c => c.Lotes)
                .Include(c => c.RedesSociais);

            if (includePalestrantes)
            {
                query = query.Include(pe => pe.PalestrantesEventos).ThenInclude(p => p.Palestrante);
            }

            query = query.AsNoTracking().OrderByDescending(c => c.DataEvento).Where(c => c.Tema.ToLower().Contains(Tema.ToLower()));

            return await query.ToArrayAsync();
        }

        //PALESTRANTES
        public async Task<Palestrante> GetPalestranteAsync(int PalestranteId, bool includeEventos = false)
        {
            IQueryable<Palestrante> query = _context.Palestrantes
                .Include(c => c.RedesSociais);

            if (includeEventos)
            {
                query = query.Include(pe => pe.PalestrantesEventos).ThenInclude(e => e.Evento);
            }

            query = query.AsNoTracking().OrderBy(p => p.Nome).Where(p => p.Id == PalestranteId);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<Palestrante[]> GetAllPalestrantesAsyncByName(string Name, bool includeEventos = false)
        {
            IQueryable<Palestrante> query = _context.Palestrantes
                .Include(c => c.RedesSociais);

            if (includeEventos)
            {
                query = query.Include(pe => pe.PalestrantesEventos).ThenInclude(e => e.Evento);
            }

            query = query.AsNoTracking().Where(p => p.Nome.ToLower().Contains(Name.ToLower()));

            return await query.ToArrayAsync();
        }
    }
}