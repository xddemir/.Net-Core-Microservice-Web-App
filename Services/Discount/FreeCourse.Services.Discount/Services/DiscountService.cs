using System.Data;
using Dapper;
using FreeCourse.Shared.DTOs;
using Npgsql;

namespace FreeCourse.Services.Discount.Services;

public class DiscountService : IDiscountService
{
    private readonly IConfiguration _configuration;
    private readonly IDbConnection _dbConnection;

    public DiscountService(IConfiguration configuration)
    {
        _configuration = configuration;
        _dbConnection = new NpgsqlConnection(_configuration.GetConnectionString("PostgreSql"));
    }
    
    public async Task<Response<List<Models.Discount>>> GetAll()
    {
        var discounts = await _dbConnection.QueryAsync<Models.Discount>("SELECT * FROM discount");
        return Response<List<Models.Discount>>.Success(discounts.ToList(), 200);
    }

    public async Task<Response<Models.Discount>> GetById(int id)
    {
        var discount =
            await _dbConnection.QuerySingleOrDefaultAsync<Models.Discount>("SELECT * FROM discount WHERE discount.Id = @Id", new{Id = id});

        return discount == null
            ? Response<Models.Discount>.Fail("Discount not found", 404)
            : Response<Models.Discount>.Success(discount, 200);
    }

    public async Task<Response<NoContent>> Save(Models.Discount discount)
    {
        var status = await _dbConnection.ExecuteAsync(
            "INSERT INTO discount (userId, rate, code) VALUES(@UserId, @Rate, @Code)",
            discount);

        return status > 0 ? Response<NoContent>.Success(204) : Response<NoContent>.Fail("An error occurred while editing",500);
    }

    public async Task<Response<NoContent>> Update(Models.Discount discount)
    {
        var status =
            await _dbConnection.ExecuteAsync("UPDATE discount set userId=@UserId, rate=@Rate, code=@Code WHERE id=@Id", new
            {
                UserId = discount.UserId,
                Rate = discount.Rate,
                Code = discount.Code,
                Id = discount.Id
            });

        return status > 0
            ? Response<NoContent>.Success(204)
            : Response<NoContent>.Fail("An error occurred while updating", 500);
    }

    public async Task<Response<NoContent>> Delete(int id)
    {
        var status = await _dbConnection.ExecuteAsync("DELETE FROM discount WHERE Id = @Id", new{Id = id});

        return status > 0 ? Response<NoContent>.Success(204) : Response<NoContent>.Fail("User Not Found", 400);
    }

    public async Task<Response<Models.Discount>> GetByCodeAndUserId(string code, string userId)
    {
        var discount = await _dbConnection.QueryFirstOrDefaultAsync<Models.Discount>(
            "SELECT * FROM discount WHERE code = @Code AND userId = @UserId", new
            {
                Code = code,
                UserId = userId
            });

        if (discount == null) return Response<Models.Discount>.Fail("Not Found!",404);

        return Response<Models.Discount>.Success(discount, 200);

    }
}