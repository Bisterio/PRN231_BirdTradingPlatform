using BusinessObject.Common;
using BusinessObject.DTOs;
using BusinessObject.Models;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class OrderRepository : IOrderRepository
    {

        // Function to create new orders
        public APIResult<string> CreateNewOrders(OrderCreateDTO newOrders, long currentUserId)
        {
            // Check for valid product (product status == 1, quantity does not exceed stock)
            foreach (CartItemDTO cartItem in newOrders.CartItems)
            {
                // Get each product and check for status and stock
                Product? p = ProductDAO.GetProductDetailById(cartItem.ProductId);
                if (p == null || p.Status == 0)
                    return new APIErrorResult<string>("Your cart contain one or more item that is unavailable. Please update your cart!");
                if (cartItem.Quantity <= 0)
                    return new APIErrorResult<string>($"Quantity for product '{p.Name}' cannot be lower than 0. Please update the quantity of this product!");
                if (cartItem.Quantity > p.Stock)
                    return new APIErrorResult<string>($"Quantity for product '{p.Name}' has exceeded the current units in stock for this product. Please update the quantity of this product!");
            }

            // Separate cart items by storeid
            var cartItemsGroupedByStore =
                newOrders.CartItems.GroupBy(c => c.StoreId)
                .Select(g => new
                {
                    StoreId = g.Key,
                    CartItems = g.Select(cartItem => new
                    {
                        cartItem.ProductId,
                        cartItem.Quantity,
                        cartItem.ShippingCost,
                        cartItem.UnitPrice
                    })
                });

            foreach (var storeOrder in cartItemsGroupedByStore.OrderBy(g => g.StoreId))
            {
                int totalItem = storeOrder.CartItems.Sum(x => x.Quantity);
                decimal totalAmountPreShipping = storeOrder.CartItems.Sum(x => x.Quantity * x.UnitPrice);
                decimal totalShippingCost = storeOrder.CartItems.First().ShippingCost;
                decimal totalAmount = totalAmountPreShipping + totalShippingCost;

                // Create order for each store
                Order newOrder = new Order()
                {
                    Address = newOrders.Address,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Email = newOrders.Email,
                    Name = newOrders.Name,
                    PaymentMethod = newOrders.PaymentMethod,
                    Phone = newOrders.Phone,
                    Status = 1,
                    TotalItem = totalItem,
                    TotalAmountPreShipping = totalAmountPreShipping,
                    TotalShippingCost = totalShippingCost,
                    TotalAmount = totalAmount,
                    StoreId = storeOrder.StoreId,
                    UserId = currentUserId
                };

                // Save order
                OrderDAO.CreateOrder(newOrder);

                // Add order items
                foreach (var cartItem in storeOrder.CartItems)
                {
                    OrderItemDAO.CreateOrderItem(new OrderItem()
                    {
                        OrderId = newOrder.OrderId,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        Price = cartItem.UnitPrice,
                        Total = cartItem.Quantity * cartItem.UnitPrice,
                    });
                    // Change product stock
                    Product? updateStockProduct = ProductDAO.GetProductDetailById(cartItem.ProductId);
                    if (updateStockProduct == null) return new APIErrorResult<string>("Change units in stock failed!");
                    updateStockProduct.Stock -= cartItem.Quantity;
                    updateStockProduct.UpdatedAt = DateTime.Now;
                    ProductDAO.UpdateProduct(updateStockProduct);
                }
            }

            return new APISuccessResult<string>("Add new order successfully!");
        }

        // Get orders of a logined customer
        public List<OrderViewDTO?> GetCurrentUserOrders(byte status, long currentUserId)
        {
            List<OrderViewDTO?> orderByUser = OrderDAO
                .GetOrdersByCurrentUser(status, currentUserId)
                .Select(x => ToOrderViewDTO(x))
                .ToList();

            return orderByUser;
        }

        // Get order detail and order items of a logined customer
        public OrderViewDTO? GetOrderDetailCustomer(long orderId, long currentUserId)
        {
            Order? orderEntity = OrderDAO.GetOrderByIdAndUserId(orderId, currentUserId);
            if(orderEntity == null) return null;

            OrderViewDTO? orderDTO = ToOrderViewDTO(orderEntity);
            List<OrderItemViewDTO?> orderItemDTO = orderEntity.OrderItems
                .Select(x => ToOrderItemViewDTO(x))
                .ToList();

            orderDTO.OrderItems = orderItemDTO;

            return orderDTO;
        }

        // Customer cancel an order by orderId and userId
        public APIResult<string> CancelOrderDetailCustomer(long orderId, long currentUserId)
        {
            Order? orderEntity = OrderDAO.GetOrderByIdAndUserId(orderId, currentUserId);
            if (orderEntity == null) return new APIErrorResult<string>("Cannot get this order detail!");

            // Change order status to 0: Cancelled
            orderEntity.Status = 0;
            OrderDAO.UpdateOrder(orderEntity);

            // Increase stock of product for each order item cancelled
            foreach(OrderItem od in orderEntity.OrderItems)
            {
                Product? updateStockProduct = ProductDAO.GetProductDetailById(od.ProductId);
                if (updateStockProduct == null) return new APIErrorResult<string>("Change units in stock failed!");
                updateStockProduct.Stock += od.Quantity;
                updateStockProduct.UpdatedAt = DateTime.Now;
                ProductDAO.UpdateProduct(updateStockProduct);
            }

            return new APISuccessResult<string>("Cancel order successfully!");
        }

        // Map Order Entity to OrderViewDTO
        public static OrderViewDTO? ToOrderViewDTO(Order? entity)
        {
            if (entity == null) return null;

            return new OrderViewDTO()
            {
                OrderId = entity.OrderId,
                Address = entity.Address,
                CreatedAt = entity.CreatedAt,
                Email = entity.Email,
                Name = entity.Name,
                PaymentMethod = entity.PaymentMethod,
                Phone = entity.Phone,
                Status = entity.Status,
                TotalAmount = entity.TotalAmount,
                TotalAmountPreShipping = entity.TotalAmountPreShipping,
                TotalItem = entity.TotalItem,
                TotalShippingCost = entity.TotalShippingCost,
                UpdatedAt = entity.UpdatedAt,
                StoreName = entity.Store?.Name,
                StoreAddress = entity.Store?.Address
            };
        }

        // Map Order Item Entity to OrderItemViewDTO
        public static OrderItemViewDTO? ToOrderItemViewDTO(OrderItem? entity)
        {
            if (entity == null) return null;

            return new OrderItemViewDTO()
            {
                ProductId = entity.ProductId,
                Description = entity.Product.Description,
                Image = entity.Product.Image,
                Name = entity.Product.Name,
                UnitPrice = entity.Price,
                CategoryName = entity.Product.Category?.Name,
                Quantity = entity.Quantity,
                Total = entity.Total
            };
        }
    }
}
