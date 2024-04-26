using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit.Services
{
    public interface IService<T>
    {
        public T GetById(string id);

        public List<T> GetAll();

        public void SaveOrUpdate(T obj);
    }
}