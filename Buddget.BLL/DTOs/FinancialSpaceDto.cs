using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buddget.BLL.DTOs
{
    public class FinancialSpaceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageName { get; set; }
        public byte[] ImageData { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
