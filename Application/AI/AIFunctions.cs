using System.Data;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using CsvHelper;
using MathNet.Numerics.Statistics;
using Microsoft.AspNetCore.Http;

namespace Application.AI
{
    public class AIFunctions
    {
        public int IndividualHash(string name, int vectorSize)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Error calculating individual hash: Name parameter cannot be null or empty", nameof(name));
            }

            if (vectorSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(vectorSize), "Error calculating individual hash: Vector size should be greater than 0");
            }

            try
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(name);
                    byte[] hash = md5.ComputeHash(bytes);
                    int value = BitConverter.ToInt32(hash, 0);
                    int index = Math.Abs(value) % vectorSize;
                    return index;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating individual hash: {ex.Message}");
                throw;
            }

        }

        public double[][] ReadDataIF(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "File path was not provided");
            }

            if (!System.IO.File.Exists(filePath))
            {
                throw new ArgumentException($"File does not exist: {filePath}", nameof(filePath));
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            if (fileBytes.Length == 0)
            {
                throw new ArgumentException("Training data file is empty", nameof(filePath));
            }

            return ProcessFileBytes(fileBytes);
        }

        public double[][] ReadDataIF(IFormFile uploadedFormFile)
        {
            if (uploadedFormFile == null)
            {
                throw new ArgumentNullException(nameof(uploadedFormFile), "Training file was not provided");
            }

            if (uploadedFormFile.Length == 0)
            {
                throw new ArgumentException("Training file is empty", nameof(uploadedFormFile));
            }

            byte[] fileBytes;

            using (Stream uploadedStream = uploadedFormFile.OpenReadStream())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    uploadedStream.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }
            }

            if (fileBytes.Length == 0)
            {
                throw new ArgumentException("Training data stream is empty", nameof(fileBytes));
            }

            return ProcessFileBytes(fileBytes);
        }

        private double[][] ProcessFileBytes(byte[] fileBytes)
        {
            using (MemoryStream memoryStreamForRecords = new MemoryStream(fileBytes))
            {
                var records = ReadCsvRecordsIF(memoryStreamForRecords);

                using (MemoryStream memoryStreamForHeaders = new MemoryStream(fileBytes))
                {
                    var headers = GetCsvHeadersIF(memoryStreamForHeaders);

                    if (records == null || headers == null)
                    {
                        throw new InvalidOperationException("Failed to read CSV records or headers");
                    }

                    double[][] X = EncodeData(records, headers);

                    if (X == null)
                    {
                        throw new InvalidOperationException("Failed to encode data");
                    }

                    //X = NormalizeMatrix(X);

                    //if (X == null)
                    //{
                    //    throw new InvalidOperationException("Failed to normalize matrix");
                    //}

                    return X;
                }
            }
        }

        private List<dynamic> ReadCsvRecordsIF(MemoryStream uploadedStream)
        {
            try
            {
                using var reader = new StreamReader(uploadedStream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                return csv.GetRecords<dynamic>().ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading CSV records: {ex.Message}");
                throw;
            }
        }

        private string[] GetCsvHeadersIF(MemoryStream uploadedStream)
        {
            try
            {
                using var reader = new StreamReader(uploadedStream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                csv.Read();
                csv.ReadHeader();

                return csv.HeaderRecord;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading CSV headers: {ex.Message}");
                throw;
            }
        }

        public double[][] EncodeData(List<dynamic> records, string[] headers)
        {
            if (records == null || headers == null)
            {
                throw new ArgumentNullException("Error encoding data: Records and headers cannot be null");
            }

            int recordCount = records.Count;
            int headerCount = headers.Length;

            double[][] X = new double[recordCount][];

            for (int i = 0; i < recordCount; i++)
            {
                X[i] = new double[headerCount];
            }

            for (int j = 0; j < headerCount; j++)
            {
                bool numeric = records.All(record =>
                {
                    var recordDict = (IDictionary<string, object>)record;
                    return double.TryParse(Convert.ToString(recordDict[headers[j]]), out double _);
                });

                for (int i = 0; i < recordCount; i++)
                {
                    var record = records[i];
                    var propertyValue = ((IDictionary<string, object>)record)[headers[j]].ToString();
                    X[i][j] = numeric ? double.Parse(propertyValue) : IndividualHash(propertyValue, 100000);
                }
            }

            return X;
        }

        private double[][] NormalizeMatrix(double[][] X)
        {
            if (X == null)
            {
                throw new ArgumentNullException("Error normalizing matrix: Matrix X cannot be null");
            }

            int rowCount = X.Length;
            int columnCount = X[0].Length;

            double[] mean = new double[columnCount];
            double[] stdDev = new double[columnCount];

            for (int j = 0; j < columnCount; j++)
            {
                double[] columnValues = new double[rowCount];

                for (int i = 0; i < rowCount; i++)
                {
                    columnValues[i] = X[i][j];
                }

                mean[j] = columnValues.Mean();
                stdDev[j] = columnValues.StandardDeviation();
            }

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    X[i][j] = stdDev[j] != 0 ? (X[i][j] - mean[j]) / stdDev[j] : 1;
                }
            }

            return X;
        }

    }
}