namespace DATN_ACV_DEV.Entity
{
    public class TbTechnology
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<TbProductDetail> ProductDetails { get; set; }
    }
}
