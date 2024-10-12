using LinkCrawler.Utils;
using StructureMap;
using System;
using LinkCrawler.Utils.Parsers;
using LinkCrawler.Utils.Settings;
using CommandLine;

namespace LinkCrawler
{
    class Program
    {
        public class Options
        {
            [Value(0, Required = true, MetaName = "BaseUrl")]
            public string BaseUrl {  get; set; }
            [Option("check-images", Default = false)]
            public bool CheckImages { get; set; }
            [Option("only-broken-links", Default = false)]
            public bool OnlyBrokenLinks { get; set; }
            [Option("dont-print-summary", Default = false)]
            public bool DontPrintSummary { get; set; }
        }
        static void Main(string[] args)
        {
            
            using (var container = Container.For<StructureMapRegistry>())
            {
                var linkCrawler = container.GetInstance<LinkCrawler>();
                Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(opts =>
                {
                    string parsed;
                    var validUrlParser = new ValidUrlParser(new Settings());
                    linkCrawler.CheckImages = opts.CheckImages;
                    linkCrawler.OnlyReportBrokenLinksToOutput = opts.OnlyBrokenLinks;
                    linkCrawler.PrintSummary = !opts.DontPrintSummary;
                    var result = validUrlParser.Parse(opts.BaseUrl, out parsed);
                    if (result)
                        linkCrawler.BaseUrl = parsed;
                    linkCrawler.Start();
                    Console.Read();
                }
                ).WithNotParsed<Options>(errs => { errs.Output();  });
            }
        }
    }
}
