using System.Drawing;

namespace DATN_ACV_DEV.Entity
{
    public class TbProductDetail
    {
        public Guid Id { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
        public Guid? ImageId { get; set; }
        public Guid? ColorId { get; set; }
        public Guid? SizeId { get; set; }
        public Guid? ShoeHeightId { get; set; }
        public Guid? TechnologyId { get; set; }
        public Guid? MaterialId { get; set; }
        public Guid ProductID { get; set; }

        public virtual TbImage? Image { get; set; }
        public virtual TbColor? Color { get; set; }
        public virtual TbSize? Size { get; set; }
        public virtual TbShoeHeight? ShoeHeight { get; set; }
        public virtual TbTechnology? Technology { get; set; }
        public virtual TbMaterial? Material { get; set; }
        public virtual TbProduct Product { get; set; }
    }
}
