using Domain.Common.Dtos;
using Domain.Entities;
using Domain.Repositories;
using Elastic.Clients.Elasticsearch;
using Infrastructure.Configs;
using Infrastructure.Context;
namespace Infrastructure.Repositories;

public class FoodElasticRepository : IFoodElasticRepository
{
    private readonly ElasticsearchClient _elasticClient;
    private readonly RestaurantFoodContext _dbContext;
    private readonly EnvironmentConfig _config;

    public FoodElasticRepository(ElasticsearchClient elasticClient, RestaurantFoodContext dbContext, EnvironmentConfig environmentConfig)
    {
        _elasticClient = elasticClient;
        _dbContext = dbContext;
        _config = environmentConfig;
    }

    public async Task<IEnumerable<Guid>> SearchRestaurantIdsByFoodNameAsync(string keyword,
        CancellationToken cancellationToken = default)
    {
        var searchResponse = await _elasticClient.SearchAsync<FoodElasticDto>(s => 
            s.Index(_config.ElasticSearchDefaultIndex)
            .Query(q => q
                .Match(m => m
                    .Field(f => f.Name)
                    .Query(keyword)
                    .Fuzziness(new Fuzziness("AUTO"))
                    .MaxExpansions(50)
                )
            )
           ,cancellationToken);

        if (!searchResponse.IsValidResponse)
        {
            var err = searchResponse.DebugInformation;
            throw new Exception($"Elasticsearch search error: {err}");
        }
        return searchResponse.Hits
            .Select(h => h.Source.RestaurantId)
            .Distinct().ToList();
    }

    public async Task CreateIndexIfNotExistsAsync(string indexName, CancellationToken cancellationToken = default)
    {
        if (!(await _elasticClient.Indices.ExistsAsync(indexName, cancellationToken)).Exists)
        {
            await _elasticClient.CreateAsync(indexName, cancellationToken);
        }
    }

    public async Task<bool> AddOrUpdate(Food food, CancellationToken cancellationToken = default)
    {
        var response = await _elasticClient.IndexAsync(food, i =>
            i.Index(_config.ElasticSearchDefaultIndex)
                .OpType(OpType.Index), cancellationToken);
        return response.IsValidResponse;
    }

    public async Task<bool> AddOrUpdateBulk(IEnumerable<Food> foods, CancellationToken cancellationToken = default)
    {

        var response = await _elasticClient.BulkAsync(b =>
            b.Index(_config.ElasticSearchDefaultIndex)
                .UpdateMany(foods, (fd, f) => fd.Doc(f).DocAsUpsert(true)), cancellationToken);
        return response.IsValidResponse;
    }

    public async Task<Food> Get(string key, CancellationToken cancellationToken = default)
    {
        var response = await _elasticClient.GetAsync<Food>(key, g => 
            g.Index(_config.ElasticSearchDefaultIndex), cancellationToken);
        return response.Source;
    }

    public async Task<IEnumerable<Food>> GetAll(CancellationToken cancellationToken = default)
    {
        var response = await _elasticClient.SearchAsync<Food>(s =>
            s.Index(_config.ElasticSearchDefaultIndex), cancellationToken);
        return response.IsValidResponse ? response.Documents.ToList(): default;
    }

    public async Task<bool> Remove(string key, CancellationToken cancellationToken = default)
    {
        var response = await _elasticClient.DeleteAsync<Food>(key,
            d => d.Index(_config.ElasticSearchDefaultIndex), cancellationToken);
        return response.IsValidResponse;
    }

    public async Task<long?> RemoveAll(CancellationToken cancellationToken = default)
    {
        var response = await _elasticClient.DeleteByQueryAsync<Food>(d => d.Indices(_config.ElasticSearchDefaultIndex), cancellationToken);
        return response.IsValidResponse ? response.Deleted : null;
    }
}