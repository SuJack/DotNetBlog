﻿using DotNetBlog.Core.Data;
using DotNetBlog.Core.Entity;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DotNetBlog.Core.Model;
using DotNetBlog.Core.Model.Category;

namespace DotNetBlog.Core.Service
{
    public class CategoryService
    {
        private static readonly string CacheKey_Category = "Cache_Category";

        private static readonly string CacheKey_CategoryTopics = "Cache_CategoryTopics";

        private BlogContext BlogContext { get; set; }

        private IMemoryCache Cache { get; set; }

        public CategoryService(BlogContext blogContext, IMemoryCache cache)
        {
            BlogContext = blogContext;
            Cache = cache;
        }

        private async Task<List<Category>> AllCategories()
        {
            var list = Cache.Get<List<Category>>(CacheKey_Category);
            if (list == null)
            {
                list = await BlogContext.Categories.ToListAsync();
                Cache.Set(CacheKey_Category, list);
            }

            return list;
        }

        private async Task<List<CategoryTopic>> AllCategoryTopics()
        {
            var list = Cache.Get<List<CategoryTopic>>(CacheKey_CategoryTopics);
            if (list == null)
            {
                list = await BlogContext.CategoryTopics.ToListAsync();
                Cache.Set(CacheKey_Category, list);
            }

            return list;
        }

        public async Task<List<CategoryModel>> All()
        {
            List<Category> entityList = await AllCategories();
            return await Transform(entityList.ToArray());
        }

        public async Task<OperationResult<CategoryModel>> Add(string name, string description)
        {
            if (await BlogContext.Categories.AnyAsync(t => t.Name == name))
            {
                return OperationResult<CategoryModel>.Failure("重复的分类名称");
            }

            var entity = new Category
            {
                Description = description,
                Name = name
            };
            BlogContext.Categories.Add(entity);
            await BlogContext.SaveChangesAsync();

            Cache.Remove(CacheKey_Category);

            CategoryModel model = (await Transform(entity)).First();

            return new OperationResult<CategoryModel>(model);
        }

        private async Task<List<CategoryModel>> Transform(params Category[] entityList)
        {
            if (entityList == null)
            {
                return null;
            }

            List<CategoryTopic> categoryTopics = await AllCategoryTopics();
            List<CategoryModel> result = entityList.Select(category => new CategoryModel
            {
                Description = category.Description,
                ID = category.ID,
                Name = category.Name,
                Topics = categoryTopics.Count(ct => ct.CategoryID == category.ID)
            }).ToList();

            return result;
        }
    }
}
