namespace DMSMicroservice.AuthService.DTOs
{
    public class ResponseDto
    {
        public string Error { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }

        public string Role { get; set; }
    }
}
