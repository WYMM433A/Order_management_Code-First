using System;
using System.Collections.Generic;

namespace Order_Code.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Lock { get; set; }
    }

    public class Item
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string Size { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class Agent
    {
        public int AgentID { get; set; }
        public string AgentName { get; set; }
        public string Address { get; set; }
        public virtual ICollection<Order> Orders { get; set; } // Optional: Track orders by agent

        public Agent()
        {
            Orders = new HashSet<Order>();
        }
    }

    public class Order
    {
        public int OrderID { get; set; }
        public DateTime? OrderDate { get; set; } // Made nullable to match previous usage
        public int AgentID { get; set; }
        public virtual Agent Agent { get; set; }
        public virtual List<OrderDetail> OrderDetails { get; set; }

        public Order()
        {
            OrderDetails = new List<OrderDetail>();
        }
    }

    public class OrderDetail
    {
        public int OrderDetailID { get; set; } // Changed from ID to OrderDetailID for clarity
        public int OrderID { get; set; }
        public int ItemID { get; set; }
        public int? Quantity { get; set; } // Made nullable to handle potential null values
        public virtual Item Item { get; set; }
        public virtual Order Order { get; set; }
    }
}