using App.Application.Contracts.Caching;
using App.Application.Contracts.Persistence;
using App.Application.Contracts.ServiceBus;
using App.Application.Features.Products.Create;
using App.Application.Features.Products.Dto;
using App.Application.Features.Products.Update;
using App.Application.Features.Products.UpdateStock;
using App.Domain.Entities;
using App.Domain.Events;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Features.Products
{
    public class ProductService(IProductRepository productRepository, IUnityOfWork unityOfWork, IMapper mapper, ICacheService cacheService,IServiceBus serviceBus) : IProductService
    {
        private const string productListCacheKey = "ProductListCacheKey";

        public async Task<ServiceResult<CreateProductResponse>> CreateAsync(CreateProductRequest request)
        {
            var anyProduct = await productRepository.AnyAsync(x => x.Name == request.Name);
            if (anyProduct)
            {
                return ServiceResult<CreateProductResponse>.Fail("Product already exists.");
            }
            var newProduct = mapper.Map<Product>(request);
            await productRepository.AddAsync(newProduct);
            await unityOfWork.SaveChangeAsync();

            await serviceBus.PublishAsync(new ProductAddedEvent(newProduct.Id, newProduct.Name, newProduct.Price));


            return ServiceResult<CreateProductResponse>.SuccessAsCreated(new CreateProductResponse(newProduct.Id), $"api/products/{newProduct.Id}");
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var product = await productRepository.GetByIdAsync(id);
            productRepository.Delete(product!);
            await unityOfWork.SaveChangeAsync();
            return ServiceResult.Success(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResult<List<ProductDto>>> GetAllListAsync()
        {
            //cache aside desing pattern
            //1. cache control
            //2. from db
            //3. caching data

            var productListAsCached = await cacheService.GetAsync<List<ProductDto>>(productListCacheKey);
            if (productListAsCached is not null) return ServiceResult<List<ProductDto>>.Success(productListAsCached);


            var products = await productRepository.GetAllAsync();

            var productAsDto = mapper.Map<List<ProductDto>>(products);
            await cacheService.AddAsync(productListCacheKey, productAsDto,TimeSpan.FromMinutes(1));
            return ServiceResult<List<ProductDto>>.Success(productAsDto);
        }

        public async Task<ServiceResult<ProductDto?>> GetByIdAsync(int id)
        {
            var product = await productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return ServiceResult<ProductDto?>.Fail("Product not found", HttpStatusCode.NotFound);
            }
            var productAsDto = mapper.Map<ProductDto>(product);
            return ServiceResult<ProductDto>.Success(productAsDto)!;
        }

        public async Task<ServiceResult<List<ProductDto>>> GetPagedAllListAsync(int pageNumber, int pageSize)
        {
            //int skip = (pageNumber - 1) * pageSize;
            var products = await productRepository.GetAllPagedAsync(pageNumber, pageSize);
            var productAsDto = mapper.Map<List<ProductDto>>(products);
            return ServiceResult<List<ProductDto>>.Success(productAsDto);
        }

        public async Task<ServiceResult<List<ProductDto>>> GetTopPriceProductsAsync(int count)
        {
            var products = await productRepository.GetTopPriceProductAsnyc(count);

            var productAsDto = mapper.Map<List<ProductDto>>(products);

            return new ServiceResult<List<ProductDto>>()
            {
                Data = productAsDto
            };
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateProductRequest request)
        {
            var isProductNameExist = await productRepository.AnyAsync(x => x.Name == request.Name && x.Id != id);
            if (isProductNameExist)
            {
                return ServiceResult.Fail("Product already exists.");
            }

            var product = mapper.Map<Product>(request);
            product.Id = id;

            productRepository.Update(product!);
            await unityOfWork.SaveChangeAsync();
            return ServiceResult.Success(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResult> UpdateStockAsync(UpdateProductStockRequest request)
        {
            var product = await productRepository.GetByIdAsync(request.ProductId);
            if (product == null)
            {
                return ServiceResult.Fail("Product not found", HttpStatusCode.NotFound);
            }
            product.Stock = request.Quantity;
            productRepository.Update(product);
            await unityOfWork.SaveChangeAsync();
            return ServiceResult.Success(HttpStatusCode.NoContent);
        }
    }
}
