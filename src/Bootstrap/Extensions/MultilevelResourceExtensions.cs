using System.Collections.Generic;
using System.Linq;
using Bootstrap.Models;
using Microsoft.EntityFrameworkCore;

namespace Bootstrap.Extensions
{
    public static class MultilevelResourceExtensions
    {
        public static List<TResource> GetPath<TResource>(this IReadOnlyCollection<TResource> allResources, int id)
            where TResource : MultilevelResource<TResource>
        {
            var child = allResources.FirstOrDefault(t => t.Id == id);
            return allResources.GetPath(child);
        }

        /// <summary>
        /// From up to down.
        /// </summary>
        /// <typeparam name="TResource"></typeparam>
        /// <param name="allResources"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public static List<TResource> GetPath<TResource>(this IEnumerable<TResource> allResources, TResource child)
            where TResource : MultilevelResource<TResource>
        {
            if (child == null)
            {
                return null;
            }

            var path = allResources.Where(t => t.Left < child.Left && t.Right > child.Right).ToList();
            path.Add(child);
            return path.OrderBy(t => t.Left).ToList();
        }

        public enum DbType
        {
            SqlServer = 1,
            Mysql = 2
        }

        /// <summary>
        /// todo: refactor
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="modelBuilder"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static ModelBuilder ConfigureMultilevelResourcePart<TData>(this ModelBuilder modelBuilder,
            DbType dbType = DbType.SqlServer) where TData : MultilevelResource<TData>
        {
            return modelBuilder.Entity<TData>(r =>
            {
                r.HasIndex(t => new { t.Left, t.Right });
                r.HasIndex(t => t.ParentId);

                if (dbType == DbType.Mysql)
                {
                    r.Property(a => a.Id).HasColumnName("id");
                    r.Property(a => a.Left).HasColumnName("left");
                    r.Property(a => a.Right).HasColumnName("right");
                    r.Property(a => a.ParentId).HasColumnName("parent_id");
                    r.Property(a => a.CreateDt).HasColumnName("create_dt");
                }
            });
        }

        public static void BuildTree<T>(this IEnumerable<T> list) where T : MultilevelResource<T>
        {
            var dataCollection = list.ToList();
            for (var i = 0; i < dataCollection.Count; i++)
            {
                var data = dataCollection[i];
                if (i == 0)
                {
                    data.Left = data.Parent?.Left + 1 ?? 1;
                }
                else
                {
                    data.Left = dataCollection[i - 1].Right + 1;
                }

                if (data.Children?.Any() == true)
                {
                    data.Children.BuildTree();
                    data.Right = data.Children.Max(t => t.Right) + 1;
                }
                else
                {
                    data.Right = data.Left + 1;
                }
            }
        }

        public static List<T> Connect<T>(this ICollection<T> list) where T : MultilevelResource<T>
        {
            var newList = list.Where(t => list.All(a => a == t || !(a.Left > t.Left && a.Right < t.Right))).ToList();
            foreach (var n in newList)
            {
                n.Children = list.Where(t => t.Left > n.Left && t.Right < n.Right).ToList().Connect();
            }

            return newList;
        }
    }
}