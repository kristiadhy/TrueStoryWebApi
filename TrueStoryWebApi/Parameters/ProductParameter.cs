namespace TrueStoryWebApi.Parameters;

public class ProductParameter : RequestParameters
{
  public ProductParameter() => OrderBy = "Id";
  public string? SrcByName { get; set; }
}
