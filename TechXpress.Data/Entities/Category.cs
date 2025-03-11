﻿

namespace TechXpress.Data.Entities
{
    public class Category
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public List<Product> Products { get; private set; } = new();

        public Category(string name)
        {
            Name = name;
        }
    }
}
