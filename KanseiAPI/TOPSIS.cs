using KanseiAPI.NewModel;

namespace KanseiAPI
{
    public class TOPSIS
    {
        private List<double> mPoints;
        private List<Evaluation> mStudents;
        private double[] mAStar, mAMinus, mSStar, mSMinus;
        private List<double> mWeights;
        private List<Criteria> mCriteria;

        public TOPSIS(List<Evaluation> students, List<double> w, List<Criteria> criteria)
        {
            this.mStudents = students;
            this.mCriteria = criteria;
            mAStar = new double[criteria.Count];
            mAMinus = new double[criteria.Count];
            mSStar = new double[students.Count];
            mSMinus = new double[students.Count];
            mWeights = w;
        }

        private void Cal_StandardizedMatrix()
        {
            for (int i = 0; i < mCriteria.Count; i++)
            {
                double sum = 0.0f;
                for (int j = 0; j < mStudents.Count; j++)
                {
                    sum += mStudents[j].ListKansei[i].Point * mStudents[j].ListKansei[i].Point;
                }

                sum = Math.Sqrt(sum);
                    for (int j = 0; j < mStudents.Count; j++)
                    {
                        mStudents[j].Standardized.Add(mStudents[j].ListKansei[i].Point / sum);

                    }
            }
        }

        private void Cal_AStarAndAMinus()
        {
            for (int i = 0; i < mCriteria.Count; i++)
                for (int j = 0; j < this.mStudents.Count; j++)
                {
                    double valWithWeight = this.mStudents[j].Standardized[i] * this.mWeights[i];
                    if (j == 0)
                    {
                        this.mAStar[i] = valWithWeight;
                        this.mAMinus[i] = valWithWeight;
                    }
                    else
                    {
                        if (valWithWeight > this.mAStar[i])
                            this.mAStar[i] = valWithWeight;
                        if (valWithWeight < this.mAMinus[i])
                            this.mAMinus[i] = valWithWeight;
                    }
                }
        }

        private void Cal_SStarAndSMinus()
        {
            for (int i = 0; i < this.mStudents.Count; i++)
            {
                for (int j = 0; j < this.mCriteria.Count; j++)
                {
                    double val = this.mStudents[i].Standardized[j] * this.mWeights[i] - this.mAStar[j];
                    this.mSStar[i] += val * val;

                    val = this.mStudents[i].Standardized[j] * this.mWeights[j] - this.mAMinus[j];
                    this.mSMinus[i] += val * val;
                }

                this.mSStar[i] = Math.Sqrt(this.mSStar[i]);
                this.mSMinus[i] = Math.Sqrt(this.mSMinus[i]);
            }
        }

        private void Cal_mCC()
        {

            try
            {
                for (int i = 0; i < this.mStudents.Count; i++)
                {
                    this.mStudents[i].mCC = this.mSMinus[i] / (this.mSMinus[i] + this.mSStar[i]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public Evaluation execute()
        {
            Cal_StandardizedMatrix();
            Cal_AStarAndAMinus();
            Cal_SStarAndSMinus();
            Cal_mCC();
            mStudents = mStudents.OrderBy(p => p.mCC).ToList();

            return mStudents[mStudents.Count / 2];
        }
    }
}
