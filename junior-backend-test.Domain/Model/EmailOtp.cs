namespace junior_backend_test.Domain.Model
{
    public class EmailOtp
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public DateTime ExpiryTime { get; set; }
    }
}
