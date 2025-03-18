namespace DATN_ACV_DEV.Model_DTO.Color_DTO
{
    public class CreateColorRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }

        public DateTime? CreateDate { get; set; }

        public Guid? CreateBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        public Guid? UpdateBy { get; set; }
    }
}
