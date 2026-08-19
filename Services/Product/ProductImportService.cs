using System.Data;
using System.Drawing;
using XTrendApp.Web.Common;
using XTrendApp.Web.Data;
using XTrendApp.Web.Models.Amazon;
using XTrendApp.Web.Models.Entities;
using XTrendApp.Web.Models.ScanJob;
using XTrendApp.Web.Repositories.Brand;
using XTrendApp.Web.Repositories.Category;
using XTrendApp.Web.Repositories.Collection;
using XTrendApp.Web.Repositories.Product;
using XTrendApp.Web.Repositories.ProductAttribute;
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

    public async Task<ScanExecutionResult> ImportAsync(
    AmazonDetailModel model,
    AmazonVariationResult variation,
    string sourceName,
    string countryCode,
    long? scanExecutionId)
    {

        var result = new ScanExecutionResult();

        using var connection = _context.CreateConnection();

        connection.Open();

        using var transaction = connection.BeginTransaction();



        try
        {
            string productAction;

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



            if (existingProduct == null)
            {
                product.Id = await _productRepository.InsertAsync(
                    connection,
                    transaction,
                    product);

                productAction = "INSERT";

                result.InsertedProducts++;

                Logger.Info("INSERT PRODUCT...");
            }
            else
            {
                product.Id = existingProduct.Id;

                await _productRepository.UpdateAsync(
                    connection,
                    transaction,
                    product);

                productAction = "UPDATE";

                result.UpdatedProducts++;

                Logger.Info("UPDATE PRODUCT...");
            }

            await ImportProductAttributesAsync(
            connection,
            transaction,
            product.Id,
            model);

            //        await ImportVariationsAsync(
            //connection,
            //transaction,
            //product.Id,
            //model,
            //variation,
            //productAction,
            //scanExecutionId);

            var variationResult = await ImportVariationsAsync(
    connection,
    transaction,
    product.Id,
    model,
    variation,
    productAction,
    scanExecutionId);

            result.InsertedVariations =
                variationResult.InsertedVariations;

            result.UpdatedVariations =
                variationResult.UpdatedVariations;

            result.SnapshotCount =
                variationResult.SnapshotCount;

            transaction.Commit();

            return result;
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
        Logger.Debug("");
        Logger.Debug("========== BRAND CHECK ==========");
        Logger.Debug($"Brand : '{model.Brand}'");
        Logger.Debug("=================================");
        Logger.Debug("");

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

            ProductUrl = BuildProductUrl(model.ProductUrl, model.Asin),

            IsActive = true
        };

    }



    private ProductVariationEntity BuildVariationEntity(
    long productId,
    AmazonVariationSize size,
    AmazonVariationColor color,
    AmazonDetailModel model,
    int displayOrder)
    {

        Logger.Debug("");
        Logger.Debug("############ BUILD VARIATION ############");
        Logger.Debug($"SIZE PARAM : {size.Name}");
        Logger.Debug($"COLOR      : {color.Name}");
        Logger.Debug($"ASIN       : {color.Asin}");
        Logger.Debug("#########################################");
        Logger.Debug("");


        var variationName = size.Name ?? string.Empty;

        Logger.Debug($"NAME CREATED : {variationName} - {color.Name}");

        if (!string.IsNullOrWhiteSpace(color.Name))
        {
            variationName += $" - {color.Name}";
        }

        return new ProductVariationEntity
        {
            ProductId = productId,

            SourceVariationId = color.Asin,

            Name = variationName,

            SKU = null,
            UPC = null,
            EAN = null,
            GTIN = null,

            ProductUrl = BuildVariationUrl(
        model.ProductUrl,
        color.Asin),

            DisplayOrder = displayOrder,

            IsDefault = size.Selected && color.Selected,

            IsActive = true
        };
    }

    private static string BuildProductUrl(
    string productUrl,
    string asin)
    {
        if (string.IsNullOrWhiteSpace(productUrl) ||
            string.IsNullOrWhiteSpace(asin))
        {
            return string.Empty;
        }

        var uri = new Uri(productUrl);

        return $"{uri.Scheme}://{uri.Host}/dp/{asin}";
    }

    private static string BuildVariationUrl(
    string productUrl,
    string asin)
    {
        if (string.IsNullOrWhiteSpace(productUrl) ||
            string.IsNullOrWhiteSpace(asin))
        {
            return string.Empty;
        }

        var uri = new Uri(productUrl);

        return $"{uri.Scheme}://{uri.Host}/dp/{asin}";
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
    AmazonVariationColor color,
    AmazonDetailModel model,
    long? scanExecutionId)
    {
        return new ProductSnapshotEntity
        {
            ProductVariationId = variationId,

            ScanExecutionId = scanExecutionId,

            Price = color.CurrentPrice,

            ListPrice = color.ListPrice,

            SalePrice = null,

            ShippingPrice = null,

            CurrencyCode = color.CurrencyCode,

            Rating = model.Rating,

            ReviewCount = model.ReviewCount,

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

    //private async Task<bool> ImportVariationImageAsync(
    //IDbConnection connection,
    //IDbTransaction transaction,
    //long productVariationId,
    //AmazonVariationColor color)
    //{
    //    if (string.IsNullOrWhiteSpace(color.ImageUrl))
    //        return false;

    //    var existing =
    //        await _productImageRepository
    //            .GetByProductVariationIdAndImageUrlAsync(
    //                connection,
    //                transaction,
    //                productVariationId,
    //                color.ImageUrl);

    //    if (existing != null)
    //        return false;

    //    await _productImageRepository.InsertAsync(
    //        connection,
    //        transaction,
    //        new ProductImageEntity
    //        {
    //            ProductVariationId = productVariationId,
    //            ImageUrl = color.ImageUrl,
    //            SortOrder = 1,
    //            IsPrimary = true
    //        });

    //    return true;
    //}

    private async Task ImportProductAttributesAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    long productId,
    AmazonDetailModel model)
    {
        // Önce eski kayıtları temizle
        await _attributeRepository.DeleteByProductIdAsync(
            connection,
            transaction,
            productId);

        var displayOrder = 1;

        // ---------- General ----------
        await InsertAttributeAsync(
            connection,
            transaction,
            productId,
            "General",
            "Brand",
            model.Brand,
            displayOrder++);

        await InsertAttributeAsync(
            connection,
            transaction,
            productId,
            "General",
            "Collection",
            model.Collection,
            displayOrder++);

        await InsertAttributeAsync(
            connection,
            transaction,
            productId,
            "General",
            "Rating",
            model.Rating?.ToString(),
            displayOrder++);

        await InsertAttributeAsync(
            connection,
            transaction,
            productId,
            "General",
            "Review Count",
            model.ReviewCount?.ToString(),
            displayOrder++);

        // ---------- Amazon Sections ----------
        foreach (var section in model.Sections)
        {
            foreach (var item in section.Value)
            {
                if (SkipAttribute(item.Key))
                    continue;

                await InsertAttributeAsync(
                    connection,
                    transaction,
                    productId,
                    section.Key,
                    item.Key,
                    item.Value,
                    displayOrder++);
            }
        }
    }



    private static readonly HashSet<string> IgnoredAttributes =
[
    "ASIN",
    "Brand Name",
    "Collection",
    "Customer Reviews",

    "Size",
    "Dimensions",
    "Item Dimensions",
    "Rug Size",

    "Best Sellers Rank",
    "Unit Count",
    "Number of Pieces"
];

    private static bool SkipAttribute(string attributeName)
    {
        if (string.IsNullOrWhiteSpace(attributeName))
            return true;

        return IgnoredAttributes.Contains(attributeName.Trim());
    }

    private async Task InsertAttributeAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    long productId,
    string group,
    string name,
    string? value,
    int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        await _attributeRepository.InsertAsync(
            connection,
            transaction,
            new ProductAttributeEntity
            {
                ProductId = productId,
                AttributeGroup = group,
                AttributeName = name,
                AttributeValue = value.Trim(),
                DisplayOrder = displayOrder
            });
    }

    private async Task<(int InsertedVariations, int UpdatedVariations, int SnapshotCount)> ImportVariationsAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    long productId,
    AmazonDetailModel model,
    AmazonVariationResult variation,
    string productAction,
    long? scanExecutionId)
    {
        if (variation.Sizes.Count == 0)
        {
            variation.Sizes.Add(new AmazonVariationSize
            {
                Asin = variation.ParentAsin,
                Name = "",
                Selected = true,
                Available = true,
                CurrentPrice = model.Price,
                Price = model.Price,
                CurrencyCode = model.CurrencyCode,
                DeliveryText = null
            });
        }

        // Amazon'dan gelen tüm Child ASIN'leri topla
        var currentAsins = variation.Sizes
            .SelectMany(x => x.Colors)
            .Select(x => x.Asin)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // DB'deki tüm varyasyonları al
        var dbVariations =
            await _variationRepository.GetByProductIdAsync(
                connection,
                transaction,
                productId);

        var passiveCount = 0;

        // Amazon'da artık olmayan aktif varyasyonları pasif yap
        foreach (var dbVariation in dbVariations)
        {
            if (!dbVariation.IsActive)
                continue;

            if (currentAsins.Contains(dbVariation.SourceVariationId))
                continue;

            await _variationRepository.SetActiveAsync(
                connection,
                transaction,
                dbVariation.Id,
                false);

            passiveCount++;

            if (passiveCount > 0)
            {
                Logger.Info($"Passive : {passiveCount}");
            }
            else
            {
                Logger.Info("Passive : 0");
            }

            Logger.Debug($"PASSIVE : {dbVariation.SourceVariationId}");
        }

        Logger.Info("");
        Logger.Info("====================================");
        Logger.Info("VARIATION SYNCHRONIZATION");
        Logger.Info($"Amazon Variations : {currentAsins.Count}");
        Logger.Info($"Database Records  : {dbVariations.Count}");
        Logger.Info($"Passive           : {passiveCount}");
        Logger.Info("====================================");
        Logger.Info("");

        var displayOrder = 1;

        var insertedCount = 0;
        var updatedCount = 0;

        //int insertedImageCount = 0;

        foreach (var size in variation.Sizes)
        {
            Logger.Debug("");
            Logger.Debug("==================================================");
            Logger.Debug($"SIZE : {size.Name}");
            Logger.Debug($"COLOR COUNT : {size.Colors.Count}");
            Logger.Debug("FIRST COLORS");
            Logger.Debug("--------------------------------------------------");

            foreach (var c in size.Colors.Take(5))
            {
                Logger.Debug($"{c.Name,-20} -> {c.Asin}");
            }

            Logger.Debug("==================================================");
            Logger.Debug("");

            foreach (var color in size.Colors)
            {
                var variationEntity = BuildVariationEntity(
                    productId,
                    size,
                    color,
                    model,
                    displayOrder++);

                Logger.Debug("====================================");
                Logger.Debug($"Size  : {size.Name}");
                Logger.Debug($"Color : {color.Name}");
                Logger.Debug($"ASIN  : {color.Asin}");
                Logger.Debug("====================================");

                var existingVariation =
                    await _variationRepository.GetBySourceVariationIdAsync(
                        connection,
                        transaction,
                        productId,
                        variationEntity.SourceVariationId);

                // BURAYA EKLE
                Logger.Debug("================================");
                Logger.Debug("CHECK VARIATION");
                Logger.Debug($"ProductId : {productId}");
                Logger.Debug($"ASIN      : {variationEntity.SourceVariationId}");

                if (existingVariation == null)
                {
                    Logger.Debug("FOUND : NO");
                }
                else
                {
                    Logger.Debug("FOUND : YES");
                    Logger.Debug($"DB Id   : {existingVariation.Id}");
                    Logger.Debug($"DB Name : {existingVariation.Name}");
                    Logger.Debug($"DB ASIN : {existingVariation.SourceVariationId}");
                }

                Logger.Debug("================================");


                if (existingVariation == null)
                {
                    Logger.Debug($"INSERT VARIATION : {variationEntity.SourceVariationId}");

                    variationEntity.Id =
                        await _variationRepository.InsertAsync(
                            connection,
                            transaction,
                            variationEntity);

                    insertedCount++;
                }
                else
                {

                    Logger.Debug($"UPDATE VARIATION : {variationEntity.SourceVariationId}");

                    variationEntity.Id = existingVariation.Id;

                    await _variationRepository.UpdateAsync(
                        connection,
                        transaction,
                        variationEntity);

                    updatedCount++;
                }

                //        if (await ImportVariationImageAsync(
                //connection,
                //transaction,
                //variationEntity.Id,
                //color))
                //        {
                //            insertedImageCount++;
                //        }

                var optionEntities = BuildVariationOptionEntities(
                    variationEntity.Id,
                    size,
                    color);

                var snapshotEntity = BuildSnapshotEntity(
                    variationEntity.Id,
                    size,
                    color,
                    model,
                    scanExecutionId);

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


            }


        }

        var variationCount = variation.Sizes.Sum(x => x.Colors.Count);

        

        Logger.Success("");
        Logger.Success("====================================");
        Logger.Success("PRODUCT COMPLETED");
        Logger.Success($"Product Action      : {productAction}");
        Logger.Success($"Inserted Variations : {insertedCount}");
        Logger.Success($"Updated Variations  : {updatedCount}");
        //Logger.Success($"Inserted Images     : {insertedImageCount}");
        Logger.Success($"Total Variations    : {variationCount}");
        Logger.Success($"Inserted Snapshots  : {variationCount}");
        Logger.Success("====================================");
        Logger.Success("");

        return (
    insertedCount,
    updatedCount,
    variationCount);

    }
}