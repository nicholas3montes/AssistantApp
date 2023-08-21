using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Assistant.Models;

namespace Assistant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet("GetOrdersOfDay")]
        public async Task<IActionResult> GetOrdersOfDay()
        {
            try
            {
                string filePath = "path\\test.json";

                if (System.IO.File.Exists(filePath))
                {
                    string fileContent = await System.IO.File.ReadAllTextAsync(filePath);

                    JArray jsonArray = JArray.Parse(fileContent);
                    List<Order> orders = new List<Order>();

                    foreach (JObject jsonObject in jsonArray)
                    {
                        Order order = new Order
                        {
                            OrderId = int.Parse(jsonObject["pedido"].ToString()),
                            PaymentMethod = jsonObject["forma de pagamento"].ToString(),
                            OrderPrice = decimal.Parse(jsonObject["valor do pedido"].ToString()),
                            WithdrawalMethod = jsonObject["entrega"].ToString(),
                            Items = new List<OrderItem>()
                        };

                        foreach (JObject itemObject in jsonObject["items"])
                        {
                            OrderItem orderItem = new OrderItem
                            {
                                Category = itemObject["categoria do produto"].ToString(),
                                Description = itemObject["item"].ToString(),
                                Quantity = int.Parse(itemObject["quantidade item"].ToString()),
                                Value = decimal.Parse(itemObject["valor item"].ToString())
                            };

                            order.Items.Add(orderItem);
                        }

                        orders.Add(order);
                    }

                    return Ok(orders);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet("GetDailyAnalysis")]
        public async Task<IActionResult> GetDailyAnalysis()
        {
            try
            {
                var ordersResponse = await GetOrdersOfDay();

                if (ordersResponse is OkObjectResult ordersOkResult)
                {
                    var orders = ordersOkResult.Value as List<Order>;
                    decimal totalSales = 0;
                    int QuantityItems =  0;
                    Dictionary<string, int> categoryCounts = new Dictionary<string, int>();
                    Dictionary<string, decimal> categoryTotalSales = new Dictionary<string, decimal>();

                    foreach (var order in orders)
                    {
                        totalSales += order.OrderPrice;

                        foreach (var item in order.Items)
                        {
                            string category = item.Category;
                            int quantity = item.Quantity;
                            QuantityItems += 1;
                            decimal itemValue = item.Value;

                            if (!string.IsNullOrEmpty(category))
                            {
                                if (categoryCounts.ContainsKey(category))
                                {
                                    categoryCounts[category]++;
                                    categoryTotalSales[category] += itemValue * quantity;
                                }
                                else
                                {
                                    categoryCounts[category] = 1;
                                    categoryTotalSales[category] = itemValue * quantity;
                                }
                            }
                        }
                    }

                    var analysis = new
                    {
                        TotalDeVendas = totalSales,
                        QuantidadeDeItems = QuantityItems,
                        CategoryAnalysis = categoryCounts.Select(pair => new
                        {
                            Categoria = pair.Key,
                            Quantidade = pair.Value,
                            Total = categoryTotalSales.GetValueOrDefault(pair.Key, 0)
                        }).ToList()
                    };

                    return Ok(analysis);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}
