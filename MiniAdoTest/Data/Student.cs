using C3R.MiniAdo.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace MiniAdoTest.Data
{
    public class Student
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public byte Status { get; set; }
    }

    public class StudentMapper : IMapper<Student>
    {
        public Student Map(DataRow row)
        {
            return new Student
            {
                StudentId = row.Field<int>("StudentId"),
                FirstName = row.Field<string>("FirstName"),
                LastName = row.Field<string>("LastName"),
                Email = row.Field<string>("Email"),
                Status = row.Field<byte>("Status")
            };
        }

        public void Populate(DataRow row, Student target)
        {
            target.StudentId = row.Field<int>("StudentId");
            target.FirstName = row.Field<string>("FirstName");
            target.LastName = row.Field<string>("LastName");
            target.Email = row.Field<string>("Email");
            target.Status = row.Field<byte>("Status");
        }

        public void PopulateRow(Student entity, DataRow target)
        {
            target.SetField<int>("StudentId", entity.StudentId);
            target.SetField<string>("FirstName", entity.FirstName);
            target.SetField<string>("LastName", entity.LastName);
            target.SetField<string>("Email", entity.Email);
            target.SetField<byte>("Status", entity.Status);            
        }
    }
}
