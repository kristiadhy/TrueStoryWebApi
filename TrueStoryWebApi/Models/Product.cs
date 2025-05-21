using System.ComponentModel.DataAnnotations;

namespace TrueStoryWebApi.Models;

public class Product
{
  public int Id { get; set; }
  [Required(ErrorMessage = "Product name is required.")]
  public string Name { get; set; } = default!;
  public Dictionary<string, object> Data { get; set; } = default!;

  public Product()
  { }

  public Product(string name, Dictionary<string, object> data)
  {
    Name = name;
    Data = data;
  }
}
