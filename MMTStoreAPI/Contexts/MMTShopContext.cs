using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using MMTStoreAPI.Models;
using MMTStoreAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MMTStoreAPI.Contexts
{
    public class MMTShopContext : DbContext
    {
        public MMTShopContext() { }

        public MMTShopContext(DbContextOptions<MMTShopContext> options,
            ILogger<MMTShopContext> logger)
            : base(options)
        {
            ConnectionString = this.Database.GetDbConnection().ConnectionString;
            _logger = logger;
        }

        private readonly ILogger<MMTShopContext> _logger;

        public DbSet<StoreItem> StoreItems { get; set; }
        public DbSet<ItemCategory> ItemCategories { get; set; }
        public DbSet<FeatureItem> FeatureItems { get; set; }

        public string ConnectionString { get; set; }

        /// <summary>
        /// Performs database read for store items
        /// Gets the categories list as a first step to complete the creation of StoreItem
        /// Adds any necessary paramters to the sql command
        /// Any exception thrown by this method will result in statuscode(500)
        /// </summary>
        /// <param name="query">Paramaterised query from DatabaseQueries</param>
        /// <param name="parameterNames">Name of all parameters as they appear in the query</param>
        /// <param name="parameterValues">List of objects for use in place of the parameter</param>
        /// <returns></returns>
        public List<StoreItem> GetStoreItemsFromDB(string query, string[] parameterNames = null, Object[] parameterValues = null)
        {
            List<ItemCategory> Categories = GetCategoriesFromDB(DatabaseQueries.GetQueries[DatabaseQueries.GetEnums.GetAllCategories]);

            List<StoreItem> items = new List<StoreItem>();

            _logger.LogInformation("Attempting database query {query}", query);
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;

                    if (parameterNames != null && parameterValues != null)
                        cmd.Parameters.AddRange(ResolveParameters(parameterNames, parameterValues));

                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            StoreItem tmpItem = new StoreItem(
                                    Convert.ToInt32(sdr["Id"]),
                                    sdr["Item_Name"].ToString(),
                                    sdr["Item_Description"].ToString(),
                                    Convert.ToInt32(sdr["Item_SKU"]),
                                    float.Parse(sdr["Item_Price"].ToString())
                                );

                            tmpItem.Categories = Categories;

                            items.Add(tmpItem);
                        }
                    }
                }
            }

            _logger.LogInformation("query success");

            return items;
        }

        /// <summary>
        /// Updates the database for the given ID
        /// OR
        /// Deletes item from database for given ID
        /// OR
        /// Inserts item into database
        /// If the item already exists, then an exception is thrown resulting in StatusCode(409)
        /// Any other exception thrown by this method will result in statuscode(500)
        /// </summary>
        /// <param name="query">Paramaterised query from DatabaseQueries</param>
        /// <param name="parameterNames">Name of all parameters as they appear in the query</param>
        /// <param name="parameterValues">List of objects for use in place of the parameter</param>
        /// <returns></returns>
        public int? UpdateDB(string query, string[] parameterNames = null, Object[] parameterValues = null)
        {
            int? rowsAffected = 0;

            _logger.LogInformation("Attempting database query {query}", query);
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;

                    if (parameterNames != null && parameterValues != null)
                        cmd.Parameters.AddRange(ResolveParameters(parameterNames, parameterValues));

                    con.Open();

                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }

            _logger.LogInformation("query success");

            return rowsAffected;
        }

        /// <summary>
        /// Performs database read returning Item Categories
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameterNames"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public List<ItemCategory> GetCategoriesFromDB(string query, string[] parameterNames = null, Object[] parameterValues = null)
        {
            List<ItemCategory> Categories = new List<ItemCategory>();

            _logger.LogInformation("Attempting database query {query}", query);

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;

                    if (parameterNames != null && parameterValues != null)
                        cmd.Parameters.AddRange(ResolveParameters(parameterNames, parameterValues));

                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            Categories.Add(new ItemCategory(
                                Convert.ToInt32(sdr["Id"]),
                                sdr["Category_Name"].ToString(),
                                sdr["Category_Filter"].ToString()
                               ));
                        }
                    }
                }
            }

            _logger.LogInformation("query success");

            return Categories;
        }

        public List<FeatureItem> GetFeaturedItemsFromDB()
        {
            string query = DatabaseQueries.GetQueries[DatabaseQueries.GetEnums.GetFeaturedItems];

            List<FeatureItem> Categories = new List<FeatureItem>();

            _logger.LogInformation("Attempting database query {query}", query);

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;

                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            Categories.Add(new FeatureItem()
                            {
                                ID = Convert.ToInt32(sdr["Id"].ToString()),
                                FeaturedItemName = sdr["Category_Name"].ToString()
                            });
                        }
                    }
                }
            }

            _logger.LogInformation("query success");

            return Categories;
        }

        /// <summary>
        /// Retrieves parameters from the update and get methods
        /// Checks to see if parameters are equal in count. if they are then proceed to create the SQLParmaters list
        /// This will throw an exception to be caught by the API, resulting in StatusCode(500)
        /// </summary>
        /// <param name="paramaterNames"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public static SqlParameter[] ResolveParameters(string[] paramaterNames, Object[] parameterValues)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();

            if (paramaterNames.Count() != parameterValues.Count())
                throw new Exception("mismatch in parameters and value counts");

            for (int i = 0; i < paramaterNames.Count(); i++)
            {
                parameters.Add(new SqlParameter(paramaterNames[i], parameterValues[i]));
            }

            return parameters.ToArray();
        }
    }
}
