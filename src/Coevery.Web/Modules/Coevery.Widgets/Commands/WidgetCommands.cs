using System;
using System.Linq;
using Coevery.Commands;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Aspects;
using Coevery.Core.Common.Models;
using Coevery.Core.Navigation.Models;
using Coevery.Core.Navigation.Services;
using Coevery.Security;
using Coevery.Settings;
using Coevery.Widgets.Models;
using Coevery.Widgets.Services;

namespace Coevery.Widgets.Commands {
    public class WidgetCommands : DefaultCoeveryCommandHandler {
        private readonly IWidgetsService _widgetsService;
        private readonly ISiteService _siteService;
        private readonly IMembershipService _membershipService;
        private readonly IMenuService _menuService;
        private const string LoremIpsum = "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur a nibh ut tortor dapibus vestibulum. Aliquam vel sem nibh. Suspendisse vel condimentum tellus.</p>";

        public WidgetCommands(
            IWidgetsService widgetsService, 
            ISiteService siteService, 
            IMembershipService membershipService,
            IMenuService menuService) {
            _widgetsService = widgetsService;
            _siteService = siteService;
            _membershipService = membershipService;
            _menuService = menuService;

            RenderTitle = true;
        }

        [CoeverySwitch]
        public string Title { get; set; }

        [CoeverySwitch]
        public string Name { get; set; }

        [CoeverySwitch]
        public bool RenderTitle { get; set; }

        [CoeverySwitch]
        public string Zone { get; set; }

        [CoeverySwitch]
        public string Position { get; set; }

        [CoeverySwitch]
        public string Layer { get; set; }

        [CoeverySwitch]
        public string Identity { get; set; }

        [CoeverySwitch]
        public string Owner { get; set; }

        [CoeverySwitch]
        public string Text { get; set; }

        [CoeverySwitch]
        public bool UseLoremIpsumText { get; set; }

        [CoeverySwitch]
        public bool Publish { get; set; }

        [CoeverySwitch]
        public string MenuName { get; set; }

        [CommandName("widget create")]
        [CommandHelp("widget create <type> /Title:<title> /Name:<name> /Zone:<zone> /Position:<position> /Layer:<layer> [/Identity:<identity>] [/RenderTitle:true|false] [/Owner:<owner>] [/Text:<text>] [/UseLoremIpsumText:true|false] [/MenuName:<name>]\r\n\t" + "Creates a new widget")]
        [CoeverySwitches("Title,Name,Zone,Position,Layer,Identity,Owner,Text,UseLoremIpsumText,MenuName,RenderTitle")]
        public void Create(string type) {
            var widgetTypeNames = _widgetsService.GetWidgetTypeNames();
            if (!widgetTypeNames.Contains(type)) {
                Context.Output.WriteLine(T("Creating widget failed : type {0} was not found. Supported widget types are: {1}.", 
                    type,
                    widgetTypeNames.Aggregate(String.Empty, (current, widgetType) => current + " " + widgetType)));
                return;
            }

            var layer = GetLayer(Layer);
            if (layer == null) {
                Context.Output.WriteLine(T("Creating widget failed : layer {0} was not found.", Layer));
                return;
            }

            var widget = _widgetsService.CreateWidget(layer.ContentItem.Id, type, T(Title).Text, Position, Zone);

            if (!String.IsNullOrWhiteSpace(Name)) {
                widget.Name = Name.Trim();
            }

            var text = String.Empty;
            if (widget.Has<BodyPart>()) {
                if (UseLoremIpsumText) {
                    text = T(LoremIpsum).Text;
                }
                else {
                    if (!String.IsNullOrEmpty(Text)) {
                        text = Text;
                    }
                }
                widget.As<BodyPart>().Text = text;
            }

            widget.RenderTitle = RenderTitle;

            if(widget.Has<MenuWidgetPart>() && !String.IsNullOrWhiteSpace(MenuName)) {
                var menu = _menuService.GetMenu(MenuName);
                
                if(menu != null) {
                    widget.RenderTitle = false;
                    widget.As<MenuWidgetPart>().Menu = menu.ContentItem.Record;
                }
            }

            if (String.IsNullOrEmpty(Owner)) {
                Owner = _siteService.GetSiteSettings().SuperUser;
            }
            var owner = _membershipService.GetUser(Owner);
            widget.As<ICommonPart>().Owner = owner;

            if (widget.Has<IdentityPart>() && !String.IsNullOrEmpty(Identity)) {
                widget.As<IdentityPart>().Identifier = Identity;
            }

            Context.Output.WriteLine(T("Widget created successfully.").Text);
        }

        private LayerPart GetLayer(string layer) {
            var layers = _widgetsService.GetLayers();
            return layers.FirstOrDefault(layerPart => String.Equals(layerPart.Name, layer, StringComparison.OrdinalIgnoreCase));
        }
    }
}