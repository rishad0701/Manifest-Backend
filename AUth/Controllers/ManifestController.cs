namespace AUth.Controllers
{
    using AUth.ManifestModels;
    using AUth.Models;
    using Dapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;

    [Authorize(Roles = UserRoles.User)]
    [Route("api/[controller]")]
    [ApiController]
    public class ManifestController : ControllerBase
    {
        public static string ConnectionString { get; set; }

        [HttpGet("DCNames")]
        public IActionResult DCNames()
        {
            using (IDbConnection con = new SqlConnection(ConnectionString))
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                var result = con.Query<DCS>("USP_DC", commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }


        [HttpPost("CarrierNames/{DCId}")]
        public IActionResult CarrierNames(int DCId)
        {
            using (IDbConnection con = new SqlConnection(ConnectionString))
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                var result = con.Query<CarriersUnderDc>("USP_CarriersUnderDc", this.SetParameters(DCId), commandType: CommandType.StoredProcedure);
                return Ok(result);
            }
        }


        [HttpPost("Manifesting")]
        public IActionResult Manifesting([FromBody] ManifestClass manifestClass)
        {
            using (IDbConnection con = new SqlConnection(ConnectionString))
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                var result = con.Query<Result>("Manifest", this.ManifestParameters(manifestClass), commandType: CommandType.StoredProcedure);

                return Ok(result);
            }
        }

        private DynamicParameters SetParameters(int dCId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@DCId", dCId);
            return parameters;
        }

        private DynamicParameters ManifestParameters(ManifestClass manifestClass)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@CarrierName", manifestClass.CarrierName);
            parameters.Add("@DCId", manifestClass.DCId);
            return parameters;
        }
    }
}
