namespace AUth.Controllers
{
    using AUth.ManifestModels;
    using AUth.Models;
    using Dapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using MimeKit;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Threading.Tasks;

    //[Authorize(Roles = UserRoles.User)]
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
                con.Close();
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
                con.Close();
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
                con.Close();
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

        private DynamicParameters ContactUdsParameters(ContactUs contactUs)
        {
            string str = string.Join(",", contactUs.LongTrips);
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Name", contactUs.Name);
            parameters.Add("@PhoneNumber", contactUs.PhoneNumber);
            parameters.Add("@Email", contactUs.Email); 
            parameters.Add("@weekend", contactUs.weekend);
            parameters.Add("@LongWeekend", contactUs.LongWeekend);
            parameters.Add("@LongTrips", str);
            return parameters;
        }

        private DynamicParameters Placesparams(int str)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@SpotID", str);
            return parameters;
        }

        [HttpPost("places/{id}")]
        public IActionResult Places(int id)
        {
            try
            {
                using (IDbConnection con = new SqlConnection(ConnectionString))
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                var result = con.Query<Places>("GetPlacesInSpot", this.Placesparams(id), commandType: CommandType.StoredProcedure);
                con.Close();
                return Ok(result);
            }
            }
            
            catch (SqlException ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("ContactUs")]
        public IActionResult ContactUs([FromBody] ContactUs contactUs)
        {
            try
            {
                using (IDbConnection con = new SqlConnection(ConnectionString))
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    var result = con.Query<Result>("USP_ContactUs", this.ContactUdsParameters(contactUs), commandType: CommandType.StoredProcedure);
                    con.Close();

                    return Ok("added succsefully");
                }
            }
            catch (SqlException ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(500, "Internal server error");
            }

        }

        private DynamicParameters BookingssParameters(Bookings bookings)
        {
            //string str = string.Join(",", bookings.PlaceId);
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Name", bookings.Name);
            parameters.Add("@PhoneNumber", bookings.PhoneNumber);
            parameters.Add("@Email", bookings.Email);
            parameters.Add("@PlaceIds", bookings.PlaceId);
            parameters.Add("@WeekendId", bookings.WeekendId);
            parameters.Add("@LongWeekendId", bookings.LongWeekendId);
            return parameters;
        }

        [HttpPost("Bookings")]
        public IActionResult Bookings([FromBody] Bookings bookings)
        {
            
                using (IDbConnection con = new SqlConnection(ConnectionString))
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    var result = con.Query<Result>("Bookings2", this.BookingssParameters(bookings), commandType: CommandType.StoredProcedure);
                    con.Close();

                    return Ok("added succsefully");
                }
            
            //catch (SqlException ex)
            //{
            //    // Log the exception or handle it as needed
            //    return StatusCode(500, "Internal server error");
            //}
        }
    }
}
