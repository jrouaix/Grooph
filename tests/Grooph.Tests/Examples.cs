using CsvHelper;
using CsvHelper.Configuration;
using SharpCompress.Archives;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Grooph.Tests
{
    public class Examples
    {
        //http://www.imdb.com/interfaces/ 
        //https://datasets.imdbws.com/
        const string TITLES_FILE = "title.basics.tsv";
        const string PRINCIPALS_FILE = "title.principals.tsv";
        const string NAMES_FILE = "name.basics.tsv";
        static readonly Configuration IMDB_CSV_CONF = new Configuration { HasHeaderRecord = true, Delimiter = "\t", BadDataFound = null, MissingFieldFound = null };

        private readonly ITestOutputHelper _output;

        public Examples(ITestOutputHelper output)
        {
            _output = output;

            foreach (var file in new[] { TITLES_FILE, PRINCIPALS_FILE, NAMES_FILE }.Where(f => !File.Exists(f)))
            {
                DownloadImdbDataSet($"https://datasets.imdbws.com/{file}.gz", file).Wait();
            }
        }

        [Fact]
        public async Task Test()
        {
            //var results = GetTitles()
            //    .GroupBy(k => k.startYear)
            //    .Select(g => new { g.Key, count = g.Count() })
            //    .OrderBy(g => g.Key)
            //    ;

            //foreach (var item in results)
            //{
            //    _output.WriteLine($"{item.Key} : {item.count}");
            //}
        }



        private async Task DownloadImdbDataSet(string uri, string fileName)
        {
            var gzfileName = fileName + ".gz";

            using (var http = new HttpClient())
            using (var dlStream = await http.GetStreamAsync(uri).ConfigureAwait(false))
            using (var toStream = File.Open(gzfileName, FileMode.Create, FileAccess.Write))
            {
                await dlStream.CopyToAsync(toStream).ConfigureAwait(false);
            }

            using (var gz = ArchiveFactory.Open(fileName + ".gz"))
            using (var entryStream = gz.Entries.First().OpenEntryStream())
            using (var toStream = File.Open(fileName, FileMode.Create, FileAccess.Write))
            {
                await entryStream.CopyToAsync(toStream).ConfigureAwait(false);
            }
        }

        IEnumerable<Title> GetTitles() => GetRecords<Title>(TITLES_FILE);
        IEnumerable<Title> GetPrincipals() => GetRecords<Title>(PRINCIPALS_FILE);
        IEnumerable<Title> GetNames() => GetRecords<Title>(NAMES_FILE);


        IEnumerable<TRecord> GetRecords<TRecord>(string file)
            where TRecord : new()
        {
            using (var fileStream = File.OpenRead(file))
            using (var textStream = new StreamReader(fileStream))
            using (var csv = new CsvReader(textStream, IMDB_CSV_CONF))
            {
                foreach (var item in csv.EnumerateRecords(new TRecord()))
                    yield return item;
            }
        }

        class Title
        {
            public string tconst { get; set; }
            public string titleType { get; set; }
            public string primaryTitle { get; set; }
            public string originalTitle { get; set; }
            public int isAdult { get; set; }
            public string startYear { get; set; }
            public string endYear { get; set; }
            public string runtimeMinutes { get; set; }
            public string genres { get; set; }
        }

        class Principals
        {
            public string tconst { get; set; }
            public string principalCast { get; set; }
        }

        class Name
        {
            public string nconst { get; set; }
            public string primaryName { get; set; }
            public string birthYear { get; set; }
            public string deathYear { get; set; }
            public string primaryProfession { get; set; }
            public string knownForTitles { get; set; }
        }
    }
}
