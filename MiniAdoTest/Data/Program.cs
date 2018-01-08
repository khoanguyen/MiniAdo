using C3R.MiniAdo.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace MiniAdoTest.Data
{
    public class Program
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public string ProgramCode { get; set; }
        public string Year { get; set; }
        public byte Status { get; set; }
    }

    public class ProgramMapper : IMapper<Program>
    {
        public Program Map(DataRow row)
        {
            return new Program
            {
                ProgramId = row.Field<int>("ProgramId"),
                ProgramName = row.Field<string>("ProgramName"),
                ProgramCode = row.Field<string>("ProgramCode"),
                Year = row.Field<string>("Year"),
                Status = row.Field<byte>("Status")
            };
        }

        public void Populate(DataRow row, Program target)
        {
            target.ProgramId = row.Field<int>("ProgramId");
            target.ProgramName = row.Field<string>("ProgramName");
            target.ProgramCode = row.Field<string>("ProgramCode");
            target.Year = row.Field<string>("Year");
            target.Status = row.Field<byte>("Status");
        }

        public void PopulateRow(Program entity, DataRow target)
        {
            target.SetField<int>("ProgramId", entity.ProgramId);
            target.SetField<string>("ProgramName", entity.ProgramName);
            target.SetField<string>("ProgramCode", entity.ProgramCode);
            target.SetField<string>("Year", entity.Year);
            target.SetField<byte>("Status", entity.Status);
        }
    }
}
