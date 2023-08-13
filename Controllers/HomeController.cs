using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

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
                string filePath = "C:\\Users\\nicholas\\source\\repos\\Assistant\\path\\test.json"; // Caminho para o arquivo JSON

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
    }
}
