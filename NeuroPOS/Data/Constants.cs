using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroPOS.Data
{
    public static class Constants
    {
        private const string DBFileName = "NeuroPOS.db3";

        public const SQLiteOpenFlags Flags =
             SQLiteOpenFlags.ReadWrite |
             SQLiteOpenFlags.Create |
             SQLiteOpenFlags.SharedCache;

        public static string DatabasePath
        {
            get
            {
                var appDataDir = FileSystem.AppDataDirectory;

                // Ensure the directory exists
                if (!Directory.Exists(appDataDir))
                {
                    Directory.CreateDirectory(appDataDir);
                }

                var dbPath = Path.Combine(appDataDir, DBFileName);

                // Ensure the database file exists (SQLite will create it if it doesn't exist)
                if (!File.Exists(dbPath))
                {
                    try
                    {
                        // Create an empty file to ensure the directory is writable
                        File.WriteAllText(dbPath, "");
                        System.Diagnostics.Debug.WriteLine($"[DB] Created database file at: {dbPath}");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[ERROR][DB] Failed to create database file: {ex.Message}");
                    }
                }

                return dbPath;
            }
        }
    }
}
