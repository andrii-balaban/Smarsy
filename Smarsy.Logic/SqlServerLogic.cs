namespace Smarsy.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using SmarsyEntities;

    public class SqlServerLogic : IDatabaseLogic
    {
        private readonly string _stringConn =
            "Data Source = localhost;Initial Catalog=Smarsy; Integrated Security = True; Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Connect Timeout = 60; Encrypt=False;TrustServerCertificate=True";
        ////"Data Source=(localdb)\\ProjectsV13;Initial Catalog=Smarsy;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=60;Encrypt=False;TrustServerCertificate=True";
        private string _smarsyLogin;

        public SqlServerLogic(string smarsyLogin)
        {
            _smarsyLogin = smarsyLogin;
        }

        public SqlServerLogic()
        {
        }

        public void UpsertLessons(List<string> lessons)
        {
            foreach (var lesson in lessons)
            {
                InsertLessonIfNotExists(lesson);
            }
        }

        public void UpsertAds(IList<Ad> ads)
        {
            foreach (var ad in ads)
            {
                InsertAdIfNotExists(ad);
            }
        }

        public void UpsertHomeWorks(List<HomeWork> homeWorks)
        {
            foreach (var homeWork in homeWorks)
            {
                UpsertHomeWork(homeWork);
            }
        }

        public void UpserStudentAllLessonsMarks(IList<LessonMark> marks)
        {
            var studentId = GetStudentIdBySmarsyLogin(_smarsyLogin);
            foreach (var mark in marks)
            {
                var lessonId = GetLessonIdByName(mark.LessonName);
                UpsertStudentMarks(studentId, lessonId, mark.Marks);
            }
        }

        public int GetLessonIdByLessonShortName(string lessonName)
        {
            using (var objconnection = new SqlConnection(_stringConn))
            {
                objconnection.Open();

                using (var objcmd = new SqlCommand("select dbo.fn_GetLessonIdByLessonShortName(@lessonName)", objconnection))
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
            using (var objconnection = new SqlConnection(_stringConn))
            {
                objconnection.Open();
                using (var objcmd = new SqlCommand("dbo.p_InsertTeacherIfNotExists", objconnection))
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
            using (var objconnection = new SqlConnection(_stringConn))
            {
                objconnection.Open();

                using (var objcmd = new SqlCommand("select dbo.fn_GetLessonIdByLessonName(@lessonName)", objconnection))
                {
                    objcmd.CommandType = CommandType.Text;
                    objcmd.Parameters.AddWithValue("@lessonName", markLessonName);
                    var res = objcmd.ExecuteScalar();
                    return int.Parse(res.ToString());
                }
            }
        }

        public Student GetStudentBySmarsyLogin(string login)
        {
            var result = new Student();
            using (var objconnection = new SqlConnection(_stringConn))
            {
                objconnection.Open();
                using (var objcmd = new SqlCommand("dbo.p_GetStudentBySmarsyId", objconnection))
                {
                    objcmd.CommandType = CommandType.StoredProcedure;
                    objcmd.Parameters.Add("@login", SqlDbType.VarChar, 50);
                    objcmd.Parameters["@login"].Value = login;

                    var res = objcmd.ExecuteReader();
                    while (res.Read())
                    {
                        result.StudentId = int.Parse(res["Id"].ToString());
                        result.Name = res["Name"].ToString();
                        result.Login = res["Login"].ToString();
                        result.Password = res["Password"].ToString();
                        result.SmarsyChildId = int.Parse(res["SmarsyChildId"].ToString());
                        result.BirthDate = Convert.ToDateTime(res["BirthDate"].ToString());
                    }
                }

                return result;
            }
        }

        public List<Student> GetStudentsWithBirthdayTomorrow()
        {
            var students = new List<Student>();
            using (var objconnection = new SqlConnection(_stringConn))
            {
                objconnection.Open();
                using (var objcmd = new SqlCommand("dbo.p_GetStudentsWithBirthdayTomorrow", objconnection))
                {
                    objcmd.CommandType = CommandType.StoredProcedure;

                    var res = objcmd.ExecuteReader();
                    while (res.Read())
                    {
                        students.Add(new Student()
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

        public List<Ad> GetNewAds()
        {
            var ads = new List<Ad>();
            using (var objconnection = new SqlConnection(_stringConn))
            {
                objconnection.Open();
                using (var objcmd = new SqlCommand("dbo.p_GetNewAds", objconnection))
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

        public List<Remark> GetNewRemarks()
        {
            var remarks = new List<Remark>();
            using (var objconnection = new SqlConnection(_stringConn))
            {
                objconnection.Open();
                using (var objcmd = new SqlCommand("dbo.p_GetNewRemarks", objconnection))
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
        
        public List<HomeWork> GetHomeWorkForFuture()
        {
            var result = new List<HomeWork>();

            using (var objconnection = new SqlConnection(_stringConn))
            {
                objconnection.Open();
                using (var objcmd = new SqlCommand("dbo.p_GetHomeWorkForFuture", objconnection))
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

        public List<LessonMark> GetStudentMarkSummary(int studentId)
        {
            var result = new List<LessonMark>();
            using (var objconnection = new SqlConnection(_stringConn))
            {
                objconnection.Open();
                using (var objcmd = new SqlCommand("dbo.p_GetStudentMarkSummary", objconnection))
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

        public void UpsertStudents(IList<Student> students)
        {
            foreach (var student in students)
            {
                UpsertStudent(student);
            }
        }

        public void UpsertRemarks(IList<Remark> remarks)
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
            using (var objconnection = new SqlConnection(_stringConn))
            {
                objconnection.Open();
                using (var objcmd = new SqlCommand("dbo.p_UpsertRemark", objconnection))
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

        private void UpsertStudent(Student student)
        {
            using (var objconnection = new SqlConnection(_stringConn))
            {
                objconnection.Open();
                using (var objcmd = new SqlCommand("dbo.p_UpsertStudent", objconnection))
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
            using (var objconnection = new SqlConnection(_stringConn))
            {
                objconnection.Open();
                using (var objcmd = new SqlCommand("dbo.p_UpsertStudentMarksByLesson", objconnection))
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

        private int GetStudentIdBySmarsyLogin(string login)
        {
            using (var objconnection = new SqlConnection(_stringConn))
            {
                objconnection.Open();

                using (var objcmd = new SqlCommand("select dbo.GetStudentIdBySmarsyLogin(@login)", objconnection))
                {
                    objcmd.CommandType = CommandType.Text;
                    objcmd.Parameters.AddWithValue("@login", login);
                    var res = objcmd.ExecuteScalar();
                    return int.Parse(res.ToString());
                }
            }
        }

        private void UpsertHomeWork(HomeWork hw)
        {
            using (var objconnection = new SqlConnection(_stringConn))
            {
                objconnection.Open();
                using (var objcmd = new SqlCommand("dbo.p_UpsertHomeWork", objconnection))
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
            using (var objconnection = new SqlConnection(_stringConn))
            {
                objconnection.Open();
                using (var objcmd = new SqlCommand("dbo.p_InsertLessonIfNotExists", objconnection))
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
            using (var objconnection = new SqlConnection(_stringConn))
            {
                objconnection.Open();
                using (var objcmd = new SqlCommand("dbo.p_InsertAdsIfNotExists", objconnection))
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
    }
}
