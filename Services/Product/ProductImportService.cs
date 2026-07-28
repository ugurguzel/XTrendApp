using System.Data;
using System.Drawing;
using XTrendApp.Web.Data;
using XTrendApp.Web.Models.Amazon;
using XTrendApp.Web.Models.Entities;
using XTrendApp.Web.Repositories.Attribute;
using XTrendApp.Web.Repositories.Brand;
using XTrendApp.Web.Repositories.Category;
using XTrendApp.Web.Repositories.Collection;
using XTrendApp.Web.Repositories.Product;
using XTrendApp.Web.Repositories.ProductDocument;
using XTrendApp.Web.Repositories.ProductImage;
using XTrendApp.Web.Repositories.ProductVariation;
using XTrendApp.Web.Repositories.ProductVariationOption;
using XTrendApp.Web.Repositories.Snapshot;
using XTrendApp.Web.Repositories.Source;

namespace XTrendApp.Web.Services.Product;

public class ProductImportService
{
    private readonly DapperContext _context;

    private readonly ISourceRepository _sourceRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly ICollectionRepository _collectionRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductImageRepository _productImageRepository;

    private readonly IProductRepository _productRepository;
    private readonly IProductAttributeRepository _attributeRepository;
    private readonly IProductVariationRepository _variationRepository;
    private readonly IProductVariationOptionRepository _variationOptionRepository;
    private readonly IProductSnapshotRepository _snapshotRepository;
    private readonly IProductImageRepository _imageRepository;
    private readonly IProductDocumentRepository _documentRepository;

    public ProductImportService(
        DapperContext context,
        ISourceRepository sourceRepository,
        IBrandRepository brandRepository,
        ICollectionRepository collectionRepository,
        ICategoryRepository categoryRepository,
        IProductRepository productRepository,
        IProductAttributeRepository attributeRepository,
        IProductVariationRepository variationRepository,
        IProductVariationOptionRepository variationOptionRepository,
        IProductSnapshotRepository snapshotRepository,
        IProductImageRepository imageRepository,
        IProductDocumentRepository documentRepository,
        IProductImageRepository productImageRepository)
    {
        _context = context;

        _sourceRepository = sourceRepository;
        _brandRepository = brandRepository;
        _collectionRepository = collectionRepository;
        _categoryRepository = categoryRepository;

        _productRepository = productRepository;
        _attributeRepository = attributeRepository;
        _variationRepository = variationRepository;
        _variationOptionRepository = variationOptionRepository;
        _snapshotRepository = snapshotRepository;
        _imageRepository = imageRepository;
        _documentRepository = documentRepository;
        _productImageRepository = productImageRepository;
    }

    public async Task ImportAsync(
    AmazonDetailModel model,
    AmazonVariationResult variation,
    string sourceName,
    string countryCode)
    {
        using var connection = _context.CreateConnection();

        connection.Open();

        using var transaction = connection.BeginTransaction();

        

        try
        {
            var sourceId = await ResolveSourceAsync(
    connection,
    transaction,
    sourceName);

            var brandId = await ResolveBrandAsync(
                connection,
                transaction,
                model);

            var collectionId = await ResolveCollectionAsync(
                connection,
                transaction,
                brandId,
                model);

            var categoryId = await ResolveCategoryAsync(
                connection,
                transaction);

            var product = BuildProductEntity(
    model,
    sourceId,
    brandId,
    collectionId,
    categoryId,
    countryCode);

            var existingProduct = await _productRepository.GetBySourceProductIdAsync(
                connection,
                transaction,
                sourceId,
                model.Asin);

            Console.WriteLine();
            Console.WriteLine("========== PRODUCT IMPORT ==========");
            Console.WriteLine($"ASIN        : {model.Asin}");
            Console.WriteLine($"Title       : {model.Title}");
            Console.WriteLine($"BrandId     : {brandId}");
            Console.WriteLine($"CollectionId: {collectionId}");
            Console.WriteLine($"CategoryId  : {categoryId}");
            Console.WriteLine($"SourceId    : {sourceId}");
            Console.WriteLine($"Country     : {countryCode}");
            Console.WriteLine($"Action      : {(existingProduct == null ? "INSERT" : "UPDATE")}");
            Console.WriteLine("====================================");
            Console.WriteLine();

            if (existingProduct == null)
            {
                product.Id = await _productRepository.InsertAsync(
                    connection,
                    transaction,
                    product);
            }
            else
            {
                product.Id = existingProduct.Id;

                Console.WriteLine("UPDATE PRODUCT...");
                await _productRepository.UpdateAsync(
                    connection,
                    transaction,
                    product);
            }

            var images = BuildImageEntities(
    product.Id,
    model);

            foreach (var image in images)
            {
                var existingImage =
                    await _productImageRepository.GetByProductIdAndImageUrlAsync(
                        connection,
                        transaction,
                        image.ProductId,
                        image.ImageUrl);

                if (existingImage == null)
                {
                    await _productImageRepository.InsertAsync(
                        connection,
                        transaction,
                        image);
                }
                else
                {
                    image.Id = existingImage.Id;

                    await _productImageRepository.UpdateAsync(
                        connection,
                        transaction,
                        image);
                }
            }

            await ImportVariationsAsync(
    connection,
    transaction,
    product.Id,
    model,
    variation);

            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    private async Task<long> ResolveSourceAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    string sourceName)
    {
        var sourceId = await _sourceRepository.GetIdByNameAsync(
            connection,
            transaction,
            sourceName);

        if (!sourceId.HasValue)
            throw new InvalidOperationException(
    $"Source '{sourceName}' not found.");

        return sourceId.Value;
    }

    private async Task<long?> ResolveCollectionAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    long brandId,
    AmazonDetailModel model)
    {
        return await _collectionRepository.GetOrCreateAsync(
            connection,
            transaction,
            brandId,
            model.Collection);
    }

    private async Task<long> ResolveBrandAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    AmazonDetailModel model)
    {
        Console.WriteLine();
        Console.WriteLine("========== BRAND CHECK ==========");
        Console.WriteLine($"Brand : '{model.Brand}'");
        Console.WriteLine("=================================");
        Console.WriteLine();

        return await _brandRepository.GetOrCreateAsync(
            connection,
            transaction,
            model.Brand);
    }

    private async Task<long> ResolveCategoryAsync(
    IDbConnection connection,
    IDbTransaction transaction)
    {
        return await _categoryRepository.GetOrCreateAsync(
            connection,
            transaction,
            "Area Rugs");



    }

    private ProductEntity BuildProductEntity(
    AmazonDetailModel model,
    long sourceId,
    long brandId,
    long? collectionId,
    long? categoryId,
    string countryCode)
    {
        return new ProductEntity
        {
            SourceId = sourceId,

            BrandId = brandId,

            CollectionId = collectionId,

            CategoryId = categoryId,

            SourceProductId = model.Asin,

            Name = model.Title,

            Description = null,

            ProductUrl = model.ProductUrl,

            IsActive = true
        };
    }

    private List<ProductImageEntity> BuildImageEntities(
    long productId,
    AmazonDetailModel model)
    {
        var images = new List<ProductImageEntity>();

        // Main Image
        if (!string.IsNullOrWhiteSpace(model.ImageUrl))
        {
            images.Add(new ProductImageEntity
            {
                ProductId = productId,
                ImageUrl = model.ImageUrl,
                ImageType = "Main",
                DisplayOrder = 1,
                IsPrimary = true
            });
        }

        // Gallery Images
        var order = 2;

        foreach (var imageUrl in model.Images)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                continue;

            if (imageUrl == model.ImageUrl)
                continue;

            images.Add(new ProductImageEntity
            {
                ProductId = productId,
                ImageUrl = imageUrl,
                ImageType = "Gallery",
                DisplayOrder = order++,
                IsPrimary = false
            });
        }

        return images;
    }

    private ProductVariationEntity BuildVariationEntity(
    long productId,
    AmazonVariationSize size,
    AmazonVariationColor color,
    AmazonDetailModel model,
    int displayOrder)
    {
        return new ProductVariationEntity
        {
            ProductId = productId,

            SourceVariationId = color.Asin,

            Name = string.IsNullOrWhiteSpace(size.Title)
    ? model.Title
    : size.Title,

            SKU = null,
            UPC = null,
            EAN = null,
            GTIN = null,

            ProductUrl = model.ProductUrl,

            DisplayOrder = displayOrder,

            IsDefault =
    size.Selected &&
    color.Selected,

            IsActive = true
        };
    }

    private List<ProductVariationOptionEntity> BuildVariationOptionEntities(
    long variationId,
    AmazonVariationSize size,
    AmazonVariationColor color)
    {
        var list = new List<ProductVariationOptionEntity>();

        var displayOrder = 1;

        if (!string.IsNullOrWhiteSpace(size.Name))
        {
            list.Add(new ProductVariationOptionEntity
            {
                ProductVariationId = variationId,
                OptionName = "Size",
                OptionValue = size.Name,
                DisplayOrder = displayOrder++
            });
        }

        if (!string.IsNullOrWhiteSpace(color.Name))
        {
            list.Add(new ProductVariationOptionEntity
            {
                ProductVariationId = variationId,
                OptionName = "Color",
                OptionValue = color.Name,
                DisplayOrder = displayOrder++
            });
        }

        return list;
    }

    private ProductSnapshotEntity BuildSnapshotEntity(
    long variationId,
    AmazonVariationSize size,
    AmazonVariationColor color)
    {
        return new ProductSnapshotEntity
        {
            ProductVariationId = variationId,

            Price = color.CurrentPrice,

            ListPrice = color.ListPrice,

            SalePrice = null,

            ShippingPrice = null,

            CurrencyCode = color.CurrencyCode,

            Rating = null,

            ReviewCount = null,

            StockQuantity = null,

            IsInStock = color.Available,

            IsPrime = false,

            HasBuyBox = false,

            SellerName = null,

            BoughtLastMonthText = null,

            BoughtLastMonthCount = null,

            CouponText = null,

            DeliveryText = color.DeliveryText,

            CapturedAt = DateTime.UtcNow
        };
    }



    private async Task ImportVariationsAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    long productId,
    AmazonDetailModel model,
    AmazonVariationResult variation)
    {
        Console.WriteLine();
        Console.WriteLine("========== VARIATION IMPORT ==========");
        Console.WriteLine($"Parent ASIN : {variation.ParentAsin}");
        Console.WriteLine($"ProductId   : {productId}");
        Console.WriteLine($"Sizes       : {variation.Sizes.Count}");
        Console.WriteLine();

        if (variation.Sizes.Count == 0)
        {
            variation.Sizes.Add(new AmazonVariationSize
            {
                Asin = variation.ParentAsin,
                Name = "",
                Title = model.Title,
                Selected = true,
                Available = true,
                CurrentPrice = model.Price,
                Price = model.Price,
                CurrencyCode = model.CurrencyCode,
                DeliveryText = null
            });

            Console.WriteLine("No variations found. Default variation created.");
            Console.WriteLine();
        }


        var displayOrder = 1;



        foreach (var size in variation.Sizes)
        {

            Console.WriteLine();
            Console.WriteLine($"SIZE : {size.Name}");

            foreach (var color in size.Colors)
            {
                var variationEntity = BuildVariationEntity(
                productId,
                size,
                color,
                model,
                displayOrder++);
                Console.WriteLine(
                    $"   {color.Name} | {color.Asin} | {color.CurrentPrice}");

                var existingVariation =
    await _variationRepository.GetBySourceVariationIdAsync(
        connection,
        transaction,
        productId,
        variationEntity.SourceVariationId);

                if (existingVariation == null)
                {
                    variationEntity.Id =
                        await _variationRepository.InsertAsync(
                            connection,
                            transaction,
                            variationEntity);
                }
                else
                {
                    variationEntity.Id = existingVariation.Id;

                    Console.WriteLine("UPDATE VARIATION...");
                    await _variationRepository.UpdateAsync(
                        connection,
                        transaction,
                        variationEntity);
                }

                var optionEntities = BuildVariationOptionEntities(
        variationEntity.Id,
        size,
        color);

                Console.WriteLine("========== SIZE DATA ==========");
                Console.WriteLine($"ASIN        : {size.Asin}");
                Console.WriteLine($"Size        : {size.Name}");
                Console.WriteLine($"Price       : {size.Price}");
                Console.WriteLine($"List Price  : {size.ListPrice}");
                Console.WriteLine($"Currency    : {size.CurrencyCode}");
                Console.WriteLine($"Available   : {size.Available}");
                Console.WriteLine($"Delivery    : {size.DeliveryText}");
                Console.WriteLine("===============================");
                Console.WriteLine();

                var snapshotEntity = BuildSnapshotEntity(
                    variationEntity.Id,
                    size,
                    color);

                Console.WriteLine("INSERT SNAPSHOT...");
                snapshotEntity.Id =


                    await _snapshotRepository.InsertAsync(
                        connection,
                        transaction,
                        snapshotEntity);

                foreach (var option in optionEntities)
                {
                    var existingOption =
                        await _variationOptionRepository.GetByVariationIdAndOptionNameAsync(
                            connection,
                            transaction,
                            option.ProductVariationId,
                            option.OptionName);

                    if (existingOption == null)
                    {
                        option.Id =
                            await _variationOptionRepository.InsertAsync(
                                connection,
                                transaction,
                                option);
                    }
                }

                PrintVariationToConsole(
                    variationEntity,
                    optionEntities,
                    snapshotEntity);

            }

            Console.WriteLine();


            

            
        }

        await Task.CompletedTask;
    }

    private void PrintVariationToConsole(
    ProductVariationEntity variation,
    List<ProductVariationOptionEntity> options,
    ProductSnapshotEntity snapshot)
    {
        Console.WriteLine();
        Console.WriteLine("==============================================");
        Console.WriteLine("PRODUCT VARIATION");
        Console.WriteLine("==============================================");

        Console.WriteLine($"Child ASIN : {variation.SourceVariationId}");

        foreach (var option in options)
        {
            Console.WriteLine($"{option.OptionName,-10}: {option.OptionValue}");
        }

        Console.WriteLine();
        Console.WriteLine($"Price      : {snapshot.Price}");
        Console.WriteLine($"List Price : {snapshot.ListPrice}");
        Console.WriteLine($"Currency   : {snapshot.CurrencyCode}");
        Console.WriteLine($"In Stock   : {snapshot.IsInStock}");
        Console.WriteLine($"Delivery   : {snapshot.DeliveryText}");

        Console.WriteLine("==============================================");
        Console.WriteLine();
    }


}