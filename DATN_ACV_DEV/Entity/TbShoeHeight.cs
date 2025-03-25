namespace DATN_ACV_DEV.Entity
{
    public class TbShoeHeight
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<TbProductDetail> ProductDetails { get; set; }
    }
}
