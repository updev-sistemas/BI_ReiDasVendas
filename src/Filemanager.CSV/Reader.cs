using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace UpDEV.BI.ReiDasVendas.Infrastructures.Filemanager.CSV
{
    public class Reader
    {
        public IEnumerable<MagaluModel> Build(string filepath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
            };

            using var reader = new StreamReader(filepath);
            using var csv = new CsvReader(reader, config);

            var atendimentos = csv.GetRecords<MagaluModel>().ToArray();

            return atendimentos;
        }
    }
}