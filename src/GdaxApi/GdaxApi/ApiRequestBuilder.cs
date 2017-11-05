﻿namespace GdaxApi
{
    using System;
    using System.Net.Http;
    using System.Text;

    public abstract class ApiRequestBuilder<T>
    {
        private static readonly Uri BaseUri = new Uri("https://api.gdax.com/");
        private readonly GdaxApiClient api;
        private readonly UriBuilder uriBuilder;
        private string content;

        public ApiRequestBuilder(GdaxApiClient api, string path)
        {
            this.api = api;
            this.uriBuilder = new UriBuilder(BaseUri)
            {
                Path = path
            };
        }

        protected GdaxApiClient Api => this.api;

        public ApiRequestBuilder<T> Content(object content)
        {
            this.content = this.api.Serializer.Serialize(content);
            return this;
        }

        public ApiRequestBuilder<T> AddQueryParam(string name, object value)
        {
            if (this.uriBuilder.Query != null)
            {
                this.uriBuilder.Query += $"&{name}={value}";
            }
            else
            {
                this.uriBuilder.Query = $"?{name}={value}";
            }

            return this;
        }

        public ApiRequest<T> Build()
        {
            var req = Create(this.uriBuilder.Uri);
            req.Content = new StringContent(this.content, Encoding.UTF8, "application/json");
            req.Headers.Add("User-Agent", "TraderBot");
            return req;
        }

        protected abstract ApiRequest<T> Create(Uri uri);
    }

    public class ApiGetRequestBuilder<T> : ApiRequestBuilder<T>
    {
        public ApiGetRequestBuilder(GdaxApiClient api, string path)
            : base(api, path)
        {
        }

        protected override ApiRequest<T> Create(Uri uri)
        {
            var req = new ApiRequest<T>(this.Api, HttpMethod.Get, uri);
            req.Headers.Add("Accept", "application/json");
            return req;
        }
    }

    public class ApiPostRequestBuilder<T> : ApiRequestBuilder<T>
    {
        public ApiPostRequestBuilder(GdaxApiClient api, string path)
            : base(api, path)
        {
        }

        protected override ApiRequest<T> Create(Uri uri)
        {
            var req = new ApiRequest<T>(this.Api, HttpMethod.Post, uri);
            req.Headers.Add("Accept", "application/json");
            return req;
        }
    }

    public class ApiDeleteRequestBuilder<T> : ApiRequestBuilder<T>
    {
        public ApiDeleteRequestBuilder(GdaxApiClient api, string path)
            : base(api, path)
        {
        }

        protected override ApiRequest<T> Create(Uri uri)
        {
            var req = new ApiRequest<T>(this.Api, HttpMethod.Delete, uri);
            req.Headers.Add("Accept", "application/json");
            return req;
        }
    }
}
