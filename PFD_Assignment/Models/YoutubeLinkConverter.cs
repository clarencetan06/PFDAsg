namespace PFD_Assignment.Models
{
    using System.Text.RegularExpressions;

    public static class YoutubeLinkConverter
    {
        public static string ConvertYoutubeLinkToEmbed(string youtubeLink)
        {
            // Extract video ID from the YouTube link
            var match = Regex.Match(youtubeLink, @"(?:https?:\/\/)?(?:www\.)?(?:youtube\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|\S*?[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})");

            if (match.Success)
            {
                // Construct the embed code
                return $"https://www.youtube.com/embed/{match.Groups[1].Value}";
            }

            // Return original link if no match
            return youtubeLink;
        }
    }
}