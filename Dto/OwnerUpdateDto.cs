namespace TodoWebApi.Dto
{
    public class OwnerUpdateDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
