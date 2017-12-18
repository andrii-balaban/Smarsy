using System.Linq;
using System.Security;

namespace Smarsy.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using SmarsyEntities;

    public class SmarsyRepository : IDatabaseLogic, ISmarsyRepository
    {
        private const string GetLessonByShortNameQuery = "select dbo.fn_GetLessonIdByLessonShortName(@lessonName)";
        private const string InsertTeacherIfnotExistsQuery = "dbo.p_InsertTeacherIfNotExists";
        private const string GetLessonIdByLessonNameQuery = "select dbo.fn_GetLessonIdByLessonName(@lessonName)";
        private const string GetStudentIdByCmarsyLoginQuery = "dbo.p_GetStudentBySmarsyId";
        private const string GetStudentsWithBirthdayTomorrowQuery = "dbo.p_GetStudentsWithBirthdayTomorrow";
        private const string GetNewAdsQuery = "dbo.p_GetNewAds";
        private const string GetNewRemarksQuery = "dbo.p_GetNewRemarks";
        private const string GetFutureHomeworkQuery = "dbo.p_GetHomeWorkForFuture";
        private const string GetStudentMarksQuery = "dbo.p_GetStudentMarkSummary";
        private const string UpsertRemakQuery = "dbo.p_UpsertRemark";
        private const string UpsertStudentQuery = "dbo.p_UpsertStudent";
        private const string UpsertStudentMarksQuery = "dbo.p_UpsertStudentMarksByLesson";
        private const string UpsertHomeworkQuery = "dbo.p_UpsertHomeWork";
        private const string InsertLessonIfNotExistsQuery = "dbo.p_InsertLessonIfNotExists";
        private const string InsertAdIfNotExistsQuery = "dbo.p_InsertAdsIfNotExists";

        private readonly string _connextionString;
    
        public SmarsyRepository(string connextionString)
        {
            _connextionString = connextionString;
        }

        public SmarsyRepository()
        {
        }

        public void UpsertLessons(IEnumerable<string> lessons)
        {
            foreach (var lesson in lessons)
            {
                InsertLessonIfNotExists(lesson);
            }
        }

        public void UpsertAds(IEnumerable<Ad> ads)
        {
            foreach (var ad in ads)
            {
                InsertAdIfNotExists(ad);
            }
        }

        public void UpsertHomeWorks(IEnumerable<HomeWork> homeWorks)
        {
            foreach (var homeWork in homeWorks)
            {
                UpsertHomeWork(homeWork);
            }
        }

        public void UpserStudentAllLessonsMarks(SmarsyStudent student, IEnumerable<LessonMark> marks)
        {
            foreach (var mark in marks)
            {
                var lessonId = GetLessonIdByName(mark.LessonName);
                UpsertStudentMarks(student.StudentId, lessonId, mark.Marks);
            }
        }

        public int GetLessonIdByLessonShortName(string lessonName)
        {
            using (SqlConnection connection = CreateDbConnection())
            {
                using (var objcmd = new SqlCommand(GetLessonByShortNameQuery, connection))
                {
                    objcmd.CommandType = CommandType.Text;
                    objcmd.Parameters.AddWithValue("@lessonName", lessonName);
                    var res = objcmd.ExecuteScalar();
                    return int.Parse(res.ToString());
                }
            }
        }

        public int InsertTeacherIfNotExists(string teacherName)
        {
            using (SqlConnection connection = CreateDbConnection())
            {
                using (var objcmd = new SqlCommand(InsertTeacherIfnotExistsQuery, connection))
                {
                    objcmd.CommandType = CommandType.StoredProcedure;
                    objcmd.Parameters.Add("@teacherName", SqlDbType.NVarChar, 100);
                    objcmd.Parameters["@teacherName"].Value = teacherName;
                    objcmd.Parameters.Add("@CreatedId", SqlDbType.Int).Direction = ParameterDirection.Output;
                    objcmd.ExecuteNonQuery();
                    return Convert.ToInt32(objcmd.Parameters["@CreatedId"].Value);
                }
            }
        }

        public int GetLessonIdByName(string markLessonName)
        {
            using (SqlConnection connection = CreateDbConnection())
            {
                using (var objcmd = new SqlCommand(GetLessonIdByLessonNameQuery, connection))
                {
                    objcmd.CommandType = CommandType.Text;
                    objcmd.Parameters.AddWithValue("@lessonName", markLessonName);
                    var res = objcmd.ExecuteScalar();
                    return int.Parse(res.ToString());
                }
            }
        }

        public SmarsyStudent GetStudentBySmarsyLogin(string login)
        {
            SmarsyStudent student = null;

            using (SqlConnection connection = CreateDbConnection())
            {
                using (var objcmd = new SqlCommand(GetStudentIdByCmarsyLoginQuery, connection))
                {
                    objcmd.CommandType = CommandType.StoredProcedure;
                    objcmd.Parameters.Add("@login", SqlDbType.VarChar, 50);
                    objcmd.Parameters["@login"].Value = login;

                    var res = objcmd.ExecuteReader();
                    while (res.Read())
                    {
                        student =  new SmarsyStudent(Parse(res));
                    }
                }
            }

            return student;
        }

        private StudentDto Parse(SqlDataReader res)
        {
            int studentId = int.Parse(res["Id"].ToString());
            string name = res["Name"].ToString();
            string login = res["Login"].ToString();
            string password = res["Password"].ToString();
            int smarsyChildId = int.Parse(res["SmarsyChildId"].ToString());
            DateTime birthDate = Convert.ToDateTime(res["BirthDate"].ToString());

            return new StudentDto
            {
                Login = login,
                Password = CreateSecurePasswordString(password),
                StudentId = studentId,
                Name = name,
                SmarsyChildId = smarsyChildId,
                BirthDate = birthDate
            };
        }

        private SecureString CreateSecurePasswordString(string password)
        {
            var secureString = new SecureString();
            password.ToList().ForEach(c => secureString.AppendChar(c));
            secureString.MakeReadOnly();

            return secureString;
        }

        public IEnumerable<StudentDto> GetStudentsWithBirthdayTomorrow()
        {
            var students = new List<StudentDto>();
            using (SqlConnection connection = CreateDbConnection())
            {
                using (var objcmd = new SqlCommand(GetStudentsWithBirthdayTomorrowQuery, connection))
                {
                    objcmd.CommandType = CommandType.StoredProcedure;

                    var res = objcmd.ExecuteReader();
                    while (res.Read())
                    {
                        students.Add(new StudentDto()
                        {
                            StudentId = int.Parse(res["Id"].ToString()),
                            Name = res["Name"].ToString(),
                            Login = res["Login"].ToString(),
                            SmarsyChildId = res["SmarsyChildId"].ToString().Equals(string.Empty) ? 0 : int.Parse(res["SmarsyChildId"].ToString()),
                            BirthDate = Convert.ToDateTime(res["BirthDate"].ToString())
                        });
                    }
                }

                return students;
            }
        }

        public IEnumerable<Ad> GetNewAds()
        {
            var ads = new List<Ad>();
            using (SqlConnection connection = CreateDbConnection())
            {
                using (var objcmd = new SqlCommand(GetNewAdsQuery, connection))
                {
                    objcmd.CommandType = CommandType.StoredProcedure;

                    var res = objcmd.ExecuteReader();
                    while (res.Read())
                    {
                        ads.Add(new Ad()
                        {
                            AdText = res["AdText"].ToString(),
                            AdDate = Convert.ToDateTime(res["AdDate"].ToString())
                        });
                    }
                }

                return ads;
            }
        }

        public IEnumerable<Remark> GetNewRemarks()
        {
            var remarks = new List<Remark>();
            using (SqlConnection connection = CreateDbConnection())
            {
                using (var objcmd = new SqlCommand(GetNewRemarksQuery, connection))
                {
                    objcmd.CommandType = CommandType.StoredProcedure;

                    var res = objcmd.ExecuteReader();
                    while (res.Read())
                    {
                        remarks.Add(new Remark()
                        {
                            LessonId = int.Parse(res["LessonId"].ToString()),
                            LessonName = res["LessonName"].ToString(),
                            RemarkDate = Convert.ToDateTime(res["RemarkDate"].ToString()),
                            RemarkText = res["RemarkText"].ToString()
                        });
                    }
                }

                return remarks;
            }
        }
        
        public IEnumerable<HomeWork> GetHomeWorkForFuture()
        {
            var result = new List<HomeWork>();

            using (SqlConnection connection = CreateDbConnection())
            {
                using (var objcmd = new SqlCommand(GetFutureHomeworkQuery, connection))
                {
                    objcmd.CommandType = CommandType.StoredProcedure;

                    var res = objcmd.ExecuteReader();
                    while (res.Read())
                    {
                        result.Add(new HomeWork()
                        {
                            LessonName = res["LessonName"].ToString(),
                            HomeWorkDescr = res["HomeWork"].ToString(),
                            TeacherName = res["TeacherName"].ToString(),
                            HomeWorkDate = Convert.ToDateTime(res["HomeWorkDate"].ToString())
                        });
                    }
                }

                return result;
            }
        }

        public IEnumerable<LessonMark> GetStudentMarks(int studentId)
        {
            var result = new List<LessonMark>();
            using (SqlConnection connection = CreateDbConnection())
            {
                using (var objcmd = new SqlCommand(GetStudentMarksQuery, connection))
                {
                    objcmd.CommandType = CommandType.StoredProcedure;
                    objcmd.Parameters.Add("@studentId", SqlDbType.VarChar, 50);
                    objcmd.Parameters["@studentId"].Value = studentId;

                    var marks = new List<StudentMark>();

                    var res = objcmd.ExecuteReader();
                    while (res.Read())
                    {
                        marks.Add(new StudentMark()
                        {
                            Date = Convert.ToDateTime(res["MarkDate"].ToString()),
                            Mark = int.Parse(res["Mark"].ToString()),
                            Reason = res["Reason"].ToString()
                        });

                        if (int.Parse(res["RowNum"].ToString()) == 1)
                        {
                            result.Add(new LessonMark()
                            {
                                LessonName = res["LessonName"].ToString(),
                                LessonId = int.Parse(res["LessonId"].ToString()),
                                Marks = marks
                            });
                            marks = new List<StudentMark>();
                        }
                    }
                }
            }

            return result;
        }

        public void UpsertStudents(IEnumerable<SmarsyStudent> students)
        {
            foreach (var student in students)
            {
                UpsertStudent(student.ToDto());
            }
        }

        public void UpsertRemarks(IEnumerable<Remark> remarks)
        {
            foreach (var remark in remarks)
            {
                if (remark.LessonId == 0)
                {
                    remark.LessonId = GetLessonIdByName(remark.LessonName);
                }

                UpsertRemark(remark);
            }
        }

        private void UpsertRemark(Remark remark)
        {
            using (SqlConnection connection = CreateDbConnection())
            {
                using (var objcmd = new SqlCommand(UpsertRemakQuery, connection))
                {
                    objcmd.CommandType = CommandType.StoredProcedure;
                    objcmd.Parameters.Add("@remarkText", SqlDbType.NVarChar, -1);
                    objcmd.Parameters["@remarkText"].Value = remark.RemarkText;
                    objcmd.Parameters.Add("@lessonId", SqlDbType.Int);
                    objcmd.Parameters["@lessonId"].Value = remark.LessonId;
                    objcmd.Parameters.Add("@remarkDate", SqlDbType.Date);
                    objcmd.Parameters["@remarkDate"].Value = remark.RemarkDate;

                    objcmd.ExecuteNonQuery();
                }
            }
        }

        private void UpsertStudent(StudentDto student)
        {
            using (SqlConnection connection = CreateDbConnection())
            {
                using (var objcmd = new SqlCommand(UpsertStudentQuery, connection))
                {
                    objcmd.CommandType = CommandType.StoredProcedure;
                    objcmd.Parameters.Add("@studentName", SqlDbType.NVarChar, 100);
                    objcmd.Parameters["@studentName"].Value = student.Name;
                    objcmd.Parameters.Add("@birthDate", SqlDbType.DateTime2);
                    objcmd.Parameters["@birthDate"].Value = student.BirthDate;

                    objcmd.ExecuteNonQuery();
                }
            }
        }

        private List<StudentMark> UpsertStudentMarks(int studentId, int lessonId, List<StudentMark> marks)
        {
            var result = new List<StudentMark>();
            using (SqlConnection connection = CreateDbConnection())
            {
                using (var objcmd = new SqlCommand(UpsertStudentMarksQuery, connection))
                {
                    objcmd.CommandType = CommandType.StoredProcedure;
                    objcmd.Parameters.Add("@studentId", SqlDbType.Int);
                    objcmd.Parameters["@studentId"].Value = studentId;
                    objcmd.Parameters.Add("@lessonId", SqlDbType.Int);
                    objcmd.Parameters["@lessonId"].Value = lessonId;

                    var marksWithDates = new DataTable("marksWithDates");
                    marksWithDates.Columns.Add("Mark", typeof(int));
                    marksWithDates.Columns.Add("MarkDate", typeof(DateTime));
                    marksWithDates.Columns.Add("Reason", typeof(string));

                    foreach (var mark in marks)
                    {
                        marksWithDates.Rows.Add(mark.Mark, mark.Date, mark.Reason);
                    }

                    SqlParameter tvpParam = objcmd.Parameters.AddWithValue("@marksWithDates", marksWithDates);

                    tvpParam.SqlDbType = SqlDbType.Structured;

                    var res = objcmd.ExecuteReader();
                    while (res.Read())
                    {
                        var mark = new StudentMark()
                        {
                            Mark = int.Parse(res["Mark"].ToString()),
                            Date = Convert.ToDateTime(res["MarkDate"].ToString()),
                            Reason = res["Reason"].ToString()
                        };
                        result.Add(mark);
                    }
                }

                return result;
            }
        }

        private void UpsertHomeWork(HomeWork hw)
        {
            using (SqlConnection connection = CreateDbConnection())
            {
                using (var objcmd = new SqlCommand(UpsertHomeworkQuery, connection))
                {
                    objcmd.CommandType = CommandType.StoredProcedure;
                    objcmd.Parameters.Add("@lessonId", SqlDbType.Int);
                    objcmd.Parameters["@lessonId"].Value = hw.LessonId;
                    objcmd.Parameters.Add("@homeWork", SqlDbType.NVarChar, 2000);
                    objcmd.Parameters["@homeWork"].Value = hw.HomeWorkDescr;
                    objcmd.Parameters.Add("@homeWorkDate", SqlDbType.Date);
                    objcmd.Parameters["@homeWorkDate"].Value = hw.HomeWorkDate;
                    objcmd.Parameters.Add("@teacherId", SqlDbType.Int);
                    objcmd.Parameters["@teacherId"].Value = hw.TeacherId;

                    objcmd.ExecuteNonQuery();
                }
            }
        }

        private void InsertLessonIfNotExists(string lesson)
        {
            using (SqlConnection connection = CreateDbConnection())
            {
                using (var objcmd = new SqlCommand(InsertLessonIfNotExistsQuery, connection))
                {
                    objcmd.CommandType = CommandType.StoredProcedure;
                    objcmd.Parameters.Add("@lessonName", SqlDbType.NVarChar, 100);
                    objcmd.Parameters["@lessonName"].Value = lesson;

                    objcmd.ExecuteNonQuery();
                }
            }
        }

        private void InsertAdIfNotExists(Ad ad)
        {
            using (SqlConnection connection = CreateDbConnection())
            {
                using (var objcmd = new SqlCommand(InsertAdIfNotExistsQuery, connection))
                {
                    objcmd.CommandType = CommandType.StoredProcedure;
                    objcmd.Parameters.Add("@adDate", SqlDbType.DateTime2, 7);
                    objcmd.Parameters["@adDate"].Value = ad.AdDate;
                    objcmd.Parameters.Add("@adText", SqlDbType.NVarChar, -1);
                    objcmd.Parameters["@adText"].Value = ad.AdText;

                    objcmd.ExecuteNonQuery();
                }
            }
        }

        private SqlConnection CreateDbConnection()
        {
            var connection =  new SqlConnection(_connextionString);
            connection.Open();

            return connection;
        }
    }
}
