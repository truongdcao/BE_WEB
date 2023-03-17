using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eshop_api.Entities;
using eshop_api.Models.DTO.Order;
namespace eshop_api.Services.Orders
{
    public interface IOderDetailService
    {
        Task<List<OrderDetail>> GetOrderDetailsById(int idOrderDetail); //Lấy chi tiết đơn hàng theo id
        Task<List<OrderDetail>> GetOrderDetailsByOrderId(Guid idOrder); //Lấy chi tiết đơn hàng theo id đơn hàng
        Task<List<OrderDetailDTOs>> GetOrderDetailByOrderId(Guid idOrder); //Lấy chi tiết đơn hàng theo id đơn hàng
        Task<OrderDetail> AddOrderDetail(CreateUpdateOrderDetail createUpdateOrderDetail);
        Task<OrderDetail> AddOrderDetail(OrderDetailDTO orderDetailDTO, Guid idOrder);// Thêm chi tiết đơn hàng
        Task<OrderDetail> UpdateOrderDetail(CreateUpdateOrderDetail createUpdateOrderDetail, int idOrderDetail); //Sửa chi tiết đơn hàng
        Task<bool> DeleteOrderDetail (int idOrderDetail); //Xóa chi tiết đơn hàng
    }
}