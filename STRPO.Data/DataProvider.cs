using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using STRPO.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.Web.Http;

namespace STRPO.Data
{
    public class DataProvider
    {
        const string appId = "AIzaSyDrtHTpueOMbDKVmeajk0LqvEgEA2ayShk";
        private const string SEPARATORS = @" _.,\/|()";

        public async Task<IEnumerable<FlatVideoData>> GetVideoDataAsync(string videoId = "Ks-_Mh1QhMc")
        {
            var videos = new List<Video>();

            videos
                .AddRange((await LoadVideosByCategoryId(2))
                .Select(vid => {
                    vid.Mark = Colors.Green;
                    return vid;
                }));
            videos
                .AddRange((await LoadVideosByCategoryId(10))
                .Select(vid => {
                    vid.Mark = Colors.Red;
                    return vid;
                }));
            videos
                .AddRange((await LoadVideosByCategoryId(15))
                .Select(vid => {
                    vid.Mark = Colors.Blue;
                    return vid;
                }));

            return videos.Select(video =>
                    new FlatVideoData
                    {
                        CategoryId = video.Snippet.CategoryId,
                        ChannelId = video.Snippet.ChannelId,
                        ChannelTitle = video.Snippet.ChannelTitle,
                        //CommentCount = video.Statistics.CommentCount,
                        //DislikeCount = video.Statistics.DislikeCount,
                        //FavoriteCount = video.Statistics.FavoriteCount,
                        //LikeCount = video.Statistics.LikeCount,
                        //ViewCount = video.Statistics.ViewCount,
                        DefaultAudioLanguage = video.Snippet.DefaultAudioLanguage,
                        DefaultLanguage = video.Snippet.DefaultLanguage,
                        //Description = video.Snippet.Description.Split(SEPARATORS.ToArray(), StringSplitOptions.RemoveEmptyEntries),
                        LiveBroadcastContent = video.Snippet.LiveBroadcastContent,
                        Tags = video.Snippet.Tags,

                        PublishedAtYear = video.Snippet.PublishedAt.Year,
                        PublishedAtMonth = video.Snippet.PublishedAt.Month,
                        PublishedAtDay = video.Snippet.PublishedAt.Day,
                        PublishedAtHour = video.Snippet.PublishedAt.Hour,

                        Title = video.Snippet.Title,
                        Mark = video.Mark
                        //Title = video.Snippet.Title.Split(SEPARATORS.ToArray(), StringSplitOptions.RemoveEmptyEntries),
                    }
                );
        }

        private static async Task<IEnumerable<Video>> LoadVideosByCategoryId(int videoCategoryId)
        {
            var videos = new List<Video>();
            using (var client = new HttpClient())
            {
                string nextPageToken = null;

                do
                {
                    string uriString = $"https://www.googleapis.com/youtube/v3/videos/?part=snippet%2CcontentDetails%2Cstatistics&chart=mostPopular&videoCategoryId={videoCategoryId}&regionCode=US&key={appId}";

                    if (!string.IsNullOrEmpty(nextPageToken))
                    {
                        uriString = $"https://www.googleapis.com/youtube/v3/videos/?part=snippet%2CcontentDetails%2Cstatistics&chart=mostPopular&videoCategoryId={videoCategoryId}&pageToken={nextPageToken}&regionCode=US&key={appId}";
                    }

                    var reponse = await client.GetAsync(
                        new Uri(uriString)
                    );

                    if (!reponse.IsSuccessStatusCode)
                    {
                        break;
                    }

                    var root = JsonConvert.DeserializeObject(await reponse.Content.ReadAsStringAsync()) as JObject;

                    nextPageToken = root["nextPageToken"]?.ToString();

                    videos.AddRange(root["items"].Select(
                        item =>
                        {
                            var statistics = item["statistics"];
                            var snippet = item["snippet"];

                            return new Video
                            {
                                Snippet = new Snippet
                                {
                                    CategoryId = snippet["categoryId"]?.ToString(),
                                    ChannelTitle = snippet["channelTitle"]?.ToString(),
                                    ChannelId = snippet["channelId"]?.ToString(),
                                    DefaultAudioLanguage = snippet["defaultAudioLanguage"]?.ToString(),
                                    DefaultLanguage = snippet["defaultLanguage"]?.ToString(),
                                    Description = snippet["description"]?.ToString(),
                                    LiveBroadcastContent = snippet["liveBroadcastContent"]?.ToString(),
                                    PublishedAt = Convert.ToDateTime(snippet["publishedAt"]?.ToString()),
                                    Tags = snippet["tags"]?.Select(tag => tag?.ToString()).ToList() ?? Enumerable.Empty<string>(),
                                    Title = snippet["title"]?.ToString()
                                },
                                ContentDetails = { },
                                Statistics = new Statistics
                                {
                                    ViewCount = Convert.ToUInt64(statistics["viewCount"]?.ToString()),
                                    CommentCount = Convert.ToUInt64(statistics["commentCount"]?.ToString()),
                                    DislikeCount = Convert.ToUInt64(statistics["dislikeCount"]?.ToString()),
                                    FavoriteCount = Convert.ToUInt64(statistics["favoriteCount"]?.ToString()),
                                    LikeCount = Convert.ToUInt64(statistics["likeCount"]?.ToString()),
                                }
                            };
                        }
                    ));
                }
                while (!string.IsNullOrEmpty(nextPageToken));
            }

            return videos;
        }
    }
}
