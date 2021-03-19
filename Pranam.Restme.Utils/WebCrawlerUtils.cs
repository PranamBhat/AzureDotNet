using System.Collections.Generic;
using System.Linq;

namespace Pranam
{
    public static class WebCrawlerUtils
    {
        public static bool IsCrawlerBot(string userAgent, int agentNameLengthDefaultAsBot = 5)
        {
            // crawlers that have 'bot' in their useragent
            List<string> Crawlers1 = new List<string>()
            {
                "googlebot", "bingbot", "yandexbot", "ahrefsbot", "msnbot", "linkedinbot", "exabot", "compspybot",
                "yesupbot", "paperlibot", "tweetmemebot", "semrushbot", "gigabot", "voilabot", "adsbot-google",
                "botlink", "alkalinebot", "araybot", "undrip bot", "borg-bot", "boxseabot", "yodaobot", "admedia bot",
                "ezooms.bot", "confuzzledbot", "coolbot", "internet cruiser robot", "yolinkbot", "diibot", "musobot",
                "dragonbot", "elfinbot", "wikiobot", "twitterbot", "contextad bot", "hambot", "iajabot", "news bot",
                "irobot", "socialradarbot", "ko_yappo_robot", "skimbot", "psbot", "rixbot", "seznambot", "careerbot",
                "simbot", "solbot", "mail.ru_bot", "spiderbot", "blekkobot", "bitlybot", "techbot", "void-bot",
                "vwbot_k", "diffbot", "friendfeedbot", "archive.org_bot", "woriobot", "crystalsemanticsbot", "wepbot",
                "spbot", "tweetedtimes bot", "mj12bot", "who.is bot", "psbot", "robot", "jbot", "bbot", "bot"
            };

// crawlers that don't have 'bot' in their useragent
            List<string> Crawlers2 = new List<string>()
            {
                "baiduspider", "80legs", "baidu", "yahoo! slurp", "ia_archiver", "mediapartners-google", "lwp-trivial",
                "nederland.zoek", "ahoy", "anthill", "appie", "arale", "araneo", "ariadne", "atn_worldwide", "atomz",
                "bjaaland", "ukonline", "bspider", "calif", "christcrawler", "combine", "cosmos", "cusco",
                "cyberspyder",
                "cydralspider", "digger", "grabber", "downloadexpress", "ecollector", "ebiness", "esculapio", "esther",
                "fastcrawler", "felix ide", "hamahakki", "kit-fireball", "fouineur", "freecrawl", "desertrealm",
                "gammaspider", "gcreep", "golem", "griffon", "gromit", "gulliver", "gulper", "whowhere",
                "portalbspider",
                "havindex", "hotwired", "htdig", "ingrid", "informant", "infospiders", "inspectorwww", "iron33",
                "jcrawler", "teoma", "ask jeeves", "jeeves", "image.kapsi.net", "kdd-explorer", "label-grabber",
                "larbin", "linkidator", "linkwalker", "lockon", "logo_gif_crawler", "marvin", "mattie", "mediafox",
                "merzscope", "nec-meshexplorer", "mindcrawler", "udmsearch", "moget", "motor", "muncher", "muninn",
                "muscatferret", "mwdsearch", "sharp-info-agent", "webmechanic", "netscoop", "newscan-online",
                "objectssearch", "orbsearch", "packrat", "pageboy", "parasite", "patric", "pegasus", "perlcrawler",
                "phpdig", "piltdownman", "pimptrain", "pjspider", "plumtreewebaccessor", "getterrobo-plus", "raven",
                "roadrunner", "robbie", "robocrawl", "robofox", "webbandit", "scooter", "search-au", "searchprocess",
                "senrigan", "shagseeker", "site valet", "skymob", "slcrawler", "slurp", "snooper", "speedy",
                "spider_monkey", "spiderline", "curl_image_client", "suke", "www.sygol.com", "tach_bw", "templeton",
                "titin", "topiclink", "udmsearch", "urlck", "valkyrie libwww-perl", "verticrawl", "victoria",
                "webscout", "voyager", "crawlpaper", "wapspider", "webcatcher", "t-h-u-n-d-e-r-s-t-o-n-e",
                "webmoose", "pagesinventory", "webquest", "webreaper", "webspider", "webwalker", "winona", "occam",
                "robi", "fdse", "jobo", "rhcs", "gazz", "dwcp", "yeti", "crawler", "fido", "wlm", "wolp", "wwwc",
                "xget",
                "legs", "curl", "webs", "wget", "sift", "cmc"
            };

            string ua = userAgent.ToLower();
            string match = null;

            if (ua.Contains("bot")) match = Crawlers1.FirstOrDefault(x => ua.Contains(x));
            else match = Crawlers2.FirstOrDefault(x => ua.Contains(x));

            if (agentNameLengthDefaultAsBot <= 0) agentNameLengthDefaultAsBot = 5;

            if (match != null && match.Length < agentNameLengthDefaultAsBot)
            {
                // OE.LogError("Possible new crawler found: ", ua);
                return true;
            }

            return match != null;
        }
    }
}