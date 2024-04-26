namespace Rabbit.Models
{
    public class Response
    {
        public string Status { get; set; } = null!;

        public string Message { get; set; } = "";

        public Response() { }

        public Response(string status, string message)
        {
            Status = status;
            Message = message;
        }
    }
}
