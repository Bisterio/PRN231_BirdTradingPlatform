using BusinessObject.Common;
using BusinessObject.DTOs;
using BusinessObject.Models;
using DataAccess;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Implementation
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

            // Create a new invoice (big order containing sub-orders)
            Invoice newInvoice = new Invoice()
            {
                Address = newOrders.Address,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Email = newOrders.Email,
                IsPaid = 1,
                Name = newOrders.Name,
                Note = newOrders.Note,
                PaymentMethod = newOrders.PaymentMethod,
                Phone = newOrders.Phone,
                TotalItem = newOrders.CartItems.Sum(x => x.Quantity),
                TotalAmountPreShipping = newOrders.CartItems.Sum(x => x.Quantity * x.UnitPrice),
                TotalShippingCost = newOrders.TotalShippingCost,
                TotalAmount = newOrders.TotalShippingCost + newOrders.CartItems.Sum(x => x.Quantity * x.UnitPrice),
                UserId = currentUserId
            };
            // Save invoice to database
            InvoiceDAO.CreateInvoice(newInvoice);

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
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Status = 1,
                    TotalItem = totalItem,
                    TotalAmountPreShipping = totalAmountPreShipping,
                    TotalShippingCost = totalShippingCost,
                    TotalAmount = totalAmount,
                    StoreId = storeOrder.StoreId,
                    InvoiceId = newInvoice.InvoiceId
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

            return new APISuccessResult<string>(newInvoice.InvoiceId.ToString());
        }

        // Get orders of a logined customer
        public ClientOrderViewListDTO GetCurrentUserOrders(int page, byte status, long currentUserId)
        {
            // Handle query data
            int size = 12;
            page = page == 0 ? 1 : page;

            List<OrderViewDTO?> orderByUser = OrderDAO
                .GetOrdersByCurrentUser(page, size, status, currentUserId)
                .Select(x => Mapper.ToOrderViewDTO(x))
                .ToList();

            // Get count of orders by search/filter
            int orderCount = OrderDAO.CountOrdersByCurrentUser(status, currentUserId);
            int totalPages = (int)Math.Ceiling((double)orderCount / size);
            List<int> pageNumbers = new List<int>();
            if (totalPages > 0)
            {
                int start = Math.Max(1, page - 2);
                int end = Math.Min(page + 2, totalPages);

                if (totalPages > 5)
                {
                    if (end == totalPages) start = end - 4;
                    else if (start == 1) end = start + 4;
                }
                else
                {
                    start = 1;
                    end = totalPages;
                }
                pageNumbers = Enumerable.Range(start, end - start + 1).ToList();
            }

            return new ClientOrderViewListDTO()
            {
                OrdersPaginated = orderByUser,
                Page = page,
                Size = size,
                PageNumbers = pageNumbers,
                TotalCount = orderCount,
                TotalPage = totalPages
            };
        }

        // Get orders of a logined store
        public ClientOrderViewListDTO GetCurrentStoreOrders(int page, byte status, long currentUserId, long orderId)
        {
            // Handle query data
            int size = 12;
            page = page == 0 ? 1 : page;

            List<OrderViewDTO?> orderByStore = OrderDAO
                .GetOrdersByCurrentStore(page, size, status, currentUserId, orderId)
                .Select(x => Mapper.ToOrderViewDTO(x))
                .ToList();

            // Get count of orders by search/filter
            int orderCount = OrderDAO.CountOrdersByCurrentStore(status, currentUserId);
            int totalPages = (int)Math.Ceiling((double)orderCount / size);
            List<int> pageNumbers = new List<int>();
            if (totalPages > 0)
            {
                int start = Math.Max(1, page - 2);
                int end = Math.Min(page + 2, totalPages);

                if (totalPages > 5)
                {
                    if (end == totalPages) start = end - 4;
                    else if (start == 1) end = start + 4;
                }
                else
                {
                    start = 1;
                    end = totalPages;
                }
                pageNumbers = Enumerable.Range(start, end - start + 1).ToList();
            }

            return new ClientOrderViewListDTO()
            {
                OrdersPaginated = orderByStore,
                Page = page,
                Size = size,
                PageNumbers = pageNumbers,
                TotalCount = orderCount,
                TotalPage = totalPages
            };
        }
        // Get order detail and order items of a logined customer
        public OrderViewDTO? GetOrderDetailCustomer(long orderId, long currentUserId)
        {
            Order? orderEntity = OrderDAO.GetOrderByIdAndUserId(orderId, currentUserId);
            if (orderEntity == null) return null;

            OrderViewDTO? orderDTO = Mapper.ToOrderViewDTO(orderEntity);
            List<OrderItemViewDTO?> orderItemDTO = orderEntity.OrderItems
                .Select(x => Mapper.ToOrderItemViewDTO(x))
                .ToList();

            orderDTO.OrderItems = orderItemDTO;

            return orderDTO;
        }

        // Get order detail and order items of a logined customer
        public OrderViewDTO? GetOrderDetailStore(long orderId, long currentUserId)
        {
            Order? orderEntity = OrderDAO.GetOrderByIdAndStoreId(orderId, currentUserId);
            if (orderEntity == null) return null;

            OrderViewDTO? orderDTO = Mapper.ToOrderViewDTO(orderEntity);
            List<OrderItemViewDTO?> orderItemDTO = orderEntity.OrderItems
                .Select(x => Mapper.ToOrderItemViewDTO(x))
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
            foreach (OrderItem od in orderEntity.OrderItems)
            {
                Product? updateStockProduct = ProductDAO.GetProductDetailById(od.ProductId);
                if (updateStockProduct == null) return new APIErrorResult<string>("Change units in stock failed!");
                updateStockProduct.Stock += od.Quantity;
                updateStockProduct.UpdatedAt = DateTime.Now;
                ProductDAO.UpdateProduct(updateStockProduct);
            }

            return new APISuccessResult<string>("Cancel order successfully!");
        }
    }
}
