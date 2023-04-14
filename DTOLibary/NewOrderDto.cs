using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOLibary
{
    public class NewOrderDto
    {
        public string SessionKey { get; set; } = "-";
        public List<GetOrderItem> OrderItems { get; set; } = new();
    }
    
}
