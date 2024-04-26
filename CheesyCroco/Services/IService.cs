using CheesyCroco.Models;
using MongoDB.Driver;

namespace CheesyCroco.Services
{
    public interface IService<T>
    {

        public T GetById(string id);

        public List<T> GetAll();

        public void SaveOrUpdate(T obj);

    }
}
