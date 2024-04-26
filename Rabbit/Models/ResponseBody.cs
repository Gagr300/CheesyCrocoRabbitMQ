namespace Rabbit.Models
{
    public class ResponseBody<T> : Response
    {
        public T Body { get; set; } = default!;
    }
}
