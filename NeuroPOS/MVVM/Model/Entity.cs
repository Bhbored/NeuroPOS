using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroPOS.MVVM.Model
{
    public class Entity
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

    }
}
