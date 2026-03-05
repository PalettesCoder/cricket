using System.Text.Json.Serialization;

namespace HRCricket.Api.Models
{
    public class CricApiResponse
    {
        [JsonPropertyName("apikey")]
        public string? ApiKey { get; set; }
        
        [JsonPropertyName("data")]
        public List<CricApiMatch>? Data { get; set; }
        
        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    public class CricApiMatch
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        
        [JsonPropertyName("matchType")]
        public string? MatchType { get; set; }
        
        [JsonPropertyName("status")]
        public string? Status { get; set; }
        
        [JsonPropertyName("venue")]
        public string? Venue { get; set; }
        
        [JsonPropertyName("date")]
        public string? Date { get; set; }
        
        [JsonPropertyName("teams")]
        public List<string>? Teams { get; set; }
        
        [JsonPropertyName("score")]
        public List<CricApiScore>? Score { get; set; }
        
        [JsonPropertyName("ms")]
        public string? MatchStatus { get; set; }
    }

    public class CricApiScore
    {
        [JsonPropertyName("r")]
        public int Runs { get; set; }
        
        [JsonPropertyName("w")]
        public int Wickets { get; set; }
        
        [JsonPropertyName("o")]
        public double Overs { get; set; }
        
        [JsonPropertyName("inning")]
        public string? Inning { get; set; }
    }

    public class CricApiSeriesResponse
    {
        [JsonPropertyName("apikey")]
        public string? ApiKey { get; set; }
        
        [JsonPropertyName("data")]
        public List<CricApiSeries>? Data { get; set; }
        
        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    public class CricApiSeries
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        
        [JsonPropertyName("startDate")]
        public string? StartDate { get; set; }
        
        [JsonPropertyName("endDate")]
        public string? EndDate { get; set; }
        
        [JsonPropertyName("matches")]
        public int Matches { get; set; }
        
        [JsonPropertyName("t20")]
        public int T20 { get; set; }
        
        [JsonPropertyName("odi")]
        public int Odi { get; set; }
        
        [JsonPropertyName("test")]
        public int Test { get; set; }
    }

    public class CricApiPlayerResponse
    {
        [JsonPropertyName("apikey")]
        public string? ApiKey { get; set; }
        
        [JsonPropertyName("data")]
        public List<CricApiPlayer>? Data { get; set; }
        
        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    public class CricApiPlayer
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        
        [JsonPropertyName("country")]
        public string? Country { get; set; }
    }

    public class CricApiSeriesInfoResponse
    {
        [JsonPropertyName("apikey")]
        public string? ApiKey { get; set; }
        
        [JsonPropertyName("data")]
        public CricApiSeriesInfo? Data { get; set; }
        
        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    public class CricApiSeriesInfo
    {
        [JsonPropertyName("info")]
        public CricApiSeries? Info { get; set; }
        
        [JsonPropertyName("matchList")]
        public List<CricApiMatch>? MatchList { get; set; }
    }
    public class CricApiMatchInfoResponse
    {
        [JsonPropertyName("apikey")]
        public string? ApiKey { get; set; }
        
        [JsonPropertyName("data")]
        public CricApiMatch? Data { get; set; }
        
        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    public class CricScoreResponse
    {
        [JsonPropertyName("apikey")]
        public string? ApiKey { get; set; }
        
        [JsonPropertyName("data")]
        public List<CricScoreData>? Data { get; set; }
        
        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    public class CricScoreData
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        
        [JsonPropertyName("dateTimeGMT")]
        public string? DateTimeGMT { get; set; }
        
        [JsonPropertyName("matchType")]
        public string? MatchType { get; set; }
        
        [JsonPropertyName("status")]
        public string? Status { get; set; }
        
        [JsonPropertyName("ms")]
        public string? MatchStatus { get; set; }
        
        [JsonPropertyName("t1")]
        public string? Team1 { get; set; }
        
        [JsonPropertyName("t2")]
        public string? Team2 { get; set; }
        
        [JsonPropertyName("t1s")]
        public string? Score1 { get; set; }
        
        [JsonPropertyName("t2s")]
        public string? Score2 { get; set; }
        
        [JsonPropertyName("t1img")]
        public string? Team1Img { get; set; }
        
        [JsonPropertyName("t2img")]
        public string? Team2Img { get; set; }
    }

    public class CricApiNewsResponse
    {
        [JsonPropertyName("apikey")]
        public string? ApiKey { get; set; }
        
        [JsonPropertyName("data")]
        public List<CricApiNews>? Data { get; set; }
        
        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    public class CricApiNews
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        
        [JsonPropertyName("title")]
        public string? Title { get; set; }
        
        [JsonPropertyName("story")]
        public string? Story { get; set; }
        
        [JsonPropertyName("image")]
        public string? Image { get; set; }
        
        [JsonPropertyName("pubTime")]
        public string? PubTime { get; set; }
    }
}
