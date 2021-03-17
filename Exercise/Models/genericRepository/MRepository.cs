using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Exercise.Models.genericRepository
{
    public class MRepository<Table> : IRepository<Table> where Table : class
    {
        dbTeam2_FinalEntities db = null;
        DbSet<Table> dbTable = null;

        public MRepository()
        {
            db = new dbTeam2_FinalEntities();
            dbTable = db.Set<Table>();
        }
        public void create(Table _entity)
        {
            dbTable.Add(_entity);
            db.SaveChanges();
        }

        public void delete(int id)
        {
            dbTable.Remove(getById(id));
            db.SaveChanges();
        }

        public IEnumerable<Table> getAll()
        {
            return dbTable;
        }

        public Table getById(int id)
        {
            return dbTable.Find(id);
        }

        public void update(Table _entity)
        {
            db.Entry<Table>(_entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}