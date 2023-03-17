using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eshop_api.Entities;
using eshop_api.Models.DTO.Order;
using Eshop_API.Helpers.Orders;

namespace eshop_api.Services.Orders
{
    public interface IOrderService
    {
        Task<List<Order>> GetListOrders(); //Lấy danh sách đơn hàng
        Task<OrderView> GetOrderById(Guid idOrder); //Lấy đơn hàng theo id
        Task<OrderView> GetCart(string username); // Lấy giỏ hàng của người dùng
        Task<List<Order>> GetOrdersByUserId(int userId); //Lấy đơn hàng theo user id
        Task<List<OrderDto>> GetOrdersByStatus(int status); // Lấy đơn hàng theo trạng thái
        Task<List <Order>> GetOrderByStatusOfEachUser(int userId, int status); // Lấy đơn hàng theo trạng thái của mỗi người dùng
        Task<OrderDto> AddOrder(List<OrderDetailDTO> orderDetailDTOs, int idUser, int idAddress, PaymentMethod payment, int time,string ipAddrV4,string urlOrigin); // Thêm đơn hàng
        Task<Order> UpdateOrder(CreateUpdateOrder createUpdateOrder, Guid idOrder); //Sửa đơn hàng
        Task<List<OrderDto>> ChangeStatus(List<Guid> idOrder, int status, string note); // đổi trạng thái
        Task<bool> DeleteOrderById(Guid idOrder); //Xóa đơn hàng theo id
        Task<bool> UpdateTotal(Guid idOrder); //Cập nhật tổng tiền giỏ hàng
        Task<Order> AddToCart(OrderDetailDTO orderDetailDTOs, string username); //Thêm vào giỏ hàng
        Task<Order> DelFromCart(int idProduct, int idUser); //Xóa khỏi giỏ hàng
        Task<Order> UpdateCart(List<OrderDetailDTO> orderDetailDTOs, int idUser); //Cập nhật giỏ hàng
        Task<OrderDto> CancelOrder(Guid idOrder, string note); //Hủy đơn hàng

    }
}