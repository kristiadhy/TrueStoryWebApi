using Microsoft.AspNetCore.Mvc;
using TrueStoryWebApi.Models;

namespace TrueStoryWebApi.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class ProductController : ControllerBase
  {
    private readonly ILogger<ProductController> _logger;
    private readonly HttpClient _httpClient;

    public ProductController(ILogger<ProductController> logger, HttpClient httpClient)
    {
      _logger = logger;
      _httpClient = httpClient;
      _httpClient.BaseAddress = new Uri("https://api.restful-api.dev/");
    }

    [HttpGet(Name = "GetProducts")]
    public async Task<IActionResult> GetProducts(string name = "", int page = 1, int pageSize = 10)
    {
      var apiResponse = await _httpClient.GetAsync("objects");
      apiResponse.EnsureSuccessStatusCode();
      var content = await apiResponse.Content.ReadFromJsonAsync<List<Product>>();

      if (content == null || content.Count == 0)
      {
        return Ok(new List<Product>());
      }

      var filtered = content
          .Where(u => u.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
          .Skip((page - 1) * pageSize)
          .Take(pageSize)
          .ToList();

      return Ok(filtered);
    }

    [HttpGet("{id:int}", Name = "GetProductById")]
    public async Task<IActionResult> GetProductById(int id)
    {
      var apiResponse = await _httpClient.GetFromJsonAsync<Product>($"objects/{id}");
      return Ok(apiResponse);
    }


    [HttpPost(Name = "CreateProduct")]
    public async Task<IActionResult> CreateProduct([FromBody] Product product)
    {
      var apiResponse = await _httpClient.PostAsJsonAsync("objects", product);
      apiResponse.EnsureSuccessStatusCode();
      return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}", Name = "UpdateProduct")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
    {
      var apiResponse = await _httpClient.PutAsJsonAsync($"objects/{id}", product);
      apiResponse.EnsureSuccessStatusCode();
      var updatedContent = await apiResponse.Content.ReadFromJsonAsync<object>();
      return Ok(updatedContent);
    }

    [HttpDelete("{id:int}", Name = "DeleteProduct")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
      var apiResponse = await _httpClient.DeleteAsync($"objects/{id}");
      apiResponse.EnsureSuccessStatusCode();
      return Ok(new
      {
        message = $"Object with id = {id}, has been deleted."
      });
    }
  }
}
