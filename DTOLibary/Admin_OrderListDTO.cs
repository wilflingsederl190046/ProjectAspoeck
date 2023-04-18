using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOLibary
{
    public class Admin_OrderListDTO
    {
        public int OrderNumber { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string State { get; set; }
        public double Price { get; set; }
    }
}
