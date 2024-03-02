using KanseiAPI.NewModel;

namespace KanseiAPI
{
    public class AHP
    {
        List<double> mCriteria;
        double[,] mCompareTable;
        List<double> finalResult;

        public AHP(List<double> criteria)
        {
            this.mCompareTable = new double[criteria.Count, criteria.Count];
            this.finalResult = new List<double>();
            this.mCriteria = criteria;
        }

        public List<double> Cal_mCompareTable()
        {
            List<double> result = new List<double>();
            for (int i = 0; i < this.mCompareTable.GetLength(0); i++)
                for (int j = 0; j < this.mCompareTable.GetLength(1); j++)
                {
                    if (i == j)
                        mCompareTable[i, j] = 1;
                    else if (i < j)
                    {
                        double val = (float)mCriteria[i] / mCriteria[j],
                            exten = (mCriteria[i] % mCriteria[j]) / (float)mCriteria[j];

                        //if (exten < 0.5f)
                        //{
                        //    val = (int)val;
                        //}
                        //else
                        //{
                        //    val = (int)val + 1.0f;
                        //}

                        mCompareTable[i, j] = val;
                    }
                    else
                    {
                        mCompareTable[i, j] = 1 / mCompareTable[j, i];
                    }
                }
            double[] sumCol = new double[this.mCompareTable.GetLength(0)];
            for (int i = 0; i < this.mCompareTable.GetLength(1); i++)
            {
                double sumTemp = 0;
                for (int j = 0; j < this.mCompareTable.GetLength(0); j++)
                    sumTemp += mCompareTable[j, i];
                sumCol[i] = sumTemp;
            }

            double[,] resTemp = new double[this.mCompareTable.GetLength(0), this.mCompareTable.GetLength(0)];
            for (int i = 0; i < this.mCompareTable.GetLength(0); i++)
                for (int j = 0; j < this.mCompareTable.GetLength(1); j++)
                    resTemp[i, j] = mCompareTable[i, j] / sumCol[j];

            for (int i = 0; i < resTemp.GetLength(0); i++)
            {
                double ava = 0;
                for (int j = 0; j < resTemp.GetLength(1); j++)
                    ava += resTemp[i, j] / (float)resTemp.GetLength(1);
                finalResult.Add(ava);
            }

            return finalResult;
        }
    }
}
