using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Order_Code.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Order_Code.Controllers
{
    public class OrdersController : Controller
    {
        private readonly AppDbContext _db;

        // Constructor for ASP.NET Core (dependency injection)
        public OrdersController(AppDbContext db)
        {
            _db = db;
        }

        // Comment out or remove the manual instantiation constructor if using ASP.NET Core
        // public OrdersController()
        // {
        //     _db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
        //         .UseSqlServer("Server=YourServer;Database=OrderDatabase;Trusted_Connection=True;")
        //         .Options);
        // }

        public IActionResult AddOrder(int? orderId)
        {
            Order order = new Order();
            if (orderId.HasValue)
            {
                order = _db.Orders
                    .Include(o => o.OrderDetails)
                    .FirstOrDefault(o => o.OrderID == orderId);
                if (order == null) return NotFound(); // ASP.NET Core method
            }

            ViewBag.AgentID = new SelectList(_db.Agents, "AgentID", "AgentName", order.AgentID);
            ViewBag.ItemID = new SelectList(_db.Items, "ItemID", "ItemName");
            return View(order);
        }

        [HttpPost]
        public IActionResult AddOrder(Order order, int[] ItemID, int[] Quantity)
        {
            if (ModelState.IsValid)
            {
                if (order.OrderID > 0) // Editing an existing order
                {
                    var existingOrder = _db.Orders
                        .Include(o => o.OrderDetails)
                        .FirstOrDefault(o => o.OrderID == order.OrderID);
                    if (existingOrder == null) return NotFound();

                    existingOrder.AgentID = order.AgentID;
                    existingOrder.OrderDate = order.OrderDate ?? DateTime.Now;

                    _db.OrderDetails.RemoveRange(existingOrder.OrderDetails);
                    for (int i = 0; i < ItemID.Length; i++)
                    {
                        _db.OrderDetails.Add(new OrderDetail
                        {
                            OrderID = existingOrder.OrderID,
                            ItemID = ItemID[i],
                            Quantity = Quantity[i]
                        });
                    }
                }
                else // Creating a new order
                {
                    order.OrderDate = DateTime.Now;
                    _db.Orders.Add(order);
                    _db.SaveChanges();

                    for (int i = 0; i < ItemID.Length; i++)
                    {
                        _db.OrderDetails.Add(new OrderDetail
                        {
                            OrderID = order.OrderID,
                            ItemID = ItemID[i],
                            Quantity = Quantity[i]
                        });
                    }
                }
                _db.SaveChanges();
                return RedirectToAction("DisplayOrders");
            }

            ViewBag.AgentID = new SelectList(_db.Agents, "AgentID", "AgentName", order.AgentID);
            ViewBag.ItemID = new SelectList(_db.Items, "ItemID", "ItemName");
            return View(order);
        }

        public IActionResult DisplayOrders()
        {
            var orders = _db.Orders
                .Include(o => o.Agent)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Item)
                .Where(o => o.OrderDate != null)
                .ToList();
            return View(orders);
        }

        [HttpGet]
        public IActionResult Delete(int orderId)
        {
            var order = _db.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.OrderID == orderId);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        public IActionResult Delete(Order order)
        {
            var dbOrder = _db.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.OrderID == order.OrderID);
            if (dbOrder != null)
            {
                _db.OrderDetails.RemoveRange(dbOrder.OrderDetails);
                _db.Orders.Remove(dbOrder);
                _db.SaveChanges();
            }
            return RedirectToAction("DisplayOrders");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}