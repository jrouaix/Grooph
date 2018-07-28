using CsvHelper;
using CsvHelper.Configuration;
using Grooph.Model;
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
            var graph = new Graph();

            var titles = GetTITLEs()
                .Where(t => t.isAdult == 0)
                .Where(t => int.TryParse(t.startYear, out var year) && year >= 1981 && year <= 1990)
                .Where(t => t.titleType == "movie" && t.isAdult == 0)
                //.Take(100000)
                .Select(t => t.GetTitleObject())
                ;

            var count = 0;
            foreach (var title in titles) { count++; graph.UpsertVertex(title.Id, title); }
            _output.WriteLine($"{count} titles.");

            var names = GetNAMEs()
                .Where(n => int.TryParse(n.birthYear, out var year) && year <= 1990)
                .Where(n => n.Famousness > 2)
                //.Take(100000)
                .Select(n => n.GetNameObject())
                ;

            count = 0;
            foreach (var name in names) { count++; graph.UpsertVertex(name.Id, name); }
            _output.WriteLine($"{count} names.");

            var principals = GetPRINCIPALS()
                //.Take(1000)
                ;

            count = 0;
            foreach (var cast in principals)
            {
                var title = graph.GetVertex<Title>(cast.tconst);
                if (title == null) continue;

                var actor = graph.GetVertex<Actor>(cast.nconst);
                if (actor == null) continue;

                var playedIn = cast.GetPlayedIn();
                var hasCast = cast.GetHasCast();

                count++;
                graph.UpsertEdge<PlayedIn, Actor, Title>(playedIn.Id, playedIn, actor.Id, title.Id);
                graph.UpsertEdge<HasCast, Title, Actor>(hasCast.Id, hasCast, title.Id, actor.Id);
            }
            _output.WriteLine($"{count} relations.");


        }

        [Fact]
        public void SomeStats()
        {
            void incrementKey(Dictionary<string, int> dico, string key)
            {
                if (dico.TryGetValue(key, out var value))
                    dico[key] = value + 1;
                else
                    dico[key] = 1;
            }

            var titlesCountByYear = new Dictionary<string, int>();
            var titleByType = new Dictionary<string, int>();
            var titleByAdult = new Dictionary<string, int>();

            foreach (var t in GetTITLEs())
            { 
                incrementKey(titlesCountByYear, t.startYear);
                incrementKey(titleByType, t.titleType);
                incrementKey(titleByAdult, t.isAdult == 0 ? "nope" : "yeah!");
            }

            _output.WriteLine(Environment.NewLine + "Adult movie or not ?");
            foreach (var item in titleByAdult.OrderBy(kv => kv.Key)) _output.WriteLine($"    {item.Key} : {item.Value}");
            _output.WriteLine(Environment.NewLine + "Movies count by type :");
            foreach (var item in titleByType.OrderBy(kv => kv.Key)) _output.WriteLine($"    {item.Key} : {item.Value}");
            _output.WriteLine(Environment.NewLine + "Movies count by year :");
            foreach (var item in titlesCountByYear.OrderBy(kv => kv.Key)) _output.WriteLine($"    {item.Key} : {item.Value}");

            var actorsByHowFamous = new Dictionary<string, int>();
            foreach (var n in GetNAMEs())
            {
                incrementKey(actorsByHowFamous, n.Famousness.ToString());
            }

            _output.WriteLine(Environment.NewLine + "Actors by 'famousness' :");
            foreach (var item in actorsByHowFamous.OrderBy(kv => kv.Key)) _output.WriteLine($"    {item.Key} : {item.Value}");
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

        private class Title
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int RuntimeMinutes { get; set; }
            public string[] Genres { get; set; }
            public int Year { get; set; }
        }

        private class PlayedIn
        {
            public string Id { get; set; }
            public string Category { get; internal set; }
            public string Role { get; set; }
        }

        private class HasCast
        {
            public string Id { get; set; }
            public string Category { get; internal set; }
            public string Role { get; set; }
        }

        private class Actor
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int BirthYear { get; set; }
            public int? DeathYear { get; set; }
        }

        IEnumerable<TITLE> GetTITLEs() => GetRecords<TITLE>(TITLES_FILE);
        IEnumerable<PRINCIPALS> GetPRINCIPALS() => GetRecords<PRINCIPALS>(PRINCIPALS_FILE);
        IEnumerable<NAME> GetNAMEs() => GetRecords<NAME>(NAMES_FILE);

        private IEnumerable<TRecord> GetRecords<TRecord>(string file)
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

        private class TITLE
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

            public Title GetTitleObject()
            {
                return new Title
                {
                    Id = tconst,
                    Name = originalTitle,
                    Year = int.TryParse(startYear, out var year) ? year : 0,
                    RuntimeMinutes = int.TryParse(runtimeMinutes, out var lenth) ? lenth : 0,
                    Genres = genres?.Split(",", StringSplitOptions.RemoveEmptyEntries),
                };
            }
        }

        private class PRINCIPALS
        {
            public string tconst { get; set; }
            public string ordering { get; set; }
            public string nconst { get; set; }
            public string category { get; set; }
            public string job { get; set; }
            public string characters { get; set; }

            string GetRoleName() => this.characters
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Where(s => s != @"\N")
                .FirstOrDefault();

            public PlayedIn GetPlayedIn()
            {
                var id = $"{nconst} played in {tconst}";

                return new PlayedIn
                {
                    Id = id,
                    Category = this.category,
                    Role = GetRoleName(),
                };
            }

            public HasCast GetHasCast()
            {
                var id = $"{tconst} has in cast {nconst}";

                return new HasCast
                {
                    Id = id,
                    Category = this.category,
                    Role = GetRoleName(),
                };
            }
        }

        private class NAME
        {
            public string nconst { get; set; }
            public string primaryName { get; set; }
            public string birthYear { get; set; }
            public string deathYear { get; set; }
            public string primaryProfession { get; set; }
            public string knownForTitles { get; set; }

            public Actor GetNameObject()
            {
                return new Actor
                {
                    Id = nconst,
                    Name = primaryName,
                    BirthYear = int.TryParse(birthYear, out var year) ? year : 0,
                    DeathYear = int.TryParse(deathYear, out var death) ? death : 0,
                };
            }

            public int Famousness => knownForTitles?.Count(c => c == ',') ?? 0;
        }
    }
}
