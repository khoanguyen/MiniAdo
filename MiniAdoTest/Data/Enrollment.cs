using C3R.MiniAdo.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MiniAdoTest.Data
{
    public class Enrollment
    {
        public int Id { get; set; }
        public int ProgramId { get; set; }
        public int StudentId { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }

    public class EnrollmentMapper : IMapper<Enrollment>
    {
        public Enrollment Map(DataRow row)
        {
            return new Enrollment
            {
                Id = row.Field<int>("Id"),
                StudentId = row.Field<int>("StudentId"),
                ProgramId = row.Field<int>("ProgramId"),
                EnrollmentDate = row.Field<DateTime>("EnrollmentDate"),
            };
        }

        public void Populate(DataRow row, Enrollment target)
        {
            target.Id = row.Field<int>("Id");
            target.StudentId = row.Field<int>("StudentId");
            target.ProgramId = row.Field<int>("ProgramId");
            target.EnrollmentDate = row.Field<DateTime>("EnrollmentDate");
            
        }

        public void PopulateRow(Enrollment entity, DataRow target)
        {
            target.SetField<int>("Id", entity.Id);
            target.SetField<int>("StudentId", entity.StudentId);
            target.SetField<int>("ProgramId", entity.ProgramId);
            target.SetField<DateTime>("EnrollmentDate", entity.EnrollmentDate);  
        }
    }
}
