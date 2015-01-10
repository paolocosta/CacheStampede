using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// A sample class for a product
/// </summary>
public class Product
{
    public int ProductId { get; set; }
    
    //This property is used to test the cache functionality
    public DateTime LastRetrievalTime { get; set; }

    public Product(string productId)
    {
        LastRetrievalTime = DateTime.Now;  
    }
}
