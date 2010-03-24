using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using SourceAFIS.Tuning.Reports;
using SourceAFIS.Visualization;

namespace SourceAFIS.Tuning.Errors
{
    public sealed class AccuracyStatistics
    {
        public sealed class PerDatabaseInfo
        {
            [XmlIgnore]
            public ScoreTable CombinedScores;
            [XmlIgnore]
            public ROCCurve ROC = new ROCCurve();
            public ErrorRange Range = new ErrorRange();
            public float Scalar;

            public void Compute(ScoreTable table, AccuracyMeasure measure)
            {
                CombinedScores = table.GetMultiFingerTable(measure.MultiFingerPolicy);
                ROC.Compute(CombinedScores);
                Range.Compute(ROC, measure.ErrorPolicyFunction);
                Scalar = measure.ScalarMeasure.Measure(Range.Rate);
            }

            public void Save(string folder)
            {
                Directory.CreateDirectory(folder);

                using (FileStream stream = File.Open(Path.Combine(folder, "CombinedScores.xml"), FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ScoreTable));
                    serializer.Serialize(stream, CombinedScores);
                }

                using (FileStream stream = File.Open(Path.Combine(folder, "ROC.xml"), FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ROCCurve));
                    serializer.Serialize(stream, ROC);
                }

                ImageIO.CreateBitmap(PixelFormat.ToColorB(new ROCGraph().Draw(ROC))).Save(Path.Combine(folder, "ROC.png"));
            }
        }

        public string Name;
        public PerDatabaseInfo[] PerDatabase;
        public float Average;
        [XmlIgnore]
        public TopErrors TopErrors;

        public void Compute(ScoreTable[] tables, AccuracyMeasure measure)
        {
            Name = measure.Name;
            PerDatabase = new PerDatabaseInfo[tables.Length];
            for (int db = 0; db < tables.Length; ++db)
            {
                PerDatabase[db] = new PerDatabaseInfo();
                PerDatabase[db].Compute(tables[db], measure);
                Average += PerDatabase[db].Scalar;
            }
            Average /= tables.Length;
            TopErrors = new TopErrors();
            TopErrors.Compute(tables);
        }

        public void Save(string folder, bool perDatabase)
        {
            Directory.CreateDirectory(folder);

            using (FileStream stream = File.Open(Path.Combine(folder, "Accuracy.xml"), FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AccuracyStatistics));
                serializer.Serialize(stream, this);
            }

            using (FileStream stream = File.Open(Path.Combine(folder, "TopErrors.xml"), FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TopErrors));
                serializer.Serialize(stream, TopErrors);
            }

            for (int i = 0; i < PerDatabase.Length; ++i)
                PerDatabase[i].Save(Path.Combine(folder, String.Format("Database{0}", i + 1)));
        }
    }
}