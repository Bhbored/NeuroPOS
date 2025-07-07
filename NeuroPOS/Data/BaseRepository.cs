using NeuroPOS.Data;
using NeuroPOS.MVVM.Model;
using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NeuroPOS.Data
{
    public class BaseRepository<T> :
         IBaseRepository<T> where T : Entity, new()
    {
        SQLiteConnection connection;
        public string StatusMessage { get; set; }

        public BaseRepository()
        {
            connection =
                 new SQLiteConnection(Constants.DatabasePath,
                 Constants.Flags);
            connection.CreateTable<T>();
            Debug.WriteLine($"Table for {typeof(T).Name} ensured created.");
        }
        #region delete

        public void DeleteItem(T item)
        {
            try
            {
                connection.Delete(item, true);
                Debug.WriteLine($"Deleted {typeof(T).Name} with Id={item.Id}");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                Debug.WriteLine($"Error deleting {typeof(T).Name} with Id={item.Id}: {ex.Message}");
            }
        }
        #endregion

        #region select
        public T GetItem(int id)
        {
            try
            {
                var item = connection.Table<T>().FirstOrDefault(x => x.Id == id);
                if (item != null)
                    Debug.WriteLine($"Fetched {typeof(T).Name} with Id={id}");
                else
                    Debug.WriteLine($"{typeof(T).Name} with Id={id} not found");
                return item;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                Debug.WriteLine($"Error fetching {typeof(T).Name} with Id={id}: {ex.Message}");
                return null;
            }
        }

        public T GetItem(Expression<Func<T, bool>> predicate)
        {
            try
            {
                var item = connection.Table<T>().Where(predicate).FirstOrDefault();
                Debug.WriteLine(item != null
                    ? $"Fetched {typeof(T).Name} by predicate."
                    : $"{typeof(T).Name} not found by predicate.");
                return item;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                Debug.WriteLine($"Error fetching {typeof(T).Name} by predicate: {ex.Message}");
                return null;
            }
        }

        public List<T> GetItems()
        {
            try
            {
                var items = connection.Table<T>().ToList();
                Debug.WriteLine($"Fetched {items.Count} {typeof(T).Name}(s) from DB");
                return items;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                Debug.WriteLine($"Error fetching {typeof(T).Name}s: {ex.Message}");
                return null;
            }
        }

        public List<T> GetItems(Expression<Func<T, bool>> predicate)
        {
            try
            {
                var items = connection.Table<T>().Where(predicate).ToList();
                Debug.WriteLine($"Fetched {items.Count} {typeof(T).Name}(s) by predicate");
                return items;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                Debug.WriteLine($"Error fetching {typeof(T).Name}s by predicate: {ex.Message}");
                return null;
            }
        }


        #endregion


        public void Dispose()
        {
            connection.Close();
            Debug.WriteLine($"Connection closed for {typeof(T).Name} repository.");
        }

        #region add /update
        public void SaveItem(T item)
        {
            try
            {
                int result;
                if (item.Id != 0)
                {
                    result = connection.Update(item);
                    StatusMessage = $"{result} row(s) updated";
                    Debug.WriteLine($"Updated {typeof(T).Name} with Id={item.Id}");
                }
                else
                {
                    result = connection.Insert(item);
                    StatusMessage = $"{result} row(s) added";
                    Debug.WriteLine($"Inserted new {typeof(T).Name} with Id={item.Id}");
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                Debug.WriteLine($"Error saving {typeof(T).Name}: {ex.Message}");
            }
        }

        public void SaveItemWithChildren(T item, bool recursive = false)
        {
            try
            {
                connection.InsertWithChildren(item, recursive);
                Debug.WriteLine($"Inserted {typeof(T).Name} with children (recursive={recursive}), Id={item.Id}");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                Debug.WriteLine($"Error inserting {typeof(T).Name} with children: {ex.Message}");
            }
        }

        public List<T> GetItemsWithChildren()
        {
            try
            {
                var items = connection.GetAllWithChildren<T>().ToList();
                Debug.WriteLine($"Fetched {items.Count} {typeof(T).Name}(s) with children");
                return items;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                Debug.WriteLine($"Error fetching {typeof(T).Name}s with children: {ex.Message}");
                return null;
            }
        }

        #endregion

    }
}
