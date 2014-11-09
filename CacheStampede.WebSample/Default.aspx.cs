using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CacheStampede.WebSample
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //As you can see the code to retrieve the data is very simple!
            if (!Page.IsPostBack)
            {
                var product = new LoadProductMethod(
                    Request.QueryString["id"],
                    10,
                    5,
                    System.Web.Caching.CacheItemPriority.Normal).GetData();


                lblCreationTime.Text = product.LastRetrievalTime.ToString("HH:mm:ss");

            }
        }
        protected void lnkClearCache_Click(object sender, EventArgs e)
        {
            foreach (DictionaryEntry de in Cache)
            {
                Cache.Remove(de.Key as string);
            }
            Response.Redirect("default.aspx");
        }
    }
}