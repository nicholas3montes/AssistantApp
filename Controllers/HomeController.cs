using Microsoft.AspNetCore.Mvc;
using System.Text;
using Newtonsoft.Json.Linq;
using Microsoft.VisualBasic;
using System.Globalization;

namespace Assistant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet("GetFileContent")]
        public async Task<IActionResult> GetFileContent()
        {
            try
            {
                string filePath = "path\\test.json";

                if (System.IO.File.Exists(filePath))
                {
                    string fileContent = await System.IO.File.ReadAllTextAsync(filePath);
                    return Content(fileContent, "application/json");
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
        [HttpGet("GetResultOfDay")]
        public async Task<IActionResult> GetResultOfDay()
        {
            try
            {
                string filePath = "path\\test.json";

                if (System.IO.File.Exists(filePath))
                {
                    string fileContent = await System.IO.File.ReadAllTextAsync(filePath);
                    var jsonArray = Newtonsoft.Json.JsonConvert.DeserializeObject<JArray>(fileContent);
                    decimal total_day = 0;
                    int total_orders_day = 0;
                    var categoryInfo = new Dictionary<string, (int Count, decimal TotalValue)>();

                    var paragraphs = new StringBuilder();

                    if (jsonArray != null)
                    {
                        foreach (var item in jsonArray)
                        {
                            total_orders_day += 1;
                            var jsonObject = (JObject)item;

                            foreach (var property in jsonObject.Properties())
                            {
                                if (property.Name == "valor do pedido")
                                {
                                    if (decimal.TryParse(property.Value.ToString(), out decimal parsedValue))
                                    {
                                        total_day += parsedValue;
                                    }
                                }
                                else if (property.Name == "items")
                                {
                                    var itemsArray = (JArray)property.Value;

                                    foreach (var itemObj in itemsArray)
                                    {
                                        decimal item_value = 0;
                                        var itemProperties = (JObject)itemObj;

                                        foreach (var itemProperty in itemProperties.Properties())
                                        {
                                            if (itemProperty.Name == "categoria do produto")
                                            {
                                                string category = itemProperty.Value.ToString();
                                                if (categoryInfo.ContainsKey(category))
                                                {
                                                    categoryInfo[category] = (
                                                        Count: categoryInfo[category].Count + 1,
                                                        TotalValue: categoryInfo[category].TotalValue + item_value
                                                    );
                                                }
                                                else
                                                {
                                                    categoryInfo[category] = (Count: 1, TotalValue: item_value);
                                                }
                                            }
                                            else if (itemProperty.Name == "quantidade item" || itemProperty.Name == "valor do pedido")
                                            {
                                                // Obter o valor do item e adicioná-lo ao item_value
                                                if (decimal.TryParse(itemProperty.Value.ToString(), out decimal itemPropertyValue))
                                                {
                                                    item_value += itemPropertyValue;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        foreach (var kvp in categoryInfo)
                        {
                            paragraphs.AppendLine($"Categoria: {kvp.Key}, Quantidade: {kvp.Value.Count}, Valor total: {kvp.Value.TotalValue.ToString("C")}");
                        }

                        paragraphs.AppendLine($"Realizados hoje um total de: {total_orders_day} pedidos com um valor total de: {total_day.ToString("C")}");
                    }

                return Content(paragraphs.ToString(), "text/plain");
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
