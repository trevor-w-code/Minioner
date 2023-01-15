using Discord;
using Google.Apis.Customsearch.v1;
using Google.Apis.Customsearch.v1.Data;
using Minioner.Common;
using System;
using static Google.Apis.Customsearch.v1.CseResource.SiterestrictResource.ListRequest;

namespace Minioner.Services
{
    public static class GoogleService
    {
        /// <summary>
        /// Returns metadata about the search performed, metadata about the custom search engine used for the search, and the search results. 
        /// Documentation https://developers.google.com/customsearch/v1/reference/cse/list
        /// Generation Note: This does not always build corectly.  Google needs to standardise things I need to figuer out which ones are wrong.
        /// </summary>
        /// <param name="service">Authenticated Customsearch service.</param>  
        /// <param name="q">Query</param>
        /// <param name="optional">Optional paramaters.</param>
        /// <returns>SearchResponse</returns>
        public static Search List(CustomsearchService service, string q)
        {
            try
            {
                // Building the initial request.
                var request = service.Cse.List();
                request.Q = q;
                request.Cx = Globals.GoogleSearchEngineID;
                request.SearchType = (CseResource.ListRequest.SearchTypeEnum?)1;
                request.Safe = CseResource.ListRequest.SafeEnum.Active;
                request.AccessToken = Globals.GoogleToken;
                return request.Execute();
            }
            catch (Exception ex)
            {
                Logger.Log(LogSeverity.Error, $"{nameof(GoogleService)}", ex.Message);
                throw;
            }
        }

    }
}