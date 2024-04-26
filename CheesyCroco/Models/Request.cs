namespace CheesyCroco.Models
{
    public class Request<T>
    {
        public string TypeOfRequest { get; set; } = null!;

        public T Body { get; set; } = default!;
    }
}
