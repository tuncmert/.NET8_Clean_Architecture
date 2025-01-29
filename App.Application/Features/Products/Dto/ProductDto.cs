namespace App.Application.Features.Products.Dto
{
    public record ProductDto(int Id, String Name, decimal Price, int Stock, int CategoryId);
}
