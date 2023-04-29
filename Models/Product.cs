//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Agent_WebForm_Project.Models
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;

    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
        }
    
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductSize { get; set; }
        public string ProductUnitSize { get; set; }
        public string ProductBrand { get; set; }
        public string ProductOrigin { get; set; }
        public Nullable<int> ProductQuantity { get; set; }
        public Nullable<decimal> ProductPrice { get; set; }
        public Nullable<bool> ProductDeleted { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        public List<Product> SelectProductQuery()
        {
            List<Product> res = new List<Product>();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConn"].ToString()))
            {
                conn.Open();
                string sql = "select * from Product";
                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Product product = new Product();
                    product.ProductID = dr["ProductID"].ToString();
                    product.ProductName = dr["ProductName"].ToString();
                    product.ProductSize = dr["ProductSize"].ToString();
                    product.ProductUnitSize = dr["ProductUnitSize"].ToString();
                    product.ProductBrand = dr["ProductBrand"].ToString();
                    product.ProductOrigin = dr["ProductOrigin"].ToString();
                    product.ProductQuantity = Convert.ToInt32(dr["ProductQuantity"].ToString());
                    product.ProductPrice = Convert.ToDecimal(dr["ProductPrice"].ToString());
                    res.Add(product);
                }
                conn.Close();
                return res;
            }
        }

    }
}
