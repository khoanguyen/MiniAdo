using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    public class Test
    {
        public int Id { get; set; }
        public DateTime? Timestamp { get; set; }
        public string TzId { get; set; }

        public override string ToString()
        {
            return $"[Id:{Id}, Timestamp:{Timestamp.HasValue}, TzId: {TzId}]";
        }
    }
}
