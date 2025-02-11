using ECommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderAPI.Application.DTOs;
using OrderAPI.Application.DTOs.Conversions;
using OrderAPI.Application.Interfaces;
using OrderAPI.Application.Services;

namespace OrderAPI.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IOrderRepository orderRepository,
        IOrderService orderService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            var orders = await orderRepository.GetAllAsync();
            if (!orders.Any())
                return NotFound("No order detected in the database");
            var (_, list) = OrderConversion.FromEntity(null, orders);
            return list!.Any() ? Ok(list) : NotFound();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            var order = await orderRepository.FindByIdAsync(id);
            if (order == null)
                return NotFound("Order not found");
            var (_order, _) = OrderConversion.FromEntity(order, null);
            return _order == null ? NotFound("Order not found") : Ok(_order);
        }

        [HttpGet("client/{clientId:int}")]
        public async Task<ActionResult<OrderDTO>> GetClientOrders(int clientId)
        {
            if (clientId <= 0)
                return BadRequest("Invalid data provided");
            var orders = await orderService.GetOrdersByClientId(clientId);
            return !orders.Any() ? NotFound() : Ok(orders);
        }

        [HttpGet("details/{orderId:int}")]
        public async Task<ActionResult<OrderDetailsDTO>> GetOrderDetails(int orderId)
        {
            if (orderId <= 0) return BadRequest("Invalid data provided");
            var orderDetails = await orderService.GetOrderDetails(orderId);
            return orderDetails.OrderId > 0 ? Ok(orderDetails) : NotFound("No order found");
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateOrder(OrderDTO order)
        {
            // Check model state if all data annotation are passed.
            if (!ModelState.IsValid)
                return BadRequest("Incomplete data submitted");

            // convert to entity
            var getEntity = OrderConversion.ToEntity(order);
            var response = await orderRepository.CreateAsync(getEntity);
            return response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateOrder(OrderDTO order)
        {
            // Check model state if all data annotation are passed.
            if (!ModelState.IsValid)
                return BadRequest("Incomplete data submitted");

            // convert to entity
            var getEntity = OrderConversion.ToEntity(order);
            var response = await orderRepository.UpdateAsync(getEntity);
            return response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteOrder(OrderDTO order)
        {
            // convert to entity
            var getEntity = OrderConversion.ToEntity(order);
            var response = await orderRepository.DeleteAsync(getEntity);
            return response.Flag ? Ok(response) : BadRequest(response);
        }
    }
}
