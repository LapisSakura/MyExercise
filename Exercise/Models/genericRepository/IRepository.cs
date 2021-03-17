using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exercise.Models.genericRepository
{
    interface IRepository<Table>
    {
        void create(Table _entity);
        IEnumerable<Table> getAll();
        Table getById(int id);
        void update(Table _entity);
        void delete(int id);
    }
}
