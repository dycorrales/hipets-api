namespace HiPets.Domain.Helpers
{
    public class Response
    {
        public static Response Ok = new Response { Success = true };
        public static Response Fail = new Response { Success = false };

        public Response(bool success = false)
        {
            Success = success;
        }

        public bool Success { get; private set; }
    }
}
