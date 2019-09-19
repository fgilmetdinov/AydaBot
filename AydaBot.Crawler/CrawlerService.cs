using System;
using System.Threading;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Generic;
using AydaBot.Common.Models;

namespace AydaBot.Notifier
{
    public class CrawlerService
    {
        private readonly Common.Abstraction.IStorage _storage;
        private readonly ILogger<CrawlerService> _logger;

        public CrawlerService(Common.Abstraction.IStorage storage, ILogger<CrawlerService> logger)
        {
            _storage = storage;
            _logger = logger;
        }

        public void Run()
        {
            var currentDate = DateTime.Now.Date.AddDays(-1);
            while (true)
            {
                if (DateTime.Now.Date != currentDate)
                {
                    currentDate = DateTime.Now.Date;
                    Process();
                    _logger.LogInformation("CrawlerService processed.");
                }
                Thread.Sleep(new TimeSpan(1, 0, 0));
            }
        }

        private void Process()
        {
            //_storage.SaveSerialMessage(new List<SerialMessage> { new SerialMessage()});
            //return; 

            var dt = DateTime.Now.Date;
            var url = $@"https://serialdata.ru/calendar/{dt.Year}/{dt.Month}/{dt.ToString("dd.MM.yyyy")}";
            var web = new HtmlWeb();
            var doc = web.Load(url);
            var messages = new List<SerialMessage>();
            foreach (var table in doc.DocumentNode.SelectNodes("//table[@rules='none']"))
            {
                try
                {
                    var td = table.Descendants("td").Skip(1).FirstOrDefault();
                    var ps = td.SelectNodes("p").ToList();
                    if (ps.Count != 2)
                        continue;
                    var p1 = ps[0].Descendants("span").ToList();
                    var p2 = ps[1].ChildNodes.Where(x => x.Name == "#text").ToList();

                    var serial1 = p1[0].InnerText.Trim();
                    var serial2 = p1.Count > 1 ? p1[1].InnerText.Trim() : "";                    
                    var sezon = p2[1].InnerText.Trim();
                    var episod = p2[3].InnerText.Trim();
                    var name = p2[5].InnerText.Trim();

                    messages.Add(new SerialMessage
                    {
                        SerialName = serial1,
                        SerialName2 = serial2,
                        Date = dt,
                        Message = $"{serial1} ({serial2}) Сезон:{sezon}, эпизод:{episod}, название:{name}"
                    });
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, table.InnerText);
                }
            }

            if (messages.Count > 0)
            {
                try
                {
                    _storage.SaveSerialMessage(messages);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Database error");
                }
            }
        }
    }
}
