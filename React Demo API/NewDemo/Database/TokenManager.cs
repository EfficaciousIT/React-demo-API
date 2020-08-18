using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace NewDemo.Database
{
    public class TokenManager
    {
        private static string Secret = "ERMN05OPLoDvbTTa/QkqLNMI7cPLguaRyHzyg7n5qNBVjQmtBhz4SzYh4NBVCXi3KJHlSXKP+oi2+bXr6CUYTR==";
        public static List<string> GenerateToken(string username)
        {
            byte[] key = Convert.FromBase64String(Secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                      new Claim(ClaimTypes.Name, username)}),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(securityKey,
                SecurityAlgorithms.HmacSha256Signature)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            //return handler.WriteToken(token);
            List<string> Data = new List<string>();
            Data.Add(handler.WriteToken(token));
            Data.Add(token.ValidFrom.ToString());
            Data.Add(token.ValidTo.ToString());
            // return handler.WriteToken(token);
            return Data;

        }
        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null)
                    return null;
                byte[] key = Convert.FromBase64String(Secret);
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token,
                      parameters, out securityToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }
        public static string ValidateToken(string token)
        {
            string username = null;
            ClaimsPrincipal principal = GetPrincipal(token);
            if (principal == null)
                return null;
            ClaimsIdentity identity = null;
            try
            {
                identity = (ClaimsIdentity)principal.Identity;
            }
            catch (NullReferenceException)
            {
                return null;
            }
            Claim usernameClaim = identity.FindFirst(ClaimTypes.Name);
            username = usernameClaim.Value;
            return username;
        }

        public static string GenerateLogoutToken(string username)
        {
            byte[] key = Convert.FromBase64String(Secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                      new Claim(ClaimTypes.Name, username)}),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(securityKey,
                SecurityAlgorithms.HmacSha256Signature)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);

        }


        public static bool CheckTokenExits(string Token)
        {
            try
            {
                string constr = System.Configuration.ConfigurationManager.ConnectionStrings["connstr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    String query = "usp_jwttoken";
                    SqlCommand com = new SqlCommand(query, con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@command", "CheckTokenExitsorNot");
                    com.Parameters.AddWithValue("@jwt_token", Token);
                    SqlDataAdapter da = new SqlDataAdapter(com);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "validToken");

                    return ds.Tables[0].Rows.Count > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool ExpireToken(string Token)
        {
            try
            {
                string constr = System.Configuration.ConfigurationManager.ConnectionStrings["connstr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    String query = "usp_jwttoken";
                    SqlCommand com = new SqlCommand(query, con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@command", "ExpireToken");
                    com.Parameters.AddWithValue("@jwt_token", Token);
                    SqlDataAdapter da = new SqlDataAdapter(com);
                    DataSet ds = new DataSet();
                    da.Fill(ds, "validToken");

                    return ds.Tables[0].Rows.Count > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}