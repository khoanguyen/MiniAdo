using C3R.MiniAdo.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ConsoleApplication1
{
    class TestMapper : IMapper<Test>
    {
        public Test Map(DataRow row)
        {
            return new Test
            {
                Id = row.Field<int>("id"),
                Timestamp = row.Field<DateTime?>("timestamp"),
                TzId = row.Field<string>("tzId")
            };
        }

        public void Populate(DataRow row, Test target)
        {
            target.Id = row.Field<int>("id");
            target.Timestamp = row.Field<DateTime?>("timestamp");
            target.TzId = row.Field<string>("tzId");
        }

        public void PopulateRow(Test entity, DataRow target)
        {
            target.SetField("id", entity.Id);
            target.SetField("timestamp", entity.Timestamp);
            target.SetField("tzId", entity.TzId);
        }
    }
}
