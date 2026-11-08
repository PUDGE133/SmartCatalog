namespace SmartCatalog.Infrastructure.Cache;

public static class CacheKeys
{
    /// <summary>
    /// Время жизни отдельного продукта в кэшэ.
    /// </summary>
    public static readonly TimeSpan ProductTtl = TimeSpan.FromMinutes(10);

    /// <summary>
    /// Время жизни листа продуктов в кэшэ.
    /// </summary>
    public static readonly TimeSpan ListTtl = TimeSpan.FromMinutes(5);

    /// <summary>
    /// По этому ключу в кэшэ будут храниться версия листов. Как только один из продуктов меняется, версия листов увеличивается на 1. Это нужно для того, чтобы при изменении продукта, все листы продуктов, которые были закэшированы ранее, стали невалидными. Они со временем сами умрут по TTL, а новые листы будут кэшироваться на новой версии.
    /// </summary>
    public static readonly string ProductListVersionKey = "products:list:version";

    /// <summary>
    /// По этому ключу в кэшэ будет храниться конкретный продукт.
    /// </summary>
    /// <param name="id">id продукта</param>
    /// <returns>Ключ</returns>
    public static string GetProductKey(Guid id) => $"product:{id}";

    /// <summary>
    /// По этому ключу в кэшэ будет храниться список продуктов под специфический запрос пользователя.
    /// </summary>
    /// <param name="version">Версия листа. После изменения любого продукта, все листы становятся не валидными из-за старой версии.</param>
    /// <param name="skip">Сколько продуктов пропустить</param>
    /// <param name="take">Сколько продуктов получить</param>
    /// <returns></returns>
    public static string GetProductListKey(long version, int skip, int take) => $"products:list:v{version}:{skip}:{take}";
}