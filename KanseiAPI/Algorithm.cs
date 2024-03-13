using KanseiAPI.NewModel;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace KanseiAPI
{
    public class Algorithm
    {
        private List<Teacher> mTeachers;
        private List<Evaluation> mStudentPointsList;
        private List<Evaluation> mStudentPoints;
        private List<Evaluation> mStudentKansei;
        private List<Criteria> mCriteria;   
        private List<Kansei> mListKansei;
        private Dictionary<string, string> mMapResult;
        private Dictionary<string,List<KeyValuePair<string, string>>> listFinal;

        /*  
            students: Danh sách điểm đánh giá
            listKansei: Danh sách điểm lúc tư vấn sinh viên chọn
         */
        public Algorithm(List<Evaluation> students,List<Evaluation> studentsWithKansei, List<Criteria> criteria, List<Teacher> teachers, List<Kansei> listKansei)
        {
            this.mTeachers = teachers;
            this.mCriteria = criteria;
            this.mStudentPointsList = students;
            this.mStudentKansei = studentsWithKansei;
            this.mListKansei = listKansei;
            this.mStudentPoints = new List<Evaluation>();
            this.mMapResult = new Dictionary<string, string>();
            listFinal = new Dictionary<string,List<KeyValuePair<string, string>>>();
        }

        public Dictionary<string, string> MMapResult { get => mMapResult; }
        public Dictionary<string, List<KeyValuePair<string, string>>> ListFinal { get => listFinal; set => listFinal = value; }

        public void execute()
        {
            List<double> w = new List<double>();
            List<double> criteriaPoint = new List<double>();
            List<double> kanseiPoint = new List<double>();


            //Tính trung bình ra điểm tiêu chí
            for (int i = 0; i < mCriteria.Count; i++)
            {
                List<double> kanseiPointWithCriteria = mListKansei.Where(w => w.Type == mCriteria[i].Id).Select(p => p.Point).ToList();
                double valTemp = kanseiPointWithCriteria.Average();
                criteriaPoint.Add(valTemp);
            }

            //Tính trọng số của từng tiêu chí
            w = new AHP(criteriaPoint).Cal_mCompareTable();

            /*for (int i = 0; i < mCriteria.Count; i++)
            {
                List<double> kanseiPointWithCriteria = mListKansei.Where(w => w.Type == mCriteria[i].Id).Select(p => p.Point).ToList();
                List<double> temp = new AHP(kanseiPointWithCriteria).Cal_mCompareTable();
                double valTemp = 0.0f;
                for (int j = 0; j < temp.Count; j++)
                    valTemp += temp[j] * kanseiPointWithCriteria[j] / temp.Count;
                kanseiPoint.Add(valTemp);
            }*/

            //Tính TOPSIS
            for (int i = 0; i < mTeachers.Count; i++)
            {
                mStudentPoints.Add(new TOPSIS(mStudentPointsList.Where(p => p.TeacherId == mTeachers[i].Id).ToList(),
                                   w, mCriteria).execute());
            }


            //Lấy ra điểm của từng tiêu chí của từng gv
            List<List<double>> pointsCriteria = new List<List<double>>();

            //Tính AHP
            List<List<double>> finalTeachersCriteria = new List<List<double>>();

            for (int i = 0; i < mCriteria.Count; i++)
            {
                List<double> teachersPoint = new List<double>();
                this.mStudentPoints[i].ListKansei.ForEach(item =>
                {
                    teachersPoint.Add(item.Point);
                });
                
                finalTeachersCriteria.Add(new AHP(teachersPoint).Cal_mCompareTable());
            }

            // this is the final ranking point
            double[] teachersFinalPoint = new double[finalTeachersCriteria.Count];

            for (int i = 0; i < finalTeachersCriteria.Count; i++)
                for (int j = 0; j < w.Count; j++)
                    teachersFinalPoint[i] += finalTeachersCriteria[i][j] * w[j];

            // Multiply matrix finalCriteriasPoint with Weight (w array)
            List<Evaluation> detailEvaluateFinal = new List<Evaluation>();
            mStudentKansei.ForEach(item =>
            {
                for(int i = 0; i< mStudentPoints.Count; i++)
                {
                    if (mStudentPoints[i].Id == item.Id)
                        detailEvaluateFinal.Add(item);
                }
            });


            for (int i = 0; i < teachersFinalPoint.Length; i++)
            {
                mMapResult.Add(detailEvaluateFinal[i].Id, teachersFinalPoint[i].ToString());
            }
            mMapResult = mMapResult.OrderByDescending(p=>p.Value).ToDictionary(x=>x.Key, x=>x.Value);

            this.listFinal = Sort(mMapResult);
           
        }

        public Dictionary<string, List<KeyValuePair<string, string>>> Sort(Dictionary<string,string> results)
        {
            //Tạo ra một Dictionary với mỗi key tương ứng với 1 list các đánh giá có điểm bằng với key đó
            Dictionary<string, List<KeyValuePair<string, string>>> filterValue = new Dictionary<string, List<KeyValuePair<string, string>>>();
            List<int> sumEvaluation = new List<int>();

            foreach (var item in results)
                {
                foreach (var item2 in results)
                {
                    if (item.Value == item2.Value && item.Key != item2.Key)
                    {
                        if (!filterValue.ContainsKey(item.Value))
                        {
                            filterValue[item.Value] = new List<KeyValuePair<string, string>>();
                        }

                        bool isAddToList = filterValue[item.Value].Any(p => p.Key == item.Key && p.Value == item.Value);

                        if (!isAddToList)
                        {
                            filterValue[item.Value].Add(item);
                        }
                    }
                }
            }

            try
            {
                List<List<Evaluation>> sortedList = new List<List<Evaluation>>();
                List<int> countEvaluation = new List<int>();
                List<int> listPosition = new List<int>();
                List<double> listPoint = new List<double>(); //List chứa điểm đã được xử lý
                List<KeyValuePair<string, string>> listGV = new List<KeyValuePair<string, string>>();


                //Tính toán thứ tự cho từng bộ giá trị trùng nhau
                Task taskSort = Task.Run(() =>
                {
                    foreach (var value in filterValue)
                    {
                        for (int i = 0; i < mCriteria.Count; i++)
                        {
                            List<Evaluation> listEvaluation = new List<Evaluation>();
                            double maxPoint = 0;
                            foreach (var item in value.Value)
                            {
                                listEvaluation.Add(mStudentKansei.Where(p => p.Id == item.Key).SingleOrDefault());
                            }

                            int countMax = 0;
                            foreach (var item in listEvaluation)
                            {
                                double currentPoint = 0;
                                currentPoint = item.ListKansei.Where(p => p.Type == mCriteria[i].Id).Max(p => p.Point);

                                int count = item.ListKansei.Where(p => p.Point == currentPoint && p.Type == mCriteria[i].Id).Count();

                                if (count > countMax)
                                    countMax = count;
                            }
                            //Xét trường hợp - nếu có trong 1 tiêu chí chỉ có 1 điểm max thì sắp xếp theo thứ tự điểm max giảm dần, 2 điểm max trở lên thì tính tổng các điểm max giống nhau
                            switch (countMax)
                            {
                                case 1:
                                    {
                                        listEvaluation = listEvaluation.OrderByDescending(p => p.ListKansei.Where(p => p.Type == mCriteria[i].Id).Max(p => p.Point)).ToList();
                                        sortedList.Add(listEvaluation);
                                    }
                                    break;
                                default:
                                    {
                                        List<KeyValuePair<double, Evaluation>> sortListWith2SamePoint = new List<KeyValuePair<double, Evaluation>>();
                                        foreach (var item in listEvaluation)
                                        {
                                            double maxPointinCriteria = item.ListKansei.Where(p => p.Type == mCriteria[i].Id).Max(p => p.Point);

                                            double totalPointInCriteria = item.ListKansei.Where(p => p.Point == maxPointinCriteria && p.Type == mCriteria[i].Id).Sum(p => p.Point);
                                            sortListWith2SamePoint.Add(new KeyValuePair<double, Evaluation>(totalPointInCriteria, item));
                                        }

                                        sortListWith2SamePoint = sortListWith2SamePoint.OrderByDescending(p => p.Key).ToList(); // Sắp xếp danh sách

                                        List<Evaluation> listEvaluations2SamePoint = sortListWith2SamePoint.Select(p => p.Value).ToList(); // Lấy ra danh sách các Evaluation từ danh sách đã sắp xếp
                                        sortedList.Add(listEvaluations2SamePoint);
                                    }
                                    break;
                            }

                        }

                    }
                });

                //Đếm số lượng phần tử trong mỗi bộ điểm trùng nhau
                Task taskCountEvaluation = Task.Run(() =>
                {
                    foreach (var item in filterValue)
                    {
                        countEvaluation.Add(item.Value.Count);
                    }
                });

                //Lấy vị trí ban đầu của các phần tử trùng điểm
                Task taskGetPosition = Task.Run(() =>
                {
                    foreach (var item in filterValue)
                    {
                        int index = 0;
                        foreach (var item2 in results)
                        {
                            if (item2.Value == item.Key)
                            {
                                listPosition.Add(index); break;
                            }
                            index++;
                        }
                    }
                });

                Task taskGetPoint = Task.Run(() =>
                {
                    foreach (var item in filterValue)
                    {
                        listPoint.Add(Convert.ToDouble(item.Key));
                    }
                });

                Task taskGetGV = Task.Run(() =>
                {
                    foreach (var item in mMapResult)
                    {
                        
                        Evaluation evaluation = this.mStudentPoints.Where(P => P.Id == item.Key).SingleOrDefault();
                        Teacher teacher = this.mTeachers.Where(p => p.Id == evaluation.TeacherId).SingleOrDefault();
                        listGV.Add(new KeyValuePair<string, string>(item.Key, teacher.Name));
                    }
                });


                Task.WaitAll(taskSort, taskCountEvaluation, taskGetPosition, taskGetPoint);

                //Trả về kết quả
                List<List<KeyValuePair<string, string>>> listResults = new List<List<KeyValuePair<string, string>>>();

                // Duyệt qua mỗi phần tử trong sortedList
                sortedList.ForEach(item =>
                {
                    // Tạo một danh sách mới cho mỗi vòng lặp
                    List<KeyValuePair<string, string>> convertDictionaryToList = new List<KeyValuePair<string, string>>();

                    // Duyệt qua mỗi phần tử trong mMapResult và thêm vào danh sách convertDictionaryToList
                    foreach (var kvp in mMapResult)
                    {
                        convertDictionaryToList.Add(new KeyValuePair<string, string>(kvp.Key, kvp.Value));
                    }

                    // Duyệt qua mỗi vị trí trong listPosition và thực hiện các thay đổi cần thiết
                    for (int i = 0; i < listPosition.Count; i++)
                    {
                        int indexSortedList = 0;

                        for (int j = listPosition[i]; j < (listPosition[i] + countEvaluation[i]); j++)
                        {
                            if (indexSortedList < item.Count)
                            {
                                convertDictionaryToList[j] = new KeyValuePair<string, string>(item[indexSortedList].Id, listPoint[i].ToString());
                                indexSortedList++;
                            }
                            else
                            {
                                Console.WriteLine("indexSortedList vượt quá độ dài của item.");
                            }
                        }
                    }
                    listResults.Add(convertDictionaryToList);
                });


                List<List<KeyValuePair<string, string>>> listFinal = new List<List<KeyValuePair<string, string>>>();

                int i = 0;
                listResults.ForEach(item =>
                {

                    List<KeyValuePair<string, string>> newList = new List<KeyValuePair<string, string>>();
                    item.ForEach(newKey =>
                    {
                        newList.Add(new KeyValuePair<string, string>(listGV.Where(p => p.Key == newKey.Key).SingleOrDefault().Value,
                                                                    newKey.Value));
                    });
                    this.listFinal.Add(mCriteria[i].Id, newList);
                    i++;
                });

                return this.listFinal;

            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

    }
}
