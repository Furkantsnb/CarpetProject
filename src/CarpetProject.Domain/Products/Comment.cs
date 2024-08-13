using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace CarpetProject.Products
{
    public class Comment : FullAuditedEntity<int>
    {
        public int ProductId { get; set; } // Ürüne referans (Hangi ürüne yorum yapıldığı)
        public int? ParentCommentId { get; set; } // Üst yoruma referans (Eğer bu bir yanıtsa)
        public string UserName { get; set; } // Yorumu yapan kullanıcının adı
        public string Email { get; set; } // Yorumu yapan kullanıcının e-posta adresi
        public string Content { get; set; } // Yorumun içeriği
        public int Rating { get; set; } // 5 üzerinden puanlama
        public DateTime CommentDate { get; set; } // Yorumun yapıldığı tarih
        public bool IsApproved { get; set; } // Yorumun onaylanıp onaylanmadığını belirten özellik
        public bool IsSpam { get; set; } // Yorumun spam olarak işaretlenip işaretlenmediğini belirten özellik

        public virtual Product Product { get; set; } // Ürüne referans
        public virtual Comment ParentComment { get; set; } // Üst yoruma referans
        public virtual ICollection<Comment> Replies { get; set; } // Bu yoruma yapılan yanıtların listesi

        public Comment()
        {
            Replies = new HashSet<Comment>();
        }
    }
}
