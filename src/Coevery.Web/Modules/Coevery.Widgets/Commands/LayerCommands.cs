using System;
using Coevery.Commands;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Aspects;
using Coevery.Security;
using Coevery.Settings;
using Coevery.Widgets.Models;

namespace Coevery.Widgets.Commands {
    public class LayerCommands : DefaultCoeveryCommandHandler {
        private readonly IContentManager _contentManager;
        private readonly ISiteService _siteService;
        private readonly IMembershipService _membershipService;

        public LayerCommands(IContentManager contentManager, ISiteService siteService, IMembershipService membershipService) {
            _contentManager = contentManager;
            _siteService = siteService;
            _membershipService = membershipService;
        }

        [CoeverySwitch]
        public string LayerRule { get; set; }

        [CoeverySwitch]
        public string Description { get; set; }

        [CoeverySwitch]
        public string Owner { get; set; }

        [CommandName("layer create")]
        [CommandHelp("layer create <name> /LayerRule:<rule> [/Description:<description>] [/Owner:<owner>]\r\n\t" + "Creates a new layer")]
        [CoeverySwitches("LayerRule,Description,Owner")]
        public void Create(string name) {
            Context.Output.WriteLine(T("Creating Layer {0}", name));

            IContent layer = _contentManager.Create<LayerPart>("Layer", t => {
                                                                            t.Record.Name = name; 
                                                                            t.Record.LayerRule = LayerRule;
                                                                            t.Record.Description = Description ?? String.Empty;
                                                                        });

            _contentManager.Publish(layer.ContentItem);
            if (String.IsNullOrEmpty(Owner)) {
                Owner = _siteService.GetSiteSettings().SuperUser;
            }
            var owner = _membershipService.GetUser(Owner);
            layer.As<ICommonPart>().Owner = owner;

            Context.Output.WriteLine(T("Layer created successfully.").Text);
        }
    }
}