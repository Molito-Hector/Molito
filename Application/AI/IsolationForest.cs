using System.Text;
using Newtonsoft.Json;

namespace Application.AI
{
    [JsonObject(MemberSerialization.OptIn)]
    public class IsolationForest
    {
        [JsonProperty]
        private List<IsolationTree> Trees { get; set; }
        [JsonProperty]
        private int MaxTreeHeight { get; set; }
        [JsonProperty]
        private int NumberOfTrees { get; set; }
        [NonSerialized]
        private Random Random;
        [JsonProperty]
        private double[][] Dataset { get; set; }
        [JsonProperty]
        public double Threshold { get; set; }

        public IsolationForest(int numberOfTrees, int maxTreeHeight)
        {
            Trees = new List<IsolationTree>();
            NumberOfTrees = numberOfTrees;
            MaxTreeHeight = maxTreeHeight;
            Random = new Random();
        }

        protected Random Randoms
        {
            get { return Random; }
        }

        public void Fit(double[][] dataset)
        {
            Dataset = dataset;
            int n = dataset.Length;

            for (int i = 0; i < NumberOfTrees; i++)
            {
                var subSample = SubSampleDataset(dataset);
                Trees.Add(BuildIsolationTree(subSample, 0, MaxTreeHeight));
            }


        }

        private double[][] SubSampleDataset(double[][] dataset)
        {
            int n = dataset.Length;
            int sampleSize = (int)Math.Floor(Math.Sqrt(n));
            var indices = Enumerable.Range(0, n).OrderBy(x => Random.Next()).Take(sampleSize).ToArray();
            var subSample = new double[sampleSize][];
            for (int i = 0; i < sampleSize; i++)
            {
                subSample[i] = dataset[indices[i]];
            }
            return subSample;
        }

        private IsolationTree BuildIsolationTree(double[][] data, int currentHeight, int maxHeight)
        {
            if (data.Length <= 1 || currentHeight == maxHeight)
            {
                return new IsolationTree { IsExternalNode = true, Height = currentHeight };
            }

            int numAttributes = data[0].Length;
            int splitAttribute = Random.Next(numAttributes);

            double min = data.Min(x => x[splitAttribute]);
            double max = data.Max(x => x[splitAttribute]);
            double splitValue = Random.NextDouble() * (max - min) + min;

            var leftData = data.Where(x => x[splitAttribute] < splitValue).ToArray();
            var rightData = data.Where(x => x[splitAttribute] >= splitValue).ToArray();

            if (leftData.Length == 0 || rightData.Length == 0)
            {
                return new IsolationTree { IsExternalNode = true, Height = currentHeight };
            }

            var tree = new IsolationTree
            {
                SplitAttribute = splitAttribute,
                SplitValue = splitValue,
                Left = BuildIsolationTree(leftData, currentHeight + 1, maxHeight),
                Right = BuildIsolationTree(rightData, currentHeight + 1, maxHeight)
            };

            return tree;
        }

        public double[] Score(double[][] instances)
        {
            return instances.Select(ScoreInstance).ToArray();
        }

        private double ScoreInstance(double[] instance)
        {
            double pathLength = Trees.Average(tree => PathLength(tree, instance));
            double c = C(Dataset.Length);
            double score = Math.Pow(2, -pathLength / c);
            return score;
        }

        public void CalculateThreshold(double[][] trainingData, double standardDeviationMultiplier = 1.0)
        {
            double[] scores = Score(trainingData);
            double mean = scores.Average();
            double stdDev = Math.Sqrt(scores.Sum(score => Math.Pow(score - mean, 2)) / scores.Length);
            double threshold = mean + standardDeviationMultiplier * stdDev;
            Threshold = threshold;
        }

        private int PathLength(IsolationTree tree, double[] instance)
        {
            if (tree.IsExternalNode)
            {
                return tree.Height;
            }

            if (instance[tree.SplitAttribute] < tree.SplitValue)
            {
                return PathLength(tree.Left, instance);
            }
            else
            {
                return PathLength(tree.Right, instance);
            }
        }

        private double C(int n)
        {
            if (n <= 1)
            {
                return 0;
            }

            double h = Math.Log(n - 1) + 0.5772156649; // Euler-Mascheroni constant
            double c = 2 * h - (2 * (n - 1) / (double)n);

            return c;
        }

        public void OnDeserialization(object sender)
        {
            Random = new Random();
        }

        public byte[] SaveModel(out bool success)
        {
            success = false;
            try
            {
                string json = JsonConvert.SerializeObject(this);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (StreamWriter writer = new StreamWriter(memoryStream, Encoding.UTF8))
                    {
                        writer.Write(json);
                        writer.Flush();
                        success = true;
                        return memoryStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving model: {ex.Message}");
                return null;
            }
        }

        public static IsolationForest LoadModel(string fileName, out bool success)
        {
            success = false;

            if (!File.Exists(fileName))
            {
                Console.WriteLine($"Error loading model: File '{fileName}' does not exist.");
                return null;
            }

            try
            {
                using (StreamReader fileStream = new StreamReader(fileName))
                {
                    string json = fileStream.ReadToEnd();
                    success = true;
                    return JsonConvert.DeserializeObject<IsolationForest>(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading model: {ex.Message}");
                return null;
            }
        }
    }
}