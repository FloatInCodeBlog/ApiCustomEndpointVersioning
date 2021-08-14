using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using System;
using System.Linq;

namespace ApiVersioning
{
    public class HighestUpToSpecifiedIncludedVersionSelector : IApiVersionSelector
    {
        private readonly ApiVersion _defaultVersion;

        public HighestUpToSpecifiedIncludedVersionSelector(ApiVersion defaultVersion)
        {
            _defaultVersion = defaultVersion;
        }

        public ApiVersion SelectVersion(HttpRequest request, ApiVersionModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var requestedVersion = ParseVersion(request) ?? _defaultVersion;

            var selectedVersion = model.ImplementedApiVersions.Count switch
            {
                0 => _defaultVersion,
                1 => model.ImplementedApiVersions[0].Status == null ? model.ImplementedApiVersions[0] : _defaultVersion,
                _ => model.ImplementedApiVersions
                   .Where(v => v.Status == null && v <= requestedVersion)
                   .Max(v => v) ?? _defaultVersion
            };

            return selectedVersion;
        }

        private static ApiVersion ParseVersion(HttpRequest request)
        {
            if (!request.Headers.TryGetValue("api-version", out var version))
                return null;

            if (string.IsNullOrEmpty(version) || version.Count != 1)
                return null;

            return ApiVersion.TryParse(version[0], out var apiVersion) ? apiVersion : null;
        }
    }
}
