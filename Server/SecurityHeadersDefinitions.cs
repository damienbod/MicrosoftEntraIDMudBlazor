﻿namespace MicrosoftEntraIdMudBlazor.Server;

public static class SecurityHeadersDefinitions
{
    public static HeaderPolicyCollection GetHeaderPolicyCollection(bool isDev, string? idpHost)
    {
        ArgumentNullException.ThrowIfNull(idpHost);

        var policy = new HeaderPolicyCollection()
            .AddFrameOptionsDeny()
            .AddContentTypeOptionsNoSniff()
            .AddReferrerPolicyStrictOriginWhenCrossOrigin()
            .AddCrossOriginOpenerPolicy(builder => builder.SameOrigin())
            .AddCrossOriginResourcePolicy(builder => builder.SameOrigin())
            .AddCrossOriginEmbedderPolicy(builder => builder.RequireCorp()) // remove for dev if using hot reload
            .AddContentSecurityPolicy(builder =>
            {
                builder.AddObjectSrc().None();
                builder.AddBlockAllMixedContent();
                builder.AddImgSrc().Self().From("data:");
                builder.AddFormAction().Self().From(idpHost);
                builder.AddFontSrc().Self();
                builder.AddBaseUri().Self();
                builder.AddFrameAncestors().None();

                builder.AddStyleSrc()
                    .UnsafeInline() // due to Mudblazor
                    .Self();

                builder.AddScriptSrc()
                   .WithNonce()
                   .UnsafeEval() // due to Blazor WASM
                   .StrictDynamic()
                   .UnsafeInline(); // only a fallback for older browsers when the nonce is used 

                // disable script and style CSP protection if using Blazor hot reload
                // if using hot reload, DO NOT deploy with an insecure CSP
            })
            .RemoveServerHeader()
            .AddPermissionsPolicyWithDefaultSecureDirectives();

        if (!isDev)
        {
            // maxage = one year in seconds
            policy.AddStrictTransportSecurityMaxAgeIncludeSubDomains(maxAgeInSeconds: 60 * 60 * 24 * 365);
        }

        return policy;
    }
}
