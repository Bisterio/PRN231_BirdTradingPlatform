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
                    IsReported = 1,
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
        public ClientOrderViewListDTO GetCurrentStoreOrders(int page, byte status, long currentUserId, string orderIdSearch)
        {
            // Handle query data
            int size = 12;
            page = page == 0 ? 1 : page;

            List<OrderViewDTO?> orderByStore = OrderDAO
                .GetOrdersByCurrentStore(page, size, status, currentUserId, orderIdSearch)
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
        public APIResult<string> CancelOrderDetailCustomer(long orderId, long currentUserId, string cancelReason)
        {
            Order? orderEntity = OrderDAO.GetOrderByIdAndUserId(orderId, currentUserId);
            if (orderEntity == null) return new APIErrorResult<string>("Cannot get this order detail!");

            // Change order status to 7: Cancelled by customer
            if (orderEntity.Status == 1)
            {
                orderEntity.Status = 7;
                orderEntity.UpdatedAt = DateTime.Now;
                orderEntity.CancelReason = cancelReason;
                OrderDAO.UpdateOrder(orderEntity);
            }
            else
            {
                return new APIErrorResult<string>("This order is being processed! You need to request for cancel from the store.");
            }

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


        // Customer send cancel request of order by orderId and userId
        public APIResult<string> RequestCancelOrderDetailCustomer(long orderId, long currentUserId, string cancelReason)
        {
            Order? orderEntity = OrderDAO.GetOrderByIdAndUserId(orderId, currentUserId);
            if (orderEntity == null) return new APIErrorResult<string>("Cannot get this order detail!");

            // Change order status to 5: waiting for cancel approval
            if (orderEntity.Status == 3)
            {
                orderEntity.Status = 5;
                orderEntity.UpdatedAt = DateTime.Now;
                orderEntity.CancelReason = cancelReason;
                OrderDAO.UpdateOrder(orderEntity);
            }
            else
            {
                return new APIErrorResult<string>("This order is being delivered!");
            }

            return new APISuccessResult<string>("Request sent successfully!");
        }
        //Store approve order
        public APIResult<string> ApproveOrderStore(long orderId, long currentUserId)
        {
            Order? orderEntity = OrderDAO.GetOrderByIdAndStoreId(orderId, currentUserId);
            if (orderEntity == null) return new APIErrorResult<string>("Cannot get this order detail!");

            // Change order status to 5: waiting for cancel approval
            if (orderEntity.Status == 1)
            {
                orderEntity.Status = 3;
                orderEntity.UpdatedAt = DateTime.Now;
                OrderDAO.UpdateOrder(orderEntity);
            }
            else
            {
                return new APIErrorResult<string>("This order is being processed!");
            }

            return new APISuccessResult<string>("Order approved!");
        }
        //Store cancel an order by orderId and storeId
        public APIResult<string> CancelOrderDetailStore(long orderId, long currentUserId, string cancelReason)
        {
            Order? orderEntity = OrderDAO.GetOrderByIdAndStoreId(orderId, currentUserId);
            if (orderEntity == null) return new APIErrorResult<string>("Cannot get this order detail!");

            // Change order status to 6: Cancelled by Store
            if (orderEntity.Status == 3 || orderEntity.Status == 1)
            {
                orderEntity.Status = 6;
                orderEntity.UpdatedAt = DateTime.Now;
                orderEntity.CancelReason = cancelReason;
                OrderDAO.UpdateOrder(orderEntity);
            }
            else
            {
                return new APIErrorResult<string>("This order is being delivered!");
            }

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
        //Store approve order cancel request
        public APIResult<string> ApproveOrderCancelRequestStore(long orderId, long currentUserId)
        {
            Order? orderEntity = OrderDAO.GetOrderByIdAndStoreId(orderId, currentUserId);
            if (orderEntity == null) return new APIErrorResult<string>("Cannot get this order detail!");

            // Change order status to 7: Cancelled by Customer
            if (orderEntity.Status == 5)
            {
                orderEntity.Status = 7;
                orderEntity.UpdatedAt = DateTime.Now;
                OrderDAO.UpdateOrder(orderEntity);
            }
            else
            {
                return new APIErrorResult<string>("This order is either being packed or delivered!");
            }

            // Increase stock of product for each order item cancelled
            foreach (OrderItem od in orderEntity.OrderItems)
            {
                Product? updateStockProduct = ProductDAO.GetProductDetailById(od.ProductId);
                if (updateStockProduct == null) return new APIErrorResult<string>("Change units in stock failed!");
                updateStockProduct.Stock += od.Quantity;
                updateStockProduct.UpdatedAt = DateTime.Now;
                ProductDAO.UpdateProduct(updateStockProduct);
            }
            return new APISuccessResult<string>("Request approved!");
        }
        //Store decline order cancel request
        public APIResult<string> DeclineOrderCancelRequestStore(long orderId, long currentUserId)
        {
            Order? orderEntity = OrderDAO.GetOrderByIdAndStoreId(orderId, currentUserId);
            if (orderEntity == null) return new APIErrorResult<string>("Cannot get this order detail!");

            // Change order status to 4: delivering
            if (orderEntity.Status == 5)
            {
                orderEntity.Status = 4;
                orderEntity.UpdatedAt = DateTime.Now;
                OrderDAO.UpdateOrder(orderEntity);
            }
            else
            {
                return new APIErrorResult<string>("This order is either being packed or delivered!");
            }

            return new APISuccessResult<string>("Request declined!");
        }

        // Store deliver the order by orderId
        public APIResult<string> DeliverOrder(long orderId, long currentStoreStaffId)
        {
            Order? orderEntity = OrderDAO.GetOrderByIdAndStoreId(orderId, currentStoreStaffId);
            if (orderEntity == null) return new APIErrorResult<string>("Cannot get this order detail!");

            // Change order status to 4: Delivering
            if (orderEntity.Status == 3)
            {
                orderEntity.Status = 4;
                orderEntity.UpdatedAt = DateTime.Now;
                OrderDAO.UpdateOrder(orderEntity);

                return new APISuccessResult<string>("Order is delivering.");
            }
            else
            {
                return new APIErrorResult<string>("Cannot deliver this order.");
            }

        }

        // Store confirm the order deliverd to user by orderId
        public APIResult<string> ConfirmOrderDelivered(long orderId, long currentStoreStaffId)
        {
            Order? orderEntity = OrderDAO.GetOrderByIdAndStoreId(orderId, currentStoreStaffId);
            if (orderEntity == null) return new APIErrorResult<string>("Cannot get this order detail!");

            // Change order status to 2: Delivered
            if (orderEntity.Status == 4)
            {
                orderEntity.Status = 2;
                orderEntity.UpdatedAt = DateTime.Now;
                orderEntity.RefundDuration = DateTime.Now.AddDays(3);
                OrderDAO.UpdateOrder(orderEntity);

                return new APISuccessResult<string>("Order is successfully delivered.");
            }
            else
            {
                return new APIErrorResult<string>("This order is not delivering.");
            }
        }

        // Customer request an order refund by orderId
        public APIResult<string> RefundRequest(long orderId, long currentUserId, string refundReason)
        {
            Order? orderEntity = OrderDAO.GetOrderByIdAndUserId(orderId, currentUserId);
            if (orderEntity == null) return new APIErrorResult<string>("Cannot get this order detail!");

            // Change order status to 8: Waiting for refund approval
            if (orderEntity.Status == 2)
            {
                if (orderEntity.RefundDuration < DateTime.Now)
                {
                    return new APIErrorResult<string>("This order's refund duration is expired.");
                }

                orderEntity.Status = 8;
                orderEntity.UpdatedAt = DateTime.Now;
                orderEntity.RefundReason = refundReason;
                OrderDAO.UpdateOrder(orderEntity);

                return new APISuccessResult<string>("Request refund successfully.");
            }
            else
            {
                return new APIErrorResult<string>("This order is not delivered.");
            }
        }

        // Store decline the refund request by orderId
        public APIResult<string> RefundDecline(long orderId, long currentStoreStaffId)
        {
            Order? orderEntity = OrderDAO.GetOrderByIdAndStoreId(orderId, currentStoreStaffId);
            if (orderEntity == null) return new APIErrorResult<string>("Cannot get this order detail!");

            // Change order status to 2: Delivered
            if (orderEntity.Status == 8)
            {
                orderEntity.Status = 2;
                orderEntity.UpdatedAt = DateTime.Now;
                orderEntity.RefundDuration = DateTime.Now;
                OrderDAO.UpdateOrder(orderEntity);

                return new APISuccessResult<string>("Refund request is successfully declined.");
            }
            else
            {
                return new APIErrorResult<string>("This order's refund is not requested.");
            }
        }

        // Store accept the refund request by orderId
        public APIResult<string> RefundAccept(long orderId, long currentStoreStaffId)
        {
            Order? orderEntity = OrderDAO.GetOrderByIdAndStoreId(orderId, currentStoreStaffId);
            if (orderEntity == null) return new APIErrorResult<string>("Cannot get this order detail!");

            // Change order status to 9: Refunded
            if (orderEntity.Status == 8)
            {
                orderEntity.Status = 9;
                orderEntity.UpdatedAt = DateTime.Now;
                orderEntity.RefundDuration = DateTime.Now;
                OrderDAO.UpdateOrder(orderEntity);

                // Increase stock of product for each order item refunded
                foreach (OrderItem od in orderEntity.OrderItems)
                {
                    Product? updateStockProduct = ProductDAO.GetProductDetailById(od.ProductId);
                    if (updateStockProduct == null) return new APIErrorResult<string>("Change units in stock failed!");
                    updateStockProduct.Stock += od.Quantity;
                    updateStockProduct.UpdatedAt = DateTime.Now;
                    ProductDAO.UpdateProduct(updateStockProduct);
                }

                return new APISuccessResult<string>("Refund request is successfully accepted.");
            }
            else
            {
                return new APIErrorResult<string>("This order's refund is not requested.");
            }
        }
    }
}
