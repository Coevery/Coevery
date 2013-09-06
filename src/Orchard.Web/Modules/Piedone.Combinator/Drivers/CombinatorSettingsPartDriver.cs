using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Environment.Extensions;
using Piedone.Combinator.EventHandlers;
using Piedone.Combinator.Models;
using Piedone.Combinator.Services;

namespace Piedone.Combinator.Drivers
{
    [OrchardFeature("Piedone.Combinator")]
    public class CombinatorSettingsPartDriver : ContentPartDriver<CombinatorSettingsPart>
    {
        private readonly ICacheFileService _cacheFileService;
        private readonly ICombinatorEventHandler _combinatorEventHandler;

        protected override string Prefix
        {
            get { return "Combinator"; }
        }

        public CombinatorSettingsPartDriver(
            ICacheFileService cacheFileService,
            ICombinatorEventHandler combinatorEventHandler)
        {
            _cacheFileService = cacheFileService;
            _combinatorEventHandler = combinatorEventHandler;
        }

        // GET
        protected override DriverResult Editor(CombinatorSettingsPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_CombinatorSettings_SiteSettings",
                    () =>   shapeHelper.EditorTemplate(
                            TemplateName: "Parts.CombinatorSettings.SiteSettings",
                            Model: part,
                            Prefix: Prefix))
                   .OnGroup("Combinator");
        }

        // POST
        protected override DriverResult Editor(CombinatorSettingsPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            var formerSettings = new CombinatorSettingsPartRecord();
            formerSettings.CombinationExcludeRegex = part.CombinationExcludeRegex;
            formerSettings.CombineCdnResources = part.CombineCDNResources;
            formerSettings.MinifyResources = part.MinifyResources;
            formerSettings.MinificationExcludeRegex = part.MinificationExcludeRegex;
            formerSettings.EmbedCssImages = part.EmbedCssImages;
            formerSettings.EmbeddedImagesMaxSizeKB = part.EmbeddedImagesMaxSizeKB;
            formerSettings.EmbedCssImagesStylesheetExcludeRegex = part.EmbedCssImagesStylesheetExcludeRegex;
            formerSettings.ResourceSetRegexes = part.ResourceSetRegexes;

            updater.TryUpdateModel(part, Prefix, null, null);

            // Not emptying the cache would cause inconsistencies
            if (part.CombinationExcludeRegex != formerSettings.CombinationExcludeRegex
                || part.CombineCDNResources != formerSettings.CombineCdnResources
                || part.MinifyResources != formerSettings.MinifyResources
                || (part.MinifyResources && part.MinificationExcludeRegex != formerSettings.MinificationExcludeRegex)
                || part.EmbedCssImages != formerSettings.EmbedCssImages
                || (part.EmbedCssImages && part.EmbeddedImagesMaxSizeKB != formerSettings.EmbeddedImagesMaxSizeKB)
                || (part.EmbedCssImages && part.EmbedCssImagesStylesheetExcludeRegex != formerSettings.EmbedCssImagesStylesheetExcludeRegex)
                || (part.ResourceSetRegexes != formerSettings.ResourceSetRegexes))
            {
                _cacheFileService.Empty();
            }

            _combinatorEventHandler.ConfigurationChanged();

            return Editor(part, shapeHelper);
        }

        protected override void Exporting(CombinatorSettingsPart part, ExportContentContext context)
        {
            var element = context.Element(part.PartDefinition.Name);

            element.SetAttributeValue("CombinationExcludeRegex", part.CombinationExcludeRegex);
            element.SetAttributeValue("CombineCDNResources", part.CombineCDNResources);
            element.SetAttributeValue("EmbedCssImages", part.EmbedCssImages);
            element.SetAttributeValue("EmbedCssImagesStylesheetExcludeRegex", part.EmbedCssImagesStylesheetExcludeRegex);
            element.SetAttributeValue("EmbeddedImagesMaxSizeKB", part.EmbeddedImagesMaxSizeKB);
            element.SetAttributeValue("MinificationExcludeRegex", part.MinificationExcludeRegex);
            element.SetAttributeValue("MinifyResources", part.MinifyResources);
            element.SetAttributeValue("ResourceSetRegexes", part.ResourceSetRegexes);
            element.SetAttributeValue("EnableForAdmin", part.EnableForAdmin);
        }

        protected override void Importing(CombinatorSettingsPart part, ImportContentContext context)
        {
            var combinationExcludeRegex = context.Attribute(part.PartDefinition.Name, "CombinationExcludeRegex");
            if (combinationExcludeRegex != null) part.CombinationExcludeRegex = combinationExcludeRegex;

            var combineCDNResources = context.Attribute(part.PartDefinition.Name, "CombineCDNResources");
            if (combineCDNResources != null) part.CombineCDNResources = bool.Parse(combineCDNResources);

            var embedCssImages = context.Attribute(part.PartDefinition.Name, "EmbedCssImages");
            if (embedCssImages != null) part.EmbedCssImages = bool.Parse(embedCssImages);

            var embedCssImagesStylesheetExcludeRegex = context.Attribute(part.PartDefinition.Name, "EmbedCssImagesStylesheetExcludeRegex");
            if (embedCssImagesStylesheetExcludeRegex != null) part.EmbedCssImagesStylesheetExcludeRegex = embedCssImagesStylesheetExcludeRegex;

            var embeddedImagesMaxSizeKB = context.Attribute(part.PartDefinition.Name, "EmbeddedImagesMaxSizeKB");
            if (embeddedImagesMaxSizeKB != null) part.EmbeddedImagesMaxSizeKB = int.Parse(embeddedImagesMaxSizeKB);

            var minificationExcludeRegex = context.Attribute(part.PartDefinition.Name, "MinificationExcludeRegex");
            if (minificationExcludeRegex != null) part.MinificationExcludeRegex = minificationExcludeRegex;

            var minifyResources = context.Attribute(part.PartDefinition.Name, "MinifyResources");
            if (minifyResources != null) part.MinifyResources = bool.Parse(minifyResources);

            var resourceSetRegexes = context.Attribute(part.PartDefinition.Name, "ResourceSetRegexes");
            if (resourceSetRegexes != null) part.ResourceSetRegexes = resourceSetRegexes;

            var enableForAdmin = context.Attribute(part.PartDefinition.Name, "EnableForAdmin");
            if (enableForAdmin != null) part.EnableForAdmin = bool.Parse(enableForAdmin);
        }
    }
}