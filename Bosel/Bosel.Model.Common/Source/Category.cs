using System.Collections.Generic;

namespace Bosel.Model.Common.Source
{
    /// <summary>
    /// category column values
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; set; }
        public List<CategoryCode> CategoryCodes { get; set; }
    }
    /// <summary>
    /// pattern for a category
    /// </summary>
    public class CategoryCode
    {
        /// <summary>
        /// Gets or sets the code used to classify the rows.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; set; }
    }
    public class CategoryList
    {
        public List<Category> Categories { get; set; }

        public static CategoryList Default()
        {
            return new CategoryList()
            {
                Categories = new List<Category>()
            };
        }
    }
}
