using BusinessObject.DTOs;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public static class Mapper
    {
        // Map Store Entity to ClientStoreDetailViewDTO
        public static ClientStoreDetailViewDTO? ToClientStoreDetailViewDTO(Store? entity)
        {
            if (entity == null) return null;

            return new ClientStoreDetailViewDTO()
            {
                StoreId = entity.StoreId,
                Name = entity.Name,
                Address = entity.Address,
                Description = entity.Description,
                LogoImage = entity.LogoImage,
                CoverImage = entity.CoverImage,
                Email = entity.User?.Email,
                Phone = entity.User?.Phone,
            };
        }

        // Map ProductEntity to ProductViewDTO
        public static ProductViewDTO? ToProductViewDTO(Product? entity)
        {
            if (entity == null) return null;

            return new ProductViewDTO()
            {
                ProductId = entity.ProductId,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                Description = entity.Description,
                Image = entity.Image,
                Name = entity.Name,
                Status = entity.Status,
                Stock = entity.Stock,
                UnitPrice = entity.UnitPrice,
                CategoryName = entity.Category?.Name,
                StoreName = entity.Store?.Name,
                StoreAddress = entity.Store?.Address,
                StoreDescription = entity.Store?.Description,
                StoreCoverImage = entity.Store?.CoverImage,
                StoreLogoImage = entity.Store?.LogoImage,
                StoreId = entity.StoreId
            };
        }

        // Map ProductEntity to ProductCreateDTO
        public static ProductCreateDTO? ToProductCreateDTO(Product? entity)
        {
            if (entity == null) return null;

            return new ProductCreateDTO()
            {
                Name = entity.Name,
                CategoryId = entity.CategoryId,
                Description = entity.Description,
                Image = entity.Image,
                ProductId = entity.ProductId,
                Stock = entity.Stock,
                UnitPrice = entity.UnitPrice
            };
        }

        // Map Order Entity to OrderViewDTO
        public static OrderViewDTO? ToOrderViewDTO(Order? entity)
        {
            if (entity == null) return null;

            return new OrderViewDTO()
            {
                OrderId = entity.OrderId,
                Address = entity.Invoice.Address,
                CreatedAt = entity.CreatedAt,
                Email = entity.Invoice.Email,
                Name = entity.Invoice.Name,
                PaymentMethod = entity.Invoice.PaymentMethod,
                Phone = entity.Invoice.Phone,
                Status = entity.Status,
                TotalAmount = entity.TotalAmount,
                TotalAmountPreShipping = entity.TotalAmountPreShipping,
                TotalItem = entity.TotalItem,
                TotalShippingCost = entity.TotalShippingCost,
                UpdatedAt = entity.UpdatedAt,
                StoreId = entity.StoreId,
                StoreName = entity.Store?.Name,
                StoreAddress = entity.Store?.Address,
                CancelReason = entity.CancelReason,
                IsReported = entity.IsReported,
                RefundDuration = entity.RefundDuration,
                RefundReason = entity.RefundReason,
                ReportedReason = entity.ReportedReason,
                DeliveredAt = entity.DeliveredAt,
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

        // Map Invoice Entity to InvoiceViewDTO
        public static InvoiceViewDTO? ToInvoiceViewDTO(Invoice? entity)
        {
            if (entity == null) return null;

            return new InvoiceViewDTO()
            {
                IsPaid = 1,
                InvoiceId = entity.InvoiceId,
                Email = entity.Email,
                CreatedAt = entity.CreatedAt,
                Address = entity.Address,
                Name = entity.Name,
                Note = entity.Note,
                PaymentMethod = entity.PaymentMethod,
                Phone = entity.Phone,
                TotalAmount = entity.TotalAmount,
                TotalAmountPreShipping = entity.TotalAmountPreShipping,
                TotalItem = entity.TotalItem,
                TotalShippingCost = entity.TotalShippingCost,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }
}
