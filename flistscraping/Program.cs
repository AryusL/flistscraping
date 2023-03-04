using flistscraping;
using HtmlAgilityPack;

var htmlSource = @"<html lang=""en""><head></head><body>
<div class=""status-looking""><img src=""https://static.f-list.net/images/avatar/charlie firetail.png""><span class=""user-view gender-male""><span class=""fa-fw fa fa-eye""></span>Charlie Firetail</span><span text=""Just chat for tonight. Why don't you come by so I can tease ya?~""><span class=""bbcode"">Just chat for tonight. Why don't you come by so I can tease ya?~</span></span></div>
<div class=""status-looking""><img src=""https://static.f-list.net/images/avatar/eldrik.png""><span class=""user-view gender-male""><span class=""fa-fw fa fa-eye""></span>Eldrik</span><span text=""""><span></span></span></div>
</body></html>";
// Scrape the character URLs from the HTML source
var characterUrls = ScrapeCharacterUrls(htmlSource);

// Create a list to store the CharacterInfo objects
var characters = new List<CharacterInfo>();

// Loop through each character URL and scrape its information
foreach (var url in characterUrls)
{
    var profileScraping = new ProfileScraping(url);
    profileScraping.ScrapeCharacterInfoSideBar();
    profileScraping.ScrapeCharacterRpInfo();
    profileScraping.ScrapeCharacterKink();
    characters.Add(profileScraping.CharacterInfo);
}

// Output the scraped character information to the console
foreach (var character in characters)
{
    character.DisplayInfos();
    Console.WriteLine("");
    Console.WriteLine("######################");
    Console.WriteLine("######################");
    Console.WriteLine("");
}

static List<string> ScrapeCharacterUrls(string htmlSource)
{
    var characterUrls = new List<string>();

    var doc = new HtmlDocument();
    doc.LoadHtml(htmlSource);

    var userViews = doc.DocumentNode.SelectNodes("//span[contains(@class, 'user-view')]");
    if (userViews != null)
    {
        foreach (var userView in userViews)
        {
            var characterName = userView.InnerText;
            characterName = characterName.Replace(" ", "%20");
            var characterUrl = $"https://www.f-list.net/c/{characterName}";
            characterUrls.Add(characterUrl);
        }
    }

    return characterUrls;
}
