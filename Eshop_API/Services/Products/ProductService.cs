using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eshop_api.Models.DTO.Products;
using eshop_api.Entities;
using eshop_api.Helpers;
using eshop_api.Services.Images;
using eshop_api.Helpers.Mapper;
using eshop_pbl6.Helpers.Products;
using System.Text.Json;
using Eshop_API.Repositories.Images;
using Eshop_API.Repositories.Products;

namespace eshop_api.Service.Products
{
    public class ProductService : IProductService
    {
        private readonly IImageService _imageService;
        private readonly IImageRepository _imageRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        public ProductService(IImageRepository imageRepository, 
                            IImageService imageService,
                            IProductRepository productRepository,
                            ICategoryRepository categoryRepository)
        {
            _imageService = imageService;
            _imageRepository = imageRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;  
        }

        public async Task<ProductDto> AddProduct(CreateUpdateProductDto createProductDto)
        {
            Product product = new Product();
            product.Status = Status.Pending;
            product.Name = createProductDto.Name;
            product.Keyword = createProductDto.Keyword;
            product.AvtImageUrl = CloudImage.UploadImage(createProductDto.AvtImage);
            product.Price = createProductDto.Price;
            product.Discount = createProductDto.Discount;
            product.ExportQuantity = 0;
            product.ImportQuantity = createProductDto.ImportQuantity;
            product.Weight = createProductDto.Weight;
            product.Description = createProductDto.Description;
            product.Color = createProductDto.Color;
            product.IsDelete = false;
            product.DetailProduct = createProductDto.Detail;
            product.CategoryId = createProductDto.IdCategory;
            var resultProduct = await _productRepository.Add(product);
            await _productRepository.SaveChangesAsync();
            ProductDto productDto = ProductMapper.toProductDto(resultProduct);

            if (createProductDto.ProductImages.Count > 0)
                foreach (IFormFile item in createProductDto.ProductImages)
                {
                    var img = await _imageService.AddImage(item, resultProduct.Id);
                    productDto.ImageUrl.Add(img.Url);
                }
            return productDto;
        }

        public async Task<bool> DeleteProductById(int id)
        {
            var product = await _productRepository.FirstOrDefault(x => x.Id == id);
            if (product != null)
            {
                await _productRepository.Remove(product);
                await _productRepository.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<ProductDto>> GetListProduct(int sortOrder)
        {
            var products = await _productRepository.GetAll();
            var imgaes = await _imageRepository.GetAll();
            if(sortOrder!=0)
            {
                switch(sortOrder)
                {
                    case 1:
                        products = products.OrderByDescending(x => x.Id).ToList();
                        break;
                    case 2:
                        products = products.OrderByDescending(x => x.ExportQuantity).ToList();
                        break;
                    case 3:
                        products = products.OrderBy(x => x.Price).ToList();
                        break;
                }
            }
            List<ProductDto> productDtos = new List<ProductDto>();
            foreach (var item in products)
            {
                ProductDto productDto = ProductMapper.toProductDto(item);
                productDto.ImageUrl = imgaes.Where(x => x.ProductID == item.Id).Select(x => x.Url).ToList();
                productDtos.Add(productDto);
            }
            return await Task.FromResult(productDtos);
        }

        public async Task<List<ProductDto>> GetProductsByIdCategory(int idCategory, int sortOrder)
        {
            List<Product> products;
            if(idCategory == 0) {
                products = await _productRepository.GetAll();
            }
            else{
                products = await _productRepository.Find(x => x.CategoryId == idCategory);
            }
            
            if(sortOrder!=0)
            {
                switch(sortOrder)
                {
                    case 1:
                        products = products.OrderByDescending(x => x.Id).ToList();
                        break;
                    case 2:
                        products = products.OrderBy(x => x.Id).ToList();
                        break;
                    case 3:
                        products = products.OrderBy(x => x.Price).ToList();
                        break;
                    case 4:
                        products = products.OrderByDescending(x => x.Price).ToList();
                        break;
                }
            }
            if (products != null)
            {
                List<ProductDto> productDtos = new List<ProductDto>();
                foreach (var item in products)
                {
                    ProductDto productDto = ProductMapper.toProductDto(item);
                    productDto.ImageUrl = (await _imageRepository.Find(x => x.ProductID == item.Id)).Select(x => x.Url).ToList();
                    productDtos.Add(productDto);
                }
                return await Task.FromResult(productDtos);
            }
            throw null;
        }
        public async Task<List<ProductDto>> GetProductsById(int idProduct)
        {
            var products = await _productRepository.Find(x => x.Id == idProduct);
            if (products != null)
            {
                List<ProductDto> productDtos = new List<ProductDto>();
                foreach (var item in products)
                {
                    ProductDto productDto = ProductMapper.toProductDto(item);
                    productDto.ImageUrl = (await _imageRepository.Find(x => x.ProductID == item.Id)).Select(x => x.Url).ToList();
                    productDtos.Add(productDto);
                }
                return await Task.FromResult(productDtos);
            }
            throw null;
        }
        public async Task<List<ProductDto>> GetProductsByName(string productName)
        {
            var products = await _productRepository.GetAll();
            if (products != null && !String.IsNullOrEmpty(productName))
            {
                productName.ToLower();
                products = products.Where(x => x.Name.ToLower().Contains(productName)).ToList();
                List<ProductDto> productDtos = new List<ProductDto>();
                foreach (var item in products)
                {
                    ProductDto productDto = ProductMapper.toProductDto(item);
                    productDto.ImageUrl = (await _imageRepository.Find(x => x.ProductID == item.Id)).Select(x => x.Url).ToList();
                    productDtos.Add(productDto);
                }
                return await Task.FromResult(productDtos);
            }
            throw null;
        }

        public async Task<List<List<Product>>> GetBestSeller()
        {
            var bestSeller = new List<List<Product>>();
            var laptop = await _productRepository.GetAll();
            var categories = await _categoryRepository.GetAll();
            var categoriesLaptop = categories.Where(x=> x.Code.StartsWith("LT"));
            var categoriesPC = categories.Where(x => x.Code.StartsWith("PC"));
            var categoriesMH = categories.Where(x => x.Code.StartsWith("MH"));
            var categoriesGear = categories.Where(x => x.Code.StartsWith("GMGR"));
            var bestLaptop = laptop.Join(categoriesLaptop, x => x.CategoryId, y => y.Id, (x, y) => new Product
            {
                Id = x.Id,
                Code = x.Code,
                Status = x.Status,
                Name = x.Name,
                Keyword = x.Keyword,
                AvtImageUrl = x.AvtImageUrl,
                Price = x.Price,
                Discount = x.Discount,
                ExportQuantity = x.ExportQuantity,
                ImportQuantity = x.ImportQuantity,
                Weight = x.Weight,
                Description = x.Description,
                Color = x.Color,
                IsDelete = x.IsDelete,
                DetailProduct = x.DetailProduct,
                CategoryId = x.CategoryId
            }).OrderByDescending(x => x.ExportQuantity).Take(8).ToList();
            var bestPC = laptop.Join(categoriesPC, x => x.CategoryId, y => y.Id, (x, y) => new Product
            {
                Id = x.Id,
                Code = x.Code,
                Status = x.Status,
                Name = x.Name,
                Keyword = x.Keyword,
                AvtImageUrl = x.AvtImageUrl,
                Price = x.Price,
                Discount = x.Discount,
                ExportQuantity = x.ExportQuantity,
                ImportQuantity = x.ImportQuantity,
                Weight = x.Weight,
                Description = x.Description,
                Color = x.Color,
                IsDelete = x.IsDelete,
                DetailProduct = x.DetailProduct,
                CategoryId = x.CategoryId
            }).OrderByDescending(x => x.ExportQuantity).Take(8).ToList();
            var bestScreen = laptop.Join(categoriesMH, x => x.CategoryId, y => y.Id, (x, y) => new Product
            {
                Id = x.Id,
                Code = x.Code,
                Status = x.Status,
                Name = x.Name,
                Keyword = x.Keyword,
                AvtImageUrl = x.AvtImageUrl,
                Price = x.Price,
                Discount = x.Discount,
                ExportQuantity = x.ExportQuantity,
                ImportQuantity = x.ImportQuantity,
                Weight = x.Weight,
                Description = x.Description,
                Color = x.Color,
                IsDelete = x.IsDelete,
                DetailProduct = x.DetailProduct,
                CategoryId = x.CategoryId
            }).OrderByDescending(x => x.ExportQuantity).Take(8).ToList();
            var bestGear = laptop.Join(categoriesGear, x => x.CategoryId, y => y.Id, (x, y) => new Product
            {
                Id = x.Id,
                Code = x.Code,
                Status = x.Status,
                Name = x.Name,
                Keyword = x.Keyword,
                AvtImageUrl = x.AvtImageUrl,
                Price = x.Price,
                Discount = x.Discount,
                ExportQuantity = x.ExportQuantity,
                ImportQuantity = x.ImportQuantity,
                Weight = x.Weight,
                Description = x.Description,
                Color = x.Color,
                IsDelete = x.IsDelete,
                DetailProduct = x.DetailProduct,
                CategoryId = x.CategoryId
            }).OrderByDescending(x => x.ExportQuantity).Take(8).ToList();

            //var bestLaptop = await _productRepository.Find(x => x.CategoryId == 1);
            //bestLaptop = bestLaptop.Where().OrderByDescending(x => x.ExportQuantity).ToList();
            //var bestPC = await _productRepository.Find(x=> x.CategoryId == 2);
            //bestPC = bestPC.OrderByDescending(x => x.ExportQuantity).ToList();
            //var bestScreen = await _productRepository.Find(x => x.CategoryId == 3);
            //bestScreen = bestScreen.OrderByDescending(x => x.ExportQuantity).ToList();
            //var bestGear = await _productRepository.Find(x => x.CategoryId == 5);
            //bestGear = bestGear.OrderByDescending(x => x.ExportQuantity).ToList();
            bestSeller.Add(new List<Product>());
            //foreach (var i in bestLaptop) bestSeller[0].Add(i);
            bestSeller[0].AddRange(bestLaptop);
            bestSeller.Add(new List<Product>());
            //foreach (var i in bestPC) bestSeller[1].Add(i);
            bestSeller[1].AddRange(bestPC);
            bestSeller.Add(new List<Product>());
            //foreach (var i in bestScreen) bestSeller[2].Add(i);
            bestSeller[2].AddRange(bestScreen);
            bestSeller.Add(new List<Product>());
            //foreach (var i in bestGear) bestSeller[3].Add(i);
            bestSeller[3].AddRange(bestGear);

            return bestSeller;
        }

        public async Task<List<ProductDto>> FindProduct(string productName, int stockfirst, int stocklast, int idCategory, int idProduct)
        {
            var products = await _productRepository.GetAll();
            if(idCategory != 0)
            {
                products = products.Where(x => x.CategoryId == idCategory).ToList();
            }
            if (idProduct != 0)
            {
                products = products.Where(x => x.Id == idProduct).ToList();
            }
            if (stockfirst != 0)
            {
                products = products.Where(x => x.ImportQuantity >= stockfirst).ToList();
            }
            if(stocklast != 0)
            {
                products = products.Where(x => x.ImportQuantity <= stockfirst).ToList();
            }
            if(!String.IsNullOrEmpty(productName))
            {
                productName.ToLower();
                products = products.Where(x => x.Name.ToLower().Contains(productName)).ToList();
            }
            if (products != null)
            {
                List<ProductDto> productDtos = new List<ProductDto>();
                foreach (var item in products)
                {
                    ProductDto productDto = ProductMapper.toProductDto(item);
                    productDto.ImageUrl = (await _imageRepository.Find(x => x.ProductID == item.Id)).Select(x => x.Url).ToList();
                    productDtos.Add(productDto);
                }
                return await Task.FromResult(productDtos);
            }
            throw null;
        }
        // private List<int> FindCategorySon(List<int> idCategory ,List<int> lstId){
        //     if(_context.Categories.Where(x => idCategory )
        //         return lstId;
        //     return 
        // }        
        public async Task<Product> UpdateProduct(CreateUpdateProductDto updateProductDto, int IdProduct)
        {
            var product = await _productRepository.FirstOrDefault(x => x.Id == IdProduct);
            if (product != null)
            {
                product.Status = Status.Pending;
                product.Name = updateProductDto.Name;
                product.Keyword = updateProductDto.Keyword;
                product.AvtImageUrl = CloudImage.UploadImage(updateProductDto.AvtImage);
                product.Price = updateProductDto.Price;
                product.Discount = updateProductDto.Discount;
                product.ImportQuantity = updateProductDto.ImportQuantity;
                product.Weight = updateProductDto.Weight;
                product.Description = updateProductDto.Description;
                product.Color = updateProductDto.Color;
                product.IsDelete = false;
                product.DetailProduct = updateProductDto.Detail;
                var resultProduct = await _productRepository.Add(product);
                await _productRepository.SaveChangesAsync();
                return await Task.FromResult(resultProduct);
            }
            return new Product();
        }

        public async Task<ProductDto> GetProductById(int IdProduct)
        {
            try{
                var product = await _productRepository.FirstOrDefault(x => x.Id == IdProduct);
                var listImgae = (await _imageRepository.Find(x => x.ProductID == IdProduct)).Select(s => s.Url).ToList();
                var json = JsonSerializer.Serialize(product);
                var result = JsonSerializer.Deserialize<ProductDto>(json);
                result.ImageUrl = listImgae;
                return await Task.FromResult(result);
            }
            catch(Exception ex){
                throw ex;
            }

        }
    }
}