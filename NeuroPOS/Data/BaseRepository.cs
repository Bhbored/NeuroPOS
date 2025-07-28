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
    public class BaseRepository<T> : IBaseRepository<T> where T : Entity, new()
    {
        #region Fields
        private readonly SQLiteConnection _connection;
        private bool _disposed;
        #endregion

        #region Ctor
        public BaseRepository()
        {
            try
            {
                _connection = new SQLiteConnection(Constants.DatabasePath, Constants.Flags);
                _connection.CreateTable<T>();
                Debug.WriteLine($"[INIT] Table ensured: {typeof(T).Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR][INIT] Failed to initialize {typeof(T).Name} repository: {ex.Message}");
                Debug.WriteLine($"[ERROR][INIT] StackTrace: {ex.StackTrace}");
                throw; // Re-throw to prevent silent failures
            }
        }
        #endregion

        #region Connection
        public void StopConnection()
        {
            _connection.Close();
            Debug.WriteLine($"[CONNECTION] Closed for {typeof(T).Name}");
        }

            #endregion

        #region Insert
            public int InsertItem(T item)
        {
            try
            {
                int rows = _connection.Insert(item);
                Debug.WriteLine($"[INSERT] {typeof(T).Name} Id={item.Id} Rows={rows}");
                return rows;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR][INSERT] {ex}");
                return 0;
            }
        }

        public int InsertItems(IEnumerable<T> items)
        {
            try
            {
                int rows = _connection.InsertAll(items);
                Debug.WriteLine($"[INSERT] Batch {rows} {typeof(T).Name}(s)");
                return rows;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR][INSERT-BATCH] {ex}");
                return 0;
            }
        }

        public void InsertItemWithChildren(T item, bool recursive = false)
        {
            try
            {
                _connection.InsertWithChildren(item, recursive);
                Debug.WriteLine($"[INSERT] {typeof(T).Name} WITH children (recursive={recursive})");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR][INSERT-WITH-CHILDREN] {ex}");
            }
        }

        public void InsertItemWithExistingChildren<TChild>(
          T parent,
          IEnumerable<TChild> existingChildren,
          Action<T, IEnumerable<TChild>> assignRelation)
          where TChild : Entity, new()
        {
            try
            {
                assignRelation(parent, existingChildren);
                _connection.InsertOrReplaceWithChildren(parent, recursive: true); // FIXED HERE
                Debug.WriteLine($"[INSERT] {typeof(T).Name} + existing {typeof(TChild).Name}(s)");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR][INSERT-EXISTING-CHILDREN] {ex}");
            }
        }

        public void InsertItemWithNestedChildren<T>(T parent)
        {
            try
            {
                _connection.InsertOrReplaceWithChildren(parent, recursive: true);
                Debug.WriteLine($"[INSERT] {typeof(T).Name} + all children");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR][INSERT-NESTED-CHILDREN] {ex}");
            }
        }




        #endregion

        #region Update
        public int UpdateItem(T item)
        {
            try
            {
                int rows = _connection.Update(item);
                Debug.WriteLine($"[UPDATE] {typeof(T).Name} Id={item.Id}");
                return rows;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR][UPDATE] {ex}");
                return 0;
            }
        }

        public int UpdateItems(IEnumerable<T> items)
        {
            try
            {
                int rows = _connection.UpdateAll(items);
                Debug.WriteLine($"[UPDATE] Batch {rows} {typeof(T).Name}(s)");
                return rows;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR][UPDATE-BATCH] {ex}");
                return 0;
            }
        }

        public void UpdateItemWithChildren(T item, bool recursive = false)
        {
            try
            {
                _connection.UpdateWithChildren(item);
                Debug.WriteLine($"[UPDATE] {typeof(T).Name} WITH children (recursive={recursive})");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR][UPDATE-WITH-CHILDREN] {ex}");
            }
        }

        public void ReplaceChildren<TChild>(
            T parent,
            IEnumerable<TChild> newChildren,
            Action<T, IEnumerable<TChild>> assignRelation)
            where TChild : Entity, new()
        {
            try
            {
                assignRelation(parent, newChildren);
                _connection.UpdateWithChildren(parent);
                Debug.WriteLine($"[UPDATE] Replaced children of {typeof(T).Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR][REPLACE-CHILDREN] {ex}");
            }
        }

        public void AddNewChildToParent<TChild>(
            T parent,
            TChild newChild,
            Action<T, IEnumerable<TChild>> assignRelation)
            where TChild : Entity, new()
        {
            try
            {
                // pull current children
                _connection.GetChildren(parent, true);
                var list = parent.GetType()
                                 .GetProperties()
                                 .First(p => p.PropertyType == typeof(List<TChild>))
                                 .GetValue(parent) as List<TChild> ?? new();
                list.Add(newChild);
                assignRelation(parent, list);

                _connection.RunInTransaction(() =>
                {
                    _connection.Insert(newChild);
                    _connection.UpdateWithChildren(parent);
                });

                Debug.WriteLine($"[UPDATE] Added new {typeof(TChild).Name} to {typeof(T).Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR][ADD-NEW-CHILD] {ex}");
            }
        }

        public void AddNewChildToParentRecursively<TChild>(
            T parent,
            TChild newChild,
            Action<T, IEnumerable<TChild>> assignRelation)
            where TChild : Entity, new()
        {
            try
            {
                // Get existing children from DB
                _connection.GetChildren(parent, true);

                var list = parent.GetType()
                                 .GetProperties()
                                 .First(p => p.PropertyType == typeof(List<TChild>))
                                 .GetValue(parent) as List<TChild> ?? new();

                list.Add(newChild);
                assignRelation(parent, list);

                _connection.RunInTransaction(() =>
                {
                    // Recursively insert the child and its children (e.g., Transaction + TransactionLines)
                    _connection.InsertWithChildren(newChild, true);

                    // Recursively update the parent as well
                    _connection.UpdateWithChildren(parent);
                });

                Debug.WriteLine($"[UPDATE] Recursively added {typeof(TChild).Name} to {typeof(T).Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR][ADD-NEW-CHILD-RECURSIVE] {ex}");
            }
        }


        public void RemoveChildFromParent<TChild>(
            T parent,
            TChild child,
            Action<T, IEnumerable<TChild>> assignRelation)
            where TChild : Entity, new()
        {
            try
            {
                _connection.GetChildren(parent, true);
                var list = parent.GetType()
                                 .GetProperties()
                                 .First(p => p.PropertyType == typeof(List<TChild>))
                                 .GetValue(parent) as List<TChild>;

                if (list == null || !list.Remove(child))
                {
                    Debug.WriteLine($"[WARN] Child not found in parent list.");
                    return;
                }

                assignRelation(parent, list);
                _connection.UpdateWithChildren(parent);
                Debug.WriteLine($"[UPDATE] Removed {typeof(TChild).Name} from {typeof(T).Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR][REMOVE-CHILD] {ex}");
            }
        }

        public void UpdateChildOnly<TChild>(TChild child)
    where TChild : Entity, new()
        {
            try
            {
                _connection.Update(child);
                Debug.WriteLine($"[UPDATE] Direct child {typeof(TChild).Name} Id={child.Id}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR][UPDATE-CHILD] {ex}");
            }
        }

        #endregion

        #region Delete
        public void DeleteItem(T item, bool withChildren = false)
        {
            try
            {
                _connection.Delete(item, withChildren);
                Debug.WriteLine($"[DELETE] {typeof(T).Name} Id={item.Id} WithChildren={withChildren}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR][DELETE] {ex}");
            }
        }

        public void DeleteChild<TChild>(TChild child) where TChild : Entity, new()
        {
            try
            {
                _connection.Delete(child);
                Debug.WriteLine($"[DELETE] {typeof(TChild).Name} Id={child.Id}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR][DELETE-CHILD] {ex}");
            }
        }
        #endregion

        #region Select
        public T GetItem(int id)
        {
            try
            {
                var item = _connection.Table<T>().FirstOrDefault(x => x.Id == id);
                Debug.WriteLine(item != null
                    ? $"[SELECT] {typeof(T).Name} Id={id}"
                    : $"[SELECT] {typeof(T).Name} Id={id} NOT FOUND");
                return item;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR][SELECT] {ex}");
                return null;
            }
        }

        public T GetItem(Expression<Func<T, bool>> predicate)
        {
            try
            {
                var item = _connection.Table<T>().FirstOrDefault(predicate);
                Debug.WriteLine(item != null
                    ? $"[SELECT] {typeof(T).Name} BY predicate"
                    : $"[SELECT] {typeof(T).Name} BY predicate NOT FOUND");
                return item;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR][SELECT-PRED] {ex}");
                return null;
            }
        }

        public List<T> GetItems()
        {
            try
            {
                var list = _connection.Table<T>().ToList();
                Debug.WriteLine($"[SELECT] {list.Count} {typeof(T).Name}(s)");
                return list;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR][SELECT-ALL] {ex}");
                return new List<T>();
            }
        }

        public List<T> GetItems(Expression<Func<T, bool>> predicate)
        {
            try
            {
                var list = _connection.Table<T>().Where(predicate).ToList();
                Debug.WriteLine($"[SELECT] {list.Count} {typeof(T).Name}(s) BY predicate");
                return list;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR][SELECT-PRED-LIST] {ex}");
                return new List<T>();
            }
        }

        public List<T> GetItemsWithChildren()
        {
            try
            {
                if (_connection == null)
                {
                    Debug.WriteLine($"[ERROR][SELECT-WITH-CHILDREN] Connection is null for {typeof(T).Name}");
                    return new List<T>();
                }

                var list = _connection.GetAllWithChildren<T>().ToList();
                Debug.WriteLine($"[SELECT] {list.Count} {typeof(T).Name}(s) WITH children");
                return list;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR][SELECT-WITH-CHILDREN] {ex.Message}");
                Debug.WriteLine($"[ERROR][SELECT-WITH-CHILDREN] StackTrace: {ex.StackTrace}");
                return new List<T>();
            }
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            if (_disposed) return;
            StopConnection();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
