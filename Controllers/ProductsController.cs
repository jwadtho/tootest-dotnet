using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace tootest_dotnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly Context context;
        private readonly IDbConnection dbConnection;

        public ProductsController(Context context, IDbConnection dbConnection)
        {
            this.context = context;
            this.dbConnection = dbConnection;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var product = await this.context.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
            return this.Ok(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetByName([FromQuery] string name)
        {
            var @params = new Dictionary<string, object> { { "@name", name } };
            var products = await dbConnection.QueryAsync<object>("SELECT * FROM ch_product.product WHERE name = @name", @params);
            return this.Ok(products);
        }
    }
}
