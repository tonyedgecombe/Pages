﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;

namespace Links
{
    internal class PageFinder
    {
        private readonly Uri _baseUri;
        private readonly string[] _ignored = {".png", ".jpg", ".exe"};

        private HashSet<string> _uris;

        private readonly HttpClient _client = new HttpClient();

        public PageFinder(Uri baseUri)
        {
            _baseUri = baseUri;
        }

        public IEnumerable<string> FindPages()
        {
            _uris = new HashSet<string>();
            FindPages(_baseUri);

            return _uris;
        }

        private void FindPages(Uri uri)
        {
            if (_uris.Contains(uri.AbsoluteUri))
            {
                // We have already processed this one
                return;
            }

            if (uri.Scheme != "http" && uri.Scheme != "https")
            {
                // Ignore other schemes such as mailto or file
                return;
            }

            if (!_baseUri.IsBaseOf(uri))
            {
                // Ignore off site links
                return;
            }

            if (_ignored.Any(e => uri.AbsolutePath.EndsWith(e)))
            {
                // Ignore ignored extensions
                return;
            }

            var task = _client.GetAsync(uri);
            var result = task.Result;

            if (!result.IsSuccessStatusCode)
            {
                // 404, etc
                return;
            }

            _uris.Add(uri.ToString());

            FindPages(result.Content);
        }

        private void FindPages(HttpContent content)
        {
            var contentType = content.Headers.ContentType;
            if (contentType.MediaType != "text/html")
            {
                return;
            }

            var streamTask = content.ReadAsStreamAsync();
            var document = new HtmlDocument();
            document.Load(streamTask.Result);

            var uris = document.DocumentNode
                .SelectNodes("//a[@href]")
                .Select(node => node.Attributes["href"].Value)
                .Select(uriString => new Uri(uriString, UriKind.RelativeOrAbsolute));

            foreach (var uri in uris)
            {
                FindPages(uri.IsAbsoluteUri ? uri : new Uri(_baseUri, uri));
            }
        }
    }
}