using Microsoft.EntityFrameworkCore;

namespace SolutionTemplate.DAL.Context
{
    public class SolutionTemplateDB : DbContext
    {
        public SolutionTemplateDB(DbContextOptions<SolutionTemplateDB> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder model)
        {
            base.OnModelCreating(model);

        }
    }
}
